// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;

var count = 455;
var basePath = @"D:\Unpacked CPK\Sonic Generations\bb\Packed\ghz200\ghz200";
var groups = new List<TerrainGroup>(count);
for (int i = 0; i < count; i++)
{
    groups.Add(ResourceUtility.Open<TerrainGroup>(Path.Combine(basePath, $"tg-{i:0000}.terrain-group")));
}

using var tree = TerrainBlockSphereTree.Build(groups);
//using var tree = ResourceUtility.Open<TerrainBlockSphereTree>(@"D:\Unpacked CPK\Sonic Generations\bb\Packed\ghz200\ghz200\terrain-block.tbst");
tree.Write("terrain-block.tbst");
//var hasNodes = tree.Traverse(new AABB(default), (x) =>
//{
//    Console.WriteLine($"{x.Key.Center}, {x.Key.Radius} : {x.Value.GroupID}, {x.Value.SubsetID}");
//});

// Console.Read();