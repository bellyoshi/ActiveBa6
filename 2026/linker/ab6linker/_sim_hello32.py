"""Simulate fixed ablinker for hello32.obj and compare to hello32.exe"""
import struct
import subprocess
from pathlib import Path

IMAGE_BASE = 0x400000
TEXT_RVA = 0x1000
DATA_RVA = 0x2000
IDATA_RVA = 0x3000
SA = 0x1000
FA = 0x200
SH = 0x400
EL = 0x80


def align(v, a):
    return (v + a - 1) & ~(a - 1)


def undecorate(s):
    if s.startswith("_"):
        s = s[1:]
    if "@" in s:
        s = s[: s.index("@")]
    return s


def lookup_dll(f):
    if f in ("MessageBoxA", "MessageBoxW", "wsprintfA"):
        return "user32.dll"
    return "kernel32.dll"


def symname(data, psym, nsym, i):
    e = data[psym + i * 18 : psym + (i + 1) * 18]
    st = psym + nsym * 18
    if e[:4] == b"\0\0\0\0":
        o = struct.unpack_from("<I", e, 4)[0]
        return data[st + o :].split(b"\0", 1)[0].decode()
    return e[:8].split(b"\0", 1)[0].decode()


data = Path("hello32.obj").read_bytes()
machine, nsect, ts, psym, nsym, opt, ch = struct.unpack_from("<HHIIIHH", data, 0)
off = 20 + opt
text = data_sec = None
sec_rva = {}
for i in range(nsect):
    name, vsize, va, rawsz, praw, preloc, _, nreloc, _, sch = struct.unpack_from(
        "<8sIIIIIIHHI", data, off
    )
    sname = name.split(b"\0", 1)[0].decode()
    if sname == ".text":
        text = (rawsz, praw, preloc, nreloc, i + 1)
        sec_rva[i + 1] = TEXT_RVA
    if sname == ".data":
        data_sec = (rawsz, praw, i + 1)
        sec_rva[i + 1] = DATA_RVA
    off += 40

traw, tpraw, preloc, nreloc, tsec = text
draw, dpraw, dsec = data_sec
code = bytearray(align(traw, FA))
code[:traw] = data[tpraw : tpraw + traw]
datab = bytearray(align(draw, FA))
datab[:draw] = data[dpraw : dpraw + draw]

imports = []
relocs = []
for j in range(nreloc):
    rva, si, rt = struct.unpack_from("<IIH", data, preloc + j * 10)
    e = data[psym + si * 18 : psym + (si + 1) * 18]
    sec = struct.unpack_from("<H", e, 12)[0]
    nm = symname(data, psym, nsym, si)
    val = struct.unpack_from("<I", e, 8)[0]
    relocs.append((rva, rt, sec, nm, val))
    if sec == 0:
        fn = undecorate(nm)
        if fn not in [x[0] for x in imports]:
            imports.append((fn, lookup_dll(fn)))

# group dlls
dlls = []
for fn, d in imports:
    if d not in dlls:
        dlls.append(d)
grouped = []
counts = []
for d in dlls:
    c = 0
    for fn, dd in imports:
        if dd == d:
            grouped.append((fn, d))
            c += 1
    counts.append(c)
imports = grouped

# idata layout
pos = (len(dlls) + 1) * 20
ilt = []
iat = []
for c in counts:
    ilt.append(pos)
    pos += 4 * (c + 1)
iat_start = pos
for c in counts:
    iat.append(pos)
    pos += 4 * (c + 1)
dlln = []
for d in dlls:
    pos = align(pos, 2)
    dlln.append(pos)
    pos += len(d) + 1
hint = []
for fn, _ in imports:
    pos = align(pos, 2)
    hint.append(pos)
    pos += 2 + len(fn) + 1
pos = align(pos, 4)
iat_rva = []
thunk_rva = []
imp_i = 0
for di, c in enumerate(counts):
    for k in range(c):
        iat_rva.append(IDATA_RVA + iat[di] + k * 4)
        thunk_rva.append(IDATA_RVA + pos)
        pos += 8
        imp_i += 1
idata_virt = pos
idata_raw = max(align(idata_virt, FA), FA)
idata = bytearray(idata_raw)


def PD(b, o, v):
    struct.pack_into("<I", b, o, v & 0xFFFFFFFF)


for di, d in enumerate(dlls):
    base = di * 20
    PD(idata, base + 0, IDATA_RVA + ilt[di])
    PD(idata, base + 12, IDATA_RVA + dlln[di])
    PD(idata, base + 16, IDATA_RVA + iat[di])
imp_i = 0
for di, c in enumerate(counts):
    for k in range(c):
        PD(idata, ilt[di] + k * 4, IDATA_RVA + hint[imp_i])
        PD(idata, iat[di] + k * 4, IDATA_RVA + hint[imp_i])
        imp_i += 1
