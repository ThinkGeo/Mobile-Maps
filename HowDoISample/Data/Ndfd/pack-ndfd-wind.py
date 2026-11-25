# Packs two EHdr float32 grids (speed, direction-from) into one flat file the mobile
# sample reads with a BinaryReader and no GDAL.
#
# Layout, little-endian: "TGWF", int32 rows, int32 cols, float64 minX minY maxX maxY,
# int32 projLen, projLen UTF-8 bytes, rows*cols float32 speed, rows*cols float32 dir.
import struct, sys, math

def hdr(path):
    d = {}
    for line in open(path):
        parts = line.split()
        if len(parts) >= 2:
            d[parts[0].upper()] = parts[1]
    return d

def read(bil, hdrpath):
    h = hdr(hdrpath)
    rows, cols = int(h["NROWS"]), int(h["NCOLS"])
    xdim, ydim = float(h["XDIM"]), float(h["YDIM"])
    ulx, uly = float(h["ULXMAP"]), float(h["ULYMAP"])
    nodata = float(h.get("NODATA", "nan"))
    raw = open(bil, "rb").read()
    assert len(raw) == rows * cols * 4, (len(raw), rows * cols * 4)
    # ULXMAP/ULYMAP are cell CENTRES in EHdr; the extent is half a cell beyond.
    minx = ulx - xdim / 2
    maxy = uly + ydim / 2
    maxx = minx + cols * xdim
    miny = maxy - rows * ydim
    return raw, rows, cols, (minx, miny, maxx, maxy), nodata

speed_raw, rows, cols, extent, snd = read("ndfd/wspd_s.bil", "ndfd/wspd_s.hdr")
dir_raw, drows, dcols, _, dnd = read("ndfd/wdir_s.bil", "ndfd/wdir_s.hdr")
assert (rows, cols) == (drows, dcols)

proj = "+proj=lcc +lat_0=25 +lon_0=-95 +lat_1=25 +lat_2=25 +x_0=0 +y_0=0 +R=6371200 +units=m +no_defs"
pb = proj.encode("utf-8")

def scrub(raw, nodata):
    out = bytearray(raw)
    nan = struct.pack("<f", float("nan"))
    n = len(raw) // 4
    bad = 0
    for i in range(n):
        v = struct.unpack_from("<f", raw, i * 4)[0]
        if v == nodata or math.isnan(v) or abs(v) > 1e6:
            out[i*4:i*4+4] = nan
            bad += 1
    return bytes(out), bad

speed_raw, sbad = scrub(speed_raw, snd)
dir_raw, dbad = scrub(dir_raw, dnd)

out = sys.argv[1]
with open(out, "wb") as f:
    f.write(b"TGWF")
    f.write(struct.pack("<ii", rows, cols))
    f.write(struct.pack("<dddd", *extent))
    f.write(struct.pack("<i", len(pb)))
    f.write(pb)
    f.write(speed_raw)
    f.write(dir_raw)

print(f"  {rows} x {cols} cells, {sbad + dbad} no-data cells blanked")
print(f"  extent {tuple(round(v, 1) for v in extent)}")
