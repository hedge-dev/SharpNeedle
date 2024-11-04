namespace SharpNeedle.SonicTeam.DivScene;

public class DivPUnknown : IDivDataBlock
{
    public byte[] Data { get; set; }
    public int Size { get; set; }

    public DivPUnknown() { }
    public DivPUnknown(BinaryObjectReader reader, GameType game, int size) 
    {
        Size = size;
        Data = new byte[Size * 4];

        Read(reader, game);
    }

    public void Read(BinaryObjectReader reader, GameType game) 
    {
        reader.ReadArray<byte>(Size * 4, Data);
    }

    public void Write(BinaryObjectWriter writer, GameType game) 
    {
        writer.WriteArrayFixedLength(Data, Size * 4);
    }
}

public class DivPCullDisabled : IDivDataBlock
{
    public DivPCullDisabled() { }
    public DivPCullDisabled(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game) { }

    public void Write(BinaryObjectWriter writer, GameType game) { }
}

public class DivPMovieDisplay : IDivDataBlock
{
    public DivPMovieDisplay() { }
    public DivPMovieDisplay(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game) { }

    public void Write(BinaryObjectWriter writer, GameType game) { }
}

class DivPDrawingOff : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DivPDrawingOff() { }
    public DivPDrawingOff(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
    }
}

public class DivPFade : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPFade() { }
    public DivPFade(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPEffect : IDivDataBlock
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public string Name { get; set; }
    public int Field84 { get; set; }
    public int Field88 { get; set; }
    public int Field8C { get; set; }
    public int Field90 { get; set; }
    public int Field94 { get; set; }
    public int Field98 { get; set; }
    public int Field9C { get; set; }
    public int FieldA0 { get; set; }
    public float[] FieldA4 { get; set; } = new float[128];

    public DivPEffect() { }
    public DivPEffect(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Name = reader.ReadDivString(64);
        Field84 = reader.Read<int>();
        Field88 = reader.Read<int>();
        Field8C = reader.Read<int>();
        Field90 = reader.Read<int>();
        Field94 = reader.Read<int>();
        Field98 = reader.Read<int>();
        Field9C = reader.Read<int>();
        FieldA0 = reader.Read<int>();
        reader.ReadArray<float>(128, FieldA4);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.WriteDivString(Name);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
        writer.Write(Field90);
        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(FieldA0);
        writer.WriteArrayFixedLength(FieldA4, 128);
    }
}

public class DivPLetterbox : IDivDataBlock
{
    public float[] Values { get; set; } = new float[32];

    public DivPLetterbox() { }
    public DivPLetterbox(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        reader.ReadArray<float>(32, Values);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteArrayFixedLength(Values, 32);
    }
}

public class DivPAnimUV : IDivDataBlock
{
    public int Field00 { get; set; }
    public string Name { get; set; }
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public DivPAnimUV() { }
    public DivPAnimUV(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDivString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDivString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }
}

public class DivPAnimMaterial : IDivDataBlock
{
    public int Field00 { get; set; }
    public string Name { get; set; }
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public DivPAnimMaterial() { }
    public DivPAnimMaterial(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDivString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDivString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }
}

public class DivPChromaAberr : IDivDataBlock
{
    public float Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPChromaAberr() { }
    public DivPChromaAberr(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<float>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<float>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<float>();
        Field3C = reader.Read<float>();
        Field40 = reader.Read<float>();

        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);

        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPSound : IDivDataBlock
{
    public string CueName { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }

    public DivPSound() { }
    public DivPSound(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        CueName = reader.ReadDivString(64);
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteDivString(CueName);
        writer.Write(Field40);
        writer.Write(Field44);
    }
}

public class DivPGameCamera : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public float Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public float NearCullingPlane { get; set; }
    public float FarCullingPlane { get; set; }
    public float Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public int Field64 { get; set; }

    public DivPGameCamera() { }
    public DivPGameCamera(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<float>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<int>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        NearCullingPlane = reader.Read<float>();
        FarCullingPlane = reader.Read<float>();
        Field58 = reader.Read<float>();
        Field5C = reader.Read<int>();
        Field60 = reader.Read<int>();
        Field64 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(NearCullingPlane);
        writer.Write(FarCullingPlane);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);
        writer.Write(Field64);
    }
}

public class DivPSubtitle : IDivDataBlock
{
    public string CellName { get; set; }
    public SubtitleLanguage Language { get; set; }
    public int Field14 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public string CellName2 { get; set; }

