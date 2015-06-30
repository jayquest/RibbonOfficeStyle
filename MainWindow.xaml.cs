using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernRibbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private bool _minizing = false;
        public MainWindow()
        {
            InitializeComponent();
            DwmDropShadow.DropShadowToWindow((Window)this);
        }

        private void RibbonWindow_StateChanged(object sender, EventArgs e)
        {

            if (this.WindowState == WindowState.Maximized && !_minizing)
            {
                this.WindowState = WindowState.Minimized;
                _minizing = true;
                this.WindowState = WindowState.Maximized;

            }
            else
            {
                _minizing = false;
            }
        }

        
    }
}
