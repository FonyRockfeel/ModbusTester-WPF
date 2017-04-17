using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using ModbusTester_WPF.ViewModels;

namespace ModbusTester_WPF
{
    /// <summary>
    /// Логика взаимодействия для ExtensiveWindow.xaml
    /// </summary>
    public partial class ExtensiveWindow : Window
    {
        public ExtensiveWindow()
        {
            InitializeComponent();
            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Visibility = Visibility.Collapsed;
            cancelEventArgs.Cancel = true;
        }

        public void Dispose()
        {
            Closing -= OnClosing;
            Close();
        }
    }
}
