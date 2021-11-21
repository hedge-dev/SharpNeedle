// Program for quickly writing temporary things
using SharpNeedle.HedgehogEngine.Mirage;

// Ultimate test
// Can it read what it wrote
var model = ResourceUtility.Open<Model>(@"D:\Unpacked CPK\Sonic Generations\bb3\Sonic\chr_Sonic_HD.model");
// whatcha doin sajid I was staring at ida pro
// my cousin was around lol
// guess i should prepare stuff for pushing

// it didn't save properly huh
// something missing?
// mesh that is in trans slot is in punch for some reason
// interesting
// oh it's your fault LOOOL
// how
// ok I fixed it
// what did you fix???
// you were reading trans before punch
// OH WAIT
// LMAO
// looks ok now
// now try the lightdashready one
// looks valid?
// that also looks fine but I think there's an extra field in the model?
// the one that for some reason points to the offset table?
// yep
// wanna write 0 to it and try in unleashed lmao
// gens supports v2 too lol
// oh cool, what model do we replace then

// sonic's ofc
// sonic has a fucking skeleton
// isnt that gonna cry
// no?????????????
// oh
// lemme see what the game does with it in the code
// imagine its just the data size lol
// it's unused :intensejoy:
// you don't even have to write it
// well it just crashed
// maybe it's cause it's one single mesh group and the ModelData class has the list and the single mesh group separate for some reason
// replace another model then?
// nah I don't think it's ever gonna work in gens
// sad
// time to try in unleashed xd
// except i cant repack the base ar
// let's just ignore it, it's unused lol
// :penisve:
// time for touchups then, should make mesh resolve materials
// resolve what now
// materials???
// you know depends
// oh you want models to load them ok
// yea


// I mean in the og file

// well it saved
// send me so I can see if properly
// there
// why the fuck is it named ev chr sonic hd
// dont worry about it

// nice
// now save it again and see if it's identical to the rewritten file
// why is it significantly larger than the original file tho?
// og like 4.2 MB rewritten is like 5 MB

// idk maybe some alignment somewhere
// or the fact that og resues offsets for empty stuff and i dont
// time to see if it works ingame lol

// already forgot where i took the model from
// ev041

// what cutscene is this lmao
// I have no idea
// its one of the mirror ones
// yea it is the files say mirror

// and i launched the game with mirror mode
// the ultra experience

// it fucking crashed
// POG

// wonder if its the morphs
// considering glvl loaded the file fine it's probs the morphs

// Debugger.Break();

// Program execute successfully!!
// Now to find out where the fucking file went
// Would be wild
// found it
// ???????????????
// is it really or are you fucking with me
// what the fuck
// imagine it actually saved it to my disk
// bro the file is also in my D: