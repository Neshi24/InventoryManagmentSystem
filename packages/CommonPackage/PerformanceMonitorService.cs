using System.Diagnostics;

namespace CommonPackage
{
    public class PerformanceMonitorService
    {
        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter ramCounter;

        public PerformanceMonitorService()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public (float initialCpuUsage, float initialRamAvailable) MeasureInitialPerformance()
        {
            // Measure initial CPU and RAM usage
            float initialCpuUsage = cpuCounter.NextValue();
            float initialRamAvailable = ramCounter.NextValue();

            // Wait a moment to get a more accurate initial reading
            System.Threading.Thread.Sleep(1000);
            initialCpuUsage = cpuCounter.NextValue();

            return (initialCpuUsage, initialRamAvailable);
        }

        public (float finalCpuUsage, float finalRamAvailable) MeasureFinalPerformance()
        {
            // Measure final CPU and RAM usage
            float finalCpuUsage = cpuCounter.NextValue();
            float finalRamAvailable = ramCounter.NextValue();

            return (finalCpuUsage, finalRamAvailable);
        }
    }
}