for di, d in enumerate(dlls):
    idata[dlln[di] : dlln[di] + len(d) + 1] = d.encode() + b"\0"
for i, (fn, _) in enumerate(imports):
    struct.pack_into("<H", idata, hint[i], 0)
    idata[hint[i] + 2 : hint[i] + 2 + len(fn) + 1] = fn.encode() + b"\0"
    to = thunk_rva[i] - IDATA_RVA
    idata[to] = 0xFF
    idata[to + 1] = 0x25
    PD(idata, to + 2, IMAGE_BASE + iat_rva[i])

name_to_i = {fn: i for i, (fn, _) in enumerate(imports)}
for rva, rt, sec, nm, val in relocs:
    if sec == 0:
        idx = name_to_i[undecorate(nm)]
        target = IMAGE_BASE + (iat_rva[idx] if rt == 6 else thunk_rva[idx])
    else:
        target = IMAGE_BASE + sec_rva[sec] + val
    if rt == 0x14:
        rel = target - (IMAGE_BASE + TEXT_RVA + rva + 4)
        struct.pack_into("<i", code, rva, rel)
    elif rt == 6:
        addend = struct.unpack_from("<I", code, rva)[0]
        struct.pack_into("<I", code, rva, (addend + target) & 0xFFFFFFFF)

print("code", bytes(code[:26]).hex())
ref = Path("hello32.exe").read_bytes()
print("ref ", ref[0x400 : 0x400 + 26].hex())
print("data", bytes(datab[:19]))
print("refd", ref[0x600 : 0x600 + 19])

# build PE
buf = bytearray(SH)


def pw(o, v):
    struct.pack_into("<H", buf, o, v)


def pd(o, v):
    struct.pack_into("<I", buf, o, v & 0xFFFFFFFF)


pw(0, 0x5A4D)
pw(2, 0x90)
pw(4, 3)
pw(8, 4)
pw(12, 0xFFFF)
pw(16, 0xB8)
pw(24, 0x40)
pd(60, EL)
pd(EL, 0x4550)
pw(EL + 4, 0x14C)
pw(EL + 6, 3)
pw(EL + 20, 224)
pw(EL + 22, 0x103)
o = EL + 24
pw(o, 0x10B)
buf[o + 2] = 1
pd(o + 4, len(code))
pd(o + 8, len(datab) + idata_raw)
pd(o + 16, TEXT_RVA)
pd(o + 20, TEXT_RVA)
pd(o + 24, DATA_RVA)
pd(o + 28, IMAGE_BASE)
pd(o + 32, SA)
pd(o + 36, FA)
pw(o + 40, 4)
pw(o + 48, 4)
pd(o + 56, IDATA_RVA + align(idata_virt, SA))
pd(o + 60, SH)
pw(o + 68, 2)
pd(o + 72, 0x100000)
pd(o + 76, 0x1000)
pd(o + 80, 0x100000)
pd(o + 84, 0x1000)
pd(o + 92, 16)
pd(o + 104, IDATA_RVA)
pd(o + 108, (len(dlls) + 1) * 20)
pd(o + 192, IDATA_RVA + iat_start)
pd(o + 196, sum(4 * (c + 1) for c in counts))
s = o + 224
for i, c in enumerate(b".text\0\0\0"):
    buf[s + i] = c
pd(s + 8, max(traw, 0x10))
pd(s + 12, TEXT_RVA)
pd(s + 16, len(code))
pd(s + 20, SH)
pd(s + 36, 0x60000020)
s += 40
for i, c in enumerate(b".data\0\0\0"):
    buf[s + i] = c
pd(s + 8, max(draw, 4))
pd(s + 12, DATA_RVA)
pd(s + 16, len(datab))
pd(s + 20, SH + len(code))
pd(s + 36, 0xC0000040)
s += 40
for i, c in enumerate(b".idata\0\0"):
    buf[s + i] = c
pd(s + 8, idata_virt)
pd(s + 12, IDATA_RVA)
pd(s + 16, idata_raw)
pd(s + 20, SH + len(code) + len(datab))
pd(s + 36, 0x60000020)

out = bytes(buf) + bytes(code) + bytes(datab) + bytes(idata)
Path("h_fixed_sim.exe").write_bytes(out)
print("size", len(out), "ref", len(ref))
print("text match?", out[SH : SH + 26] == ref[0x400 : 0x400 + 26])
# run - MessageBox will block; use timeout or just check loadable
import ctypes

# Check PE loads: use LoadLibrary doesn't work for EXE. Just run with short wait - MessageBox needs user.
# Instead verify GetExitCode after starting suspended... skip UI.
print("imports", imports)
print("DIR32 patches", bytes(code[2:14]).hex())
