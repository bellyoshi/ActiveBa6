using NUnit.Framework;
using ConsoleApp1;
using System;


namespace TestProject1
{
	public class HelloWorld
	{
		public interface IHello
		{
			void SayHello(string toWhom);
		}

		[Test]
		public void DiskOutputTest()
		{
			//var exename = "HelloWorld.exe";
			//EmitHelper emit = new AssemblyBuilderHelper(exename)
			//	.DefineType("Hello", typeof(object), typeof(IHello))
			//	.DefineMethod(typeof(IHello).GetMethod("SayHello"))
			//	.Emitter;

			//emit
			//	.ldstr("Hello MSIL")
			//	//
			//	// Console.WriteLine("Hello, World!");
			//	//
			//	.call(typeof(Console), "WriteLine", typeof(string))
			//	.ret()
			//	;
			//pe.Create(emit.MethodDefinitionHandle);
			var actual = new Class1("HelloWorld.exe").BuildAndRun();
			Assert.AreEqual("Hello MSIL\r\n", actual);
		}


	
	}
}
