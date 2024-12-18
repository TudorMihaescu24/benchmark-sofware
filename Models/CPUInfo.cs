using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Management;
using System.Timers;

namespace benchmark_software.Models
{
    internal class CPUInfo
    {
        #region CPU VARIABLES
        private bool active;
        private PerformanceCounter CPUCounter;
        public string Name = "Unknown";
        public uint Threads = 0;
        public uint Cores = 0;
        public uint Frequency = 0;
        public uint L1Cache = 0;
        public uint L2Cache = 0;
        public uint L3Cache = 0;
        public string Architecture = "Unknown";
        public int Temperature = 0;
        public int Usage = 0;

        private int previousTemperature = -1;
        private int previousUsage = -1;
        private Computer computer;
        private System.Timers.Timer _timer;

        public event Action<int, int> OnCPUDataUpdated;
        #endregion

        #region CONSTRUCTOR

        public CPUInfo(bool active)
        {
            this.active = active;

            CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            computer = new Computer
            {
                IsCpuEnabled = true
            };

            computer.Open();
            RetrieveCPUInfo();
            RetrieveCPUCache();

            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += UpdateCPUInfoPeriodically;
            _timer.Start();
        }

        private void UpdateCPUInfoPeriodically(object sender, ElapsedEventArgs e)
        {
            if (active)
            {
                int newTemperature = RetrieveCPUTemperature();
                RetrieveCPUUsage();
                OnCPUDataUpdated?.Invoke(Usage, Temperature);
            }
        }



        #endregion

        #region WMI AND METHODS

        private void RetrieveCPUInfo()
        {
            ManagementClass processorClass = new ManagementClass("Win32_Processor");
            var processorInstance = processorClass.GetInstances().Cast<ManagementObject>().ToList();
            foreach (ManagementObject obj in processorInstance)
            {
                Name = obj["Name"].ToString() ?? "Unknown";
                Threads = Convert.ToUInt32(obj["ThreadCount"]);
                Cores = Convert.ToUInt32(obj["NumberOfCores"]);
                Frequency = Convert.ToUInt32(obj["MaxClockSpeed"]);
                uint architectureNo = Convert.ToUInt32(obj["Architecture"]);

                Architecture = architectureNo switch
                {
                    0 => "x86",
                    1 => "MIPS",
                    2 => "Alpha",
                    3 => "PowerPC",
                    5 => "ARM",
                    6 => "ia64",
                    9 => "x64",
                    _ => "Unknown"
                };
            }
        }

        private void RetrieveCPUCache()
        {
            ManagementClass cacheClass = new ManagementClass("Win32_CacheMemory");
            var cacheInstances = cacheClass.GetInstances().Cast<ManagementObject>().ToList();

            foreach (ManagementObject obj in cacheInstances)
            {
                int level = Convert.ToInt32(obj["Level"]);
                uint cacheSize = Convert.ToUInt32(obj["MaxCacheSize"]);
                switch (level)
                {
                    case 3:
                        L1Cache = cacheSize;
                        break;

                    case 4:
                        L2Cache = cacheSize;
                        break;

                    case 5:
                        L3Cache = cacheSize;
                        break;
                }
            }
        }

        public int RetrieveCPUTemperature()
        {
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            Temperature = (int)sensor.Value.GetValueOrDefault();
                            return Temperature;
                        }
                    }
                }
            }

            return Temperature;
        }


        public void RetrieveCPUUsage()
        {
            float usage = CPUCounter.NextValue();
            System.Threading.Thread.Sleep(1000); 
            Usage = (int)usage;
        }
        #endregion

        public void StartUpdates()
        {
            active = true;
        }
        public void StopUpdates()
        {
            active = false;
        }

        #region PRINT ALL DETAILS

        public void printAll()
        {
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Threads: {Threads}");
            Console.WriteLine($"Cores: {Cores}");
            Console.WriteLine($"Frequency: {Frequency}");
            Console.WriteLine($"L1 Cache: {L1Cache}");
            Console.WriteLine($"L2 Cache: {L2Cache}");
            Console.WriteLine($"L3 Cache: {L3Cache}");
            Console.WriteLine($"Architecture: {Architecture}");
            Console.WriteLine($"Temperature: {Temperature}°C");
            Console.WriteLine($"Usage: {Usage}%");
        }

        public void Close()
        {
            computer.Close();
        }

        #endregion
    }
}
