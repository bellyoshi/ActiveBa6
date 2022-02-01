


namespace ConsoleApp1
{
    public class MetadataHelper
    {
        public BlobBuilder ilBuilder { get; } =new BlobBuilder();
        public Guid s_guid { get; set; }
        public MetadataBuilder metadata { get; } = new MetadataBuilder();

        //public EmitHelper Emitter { get; }
        public MetadataHelper AddTypeReference()
        {
            return this;
        }
        public MetadataHelper AddAssembly(string assemblyName)
        {
            metadata.AddAssembly(
    metadata.GetOrAddString(assemblyName),
    version: new Version(1, 0, 0, 0),
    culture: default(StringHandle),
    publicKey: default(BlobHandle),
    flags: 0,
    hashAlgorithm: AssemblyHashAlgorithm.None)
           ;return this;
        }
        public MetadataHelper AddModule(string moduleName)
        {
                metadata.AddModule(
                0,
                metadata.GetOrAddString(moduleName),
                metadata.GetOrAddGuid(s_guid),
                default(GuidHandle),
                default(GuidHandle));
            return this;
        }

        public AssemblyReferenceHandle AddAssemblyReference_mscoreLib()
        {
            // Create references to System.Object and System.Console types.
            AssemblyReferenceHandle mscorlibAssemblyRef = metadata.AddAssemblyReference(
                name: metadata.GetOrAddString("mscorlib"),
                version: new Version(4, 0, 0, 0),
                culture: default(StringHandle),
                publicKeyOrToken: metadata.GetOrAddBlob(
                    new byte[] { 0xB7, 0x7A, 0x5C, 0x56, 0x19, 0x34, 0xE0, 0x89 }
                    ),
                flags: default(AssemblyFlags),
                hashValue: default(BlobHandle));
            return mscorlibAssemblyRef;
        }
    }
}
