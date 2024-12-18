using benchmark_sofware.Models;
using benchmark_sofware.Score;
using System.Windows;
using System.Windows.Controls;


namespace benchmark_sofware.Views
{
    public partial class RAMView : UserControl
    {
        private RAMInfo ram;
        private RAMScore score;

        private bool active;
        public RAMView(bool active)
        {
            this.active = active; 
            InitializeComponent();
        
            ram  = new RAMInfo(active);
            ram.OnRAMDataUpdated += UpdateRAMDynamic;

            score = new RAMScore();
            score.OnRAMProgressBar1 += UpdateProgressBarTest1;
            score.OnRAMProgressBar2 += UpdateProgressBarTest2;

        }
        private void UpdateRAMDynamic(uint obj)
        {
            Dispatcher.Invoke(() =>
            {
                RAM_TotalMemory.Text = $"{ram.TotalMemory} MB";
                RAM_ReservedMemory.Text = $"{ram.ReservedMemory} MB";
                RAM_UsableMemory.Text = $"{ram.UsableMemory} MB";
                RAM_Usage.Text = $"{ram.Usage}";

                for (int i = 0; i < ram.Slots.Length; i++)
                {
                    switch (i)
                    { 
                        case 0:
                            RAM_Slot1Manufacturer.Text = ram.Slots[i].Manufacturer;
                            RAM_Slot1Capacity.Text = $"{ram.Slots[i].Capacity} MB";
                            RAM_Slot1Speed.Text = $"{ram.Slots[i].Speed} MHz";
                            break;
                        case 1:
                            RAM_Slot2Manufacturer.Text = ram.Slots[i].Manufacturer;
                            RAM_Slot2Capacity.Text = $"{ram.Slots[i].Capacity} MB";
                            RAM_Slot2Speed.Text = $"{ram.Slots[i].Speed} MHz";
                            break;
                        case 2:
                            RAM_Slot3Manufacturer.Text = ram.Slots[i].Manufacturer;
                            RAM_Slot3Capacity.Text = $"{ram.Slots[i].Capacity} MB";
                            RAM_Slot3Speed.Text = $"{ram.Slots[i].Speed} MHz";
                            break;
                        case 3:
                            RAM_Slot4Manufacturer.Text = ram.Slots[i].Manufacturer;
                            RAM_Slot4Capacity.Text = $"{ram.Slots[i].Capacity} MB";
                            RAM_Slot4Speed.Text = $"{ram.Slots[i].Speed} MHz";
                            break;
                    }
                }

                for(uint i = ram.NumberOfSlots; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            RAM_Slot1Manufacturer.Text = "None";
                            RAM_Slot1Capacity.Text = "None";
                            RAM_Slot1Speed.Text = "None";
                            break;
                        case 1:
                            RAM_Slot2Manufacturer.Text = "None";
                            RAM_Slot2Capacity.Text = "None";
                            RAM_Slot2Speed.Text = "None";
                            break;
                        case 2:
                            RAM_Slot3Manufacturer.Text = "None";
                            RAM_Slot3Capacity.Text = "None";
                            RAM_Slot3Speed.Text = "None";
                            break;
                        case 3:
                            RAM_Slot4Manufacturer.Text = "None";
                            RAM_Slot4Capacity.Text = "None";
                            RAM_Slot4Speed.Text = "None";
                            break;
                    }
                }

            });
        }
        private void UpdateProgressBarTest1(uint progress, string text)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    RAM_ProgressBar_Test_1.Value = (int)progress;
                    RAM_Progress_Test_1.Text = $"{progress}%";
                    RAM_ProgressText_Test_1.Text = text;
                });
            }
        }
        private void UpdateProgressBarTest2(uint progress, string text)
        {
            if (active)
            {
                Dispatcher.Invoke(() =>
                {
                    RAM_ProgressBar_Test_2.Value = (int)progress;
                    RAM_Progress_Test_2.Text = $"{progress}%";
                    RAM_ProgressText_Test_2.Text = text;
                });
            }
        }

        public void StartUpdates()
        {
            ram.StartUpdates();
            active = true;
        }
        public void StopUpdates()
        {
            ram.StopUpdates();
            active = false;
        }
        private void UpdateRAMScoreValues()
        {
            RAM_Value_Latency.Text = $"{score.latency:F2} ns";
            RAM_Value_Read.Text = $"{score.read_bandwidth:F2} MB/s";
            RAM_Value_Write.Text = $"{score.write_bandwidth:F2} MB/s";
            RAM_Score.Text = $"{score.calculateScore()}";
        }

        private void ResetRAMScoreValues()
        {
            RAM_Value_Latency.Text = "loading...";
            RAM_Value_Read.Text = "loading...";
            RAM_Value_Write.Text = "loading...";
            RAM_Score.Text = "0000";
        }
        private async void StartTest(object sender, RoutedEventArgs e)
        {
            ResetRAMScoreValues();
            score.initTest();
            await score.startTest1();
            await score.startTest2();
            UpdateRAMScoreValues();
        }
    }
}