    public DivPSubtitle() { }
    public DivPSubtitle(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        if(game == GameType.ShadowGenerations)
        {
            CellName = reader.ReadDivString(32);
            Language = (SubtitleLanguage)reader.Read<int>();
            Field14 = reader.Read<int>();
            Field24 = reader.Read<int>();
            Field28 = reader.Read<int>();
            CellName2 = reader.ReadDivString(32);
        }
        else
        {
            CellName = reader.ReadDivString(16);
            Language = (SubtitleLanguage)reader.Read<int>();
            Field14 = reader.Read<int>();
        }
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        if (game == GameType.ShadowGenerations)
        {
            writer.WriteDivString(CellName, 32);
            writer.Write((int)Language);
            writer.Write(Field14);
            writer.Write(Field24);
            writer.Write(Field28);
            writer.WriteDivString(CellName2, 32);
        }
        else
        {
            writer.WriteDivString(CellName, 16);
            writer.Write((int)Language);
            writer.Write(Field14);
        }
    }

    public enum SubtitleLanguage
    {
        English = 0,
        French = 1,
        Italian = 2,
        German = 3,
        Spanish = 4,
        Polish = 5,
        Portuguese = 6,
        Russian = 7,
        Japanese = 8,
        Chinese = 9,
        ChineseSimplified = 10,
        Korean = 11
    }
}

public class DivPQTE : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public int Field64 { get; set; }
    public int Field68 { get; set; }
    public int Field6C { get; set; }
    public float Field70 { get; set; }
    public float Field74 { get; set; }
    public float Field78 { get; set; }
    public float Field7C { get; set; }
    public int[] Field80 { get; set; } = new int[48];
    public string Field140 { get; set; }

    public DivPQTE() { }
    public DivPQTE(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<int>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
        Field54 = reader.Read<int>();
        Field58 = reader.Read<int>();
        Field5C = reader.Read<int>();
        Field60 = reader.Read<int>();
        Field64 = reader.Read<int>();
        Field68 = reader.Read<int>();
        Field6C = reader.Read<int>();
        Field70 = reader.Read<float>();
        Field74 = reader.Read<float>();
        Field78 = reader.Read<float>();
        Field7C = reader.Read<float>();
        reader.ReadArray<int>(48, Field80);

        Field140 = reader.ReadDivString(64);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
        writer.Write(Field54);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);
        writer.Write(Field64);
        writer.Write(Field68);
        writer.Write(Field6C);
        writer.Write(Field70);
        writer.Write(Field74);
        writer.Write(Field78);
        writer.Write(Field7C);
        writer.WriteArrayFixedLength(Field80, 48);

        writer.WriteDivString(Field140, 64);
    }
}

public class DivPTimescale : IDivDataBlock
{
    public int Field00 { get; set; }
    public float Scale { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DivPTimescale() { }
    public DivPTimescale(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Scale = reader.Read<float>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Scale);
        writer.Write(Field08);
        writer.Write(Field0C);
    }
}

public class DivPVignette : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float Field44 { get; set; }
    public float Field48 { get; set; }
    public float Field4C { get; set; }
    public float Field50 { get; set; }
    public float Field54 { get; set; }
    public float Field58 { get; set; }
    public float Field5C { get; set; }
    public float Field60 { get; set; }
    public float Field64 { get; set; }
    public float Field68 { get; set; }
    public float Field6C { get; set; }
    public float Field70 { get; set; }
    public float Field74 { get; set; }
    public float Field78 { get; set; }
    public float Field7C { get; set; }
    public float Field80 { get; set; }
    public float Field84 { get; set; }
    public int Field88 { get; set; }
    public float Field8C { get; set; }
    public float Field90 { get; set; }
    public float Field94 { get; set; }
    public float Field98 { get; set; }
    public float Field9C { get; set; }
    public float FieldA0 { get; set; }
    public float FieldA4 { get; set; }
    public float FieldA8 { get; set; }
    public float FieldAC { get; set; }
    public float FieldB0 { get; set; }
    public float FieldB4 { get; set; }
    public float FieldB8 { get; set; }
    public float FieldBC { get; set; }
    public float FieldC0 { get; set; }
    public float FieldC4 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPVignette() { }
    public DivPVignette(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<float>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<float>();
        Field3C = reader.Read<float>();
        Field40 = reader.Read<float>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<float>();
        Field50 = reader.Read<float>();
        Field54 = reader.Read<float>();
        Field58 = reader.Read<float>();
        Field5C = reader.Read<float>();
        Field60 = reader.Read<float>();
        Field64 = reader.Read<float>();
        Field68 = reader.Read<float>();
        Field6C = reader.Read<float>();
        Field70 = reader.Read<float>();
        Field74 = reader.Read<float>();
        Field78 = reader.Read<float>();
        Field7C = reader.Read<float>();
        Field80 = reader.Read<float>();
        Field84 = reader.Read<float>();
        Field88 = reader.Read<int>();
        Field8C = reader.Read<float>();
        Field90 = reader.Read<float>();
        Field94 = reader.Read<float>();
        Field98 = reader.Read<float>();
        Field9C = reader.Read<float>();
        FieldA0 = reader.Read<float>();
        FieldA4 = reader.Read<float>();
        FieldA8 = reader.Read<float>();
        FieldAC = reader.Read<float>();
        FieldB0 = reader.Read<float>();
        FieldB4 = reader.Read<float>();
        FieldB8 = reader.Read<float>();
        FieldBC = reader.Read<float>();
        FieldC0 = reader.Read<float>();
        FieldC4 = reader.Read<float>();

        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
        writer.Write(Field54);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);
        writer.Write(Field64);
        writer.Write(Field68);
        writer.Write(Field6C);
        writer.Write(Field70);
        writer.Write(Field74);
        writer.Write(Field78);
        writer.Write(Field7C);
        writer.Write(Field80);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
        writer.Write(Field90);
        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(FieldA0);
        writer.Write(FieldA4);
        writer.Write(FieldA8);
        writer.Write(FieldAC);
        writer.Write(FieldB0);
        writer.Write(FieldB4);
        writer.Write(FieldB8);
        writer.Write(FieldBC);
        writer.Write(FieldC0);
        writer.Write(FieldC4);

        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPPathAdjust : IDivDataBlock
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivPPathAdjust() { }
    public DivPPathAdjust(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DivPBossCutoff : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }

    public DivPBossCutoff() { }
    public DivPBossCutoff(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
    }
}

