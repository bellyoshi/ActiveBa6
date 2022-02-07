
namespace ConsoleApp1.Helper
{
    public class ReferenceResolver
    {
        MetadataBuilder metadata;
        AssemblyReferenceHandle mscorlibAssemblyRef;
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

            systemObjectTypeRef = GetTypeRef("System", "Object");

            systemConsoleTypeRefHandle = GetTypeRef("System", "Console");
        }
        public AssemblyReferenceHandle GetHandle()
        {
            return mscorlibAssemblyRef;
        }

        public TypeReferenceHandle systemObjectTypeRef;
        public TypeReferenceHandle systemConsoleTypeRefHandle;
        TypeReferenceHandle GetTypeRef(AssemblyReferenceHandle assemblyRef, string namespaceStr, string name)
        {
            return metadata.AddTypeReference(
                assemblyRef,
                metadata.GetOrAddString(namespaceStr),
                metadata.GetOrAddString(name));
        }
        TypeReferenceHandle GetTypeRef(string namespacestr, string name)
        {
            var assemblyRef = GetHandle();
            return GetTypeRef(assemblyRef, namespacestr, name); 
        }
    }
}
