// Program for quickly writing temporary things
using SharpNeedle.Ninja.Csd;
int indent = 0;

var arc = ResourceUtility.Open<CsdProject>(@"ui_playscreen_ev.yncp");
arc.Endianness = Endianness.Little;
arc.Write(FileSystem.Create("ui_playscreen_ev.xncp"));


var project = arc.Project;
WriteLine($"{project.Name}:");
PushIndentation();
PrintSceneNode(project.Root);
PopIndentation();
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