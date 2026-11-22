#!/bin/bash
# The same thing on an Android emulator. Android has no way to set a child
# process's environment, so the sample name travels as an intent extra and
# MainActivity puts it into the environment before MAUI starts - which is what lets
# the one line in App.xaml.cs drive both platforms.
#
#   Tools/shoot-android.sh "Cross the Dateline" 45
#
# The APK must have been built with -p:EmbedAssembliesIntoApk=true, or it installs
# as an empty shell and exits on launch.
set -u
A=~/Library/Android/sdk/platform-tools/adb
PKG="${HOWDOI_PKG:-com.thinkgeo.ioshowdoi}"
ACT="$PKG/$(${A} shell cmd package resolve-activity --brief "$PKG" | tail -1 | cut -d/ -f2 | tr -d "\r")"
OUT="$(dirname "$0")"/../shots-android
TITLE="$1"
WAIT="${2:-30}"
SLUG=$(echo "$TITLE" | tr ' ' '-' | tr -cd '[:alnum:]-')

mkdir -p "$OUT"
$A shell am force-stop "$PKG" >/dev/null 2>&1
$A logcat -c >/dev/null 2>&1
$A shell am start -n "$ACT" --es HOWDOI_SAMPLE "'$TITLE'" >/dev/null 2>&1
sleep "$WAIT"
$A exec-out screencap -p > "$OUT/$SLUG.png" 2>/dev/null
$A logcat -d > "$OUT/$SLUG.log" 2>/dev/null

crash=$(grep -cE "FATAL|AndroidRuntime: *Unhandled|SIGSEGV" "$OUT/$SLUG.log")
opened=$(grep -c "opening '" "$OUT/$SLUG.log")
gl=$(grep -c "GLES up" "$OUT/$SLUG.log")
missing=$(grep -c "no sample titled" "$OUT/$SLUG.log")
printf '%-34s opened=%s gl=%s crash=%s missing=%s\n' "$TITLE" "$opened" "$gl" "$crash" "$missing"
