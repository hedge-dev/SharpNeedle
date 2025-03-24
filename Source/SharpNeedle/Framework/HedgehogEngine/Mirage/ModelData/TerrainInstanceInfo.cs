namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;

[NeedleResource("hh/terrain-instance-info", $@"\{Extension}$")]
public class TerrainInstanceInfo : SampleChunkResource
{
    public const string Extension = ".terrain-instanceinfo";

    public ResourceReference<TerrainModel> Model { get; set; }
    public Matrix4x4 Transform { get; set; }
    public List<LightIndexMeshGroup> LightGroups { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        Model = reader.ReadStringOffsetOrEmpty();
        Transform = Matrix4x4.Transpose(reader.ReadValueOffset<Matrix4x4>());
        Name = reader.ReadStringOffsetOrEmpty();

        if (DataVersion >= 5)
        {
            LightGroups = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMeshGroup>>>().Unwind();
        }
        else if (DataVersion > 0)
        {
            LightGroups = new List<LightIndexMeshGroup>(2) { reader.ReadObject<LightIndexMeshGroup>() };
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Model.Name);
        writer.WriteValueOffset(Matrix4x4.Transpose(Transform));
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if (DataVersion >= 5)
        {
            writer.Write(LightGroups.Count);
            writer.WriteOffset(() =>
            {
                foreach (LightIndexMeshGroup group in LightGroups)
                {
                    writer.WriteObjectOffset(group);
                }
            });
        }
        else if (DataVersion > 0)
        {
            if (LightGroups.Count == 1)
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
        if (Model.IsValid())
        {
            return;
        }

        string filename = $"{Model.Name}.terrain-model";
        Model = resolver.Open<TerrainModel>(filename)
            ?? throw new ResourceResolveException($"Failed to resolve {filename}", [filename]);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
        {
            return;
        }

        Model.Resource?.Dispose();
        Model = default;
    }
}