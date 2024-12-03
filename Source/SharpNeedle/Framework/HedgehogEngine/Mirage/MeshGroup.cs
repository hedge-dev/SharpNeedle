namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Structs;

public class MeshGroup : List<Mesh>, IBinarySerializable<uint>, IDisposable, ICloneable<MeshGroup>
{
    public string? Name { get; set; }

    public void Read(BinaryObjectReader reader, uint version)
    {
        bool readSpecial = version >= 5;

        BinaryList<BinaryPointer<Mesh, uint>, uint> opaqMeshes = reader.ReadObject<BinaryList<BinaryPointer<Mesh, uint>, uint>, uint>(version);
        BinaryList<BinaryPointer<Mesh, uint>, uint> transMeshes = reader.ReadObject<BinaryList<BinaryPointer<Mesh, uint>, uint>, uint>(version);
        BinaryList<BinaryPointer<Mesh, uint>, uint> punchMeshes = reader.ReadObject<BinaryList<BinaryPointer<Mesh, uint>, uint>, uint>(version);

        Clear();

        if(!readSpecial)
        {
            Capacity = opaqMeshes.Count + transMeshes.Count + punchMeshes.Count;
            AddMeshes(opaqMeshes, Mesh.Type.Opaque);
            AddMeshes(transMeshes, Mesh.Type.Transparent);
            AddMeshes(punchMeshes, Mesh.Type.PunchThrough);
            return;
        }

        int specialMeshCount = reader.Read<int>();
        {
            Capacity = opaqMeshes.Count + transMeshes.Count + punchMeshes.Count + specialMeshCount;
            AddMeshes(opaqMeshes, Mesh.Type.Opaque);
            AddMeshes(transMeshes, Mesh.Type.Transparent);
            AddMeshes(punchMeshes, Mesh.Type.PunchThrough);
        }

        (string Name, int Count)[] slots = new (string Name, int Count)[specialMeshCount];
        reader.ReadOffset(() =>
        {
            for(int i = 0; i < specialMeshCount; i++)
            {
                slots[i] = new(reader.ReadStringOffsetOrEmpty(), 0);
            }
        });

        // What the fuck
        reader.ReadOffset(() =>
        {
            for(int i = 0; i < slots.Length; i++)
            {
                slots[i].Count = reader.ReadValueOffset<int>();
            }
        });

        // It only gets worse
        reader.ReadOffset(() =>
        {
            foreach((string Name, int Count) slot in slots)
            {
                reader.ReadOffset(() =>
                {
                    for(int i = 0; i < slot.Count; i++)
                    {
                        Mesh mesh = reader.ReadObjectOffset<Mesh>();
                        mesh.Slot = slot.Name;
                        Add(mesh);
                    }
                });
            }
        });

        Name = reader.ReadString(StringBinaryFormat.NullTerminated);

        void AddMeshes(BinaryList<BinaryPointer<Mesh, uint>, uint> meshes, MeshSlot slot)
        {
            foreach(BinaryPointer<Mesh, uint> mesh in meshes)
            {
                mesh.Value.Slot = slot;
                Add(mesh);
            }
        }
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        bool writeSpecial = version >= 5;

        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Opaque));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Transparent));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.PunchThrough));

        if(!writeSpecial)
        {
            return;
        }

        Dictionary<string, List<Mesh>> specialGroups = [];
        foreach(Mesh mesh in this)
        {
            if(mesh.Slot != Mesh.Type.Special)
            {
                continue;
            }

            if(specialGroups.TryGetValue(mesh.Slot.Name!, out List<Mesh>? meshes))
            {
                meshes.Add(mesh);
            }
            else
            {
                specialGroups.Add(mesh.Slot.Name!, [mesh]);
            }
        }

        writer.Write(specialGroups.Count);
        writer.WriteOffset(() =>
        {
            foreach(KeyValuePair<string, List<Mesh>> group in specialGroups)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, group.Key);
            }
        });

        writer.WriteOffset(() =>
        {
            foreach(KeyValuePair<string, List<Mesh>> group in specialGroups)
            {
                writer.WriteValueOffset(group.Value.Count);
            }
        });

        writer.WriteOffset(() =>
        {
            foreach(KeyValuePair<string, List<Mesh>> group in specialGroups)
            {
                writer.WriteOffset(() =>
                {
                    foreach(Mesh mesh in group.Value)
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
                foreach(Mesh mesh in meshes)
                {
                    writer.WriteObjectOffset(mesh, version);
                }
            });
        }
    }

    public void ResolveDependencies(IResourceResolver resolver)
    {
        foreach(Mesh mesh in this)
        {
            mesh.ResolveDependencies(resolver);
        }
    }

    public void Dispose()
    {
        foreach(Mesh mesh in this)
        {
            mesh.Dispose();
        }

        Clear();
    }

    public MeshGroup Clone()
    {
        MeshGroup result = new()
        {
            Name = Name,
            Capacity = Capacity
        };

        foreach(Mesh mesh in this)
        {
            result.Add(mesh);
        }

        return result;
    }
}