using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using AuthMe.NET.Models;

namespace AuthMe.NET.Services
{
    /// <summary>
    /// Generates unique hardware IDs for device binding
    /// </summary>
    public static class HardwareIdGenerator
    {
        /// <summary>
        /// Generates a hardware ID using the specified method
        /// </summary>
        /// <param name="method">Hardware ID generation method</param>
        /// <returns>Unique hardware ID string</returns>
        public static string Generate(HwidMethod method = HwidMethod.Comprehensive)
        {
            try
            {
                return method switch
                {
                    HwidMethod.Simple => GenerateSimple(),
                    HwidMethod.MacAddress => GetMacAddress(),
                    HwidMethod.SystemUuid => GetSystemUuid(),
                    HwidMethod.Comprehensive => GenerateComprehensive(),
                    _ => GenerateComprehensive()
                };
            }
            catch
            {
                // Fallback to basic system info if hardware detection fails
                return GenerateFallback();
            }
        }

        /// <summary>
        /// Detects if the current system is running in a virtual machine
        /// </summary>
        /// <returns>True if VM detected, false otherwise</returns>
        public static bool IsVirtualMachine()
        {
            try
            {
#if WINDOWS
                return IsVirtualMachineWindows();
#else
                return IsVirtualMachineUnix();
#endif
            }
            catch
            {
                // If detection fails, assume not a VM for safety
                return false;
            }
        }

        /// <summary>
        /// Gets detailed hardware information for debugging
        /// </summary>
        /// <returns>Hardware information dictionary</returns>
        public static System.Collections.Generic.Dictionary<string, string> GetHardwareInfo()
        {
            var info = new System.Collections.Generic.Dictionary<string, string>();

            try
            {
                info["System"] = Environment.OSVersion.ToString();
                info["Machine"] = Environment.MachineName;
                info["ProcessorCount"] = Environment.ProcessorCount.ToString();
                info["MacAddress"] = GetMacAddress();
                info["IsVirtualMachine"] = IsVirtualMachine().ToString();
#if WINDOWS
                info["CpuInfo"] = GetCpuInfo();
                info["MotherboardSerial"] = GetMotherboardSerial();
                info["SystemUuid"] = GetSystemUuid();
#else
                info["CpuInfo"] = "Not available on this platform";
                info["MotherboardSerial"] = "Not available on this platform";
                info["SystemUuid"] = "Not available on this platform";
#endif
                info["HardwareId"] = Generate();
            }
            catch (Exception ex)
            {
                info["Error"] = ex.Message;
            }

            return info;
        }

        private static string GenerateSimple()
        {
            var components = new[]
            {
                Environment.OSVersion.Platform.ToString(),
                Environment.MachineName,
                GetMacAddress()
            };

            return CreateHash(components);
        }

        private static string GenerateComprehensive()
        {
            var components = new[]
            {
                Environment.OSVersion.Platform.ToString(),
                Environment.MachineName,
                Environment.ProcessorCount.ToString(),
                GetMacAddress(),
#if WINDOWS
                GetCpuInfo(),
                GetMotherboardSerial(),
                GetSystemUuid()
#else
                "cpu-unknown",
                "mb-unknown",
                "uuid-unknown"
#endif
            };

            return CreateHash(components.Where(c => !string.IsNullOrEmpty(c) && c != "unknown"));
        }

        private static string GenerateFallback()
        {
            var components = new[]
            {
                Environment.OSVersion.ToString(),
                Environment.MachineName,
                Environment.ProcessorCount.ToString(),
                Environment.UserName
            };

            return CreateHash(components);
        }

        private static string GetMacAddress()
        {
            try
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                          nic.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                return networkInterface?.GetPhysicalAddress().ToString() ?? "unknown";
            }
            catch
            {
                return "unknown";
            }
        }

        private static string GetCpuInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                using var collection = searcher.Get();
                var cpu = collection.Cast<ManagementObject>().FirstOrDefault();
                return cpu?["Name"]?.ToString()?.Trim() ?? "unknown";
            }
            catch
            {
                return "unknown";
            }
        }

        private static string GetMotherboardSerial()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                using var collection = searcher.Get();
                var board = collection.Cast<ManagementObject>().FirstOrDefault();
                var serial = board?["SerialNumber"]?.ToString()?.Trim();
                return string.IsNullOrEmpty(serial) || serial == "To be filled by O.E.M." ? "unknown" : serial;
            }
            catch
            {
                return "unknown";
            }
        }

        private static string GetSystemUuid()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
                using var collection = searcher.Get();
                var system = collection.Cast<ManagementObject>().FirstOrDefault();
                return system?["UUID"]?.ToString()?.Trim() ?? "unknown";
            }
            catch
            {
                return "unknown";
            }
        }

