using System.Collections.Specialized;

namespace SharpNeedle.Studio;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class Workspace : INotifyPropertyChanged
{
    private IViewModel mSelectedDocument;

    public IViewModel SelectedDocument
    {
        get => mSelectedDocument;
        set
        {
            if (Equals(value, mSelectedDocument)) return;
            var oldDoc = mSelectedDocument;
            mSelectedDocument = value;
            OnPropertyChanged();
            RaiseDocumentChanged(oldDoc, value);
        }
    }

    public event DocumentChangedDelegate DocumentChanged;
    public event DocumentClosedDelegate DocumentClosed;
    public ObservableCollection<IViewModel> Documents { get; } = new();
    public static Workspace Instance => default(Singleton<Workspace>);

    static Workspace()
    {
        Singleton.SetInstance(new Workspace());
    }

    public void AddDocument(IViewModel document)
    {
        Documents.Add(document);
        SelectedDocument = document;
    }

    public void CloseDocument(IViewModel document)
    {
        Documents.Remove(document);
        document.Dispose();
        RaiseDocumentClosed(document);

        // The GC refuses to collect the unused resources on demand if i don't do it here
        GC.Collect();
    }

    private void RaiseDocumentChanged(IViewModel oldDoc, IViewModel newDoc)
    {
        DocumentChanged?.Invoke(oldDoc, newDoc);
    }

    private void RaiseDocumentClosed(IViewModel document)
    {
        DocumentClosed?.Invoke(document);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public delegate void DocumentChangedDelegate(IViewModel oldDocument, IViewModel newDocument);
public delegate void DocumentClosedDelegate(IViewModel document);