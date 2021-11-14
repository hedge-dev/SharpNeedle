using System;
using System.Windows.Controls;
using AvalonDock;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace SharpNeedle.Studio.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void OnDockLayoutUpdated(object sender, EventArgs e)
        {
            foreach (var window in DockMan.FloatingWindows)
            {
                window.Owner = null;
            }
        }

        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            if (e.Document.Content is IViewModel model)
            {
                e.Document.CanClose = false;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Singleton.GetInstance<Workspace>().CloseDocument(model)));
            }
            else if (e.Document.Content is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
