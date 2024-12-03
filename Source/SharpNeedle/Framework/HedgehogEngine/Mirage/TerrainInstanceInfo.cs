namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource("hh/terrain-instance-info", $@"\{Extension}$")]
public class TerrainInstanceInfo : SampleChunkResource
{
    public const string Extension = ".terrain-instanceinfo";
    private string? _modelName;

    public TerrainModel? Model { get; set; }
    public Matrix4x4 Transform { get; set; }
    public List<LightIndexMeshGroup> LightGroups { get; set; } = [];

    public string? ModelName
    {
        get => Model == null ? _modelName : Model.Name;
        set
        {
            _modelName = value;
            if(Model != null)
            {
                Model.Name = value ?? string.Empty;
            }
        }
    }

    public override void Read(BinaryObjectReader reader)
    {
        ModelName = reader.ReadStringOffset();
        Transform = Matrix4x4.Transpose(reader.ReadValueOffset<Matrix4x4>());
        Name = reader.ReadStringOffsetOrEmpty();
        LightGroups = DataVersion >= 5
            ? reader.ReadObject<BinaryList<BinaryPointer<LightIndexMeshGroup>>>().Unwind()
            : new List<LightIndexMeshGroup>(2) { reader.ReadObject<LightIndexMeshGroup>() };
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ModelName);
        writer.WriteValueOffset(Matrix4x4.Transpose(Transform));
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if(DataVersion >= 5)
        {
            writer.Write(LightGroups.Count);
            writer.WriteOffset(() =>
            {
                foreach(LightIndexMeshGroup group in LightGroups)
                {
                    writer.WriteObjectOffset(group);
                }
            });
        }
        else
        {
            if(LightGroups.Count == 1)
            {
                LightGroups[0].Write(writer);
            }
            else
            {
                LightIndexMeshGroup dummyGroup = [];
                dummyGroup.Capacity = LightGroups.Sum(x => x.Count);
                dummyGroup.AddRange(LightGroups.SelectMany(x => x));
                dummyGroup.Write(writer);
            }
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if(string.IsNullOrEmpty(_modelName))
        {
            return;
        }

        Model = resolver.Open<TerrainModel>($"{_modelName}.terrain-model");
        _modelName = null;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if(!disposing)
        {
            return;
        }

        Model?.Dispose();
        Model = null;
    }
}