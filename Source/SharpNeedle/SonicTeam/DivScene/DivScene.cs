namespace SharpNeedle.SonicTeam.DivScene;

using System.IO;

// Reference: https://github.com/ik-01/DiEventRangers/tree/main
[NeedleResource("st/dv-scene", @"\.dvscene$")]
public class DivScene : ResourceBase, IBinarySerializable
{
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public int NodeDrawCount { get; set; }
    public List<float> Cuts { get; set; } = new();
    public List<DivPage> Pages { get; set; } = new();
    public List<float> ResourceCuts { get; set; } = new();
    public DivNode RootNode { get; set; } = new();
    public float ChainCameraIn { get; set; }
    public float ChainCameraOut { get; set; }
    int Type { get; set; }
    int SkipPointTick { get; set; }

    public List<DivResource> Resources { get; set; } = new();

    public DivScene() 
    {
        // Register encoding provider for Shift-JIS strings
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public override void Read(IFile file)
        => Read(file, GameType.Common);
    public void Read(string path, GameType game)
      => Read(FileSystem.Open(path), game);

    public void Read(IFile file, GameType game)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);

        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        reader.OffsetBinaryFormat = OffsetBinaryFormat.U32;

        Read(reader, game);
    }

    public override void Write(IFile file)
        => Write(file, GameType.Common);

    public void Write(IFile file, GameType game)
    {
        BaseFile = file;

        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);
        writer.OffsetBinaryFormat = OffsetBinaryFormat.U32;

        Write(writer, game);
    }

    public void Read(BinaryObjectReader reader)
        => Read(reader, GameType.Common);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        long dataOffset = reader.ReadOffsetValue();
        long resourcesOffset = reader.ReadOffsetValue();
        reader.Skip(24);

        reader.PushOffsetOrigin();

        reader.Seek(dataOffset + 32, SeekOrigin.Begin);
        {
            reader.Skip(8);

            StartFrame = reader.Read<float>();
            EndFrame = reader.Read<float>();
            NodeDrawCount = reader.Read<int>();

            reader.ReadOffset(() =>
            {
                int cutCount = reader.Read<int>();
                int allocSize = reader.Read<int>();
                reader.Skip(8);

                reader.ReadCollection(cutCount, Cuts);
            });

            reader.ReadOffset(() =>
            {
                int pageCount = reader.Read<int>();
                int allocSize = reader.Read<int>();
                reader.Skip(8);

                Pages.AddRange(reader.ReadObjectArray<DivPage>(pageCount));
            });

            reader.ReadOffset(() =>
            {
                int unknownCount = reader.Read<int>();
                int allocSize = reader.Read<int>();
                reader.Skip(8);
            });

            reader.ReadOffset(() =>
            {
                int resourceCutCount = reader.Read<int>();
                int allocSize = reader.Read<int>();
                reader.Skip(8);

                reader.ReadCollection(resourceCutCount, ResourceCuts);
            });

            reader.ReadOffset(() =>
            {
                int unknownCount = reader.Read<int>();
                int allocSize = reader.Read<int>();
                reader.Skip(8);
            });

            reader.ReadOffset(() =>
            {
                RootNode.Read(reader, game);
            });

            ChainCameraIn = reader.Read<float>();
            ChainCameraOut = reader.Read<float>();
            Type = reader.Read<int>();
            SkipPointTick = reader.Read<int>();

            reader.Skip(4);
        }

        reader.ReadAtOffset(resourcesOffset, () =>
        {
            int resourceCount = reader.Read<int>();
            int allocSize = reader.Read<int>();
            reader.Skip(8);

            Resources.AddRange(reader.ReadObjectArray<DivResource>(resourceCount));
        });
    }

    public void Write(BinaryObjectWriter writer)
        => Write(writer, GameType.Common);

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        var resourcesOffsetPos = writer.Position + 4;

        writer.WriteNulls(32);
        var posStart = writer.Position;

        {
            writer.WriteNulls(8);
            writer.Write(StartFrame);
            writer.Write(EndFrame);
            writer.Write(NodeDrawCount);

            long posCutsOffset = writer.Position;
            long posPagesOffset = writer.Position + 4;
            long unknownList2OffsetPos = writer.Position + 8;
            long posResourceCutsOffset = writer.Position + 12;
            long unknownList4OffsetPos = writer.Position + 16;
            long rootNodeOffsetPos = writer.Position + 20;
            writer.WriteNulls(24);

            writer.Write(ChainCameraIn);
            writer.Write(ChainCameraOut);
            writer.Write(Type);
            writer.Write(SkipPointTick);
            writer.WriteNulls(4);

            {
                var posCuts = writer.Position;
                writer.Seek(posCutsOffset, SeekOrigin.Begin);
                writer.Write((int)(posCuts - posStart));
                writer.Seek(posCuts, SeekOrigin.Begin);

                writer.Write(Cuts.Count);
                writer.WriteNulls(12);
                writer.WriteCollection(Cuts);
            }

            {
                long posPages = writer.Position;
                writer.Seek(posPagesOffset, SeekOrigin.Begin);
                writer.Write((int)(posPages - posStart));
                writer.Seek(posPages, SeekOrigin.Begin);

                writer.Write(Pages.Count);
                long pageChunkSizePos = writer.Position;
                writer.WriteNulls(12);
                
                writer.WriteObjectCollection(Pages);

                long pageChunkEndPos = writer.Position;
                long pageChunkSize = pageChunkEndPos - (pageChunkSizePos + 12);
                writer.Seek(pageChunkSizePos, SeekOrigin.Begin);
                writer.Write((int)pageChunkSize);
                writer.Seek(pageChunkSize, SeekOrigin.Begin);
            }

            {
                long unknownList2Pos = writer.Position;
                writer.Seek(unknownList2OffsetPos, SeekOrigin.Begin);
                writer.Write((int)(unknownList2Pos - posStart));
                writer.Seek(unknownList2Pos, SeekOrigin.Begin);

                writer.WriteNulls(16);
            }

            {
                long posResourceCuts = writer.Position;
                writer.Seek(posResourceCutsOffset, SeekOrigin.Begin);
                writer.Write((int)(posResourceCuts - posStart));
                writer.Seek(posResourceCuts, SeekOrigin.Begin);

                writer.Write(ResourceCuts.Count);
                writer.WriteNulls(12);
                writer.WriteCollection(ResourceCuts);
            }

            {
                long unknownList4Pos = writer.Position;
                writer.Seek(unknownList4OffsetPos, SeekOrigin.Begin);
                writer.Write((int)(unknownList4Pos - posStart));
                writer.Seek(unknownList4Pos, SeekOrigin.Begin);

                writer.WriteNulls(16);
            }

            {
                var posRootNode = writer.Position;
                writer.Seek(rootNodeOffsetPos, SeekOrigin.Begin);
                writer.Write((int)(posRootNode - posStart));
                writer.Seek(posRootNode, SeekOrigin.Begin);

                RootNode.Write(writer, game);
            }
        }

        {
            long posResources = writer.Position;
            writer.Seek(resourcesOffsetPos, SeekOrigin.Begin);
            writer.Write((int)(posResources - posStart));
            writer.Seek(posResources, SeekOrigin.Begin);

            writer.Write(Resources.Count);
            writer.WriteNulls(12);

            writer.WriteObjectCollection(Resources);
        }
    }
}

public enum GameType
{ 
    Common,
    Frontiers,
    ShadowGenerations,
}