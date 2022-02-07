global using System.Reflection.Metadata.Ecma335;
global using System.Reflection;
global using System.Reflection.Metadata;
global using System.Reflection.PortableExecutable;

//// See https://aka.ms/new-console-template for more information
var exename = Guid.NewGuid().ToString() + ".exe";
Console.WriteLine(exename);
var obj1 = new ConsoleApp1.Class1(exename);
obj1.BuildHelloWorldApp();
System.Diagnostics.Process.Start(exename);  

//var t_object = typeof(Object);

//Console.WriteLine(t_object.FullName);
//Console.WriteLine(t_object.Assembly.FullName);
//Console.WriteLine(t_object.AssemblyQualifiedName);


//ConsoleApp1.Class3.BuildAndRun();