


using ConsoleApp1.Helper;

namespace ConsoleApp1
{
    public class MetadataHelper
    {
        public BlobBuilder ilBuilder { get; } =new BlobBuilder();
        public Guid s_guid { get; set; }
        public MetadataBuilder metadata { get; }

        public MetadataHelper()
        {
            metadata = new MetadataBuilder();
            referenceResolver = new ReferenceResolver(metadata);
        }

        ////public EmitHelper Emitter { get; }
        //public MetadataHelper AddTypeReference()
        //{
        //    return this;
        //}
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
        ReferenceResolver referenceResolver { get; set; } 

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




        public MemberReferenceHandle getConsoleWriteLineMemberRef()
        {
            // Get reference to Console.WriteLine(string) method.
            var consoleWriteLineSignature = new BlobBuilder();

            new BlobEncoder(consoleWriteLineSignature).
                MethodSignature().
                Parameters(1,
                    returnType => returnType.Void(),
                    parameters => parameters.AddParameter().Type().String());
            return metadata.AddMemberReference(
              referenceResolver.GetTypeRef("System","Console"),
               metadata.GetOrAddString("WriteLine"),
               metadata.GetOrAddBlob(consoleWriteLineSignature));

        }
        public (MemberReferenceHandle, BlobHandle) getObjectCtorMemberRef()
        {

            // Get reference to Object's constructor.
            var parameterlessCtorSignature = new BlobBuilder();

            new BlobEncoder(parameterlessCtorSignature).
                MethodSignature(isInstanceMethod: true).
                Parameters(0, returnType => returnType.Void(), parameters => { });

            BlobHandle parameterlessCtorBlobIndex = metadata.GetOrAddBlob(parameterlessCtorSignature);

            return (metadata.AddMemberReference(
                referenceResolver.GetTypeRef("System","Object"),
                metadata.GetOrAddString(".ctor"),
                parameterlessCtorBlobIndex), parameterlessCtorBlobIndex);
        }

        public void AddTypeDefinition(MethodDefinitionHandle mainMethodDef)
        {

            // Create type definition for the special <Module> type that holds global functions
            metadata.AddTypeDefinition(
                default(TypeAttributes),
                default(StringHandle),
                metadata.GetOrAddString("<Module>"),
                baseType: default(EntityHandle),
                fieldList: MetadataTokens.FieldDefinitionHandle(1),
                methodList: mainMethodDef);

            // Create type definition for ConsoleApplication.Program
            metadata.AddTypeDefinition(
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit,
                metadata.GetOrAddString("ConsoleApplication"),
                metadata.GetOrAddString("Program"),
                baseType: referenceResolver.GetTypeRef("System","Object"),
                fieldList: MetadataTokens.FieldDefinitionHandle(1),
                methodList: mainMethodDef);
        }
    }
}
