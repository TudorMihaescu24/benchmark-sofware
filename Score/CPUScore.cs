using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace benchmark_sofware.Score
{
    internal class CPUScore
    {
        public event Action<uint, string> OnCPUProgressBar1;
        public event Action<uint, string> OnCPUProgressBar2;
        public event Action<uint, uint, uint, uint, uint> OnCPUScore;

        public long operations_second = 0;
        public uint time_single = 0;
        public uint time_multi = 0;

        private uint ref_operations_second = 749636288;
        private uint ref_time_single = 3998;
        private uint ref_time_multi = 617;

        private uint progressTest1 = 0;
        private uint progressTest2 = 0;

        private const string defaultTextTest1 = "Number of Floating Point Operations in a minute.";
        private const string defaultTextTest2 = "Complicated Computational Task Performance.";

        public CPUScore() {

        }


        #region PERFORMING TEST
        public async Task startTest1()
        {
            progressTest1 = 0;
            
            long average = 0;
            long numberOfTests = 5;

            for(int i = 0; i < numberOfTests; i++)
            {
                await UpdateTest1(20, $"Starting test {i}");
                average += PerformCPUTest1();
            }

            await UpdateTest1(0, defaultTextTest1);
            average = (long) average / numberOfTests;

            operations_second = average;
        }
        public async Task startTest2()
        {
            int n = 1000;
            
            await UpdateTest2(10, $"Generating matrix A of size {n} x {n}");
            IntPtr matrixA = generateRandomMatrix(n);
            
            await UpdateTest2(10, $"Generating matrix B of size {n} x {n}");
            IntPtr matrixB = generateRandomMatrix(n);
            
            await UpdateTest2(10, "Calculating A x B");
            time_single = (uint)multiplyMatricesSingleThread(matrixA, matrixB, n);

            await UpdateTest2(10, "Free memory for matrix A");
            freeMatrix(matrixA, n);

            await UpdateTest2(10, "Free memory for matrix B");
            freeMatrix(matrixB, n);


            //Multiple thread matrix multiplication 1000 x 1000

            await UpdateTest2(10, $"Generating matrix D of size {n} x {n}");
            IntPtr matrixC = generateRandomMatrix(n);

            await UpdateTest2(10, $"Generating matrix D of size {n} x {n}");
            IntPtr matrixD = generateRandomMatrix(n);

            await UpdateTest2(10, "Calculating C x D");
            time_multi = (uint)multiplyMatricesMultiThread(matrixC, matrixD, n);

            await UpdateTest2(10, "Free memory for matrix C");
            freeMatrix(matrixC, n);

            await UpdateTest2(10, "Free memory for matrix D");
            freeMatrix(matrixD, n);

            await UpdateTest2(0, defaultTextTest2);
        }
        #endregion 

        #region UPDATE ACTION

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
            if (OnCPUProgressBar1 != null)
            {
                foreach (var handler in OnCPUProgressBar1.GetInvocationList())
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
            if (OnCPUProgressBar2 != null)
            {
                foreach (var handler in OnCPUProgressBar2.GetInvocationList())
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

        #endregion

        #region CALCULATE SCORE
        public uint calculateScore()
        {
            uint scoreTest1 = (uint) (1000 * operations_second) / ref_operations_second;
            uint scoreSingle = (uint) (1000 * time_single) / ref_time_single;
            uint scoreMulti = (uint) (1000 * time_multi) / ref_time_multi;
            uint scoreTest2 = (uint)((0.5 * scoreSingle) + (0.5 * scoreMulti));
            return (uint)((scoreTest1 * 0.3) + (scoreTest2 * 0.7));
        }
        #endregion

        #region CPU-TEST-1
        [DllImport(@"..\..\..\Tests\cpu-test-1.dll")]
        public static extern long PerformCPUTest1();

        #endregion

        #region CPU-TEST-2
        [DllImport(@"..\..\..\Tests\cpu-test-2.dll")]
        public static extern IntPtr generateRandomMatrix(int n);

        [DllImport(@"..\..\..\Tests\cpu-test-2.dll")]
        public static extern void freeMatrix(IntPtr matrix, int n);

        [DllImport(@"..\..\..\Tests\cpu-test-2.dll")]
        public static extern int multiplyMatricesSingleThread(IntPtr A, IntPtr B, int n);

        [DllImport(@"..\..\..\Tests\cpu-test-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int multiplyMatricesMultiThread(IntPtr A, IntPtr B, int n);

        #endregion
    }
}
