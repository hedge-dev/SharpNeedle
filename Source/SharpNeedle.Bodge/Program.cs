// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;

var parameters = new ShaderParameter();
parameters.Read(@"SurfRide3DTransform.vsparam");
parameters.AddFloat4Usage("g_Screen_Size", 0xF4);
parameters.Save();

foreach (var usage in parameters.AllUsages())
{
    Console.WriteLine($"{usage} : {usage.Type.ToString().ToLower()}");
}
