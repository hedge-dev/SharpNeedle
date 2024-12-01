namespace SharpNeedle.Framework.Ninja.Csd;

using NodeCollection = CsdDictionary<SceneNode>;
using SceneCollection = CsdDictionary<BinaryPointer<Scene>>;

public class SceneNode : IBinarySerializable
{
    public SceneNode Parent { get; set; }
    public CsdDictionary<Scene> Scenes { get; set; } = [];
    public NodeCollection Children { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        SceneCollection scenes = reader.ReadObject<SceneCollection>();
        Children = reader.ReadObject<NodeCollection>();

        foreach(KeyValuePair<string, SceneNode> child in Children)
        {
            child.Value.Parent = this;
        }

        foreach(KeyValuePair<string, BinaryPointer<Scene>> scene in scenes)
        {
            Scenes.Add(scene.Key, scene.Value);
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        SceneCollection scenes = [];
        foreach(KeyValuePair<string, Scene> scene in Scenes)
        {
            scenes.Add(scene.Key, scene.Value);
        }

        writer.WriteObject(scenes);
        writer.WriteObject(Children);
    }
}