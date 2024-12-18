using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace benchmark_sofware.Score
{


    internal class DiskScore
    {
        public event Action<uint, string> OnDiskProgressBar1;
        public event Action<uint, string> OnDiskProgressBar2;
        public event Action<uint, uint, uint, uint, uint> OnDiskScore;

        
        public double read_bandwidth = 0;
        public double write_bandwidth = 0;
        public double read_iops = 0;
        public double write_iops = 0;

        private double ref_read_bandwidth = 598.90;
        private double ref_write_bandwidth = 1634.30;
        private double ref_read_iops = 849.30;
        private double ref_write_iops = 1631.25;

        private uint progressTest1 = 0;
        private uint progressTest2 = 0;

        private const string defaultTextTest1 = "Read/Write Bandwidth of the disk";
        private const string defaultTextTest2 = "Read/Write IOPS operations";

        public DiskScore() { }

        public async Task startTest1()
        {
            await UpdateTest1(50, "Calculating write speed of the disk.");
            write_bandwidth = calculateWriteSpeed();
            await UpdateTest1(50, "Calculating read speed of the disk.");
            read_bandwidth = calculateReadSpeed();
            await UpdateTest1(0, defaultTextTest1);
        }

        public async Task startTest2()
        {
            await UpdateTest2(33, "Calculating write IOPS of the disk.");
            write_iops = calculateWriteSpeed();
            await UpdateTest2(33, "Calculating read IOPS of the disk.");
            read_iops = calculateReadSpeed();
            await UpdateTest2(34, "Removing created files");
            calculateReadSpeed();
            await UpdateTest2(0, defaultTextTest2);
        }

        public void initTest()
        {
            progressTest1 = 0;
            progressTest2 = 0;
            UpdateProgressTest1(0, defaultTextTest1);
            UpdateProgressTest2(0, defaultTextTest2);
        }

        private async Task UpdateTest1(uint step, string text)
        {
            progressTest1 += step;
            UpdateProgressTest1(progressTest1, text);
            await Task.Delay(500);
        }

        private async Task UpdateTest2(uint step, string text)
        {
            progressTest2 += step;
            UpdateProgressTest2(progressTest2, text);
            await Task.Delay(500);
        }

        private void UpdateProgressTest1(uint progress, string text)
        {
            if (OnDiskProgressBar1 != null)
            {
                foreach (var handler in OnDiskProgressBar1.GetInvocationList())
                {
                    if (handler.Target is System.Windows.Threading.DispatcherObject dispatcherObject)
                    {
                        dispatcherObject.Dispatcher.Invoke(() => handler.DynamicInvoke(progress, text));
                    }
                    else
                    {
                        handler.DynamicInvoke(progress, text);
                    }
                }
            }
        }

        private void UpdateProgressTest2(uint progress, string text)
        {
            if (OnDiskProgressBar2 != null)
            {
                foreach (var handler in OnDiskProgressBar2.GetInvocationList())
                {
                    if (handler.Target is System.Windows.Threading.DispatcherObject dispatcherObject)
                    {
                        dispatcherObject.Dispatcher.Invoke(() => handler.DynamicInvoke(progress, text));
                    }
                    else
                    {
                        handler.DynamicInvoke(progress, text);
                    }
                }
            }
        }

        #region CALCULATE SCORE
        public uint calculateScore()
        {
            uint scoreWrite = (uint)((1000 * write_bandwidth) / ref_write_bandwidth);
            uint scoreRead = (uint)((1000 * read_bandwidth) / ref_read_bandwidth);
            uint scoreTest1 = (uint)((0.5 * scoreRead) + (0.5 * scoreWrite));
            uint scoreWriteIOPS = (uint)((1000 * write_iops) / ref_write_iops); ;
            uint scoreReadIOPS = (uint)((1000 * write_iops) / ref_write_iops);
            uint scoreTest2 = (uint)((0.5 * scoreWriteIOPS) + (0.5 * scoreReadIOPS));
            return (uint)((scoreTest1 * 0.5) + (scoreTest2 * 0.5));
        }
        #endregion

        #region DISK-TEST-1
        [DllImport(@"..\..\..\Tests\disk-test-1.dll")]
        public static extern double calculateWriteSpeed();
        [DllImport(@"..\..\..\Tests\disk-test-1.dll")]
        public static extern double calculateReadSpeed();
        #endregion

        #region DISK-TEST-2
        [DllImport(@"..\..\..\Tests\disk-test-2.dll")]
        public static extern double writeIOPSTest();

        [DllImport(@"..\..\..\Tests\disk-test-2.dll")]
        public static extern double readIOPSTest();
        
        [DllImport(@"..\..\..\Tests\disk-test-2.dll")]
        public static extern void cleanUpFiles();
        #endregion
        
    }
}
