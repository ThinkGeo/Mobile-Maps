# Packs the 44-band NDFD sky-cover forecast into one flat file the mobile sample
# reads without GDAL. Sky cover is a percentage, so a byte per cell is exact.
#
# Layout, little-endian: "TGSK", int32 bands, int32 rows, int32 cols,
#   float64 minX minY maxX maxY, int32 projLen, proj bytes,
#   per band: int64 unix seconds of GRIB_VALID_TIME (0 when absent),
#   then bands * rows * cols bytes, band-sequential, row 0 on the north edge.
import struct, subprocess, sys, re

img, hdr, src, out = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]

h = {}
for line in open(hdr):
    if "=" in line:
        k, v = line.split("=", 1)
        h[k.strip().lower()] = v.strip()

cols, rows, bands = int(h["samples"]), int(h["lines"]), int(h["bands"])
mi = [p.strip() for p in h["map info"].strip("{}").split(",")]
ulx, uly, xdim, ydim = float(mi[3]), float(mi[4]), float(mi[5]), float(mi[6])
minx, maxy = ulx, uly
maxx, miny = minx + cols * xdim, maxy - rows * ydim

# The hour each band is valid for is GRIB metadata, which only GDAL can read - so it
# is read here, on the desktop, and written into the file.
info = subprocess.run(["gdalinfo", src], capture_output=True, text=True).stdout
times = [int(m) for m in re.findall(r"GRIB_VALID_TIME=(\d+)", info)]
while len(times) < bands:
    times.append(0)

proj = ("+proj=lcc +lat_0=25 +lon_0=-95 +lat_1=25 +lat_2=25 "
        "+x_0=0 +y_0=0 +R=6371200 +units=m +no_defs")
pb = proj.encode("utf-8")
raw = open(img, "rb").read()
assert len(raw) == bands * rows * cols, (len(raw), bands * rows * cols)

with open(out, "wb") as f:
    f.write(b"TGSK")
    f.write(struct.pack("<iii", bands, rows, cols))
    f.write(struct.pack("<dddd", minx, miny, maxx, maxy))
    f.write(struct.pack("<i", len(pb)))
    f.write(pb)
    for t in times[:bands]:
        f.write(struct.pack("<q", t))
    f.write(raw)

print(f"  {bands} forecast hours, {rows} x {cols} cells")
print(f"  first hour unix {times[0]}, last {times[bands-1]}")
