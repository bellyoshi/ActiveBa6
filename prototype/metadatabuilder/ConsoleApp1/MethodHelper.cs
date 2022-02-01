
namespace ConsoleApp1
{
    public class MethodHelper
    {
        public MetadataBuilder metadata;
        public MethodBodyStreamEncoder methodBodyStream;

        public EmitHelper Emitter()
        {
            return new EmitHelper(this.metadata,this.methodBodyStream);
        }
    }
}
