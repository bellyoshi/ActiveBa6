using Mariana.CodeGen;
using Mariana.CodeGen.IL;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

public class Example
{
    private static readonly Guid s_guid = new Guid("87D4DBE1-1143-4FAD-AAB3-1001F92068E6");
    private static readonly BlobContentId s_contentId = new BlobContentId(s_guid, 0x04030201);


    public static void Main()
    {
        // Create a new assembly.
        var asmBuilder = new AssemblyBuilder("MyDynamicAssembly", new Version(1, 0, 0, 0));

        // Create a class named Point
        var typeBuilder = asmBuilder.defineType("Program", TypeAttributes.Public | TypeAttributes.Sealed);


        // The metadataContext of an AssemblyBuilder is what is used to reference external types
        // and members in the generated code.
        var context = asmBuilder.metadataContext;

        // Create a new ILBuilder. This will be used for emitting method bodies.
        var ilBuilder = new ILBuilder(context.ilTokenProvider);



        // Create a static method named "distance" that calculates the distance
        // between two points...
        var distanceMethodBuilder = typeBuilder.defineMethod(
            "Main",
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            returnType: TypeSignature.fromType(typeof(void)),
            paramTypes: new[] {
                TypeSignature.fromType(typeof(int)),
                TypeSignature.fromType(typeof(string[]))
            }
        );

        ilBuilder.emit(ILOp.ldstr, "Hello MSIL");
        ilBuilder.emit(ILOp.call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) }));
        ilBuilder.emit(ILOp.ret);

        distanceMethodBuilder.setMethodBody(ilBuilder.createMethodBody());

        asmBuilder.setEntryPoint(distanceMethodBuilder);

        var peHeaderBuilder = new PEHeaderBuilder(
    imageCharacteristics: Characteristics.ExecutableImage
    );
        asmBuilder.setPEHeader(peHeaderBuilder);
        // Create the dynamic assembly...
        var emitResult = asmBuilder.emit();


        var exeName = "xxx9.exe";
        using var peStream = new FileStream(
     exeName, FileMode.OpenOrCreate, FileAccess.Write
    );
        peStream.Write(emitResult.peImageBytes);
        peStream.Close();
      
        System.Diagnostics.Process.Start(exeName);
    }

}
