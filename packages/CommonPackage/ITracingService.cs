namespace CommonPackage;

public interface ITracingService
{
    IDisposable StartActiveSpan(string operationName);
}