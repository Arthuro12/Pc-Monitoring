using System;
using System.Management;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PcMonitoring
{
    class SystemInfos
    {
        /// <summary>
        /// Get the OS´s infos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string GetOSInfos(string param)
        {
            // The ManagementObjectSearcher´s class allows access to system information
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_operatingSystem");
            foreach (ManagementObject mo in mos.Get())
            {
                switch (param)
                {
                    case "os":
                        return mo["Caption"].ToString();
                    case "architecture":
                        return mo["OSArchitecture"].ToString();
                    case "osversion":
                        return mo["CSDVersion"].ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// Get the CPU´s infos
        /// </summary>
        /// <returns></returns>
        public string GetCPUInfos()
        {
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);

            if (processor_name != null)
            {
                return processor_name.GetValue("ProcessorNameString").ToString();
            }

            return "";
        }

        /// <summary>
        /// Get Graphics Processing Unit´s infos
        /// </summary>
        /// <returns></returns>
        public string GetGPUInfos()
        {
            using (var searcher = new ManagementObjectSearcher("select * from win32_VideoController"))
            {
                foreach (ManagementObject managementObject in searcher.Get())
                {
                    Console.WriteLine("Name - " + managementObject["Name"]);

                    return managementObject["Name"].ToString() + " (Driver´s version : " + managementObject["DriverVersion"].ToString() + ")";
                }
            }
            return "";
        }
    }
}
