using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;


namespace ConsoleApp1
{
    public class EmitHelper
    {
        private MetadataBuilder metadataBuilder;

        private MethodBodyStreamEncoder methodBodyStream;
        public BlobBuilder codeBuilder { get; } = new BlobBuilder();
        public ControlFlowBuilder flowBuilder{get;} = new ControlFlowBuilder();
        public InstructionEncoder il { get; }
        public EmitHelper ret { get { il.OpCode(ILOpCode.Ret); return this; } }

        MethodDefinitionHandle MethodDefinitionHandle { get; }
        public EmitHelper(MetadataBuilder metadataBuilder, MethodBodyStreamEncoder methodBodyStream)
    
        {
            this.metadataBuilder = metadataBuilder;
            this.methodBodyStream = methodBodyStream;
            il = new InstructionEncoder(codeBuilder,flowBuilder);
        }
        public EmitHelper ldarg_0 { get { il.LoadArgument(0); return this; } }
        
        public EmitHelper ldstr(string str)
        {
            var usrStrHandle = metadataBuilder.GetOrAddUserString(str);
            il.LoadString(usrStrHandle);
            return this;
        }
        public EmitHelper call(MemberReferenceHandle objectCtorMemberRef) 
        {
            il.Call(objectCtorMemberRef); return this;
        }


        public int AddMethodBody()
        {
            var ret = 
             methodBodyStream.AddMethodBody(il);
            codeBuilder.Clear();return ret;
        }
        
    }
}
