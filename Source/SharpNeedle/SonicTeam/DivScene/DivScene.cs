namespace SharpNeedle.SonicTeam.DivScene;

using System.IO;

// Reference: https://github.com/ik-01/DiEventRangers/tree/main
[NeedleResource("st/dv-scene", @"\.dvscene$")]
public class DivScene : ResourceBase, IBinarySerializable
{
    public float Duration { get; set; }
    public int Field10 { get; set; }
    public List<float> UnknownList0 { get; set; } = new();
    public List<DivPage> Pages { get; set; } = new();
    public List<float> UnknownList1 { get; set; } = new();
    public DivNode RootNode { get; set; } = new();
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public List<DivResource> Resources { get; set; } = new();

    public DivScene() 
    {
        // Register encoding provider for Shift-JIS strings
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public override void Read(IFile file)
        => Read(file, GameType.Common);

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
        // Header
        long dataOffset = reader.ReadOffsetValue();
        long resourcesOffset = reader.ReadOffsetValue();
        reader.Skip(24);

        reader.PushOffsetOrigin();

        reader.Seek(dataOffset + 32, SeekOrigin.Begin);
        {
            reader.Skip(12);

            Duration = reader.Read<float>();
            Field10 = reader.Read<int>();

            reader.ReadOffset(() =>
            {
                int triggerCount = reader.Read<int>();
                reader.Skip(12);

                reader.ReadCollection(triggerCount, UnknownList0);
            });

            reader.ReadOffset(() =>
            {
                int pageCount = reader.Read<int>();
                int pageChunkSize = reader.Read<int>();

                reader.Skip(8);

                Pages.AddRange(reader.ReadObjectArray<DivPage>(pageCount));
            });

            reader.ReadOffset(() =>
            {
                int unknownCount = reader.Read<int>();
                reader.Skip(12);
            });

            reader.ReadOffset(() =>
            {
                int unknownCount = reader.Read<int>();
                reader.Skip(12);

                reader.ReadCollection(unknownCount, UnknownList1);
            });

            reader.ReadOffset(() =>
            {
                int unknownCount = reader.Read<int>();
                reader.Skip(12);
            });

            reader.ReadOffset(() =>
            {
                RootNode.Read(reader, game);
            });

            Field2C = reader.Read<float>();
            Field30 = reader.Read<float>();
            reader.Skip(12);
        }

        reader.ReadAtOffset(resourcesOffset, () =>
        {
            int resourceCount = reader.Read<int>();
            reader.Skip(12);

            Resources.AddRange(reader.ReadObjectArray<DivResource>(resourceCount));
        });
    }

    public void Write(BinaryObjectWriter writer)
        => Write(writer, GameType.Common);

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        long resourcesOffsetPos = writer.Position + 4;

        writer.WriteNulls(32);
        long dataPos = writer.Position;

        {
            writer.WriteNulls(12);
            writer.Write(Duration);
            writer.Write(Field10);

            long unknownList0OffsetPos = writer.Position;
            long pagesOffsetPos = writer.Position + 4;
            long unknownList2OffsetPos = writer.Position + 8;
            long unknownList3OffsetPos = writer.Position + 12;
            long unknownList4OffsetPos = writer.Position + 16;
            long rootNodeOffsetPos = writer.Position + 20;
            writer.WriteNulls(24);

            writer.Write(Field2C);
            writer.Write(Field30);
            writer.WriteNulls(12);

            {
                long unknownList0Pos = writer.Position;
                writer.Seek(unknownList0OffsetPos, SeekOrigin.Begin);
                writer.Write((int)(unknownList0Pos - dataPos));
                writer.Seek(unknownList0Pos, SeekOrigin.Begin);

                writer.Write(UnknownList0.Count);
                writer.WriteNulls(12);
                writer.WriteCollection(UnknownList0);
            }

            {
                long pagesPos = writer.Position;
                writer.Seek(pagesOffsetPos, SeekOrigin.Begin);
                writer.Write((int)(pagesPos - dataPos));
                writer.Seek(pagesPos, SeekOrigin.Begin);

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
                writer.Write((int)(unknownList2Pos - dataPos));
                writer.Seek(unknownList2Pos, SeekOrigin.Begin);

                writer.WriteNulls(16);
            }

            {
                long unknownList3Pos = writer.Position;
                writer.Seek(unknownList3OffsetPos, SeekOrigin.Begin);
                writer.Write((int)(unknownList3Pos - dataPos));
                writer.Seek(unknownList3Pos, SeekOrigin.Begin);

                writer.Write(UnknownList1.Count);
                writer.WriteNulls(12);
                writer.WriteCollection(UnknownList1);
            }

            {
                long unknownList4Pos = writer.Position;
                writer.Seek(unknownList4OffsetPos, SeekOrigin.Begin);
                writer.Write((int)(unknownList4Pos - dataPos));
                writer.Seek(unknownList4Pos, SeekOrigin.Begin);

                writer.WriteNulls(16);
            }

            {
                long rootNodePos = writer.Position;
                writer.Seek(rootNodeOffsetPos, SeekOrigin.Begin);
                writer.Write((int)(rootNodePos - dataPos));
                writer.Seek(rootNodePos, SeekOrigin.Begin);

                RootNode.Write(writer, game);
            }
        }

        {
            long resourcesPos = writer.Position;
            writer.Seek(resourcesOffsetPos, SeekOrigin.Begin);
            writer.Write((int)(resourcesPos - dataPos));
            writer.Seek(resourcesPos, SeekOrigin.Begin);

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