namespace SharpNeedle.Ninja.Csd;
using SceneCollection = CsdDictionary<BinaryPointer<Scene>>;
using NodeCollection = CsdDictionary<SceneNode>;

public class SceneNode : IBinarySerializable
{
    public SceneNode Parent { get; set; }
    public CsdDictionary<Scene> Scenes { get; set; } = new();
    public NodeCollection Children { get; set; } = new();

    public void Read(BinaryObjectReader reader)
    {
        var scenes = reader.ReadObject<SceneCollection>();
        Children = reader.ReadObject<NodeCollection>();

        foreach (var child in Children)
            child.Value.Parent = this;

        foreach (var scene in scenes)
            Scenes.Add(scene.Key, scene.Value);
    }

    public void Write(BinaryObjectWriter writer)
    {
        var scenes = new SceneCollection();
        foreach (var scene in Scenes)
            scenes.Add(scene.Key, scene.Value);

        writer.WriteObject(scenes);
        writer.WriteObject(Children);
    }
}