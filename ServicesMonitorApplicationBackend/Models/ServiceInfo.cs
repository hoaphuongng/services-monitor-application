using System.Text.Json.Serialization;

namespace ServicesMonitorApplicationBackend.Models
{
    public class ServiceInfo
    {
        public string? IPAddress { get; set; }
        public string? RunningProcesses { get; set; }
        public string? AvailableDiskSpace {  get; set; }
        public string? TimeSincedLastBoot {  get; set; }
    }
}
