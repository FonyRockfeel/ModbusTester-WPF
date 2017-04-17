using System;
using System.ComponentModel;
using System.Windows;
using ModbusTester_WPF.ViewModels;
using ModbusTester_WPF.Navigation;
using ModbusTester_WPF.AuxWindows;
using ModbusTester_WPF.Views;

namespace ModbusTester_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExtensiveWindow window;
        public MainWindow()
        {
            InitializeComponent();
            Navigator.Service = Frame.NavigationService;
            var dataContext = new MainViewModel();
            this.DataContext = dataContext;
            Closing+= OnClosing;
            window = new ExtensiveWindow();
            window.DataContext = dataContext;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            window.Dispose();
            Navigator.Closing();
        }


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            window.Show();
            window.Activate();
        }
    }
}
