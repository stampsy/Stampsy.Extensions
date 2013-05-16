Stampsy.Extensions
==================

This is meant to be a small "misc utils" library for MonoTouch that makes some UIKit/Foundation API more C#-friendly.  
It contains friendlier wrappers around some MonoTouch classes/methods and fairly popular extension methods.

My rule of thumb for inclusion of a class or method is that:

- it should be the kind of thing that isn't in the framework but is commonly requested and has lots of upvotes on SO;
- it involves little to no abstraction and is a really thin wrapper above Apple classes.

Contributions are welcome, if they satisfy both criteria.


### Code Samples

#### Reading and Writing iOS Settings

```c#
using MySettings = Stampsy.Extensions.Foundation.UserSettings<MySetting>;

enum MySetting {
    GodMode,
    LikesCheese
}

MySettings.RegisterDefaultsFromSettingsBundle ();
bool god = MySettings.Read<bool> (MySetting.GodMode);
```

#### Task Extensions for UIKit
   
```c#
Api.UploadAvatar (img)
   .WithNetworkIndicator () // takes care of UIApplication.NetworkActivityIndicatorVisible
   .RegisterAsBackgroundTask (); // does the UIApplication.BeginBackgroundTask dance
```

#### Misc

```c#
var titleView = bar.Subviews.FirstOrDefault (
    v => v.GetClassName () == "UINavigationItemView" // gets ObjC class name
    );

```
