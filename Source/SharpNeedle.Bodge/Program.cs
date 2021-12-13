// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;
var info = ResourceUtility.Open<GITextureGroupInfo>(@"D:\Unpacked CPK\Sonic Generations\bb\Packed\ghz200\ghz200\gi-texture.gi-texture-group-info");
info.Write("gi-texture.gi-texture-group-info");
// Console.Read();