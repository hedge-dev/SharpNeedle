namespace SharpNeedle.HedgehogEngine.Mirage;

public class LightIndexMeshGroup : List<LightIndexMesh>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        var opaqMeshes = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMesh>>>();
        var punchMeshes = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMesh>>>();
        var transMeshes = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMesh>>>();

        Capacity = opaqMeshes.Count + transMeshes.Count + transMeshes.Count;
        AddMeshes(opaqMeshes, Mesh.Type.Opaque);
        AddMeshes(transMeshes, Mesh.Type.PunchThrough);
        AddMeshes(transMeshes, Mesh.Type.Transparent);

        void AddMeshes(BinaryList<BinaryPointer<LightIndexMesh>> meshes, MeshSlot slot)
        {
            foreach (var mesh in meshes)
            {
                mesh.Value.Slot = slot;
                Add(mesh);
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Opaque));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.PunchThrough));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Transparent));

        void WriteMeshes(IEnumerable<LightIndexMesh> meshes)
        {
            writer.Write(meshes.Count());
            writer.WriteOffset(() =>
            {
                foreach (var mesh in meshes)
                    writer.WriteObjectOffset(mesh);
            });
        }
    }
}