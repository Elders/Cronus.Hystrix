using System;
using Elders.Cronus.MessageProcessingMiddleware;
using Elders.Cronus.Middleware;
using Netflix.Hystrix;

namespace Elders.Cronus.Hystrix
{
    public interface IHystrixCommandFactory
    {
        HystrixCommandSetter BuildCommandSettings(Type type);
    }

    public class HystrixCommandFactory : IHystrixCommandFactory
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
        public static Middleware<HandleContext> UseHystrix(this Middleware<HandleContext> self, Func<IHystrixCommandFactory> settings = null)
        {
            var commandSettings = settings?.Invoke() ?? new HystrixCommandFactory();
            return new HystrixMiddleware(self, commandSettings);
        }
    }
}
