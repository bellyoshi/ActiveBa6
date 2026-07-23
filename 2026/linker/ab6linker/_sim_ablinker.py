"""Simulate ablinker auto-IAT logic for exit32.obj"""
import struct
import subprocess
from pathlib import Path

IMAGE_BASE = 0x00400000
TEXT_RVA = 0x00001000
IDATA_RVA = 0x00002000
SECTION_ALIGN = 0x1000
FILE_ALIGN = 0x200
SIZEOF_HEADERS = 0x200
E_LFANEW = 0x80
SIZEOF_IMPORT_DESCRIPTOR = 20


def align_up(v, a):
    return (v + a - 1) & ~(a - 1)


def undecorate(sym):
    s = sym
    if s.startswith("_"):
        s = s[1:]
    if "@" in s:
        s = s[: s.index("@")]
    return s


def lookup_dll(func):
    if func in ("MessageBoxA", "MessageBoxW", "wsprintfA"):
        return "user32.dll"
    return "kernel32.dll"


def symname(data, psym, nsym, i):
    e = data[psym + i * 18 : psym + (i + 1) * 18]
    strtab = psym + nsym * 18
    if e[0:4] == b"\x00\x00\x00\x00":
        o = struct.unpack_from("<I", e, 4)[0]
        return data[strtab + o :].split(b"\x00", 1)[0].decode()
    return e[:8].split(b"\x00", 1)[0].decode()


