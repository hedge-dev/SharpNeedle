// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;

var gil = ResourceUtility.Open<GIMipLevelLimitation>(@"D:\Unpacked CPK\Sonic Generations\bb\Packed\ghz200\ghz200\gi-lim.gil");
Console.WriteLine(gil);
gil.Write("gi-lim.gil");