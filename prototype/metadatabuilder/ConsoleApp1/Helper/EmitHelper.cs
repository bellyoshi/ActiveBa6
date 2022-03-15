


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
        private BlobEncoder blobEncoder;
        private LocalVariablesEncoder sig;
        public EmitHelper(MetadataHelper metadataHelper, BlobBuilder ilBuilder)
    
        {
            var methodBodyStream = new MethodBodyStreamEncoder(ilBuilder);
            this._metadataHelper = metadataHelper;
            this.methodBodyStream = methodBodyStream;
            codeBuilder = new BlobBuilder();
            flowBuilder = new ControlFlowBuilder();
            il = new InstructionEncoder(codeBuilder,flowBuilder);
            blobEncoder = new BlobEncoder(new BlobBuilder());
        }
        //public EmitHelper Local(int num)
        //{

        //    return this;
        //}

        public EmitHelper StoreLocal(int v)
        {
            il.StoreLocal(v);
            return this;
        }

        public EmitHelper LoadLocal(int v)
        {
            il.LoadLocal(v);
            return this;
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
            int maxStack = 8;
            sig = blobEncoder.LocalVariableSignature(1);
            sig.AddVariable().Type(false,true).Int32();
            var value = blobEncoder.Builder;
            StandaloneSignatureHandle localVariablesSignature = _metadata.AddStandaloneSignature(
                _metadata.GetOrAddBlob(value)
                );
            var ret = methodBodyStream.AddMethodBody(il, maxStack, localVariablesSignature);
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

        public EmitHelper sub()
        {
            il.OpCode(ILOpCode.Sub);
            return this;
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

        public EmitHelper mul()
        {
            il.OpCode(ILOpCode.Mul);
            return this;
        }

        public EmitHelper div()
        {
            il.OpCode(ILOpCode.Div);
            return this;
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

            if(returnType == "void")
            {
                return MethodDefinition(name, GetVoidSignature());
            }
            throw new NotImplementedException();//todo:
        }

        public MethodDefinitionHandle MethodDefinition(string MethodName, BlobBuilder MethodSignature)
        {
            var bodyOffset = AddMethodBody();
            MethodDefinitionHandle methodDef = _metadata.AddMethodDefinition(
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                MethodImplAttributes.IL,
                _metadata.GetOrAddString(MethodName),
                _metadata.GetOrAddBlob(MethodSignature),
                bodyOffset,
                parameterList: default(ParameterHandle));
           return methodDef;
        }
    }
}
