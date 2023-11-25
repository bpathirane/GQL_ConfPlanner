using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;

namespace ConferencePlanner.GraphQL.EventListeners
{
    public class CustomExecutionEventListener : ExecutionDiagnosticEventListener
    {
        private readonly ILogger _logger;

        public CustomExecutionEventListener(ILogger<CustomExecutionEventListener> logger)
        {
            _logger = logger;
        }

        public override void RequestError(IRequestContext context, Exception exception)
        {
            _logger.LogError(exception, "An request error occurred!");
        }
    }
}