public class DivPAura : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float Field44 { get; set; }
    public int Field48 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPAura() { }
    public DivPAura(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<float>();
        Field3C = reader.Read<float>();
        Field40 = reader.Read<float>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<int>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPDOF : IDivDataBlock
{
    public int Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPDOF() { }
    public DivPDOF(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPTheEndCable : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float[] Field08 { get; set; } = new float[1024];

    public DivPTheEndCable() { }
    public DivPTheEndCable(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        reader.ReadArray<float>(1024, Field08);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.WriteArrayFixedLength(Field08, 1024);
    }
}

public class DivPCompositeAnim : IDivDataBlock
{
    public int Field00 { get; set; }
    public string Field04 { get; set; }
    public Animation[] Animations { get; set; } = new Animation[16];
    public int Field450 { get; set; }

    public DivPCompositeAnim() { }
    public DivPCompositeAnim(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.ReadString(StringBinaryFormat.FixedLength, 12);
        Animations = reader.ReadObjectArray<Animation>(16);
        Field450 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, Field04, 12);
        writer.WriteObjectCollection(Animations);
        writer.Write(Field450);
    }

    public class Animation : IBinarySerializable
    {
        public int Type { get; set; }
        public string Name { get; set; }

        public Animation() 
        {
            Type = (int)AnimationType.None;
            Name = "";
        }

        public void Read(BinaryObjectReader reader)
        {
            Type = reader.Read<int>();
            Name = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Type);
            writer.WriteString(StringBinaryFormat.FixedLength, Name, 64);
        }
    }

    public enum AnimationType
    {
        None = 0,
        PXD = 1,
        UV = 2
    }
}

public class DivPAnimStateMachine : IDivDataBlock
{
    public string Field00 { get; set; }
    public string Field40 { get; set; }

    public DivPAnimStateMachine() { }
    public DivPAnimStateMachine(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field40 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, Field00, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Field40, 64);
    }
}
public class DivPGeneralTrigger : IDivDataBlock
{
    public uint Field00 { get; set; }
    public string TriggerName { get; set; }

    public DivPGeneralTrigger() { }
    public DivPGeneralTrigger(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        TriggerName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, TriggerName, 64);
    }
}

public class DivPCameraBlur : IDivDataBlock
{
    public uint Field00 { get; set; }
    public uint Field04 { get; set; }
    public float Field08 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];
    public uint Flags { get; set; }

    public DivPCameraBlur() { }
    public DivPCameraBlur(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<uint>();
        Field08 = reader.Read<float>();
        reader.ReadArray<float>(32, ValuesTimeline);
        Flags = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
        writer.Write(Flags);
    }
}   

public class DivPShadowRes : IDivDataBlock
{
    public Vector2Int Resolution { get; set; }

    public DivPShadowRes() { }
    public DivPShadowRes(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Resolution = reader.Read<Vector2Int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Resolution);
    }
}

