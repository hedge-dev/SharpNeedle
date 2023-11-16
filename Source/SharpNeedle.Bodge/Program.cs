// Program for quickly writing temporary things

using SharpNeedle.Ninja.Csd;
using SharpNeedle.Ninja.Csd.Extensions;
using SharpNeedle.SurfRide.Draw.Extensions;

var csd = ResourceUtility.Open<CsdProject>(@"E:\SteamLibrary\steamapps\common\Sonic Generations\disk\bb3\SonicActionCommonHud\ui_gameplay.xncp");

var srd = csd.ToSrdProject();

var infoRing = srd.Project.GetScene("info_ring").GetLayer("Layer_001");
infoRing.SwapParentWithChild(infoRing.GetCast("icon_ring"), infoRing.GetCast("shade"));

var infoLife = srd.Project.GetScene("info_life").GetLayer("Layer_001");
infoLife.SwapParentWithChild(infoLife.GetCast("icon_generic"), infoLife.GetCast("Cast_0237"));
infoLife.SwapParentWithChild(infoLife.GetCast("icon_classic"), infoLife.GetCast("Cast_0238"));
//infoLife.GetCast("Cast_0238").Cell.IsVisible = false;
//infoLife.GetCast("icon_classic").Cell.IsVisible = false;
srd.Write("ui_gameplay.swif");

Console.ReadKey();
