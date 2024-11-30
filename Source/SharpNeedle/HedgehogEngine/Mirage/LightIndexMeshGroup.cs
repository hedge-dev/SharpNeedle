namespace SharpNeedle.HedgehogEngine.Mirage;

public class LightIndexMeshGroup : List<LightIndexMesh>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        BinaryList<BinaryPointer<LightIndexMesh>> opaqMeshes = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMesh>>>();
        BinaryList<BinaryPointer<LightIndexMesh>> transMeshes = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMesh>>>();
        BinaryList<BinaryPointer<LightIndexMesh>> punchMeshes = reader.ReadObject<BinaryList<BinaryPointer<LightIndexMesh>>>();

        Capacity = opaqMeshes.Count + transMeshes.Count + punchMeshes.Count;
        AddMeshes(opaqMeshes, Mesh.Type.Opaque);
        AddMeshes(transMeshes, Mesh.Type.Transparent);
        AddMeshes(punchMeshes, Mesh.Type.PunchThrough);

        void AddMeshes(BinaryList<BinaryPointer<LightIndexMesh>> meshes, MeshSlot slot)
        {
            foreach(BinaryPointer<LightIndexMesh> mesh in meshes)
            {
                mesh.Value.Slot = slot;
                Add(mesh);
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Opaque));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.Transparent));
        WriteMeshes(this.Where(x => x.Slot == Mesh.Type.PunchThrough));

        void WriteMeshes(IEnumerable<LightIndexMesh> meshes)
        {
            writer.Write(meshes.Count());
            writer.WriteOffset(() =>
            {
                foreach(LightIndexMesh mesh in meshes)
                {
                    writer.WriteObjectOffset(mesh);
                }
            });
        }
    }
}