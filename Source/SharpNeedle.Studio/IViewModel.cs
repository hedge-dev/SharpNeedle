namespace SharpNeedle.Studio;
using System.ComponentModel;

public interface IViewModel : IDisposable, INotifyPropertyChanged
{
    public string Title { get; set; }
    public MenuItem Menu { get; }
}