#if WINDOWS
        private static bool IsVirtualMachineWindows()
        {
            try
            {
                // Check multiple VM indicators
                return CheckVMRegistry() ||
                       CheckVMProcesses() ||
                       CheckVMHardware() ||
                       CheckVMServices();
            }
            catch
            {
                return false;
            }
        }

        private static bool CheckVMRegistry()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Disk\Enum");
                if (key != null)
                {
                    var diskEnum = key.GetValue("0")?.ToString()?.ToLower();
                    if (!string.IsNullOrEmpty(diskEnum))
                    {
                        var vmIndicators = new[] { "vmware", "vbox", "qemu", "virtual", "xen" };
                        if (vmIndicators.Any(indicator => diskEnum.Contains(indicator)))
                            return true;
                    }
                }

                // Check BIOS
                using var biosKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
                if (biosKey != null)
                {
                    var biosVersion = biosKey.GetValue("BIOSVersion")?.ToString()?.ToLower();
                    var systemManufacturer = biosKey.GetValue("SystemManufacturer")?.ToString()?.ToLower();

                    var vmBiosIndicators = new[] { "vmware", "virtualbox", "qemu", "xen", "microsoft corporation", "innotek" };
                    if (!string.IsNullOrEmpty(biosVersion) && vmBiosIndicators.Any(indicator => biosVersion.Contains(indicator)))
                        return true;
                    if (!string.IsNullOrEmpty(systemManufacturer) && vmBiosIndicators.Any(indicator => systemManufacturer.Contains(indicator)))
                        return true;
                }
            }
            catch { }
            return false;
        }

        private static bool CheckVMProcesses()
        {
            try
            {
                var vmProcesses = new[] { "vmtoolsd", "vmwaretray", "vmwareuser", "vboxservice", "vboxtray", "xenservice" };
                var processes = System.Diagnostics.Process.GetProcesses();
                return processes.Any(p => vmProcesses.Any(vm => p.ProcessName.ToLower().Contains(vm)));
            }
            catch { }
            return false;
        }

        private static bool CheckVMHardware()
        {
            try
            {
                // Check computer system
                using var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem");
                using var collection = searcher.Get();
                foreach (var obj in collection.Cast<ManagementObject>())
                {
                    var manufacturer = obj["Manufacturer"]?.ToString()?.ToLower();
                    var model = obj["Model"]?.ToString()?.ToLower();

                    var vmIndicators = new[] { "vmware", "virtualbox", "qemu", "xen", "microsoft corporation", "innotek", "parallels", "virtual" };
                    if (!string.IsNullOrEmpty(manufacturer) && vmIndicators.Any(indicator => manufacturer.Contains(indicator)))
                        return true;
                    if (!string.IsNullOrEmpty(model) && vmIndicators.Any(indicator => model.Contains(indicator)))
                        return true;
                }

                // Check BIOS
                using var biosSearcher = new ManagementObjectSearcher("SELECT SerialNumber, SMBIOSBIOSVersion FROM Win32_BIOS");
                using var biosCollection = biosSearcher.Get();
                foreach (var obj in biosCollection.Cast<ManagementObject>())
                {
                    var serialNumber = obj["SerialNumber"]?.ToString()?.ToLower();
                    var biosVersion = obj["SMBIOSBIOSVersion"]?.ToString()?.ToLower();

                    var vmIndicators = new[] { "vmware", "virtualbox", "qemu", "xen", "innotek", "parallels" };
                    if (!string.IsNullOrEmpty(serialNumber) && vmIndicators.Any(indicator => serialNumber.Contains(indicator)))
                        return true;
                    if (!string.IsNullOrEmpty(biosVersion) && vmIndicators.Any(indicator => biosVersion.Contains(indicator)))
                        return true;
                }
            }
            catch { }
            return false;
        }

        private static bool CheckVMServices()
        {
            try
            {
                var vmServices = new[] { "vmtools", "vmhgfs", "vmci", "vboxservice", "vboxsf", "xenservice" };
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Service");
                using var collection = searcher.Get();
                foreach (var obj in collection.Cast<ManagementObject>())
                {
                    var serviceName = obj["Name"]?.ToString()?.ToLower();
                    if (!string.IsNullOrEmpty(serviceName) && vmServices.Any(vm => serviceName.Contains(vm)))
                        return true;
                }
            }
            catch { }
            return false;
        }
#else
        private static bool IsVirtualMachineUnix()
        {
            try
            {
                // Check for common VM indicators on Unix systems
                var vmFiles = new[]
                {
                    "/proc/scsi/scsi",
                    "/proc/cpuinfo",
                    "/sys/class/dmi/id/product_name",
                    "/sys/class/dmi/id/sys_vendor"
                };

                foreach (var file in vmFiles)
                {
                    if (System.IO.File.Exists(file))
                    {
                        var content = System.IO.File.ReadAllText(file).ToLower();
                        var vmIndicators = new[] { "vmware", "virtualbox", "qemu", "xen", "kvm", "hyperv", "parallels" };
                        if (vmIndicators.Any(indicator => content.Contains(indicator)))
                            return true;
                    }
                }

                // Check running processes
                try
                {
                    var processInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ps",
                        Arguments = "aux",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = System.Diagnostics.Process.Start(processInfo);
                    if (process != null)
                    {
                        var output = process.StandardOutput.ReadToEnd().ToLower();
                        var vmProcesses = new[] { "vmtoolsd", "vmware", "vboxservice", "qemu", "xen" };
                        if (vmProcesses.Any(vm => output.Contains(vm)))
                            return true;
                    }
                }
                catch { }
            }
            catch { }
            return false;
        }
#endif

        private static string CreateHash(System.Collections.Generic.IEnumerable<string> components)
        {
            var combined = string.Join("|", components.Where(c => !string.IsNullOrEmpty(c)));
            
            if (string.IsNullOrEmpty(combined))
            {
                combined = Environment.MachineName + "|" + Environment.OSVersion;
            }

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));

            // Convert to hex string (compatible with older .NET versions)
#if NET5_0_OR_GREATER
            var hash = Convert.ToHexString(hashBytes);
            return hash[..Math.Min(32, hash.Length)];
#else
            var hash = BitConverter.ToString(hashBytes).Replace("-", "");
            return hash.Substring(0, Math.Min(32, hash.Length));
#endif
        }
    }
}
