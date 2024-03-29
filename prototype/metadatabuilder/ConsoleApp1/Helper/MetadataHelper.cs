﻿


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

        public MetadataHelper AddAssembly(string assemblyName)
        {
            metadata.AddAssembly(
            metadata.GetOrAddString(assemblyName),
            version: new Version(1, 0, 0, 0),
            culture: default(StringHandle),
            publicKey: default(BlobHandle),
            flags: 0,
            hashAlgorithm: AssemblyHashAlgorithm.None);
            return this;
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

        public MemberReferenceHandle GetMemberRef(string namespacestr, string typenamestr, string methodnamestr,int parameterCount,
            Action<ReturnTypeEncoder> returnType, Action<ParametersEncoder> parameters)
        {
            
            var signature = new BlobBuilder();

            new BlobEncoder(signature).
                MethodSignature().
                Parameters(parameterCount, returnType, parameters);
            return metadata.AddMemberReference(
                referenceResolver.GetTypeRef(namespacestr, typenamestr),
               metadata.GetOrAddString(methodnamestr),
               metadata.GetOrAddBlob(signature));
        }


        public MemberReferenceHandle ConsoleWriteLine()
        {
            return GetMemberRef("System","Console","WriteLine",1, returnType => returnType.Void(),
                    parameters => parameters.AddParameter().Type().String());
                
        }
        public MemberReferenceHandle Constructor()
        {
            return GetMemberRef("System", "Object", ".ctor", 0, returnType => returnType.Void(),
                parameters => { });
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
        public EmitHelper GetEmit()
        {
            return new EmitHelper(this, ilBuilder);
        }
    }
}