def build(obj_path, out_path):
    data = Path(obj_path).read_bytes()
    machine, nsect, ts, psym, nsym, opt, ch = struct.unpack_from("<HHIIIHH", data, 0)
    assert machine == 0x14C

    off = 20 + opt
    text = None
    for i in range(nsect):
        name, vsize, va, rawsz, praw, preloc, _, nreloc, _, sch = struct.unpack_from(
            "<8sIIIIIIHHI", data, off
        )
        if name.startswith(b".text"):
            text = (rawsz, praw, preloc, nreloc)
        off += 40
    rawsz, praw, preloc, nreloc = text
    code = bytearray(align_up(max(rawsz, 1), FILE_ALIGN))
    code[:rawsz] = data[praw : praw + rawsz]
    text_virt = max(rawsz, 0x10)
    text_raw = len(code)

    # collect imports
    imports = []  # list of undecorated names in order first-seen
    relocs = []
    for j in range(nreloc):
        rva, si, rt = struct.unpack_from("<IIH", data, preloc + j * 10)
        e = data[psym + si * 18 : psym + (si + 1) * 18]
        sec = struct.unpack_from("<H", e, 12)[0]
        nm = symname(data, psym, nsym, si)
        relocs.append((rva, si, rt, sec, nm))
        if sec == 0:
            fn = undecorate(nm)
            if fn not in imports:
                imports.append(fn)

    # group by dll
    dll_order = []
    for fn in imports:
        d = lookup_dll(fn)
        if d not in dll_order:
            dll_order.append(d)
    grouped = []
    dll_first = []
    dll_count = []
    for d in dll_order:
        dll_first.append(len(grouped))
        c = 0
        for fn in imports:
            if lookup_dll(fn) == d:
                grouped.append((fn, d))
                c += 1
        dll_count.append(c)
    imports = grouped  # (func, dll)

    # layout idata
    n_dlls = len(dll_order)
    pos = 0
    idt_off = 0
    idt_size = (n_dlls + 1) * SIZEOF_IMPORT_DESCRIPTOR
    pos = idt_size
    ilt_off = []
    iat_off = []
    dll_name_off = []
    hint_off = []
    iat_rva = []
    thunk_rva = []

    for d in range(n_dlls):
        ilt_off.append(pos)
        pos += 4 * (dll_count[d] + 1)
    iat_start = pos
    iat_size = 0
    for d in range(n_dlls):
        iat_off.append(pos)
        iat_size += 4 * (dll_count[d] + 1)
        pos += 4 * (dll_count[d] + 1)
    for d in range(n_dlls):
        pos = align_up(pos, 2)
        dll_name_off.append(pos)
        pos += len(dll_order[d]) + 1
    imp_i = 0
    for d in range(n_dlls):
        for k in range(dll_count[d]):
            pos = align_up(pos, 2)
            hint_off.append(pos)
            pos += 2 + len(imports[imp_i][0]) + 1
            imp_i += 1
    pos = align_up(pos, 4)
    imp_i = 0
    for d in range(n_dlls):
        for k in range(dll_count[d]):
            iat_rva.append(IDATA_RVA + iat_off[d] + k * 4)
            thunk_rva.append(IDATA_RVA + pos)
            pos += 8
            imp_i += 1

    idata_virt = pos
    idata_raw = max(align_up(idata_virt, FILE_ALIGN), FILE_ALIGN)
    idata = bytearray(idata_raw)

    def PD(p, o, v):
        struct.pack_into("<I", p, o, v & 0xFFFFFFFF)

    def PW(p, o, v):
        struct.pack_into("<H", p, o, v & 0xFFFF)

    for d in range(n_dlls):
        base = idt_off + d * SIZEOF_IMPORT_DESCRIPTOR
        PD(idata, base + 0, IDATA_RVA + ilt_off[d])
        PD(idata, base + 12, IDATA_RVA + dll_name_off[d])
        PD(idata, base + 16, IDATA_RVA + iat_off[d])

    imp_i = 0
    for d in range(n_dlls):
        for k in range(dll_count[d]):
            PD(idata, ilt_off[d] + k * 4, IDATA_RVA + hint_off[imp_i])
            PD(idata, iat_off[d] + k * 4, IDATA_RVA + hint_off[imp_i])
            imp_i += 1
    for d in range(n_dlls):
        nm = dll_order[d].encode() + b"\0"
        idata[dll_name_off[d] : dll_name_off[d] + len(nm)] = nm
    imp_i = 0
    for d in range(n_dlls):
        for k in range(dll_count[d]):
            PW(idata, hint_off[imp_i], 0)
            fn = imports[imp_i][0].encode() + b"\0"
            o = hint_off[imp_i] + 2
            idata[o : o + len(fn)] = fn
            iat_abs = IMAGE_BASE + iat_rva[imp_i]
            to = thunk_rva[imp_i] - IDATA_RVA
            idata[to] = 0xFF
            idata[to + 1] = 0x25
            PD(idata, to + 2, iat_abs)
            imp_i += 1

    # apply relocs
    name_to_idx = {fn: i for i, (fn, _) in enumerate(imports)}
    for rva, si, rt, sec, nm in relocs:
        if sec == 0:
            fn = undecorate(nm)
            idx = name_to_idx[fn]
            if rt == 6:  # DIR32
                target = IMAGE_BASE + iat_rva[idx]
            else:  # REL32 etc -> thunk
                target = IMAGE_BASE + thunk_rva[idx]
        else:
            val = struct.unpack_from("<I", data, psym + si * 18 + 8)[0]
            target = IMAGE_BASE + TEXT_RVA + val
        if rt == 0x14:  # REL32
            rel = target - (IMAGE_BASE + TEXT_RVA + rva + 4)
            struct.pack_into("<i", code, rva, rel)
        elif rt == 6:
            struct.pack_into("<I", code, rva, target)

    # headers
    buf = bytearray(SIZEOF_HEADERS)

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
    pd(60, E_LFANEW)
    pd(E_LFANEW, 0x4550)
    pw(E_LFANEW + 4, 0x14C)
    pw(E_LFANEW + 6, 2)
    pw(E_LFANEW + 20, 224)
    pw(E_LFANEW + 22, 0x103)
    opt_off = E_LFANEW + 24
    pw(opt_off, 0x10B)
    buf[opt_off + 2] = 1
    pd(opt_off + 4, text_raw)
    pd(opt_off + 8, idata_raw)
    pd(opt_off + 16, TEXT_RVA)
    pd(opt_off + 20, TEXT_RVA)
    pd(opt_off + 28, IMAGE_BASE)
    pd(opt_off + 32, SECTION_ALIGN)
    pd(opt_off + 36, FILE_ALIGN)
    pw(opt_off + 40, 4)
    pw(opt_off + 48, 4)
    size_image = IDATA_RVA + align_up(idata_virt, SECTION_ALIGN)
    pd(opt_off + 56, size_image)
    pd(opt_off + 60, SIZEOF_HEADERS)
    pw(opt_off + 68, 3)
    pd(opt_off + 72, 0x100000)
    pd(opt_off + 76, 0x1000)
    pd(opt_off + 80, 0x100000)
    pd(opt_off + 84, 0x1000)
    pd(opt_off + 92, 16)
    pd(opt_off + 104, IDATA_RVA + idt_off)
    pd(opt_off + 108, idt_size)
    pd(opt_off + 192, IDATA_RVA + iat_start)
    pd(opt_off + 196, iat_size)
    sect = opt_off + 224
    buf[sect : sect + 5] = b".text"
    pd(sect + 8, text_virt)
    pd(sect + 12, TEXT_RVA)
    pd(sect + 16, text_raw)
    pd(sect + 20, SIZEOF_HEADERS)
    pd(sect + 36, 0x60000020)
    sect2 = sect + 40
    buf[sect2 : sect2 + 7] = b".idata"
    pd(sect2 + 8, idata_virt)
    pd(sect2 + 12, IDATA_RVA)
    pd(sect2 + 16, idata_raw)
    pd(sect2 + 20, SIZEOF_HEADERS + text_raw)
    pd(sect2 + 36, 0x60000020)

    out = bytes(buf) + bytes(code) + bytes(idata)
    Path(out_path).write_bytes(out)
    print("wrote", out_path, "size", len(out))
    print("imports", imports)
    print("text", bytes(code[:16]).hex())
    print("thunk", bytes(idata[thunk_rva[0] - IDATA_RVA : thunk_rva[0] - IDATA_RVA + 6]).hex())
    return out


out = build("exit32.obj", "a_ablinker_sim.exe")
r = subprocess.run([str(Path("a_ablinker_sim.exe").resolve())])
print("exitcode", r.returncode)
