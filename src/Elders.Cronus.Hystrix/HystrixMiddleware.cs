using System;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.Middleware;
using Elders.Cronus.Pipeline.Config;
using Netflix.Hystrix;
using static Elders.Cronus.MessageProcessingMiddleware.MessageHandlerMiddleware;

namespace Elders.Cronus.Hystrix
{
    public static class HystrixMiddlewareConfig
    {
        public static T UseHystrix<T>(this T self) where T : ISubscrptionMiddlewareSettings<IMessage>
        {
            self.ActualHandle = new HystrixMiddleware(self.ActualHandle);
            return self;
        }
    }

    public class HystrixMiddleware : Middleware<HandleContext>
    {
        Middleware<HandleContext> actualHandle;
        public HystrixMiddleware(Middleware<HandleContext> actualHandle)
        {
            this.actualHandle = actualHandle;
        }

        protected override void Run(Execution<HandleContext> execution)
        {
            var key = execution.Context.HandlerInstance.GetType().Name;
            var cfg = HystrixCommandSetter.WithGroupKey(key)
                .AndCommandKey(key)
                .AndCommandPropertiesDefaults(
                    new HystrixCommandPropertiesSetter()
                    .WithExecutionIsolationThreadTimeout(TimeSpan.FromSeconds(1.0))
                    .WithExecutionIsolationStrategy(ExecutionIsolationStrategy.Semaphore)
                    .WithExecutionIsolationThreadInterruptOnTimeout(true));

            var cmd = new HandleMessageHystrixCommand(actualHandle, execution, cfg);
            cmd.Execute();
        }
    }

    public class HandleMessageHystrixCommand : HystrixCommand<bool>
    {
        Middleware<HandleContext> actualHandle;
        Execution<HandleContext> context;

        public HandleMessageHystrixCommand(Middleware<HandleContext> actualHandle, Execution<HandleContext> context, HystrixCommandSetter hystrixCfg)
            : base(hystrixCfg)
        {
            this.actualHandle = actualHandle;
            this.context = context;
        }

        protected override bool Run()
        {
            actualHandle.Run(context.Context);
            return true;
        }
    }
}
