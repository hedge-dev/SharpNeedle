// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;
var pfi = ResourceUtility.Open<PackedFileInfo>(@"D:\Unpacked CPK\Sonic Generations\bb\Packed\ghz200\ghz200\Stage.pfi");
pfi.Write("Stage.pfi");