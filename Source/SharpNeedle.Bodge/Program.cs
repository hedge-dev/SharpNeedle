// Program for quickly writing temporary things
using SharpNeedle.SonicTeam.DiEvent;

int a = 0;

void WriteUnknownParams(Node node)
{
    if(node.Type == (int)NodeType.Parameter)
    {
        ParameterData? data = node.Data as ParameterData;
        UnknownParam? param = data.Parameter as UnknownParam;

        if (param != null)
        {
            Console.WriteLine("{0}: {1}", node.Name, param.Type);

            var f = new BinaryWriter(File.Open(string.Format("{0}__param_{1}.bin", a, param.Type), FileMode.Create));
            f.Write(param.Data);
            f.Close();
            a += 1;
        }
    }

    foreach(var c in node.Children)
    {
        WriteUnknownParams(c);
    }
}

Scene dievent = new Scene();
dievent.Read(@"bo1140.dvscene", GameType.Frontiers);

Console.OutputEncoding = Encoding.GetEncoding(932);

WriteUnknownParams(dievent.RootNode);

dievent.Write("outputtest.dvscene", GameType.ShadowGenerations);