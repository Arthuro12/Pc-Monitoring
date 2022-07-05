using System;
using System.Collections.Generic;
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
        }

        private void infoMessageLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //System.Diagnostics.Process.Start("");
        }
    }
}
