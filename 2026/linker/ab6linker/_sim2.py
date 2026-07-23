import struct
import subprocess
from pathlib import Path

IMAGE_BASE = 0x400000
TEXT_RVA = 0x1000
IDATA_RVA = 0x2000
SA = 0x1000
FA = 0x200
SH = 0x200
EL = 0x80


def align(v, a):
    return (v + a - 1) & ~(a - 1)


data = Path("exit32.obj").read_bytes()
vals = struct.unpack_from("<8sIIIIIIHHI", data, 20)
rawsz, praw, preloc, nreloc = vals[3], vals[4], vals[5], vals[7]
code = bytearray(align(rawsz, FA))
code[:rawsz] = data[praw : praw + rawsz]
rva, si, rt = struct.unpack_from("<IIH", data, preloc)

pos = 40
ilt = pos
pos += 8
iat = pos
iat_start = iat
pos += 8
pos = align(pos, 2)
dll = pos
pos += len("kernel32.dll") + 1
pos = align(pos, 2)
hint = pos
pos += 2 + len("ExitProcess") + 1
pos = align(pos, 4)
thunk = pos
pos += 8
idata_virt = pos
idata_raw = max(align(idata_virt, FA), FA)
idata = bytearray(idata_raw)


def PD(b, o, v):
    struct.pack_into("<I", b, o, v)


PD(idata, 0, IDATA_RVA + ilt)
PD(idata, 12, IDATA_RVA + dll)
PD(idata, 16, IDATA_RVA + iat)
PD(idata, ilt, IDATA_RVA + hint)
PD(idata, iat, IDATA_RVA + hint)
idata[dll : dll + 13] = b"kernel32.dll\x00"
struct.pack_into("<H", idata, hint, 0)
idata[hint + 2 : hint + 2 + 12] = b"ExitProcess\x00"
idata[thunk] = 0xFF
idata[thunk + 1] = 0x25
PD(idata, thunk + 2, IMAGE_BASE + IDATA_RVA + iat)

target = IMAGE_BASE + IDATA_RVA + thunk
rel = target - (IMAGE_BASE + TEXT_RVA + rva + 4)
struct.pack_into("<i", code, rva, rel)
print("rel", hex(rel & 0xFFFFFFFF), "thunk", hex(IDATA_RVA + thunk), "iat", hex(IDATA_RVA + iat))
print("code", bytes(code[:7]).hex())

buf = bytearray(512)


def pw(o, v):
    struct.pack_into("<H", buf, o, v)


def pd(o, v):
    struct.pack_into("<I", buf, o, v)


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
pw(EL + 6, 2)
pw(EL + 20, 224)
pw(EL + 22, 0x103)
o = EL + 24
pw(o, 0x10B)
buf[o + 2] = 1
pd(o + 4, len(code))
pd(o + 8, idata_raw)
pd(o + 16, TEXT_RVA)
pd(o + 20, TEXT_RVA)
pd(o + 28, IMAGE_BASE)
pd(o + 32, SA)
pd(o + 36, FA)
pw(o + 40, 4)
pw(o + 48, 4)
pd(o + 56, IDATA_RVA + align(idata_virt, SA))
pd(o + 60, SH)
pw(o + 68, 3)
pd(o + 72, 0x100000)
pd(o + 76, 0x1000)
pd(o + 80, 0x100000)
pd(o + 84, 0x1000)
pd(o + 92, 16)
pd(o + 104, IDATA_RVA)
pd(o + 108, 40)
pd(o + 192, IDATA_RVA + iat_start)
pd(o + 196, 8)
s = o + 224
for i, c in enumerate(b".text\0\0\0"):
    buf[s + i] = c
pd(s + 8, 0x10)
pd(s + 12, TEXT_RVA)
pd(s + 16, len(code))
pd(s + 20, SH)
pd(s + 36, 0x60000020)
s2 = s + 40
for i, c in enumerate(b".idata\0\0"):
    buf[s2 + i] = c
pd(s2 + 8, idata_virt)
pd(s2 + 12, IDATA_RVA)
pd(s2 + 16, idata_raw)
pd(s2 + 20, SH + len(code))
pd(s2 + 36, 0x60000020)

assert len(buf) == 512
out = bytes(buf) + bytes(code) + bytes(idata)
print("size", len(out))
Path("a_ablinker_sim.exe").write_bytes(out)
r = subprocess.run([str(Path("a_ablinker_sim.exe").resolve())])
print("exitcode", r.returncode)
