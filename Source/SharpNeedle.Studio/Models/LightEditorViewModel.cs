namespace SharpNeedle.Studio.Models;
using System.ComponentModel;
using HedgehogEngine.Mirage;
using SharpNeedle.Framework.HedgehogEngine.Mirage;
using SharpNeedle.Resource;

[ResourceEditor(typeof(Light))]
public class LightEditorViewModel : IViewModel
{
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

    public LightEditorModel Model { get; }

    public string Name => Model.BaseLight.Name;

    public NotifyVector3 Position
    {
        get => Model.Position;
        set => Model.Position = value;
    }

    public NotifyVector3 Color
    {
        get => Model.Color;
        set => Model.Color = value;
    }

    public LightEditorViewModel()
    {
        Model = new LightEditorModel();
        Setup();
    }

    public LightEditorViewModel(Light light)
    {
        Model = new LightEditorModel(light);
        Title = $"💡 {Name}";
        Setup();
    }

    protected void Setup()
    {
        Menu = MenuItem.Create("File/Save", new RelayCommand(Save));
    }

    public void Save()
    {
        Model.Save();
    }

    public void Dispose()
    {

    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [ResourceEditorCreator]
    public static IViewModel CreateEditor(IResource resource)
    {
        return new LightEditorViewModel((Light)resource);
    }
}

public class LightEditorModel : INotifyPropertyChanged
{
    private NotifyVector3 mPosition;
    private NotifyVector3 mColor;
    public Light BaseLight { get; }

    public NotifyVector3 Position
    {
        get => mPosition;
        set
        {
            if (Equals(value, mPosition)) return;
            mPosition = value;
            BaseLight.Position = mPosition;
            OnPropertyChanged();
        }
    }

    public NotifyVector3 Color
    {
        get => mColor;
        set
        {
            if (Equals(value, mColor)) return;
            mColor = value;
            BaseLight.Color = mColor;
            OnPropertyChanged();
        }
    }

    public LightEditorModel()
    {
        BaseLight = new Light();
        Setup(BaseLight);
    }

    public LightEditorModel(Light light)
    {
        BaseLight = light;
        Setup(BaseLight);
    }

    private void Setup(Light light)
    {
        mColor = light.Color;
        mPosition = light.Position;
    }

    public void Save()
    {
        BaseLight.Color = Color;
        BaseLight.Position = Position;
        BaseLight.Save();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}