using System.Reflection.PortableExecutable;

namespace ConsoleApp1
{
    public class PEImageCreator
    {
        public string exename { get; }
        public PEImageCreator(string exename)
        {
            this.exename = exename;
        }
        public MetadataHelper metadataHelper { get; } =new MetadataHelper();
        public void Create(MethodDefinitionHandle entryPoint)
        {

            using var peStream = new FileStream(
                exename, FileMode.OpenOrCreate, FileAccess.ReadWrite
                );



            PEImageCreator.WritePEImage(peStream, metadataHelper, entryPoint);
        }
        public static readonly Guid s_guid = new Guid("87D4DBE1-1143-4FAD-AAB3-1001F92068E6");
        private static readonly BlobContentId s_contentId = new BlobContentId(s_guid, 0x04030201);

        private static void WritePEImage(
            Stream peStream,
            MetadataHelper metadataHelper,
            MethodDefinitionHandle entryPointHandle
            )
        {
            MetadataBuilder metadataBuilder = metadataHelper.metadata;
            BlobBuilder ilBuilder = metadataHelper.ilBuilder;

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
    }
}
