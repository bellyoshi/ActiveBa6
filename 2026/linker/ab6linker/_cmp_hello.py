import struct
from pathlib import Path


def pe_info(path):
    data = Path(path).read_bytes()
    print("=" * 60)
    print(path, "size", len(data))
    if data[0:2] != b"MZ":
        print("NOT MZ")
        return
    e = struct.unpack_from("<I", data, 0x3C)[0]
    print("e_lfanew", hex(e), "PE", data[e : e + 4])
    coff = e + 4
    machine, nsect, _, _, _, optsz, chars = struct.unpack_from("<HHIIIHH", data, coff)
    print(f"machine={machine:#x} nsect={nsect} optsz={optsz} chars={chars:#x}")
    opt = coff + 20
    magic = struct.unpack_from("<H", data, opt)[0]
    entry = struct.unpack_from("<I", data, opt + 16)[0]
    imagebase = struct.unpack_from("<I", data, opt + 28)[0]
    sa = struct.unpack_from("<I", data, opt + 32)[0]
    fa = struct.unpack_from("<I", data, opt + 36)[0]
    soimage = struct.unpack_from("<I", data, opt + 56)[0]
    soheaders = struct.unpack_from("<I", data, opt + 60)[0]
    subsys = struct.unpack_from("<H", data, opt + 68)[0]
    print(
        f"magic={magic:#x} entry={entry:#x} imagebase={imagebase:#x} "
        f"sa={sa:#x} fa={fa:#x} sizeImage={soimage:#x} sizeHdr={soheaders:#x} sub={subsys}"
    )
    dd = opt + 96
    for i, n in enumerate(
        ["Exp", "Imp", "Res", "Exc", "Sec", "Rel", "Dbg", "A", "GP", "TLS", "LC", "BI", "IAT"]
    ):
        rva, sz = struct.unpack_from("<II", data, dd + i * 8)
        if rva or sz:
            print(f"  DD[{i}] {n}: rva={rva:#x} size={sz:#x}")
    sect = opt + optsz
    for i in range(nsect):
        name = data[sect + i * 40 : sect + i * 40 + 8].split(b"\0", 1)[0].decode()
        vsize, va, rawsz, praw = struct.unpack_from("<IIII", data, sect + i * 40 + 8)
        ch = struct.unpack_from("<I", data, sect + i * 40 + 36)[0]
        print(
            f"  sect {name!r} vsize={vsize:#x} va={va:#x} rawsz={rawsz:#x} "
            f"praw={praw:#x} ch={ch:#x}"
        )
        raw = data[praw : praw + min(rawsz, 64)]
        print("    raw:", raw.hex())
        # ascii snippets
        s = "".join(chr(b) if 32 <= b < 127 else "." for b in raw)
        print("    asc:", s)

    # dump import names if possible
    imp_rva, imp_sz = struct.unpack_from("<II", data, dd + 8)
    if imp_rva:

        def rva_to_off(rva):
            for i in range(nsect):
                name = data[sect + i * 40 : sect + i * 40 + 8]
                vsize, va, rawsz, praw = struct.unpack_from(
                    "<IIII", data, sect + i * 40 + 8
                )
                if va <= rva < va + max(vsize, rawsz):
                    return praw + (rva - va)
            return None

        off = rva_to_off(imp_rva)
        print("  imports @ file", hex(off) if off is not None else None)
        if off is not None:
            while True:
                oft, td, fc, name_rva, ft = struct.unpack_from("<IIIII", data, off)
                if oft == 0 and name_rva == 0 and ft == 0:
                    break
                no = rva_to_off(name_rva)
                dll = (
                    data[no:].split(b"\0", 1)[0].decode()
                    if no is not None
                    else "?"
                )
                print(f"   DLL {dll} OFT={oft:#x} FT={ft:#x}")
                # walk IAT
                iat = rva_to_off(ft)
                if iat is not None:
                    k = 0
                    while True:
                        entry = struct.unpack_from("<I", data, iat + k * 4)[0]
                        if entry == 0:
                            break
                        if entry & 0x80000000:
                            print(f"     ordinal {entry & 0xFFFF}")
                        else:
                            ho = rva_to_off(entry)
                            if ho is not None:
                                hint = struct.unpack_from("<H", data, ho)[0]
                                nm = data[ho + 2 :].split(b"\0", 1)[0].decode()
                                print(f"     [{k}] hint={hint} {nm}")
                        k += 1
                off += 20


def obj_info(path):
    data = Path(path).read_bytes()
    print("=" * 60)
    print("OBJ", path, "size", len(data))
    machine, nsect, ts, psym, nsym, opt, ch = struct.unpack_from("<HHIIIHH", data, 0)
    print(f"machine={machine:#x} nsect={nsect} nsym={nsym}")

    def symname(i):
        e = data[psym + i * 18 : psym + (i + 1) * 18]
        strtab = psym + nsym * 18
        if e[0:4] == b"\x00\x00\x00\x00":
            o = struct.unpack_from("<I", e, 4)[0]
            return data[strtab + o :].split(b"\0", 1)[0].decode()
        return e[:8].split(b"\0", 1)[0].decode()

    off = 20 + opt
    for i in range(nsect):
        name, vsize, va, rawsz, praw, preloc, _, nreloc, _, sch = struct.unpack_from(
            "<8sIIIIIIHHI", data, off
        )
        sname = name.split(b"\0", 1)[0].decode()
        print(f"  {sname} raw={rawsz} praw={praw:#x} nreloc={nreloc} ch={sch:#x}")
        print("   ", data[praw : praw + rawsz].hex())
        for j in range(nreloc):
            rva, si, rt = struct.unpack_from("<IIH", data, preloc + j * 10)
            sec = struct.unpack_from("<H", data, psym + si * 18 + 12)[0]
            print(
                f"     reloc va={rva} type={rt:#x} sym={symname(si)!r} sec={sec}"
            )
        off += 40
    print("  symbols:")
    i = 0
    while i < nsym:
        e = data[psym + i * 18 : psym + (i + 1) * 18]
        val, sec, typ, sc, naux = struct.unpack_from("<IHHBB", e, 8)
        print(f"   [{i}] {symname(i)!r} val={val} sec={sec} class={sc} aux={naux}")
        i += 1 + naux


obj_info("hello32.obj")
pe_info("hello32.exe")
pe_info("h.exe")
