## Welcome to ModLiquid Library!
This library aims to have a concise and consistent way of implementing liquids for other mods while also sticking to similar implementation of other tmodloader content. (ie: ModTiles and ModWalls)

For example, creating a liquid should be just as easy as making a tile or wall, using SetStaticDefaults for settings and map entries, using the various pre/post draws and updates for rendering and actions as well as other various miscellaneous methods for your own needs

The library will include:
* ModLiquid
* GlobalLiquid
* ModLiquidFall (an updated and expanded upon ModWaterfallStyle, used for more than just water styles)

This mod is now on the workshop, download it here:
https://steamcommunity.com/sharedfiles/filedetails/?id=3539368598

This mod is in it's beta phase, with enough content for mod use, although with future plans and bugfixes still left to do before fully done.

For developers looking to use this mod, please see this mod's example mod. It contains a few examples of what you can do with this library.
Please see the example mod's github below:
https://github.com/Lion8cake/ModLiquidExampleMod

FaQ:

### When will it be released?

The mod is currently released to the workshop for the preview version of Tmodloader. Mods are able to use this library there.
This mod is planned to release on the stable branch on the 1st of September (although this can change).
...as usual, still will come when its ready!


### Am I able to download and use this mod?

This mod is now on the workshop, download and use this there for both players and developers alike.


### Is this mod too early in development to use?

Nope! 
Currently you are able to make a whole new liquid and do a lot with it.
Feel free to check out the mod now! The more people using it, the more issues and suggestions will be fixed and added before it's full release, only making the mod better.


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

It's planned to have ModLiquid added to tmodloader when the Terraria 1.4.5 update drops. Once it comes out, all these features will be brought over to tmodloader.
A main reason why this library will appeal to you is hopefully, in the future, all you'll need when tmodloader 1.4.5 comes out to update is removing this from your mod dependencies.


### Wiki on how to use this library?

A wiki has been started, but it’s extremely bare bones, pages will slowly be made on how to use this mod, although the top priority currently is working on this library for an official workshop release.
As for documentation in general, All/most hooks and fields for GLobalLiquid and ModLiquid have had comments added, describing what a hook/field does.