// Program for quickly writing temporary things
using SharpNeedle.LostWorld.Animation;

//var res = new BinaryResource<BinaryPrimitive<int>>();
//res.Read(FileSystem.Open(@"D:\Unpacked CPK\Sonic Lost World\sonic2013_0\set\batl01_obj_00.orc"));

var model = ResourceUtility.Open<CharAnimScript>(@"player_sonic.anm");
model.Version = new(0, 0, 1, Endianness.Little);

model.Write(FileSystem.Create("player_sonic.anm"));
Console.Read();