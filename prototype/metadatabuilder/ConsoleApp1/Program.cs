// See https://aka.ms/new-console-template for more information
var exename = Guid.NewGuid().ToString() + ".exe";
Console.WriteLine(exename);
new ConsoleApp1.Class1(exename).BuildHelloWorldApp();
System.Diagnostics.Process.Start(exename);  