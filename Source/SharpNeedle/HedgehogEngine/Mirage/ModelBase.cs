﻿namespace SharpNeedle.HedgehogEngine.Mirage;

public abstract class ModelBase : SampleChunkResource
{
    public List<MeshGroup> Groups { get; set; }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        foreach (var group in Groups)
            group.ResolveDependencies(resolver);
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var group in Groups)
            group.Dispose();

        Groups.Clear();
        base.Dispose(disposing);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CommonRead(BinaryObjectReader reader)
    {
        if (DataVersion >= 5)
            Groups = reader.ReadObject<BinaryList<BinaryPointer<MeshGroup, uint>, uint>, uint>(DataVersion).Unwind();
        else
            Groups = new List<MeshGroup> { reader.ReadObject<MeshGroup, uint>(DataVersion) };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CommonWrite(BinaryObjectWriter writer)
    {
        if (DataVersion >= 5)
        {
            writer.Write(Groups.Count);
            writer.WriteOffset(() =>
            {
                foreach (var group in Groups)
                    writer.WriteObjectOffset(group, DataVersion);
            });
        }
        else
        {
            if (Groups.Count == 1)
                writer.WriteObject(Groups[0], DataVersion);
            else
            {
                var dummyMeshGroup = new MeshGroup();
                dummyMeshGroup.Capacity = Groups.Sum(x => x.Count);
                dummyMeshGroup.AddRange(Groups.SelectMany(x => x));
                writer.WriteObject(dummyMeshGroup, DataVersion);
            }
        }
    }
}