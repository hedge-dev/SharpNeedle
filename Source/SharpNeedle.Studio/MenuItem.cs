namespace SharpNeedle.Studio;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class MenuItem : INotifyPropertyChanged, ICloneable<MenuItem>
{
    private string mHeader;
    private ICommand mCommand;
    private bool mEnabled = true;

    public bool Enabled
    {
        get => mEnabled;
        set
        {
            if (value == mEnabled) return;
            mEnabled = value;
            OnPropertyChanged();
        }
    }

    public string Header
    {
        get => mHeader;
        set
        {
            if (value == mHeader) return;
            mHeader = value;
            OnPropertyChanged();
        }
    }

    public ICommand Command
    {
        get => mCommand;
        set
        {
            if (Equals(value, mCommand)) return;
            mCommand = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<MenuItem> Children { get; set; } = new();

    public MenuItem()
    {

    }

    public MenuItem(string header)
    {
        Header = header;
    }

    public MenuItem(string header, ICommand command) : this(header)
    {
        Command = command;
    }

    public static MenuItem Create(string path, ICommand command)
    {
        var item = new MenuItem();
        item.WithChild(path, command);
        return item;
    }

    public MenuItem GetItem(string header)
        => Children.FirstOrDefault(m => m.Header.Equals(header, StringComparison.InvariantCultureIgnoreCase));

    public MenuItem WithChild(string path, ICommand command)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        var names = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = this;

        for (int i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var isLast = i == names.Length - 1;

            var child = current.Children.FirstOrDefault(m => m.Header.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (child == null)
            {
                child = new MenuItem(name, isLast ? command : null);
                current.Add(child);
            }

            current = child;
        }

        return this;
    }

    public MenuItem MergeWith(MenuItem other)
    {
        var cloned = Clone();
        if (other == null || other.Children.Count == 0)
        {
            if (!string.IsNullOrEmpty(other?.Header))
                cloned.Add(other);

            return cloned;
        }

        foreach (var child in other.Children)
        {
            var item = cloned.GetItem(child.Header);
            if (item == null)
            {
                cloned.Children.Add(child);
                continue;
            }
                
            cloned.Children[cloned.Children.IndexOf(item)] = item.MergeWith(child);
        }

        return cloned;
    }

    public void Add(MenuItem item)
    {
        Children.Add(item);
    }

    public MenuItem Clone()
    {
        var cloned = new MenuItem(Header, Command);

        foreach (var child in Children)
        {
            cloned.Add(child.Clone());
        }

        return cloned;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}