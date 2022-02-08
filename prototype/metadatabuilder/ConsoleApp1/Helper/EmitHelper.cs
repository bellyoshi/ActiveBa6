


namespace ConsoleApp1
{
    public class EmitHelper
    {
        private MetadataBuilder metadata;

        private MethodBodyStreamEncoder methodBodyStream;
        public BlobBuilder codeBuilder { get; } = new BlobBuilder();
        public ControlFlowBuilder flowBuilder{get;} = new ControlFlowBuilder();
        public InstructionEncoder il { get; }
        public EmitHelper ret { get { il.OpCode(ILOpCode.Ret); return this; } }

        public EmitHelper(MetadataBuilder metadataBuilder, BlobBuilder ilBuilder)
    
        {
            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);
            this.metadata = metadataBuilder;
            this.methodBodyStream = methodBodyStream;
            il = new InstructionEncoder(codeBuilder,flowBuilder);
        }
        public EmitHelper ldarg_0 { get { il.LoadArgument(0); return this; } }
        
        public EmitHelper ldstr(string str)
        {
            var usrStrHandle = metadata.GetOrAddUserString(str);
            il.LoadString(usrStrHandle);
            return this;
        }
        public EmitHelper call(MemberReferenceHandle objectCtorMemberRef) 
        {
            il.Call(objectCtorMemberRef); return this;
        }
        private int AddMethodBody()
        {
            var ret = methodBodyStream.AddMethodBody(il);
            codeBuilder.Clear();
            return ret;
        }

        public void CtorDefinition(BlobBuilder mainSignature)
        {
            var ctorBodyOffset = AddMethodBody();
            MethodDefinitionHandle ctorDef = metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                MethodImplAttributes.IL,
                metadata.GetOrAddString(".ctor"),
                metadata.GetOrAddBlob(mainSignature),
                ctorBodyOffset,
                parameterList: default(ParameterHandle));
        }

        internal MethodDefinitionHandle MethodDefinition(string MethodName, BlobBuilder mainSignature)
        {
            var mainBodyOffset = AddMethodBody();
            MethodDefinitionHandle mainMethodDef = metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                MethodImplAttributes.IL,
                metadata.GetOrAddString(MethodName),
                metadata.GetOrAddBlob(mainSignature),
                mainBodyOffset,
                parameterList: default(ParameterHandle));
           return mainMethodDef;
        }
    }
}
