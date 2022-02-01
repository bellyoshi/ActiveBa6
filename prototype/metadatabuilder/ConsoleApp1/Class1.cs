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
        private MethodDefinitionHandle EmitHelloWorld(MetadataHelper metadataHelper)
        {

            metadataHelper.Init();
            BlobBuilder ilBuilder = metadataHelper.ilBuilder;
            metadataHelper.s_guid = PEImageCreator.s_guid;
            metadataHelper.AddModule("ConsoleApplication.exe")
                .AddAssembly("ConsoleApplication");

            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);

            var emit = new EmitHelper(metadataHelper.metadata, methodBodyStream);

            //.ctor
            var (objectCtorMemberRef, parameterlessCtorBlobIndex) = metadataHelper.getObjectCtorMemberRef();
            emit
                .ldarg_0
                .call(objectCtorMemberRef)
                .ret
                .CtorDefinition(parameterlessCtorBlobIndex);
            ;

            //Main
            var mainMethodDef =
             emit
                 .ldstr("Hello MSIL")
                 .call(metadataHelper.getConsoleWriteLineMemberRef())
                .ret
                .MethodDefinition("Main",GetMainSignature());




            metadataHelper.AddTypeDefinition(mainMethodDef);


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
