using System.Reflection.Metadata.Ecma335;
using System.Reflection.Metadata;

namespace ConsoleApp1
{
    public class MetadataHelper
    {
        public BlobBuilder ilBuilder = new BlobBuilder();
        public MetadataBuilder metadataBuilder = new MetadataBuilder();

        //public EmitHelper Emitter { get; }
        public MetadataHelper AddTypeReference()
        {
            return this;
        }
    }
}
