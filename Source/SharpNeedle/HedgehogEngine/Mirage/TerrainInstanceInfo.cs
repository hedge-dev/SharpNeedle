namespace SharpNeedle.HedgehogEngine.Mirage;

[NeedleResource("hh/terrain-instance-info", $@"\{Extension}$")]
public class TerrainInstanceInfo : SampleChunkResource
{
    public const string Extension = ".terrain-instanceinfo";
    private string mModelName;

    public TerrainModel Model { get; set; }
    public Matrix4x4 Transform { get; set; }
    public List<LightIndexMeshGroup> LightGroups { get; set; }

    public string ModelName
    {
        get => Model == null ? mModelName : Model.Name;
        set
        {
            mModelName = value;
            if (Model != null)
                Model.Name = value;
        }
    }

    public override void Read(BinaryObjectReader reader)
    {
        ModelName = reader.ReadStringOffset();
        Transform = Matrix4x4.Transpose(reader.ReadValueOffset<Matrix4x4>());
        Name = reader.ReadStringOffset();
        LightGroups = DataVersion >= 5
            ? reader.ReadObject<BinaryList<BinaryPointer<LightIndexMeshGroup>>>().Unwind()
            : new List<LightIndexMeshGroup>(2) { reader.ReadObject<LightIndexMeshGroup>() };
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ModelName);
        writer.WriteValueOffset(Matrix4x4.Transpose(Transform));
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if (DataVersion >= 5)
        {
            writer.Write(LightGroups.Count);
            writer.WriteOffset(() =>
            {
                foreach (var group in LightGroups)
                    writer.WriteObjectOffset(group);
            });
        }
        else
        {
            if (LightGroups.Count == 1)
                LightGroups[0].Write(writer);
            else
            {
                var dummyGroup = new LightIndexMeshGroup();
                dummyGroup.Capacity = LightGroups.Sum(x => x.Count);
                dummyGroup.AddRange(LightGroups.SelectMany(x => x));
                dummyGroup.Write(writer);
            }
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if (string.IsNullOrEmpty(mModelName))
            return;

        Model = resolver.Open<TerrainModel>($"{mModelName}.terrain-model");
        mModelName = null;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        
        Model?.Dispose();
        Model = null;
    }
}