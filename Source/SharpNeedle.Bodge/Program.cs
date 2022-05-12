// Program for quickly writing temporary things
using System.Diagnostics;
using SharpNeedle.Ninja.Csd;
int indent = 0;

var watch = Stopwatch.StartNew();

var shadowCsd = ResourceUtility.Open<CsdProject>(@"Shadow\files\csdFiles\Logo.gncp");
watch.Stop();

Console.WriteLine(watch.Elapsed);

shadowCsd.Project.TextureFormat = TextureFormat.DirectX;
shadowCsd.Textures = new TextureListChunk
{
    new ("000_sega.dds"),
    new ("001_sonicteam.dds"),
    new ("002_sega_jp.dds"),
    new ("003_dolby_gc.dds")
};

shadowCsd.Endianness = Endianness.Little;
shadowCsd.Write(FileSystem.Create("Logo.xncp"));

var project = shadowCsd.Project;

WriteLine($"{project.Name}:");
PushIndentation();
PrintSceneNode(project.Root);
PopIndentation();

if (shadowCsd.Textures != null)
{
    WriteLine("Textures:");
    PushIndentation();
    
    foreach (var texture in shadowCsd.Textures)
        WriteLine(texture.Name);

    PopIndentation();
}

Console.Read();

void PrintSceneNode(SceneNode node)
{
    foreach (var scenePair in node.Scenes)
    {
        WriteLine($"{scenePair.Key}:");
        PushIndentation();
        int i = 0;
        foreach (var group in scenePair.Value.Layers)
        {
            WriteLine($"Layer_{i++}:");
            PushIndentation();
            foreach (var cast in group)
                PrintCast(cast);

            PopIndentation();
            Console.WriteLine();
        }
        PopIndentation();

        void PrintCast(Cast cast)
        {
            WriteLine($"{cast.Name}{(cast.Count == 0 ? "" : ":")}");
            PushIndentation();

            foreach (var child in cast)
                PrintCast(child);

            PopIndentation();
        }
    }

    foreach (var child in node.Children)
    {
        WriteLine($"{child.Key}:");
        PushIndentation();
        PrintSceneNode(child.Value);
        PopIndentation();
    }
}

Console.WriteLine();

void ApplyIndentation()
{
    for (int i = 0; i < indent; i++)
        Console.Write("  ");
}

void Write(string text)
{
    ApplyIndentation();
    Console.Write(text);
}

void WriteLine(string text)
{
    ApplyIndentation();
    Console.WriteLine(text);
}

void PushIndentation() => ++indent;
void PopIndentation() => --indent;