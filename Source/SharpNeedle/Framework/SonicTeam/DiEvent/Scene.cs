namespace SharpNeedle.Framework.SonicTeam.DiEvent;

// Reference: https://github.com/ik-01/DiEventRangers/tree/main
[NeedleResource("st/dv-scene", @"\.dvscene$")]
public class Scene : ResourceBase, IBinarySerializable<GameType>
{
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public int NodeDrawCount { get; set; }
    public List<float> Cuts { get; set; } = [];
    public List<Page> Pages { get; set; } = [];
    public List<float> ResourceCuts { get; set; } = [];
    public Node RootNode { get; set; } = new();
    public float ChainCameraIn { get; set; }
    public float ChainCameraOut { get; set; }
    int Type { get; set; }
    int SkipPointTick { get; set; }

    public List<Resource> Resources { get; set; } = [];

    public Scene()
    {
        // Register encoding provider for Shift-JIS strings
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public override void Read(IFile file)
    {
        Read(file, GameType.Common);
    }

    public void Read(string path, GameType game)
    {
        IFile? file = FileSystem.Instance.Open(path) 
            ?? throw new FileNotFoundException($"Could not open file \"{path}\"", path);

        Read(file, game);
    }

    public void Read(IFile file, GameType game)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);

        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        reader.OffsetBinaryFormat = OffsetBinaryFormat.U32;

        Read(reader, game);
    }

    public override void Write(IFile file)
    {
        Write(file, GameType.Common);
    }

    public void Write(IFile file, GameType game)
    {
        BaseFile = file;

        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);
        writer.OffsetBinaryFormat = OffsetBinaryFormat.U32;

        Write(writer, game);
    }

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

                Pages.AddRange(reader.ReadObjectArray<Page>(pageCount));
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

            reader.ReadOffset(() => RootNode.Read(reader, game));

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

            Resources.AddRange(reader.ReadObjectArray<Resource>(resourceCount));
        });
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        long resourcesOffsetPos = writer.Position + 4;

        writer.WriteNulls(32);
        long posStart = writer.Position;

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
                long posCuts = writer.Position;
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
                long pageAllocSizePos = writer.Position;
                writer.WriteNulls(12);

                long pageChunkStartPos = writer.Position;
                writer.WriteObjectCollection(Pages);
                long pageChunkEndPos = writer.Position;

                long pageChunkSize = pageChunkEndPos - pageChunkStartPos;
                writer.Seek(pageAllocSizePos, SeekOrigin.Begin);
                writer.Write((int)pageChunkSize);

                writer.Seek(pageChunkEndPos, SeekOrigin.Begin);
            }

            {
                long posUnknownList2 = writer.Position;
                writer.Seek(unknownList2OffsetPos, SeekOrigin.Begin);
                writer.Write((int)(posUnknownList2 - posStart));

                writer.Seek(posUnknownList2, SeekOrigin.Begin);
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
                long posRootNode = writer.Position;
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