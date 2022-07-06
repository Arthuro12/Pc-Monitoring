using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
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

            // Get Disk´s infos
            GetDrivesInfos();

            // Timer for real time updates
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.75);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void GetDrivesInfos()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<Disk> disks = new List<Disk>();

            foreach(DriveInfo info in allDrives)
            {
                if (info.IsReady == true)
                {
                    Console.WriteLine("Festplatte " + info.Name + " ist bereit !");                   
                }
                disks.Add(new Disk(info.Name, info.DriveFormat, FormatBytes(info.TotalSize), FormatBytes(info.AvailableFreeSpace)));
            }

            disksListLb.ItemsSource = disks;

        } 

        private static string FormatBytes(long bytes)
        {
            string[] suffixe = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < suffixe.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, suffixe[i]);
        }

        /// <summary>
        /// Timer function to refresh the screen 2 times per second
        /// </summary>
        void Timer_Tick(object sender, EventArgs e)
        {
            // Update of CPU´s infos
            CPULabel.Content = RefreshCPUInfos();

            // Update RAM´s infos
            RefreshRamInfos();

            // ZUpdate network´s infos 
            RefreshNetworkInfos();

            // Update temperatur´s infos
            RefreshTemperatureInfos();
        }

        public void RefreshTemperatureInfos()
        {
            Double temperature = 0;
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher("root\\WMI", "Select * From MSAcpi_ThermalZoneTemperature");

            foreach (System.Management.ManagementObject mo in mos.Get())
            {
                temperature = Convert.ToDouble(Convert.ToDouble(mo.GetPropertyValue("CurrentTemperature").ToString()) - 2732) / 10;
            }
            tempLabel.Content = temperature + "°C"; 
            /*Double temperature = 0;
            String instanceName = "";

            ManagementObjectSearcher mos = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_TemperatureProbe");

            try
            {
                
                foreach (ManagementObject mo in mos.Get())
                {
                    if(Convert.ToDouble(mo["CurrentTemperature"].ToString()) > 0)
                    {
                        temperature = Convert.ToDouble(mo["CurrentTemperature"].ToString());
                        // °F to °C
                        temperature = (temperature - 2732) / 10.0;
                        instanceName = mo["InstanceName"].ToString();
                    }                  
                   
                }               
            }
            catch (ManagementException me)
            {
                MessageBox.Show(me.Message);
            }
            tempLabel.Content = temperature + "°C";*/
        }

        public void RefreshNetworkInfos()
        {
            if(!NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach(NetworkInterface ni in interfaces)
            {
                if(ni.GetIPv4Statistics().BytesSent > 0)
                   sentDataLabel.Content = ni.GetIPv4Statistics().BytesSent / 1000 + " KB";
                if(ni.GetIPv4Statistics().BytesReceived > 0)
                   receivedDataLabel.Content = ni.GetIPv4Statistics().BytesReceived / 1000 + " KB";
            }
        }

        public void RefreshRamInfos()
        {
            totalRAM.Content = "Gesamt : " + FormatSize(GetTotalPhys());
            usedRAM.Content = "Update : " + FormatSize(GetUsedPhys());
            freeRAM.Content = "Verfügbar : " + FormatSize(GetAvailPhys());

            string[] maxValue = FormatSize(GetTotalPhys()).Split(' ');
            barRAM.Maximum = float.Parse(maxValue[0]);
            string[] memoryValue = FormatSize(GetUsedPhys()).Split(' ');
            barRAM.Value = float.Parse(memoryValue[0]);
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

        /* Working with the memory (RAM) */
        #region specific functions to the RAM
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO min);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFiles;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        static string FormatSize(double size)
        {
            double d = (double)size;
            int i = 0;
            while((d > 1024) && (i < 5))
            {
                d /= 1024;
                i++;
            }
            string[] unit = { "B", "KB", "MB", "GB", "TB" };
            return (string.Format("{0} {1}", Math.Round(d, 2), unit[i])); 
        }

        public static MEMORY_INFO GetMemoryStatus()
        {
            MEMORY_INFO mi = new MEMORY_INFO();
            mi.dwLength = (uint)Marshal.SizeOf(mi);
            GlobalMemoryStatusEx(ref mi);
            return mi;
        }

        // Get free physic memory
        public static ulong GetAvailPhys()
        {
            MEMORY_INFO mi = GetMemoryStatus();
            return mi.ullAvailPhys;
        }

        // Get used memory
        public static ulong GetUsedPhys()
        {
            MEMORY_INFO mi = GetMemoryStatus();
            return (mi.ullTotalPhys - mi.ullAvailPhys);
        }

        //Get total physic memory
        public static ulong GetTotalPhys()
        {
            MEMORY_INFO mi = GetMemoryStatus();
            return mi.ullTotalPhys;
        }

        #endregion

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
