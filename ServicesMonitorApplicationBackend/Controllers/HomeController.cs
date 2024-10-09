using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServicesMonitorApplicationBackend.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ServicesMonitorApplicationBackend.Controllers
{
    [EnableCors("SpecificOriginPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult GetServicesInfo()
        {
            var serviceInfo = new ServiceInfo()
            {
                IPAddress = GetProcessInfo(ListCommand.IPAddress),
                RunningProcesses = GetProcessInfo(ListCommand.RunningProcesses),
                AvailableDiskSpace = GetProcessInfo(ListCommand.AvailableDiskSpace),
                TimeSincedLastBoot = GetProcessInfo(ListCommand.TimeSincedLastBoot)
            };

            return Ok(serviceInfo);
        }

        private OSCommand? GetOSCommand(ListCommand command)
        {
            if (command == ListCommand.IPAddress)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return new OSCommand() { FileName = "hostname", Arguments = "-I" };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new OSCommand() { FileName = "cmd.exe", Arguments = "/C ipconfig" };
                }
                return null;
            }
            else if (command == ListCommand.RunningProcesses) {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return new OSCommand() { FileName = "ps", Arguments = "-ax" };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new OSCommand() { FileName = "cmd.exe", Arguments = "/C tasklist" };
                }
                return null;
            }
            else if (command == ListCommand.AvailableDiskSpace)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return new OSCommand() { FileName = "df", Arguments = "-h" };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new OSCommand() { FileName = "cmd.exe", Arguments = "/C wmic logicaldisk get size,freespace,caption" };
                }
                return null;
            }
            else if (command == ListCommand.TimeSincedLastBoot)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return new OSCommand() { FileName = "uptime", Arguments = "-p" };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new OSCommand() { FileName = "cmd.exe", Arguments = "/C net stats workstation" };
                }
                return null;
            }
            return null;
        } 

        private string? GetProcessInfo(ListCommand command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = GetOSCommand(command)?.FileName,
                Arguments = GetOSCommand(command)?.Arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true

            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();

                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    return null;
                }

                return result;
            }

        }
    }                                                                     
}
