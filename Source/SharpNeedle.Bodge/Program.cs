// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine;
using SharpNeedle.HedgehogEngine.Mirage;

using var group = ResourceUtility.Open<TerrainGroup>(@"tg-0002.terrain-group");
var instance = group.GetInstance(new Vector3(1714.7402f, 24.829796f, -2695.4512f));
Console.Read();