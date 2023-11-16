// Program for quickly writing temporary things

using SharpNeedle.Ninja.Csd;
using SharpNeedle.Ninja.Csd.Extensions;
using SharpNeedle.SurfRide.Draw.Extensions;
using SharpNeedle.SurfRide.Draw;
using SharpNeedle.Glitter;
using SharpNeedle.HedgehogEngine.Mirage;

var sonic = ResourceUtility.Open<Model>(@"E:\SteamLibrary\steamapps\common\Sonic Lost World\disk\sonic2013_0\Sonic\chr_Sonic.model");
var supersonic = ResourceUtility.Open<Model>(@"E:\SteamLibrary\steamapps\common\Sonic Lost World\disk\sonic2013_0\Supersonic\chr_supersonic.model");

supersonic.Groups.Add(
    new()
    {
        Name = "Sonic_Mouth_L",
        Capacity = 2,
    }
);
supersonic.Groups[2].Add(supersonic.Groups[0][3]);
supersonic.Groups[0].RemoveAt(3);
supersonic.Groups[2].Add(
    new()
    {
        Material = supersonic.Groups[0][1].Material,
        MaterialName = "super_sonic_gm_body",
        BoneIndices = new byte[] { 47, 51 },
        Elements = supersonic.Groups[0][1].Elements,
        Slot = supersonic.Groups[0][1].Slot,
        Textures = supersonic.Groups[0][1].Textures,
        VertexSize = 104,
        VertexCount = 76,
        Faces = new ushort[139],
        Vertices = new byte[104 * 76]
    }
);

int faceCount = 0;
for (int i = 0; i < supersonic.Groups[0][0].Faces.Length; i++)
{
    if (supersonic.Groups[0][0].Faces[i] == ushort.MaxValue)
        continue;

    if (supersonic.Groups[0][0].Vertices[supersonic.Groups[0][0].VertexSize * supersonic.Groups[0][0].Faces[i] + 99] == 9 || supersonic.Groups[0][0].Vertices[supersonic.Groups[0][0].VertexSize * supersonic.Groups[0][0].Faces[i] + 99] == 10)
    {
        supersonic.Groups[2][1].Faces[faceCount] = (ushort)(supersonic.Groups[0][0].Faces[i] - 254);
        supersonic.Groups[0][0].Faces[i] = ushort.MaxValue;
        faceCount++;
    }
}

int vertexCount = 0;
for (int i = 0; i < supersonic.Groups[0][0].Vertices.Count(); i += (int)supersonic.Groups[0][0].VertexSize)
{
    if (supersonic.Groups[0][0].Vertices[i + 99] == 9 || supersonic.Groups[0][0].Vertices[i + 99] == 10)
    {
        for (int j = 0; j < supersonic.Groups[0][0].VertexSize; j++)
        {
            if (j == 99)
            {
                supersonic.Groups[2][1].Vertices[supersonic.Groups[2][1].VertexSize * vertexCount + j] = (byte)(supersonic.Groups[0][0].Vertices[i + j] - 9);
            }
            else
            {
                supersonic.Groups[2][1].Vertices[supersonic.Groups[2][1].VertexSize * vertexCount + j] = supersonic.Groups[0][0].Vertices[i + j];
            }
        }

        vertexCount++;
    }
}

supersonic.Root = null;
//supersonic.Write("chr_supersonic.model");

/*var csd = ResourceUtility.Open<CsdProject>(@"E:\SteamLibrary\steamapps\common\Sonic Generations\disk\bb3\SonicActionCommonHud\ui_gameplay.xncp");

var srd = csd.ToSrdProject();

var infoRing = srd.Project.GetScene("info_ring").GetLayer("Layer_001");
infoRing.SwapParentWithChild(infoRing.GetCast("icon_ring"), infoRing.GetCast("shade"));

var infoLife = srd.Project.GetScene("info_life").GetLayer("Layer_001");
infoLife.SwapParentWithChild(infoLife.GetCast("icon_generic"), infoLife.GetCast("Cast_0237"));
infoLife.SwapParentWithChild(infoLife.GetCast("icon_classic"), infoLife.GetCast("Cast_0238"));
//infoLife.GetCast("Cast_0238").Cell.IsVisible = false;
//infoLife.GetCast("icon_classic").Cell.IsVisible = false;
srd.Write("ui_gameplay.swif");*/

Console.ReadKey();
