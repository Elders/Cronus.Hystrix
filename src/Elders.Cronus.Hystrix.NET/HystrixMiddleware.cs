using Elders.Cronus.MessageProcessing;
using Elders.Cronus.Middleware;

namespace Elders.Cronus.Hystrix
{
    public class HystrixMiddleware : Middleware<HandleContext>
    {
        Middleware<HandleContext> actualHandle;
        readonly IHystrixCommandFactory commandFactory;

        public HystrixMiddleware(Middleware<HandleContext> actualHandle, IHystrixCommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
            this.actualHandle = actualHandle;
        }

        protected override void Run(Execution<HandleContext> execution)
        {
            var settings = commandFactory.BuildCommandSettings(execution.Context.HandlerType);
            var cmd = new HandleMessageHystrixCommand(actualHandle, execution, settings);
            cmd.Execute();
        }
    }
}
