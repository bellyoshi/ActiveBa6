﻿


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
            metadataHelper.s_guid = PEImageCreator.s_guid;
            metadataHelper.AddModule("ConsoleApplication.exe")
                .AddAssembly("ConsoleApplication");

            var emit = metadataHelper.GetEmit(); 

            //.ctor
            emit
                .ldarg_0
                .call(metadataHelper.Constructor())
                .ret
                .MethodDefinition(".ctor", GetVoidSignature());
            ;

            //Main
            var mainMethodDef =
             emit
                 .ldstr("Hello MSIL")
                 .call(metadataHelper.ConsoleWriteLine())
                 .ldstr("PRINT MESSAGE")
                 .call(metadataHelper.ConsoleWriteLine())
                .ret
                .MethodDefinition("Main",GetVoidSignature());

            metadataHelper.AddTypeDefinition(mainMethodDef);

            return mainMethodDef;
        }

        private BlobBuilder GetVoidSignature()
        {
            var blob =new BlobBuilder();

            new BlobEncoder(blob).
                MethodSignature().
                Parameters(0, returnType => returnType.Void(), parameters => { });
            return blob;
        }



        public Byte[] BuildHelloWorldAppInMemory()
        {
            var memory = new MemoryStream();
            var entryPoint = EmitHelloWorld(peImageCreator.metadataHelper);
            peImageCreator.stream = memory;
            peImageCreator.Create(entryPoint);
            return memory.ToArray();
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
