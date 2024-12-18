using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using HidSharp.Reports;
using LibreHardwareMonitor.Hardware;

namespace benchmark_sofware.Models
{
    internal class GPUInfo
    {
        private uint noOfGPUs = 0;
        private Computer computer;

        private System.Timers.Timer _timer;
        public event Action<uint, uint, uint, uint> OnGPUDataUpdated;
        public bool active;


        #region DEDICATED GPU VARIABLES

        public string NameDedicated = "Unknow";
        public string TypeDedicated = "Unknow";
        public uint TotalMemoryDedicated = 0;
        public uint UsedMemoryDedicated = 0;
        public uint TemperatureDedicated = 0;

        #endregion

        #region INTEGRATED GPU VARIABLES

        public string NameIntegrated = "Unknow";
        public string TypeIntegrated = "Unknow";
        public uint TotalMemoryIntegrated = 0;
        public uint UsedMemoryIntegrated = 0;
        public uint TemperatureIntegrated = 0;

        #endregion
        
        public GPUInfo(bool active)
        {
            this.active = active;
            computer = new Computer
            {
                IsGpuEnabled = true,
            };

            computer.Open();

            retreiveGPUInfo();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += UpdateGPUInfoPeriodically;
            _timer.Start();
        }
        private void UpdateGPUInfoPeriodically(object sender, ElapsedEventArgs e)
        {
            if(active)
                {            
                RetrieveDedicatedGPUTemperature();
                RetrieveDedicatedGPUUsage();

                RetrieveIntegratedGPUTemperature();
                RetrieveIntegratedGPUUsage();

                uint usageDedicated = (uint)(UsedMemoryDedicated * 100) / TotalMemoryDedicated;
                uint usageIntegrated = (uint)(UsedMemoryIntegrated * 100) / TotalMemoryIntegrated;


                OnGPUDataUpdated?.Invoke(
                    usageDedicated,
                    TemperatureDedicated,
                    usageIntegrated,
                    TemperatureIntegrated
                );
                }
        }

        #region DEDICATED GPU RETRIEVE FUNCTIONS
        public void  RetrieveDedicatedGPUUsage()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                if (!string.IsNullOrEmpty(NameDedicated) && hardware.Name.Contains(NameDedicated))
                {
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("GPU Memory Used"))
                        {
                            UsedMemoryDedicated = (uint)(sensor.Value ?? 0);
                            break;
                        }
                    }
                }
            }
        }
        public void RetrieveDedicatedGPUTemperature()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                if (!string.IsNullOrEmpty(NameDedicated) && hardware.Name.Contains(NameDedicated))
                {
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            TemperatureDedicated = (uint)(sensor.Value ?? 0);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region INTEGRATED GPU RETRIEVE FUNCTIONS
        public void RetrieveIntegratedGPUUsage()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                if (!string.IsNullOrEmpty(NameIntegrated) && hardware.Name.Contains(NameIntegrated))
                {
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("GPU Memory Used"))
                        {
                            UsedMemoryIntegrated = (uint)(sensor.Value ?? 0);
                        }
                    }
                }
            }
        }

        public void RetrieveIntegratedGPUTemperature()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                if (!string.IsNullOrEmpty(NameIntegrated) && hardware.Name.Contains(NameIntegrated))
                {
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            TemperatureIntegrated = (uint)(sensor.Value ?? 0);
                        }
                    }
                }
            }
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

        #region GPU RETRIEVE FUNCTIONS

        public void retreiveGPUInfo()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                 if (hardware.HardwareType == HardwareType.GpuAmd ||
                    hardware.HardwareType == HardwareType.GpuNvidia ||
                    hardware.HardwareType == HardwareType.GpuIntel)
                {
                    hardware.Update();

                    bool isIntegrated = false;
                    uint totalMemory = 0;
                   
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.SmallData && sensor.Name.Contains("GPU Memory Total"))
                        {
                            totalMemory = (uint)(sensor.Value ?? 0);
                        }
                    }

                    if (hardware.HardwareType == HardwareType.GpuIntel || totalMemory < 1024)
                    {
                        isIntegrated = true;
                    }

                    if (isIntegrated)
                    {
                        NameIntegrated = hardware.Name;
                        TotalMemoryIntegrated = totalMemory;
                    }
                    else
                    {
                        NameDedicated = hardware.Name;
                        TotalMemoryDedicated = totalMemory;
                    }
                }
            }

        }
        #endregion

        public void printAll()
        {
            Console.WriteLine("\nDedicated GPU:");
            Console.WriteLine($"  Name: {NameDedicated}");
            Console.WriteLine($"  Type: {TypeDedicated}");
            Console.WriteLine($"  Total Memory: {TotalMemoryDedicated} MB");
            Console.WriteLine($"  Used Memory: {UsedMemoryDedicated} MB");
            Console.WriteLine($"  Temperature: {TemperatureDedicated} °C");

            Console.WriteLine("\nIntegrated GPU:");
            Console.WriteLine($"  Name: {NameIntegrated}");
            Console.WriteLine($"  Type: {TypeIntegrated}");
            Console.WriteLine($"  Total Memory: {TotalMemoryIntegrated} MB");
            Console.WriteLine($"  Used Memory: {UsedMemoryIntegrated} MB");
            Console.WriteLine($"  Temperature: {TemperatureIntegrated} °C");
        }

    }
}
