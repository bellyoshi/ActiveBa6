
using System.Reflection.PortableExecutable;

namespace ConsoleApp1
{
    public class Class3
    {
        private static readonly Guid s_guid = new Guid("87D4DBE1-1143-4FAD-AAB3-1001F92068E6");
        private static readonly BlobContentId s_contentId = new BlobContentId(s_guid, 0x04030201);

        private static MethodDefinitionHandle EmitHelloWorld(MetadataBuilder metadata, BlobBuilder ilBuilder)
        {
            // Create module and assembly for a console application.
            metadata.AddModule(
                0,
                metadata.GetOrAddString("ConsoleApplication.exe"),
                metadata.GetOrAddGuid(s_guid),
                default(GuidHandle),
                default(GuidHandle));

            metadata.AddAssembly(
                metadata.GetOrAddString("ConsoleApplication"),
                version: new Version(1, 0, 0, 0),
                culture: default(StringHandle),
                publicKey: default(BlobHandle),
                flags: 0,
                hashAlgorithm: AssemblyHashAlgorithm.None);

            // Create references to System.Object and System.Console types.
            //AssemblyReferenceHandle mscorlibAssemblyRef = metadata.AddAssemblyReference(
            //    name: metadata.GetOrAddString("mscorlib"),
            //    version: new Version(4, 0, 0, 0),
            //    culture: default(StringHandle),
            //    publicKeyOrToken: metadata.GetOrAddBlob(
            //        new byte[] { 0xB7, 0x7A, 0x5C, 0x56, 0x19, 0x34, 0xE0, 0x89 }
            //        ),
            //    flags: default(AssemblyFlags),
            //    hashValue: default(BlobHandle));

            AssemblyReferenceHandle mscorlibAssemblyRef = metadata.AddAssemblyReference(
                metadata.GetOrAddString("System.Private.CoreLib"), 
                new Version(6, 0, 0, 0), default,
                metadata.GetOrAddBlob(
                    new byte[] { 0x7C, 0xEC, 0x85, 0xD7, 0xBE, 0xA7, 0x79, 0x8E }), 
                default,
                default);


            TypeReferenceHandle systemObjectTypeRef = metadata.AddTypeReference(
                mscorlibAssemblyRef,
                metadata.GetOrAddString("System"),
                metadata.GetOrAddString("Object"));

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

            var codeBuilder = new BlobBuilder();
            InstructionEncoder il;

            // Emit IL for Program::.ctor
            il = new InstructionEncoder(codeBuilder);

            // ldarg.0
            il.LoadArgument(0);

            // call instance void [mscorlib]System.Object::.ctor()
            il.Call(objectCtorMemberRef);

            // ret
            il.OpCode(ILOpCode.Ret);

            int ctorBodyOffset = methodBodyStream.AddMethodBody(il);
            codeBuilder.Clear();

            // Emit IL for Program::Main
            var flowBuilder = new ControlFlowBuilder();
            il = new InstructionEncoder(codeBuilder, flowBuilder);

            // ldstr "hello"
            il.LoadString(metadata.GetOrAddUserString("Hello MSIL"));

            // call void [mscorlib]System.Console::WriteLine(string)
            il.Call(consoleWriteLineMemberRef);

            // ret
            il.OpCode(ILOpCode.Ret);

            int mainBodyOffset = methodBodyStream.AddMethodBody(il);
            codeBuilder.Clear();

            // Create method definition for Program::Main
            MethodDefinitionHandle mainMethodDef = metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                MethodImplAttributes.IL,
                metadata.GetOrAddString("Main"),
                metadata.GetOrAddBlob(mainSignature),
                mainBodyOffset,
                parameterList: default(ParameterHandle));

            // Create method definition for Program::.ctor
            MethodDefinitionHandle ctorDef = metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                MethodImplAttributes.IL,
                metadata.GetOrAddString(".ctor"),
                parameterlessCtorBlobIndex,
                ctorBodyOffset,
                parameterList: default(ParameterHandle));

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

            return mainMethodDef;
        }

        private static void WritePEImage(
            Stream peStream,
            MetadataBuilder metadataBuilder,
            BlobBuilder ilBuilder,
            MethodDefinitionHandle entryPointHandle
            )
        {
            // Create executable with the managed metadata from the specified MetadataBuilder.
            var peHeaderBuilder = new PEHeaderBuilder(
                imageCharacteristics: Characteristics.ExecutableImage
                );

            var peBuilder = new ManagedPEBuilder(
                peHeaderBuilder,
                new MetadataRootBuilder(metadataBuilder),
                ilBuilder,
                entryPoint: entryPointHandle,
                flags: CorFlags.ILOnly,
                deterministicIdProvider: content => s_contentId);

            // Write executable into the specified stream.
            var peBlob = new BlobBuilder();
            BlobContentId contentId = peBuilder.Serialize(peBlob);
            peBlob.WriteContentTo(peStream);
        }

        public static void BuildHelloWorldApp(string exename)
        {
            using var peStream = new FileStream(
                exename, FileMode.OpenOrCreate, FileAccess.ReadWrite
                );
            var ilBuilder = new BlobBuilder();
            var metadataBuilder = new MetadataBuilder();

            MethodDefinitionHandle entryPoint = EmitHelloWorld(metadataBuilder, ilBuilder);
            WritePEImage(peStream, metadataBuilder, ilBuilder, entryPoint);
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
        public static void BuildAndRun()
        {
            string exename = "class3.exe";
            BuildHelloWorldApp(exename);
            Console.WriteLine(RunApp(exename));

        }
    }
}
