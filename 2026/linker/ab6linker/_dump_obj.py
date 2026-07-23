import struct
from pathlib import Path

def symname(data, psym, nsym, i):
    e = data[psym + i * 18 : psym + (i + 1) * 18]
    strtab = psym + nsym * 18
    if e[0:4] == b"\x00\x00\x00\x00":
        o = struct.unpack_from("<I", e, 4)[0]
        return data[strtab + o :].split(b"\x00", 1)[0].decode()
    return e[:8].split(b"\x00", 1)[0].decode()

for path in ["exit32.obj", "hello.obj"]:
    data = Path(path).read_bytes()
    machine, nsect, ts, psym, nsym, opt, ch = struct.unpack_from("<HHIIIHH", data, 0)
    print(path, "machine", hex(machine), "nsym", nsym)
    off = 20 + opt
    text = None
    for i in range(nsect):
        name, vsize, va, rawsz, praw, preloc, _, nreloc, _, sch = struct.unpack_from(
            "<8sIIIIIIHHI", data, off
        )
        sname = name.split(b"\0", 1)[0].decode()
        print(f"  sect {sname!r} raw={rawsz} praw={praw:#x} nreloc={nreloc}")
        if sname == ".text":
            text = (rawsz, praw, preloc, nreloc)
            print("   bytes", data[praw : praw + rawsz].hex())
        off += 40
    if text:
        rawsz, praw, preloc, nreloc = text
        for j in range(nreloc):
            rva, si, rt = struct.unpack_from("<IIH", data, preloc + j * 10)
            sec = struct.unpack_from("<H", data, psym + si * 18 + 12)[0]
            print(
                f"   reloc va={rva} type={rt}({rt:#x}) "
                f"sym={symname(data, psym, nsym, si)!r} sec={sec}"
            )
    print("  symbols:")
    i = 0
    while i < nsym:
        e = data[psym + i * 18 : psym + (i + 1) * 18]
        val, sec, typ, sc, naux = struct.unpack_from("<IHHBB", e, 8)
        print(
            f"   [{i}] {symname(data, psym, nsym, i)!r} "
            f"val={val} sec={sec} class={sc} aux={naux}"
        )
        i += 1 + naux
    print()
