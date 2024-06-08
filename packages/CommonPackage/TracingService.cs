using OpenTelemetry.Trace;

namespace CommonPackage;

    public class TracingService : ITracingService
    {
        private readonly Tracer _tracer;

        public TracingService(Tracer tracer)
        {
            _tracer = tracer;
        }

        public IDisposable StartActiveSpan(string operationName)
        {
            return _tracer.StartActiveSpan(operationName);
        }
    }
