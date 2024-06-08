using CommonPackage;

namespace InventoryService.Middleware
{
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PerformanceMonitorService _performanceMonitorService;

        public PerformanceMonitoringMiddleware(RequestDelegate next, PerformanceMonitorService performanceMonitorService)
        {
            _next = next;
            _performanceMonitorService = performanceMonitorService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Measure initial performance
            var (initialCpuUsage, initialRamAvailable) = _performanceMonitorService.MeasureInitialPerformance();

            // Process the request
            await _next(context);

            // Measure final performance
            var (finalCpuUsage, finalRamAvailable) = _performanceMonitorService.MeasureFinalPerformance();

            // Log the performance data to the console in a table format
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"| {"Endpoint",-30} | {context.Request.Path,-30} |");
            Console.WriteLine($"| {"HTTP Method",-30} | {context.Request.Method,-30} |");
            Console.WriteLine($"| {"Timestamp",-30} | {DateTime.UtcNow,-30} |");
            Console.WriteLine($"| {"Initial CPU Usage (%)",-30} | {initialCpuUsage,-30:F2} |");
            Console.WriteLine($"| {"Final CPU Usage (%)",-30} | {finalCpuUsage,-30:F2} |");
            Console.WriteLine($"| {"Initial Available RAM (MB)",-30} | {initialRamAvailable,-30:F2} |");
            Console.WriteLine($"| {"Final Available RAM (MB)",-30} | {finalRamAvailable,-30:F2} |");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();
        }
    }
}