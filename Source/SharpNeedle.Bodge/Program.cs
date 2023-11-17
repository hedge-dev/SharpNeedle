// Program for quickly writing temporary things

using SharpNeedle.Ninja.Csd;
using SharpNeedle.Ninja.Csd.Extensions;
using SharpNeedle.SurfRide.Draw.Extensions;

var csd = ResourceUtility.Open<CsdProject>(@"E:\SteamLibrary\steamapps\common\Sonic Generations\disk\bb3\SonicActionCommonHud\ui_gameplay.xncp");

var srd = csd.ToSrdProject();

var infoLife = srd.Project.GetScene("info_life").GetLayer("Layer_001");
infoLife.SwapParentWithChild(infoLife.GetCast("icon_generic"), infoLife.GetCast("Cast_0237"));
infoLife.SwapParentWithChild(infoLife.GetCast("icon_classic"), infoLife.GetCast("Cast_0238"));
//infoLife.GetCast("Cast_0238").Cell.IsVisible = false;
//infoLife.GetCast("icon_classic").Cell.IsVisible = false;

var title30sec = srd.Project.GetScene("30sec_title").GetLayer("Layer_001");
title30sec.SwapParentWithChild(title30sec.GetCast("word_30sec"), title30sec.GetCast("shade"));

var infoCustomBlb = srd.Project.GetScene("info_custom_blb").GetLayer("Layer_001");
infoCustomBlb.SwapParentWithChild(infoCustomBlb.GetCast("icon_cl"), infoCustomBlb.Casts[7]);
infoCustomBlb.SwapParentWithChild(infoCustomBlb.GetCast("icon_gn"), infoCustomBlb.Casts[8]);

var infoRing = srd.Project.GetScene("info_ring").GetLayer("Layer_001");
infoRing.SwapParentWithChild(infoRing.GetCast("icon_ring"), infoRing.GetCast("shade"));

var crmInfoChaoSo = srd.Project.GetScene("crm_info_chao_so").GetLayer("Layer_001");
crmInfoChaoSo.SwapParentWithChild(crmInfoChaoSo.GetCast("icon_chao"), crmInfoChaoSo.GetCast("shade"));

var crmInfoRingSo = srd.Project.GetScene("crm_info_ring_so").GetLayer("Layer_001");
crmInfoRingSo.SwapParentWithChild(crmInfoRingSo.GetCast("icon_ring"), crmInfoRingSo.GetCast("icon_ring_shade"));

var crmInfoChaoCr = srd.Project.GetScene("crm_info_chao_cr").GetLayer("Layer_001");
crmInfoChaoCr.SwapParentWithChild(crmInfoChaoCr.GetCast("icon_chao"), crmInfoChaoCr.GetCast("shade"));

srd.Write("ui_gameplay.swif");

Console.ReadKey();
