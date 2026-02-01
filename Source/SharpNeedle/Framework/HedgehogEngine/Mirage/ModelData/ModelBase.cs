namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.IO;

public abstract class ModelBase : SampleChunkResource
{
    public List<MeshGroup> Groups { get; set; } = [];

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        List<ResourceResolveException> exceptions = [];

        foreach (MeshGroup group in Groups)
        {
            try
            {
                group.ResolveDependencies(resolver);
            }
            catch (ResourceResolveException exc)
            {
                exceptions.Add(exc);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new ResourceResolveException(
                $"Failed to resolve dependencies of {exceptions.Count} mesh groups",
                exceptions.SelectMany(x => x.GetRecursiveResources()).ToArray()
            );
        }
    }

    public override void WriteDependencies(IDirectory dir)
    {
        foreach (MeshGroup group in Groups)
        {
            group.WriteDependencies(dir);
        }
    }

    protected override void Dispose(bool disposing)
    {
        foreach (MeshGroup group in Groups)
        {
            group.Dispose();
        }

        Groups.Clear();
        base.Dispose(disposing);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CommonRead(BinaryObjectReader reader)
    {
        MeshSerializeContext context = new(DataVersion, false);

        if (DataVersion >= 5)
        {
            Groups = reader.ReadObject<BinaryList<BinaryPointer<MeshGroup, MeshSerializeContext>, MeshSerializeContext>, MeshSerializeContext>(context).Unwind();
        }
        else
        {
            Groups = [reader.ReadObject<MeshGroup, MeshSerializeContext>(context)];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CommonWrite(BinaryObjectWriter writer)
    {
        MeshSerializeContext context = new(DataVersion, false);

        if (DataVersion >= 5)
        {
            writer.WriteInt32(Groups.Count);
            writer.WriteOffset(() =>
            {
                foreach (MeshGroup group in Groups)
                {
                    writer.WriteObjectOffset(group, context);
                }
            });
        }
        else
        {
            if (Groups.Count == 1)
            {
                writer.WriteObject(Groups[0], context);
            }
            else
            {
                MeshGroup dummyMeshGroup = [];
                dummyMeshGroup.Capacity = Groups.Sum(x => x.Count);
                dummyMeshGroup.AddRange(Groups.SelectMany(x => x));
                writer.WriteObject(dummyMeshGroup, context);
            }
        }
    }
}