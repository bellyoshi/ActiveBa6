// See https://aka.ms/new-console-template for more information
var exename = Guid.NewGuid().ToString() + ".exe";
Console.WriteLine(exename);
ConsoleApp1.Class1.BuildHelloWorldApp(exename);
System.Diagnostics.Process.Start(exename);  