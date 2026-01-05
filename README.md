## Welcome to ModLiquid Library!
This library aims to have a concise and consistent way of implementing liquids for other mods while also sticking to similar implementation of other tmodloader content. (ie: ModTiles and ModWalls)

For example, creating a liquid should be just as easy as making a tile or wall, using SetStaticDefaults for settings and map entries, using the various pre/post draws and updates for rendering and actions as well as other various miscellaneous methods for your own needs

The library will include:
* ModLiquid
* GlobalLiquid
* ModLiquidFall (an updated and expanded upon ModWaterfallStyle, used for more than just water styles)
* Extentions for ModTiles
* Several ID sets for multiple different use cases

This mod is now on the workshop, download it here:
https://steamcommunity.com/sharedfiles/filedetails/?id=3539368598

For developers looking to use this mod, please see this mod's example mod. It contains a few examples of what you can do with this library.
Please see the example mod's github below:
https://github.com/Lion8cake/ModLiquidExampleMod

This mod currently is being drafted into tmodloader natively, if you want to show support, please see the PR below:
https://github.com/tModLoader/tModLoader/pull/4899

FaQ:

### When will it be released?

This mod is currently released! To use the mod reference ModLiquidLib in your mod's build.txt file and users will use your mod alongside ModLiquid Library.


### Am I able to download and use this mod?

This mod is now on the workshop, download and use this there for both players and developers alike.


### Is this mod too early in development to use?

Nope! 
Currently this mod is fully releaseds, feature complete in all aspects ready for both developers and players!

### I've found an issue, where do I report?

Report it in the Issues section of this github, there isn't anywhere else outside of contacting me personally (Lion8cake on discord) and I would much prefer all issues are in 1 concise area rather than in random DMs or discord servers.


### Will this change or add anything extra to the game outside of just liquid support?

No. There are a few points of support this mod will add related to liquids, such as ModLiquidFall, which is an updated ModWaterfallStyle used for both water styles and liquids.


### Can I contribute?

Sure, make sure you make a fork and then PR to the project so I (Lion8cake) can review all changes to make sure that everything will work.


### 'I think X code is horrible' or 'I think something should be implemented a different way'

The issues page on github is where to make suggestions as well as bug reports.
If you want to contact me personally (Lion8cake), please do so on discord (friend me with the Lion8cake username). I’m open to changing any aspect of this library to appeal to all modders and I would love to hear some constructive criticism over some code and how to make it better.


### Why is this a mod and not in Tmodloader itself?

Actually, it is! 
The mod currently has been ported into tmodloader and a whole PR has been made for ModLiquid to be added into tmodloader.
The Tmodloader team is currently awaiting for the 1.4.5 update to start intergrating this mod into tmodloader.
Until 1.4.5 tmod comes out, this mod will be updated to contain features added to the PR when possible (Features like 'liquidsIn' for the FishingAttempt struct wont be possible for mod).
Once 1.4.5 tmod comes out, this mod will become legacy and will stay and be updated on the 1.4.4 versions of the game.
Hopefully, a reason why this mod may appeal to you as it's being intergrated into tmodloader and extremely easy to port from using this library into using native tmodloader.
Please see the PR here: https://github.com/tModLoader/tModLoader/pull/4899


### Wiki on how to use this library?

A wiki has been started, but it’s extremely bare bones, pages will slowly be made on how to use this mod.
As for documentation in general, All/most hooks and fields for GLobalLiquid and ModLiquid have had comments added, describing what a hook/field does.