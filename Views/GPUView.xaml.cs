using benchmark_sofware.Models;
using benchmark_sofware.Score;
using System.Windows;
using System.Windows.Controls;

namespace benchmark_sofware.Views
{
    public partial class GPUView : UserControl
    {
        private GPUInfo gpu;
        private GPUScore score;
        private bool active;
        public GPUView(bool active)
        {
            InitializeComponent();
            gpu = new GPUInfo(active);
            score = new GPUScore();
            this.active = active;
            
            UpdateGPUInfo();
            gpu.OnGPUDataUpdated += UpdateGPUDynamic;
            score.OnGPUProgressBar1 += UpdateProgressBarTest1;
            score.OnGPUProgressBar2 += UpdateProgressBarTest2;
        }

        private void UpdateGPUDynamic(uint usageD, uint temperatureD, uint usageI, uint temperatureI)
        {
            Dispatcher.Invoke(() =>
            {
                GPU_Dedicated_Usage.Text = usageD.ToString();
                GPU_Integrated_Usage.Text = usageI.ToString();
                GPU_Dedicated_Temperature.Text = temperatureD.ToString();
                GPU_Integrated_Temperature.Text = temperatureI.ToString();
            });
        }


        private void UpdateGPUInfo()
        {
            gpu.retreiveGPUInfo();
            GPU_Dedicated_Name.Text = gpu.NameDedicated;
            GPU_Integrated_Name.Text = gpu.NameIntegrated;
            GPU_Dedicated_Type.Text = gpu.TypeDedicated;
            GPU_Integrated_Type.Text = gpu.TypeIntegrated;
            GPU_Dedicated_VRAM.Text = $"{gpu.TotalMemoryDedicated}MB";
            GPU_Integrated_VRAM.Text = $"{gpu.TotalMemoryIntegrated} MB";
        }

        public void StartUpdates()
        {
            gpu.StartUpdates();
            active = true;
        }
        public void StopUpdates()
        {
            gpu.StopUpdates();
            active = false;
        }

        private void UpdateProgressBarTest1(uint progress, string text)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    GPU_ProgressBar_Test_1.Value = (int)progress;
                    GPU_Progress_Test_1.Text = $"{progress}%";
                    GPU_ProgressText_Test_1.Text = text;
                });
            }
        }

        private void UpdateProgressBarTest2(uint progress, string text)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    GPU_ProgressBar_Test_2.Value = (int)progress;
                    GPU_Progress_Test_2.Text = $"{progress}%";
                    GPU_ProgressText_Test_2.Text = text;
                });
            }
        }

        private void UpdateGPUScoreValues()
        {
            GPU_Value_2D.Text = $"{score.frames_2d:F2} FPS";
            GPU_Value_3D.Text = $"{score.frames_3d:F2} FPS";
            GPU_Score.Text = $"{score.calculateScore()}";
        }

        private void ResetGPUScoreValues()
        {
            GPU_Value_2D.Text = "loading...";
            GPU_Value_3D.Text = "loading...";
            GPU_Score.Text = "0000";
        }

        private async void StartTest(object sender, RoutedEventArgs e)
        {
            ResetGPUScoreValues();
            score.initTest();
            await score.startTest1();
            await score.startTest2();
            UpdateGPUScoreValues();
        }
    }
}
