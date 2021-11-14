using System.Windows;
using MahApps.Metro.Controls;

namespace SharpNeedle.Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public IViewModel Model { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Model = Content as IViewModel;
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Model?.Dispose();
        }
    }
}
