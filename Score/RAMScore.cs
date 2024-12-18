using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace benchmark_sofware.Score
{
    internal class RAMScore
    {
        public event Action<uint, string> OnRAMProgressBar1;
        public event Action<uint, string> OnRAMProgressBar2;
        public event Action<uint, uint, uint, uint, uint> OnRAMScore;

        public double latency = 0;
        public double read_bandwidth = 0;
        public double write_bandwidth = 0;

        private double ref_latency = 2.10;
        private double ref_read_bandwidth = 21531.08;
        private double ref_write_bandwidth = 27066.66;

        private uint progressTest1 = 0;
        private uint progressTest2 = 0;

        private const string defaultTextTest1 = "RAM memory latency.";
        private const string defaultTextTest2 = "RAM memory read/write bandwidth.";
        public RAMScore() { 
            
        }

        public async Task startTest1()
        {
            ulong size = 512L * 1024 * 1024; // 512 MB
            ulong numOfAccesses = 1000000;

            await UpdateTest1(25, "Generating array");
            IntPtr arrPtr = generateRandomArrayTest1(size);

            await UpdateTest1(25, "Generating random indices");
            IntPtr indicesPtr = generateRandomIndicesTest1(numOfAccesses, size);

            await UpdateTest1(25, "Calculating latency");
            latency = measureLatency(arrPtr, indicesPtr, numOfAccesses);

            await UpdateTest1(25, "Freeing memory");
            freeMemoryTest1(arrPtr, indicesPtr);

            await UpdateTest1(0, defaultTextTest1);

        }

        public async Task startTest2() {
            ulong size = 512L * 1024 * 1024; // 512 MB test size

            await UpdateTest2(25, "Generating array");
            IntPtr arrayPtr = generateRandomArrayTest2(size);

            await UpdateTest2(25, "Calculating read bandwidth");
            read_bandwidth = measureReadBandwidth(arrayPtr, size);

            await UpdateTest2(25, "Calculating write bandwidth");
            write_bandwidth = measureWriteBandwidth(arrayPtr, size);

            await UpdateTest2(25, "Freeing memory");
            freeMemoryTest2(arrayPtr);

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
            if (OnRAMProgressBar1 != null)
            {
                foreach (var handler in OnRAMProgressBar1.GetInvocationList())
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
            if (OnRAMProgressBar2 != null)
            {
                foreach (var handler in OnRAMProgressBar2.GetInvocationList())
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
            uint scoreTest1 = (uint)((1000 * latency) / ref_latency);
            uint scoreWrite = (uint)((1000 * write_bandwidth) / ref_write_bandwidth);
            uint scoreRead = (uint)((1000 *  read_bandwidth) / ref_read_bandwidth);
            uint scoreTest2 = (uint)((0.5 * write_bandwidth) + (0.5 * read_bandwidth));
            return (uint)((scoreTest1 * 0.5) + (scoreTest2 * 0.5));
        }

        #endregion

        [DllImport(@"..\..\..\Tests\ram-test-1.dll")]
        public static extern IntPtr generateRandomArrayTest1(ulong size);

        [DllImport(@"..\..\..\Tests\ram-test-1.dll")]
        public static extern IntPtr generateRandomIndicesTest1(ulong numOfAccesses, ulong size);

        [DllImport(@"..\..\..\Tests\ram-test-1.dll")]
        public static extern double measureLatency(IntPtr arr, IntPtr indices, ulong numOfAccesses);

        [DllImport(@"..\..\..\Tests\ram-test-1.dll")]
        public static extern void freeMemoryTest1(IntPtr arr, IntPtr indices);


        [DllImport(@"..\..\..\Tests\ram-test-2.dll")]
        public static extern IntPtr generateRandomArrayTest2(ulong size);
        [DllImport(@"..\..\..\Tests\ram-test-2.dll")]
        public static extern double measureReadBandwidth(IntPtr arr, ulong size);
        [DllImport(@"..\..\..\Tests\ram-test-2.dll")]
        public static extern double measureWriteBandwidth(IntPtr arr, ulong size);
        [DllImport(@"..\..\..\Tests\ram-test-2.dll")]
        public static extern void freeMemoryTest2(IntPtr arr);
    }

}
