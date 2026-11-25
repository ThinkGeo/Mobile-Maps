# Driving the gallery without a hand on the screen

Every sample in this gallery was opened and screenshotted on a simulator before it
was committed. These are the two scripts that did it.

```sh
Tools/shoot-ios.sh      "Cross the Dateline" 45
Tools/shoot-android.sh  "Cross the Dateline" 45
```

Each one launches the app straight into a named sample, waits, takes a screenshot
and prints four numbers from the log:

| | |
|---|---|
| `opened` | the app reached the sample and said so |
| `gl` | a GL context came up (2 for the samples that run two maps) |
| `crash` | SIGSEGV, an unhandled exception, or a FATAL line |
| `missing` | the title did not match a catalog row - a typo in the call |

`opened=1 gl=1 crash=0 missing=0` is a pass. `opened=1 gl=0` means the page came up
without a GPU map, which is right for the one or two samples that draw only with
the classic pipeline and wrong for every other.

## How a sample gets opened

`App.xaml.cs` reads the `HOWDOI_SAMPLE` environment variable and, if it names a
catalog row, pushes that page instead of the menu.

On iOS `simctl launch` passes `SIMCTL_CHILD_HOWDOI_SAMPLE` through as the child's
environment, so the variable is simply there. Android has no equivalent - a process
cannot be handed an environment - so the name travels as an intent extra and
`MainActivity.OnCreate` copies it into the environment before MAUI starts. The one
line in `App.xaml.cs` then works on both.

## Sweeping the whole gallery

```sh
# every title in the catalog, one after another
grep -oE 'new\("[^"]+",\s*"[^"]+"' Shell/SampleCatalog.cs \
  | sed 's/.*,\s*"//' \
  | while IFS= read -r t; do Tools/shoot-android.sh "$t" 26 </dev/null; done
```

The `</dev/null` matters: `adb` reads standard input, and without it the first call
swallows the rest of the list and the sweep quietly stops after one sample.

## Building for Android

```sh
dotnet build HowDoISample.csproj -f net10.0-android36.0 -p:RuntimeIdentifier=android-arm64 -p:EmbedAssembliesIntoApk=true
adb install -r bin/Debug/net10.0-android36.0/com.thinkgeo.androidhowdoi-Signed.apk
```

A default Android debug build leaves the assemblies out of the APK for the IDE to
push separately. An APK installed by hand without them starts and exits with
`No assemblies found in .__override__`.
