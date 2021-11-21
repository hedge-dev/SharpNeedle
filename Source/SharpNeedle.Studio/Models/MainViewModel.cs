namespace SharpNeedle.Studio.Models;
using System.ComponentModel;
using System.Windows;
using Ookii.Dialogs.Wpf;

public class MainViewModel : IViewModel
{
    private string mTitle;
    private MenuItem mMenu;
    private readonly MenuItem mBaseMenu = new();

    public string Title
    {
        get => mTitle;
        set
        {
            if (value == mTitle) return;
            mTitle = value;
            OnPropertyChanged();
        }
    }

    public MenuItem Menu
    {
        get => mMenu;
        private set
        {
            if (Equals(value, mMenu)) return;
            mMenu = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand OpenFileCommand { get; }

    public static ResourceManager ResourceManager { get; set; } = new();

    static MainViewModel()
    {
        Singleton.SetInstance<IResourceManager>(ResourceManager);
    }

    public MainViewModel()
    {
        OpenFileCommand = new RelayCommand(OpenFile);
        mBaseMenu.WithChild("File/Open", OpenFileCommand);

        Menu = mBaseMenu;
        Workspace.Instance.DocumentChanged += OnDocumentChanged;
    }
    
    public void OpenFile()
    {
        var dialog = new VistaOpenFileDialog();
        if (dialog.ShowDialog() is null or false)
            return;

        var res = ResourceManager.Open(dialog.FileName);
        if (ResourceEditor.OpenEditor(res) == null)
            EndOpen();

        void EndOpen()
        {
            ResourceManager.Close(res);
            MessageBox.Show("Resource is not supported!", App.Name);
        }
    }

    private void OnDocumentChanged(IViewModel oldDocument, IViewModel newDocument)
    {
        if (newDocument == null)
        {
            Menu = mBaseMenu;
            return;
        }

        Menu = mBaseMenu.MergeWith(newDocument.Menu);
    }

    public void Dispose()
    {
        ResourceManager.Dispose();
        Singleton.SetInstance<IResourceManager>(null);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}