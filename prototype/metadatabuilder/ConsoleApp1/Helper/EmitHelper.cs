


namespace ConsoleApp1.Helper
{
    public class EmitHelper
    {
        private MetadataBuilder _metadata => _metadataHelper.metadata;
        private MetadataHelper _metadataHelper;
        private MethodBodyStreamEncoder methodBodyStream;
        private BlobBuilder codeBuilder;
        private ControlFlowBuilder flowBuilder;
        private InstructionEncoder il;

        public EmitHelper(MetadataHelper metadataHelper, BlobBuilder ilBuilder)
    
        {
            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);
            this._metadataHelper = metadataHelper;
            this.methodBodyStream = methodBodyStream;
            codeBuilder = new BlobBuilder();
            flowBuilder = new ControlFlowBuilder();
            il = new InstructionEncoder(codeBuilder,flowBuilder);

        }
        public EmitHelper ldarg_0 { 
            get {
                il.LoadArgument(0);
                return this;
            }
        }
        
        public EmitHelper ldstr(string str)
        {
            var usrStrHandle = _metadata.GetOrAddUserString(str);
            il.LoadString(usrStrHandle);
            return this;
        }
        public EmitHelper call(MemberReferenceHandle memberRef) 
        {
            il.Call(memberRef); 
            return this;
        }
        public EmitHelper call(string ret, string fullMethodName, params string[] parameters)
        {
            var names = fullMethodName.Split('.');
            var nameSpaceName = names[0];
            var typeName = names[1];
            var methodName = names[2];

            var paramHelper = new ParametersEncorderHelper() { parameterList = parameters};
           var memberref =  _metadataHelper.GetMemberRef(nameSpaceName, typeName, methodName,
               parameters.Count(), returnType => returnType.Void(),
         paramHelper.ParameterAction );

            //todo:: return , parameter
            return call(memberref);


        }
        public EmitHelper ret {
            get {
                il.OpCode(ILOpCode.Ret);
                return this;
            }
        }

        private int AddMethodBody()
        {
            var ret = methodBodyStream.AddMethodBody(il);
            codeBuilder.Clear();
            return ret;
        }

        public EmitHelper ldc(int num)
        {
            il.LoadConstantI4(num);
            return this;
        }

        private BlobBuilder GetVoidSignature()
        {
            var blob = new BlobBuilder();

            new BlobEncoder(blob).
                MethodSignature().
                Parameters(0, returnType => returnType.Void(), parameters => { });
            return blob;
        }

        public EmitHelper add()
        {
            il.OpCode(ILOpCode.Add);
            return this;
        }

        public void CtorDefinition()
        {
            CtorDefinition(GetVoidSignature());
        }
        public void CtorDefinition(BlobBuilder signature)
        {
            var ctorBodyOffset = AddMethodBody();
            MethodDefinitionHandle ctorDef = _metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                MethodImplAttributes.IL,
                _metadata.GetOrAddString(".ctor"),
                _metadata.GetOrAddBlob(signature),
                ctorBodyOffset,
                parameterList: default(ParameterHandle));
        }

        public MethodDefinitionHandle MethodDefinition(string returnType, string name)
        {
            return MethodDefinition(name, GetVoidSignature());
        }

        public MethodDefinitionHandle MethodDefinition(string MethodName, BlobBuilder mainSignature)
        {
            var mainBodyOffset = AddMethodBody();
            MethodDefinitionHandle mainMethodDef = _metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                MethodImplAttributes.IL,
                _metadata.GetOrAddString(MethodName),
                _metadata.GetOrAddBlob(mainSignature),
                mainBodyOffset,
                parameterList: default(ParameterHandle));
           return mainMethodDef;
        }
    }
}
