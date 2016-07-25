using Elders.Cronus.Middleware;
using Netflix.Hystrix;
using static Elders.Cronus.MessageProcessingMiddleware.MessageHandlerMiddleware;

namespace Elders.Cronus.Hystrix
{
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
