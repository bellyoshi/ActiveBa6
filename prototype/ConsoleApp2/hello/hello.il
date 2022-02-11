.assembly helloworld {}
.class helloworld
{
 .method static void Main() cil managed
 {
  .entrypoint
  ldstr "Hello World!"
  call void [System.Console]System.Console::WriteLine(string)
  ret
 }
}