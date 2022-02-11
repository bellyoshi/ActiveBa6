
namespace ConsoleApp1.Helper
{
    public class ReferenceResolver
    {
        MetadataBuilder metadata;
        AssemblyReferenceHandle mscorlibAssemblyRef;
        Dictionary<(string,string), TypeReferenceHandle> references = new Dictionary<(string, string), TypeReferenceHandle>();
        public ReferenceResolver(MetadataBuilder metadata)
        {
            this.metadata = metadata;
            mscorlibAssemblyRef = metadata.AddAssemblyReference(
                name: metadata.GetOrAddString("mscorlib"),
                version: new Version(4, 0, 0, 0),
                culture: default(StringHandle),
                publicKeyOrToken: metadata.GetOrAddBlob(
                    new byte[] { 0xB7, 0x7A, 0x5C, 0x56, 0x19, 0x34, 0xE0, 0x89 }
                    ),
                flags: default(AssemblyFlags),
                hashValue: default(BlobHandle));

        }
        public AssemblyReferenceHandle GetHandle()
        {
            return mscorlibAssemblyRef;
        }



        private TypeReferenceHandle GetTypeRef(AssemblyReferenceHandle assemblyRef, string namespaceStr, string name)
        {
            return metadata.AddTypeReference(
                assemblyRef,
                metadata.GetOrAddString(namespaceStr),
                metadata.GetOrAddString(name))
            ;
        }
        public TypeReferenceHandle GetTypeRef(string namespacestr, string name)
        {
            var key = (namespacestr, name);
            if (!references.ContainsKey(key))
            {
                var value = GetTypeRef(GetHandle(), namespacestr ,name);
                references.Add(key, value);
            }
            return references[key];
         
        }
    }
}
