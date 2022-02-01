global using System.Reflection.Metadata.Ecma335;
global using System.Reflection;
global using System.Reflection.Metadata;


namespace ConsoleApp1
{
    public class Class1
    {
        PEImageCreator peImageCreator;

        public Class1(string exename)
        {
            peImageCreator = new PEImageCreator(exename);   

        }
         AssemblyReferenceHandle mscorlibAssemblyRef;
         TypeReferenceHandle systemObjectTypeRef;
         TypeReferenceHandle systemConsoleTypeRefHandle;
        TypeReferenceHandle GetTypeRef(AssemblyReferenceHandle assemblyRef,string namespaceStr, string name)
        {
            return metadata.AddTypeReference(
                assemblyRef,
                metadata.GetOrAddString(namespaceStr),
                metadata.GetOrAddString(name));
        }
        private MemberReferenceHandle getConsoleWriteLineMemberRef()
        {
            // Get reference to Console.WriteLine(string) method.
            var consoleWriteLineSignature = new BlobBuilder();

            new BlobEncoder(consoleWriteLineSignature).
                MethodSignature().
                Parameters(1,
                    returnType => returnType.Void(),
                    parameters => parameters.AddParameter().Type().String());
            return metadata.AddMemberReference(
               systemConsoleTypeRefHandle,
               metadata.GetOrAddString("WriteLine"),
               metadata.GetOrAddBlob(consoleWriteLineSignature));

        }
        private MethodDefinitionHandle EmitHelloWorld(MetadataHelper metadataHelper)
        {

            metadata = metadataHelper.metadata;
            BlobBuilder ilBuilder = metadataHelper.ilBuilder;
            metadataHelper.s_guid = PEImageCreator.s_guid;
        // Create module and assembly for a console application.

            metadataHelper.AddModule("ConsoleApplication.exe")
                .AddAssembly("ConsoleApplication");


            mscorlibAssemblyRef = metadataHelper.AddAssemblyReference_mscoreLib();

            systemObjectTypeRef = GetTypeRef(mscorlibAssemblyRef,"System", "Object");

            systemConsoleTypeRefHandle = GetTypeRef(mscorlibAssemblyRef, "System", "Console");




            MemberReferenceHandle consoleWriteLineMemberRef = getConsoleWriteLineMemberRef();
            var( objectCtorMemberRef, parameterlessCtorBlobIndex )= getObjectCtorMemberRef();

            // Create signature for "void Main()" method.
            var mainSignature = GetMainSignature(); 

            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);

            var emit = new EmitHelper(metadata, methodBodyStream);

            emit
                .ldarg_0
                .call(objectCtorMemberRef)
                .ret
                .CtorDefinition(parameterlessCtorBlobIndex);
            ;
            // Create method definition for Program::.ctor

           var mainMethodDef =
            emit
                .ldstr("Hello MSIL")
                .call(consoleWriteLineMemberRef)
                .ret
                .MethodDefinition("Main",GetMainSignature());




            AddTypeDefinition(mainMethodDef);


            return mainMethodDef;
        }

        private BlobBuilder GetMainSignature()
        {
            var mainSignature =new BlobBuilder();

            new BlobEncoder(mainSignature).
                MethodSignature().
                Parameters(0, returnType => returnType.Void(), parameters => { });
            return mainSignature;
        }

        private (MemberReferenceHandle, BlobHandle) getObjectCtorMemberRef()
        {

            // Get reference to Object's constructor.
            var parameterlessCtorSignature = new BlobBuilder();

            new BlobEncoder(parameterlessCtorSignature).
                MethodSignature(isInstanceMethod: true).
                Parameters(0, returnType => returnType.Void(), parameters => { });

            BlobHandle parameterlessCtorBlobIndex = metadata.GetOrAddBlob(parameterlessCtorSignature);

            return  (metadata.AddMemberReference(
                systemObjectTypeRef,
                metadata.GetOrAddString(".ctor"),
                parameterlessCtorBlobIndex), parameterlessCtorBlobIndex);
        }

        MetadataBuilder metadata;
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
                baseType: systemObjectTypeRef,
                fieldList: MetadataTokens.FieldDefinitionHandle(1),
                methodList: mainMethodDef);
        }


        public void BuildHelloWorldApp()
        {

            var entryPoint = EmitHelloWorld(peImageCreator.metadataHelper);
            peImageCreator.Create(entryPoint);
        }
        public string RunApp()
        {

            using var process = new System.Diagnostics.Process();

            process.StartInfo.FileName = peImageCreator.exename;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();


            process.WaitForExit();


            return output;
        }
        public string BuildAndRun()
        {
            BuildHelloWorldApp();
            return RunApp();
            
        }

    }

}
