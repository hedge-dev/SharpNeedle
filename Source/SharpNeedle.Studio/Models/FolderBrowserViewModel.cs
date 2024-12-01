namespace SharpNeedle.Studio.Models;

using SharpNeedle.IO;
using SharpNeedle.Resource;
using System.Collections.ObjectModel;
using System.ComponentModel;

[ResourceEditor(typeof(IDirectory), TypeMatchMethod.Assignable)]
public class FolderBrowserViewModel : IViewModel
{
    private IDirectory mCurrentDirectory;
    private FolderItem mSelectedItem;
    private string mTitle;

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

    public MenuItem Menu { get; set; }

    public FolderItem SelectedItem
    {
        get => mSelectedItem;
        set
        {
            if (Equals(value, mSelectedItem)) return;
            mSelectedItem = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<FolderItem> Items { get; set; } = new();

    public IDirectory CurrentDirectory
    {
        get => mCurrentDirectory;
        set => ChangeDirectory(value);
    }

    public IDirectory RootDirectory { get; set; }

    public FolderBrowserViewModel()
    {
        Menu = MenuItem.Create("File/Save", new RelayCommand(Save));
    }

    public FolderBrowserViewModel(IDirectory root) : this()
    {
        RootDirectory = root;
        CurrentDirectory = root;
    }

    public void ChangeDirectory(IDirectory dir)
    {
        if (dir == null)
        {
            Items.Clear();
            return;
        }
        Title = $"📂 {dir.Name}";
        mCurrentDirectory = dir;
        Items.Clear();
        foreach (var subDir in dir.GetDirectories())
            Items.Add(new FolderItem(subDir));

        foreach (var file in dir)
            Items.Add(new FolderItem(file));
    }

    public void GoBack()
    {
        var name = CurrentDirectory.Name;
        if (CurrentDirectory?.Parent == null)
            return;

        ChangeDirectory(CurrentDirectory.Parent);
        SelectedItem = Items.FirstOrDefault(x => x.IsDirectory && x.Name == name);
    }

    public void Save()
    {
        if (RootDirectory is not IResource res)
            return;

        if (res is IStreamable streamable)
            streamable.LoadToMemory();

        res.Write(RootDirectory.Path);
    }

    public void Dispose()
    {
        mCurrentDirectory = null;
        Items.Clear();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [ResourceEditorCreator]
    public static IViewModel Create(IResource res)
    {
        return res is not IDirectory dir ? null : new FolderBrowserViewModel(dir);
    }
}

public class FolderItem
{
    public object Item { get; }
    public string Name { get; }
    public bool IsDirectory { get; }
    public bool IsFile { get; }

    public IDirectory Directory => IsDirectory ? Item as IDirectory : null;
    public IFile File => !IsDirectory ? Item as IFile : null;

    private FolderItem()
    {

    }

    public FolderItem(IDirectory dir) : this()
    {
        Item = dir;
        Name = dir.Name;
        IsDirectory = true;
    }

    public FolderItem(IFile file) : this()
    {
        Item = file;
        Name = file.Name;
        IsFile = true;
    }
}