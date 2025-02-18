namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

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
                exceptions.SelectMany(x => x.Resources).ToArray()
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
        if (DataVersion >= 5)
        {
            Groups = reader.ReadObject<BinaryList<BinaryPointer<MeshGroup, uint>, uint>, uint>(DataVersion).Unwind();
        }
        else
        {
            Groups = [reader.ReadObject<MeshGroup, uint>(DataVersion)];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CommonWrite(BinaryObjectWriter writer)
    {
        if (DataVersion >= 5)
        {
            writer.Write(Groups.Count);
            writer.WriteOffset(() =>
            {
                foreach (MeshGroup group in Groups)
                {
                    writer.WriteObjectOffset(group, DataVersion);
                }
            });
        }
        else
        {
            if (Groups.Count == 1)
            {
                writer.WriteObject(Groups[0], DataVersion);
            }
            else
            {
                MeshGroup dummyMeshGroup = [];
                dummyMeshGroup.Capacity = Groups.Sum(x => x.Count);
                dummyMeshGroup.AddRange(Groups.SelectMany(x => x));
                writer.WriteObject(dummyMeshGroup, DataVersion);
            }
        }
    }
}