// See https://aka.ms/new-console-template for more information
var exename = Guid.NewGuid().ToString() + ".exe";
Console.WriteLine(exename);
var obj1 = new ConsoleApp1.Class1(exename);
obj1.BuildHelloWorldApp();
System.Diagnostics.Process.Start(exename);  