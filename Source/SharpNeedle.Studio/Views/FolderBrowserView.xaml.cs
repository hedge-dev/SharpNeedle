namespace SharpNeedle.Studio.Views;
using System.Windows.Controls;
using System.Windows.Input;
using Models;

/// <summary>
/// Interaction logic for FolderBrowserView.xaml
/// </summary>
public partial class FolderBrowserView : UserControl
{
    public FolderBrowserViewModel ViewModel
    {
        get => DataContext as FolderBrowserViewModel;
        set => DataContext = value;
    }

    public FolderBrowserView()
    {
        InitializeComponent();
    }

    private void OnDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ViewModel.SelectedItem.IsDirectory)
            ViewModel.ChangeDirectory(ViewModel.SelectedItem.Directory);
        else if (ViewModel.SelectedItem.IsFile)
            ResourceEditor.OpenEditor(ViewModel.SelectedItem.File);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
            {
                OnDoubleClick(null, null);
                return;
            }
            case Key.Back:
            {
                ViewModel.GoBack();
                return;
            }
            default:
                return;
        }
    }
}