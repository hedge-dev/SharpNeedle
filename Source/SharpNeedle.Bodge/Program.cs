// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;

var instanceInfo = ResourceUtility.Open<TerrainInstanceInfo>(@"evil_euc03_ter_dml_clocktower_001_1.terrain-instanceinfo");
instanceInfo.Write("lol.terrain-instanceinfo");
Console.WriteLine(instanceInfo.Name);