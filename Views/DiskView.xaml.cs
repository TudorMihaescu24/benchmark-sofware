using benchmark_software.Models;
using benchmark_sofware.Score;
using Hardware.Info;
using System.Windows;
using System.Windows.Controls;

namespace benchmark_sofware.Views
{
    public partial class DiskView : UserControl
    {
        private DiskInfo disk;
        private DiskScore score;
        public DiskView()
        {
            InitializeComponent();
            disk = new DiskInfo();
            score = new DiskScore();

            PopulateDiskInfoUI(DiskCanvas, disk);
            score.OnDiskProgressBar1 += UpdateProgressBarTest1;
            score.OnDiskProgressBar2 += UpdateProgressBarTest2;
        }


        private void PopulateDiskInfoUI(Canvas canvas, DiskInfo disk)
        {
            const double initialLeft = 25; 
            const double spacing = 200;   
            int diskIndex = 0;
            
            DISK_Total_Memory.Text = $"{disk.TotalSpace} GB";
            DISK_Free_Memory.Text = $"{disk.TotalFreeSpace} GB";
            DISK_Used_Memory.Text = $"{disk.TotalUsedSpace} GB";
            
            foreach (var d in disk.Disks)
            {
                if (d == null) continue;

                double leftOffset = initialLeft + diskIndex * spacing;

                var diskName = new TextBlock
                {
                    Text = $"Local Disk {d.Id}",
                    Style = (Style)FindResource("TextSimpleTitle")
                };
                Canvas.SetLeft(diskName, leftOffset);
                Canvas.SetTop(diskName, 85);
                canvas.Children.Add(diskName);

                var totalText = new TextBlock
                {
                    Text = $"Total:",
                    Style = (Style)FindResource("TextSimpleTitle")
                };
                Canvas.SetLeft(totalText, leftOffset);
                Canvas.SetTop(totalText, 135);
                canvas.Children.Add(totalText);

                var freeText = new TextBlock
                {
                    Text = $"Free:",
                    Style = (Style)FindResource("TextSimpleTitle")
                };
                Canvas.SetLeft(freeText, leftOffset);
                Canvas.SetTop(freeText, 185);
                canvas.Children.Add(freeText);

                var usedText = new TextBlock
                {
                    Text = $"Used:",
                    Style = (Style)FindResource("TextSimpleTitle")
                };
                Canvas.SetLeft(usedText, leftOffset);
                Canvas.SetTop(usedText, 235);
                canvas.Children.Add(usedText);


                var totalValue = new TextBlock
                {
                    Text = $"{d.Size} GB",
                    Style = (Style)FindResource("TextSimpleValue")
                };
                Canvas.SetLeft(totalValue, leftOffset + 75);
                Canvas.SetTop(totalValue, 135);
                canvas.Children.Add(totalValue);

                var freeValue = new TextBlock
                {
                    Text = $"{d.FreeSpace} GB",
                    Style = (Style)FindResource("TextSimpleValue")
                };
                Canvas.SetLeft(freeValue, leftOffset + 75);
                Canvas.SetTop(freeValue, 185);
                canvas.Children.Add(freeValue);

                var usedValue = new TextBlock
                {
                    Text = $"{d.UsedSpace} GB",
                    Style = (Style)FindResource("TextSimpleValue")
                };
                Canvas.SetLeft(usedValue, leftOffset + 75);
                Canvas.SetTop(usedValue, 235);
                canvas.Children.Add(usedValue);

                diskIndex++;
            }
        }

        private void UpdateProgressBarTest1(uint progress, string text)
        {
            Dispatcher.Invoke(() =>
            {
                DISK_ProgressBar_Test_1.Value = (int)progress;
                DISK_Progress_Test_1.Text = $"{progress}%";
                DISK_ProgressText_Test_1.Text = text;
            });
        }

        private void UpdateProgressBarTest2(uint progress, string text)
        {
            Dispatcher.Invoke(() =>
            {
                DISK_ProgressBar_Test_2.Value = (int)progress;
                DISK_Progress_Test_2.Text = $"{progress}%";
                DISK_ProgressText_Test_2.Text = text;
            });
        }

        private void UpdateDISKScoreValues()
        {
            DISK_Value_Read.Text = $"{score.read_bandwidth:F2} MB/s";
            DISK_Value_Write.Text = $"{score.write_bandwidth:F2} MB/s";
            DISK_Value_Write_IOPS.Text = $"{score.write_iops:F2} op/s";
            DISK_Value_Read_IOPS.Text = $"{score.read_iops:F2} op/s";
            DISK_Score.Text = $"{score.calculateScore()}";
        }

        private void ResetDISKScoreValues()
        {
            DISK_Value_Read.Text = "loading...";
            DISK_Value_Write.Text = "loading...";
            DISK_Value_Write_IOPS.Text = "loading...";
            DISK_Value_Read_IOPS.Text = "loading...";
            DISK_Score.Text = "0000";
        }

        private async void StartTest(object sender, RoutedEventArgs e)
        {
            ResetDISKScoreValues();
            score.initTest();
            await score.startTest1();
            await score.startTest2();
            UpdateDISKScoreValues();
        }

    }
}
