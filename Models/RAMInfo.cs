using System;
using System.Linq;
using System.Management;
using System.Timers;

namespace benchmark_sofware.Models
{
    internal class RAMInfo
    {
        private System.Timers.Timer _timer;
        public event Action<uint> OnRAMDataUpdated;
        private bool active;

        #region RAM VARIABLES
        public uint NumberOfSlots = 0;
        public uint TotalMemory = 0;
        public uint UsableMemory = 0;
        public uint FreeMemory = 0;
        public uint UsedMemory = 0;
        public uint Usage = 0;
        public uint ReservedMemory = 0;
        public RAMSlot[] Slots;

        public class RAMSlot
        {
            public string Manufacturer = "Unknown";
            public uint Capacity = 0;
            public uint Speed = 0;
        }

        #endregion

        #region RAM CONSTRUCTOR
        public RAMInfo(bool active)
        {
            this.active = active;   
            RetrieveTotalMemory();  
            RetrieveNumberOfSlots();
            RetrieveUsedFreeReservedMemory();
            RetrieveRAMSlotDetails();

            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += UpdateRAMUsage;
            _timer.Start();
        }

        private void UpdateRAMUsage(object? sender, ElapsedEventArgs e)
        {
            uint newUsage = RetrieveRAMUsage();

            if (Usage != newUsage)
            {
                OnRAMDataUpdated?.Invoke(newUsage);
                Usage = newUsage;
            }
        }

        #endregion

        #region WMI AND METHODS
        private uint RetrieveRAMUsage()
        {
            uint totalVisibleMemory = 0;
            uint freePhysicalMemory = 0;

            ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject obj in osClass.GetInstances())
            {
                totalVisibleMemory = (uint)(Convert.ToUInt64(obj["TotalVisibleMemorySize"]) / 1024);
                freePhysicalMemory = (uint)(Convert.ToUInt64(obj["FreePhysicalMemory"]) / 1024);
            }

            uint usedMemory = totalVisibleMemory - freePhysicalMemory;
            return (uint)((double)usedMemory / totalVisibleMemory * 100);
        }

        private void RetrieveNumberOfSlots()
        {
            try
            {
                ManagementClass managementClass = new ManagementClass("Win32_PhysicalMemoryArray");
                var inst = managementClass.GetInstances().Cast<ManagementObject>().ToList();

                foreach (var obj in inst)
                {
                    NumberOfSlots = Convert.ToUInt32(obj["MemoryDevices"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving number of slots: {ex.Message}");
            }
        }

        public void RetrieveUsedFreeReservedMemory()
        {
            ulong usableRAM = 0;
            ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject obj in osClass.GetInstances())
            {
                UsableMemory = (uint)Convert.ToUInt64(obj["TotalVisibleMemorySize"]) / 1024;
                FreeMemory = (uint)Convert.ToUInt64(obj["FreePhysicalMemory"]) / 1024;
            }
            ReservedMemory = TotalMemory - UsableMemory;
        }

        private void RetrieveTotalMemory()
        {
            ManagementClass managementClass = new ManagementClass("Win32_PhysicalMemory");
            var inst = managementClass.GetInstances().Cast<ManagementObject>().ToList();

            TotalMemory = 0;
            foreach (var obj in inst)
            {
                TotalMemory += (uint)(Convert.ToUInt64(obj["Capacity"]) / (1024 * 1024));
            }
        }

        private void RetrieveRAMSlotDetails()
        {
            ManagementClass managementClass = new ManagementClass("Win32_PhysicalMemory");
            var inst = managementClass.GetInstances().Cast<ManagementObject>().ToList();

            Slots = new RAMSlot[NumberOfSlots];

            int slotIndex = 0;
            foreach (var obj in inst)
            {
                var slot = new RAMSlot
                {
                    Manufacturer = Convert.ToString(obj["Manufacturer"]),
                    Capacity = (uint)(Convert.ToUInt64(obj["Capacity"]) / (1024 * 1024)),
                    Speed = (uint)(Convert.ToUInt32(obj["Speed"]))
                };

                Slots[slotIndex] = slot;
                slotIndex++;
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

        #region PRINT METHOD
        public void printAll()
        {
            Console.WriteLine($"Number of Slots: {NumberOfSlots}");
            Console.WriteLine($"Total Capacity: {TotalMemory} MB");
            Console.WriteLine($"Reserved Memory: {ReservedMemory} MB");
            Console.WriteLine($"Usable Memory: {UsableMemory} MB");
            Console.WriteLine($"Usage: {Usage}%");

        }
        #endregion
    }
}
