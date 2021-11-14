namespace SharpNeedle.Studio.Models;
using System.ComponentModel;

public class NotifyVector3 : INotifyPropertyChanged
{
    private Vector3 mValue;
        
    public float X
    {
        get => mValue.X;
        set
        {
            if (value.Equals(mValue.X)) return;
            mValue.X = value;
            OnPropertyChanged();
        }
    }

    public float Y
    {
        get => mValue.Y;
        set
        {
            if (value.Equals(mValue.Y)) return;
            mValue.Y = value;
            OnPropertyChanged();
        }
    }

    public float Z
    {
        get => mValue.Z;
        set
        {
            if (value.Equals(mValue.Z)) return;
            mValue.Z = value;
            OnPropertyChanged();
        }
    }

    public NotifyVector3()
    {

    }

    public NotifyVector3(Vector3 value)
    {
        mValue = value;
    }

    public static implicit operator NotifyVector3(Vector3 value) => new (value);
    public static implicit operator Vector3(NotifyVector3 value) => value.mValue;

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}