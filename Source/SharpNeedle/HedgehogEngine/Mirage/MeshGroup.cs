namespace SharpNeedle.HedgehogEngine.Mirage;

public class MeshGroup : List<Mesh>, IBinarySerializable, IDisposable, ICloneable<MeshGroup>
{
    public string Name { get; set; }

    public void Read(BinaryObjectReader reader)
        => Read(reader, true);

    public void Read(BinaryObjectReader reader, bool readSpecial)
    {
        var opaqMeshes = reader.ReadObject<BinaryList<BinaryPointer<Mesh>>>();
        var transMeshes = reader.ReadObject<BinaryList<BinaryPointer<Mesh>>>();
        var punchMeshes = reader.ReadObject<BinaryList<BinaryPointer<Mesh>>>();

        Clear();

        if (!readSpecial)
        {
            Capacity = opaqMeshes.Count + transMeshes.Count + punchMeshes.Count;
            AddMeshes(opaqMeshes, Mesh.Type.Opaque);
            AddMeshes(transMeshes, Mesh.Type.Transparent);
            AddMeshes(punchMeshes, Mesh.Type.PunchThrough);
            return;
        }

        var specialMeshCount = reader.Read<int>();
        {
            Capacity = opaqMeshes.Count + transMeshes.Count + punchMeshes.Count + specialMeshCount;
            AddMeshes(opaqMeshes, Mesh.Type.Opaque);
            AddMeshes(transMeshes, Mesh.Type.Transparent);
            AddMeshes(punchMeshes, Mesh.Type.PunchThrough);
        }

        var slots = new (string Name, int Count)[specialMeshCount];
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < specialMeshCount; i++)
                slots[i] = new(reader.ReadStringOffset(), 0);
        });
        
        // What the fuck
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < slots.Length; i++)
                slots[i].Count = reader.ReadValueOffset<int>();
        });
        
        // It only gets worse
        reader.ReadOffset(() =>
        {
            foreach (var slot in slots)
            {
                reader.ReadOffset(() =>
                {
                    for (int i = 0; i < slot.Count; i++)
                    {
                        var mesh = reader.ReadObjectOffset<Mesh>();
                        mesh.Slot = slot.Name;
                        Add(mesh);
                    }
                });
            }
        });

        Name = reader.ReadString(StringBinaryFormat.NullTerminated);

        void AddMeshes(BinaryList<BinaryPointer<Mesh>> meshes, MeshSlot slot)
        {
            foreach (var mesh in meshes)
            {
                mesh.Value.Slot = slot;
                Add(mesh);
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
        => Write(writer, true);

    public void Write(BinaryObjectWriter writer, bool writeSpecial)
    {
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Opaque));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Transparent));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.PunchThrough));

        if (!writeSpecial)
            return;

        var specialGroups = new Dictionary<string, List<Mesh>>();
        foreach (var mesh in this)
        {
            if (mesh.Slot != Mesh.Type.Special)
                continue;

            if (specialGroups.TryGetValue(mesh.Slot.Name, out var meshes))
                meshes.Add(mesh);
            else
                specialGroups.Add(mesh.Slot.Name, new List<Mesh> { mesh });
        }

        writer.Write(specialGroups.Count);
        writer.WriteOffset(() =>
        {
            foreach (var group in specialGroups)
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, group.Key);
        });

        writer.WriteOffset(() =>
        {
            foreach (var group in specialGroups)
                writer.WriteValueOffset(group.Value.Count);
        });

        writer.WriteOffset(() =>
        {
            foreach (var group in specialGroups)
            {
                writer.WriteOffset(() =>
                {
                    foreach (var mesh in group.Value)
                    {
                        writer.WriteObjectOffset(mesh);
                    }
                });
            }
        });
        
        writer.WriteString(StringBinaryFormat.NullTerminated, Name ?? string.Empty);
        writer.Align(4);

        void WriteMeshes(IEnumerable<Mesh> meshes)
        {
            writer.Write(meshes.Count());
            writer.WriteOffset(() =>
            {
                foreach (var mesh in meshes)
                    writer.WriteObjectOffset(mesh);
            });
        }
    }

    public void ResolveDependencies(IResourceResolver resolver)
    {
        foreach (var mesh in this)
            mesh.ResolveDependencies(resolver);
    }

    public void Dispose()
    {
        foreach (var mesh in this)
            mesh.Dispose();

        Clear();
    }

    public MeshGroup Clone()
    {
        var result = new MeshGroup
        {
            Name = Name,
            Capacity = Capacity
        };

        foreach (var mesh in this)
            result.Add(mesh);

        return result;
    }
}