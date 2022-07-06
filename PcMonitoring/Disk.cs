using System;
using System.Collections.Generic;
using System.Text;

namespace PcMonitoring
{
    class Disk
    {
        private string Name;
        private string Format;
        private string TotalSpace;
        private string FreeSpace;

        public Disk(string name, string format, string totalSpace, string freeSpace)
        {
            Name = name;
            Format = format;
            TotalSpace = totalSpace;
            FreeSpace = freeSpace;
        }

        public override string ToString()
        {
            return Name + " (" + Format + ") " + FreeSpace + "Frei / " + TotalSpace;
        }
    }
}
