using benchmark_sofware.Views;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace benchmark_sofware
{
    public partial class MainWindow : Window
    {

        private CPUView CPUView;
        private GPUView GPUView;
        private RAMView RAMView;    
        private DiskView DiskView;

        private SolidColorBrush unselected;
        private SolidColorBrush selected;
        public MainWindow()
        {
            InitializeComponent();

            unselected = new SolidColorBrush(Color.FromArgb(0xFF, 0x26, 0x25, 0x25));
            selected = new SolidColorBrush(Color.FromArgb(0xFF, 0x3C, 0x3A, 0x3B));

            CPUView = new CPUView(true);
            GPUView = new GPUView(false);
            RAMView = new RAMView(false);
            DiskView = new DiskView();

            MainWindowPageView.Content = CPUView;
            CPUMenuButton.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x3C, 0x3A, 0x3B));

        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MenuSelection(object sender, RoutedEventArgs e)
        {
            setUnselected();

            if (sender == CPUMenuButton)
            {
                CPUView.StartUpdates();
                MainWindowPageView.Content = CPUView;
                CPUMenuButton.Background = selected;
            }
            else if (sender == GPUMenuButton)
            {
                GPUView.StartUpdates();
                MainWindowPageView.Content = GPUView;
                GPUMenuButton.Background = selected;
            }
            else if (sender == RAMMenuButton)
            {
                RAMView.StartUpdates();
                MainWindowPageView.Content = RAMView;
                RAMMenuButton.Background = selected;
            }
            else if (sender == StorageMenuButton)
            {
                MainWindowPageView.Content = DiskView;
                StorageMenuButton.Background = selected;
            }
        }

        private void setUnselected()
        {
            CPUMenuButton.Background = unselected;
            GPUMenuButton.Background = unselected;
            RAMMenuButton.Background = unselected;
            StorageMenuButton.Background = unselected;
            CPUView.StopUpdates();
            GPUView.StopUpdates();
            RAMView.StopUpdates();
        }
    }
}