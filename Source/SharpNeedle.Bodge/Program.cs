// Program for quickly writing temporary things

using SharpNeedle.Ninja.Csd;
using SharpNeedle.Ninja.Csd.Extensions;
using SharpNeedle.SurfRide.Draw;
using SharpNeedle.SurfRide.Draw.Extensions;
using static SharpNeedle.SurfRide.Draw.CastNode;

var csd = ResourceUtility.Open<CsdProject>(@"E:\SteamLibrary\steamapps\common\Sonic Generations\disk\bb3\SonicActionCommonHud\ui_gameplay.xncp");

var srd = csd.ToSrdProject();

var infoLife = srd.Project.GetScene("info_life").GetLayer("Layer_001");
infoLife.SwapParentWithChild(infoLife.GetCast("icon_generic"), infoLife.GetCast("Cast_0237"));
infoLife.SwapParentWithChild(infoLife.GetCast("icon_classic"), infoLife.GetCast("Cast_0238"));

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

var pinMedal = srd.Project.GetScene("pin_medal").GetLayer("Layer_001");
pinMedal.SwapParentWithChild(pinMedal.GetCast("icon_medal"), pinMedal.GetCast("shade"));

var rankTime = srd.Project.GetScene("rank_time").GetLayer("Layer_001");
rankTime.SwapParentWithChild(rankTime.GetCast("icon_0"), rankTime.Casts[2]);
rankTime.SwapParentWithChild(rankTime.GetCast("icon_1"), rankTime.Casts[7]);
rankTime.SwapParentWithChild(rankTime.GetCast("icon_2"), rankTime.Casts[11]);
rankTime.SwapParentWithChild(rankTime.GetCast("icon_3"), rankTime.Casts[15]);
rankTime.SwapParentWithChild(rankTime.GetCast("icon_4"), rankTime.Casts[19]);

var missionTarget = srd.Project.GetScene("mission_target").GetLayer("Layer_001");
missionTarget.SwapParentWithChild(missionTarget.GetCast("img_icon"), missionTarget.GetCast("img_icon_shade"));

var missionCourseTab = srd.Project.GetScene("mission_course_tab").GetLayer("Layer_001");
missionCourseTab.SwapParentWithChild(missionCourseTab.GetCast("bg"), missionCourseTab.GetCast("bg_shade"));
missionCourseTab.SwapParentWithChild(missionCourseTab.GetCast("icon_chara"), missionCourseTab.GetCast("icon_chara_shade"));

var blbGaugeEff = srd.Project.GetScene("blb_gauge_eff").GetLayer("Layer_001");
blbGaugeEff.SwapParentWithChild(blbGaugeEff.GetCast("icon_cl"), blbGaugeEff.GetCast("icon_cl_shade"));
blbGaugeEff.SwapParentWithChild(blbGaugeEff.GetCast("icon_gn"), blbGaugeEff.GetCast("icon_gn_shade"));

srd.Write("ui_gameplay.swif");

Console.ReadKey();