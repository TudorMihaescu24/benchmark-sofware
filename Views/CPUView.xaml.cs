using benchmark_software.Models;
using benchmark_sofware.Score;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace benchmark_sofware.Views
{
    public partial class CPUView : UserControl
    {
        private bool active;
        private CPUInfo cpu;
        private CPUScore score;

        public CPUView(bool active)
        {
            InitializeComponent();
            cpu = new CPUInfo(true);
            score = new CPUScore();
            this.active = active;

            UpdateCPUInfo();
            cpu.OnCPUDataUpdated += UpdateCPUDynamic;
            score.OnCPUProgressBar1 += UpdateProgressBarTest1;
            score.OnCPUProgressBar2 += UpdateProgressBarTest2;
        }

        private void UpdateProgressBarTest1(uint progress, string text)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    CPU_ProgressBar_Test_1.Value = (int)progress;
                    CPU_Progress_Test_1.Text = $"{progress}%";
                    CPU_ProgressText_Test_1.Text = text;
                });
            }
        }
        private void UpdateProgressBarTest2(uint progress, string text)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    CPU_ProgressBar_Test_2.Value = (int)progress;
                    CPU_Progress_Test_2.Text = $"{progress}%";
                    CPU_ProgressText_Test_2.Text = text;
                });
            }
        }

        private void UpdateCPUDynamic(int usage, int temperature)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    CPU_Temperature.Text = $"{temperature}";
                    CPU_Usage.Text = $"{usage}";
                });
            }
        }

        private void UpdateCPUInfo()
        {
            CPU_Name.Text = Regex.Replace(cpu.Name, @"w/.*", "");
            CPU_Architecture.Text = cpu.Architecture;
            CPU_Frequency.Text = $"{cpu.Frequency} MHz";
            CPU_Cores.Text = cpu.Cores.ToString();
            CPU_Threads.Text = cpu.Threads.ToString();
            CPU_L1Cache.Text = $"{cpu.L1Cache} KB";
            CPU_L2Cache.Text = $"{cpu.L2Cache / 1024} MB";
            CPU_L3Cache.Text = $"{cpu.L3Cache / 1024} MB";
        }

        private void UpdateCPUScoreValues()
        {
            CPU_Value_Operations.Text = $"{score.operations_second} op";
            CPU_Value_Singlethread.Text = $"{score.time_single:F2} ms";
            CPU_Value_Multithread.Text = $"{score.time_multi:F2} ms";
            CPU_Score.Text = $"{score.calculateScore()}";
        }

        private void ResetCPUScoreValues()
        {
            CPU_Value_Singlethread.Text = "loading...";
            CPU_Value_Multithread.Text = "loading...";
            CPU_Value_Operations.Text = "loading...";
            CPU_Score.Text = "0000";
        }

        private void Close()
        {
            cpu.Close();
        }

        private async void StartTest(object sender, RoutedEventArgs e)
        {
            ResetCPUScoreValues();
            score.initTest();
            await score.startTest1();
            await score.startTest2();
            UpdateCPUScoreValues();
        }

        public void StartUpdates()
        {
            cpu.StartUpdates();
            active = true;
        }
        public void StopUpdates()
        {
            cpu.StopUpdates();
            active = false;
        }
    }
}
