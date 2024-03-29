// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  
// DateTime: 
// UserName: 
// Input file 

// options: no-lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace Calculator
{
internal enum Token {error=2,EOF=3,NUMBER=4,OP_PLUS=5,OP_MINUS=6,
    OP_MULT=7,OP_DIV=8,P_OPEN=9,P_CLOSE=10};

internal partial struct ValueType
{ 
			public int n; 
			public string s; 
	   }
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal partial class CalculatorParser: ShiftReduceParser<ValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[13];
  private static State[] states = new State[19];
  private static string[] nonTerms = new string[] {
      "line", "$accept", "exp", "term", "factor", "number", };

  static CalculatorParser() {
    states[0] = new State(new int[]{4,9,9,10,7,-11,8,-11,5,-11,6,-11,3,-11},new int[]{-1,1,-3,3,-4,18,-5,17,-6,8});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{5,4,6,13,3,-2});
    states[4] = new State(new int[]{4,9,9,10,7,-11,8,-11,5,-11,6,-11,3,-11,10,-11},new int[]{-4,5,-5,17,-6,8});
    states[5] = new State(new int[]{7,6,8,15,5,-4,6,-4,3,-4,10,-4});
    states[6] = new State(new int[]{4,9,9,10,7,-11,8,-11,5,-11,6,-11,3,-11,10,-11},new int[]{-5,7,-6,8});
    states[7] = new State(-7);
    states[8] = new State(-9);
    states[9] = new State(-12);
    states[10] = new State(new int[]{4,9,9,10,7,-11,8,-11,10,-11,5,-11,6,-11},new int[]{-3,11,-4,18,-5,17,-6,8});
    states[11] = new State(new int[]{10,12,5,4,6,13});
    states[12] = new State(-10);
    states[13] = new State(new int[]{4,9,9,10,7,-11,8,-11,5,-11,6,-11,3,-11,10,-11},new int[]{-4,14,-5,17,-6,8});
    states[14] = new State(new int[]{7,6,8,15,5,-5,6,-5,3,-5,10,-5});
    states[15] = new State(new int[]{4,9,9,10,7,-11,8,-11,5,-11,6,-11,3,-11,10,-11},new int[]{-5,16,-6,8});
    states[16] = new State(-8);
    states[17] = new State(-6);
    states[18] = new State(new int[]{7,6,8,15,5,-3,6,-3,3,-3,10,-3});

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-2, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{-3});
    rules[3] = new Rule(-3, new int[]{-4});
    rules[4] = new Rule(-3, new int[]{-3,5,-4});
    rules[5] = new Rule(-3, new int[]{-3,6,-4});
    rules[6] = new Rule(-4, new int[]{-5});
    rules[7] = new Rule(-4, new int[]{-4,7,-5});
    rules[8] = new Rule(-4, new int[]{-4,8,-5});
    rules[9] = new Rule(-5, new int[]{-6});
    rules[10] = new Rule(-5, new int[]{9,-3,10});
    rules[11] = new Rule(-6, new int[]{});
    rules[12] = new Rule(-6, new int[]{4});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Token.error, (int)Token.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // line -> exp
{ Console.WriteLine("result is {0}\n", ValueStack[ValueStack.Depth-1].n);}
        break;
      case 3: // exp -> term
{ CurrentSemanticValue.n = ValueStack[ValueStack.Depth-1].n;			Console.WriteLine("Rule -> exp: {0}", ValueStack[ValueStack.Depth-1].n); }
        break;
      case 4: // exp -> exp, OP_PLUS, term
{ CurrentSemanticValue.n = ValueStack[ValueStack.Depth-3].n + ValueStack[ValueStack.Depth-1].n;	Console.WriteLine("Rule -> exp: {0} + {1}", ValueStack[ValueStack.Depth-3].n, ValueStack[ValueStack.Depth-1].n); }
        break;
      case 5: // exp -> exp, OP_MINUS, term
{ CurrentSemanticValue.n = ValueStack[ValueStack.Depth-3].n - ValueStack[ValueStack.Depth-1].n;	Console.WriteLine("Rule -> exp: {0} - {1}", ValueStack[ValueStack.Depth-3].n, ValueStack[ValueStack.Depth-1].n); }
        break;
      case 6: // term -> factor
{CurrentSemanticValue.n = ValueStack[ValueStack.Depth-1].n;			Console.WriteLine("Rule -> term: {0}", ValueStack[ValueStack.Depth-1].n); }
        break;
      case 7: // term -> term, OP_MULT, factor
{CurrentSemanticValue.n = ValueStack[ValueStack.Depth-3].n * ValueStack[ValueStack.Depth-1].n;	Console.WriteLine("Rule -> term: {0} * {1}", ValueStack[ValueStack.Depth-3].n, ValueStack[ValueStack.Depth-1].n); }
        break;
      case 8: // term -> term, OP_DIV, factor
{CurrentSemanticValue.n = ValueStack[ValueStack.Depth-3].n / ValueStack[ValueStack.Depth-1].n;	Console.WriteLine("Rule -> term: {0} / {1}", ValueStack[ValueStack.Depth-3].n, ValueStack[ValueStack.Depth-1].n); }
        break;
      case 9: // factor -> number
{CurrentSemanticValue.n = ValueStack[ValueStack.Depth-1].n;			Console.WriteLine("Rule -> factor: {0}", ValueStack[ValueStack.Depth-1].n); }
        break;
      case 10: // factor -> P_OPEN, exp, P_CLOSE
{CurrentSemanticValue.n = ValueStack[ValueStack.Depth-2].n;			Console.WriteLine("Rule -> factor: ( {0} )", ValueStack[ValueStack.Depth-1].n);}
        break;
      case 12: // number -> NUMBER
{ Console.WriteLine("Rule -> number: {0}", ValueStack[ValueStack.Depth-1].n); }
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Token)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Token)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

}
}
