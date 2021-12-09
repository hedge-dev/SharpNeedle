// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;
var mat = ResourceUtility.Open<Material>(@"D:\Games\SteamLibrary\steamapps\common\Sonic Generations\Mods\BetterFxPipeline\disk\bb3\Sonic\sonic_gm_body.material");
//mat1.Write($@"E:\Downloads\SonicEVRoot\saved\{mat1.Name}.material");
mat.DataVersion = 1;
mat.Save();