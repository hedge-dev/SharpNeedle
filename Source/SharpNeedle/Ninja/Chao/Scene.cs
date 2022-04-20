namespace SharpNeedle.Ninja.Chao;
using Animation;

public class Scene : IBinarySerializable
{
    public int Field00 { get; set; }
    public float ZIndex { get; set; }
    public float AnimationFrameRate { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float AspectRatio { get; set; }
    public List<Vector2> Data1 { get; set; }
    public List<Image> Images { get; set; }
    public List<Group> Groups { get; set; }
    public ChaoCollection<KeyFrameData> KeyFrameData { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        ZIndex = reader.Read<float>();
        AnimationFrameRate = reader.Read<float>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<float>();
        Data1 = new List<Vector2>(reader.ReadArrayOffset<Vector2>(reader.Read<int>()));
        Images = new List<Image>(reader.ReadArrayOffset<Image>(reader.Read<int>()));
        Groups = reader.ReadObject<BinaryList<Group>>();
        var castDic = reader.ReadObject<CastDictionary>();
        KeyFrameData = reader.ReadObject<ChaoCollection<KeyFrameData>>();
        AspectRatio = reader.Read<float>();
        var frameData = reader.ReadArrayOffset<(int Index, float AnimationLength)>(KeyFrameData.Count);
        var animCastTableOffset = reader.ReadOffsetValue();

        foreach (var item in castDic)
        {
            Groups[item.GroupIdx].Casts[item.CastIdx].Name = item.Name;
        }

        foreach (var fData in frameData)
        {
            KeyFrameData[fData.Index].AnimationLength = fData.AnimationLength;
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(ZIndex);
        writer.Write(AnimationFrameRate);
        writer.Write(Field0C);
        writer.Write(Field10);

        writer.Write(Data1.Count);
        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Data1).AsMemory());

        writer.Write(Images.Count);
        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Images).AsMemory());

        writer.WriteObject<BinaryList<Group>>(Groups);
        writer.WriteObject(BuildCastDictionary());
        writer.WriteObject(KeyFrameData);

        writer.Write(AspectRatio);
    }

    public CastDictionary BuildCastDictionary()
    {
        var result = new CastDictionary();
        for (int i = 0; i < Groups.Count; i++)
        {
            var group = Groups[i];
            for (int c = 0; c < group.Casts.Count; c++)
            {
                result.Add(new (group.Casts[c].Name, i, c));
            }
        }

        return result;
    }
}