using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PcMonitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
         Pointer
        Rotation -90° = 0%
        Rotation 180° = 100%
        Bewegung = 270°
        2.7° per %
        */
        public MainWindow()
        {
            InitializeComponent();

            // Get PC´s infos
            GetAllSystemInfos();

            // Timer for real time updates
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.75);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        /// <summary>
        /// Timer function to refresh the screen 2 times per second
        /// </summary>
        void Timer_Tick(object sender, EventArgs e)
        {
            // Update of CPU´s infos
            CPULabel.Content = RefreshCPUInfos();
        }

        public string RefreshCPUInfos()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            dynamic firstVal = cpuCounter.NextValue(); // This variable will always be 0
            System.Threading.Thread.Sleep(75);
            dynamic val = cpuCounter.NextValue();// Thrue value

            // Rotate the image
            RotateTransform rotaateTransform = new RotateTransform((val*2.7f) - 90);
            pointerImage.RenderTransform = rotaateTransform;

            decimal roundVal = Convert.ToDecimal(val);
            roundVal = Math.Round(roundVal, 2);

            return roundVal +" %";
        }

        public void GetAllSystemInfos()
        {
            SystemInfos si = new SystemInfos();
            OSNameLabel.Content = si.GetOSInfos("os");
            OSArchitectureNameLabel.Content = si.GetOSInfos("architecture");
            processorNameLabel.Content = si.GetCPUInfos();
            GPUNameLabel.Content = si.GetGPUInfos();
        }

        private void infoMessageLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //System.Diagnostics.Process.Start("");
        }
    }
}
