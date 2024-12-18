using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace benchmark_sofware.Score
{
    internal class GPUScore
    {

        public event Action<uint, string> OnGPUProgressBar1;
        public event Action<uint, string> OnGPUProgressBar2;
        public event Action<uint, uint, uint, uint, uint> OnGPUScore;

        public double frames_2d = 0;
        public double frames_3d = 0;
        
        private double ref_frames_2d = 7.16;
        private double ref_frames_3d = 135.67;

        private uint scoreTest1 = 0;
        private uint scoreTest2 = 0;
        private uint scoreFinal = 0;

        private uint progressTest1 = 0;
        private uint progressTest2 = 0;

        private const string defaultTextTest1 = "Generating 2d shapes";
        private const string defaultTextTest2 = "Generating 3d shape";

        public GPUScore() { }

        public async Task startTest1()
        {
            await UpdateTest1(0, "Starting gpu test 1");
            frames_2d = startGPUTest1();
            await UpdateTest1(100, defaultTextTest1);

        }

        public async Task startTest2()
        {
            await UpdateTest2(0, "Starting gpu test 2");
            frames_3d = startGPUTest2();
            await UpdateTest2(100, defaultTextTest2);
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
            if (OnGPUProgressBar1 != null)
            {
                foreach (var handler in OnGPUProgressBar1.GetInvocationList())
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
            if (OnGPUProgressBar2 != null)
            {
                foreach (var handler in OnGPUProgressBar2.GetInvocationList())
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
            uint scoreTest1 = (uint)((0.5 * frames_2d) + (0.5 * ref_frames_2d));
            uint scoreTest2 = (uint)((0.5 * frames_3d) + (0.5 * ref_frames_3d));
            return (uint)((scoreTest1 * 0.5) + (scoreTest2 * 0.5));
        }
        #endregion


        [DllImport(@"..\..\..\Tests\gpu-test-1.dll")]
        public static extern float startGPUTest1();

        [DllImport(@"..\..\..\Tests\gpu-test-2.dll")]
        public static extern double startGPUTest2();
    }
}
