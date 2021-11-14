using System;
using System.ComponentModel;

namespace SharpNeedle.Studio
{
    public interface IViewModel : IDisposable, INotifyPropertyChanged
    {
        public string Title { get; set; }
        public MenuItem Menu { get; }
    }
}
