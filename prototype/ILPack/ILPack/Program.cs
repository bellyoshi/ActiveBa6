using System.Reflection;
using ClassLibrary1;


public class Program
{
     static void Main(string[] argv)
    {
        var assembly = ClassLibrary1.Demo.GetAssembly();
        var generator = new Lokad.ILPack.AssemblyGenerator();

        // for ad-hoc serialization
        var bytes = generator.GenerateAssemblyBytes(assembly);

        // direct serialization to disk
        generator.GenerateAssembly(assembly, "t.dll");
    }
}
