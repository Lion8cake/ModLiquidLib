## Welcome to ModLiquid Library!
This library aims to have a concise and consistent way of implementing liquids for other mods while also sticking to similar implementation of other tmodloader content. (ie: ModTiles and ModWalls)

For example, creating a liquid should be just as easy as making a tile or wall, using SetStaticDefaults for settings and map entries, using the various pre/post draws and updates for rendering and actions as well as other various miscellaneous methods for your own needs

The library will include:
* ModLiquid
* GlobalLiquid
* ModLiquidFall (an updated and expanded upon ModWaterfallStyle, used for more than just water styles)

When finished this mod will be uploaded to the Tmodloader workshop for mods to use alongside with. This mod currently is still in an early alpha release (github only) although most of the main methods for implementation are added (with only a handful left to be made).

FaQ:

### When will it be released?

When its ready!

### Am I able to download and use this mod?

For a normal player I wouldn't recommend since this only adds modding tools for developers. 
For developers and players using this mod for other experimental mods, definitely! 
Although until full release expect a ton of issues, and if you do find bugs, please post it in the issues page to be fixed.


### Is this mod too early in development to use?
Nope! 
Currently you are able to make a whole new liquid and do a lot with it. Fishing and official bucket support is not added yet but is planned to be worked on soon.
Feel free to check out the mod now! The more people using it, the more issues and suggestions will be fixed and added before a steam workshop release, only making the mod better.


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