// Program for quickly writing temporary things
using SharpNeedle.LostWorld;
using SharpNeedle.LostWorld.Animation;

//var res = new BinaryResource<BinaryPrimitive<int>>();
//res.Read(FileSystem.Open(@"D:\Unpacked CPK\Sonic Lost World\sonic2013_0\set\batl01_obj_00.orc"));

var model = ResourceUtility.Open<ShadowModel>(@"D:\Games\SteamLibrary\steamapps\common\Sonic Lost World\mods\Link Sonic\disk\sonic2013_patch_0\Sonic\chr_Sonic.shadow-model");
model.Write(FileSystem.Create("chr_Sonic.shadow-model"));
Console.Read();