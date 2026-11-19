#!/bin/bash
# Opens one sample on an iOS simulator, waits for it to settle, screenshots it and
# reports what the log says. The app reads HOWDOI_SAMPLE, and `simctl launch` passes
# SIMCTL_CHILD_* through as the child's environment.
#
#   Tools/shoot-ios.sh "Cross the Dateline" 45
#
# HOWDOI_SIM picks the simulator; the default is whichever is booted.
set -u
# The simulator to run on. `xcrun simctl list devices` prints the ids.
SIM="${HOWDOI_SIM:-booted}"
APP="${HOWDOI_APP:-com.thinkgeo.mauihowdoi}"
OUT="$(dirname "$0")"/../shots
TITLE="$1"
WAIT="${2:-26}"
SLUG=$(echo "$TITLE" | tr ' ' '-' | tr -cd '[:alnum:]-')

mkdir -p "$OUT"
xcrun simctl terminate "$SIM" "$APP" >/dev/null 2>&1
rm -f "$OUT/$SLUG.log"
SIMCTL_CHILD_HOWDOI_SAMPLE="$TITLE" \
  nohup xcrun simctl launch --console-pty "$SIM" "$APP" > "$OUT/$SLUG.log" 2>&1 &
disown
sleep "$WAIT"
xcrun simctl io "$SIM" screenshot "$OUT/$SLUG.png" >/dev/null 2>&1

crash=$(grep -c 'SIGSEGV\|Unhandled\|uncaught exception' "$OUT/$SLUG.log")
opened=$(grep -c "opening '" "$OUT/$SLUG.log")
gl=$(grep -c 'GLES up' "$OUT/$SLUG.log")
printf '%-28s opened=%s gl=%s crash=%s  %s\n' "$TITLE" "$opened" "$gl" "$crash" "$OUT/$SLUG.png"
if [ "$crash" -gt 0 ]; then
  grep -m3 -A4 'SIGSEGV\|uncaught exception' "$OUT/$SLUG.log" | head -12
fi
