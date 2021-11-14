using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpNeedle.Studio
{
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

        public event OnDocumentChangedDelegate DocumentChanged;
        public ObservableCollection<IViewModel> Documents { get; set; } = new();

        public Workspace()
        {
            if (!Singleton.HasInstance<Workspace>())
                Singleton.SetInstance(this);
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
        }

        private void RaiseDocumentChanged(IViewModel oldDoc, IViewModel newDoc)
        {
            DocumentChanged?.Invoke(oldDoc, newDoc);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public delegate void OnDocumentChangedDelegate(IViewModel oldDocument, IViewModel newDocument);
}
