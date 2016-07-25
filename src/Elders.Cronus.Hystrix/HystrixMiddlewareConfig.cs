using System;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.Pipeline.Config;
using Netflix.Hystrix;

namespace Elders.Cronus.Hystrix
{
    public class HystrixCommandFactory
    {
        public HystrixCommandSetter BuildCommandSettings(Type type)
        {
            var key = type.Name;

            return HystrixCommandSetter.WithGroupKey(key)
                .AndCommandKey(key)
                .AndCommandPropertiesDefaults(
                    new HystrixCommandPropertiesSetter()
                        .WithExecutionIsolationThreadTimeout(TimeSpan.FromSeconds(1.0))
                        .WithExecutionIsolationStrategy(ExecutionIsolationStrategy.Semaphore)
                        .WithExecutionIsolationThreadInterruptOnTimeout(true));
        }
    }

    public static class HystrixMiddlewareConfig
    {
        public static T UseHystrix<T>(this T self, Func<HystrixCommandFactory> settings = null) where T : ISubscrptionMiddlewareSettings<IMessage>
        {
            var commandSettings = settings?.Invoke() ?? new HystrixCommandFactory();
            self.ActualHandle = new HystrixMiddleware(self.ActualHandle, commandSettings);
            return self;
        }
    }
}
