using Elders.Cronus.Middleware;
using static Elders.Cronus.MessageProcessingMiddleware.MessageHandlerMiddleware;

namespace Elders.Cronus.Hystrix
{
    public class HystrixMiddleware : Middleware<HandleContext>
    {
        Middleware<HandleContext> actualHandle;
        readonly HystrixCommandFactory commandFactory;

        public HystrixMiddleware(Middleware<HandleContext> actualHandle, HystrixCommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
            this.actualHandle = actualHandle;
        }

        protected override void Run(Execution<HandleContext> execution)
        {
            var settings = commandFactory.BuildCommandSettings(execution.Context.HandlerInstance.GetType());
            var cmd = new HandleMessageHystrixCommand(actualHandle, execution, settings);
            cmd.Execute();
        }
    }
}
