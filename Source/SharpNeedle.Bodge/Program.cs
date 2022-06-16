// Program for quickly writing temporary things
using System.Diagnostics;
using SharpNeedle.SurfRide.Draw;

var forcesSrd = ResourceUtility.Open<SrdProject>(@"F:\SWIF\Forces\PC\ui_stage_sonic_en\ui_cockpitindicator.swif");
forcesSrd.Write("ui_cockpitindicator.swif");
