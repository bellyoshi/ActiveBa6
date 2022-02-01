

using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using System.Reflection.Metadata;


namespace ConsoleApp1
{
    public class Class1
    {

        static TypeReferenceHandle systemObjectTypeRef;
        static TypeReferenceHandle GetSystemObjectTypeRef(AssemblyReferenceHandle mscorlibAssemblyRef,string namespaceStr, string name)
        {
            return metadata.AddTypeReference(
                mscorlibAssemblyRef,
                metadata.GetOrAddString(namespaceStr),
                metadata.GetOrAddString(name));
        }
        private static MethodDefinitionHandle EmitHelloWorld(MetadataHelper metadataHelper)
        {

            Class1.metadata = metadataHelper.metadata;
            BlobBuilder ilBuilder = metadataHelper.ilBuilder;
            metadataHelper.s_guid = PEImageCreator.s_guid;
        // Create module and assembly for a console application.

            metadataHelper.AddModule("ConsoleApplication.exe")
                .AddAssembly("ConsoleApplication");


            var mscorlibAssemblyRef = metadataHelper.AddAssemblyReference_mscoreLib();

            systemObjectTypeRef = GetSystemObjectTypeRef(mscorlibAssemblyRef,"System", "Object");

            TypeReferenceHandle systemConsoleTypeRefHandle = metadata.AddTypeReference(
                mscorlibAssemblyRef,
                metadata.GetOrAddString("System"),
                metadata.GetOrAddString("Console"));

            // Get reference to Console.WriteLine(string) method.
            var consoleWriteLineSignature = new BlobBuilder();

            new BlobEncoder(consoleWriteLineSignature).
                MethodSignature().
                Parameters(1,
                    returnType => returnType.Void(),
                    parameters => parameters.AddParameter().Type().String());

            MemberReferenceHandle consoleWriteLineMemberRef = metadata.AddMemberReference(
                systemConsoleTypeRefHandle,
                metadata.GetOrAddString("WriteLine"),
                metadata.GetOrAddBlob(consoleWriteLineSignature));

            // Get reference to Object's constructor.
            var parameterlessCtorSignature = new BlobBuilder();

            new BlobEncoder(parameterlessCtorSignature).
                MethodSignature(isInstanceMethod: true).
                Parameters(0, returnType => returnType.Void(), parameters => { });

            BlobHandle parameterlessCtorBlobIndex = metadata.GetOrAddBlob(parameterlessCtorSignature);

            MemberReferenceHandle objectCtorMemberRef = metadata.AddMemberReference(
                systemObjectTypeRef,
                metadata.GetOrAddString(".ctor"),
                parameterlessCtorBlobIndex);

            // Create signature for "void Main()" method.
            var mainSignature = new BlobBuilder();

            new BlobEncoder(mainSignature).
                MethodSignature().
                Parameters(0, returnType => returnType.Void(), parameters => { });

            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);

            var emit = new EmitHelper(metadata, methodBodyStream);

            var ctorBodyOffset = 
            emit
                .ldarg_0
                .call(objectCtorMemberRef)
                .ret
                .AddMethodBody();
            ;
            // Create method definition for Program::.ctor
            MethodDefinitionHandle ctorDef = metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                MethodImplAttributes.IL,
                metadata.GetOrAddString(".ctor"),
                parameterlessCtorBlobIndex,
                ctorBodyOffset,
                parameterList: default(ParameterHandle));

            var mainBodyOffset = emit
                .ldstr("Hello MSIL")
                .call(consoleWriteLineMemberRef)
                .ret
                .AddMethodBody();


            // Create method definition for Program::Main
            MethodDefinitionHandle mainMethodDef = metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                MethodImplAttributes.IL,
                metadata.GetOrAddString("Main"),
                metadata.GetOrAddBlob(mainSignature),
                mainBodyOffset,
                parameterList: default(ParameterHandle));

            AddTypeDefinition(mainMethodDef);


            return mainMethodDef;
        }
        static MetadataBuilder metadata;
        public static void AddTypeDefinition(MethodDefinitionHandle mainMethodDef)
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

       

        public static void BuildHelloWorldApp(string exename)
        {
            PEImageCreator pEImageCreator = new PEImageCreator(exename);

            var entryPoint = EmitHelloWorld(pEImageCreator.metadataHelper);
            pEImageCreator.Create(entryPoint);
        }
        public static string RunApp(string exename)
        {

            using var process = new System.Diagnostics.Process();

            process.StartInfo.FileName = exename;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();


            process.WaitForExit();


            return output;
        }
        public static string BuildAndRun(string exename)
        {
            BuildHelloWorldApp(exename);
            return RunApp(exename);
            
        }

    }

}