public class DivPTime : IDivDataBlock
{
    public uint Field00 { get; set; }
    public uint Field04 { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }
    public uint Field14 { get; set; }
    public uint Field18 { get; set; }
    public uint Field1C { get; set; }
    public uint Field20 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPTime() { }
    public DivPTime(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<uint>();
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<uint>();
        Field1C = reader.Read<uint>();
        Field20 = reader.Read<uint>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPWeather : IDivDataBlock
{
    public uint Field00 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPWeather() { }
    public DivPWeather(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPFinalBossLighting : IDivDataBlock
{
    public DivPFinalBossLighting() { }
    public DivPFinalBossLighting(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game) { }

    public void Write(BinaryObjectWriter writer, GameType game) { }
}

public class DivPCyberNoise : IDivDataBlock
{
    public uint Field00 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPCyberNoise() { }
    public DivPCyberNoise(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game) 
    {
        Field00 = reader.Read<uint>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game) 
    {
        writer.Write(Field00);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPDitherDepth : IDivDataBlock
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }

    public DivPDitherDepth() { }
    public DivPDitherDepth(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
    }
}

public class DivPCameraExposure : IDivDataBlock
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }
    public uint Field14 { get; set; }
    public uint Field18 { get; set; }
    public uint Field1C { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPCameraExposure() { }
    public DivPCameraExposure(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<uint>();
        Field1C = reader.Read<uint>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPColorCorrection : IDivDataBlock
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public uint Field14 { get; set; }
    public float Field18 { get; set; }
    public uint Field1C { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public DivPColorCorrection() { }
    public DivPColorCorrection(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<uint>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }
}

public class DivPMaterialParam : IDivDataBlock
{
    public string MaterialName { get; set; }
    public string ParamName { get; set; }
    public uint Type { get; set; }
    public uint[] UnknownData { get; set; } = new uint[40];

    public DivPMaterialParam() { }
    public DivPMaterialParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        MaterialName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        ParamName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Type = reader.Read<uint>();
        reader.ReadArray<uint>(40, UnknownData);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, MaterialName, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, ParamName, 64);
        writer.Write(Type);
        writer.WriteArrayFixedLength(UnknownData, 40);
    }

    public enum ParamType
    { 
        Float = 3
    }
}

public class DivPTimeStop : IDivDataBlock
{
    public int Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }

    public DivPTimeStop() { }
    public DivPTimeStop(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
    }

    public enum ParamType
    {
        Float = 3
    }
}

// < 1000 values appear to be shared between games, the others are game-specific.
public enum ParameterType
{
    ParameterSpecifiedCamera = 1,
    DrawingOff = 3,
    PathAdjust = 5,
    CameraShake = 6,
    CameraShakeLoop = 7,
    Effect = 8,
    PathInterpolation = 10,
    CullDisabled = 11,
    NearFarSetting = 12,
    UVAnimation = 13,
    VisibilityAnimation = 14,
    MaterialAnimation = 15,
    CompositeAnimation = 16,
    CameraOffset = 17,
    ModelFade = 18,
    SonicCamera = 20,
    GameCamera = 21,
    PointLightSource = 22,
    ControllerVibration = 25,
    SpotlightModel = 26,
    MaterialParameter = 27,
}

enum FrontiersParams
{
    DepthOfField = 1001,
    ColorCorrection = 1002,
    CameraExposure = 1003,
    ShadowResolution = 1004,
    HeightFog = 1007,
    ChromaticAberration = 1008,
    Vignette = 1009,
    Fade = 1010,
    Letterbox = 1011,
    ModelClipping = 1012,
    BossCutoff = 1014,
    Subtitle = 1015,
    Sound = 1016,
    Time = 1017,
    LookAtIK = 1019,
    CameraBlur = 1020,
    GeneralPurposeTrigger = 1021,
    DitherDepth = 1023,
    QTE = 1024,
    FacialAnimation = 1025,
    ASMForcedOverwrite = 1026,
    Aura = 1027,
    TimescaleChange = 1028,
    CyberNoise = 1029,
    AuraRoad = 1031,
    MovieDisplay = 1032,
    Weather = 1034,
    PointLightSource = 1036,
    TheEndCable = 1042,
    FinalBossLighting = 1043
}

enum ShadowGensParams
{
    DepthOfField = 1001,
    HeightFog = 1009,
    ChromaticAberration = 1010,
    Vignette = 1011,
    Fade = 1012,
    BossCutoff = 1016,
    Subtitle = 1017,
    Sound = 1018,
    CameraBlur = 1022,
    GeneralPurposeTrigger = 1023,
    QTE = 1026,
    TimescaleChange = 1030,
    TimeStop = 1041,
    DepthOfFieldNew = 1047,
}