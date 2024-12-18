using System;
using System.Linq;
using System.Management;

namespace benchmark_software.Models
{
    internal class DiskInfo
    {
        public uint TotalSpace = 0;
        public uint TotalFreeSpace = 0;
        public uint TotalUsedSpace = 0;

        public class DiskSlot
        {
            public string Id { get; set; }
            public uint Size { get; set; } 
            public uint FreeSpace { get; set; } 
            public uint UsedSpace { get; set; } 
        }

        public DiskSlot[] Disks;

        public DiskInfo()
        {
            RetrieveDiskInfo();
        }

        private void RetrieveDiskInfo()
        {
            ManagementClass logicalDiskClass = new ManagementClass("Win32_LogicalDisk");
            var instances = logicalDiskClass.GetInstances().Cast<ManagementObject>().ToList();

            Disks = new DiskSlot[instances.Count];

            int diskIndex = 0;
            foreach (var obj in instances)
            {
                if (obj["Size"] == null || obj["FreeSpace"] == null) continue;

                uint size = (uint)(Convert.ToUInt64(obj["Size"]) / (1024 * 1024 * 1024));
                uint freeSpace = (uint)(Convert.ToUInt64(obj["FreeSpace"]) / (1024 * 1024 * 1024));
                uint usedSpace = size - freeSpace;

                TotalSpace += size;
                TotalFreeSpace += freeSpace;
                TotalUsedSpace += usedSpace;

                var disk = new DiskSlot
                {
                    Id = Convert.ToString(obj["DeviceID"]),
                    Size = size,
                    FreeSpace = freeSpace,
                    UsedSpace = usedSpace
                };

                Disks[diskIndex] = disk;
                diskIndex++;
            }
        }

        private void PrintAll()
        {
            Console.WriteLine("Disk Information Summary:");
            Console.WriteLine("--------------------------");

            for (int i = 0; i < Disks.Length; i++)
            {
                var disk = Disks[i];
                if (disk == null) continue;

                Console.WriteLine($"Disk {i + 1} (ID: {disk.Id}):");
                Console.WriteLine($"  Total Size: {disk.Size} GB");
                Console.WriteLine($"  Used Space: {disk.UsedSpace} GB");
                Console.WriteLine($"  Free Space: {disk.FreeSpace} GB");
                Console.WriteLine();
            }

            Console.WriteLine("Total Disk Summary:");
            Console.WriteLine($"  Total Space: {TotalSpace} GB");
            Console.WriteLine($"  Total Used Space: {TotalUsedSpace} GB");
            Console.WriteLine($"  Total Free Space: {TotalFreeSpace} GB");
            Console.WriteLine("--------------------------");
        }
    }
}      
