// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  TAL-PC
// DateTime: 6/4/2017 10:49:36 AM
// UserName: Tal
// Input file <Indexing\LuceneQuery.Language.grammar.y - 2/20/2017 11:55:14 AM>

// options: no-lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace Raven35.Database.Indexing
{
internal enum Token {error=2,EOF=3,NOT=4,OR=5,AND=6,
    INTERSECT=7,PLUS=8,MINUS=9,OPEN_CURLY_BRACKET=10,CLOSE_CURLY_BRACKET=11,OPEN_SQUARE_BRACKET=12,
    CLOSE_SQUARE_BRACKET=13,TILDA=14,BOOST=15,QUOTE=16,TO=17,COLON=18,
    OPEN_PAREN=19,CLOSE_PAREN=20,ALL_DOC=21,UNANALIZED_TERM=22,METHOD=23,UNQUOTED_TERM=24,
    QUOTED_TERM=25,QUOTED_WILDCARD_TERM=26,FLOAT_NUMBER=27,INT_NUMBER=28,DOUBLE_NUMBER=29,LONG_NUMBER=30,
    DATETIME=31,NULL=32,PREFIX_TERM=33,WILDCARD_TERM=34,HEX_NUMBER=35};

internal partial struct ValueType
{ 
			public string s; 
			public FieldLuceneASTNode fn;
			public ParenthesistLuceneASTNode pn;
			public PostfixModifiers pm;
			public LuceneASTNodeBase nb;
			public OperatorLuceneASTNode.Operator o;
			public RangeLuceneASTNode rn;
			public TermLuceneASTNode tn;
			public MethodLuceneASTNode mn;
			public List<TermLuceneASTNode> ltn;
			public LuceneASTNodeBase.PrefixOperator npo;
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
internal partial class LuceneQueryParser: ShiftReduceParser<ValueType, LexLocation>
{
  // Verbatim content from Indexing\LuceneQuery.Language.grammar.y - 2/20/2017 11:55:14 AM
	public LuceneASTNodeBase LuceneAST {get; set;}
  // End verbatim content from Indexing\LuceneQuery.Language.grammar.y - 2/20/2017 11:55:14 AM

#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[63];
  private static State[] states = new State[88];
  private static string[] nonTerms = new string[] {
      "main", "prefix_operator", "methodName", "fieldname", "fuzzy_modifier", 
      "boost_modifier", "proximity_modifier", "operator", "term_exp", "term", 
      "postfix_modifier", "paren_exp", "node", "field_exp", "range_operator_exp", 
      "method_exp", "term_match_list", "$accept", };

  static LuceneQueryParser() {
    states[0] = new State(new int[]{24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87},new int[]{-1,1,-13,3,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{3,4,5,8,6,9,7,10,24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87},new int[]{-8,5,-13,7,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[4] = new State(-2);
    states[5] = new State(new int[]{24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87},new int[]{-13,6,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[6] = new State(new int[]{5,8,6,9,7,10,24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87,3,-3,20,-3},new int[]{-8,5,-13,7,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[7] = new State(new int[]{5,8,6,9,7,10,24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87,3,-4,20,-4},new int[]{-8,5,-13,7,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[8] = new State(-57);
    states[9] = new State(-58);
    states[10] = new State(-59);
    states[11] = new State(-5);
    states[12] = new State(new int[]{10,16,12,35,8,55,9,56,4,57,25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,19,60},new int[]{-15,13,-9,14,-12,15,-2,41,-10,58});
    states[13] = new State(-15);
    states[14] = new State(-16);
    states[15] = new State(-17);
    states[16] = new State(new int[]{25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-10,17});
    states[17] = new State(new int[]{17,18});
    states[18] = new State(new int[]{25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-10,19});
    states[19] = new State(new int[]{11,20,13,21});
    states[20] = new State(-53);
    states[21] = new State(-55);
    states[22] = new State(-30);
    states[23] = new State(-31);
    states[24] = new State(-32);
    states[25] = new State(-33);
    states[26] = new State(-34);
    states[27] = new State(-35);
    states[28] = new State(-36);
    states[29] = new State(-37);
    states[30] = new State(-38);
    states[31] = new State(-39);
    states[32] = new State(-40);
    states[33] = new State(-41);
    states[34] = new State(-42);
    states[35] = new State(new int[]{25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-10,36});
    states[36] = new State(new int[]{17,37});
    states[37] = new State(new int[]{25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-10,38});
    states[38] = new State(new int[]{11,39,13,40});
    states[39] = new State(-54);
    states[40] = new State(-56);
    states[41] = new State(new int[]{25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-10,42});
    states[42] = new State(new int[]{14,49,15,46,3,-28,5,-28,6,-28,7,-28,24,-28,19,-28,8,-28,9,-28,4,-28,25,-28,28,-28,27,-28,35,-28,30,-28,29,-28,22,-28,31,-28,32,-28,26,-28,34,-28,33,-28,23,-28,21,-28,20,-28},new int[]{-11,43,-7,44,-5,52,-6,54});
    states[43] = new State(-26);
    states[44] = new State(new int[]{15,46,3,-47,5,-47,6,-47,7,-47,24,-47,19,-47,8,-47,9,-47,4,-47,25,-47,28,-47,27,-47,35,-47,30,-47,29,-47,22,-47,31,-47,32,-47,26,-47,34,-47,33,-47,23,-47,21,-47,20,-47},new int[]{-6,45});
    states[45] = new State(-43);
    states[46] = new State(new int[]{28,47,27,48});
    states[47] = new State(-49);
    states[48] = new State(-50);
    states[49] = new State(new int[]{28,50,27,51,15,-52,3,-52,5,-52,6,-52,7,-52,24,-52,19,-52,8,-52,9,-52,4,-52,25,-52,35,-52,30,-52,29,-52,22,-52,31,-52,32,-52,26,-52,34,-52,33,-52,23,-52,21,-52,20,-52});
    states[50] = new State(-48);
    states[51] = new State(-51);
    states[52] = new State(new int[]{15,46,3,-46,5,-46,6,-46,7,-46,24,-46,19,-46,8,-46,9,-46,4,-46,25,-46,28,-46,27,-46,35,-46,30,-46,29,-46,22,-46,31,-46,32,-46,26,-46,34,-46,33,-46,23,-46,21,-46,20,-46},new int[]{-6,53});
    states[53] = new State(-44);
    states[54] = new State(-45);
    states[55] = new State(-60);
    states[56] = new State(-61);
    states[57] = new State(-62);
    states[58] = new State(new int[]{14,49,15,46,3,-29,5,-29,6,-29,7,-29,24,-29,19,-29,8,-29,9,-29,4,-29,25,-29,28,-29,27,-29,35,-29,30,-29,29,-29,22,-29,31,-29,32,-29,26,-29,34,-29,33,-29,23,-29,21,-29,20,-29},new int[]{-11,59,-7,44,-5,52,-6,54});
    states[59] = new State(-27);
    states[60] = new State(new int[]{24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87},new int[]{-13,61,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[61] = new State(new int[]{20,62,5,8,6,9,7,10,24,64,19,60,8,55,9,56,4,57,25,22,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,23,84,21,87},new int[]{-8,5,-13,7,-14,11,-4,12,-12,66,-9,67,-2,68,-10,58,-16,86,-3,76});
    states[62] = new State(new int[]{15,46,3,-22,5,-22,6,-22,7,-22,24,-22,19,-22,8,-22,9,-22,4,-22,25,-22,28,-22,27,-22,35,-22,30,-22,29,-22,22,-22,31,-22,32,-22,26,-22,34,-22,33,-22,23,-22,21,-22,20,-22},new int[]{-6,63});
    states[63] = new State(-23);
    states[64] = new State(new int[]{18,65,14,-31,15,-31,3,-31,5,-31,6,-31,7,-31,24,-31,19,-31,8,-31,9,-31,4,-31,25,-31,28,-31,27,-31,35,-31,30,-31,29,-31,22,-31,31,-31,32,-31,26,-31,34,-31,33,-31,23,-31,21,-31,20,-31});
    states[65] = new State(-25);
    states[66] = new State(-6);
    states[67] = new State(-7);
    states[68] = new State(new int[]{21,75,25,22,24,64,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,19,60,8,55,9,56,4,57,23,84},new int[]{-10,69,-14,71,-12,72,-9,73,-16,74,-4,12,-2,41,-3,76});
    states[69] = new State(new int[]{14,49,15,46,3,-28,5,-28,6,-28,7,-28,24,-28,19,-28,8,-28,9,-28,4,-28,25,-28,28,-28,27,-28,35,-28,30,-28,29,-28,22,-28,31,-28,32,-28,26,-28,34,-28,33,-28,23,-28,21,-28,20,-28},new int[]{-11,70,-7,44,-5,52,-6,54});
    states[70] = new State(-26);
    states[71] = new State(-9);
    states[72] = new State(-10);
    states[73] = new State(-11);
    states[74] = new State(-12);
    states[75] = new State(-13);
    states[76] = new State(new int[]{19,77});
    states[77] = new State(new int[]{8,55,9,56,4,57,25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-17,78,-9,80,-2,41,-10,58});
    states[78] = new State(new int[]{20,79});
    states[79] = new State(-18);
    states[80] = new State(new int[]{20,81,8,55,9,56,4,57,25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34},new int[]{-9,82,-17,83,-2,41,-10,58});
    states[81] = new State(-19);
    states[82] = new State(new int[]{8,55,9,56,4,57,25,22,24,23,28,24,27,25,35,26,30,27,29,28,22,29,31,30,32,31,26,32,34,33,33,34,20,-20},new int[]{-9,82,-17,83,-2,41,-10,58});
    states[83] = new State(-21);
    states[84] = new State(new int[]{18,85});
    states[85] = new State(-24);
    states[86] = new State(-8);
    states[87] = new State(-14);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-18, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{-13,3});
    rules[3] = new Rule(-13, new int[]{-13,-8,-13});
    rules[4] = new Rule(-13, new int[]{-13,-13});
    rules[5] = new Rule(-13, new int[]{-14});
    rules[6] = new Rule(-13, new int[]{-12});
    rules[7] = new Rule(-13, new int[]{-9});
    rules[8] = new Rule(-13, new int[]{-16});
    rules[9] = new Rule(-13, new int[]{-2,-14});
    rules[10] = new Rule(-13, new int[]{-2,-12});
    rules[11] = new Rule(-13, new int[]{-2,-9});
    rules[12] = new Rule(-13, new int[]{-2,-16});
    rules[13] = new Rule(-13, new int[]{-2,21});
    rules[14] = new Rule(-13, new int[]{21});
    rules[15] = new Rule(-14, new int[]{-4,-15});
    rules[16] = new Rule(-14, new int[]{-4,-9});
    rules[17] = new Rule(-14, new int[]{-4,-12});
    rules[18] = new Rule(-16, new int[]{-3,19,-17,20});
    rules[19] = new Rule(-16, new int[]{-3,19,-9,20});
    rules[20] = new Rule(-17, new int[]{-9,-9});
    rules[21] = new Rule(-17, new int[]{-9,-17});
    rules[22] = new Rule(-12, new int[]{19,-13,20});
    rules[23] = new Rule(-12, new int[]{19,-13,20,-6});
    rules[24] = new Rule(-3, new int[]{23,18});
    rules[25] = new Rule(-4, new int[]{24,18});
    rules[26] = new Rule(-9, new int[]{-2,-10,-11});
    rules[27] = new Rule(-9, new int[]{-10,-11});
    rules[28] = new Rule(-9, new int[]{-2,-10});
    rules[29] = new Rule(-9, new int[]{-10});
    rules[30] = new Rule(-10, new int[]{25});
    rules[31] = new Rule(-10, new int[]{24});
    rules[32] = new Rule(-10, new int[]{28});
    rules[33] = new Rule(-10, new int[]{27});
    rules[34] = new Rule(-10, new int[]{35});
    rules[35] = new Rule(-10, new int[]{30});
    rules[36] = new Rule(-10, new int[]{29});
    rules[37] = new Rule(-10, new int[]{22});
    rules[38] = new Rule(-10, new int[]{31});
    rules[39] = new Rule(-10, new int[]{32});
    rules[40] = new Rule(-10, new int[]{26});
    rules[41] = new Rule(-10, new int[]{34});
    rules[42] = new Rule(-10, new int[]{33});
    rules[43] = new Rule(-11, new int[]{-7,-6});
    rules[44] = new Rule(-11, new int[]{-5,-6});
    rules[45] = new Rule(-11, new int[]{-6});
    rules[46] = new Rule(-11, new int[]{-5});
    rules[47] = new Rule(-11, new int[]{-7});
    rules[48] = new Rule(-7, new int[]{14,28});
    rules[49] = new Rule(-6, new int[]{15,28});
    rules[50] = new Rule(-6, new int[]{15,27});
    rules[51] = new Rule(-5, new int[]{14,27});
    rules[52] = new Rule(-5, new int[]{14});
    rules[53] = new Rule(-15, new int[]{10,-10,17,-10,11});
    rules[54] = new Rule(-15, new int[]{12,-10,17,-10,11});
    rules[55] = new Rule(-15, new int[]{10,-10,17,-10,13});
    rules[56] = new Rule(-15, new int[]{12,-10,17,-10,13});
    rules[57] = new Rule(-8, new int[]{5});
    rules[58] = new Rule(-8, new int[]{6});
    rules[59] = new Rule(-8, new int[]{7});
    rules[60] = new Rule(-2, new int[]{8});
    rules[61] = new Rule(-2, new int[]{9});
    rules[62] = new Rule(-2, new int[]{4});
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
      case 2: // main -> node, EOF
{
	//Console.WriteLine("Found rule main -> node EOF");
	CurrentSemanticValue.nb = ValueStack[ValueStack.Depth-2].nb;
	LuceneAST = CurrentSemanticValue.nb;
	}
        break;
      case 3: // node -> node, operator, node
{
		//Console.WriteLine("Found rule node -> node operator node");
		var res =  new OperatorLuceneASTNode(ValueStack[ValueStack.Depth-3].nb,ValueStack[ValueStack.Depth-1].nb,ValueStack[ValueStack.Depth-2].o, IsDefaultOperatorAnd);
		CurrentSemanticValue.nb = res;
	}
        break;
      case 4: // node -> node, node
{
		//Console.WriteLine("Found rule node -> node node");
		CurrentSemanticValue.nb = new OperatorLuceneASTNode(ValueStack[ValueStack.Depth-2].nb,ValueStack[ValueStack.Depth-1].nb,OperatorLuceneASTNode.Operator.Implicit, IsDefaultOperatorAnd);
	}
        break;
      case 5: // node -> field_exp
{
		//Console.WriteLine("Found rule node -> field_exp");
		CurrentSemanticValue.nb =ValueStack[ValueStack.Depth-1].fn;
	}
        break;
      case 6: // node -> paren_exp
{
		//Console.WriteLine("Found rule node -> paren_exp");
		CurrentSemanticValue.nb =ValueStack[ValueStack.Depth-1].pn;
	}
        break;
      case 7: // node -> term_exp
{
	//Console.WriteLine("Found rule node -> term_exp");
		CurrentSemanticValue.nb = ValueStack[ValueStack.Depth-1].tn;
	}
        break;
      case 8: // node -> method_exp
{
		//Console.WriteLine("Found rule node -> method_exp");
		CurrentSemanticValue.nb = ValueStack[ValueStack.Depth-1].mn;
	}
        break;
      case 9: // node -> prefix_operator, field_exp
{
		//Console.WriteLine("Found rule node -> prefix_operator field_exp");
		CurrentSemanticValue.nb =ValueStack[ValueStack.Depth-1].fn;
		CurrentSemanticValue.nb.Prefix = ValueStack[ValueStack.Depth-2].npo;
	}
        break;
      case 10: // node -> prefix_operator, paren_exp
{
		//Console.WriteLine("Found rule node -> prefix_operator paren_exp");
		CurrentSemanticValue.nb =ValueStack[ValueStack.Depth-1].pn;
		CurrentSemanticValue.nb.Prefix = ValueStack[ValueStack.Depth-2].npo;
	}
        break;
      case 11: // node -> prefix_operator, term_exp
{
	//Console.WriteLine("Found rule node -> prefix_operator term_exp");
		CurrentSemanticValue.nb = ValueStack[ValueStack.Depth-1].tn;
		CurrentSemanticValue.nb.Prefix = ValueStack[ValueStack.Depth-2].npo;
	}
        break;
      case 12: // node -> prefix_operator, method_exp
{
		//Console.WriteLine("Found rule node -> prefix_operator method_exp");
		CurrentSemanticValue.nb = ValueStack[ValueStack.Depth-1].mn;
		CurrentSemanticValue.nb.Prefix = ValueStack[ValueStack.Depth-2].npo;
	}
        break;
      case 13: // node -> prefix_operator, ALL_DOC
{
		//Console.WriteLine("Found rule node -> prefix_operator ALL_DOC");
		CurrentSemanticValue.nb = new AllDocumentsLuceneASTNode();
		CurrentSemanticValue.nb.Prefix = ValueStack[ValueStack.Depth-2].npo;
	}
        break;
      case 14: // node -> ALL_DOC
{
		CurrentSemanticValue.nb = new AllDocumentsLuceneASTNode();
	}
        break;
      case 15: // field_exp -> fieldname, range_operator_exp
{
		//Console.WriteLine("Found rule field_exp -> fieldname range_operator_exp");		
		CurrentSemanticValue.fn = new FieldLuceneASTNode(){FieldName = ValueStack[ValueStack.Depth-2].s, Node = ValueStack[ValueStack.Depth-1].rn};
		}
        break;
      case 16: // field_exp -> fieldname, term_exp
{
		//Console.WriteLine("Found rule field_exp -> fieldname term_exp");
		CurrentSemanticValue.fn = new FieldLuceneASTNode(){FieldName = ValueStack[ValueStack.Depth-2].s, Node = ValueStack[ValueStack.Depth-1].tn};
		}
        break;
      case 17: // field_exp -> fieldname, paren_exp
{
		//Console.WriteLine("Found rule field_exp -> fieldname paren_exp");
		CurrentSemanticValue.fn = new FieldLuceneASTNode(){FieldName = ValueStack[ValueStack.Depth-2].s, Node = ValueStack[ValueStack.Depth-1].pn};
	}
        break;
      case 18: // method_exp -> methodName, OPEN_PAREN, term_match_list, CLOSE_PAREN
{
		//Console.WriteLine("Found rule method_exp -> methodName OPEN_PAREN term_match_list CLOSE_PAREN");
		CurrentSemanticValue.mn = new MethodLuceneASTNode(ValueStack[ValueStack.Depth-4].s,ValueStack[ValueStack.Depth-2].ltn);
		InMethod = false;
}
        break;
      case 19: // method_exp -> methodName, OPEN_PAREN, term_exp, CLOSE_PAREN
{
		//Console.WriteLine("Found rule method_exp -> methodName OPEN_PAREN term_exp CLOSE_PAREN");
		CurrentSemanticValue.mn = new MethodLuceneASTNode(ValueStack[ValueStack.Depth-4].s,ValueStack[ValueStack.Depth-2].tn);
		InMethod = false;
}
        break;
      case 20: // term_match_list -> term_exp, term_exp
{
	//Console.WriteLine("Found rule term_match_list -> term_exp term_exp");
	CurrentSemanticValue.ltn = new List<TermLuceneASTNode>(){ValueStack[ValueStack.Depth-2].tn,ValueStack[ValueStack.Depth-1].tn};
}
        break;
      case 21: // term_match_list -> term_exp, term_match_list
{
	//Console.WriteLine("Found rule term_match_list -> term_exp term_match_list");
	ValueStack[ValueStack.Depth-1].ltn.Add(ValueStack[ValueStack.Depth-2].tn);
	CurrentSemanticValue.ltn = ValueStack[ValueStack.Depth-1].ltn;
}
        break;
      case 22: // paren_exp -> OPEN_PAREN, node, CLOSE_PAREN
{
		//Console.WriteLine("Found rule paren_exp -> OPEN_PAREN node CLOSE_PAREN");
		CurrentSemanticValue.pn = new ParenthesistLuceneASTNode();
		CurrentSemanticValue.pn.Node = ValueStack[ValueStack.Depth-2].nb;
		}
        break;
      case 23: // paren_exp -> OPEN_PAREN, node, CLOSE_PAREN, boost_modifier
{
		//Console.WriteLine("Found rule paren_exp -> OPEN_PAREN node CLOSE_PAREN boost_modifier");
		CurrentSemanticValue.pn = new ParenthesistLuceneASTNode();
		CurrentSemanticValue.pn.Node = ValueStack[ValueStack.Depth-3].nb;
		CurrentSemanticValue.pn.Boost = ValueStack[ValueStack.Depth-1].s;
		}
        break;
      case 24: // methodName -> METHOD, COLON
{
		//Console.WriteLine("Found rule methodName -> METHOD COLON");
		CurrentSemanticValue.s = ValueStack[ValueStack.Depth-2].s;
		InMethod = true;
}
        break;
      case 25: // fieldname -> UNQUOTED_TERM, COLON
{
		//Console.WriteLine("Found rule fieldname -> UNQUOTED_TERM COLON");
		CurrentSemanticValue.s = ValueStack[ValueStack.Depth-2].s;
	}
        break;
      case 26: // term_exp -> prefix_operator, term, postfix_modifier
{
		//Console.WriteLine("Found rule term_exp -> prefix_operator term postfix_modifier");
		CurrentSemanticValue.tn = ValueStack[ValueStack.Depth-2].tn;
		CurrentSemanticValue.tn.Prefix =ValueStack[ValueStack.Depth-3].npo;
		CurrentSemanticValue.tn.SetPostfixOperators(ValueStack[ValueStack.Depth-1].pm);
	}
        break;
      case 27: // term_exp -> term, postfix_modifier
{
		//Console.WriteLine("Found rule term_exp -> postfix_modifier");
		CurrentSemanticValue.tn = ValueStack[ValueStack.Depth-2].tn;
		CurrentSemanticValue.tn.SetPostfixOperators(ValueStack[ValueStack.Depth-1].pm);
	}
        break;
      case 28: // term_exp -> prefix_operator, term
{
		//Console.WriteLine("Found rule term_exp -> prefix_operator term");
		CurrentSemanticValue.tn = ValueStack[ValueStack.Depth-1].tn;
		CurrentSemanticValue.tn.Prefix = ValueStack[ValueStack.Depth-2].npo;
	}
        break;
      case 29: // term_exp -> term
{
		//Console.WriteLine("Found rule term_exp -> term");
		CurrentSemanticValue.tn = ValueStack[ValueStack.Depth-1].tn;
	}
        break;
      case 30: // term -> QUOTED_TERM
{
		//Console.WriteLine("Found rule term -> QUOTED_TERM");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s.Substring(1,ValueStack[ValueStack.Depth-1].s.Length-2), Type=TermLuceneASTNode.TermType.Quoted};
	}
        break;
      case 31: // term -> UNQUOTED_TERM
{
		//Console.WriteLine("Found rule term -> UNQUOTED_TERM");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s,Type=TermLuceneASTNode.TermType.UnQuoted};
		}
        break;
      case 32: // term -> INT_NUMBER
{
		//Console.WriteLine("Found rule term -> INT_NUMBER");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.Int};
		}
        break;
      case 33: // term -> FLOAT_NUMBER
{
		//Console.WriteLine("Found rule term -> FLOAT_NUMBER");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.Float};
	}
        break;
      case 34: // term -> HEX_NUMBER
{
		//Console.WriteLine("Found rule term -> HEX_NUMBER");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.Hex};
	}
        break;
      case 35: // term -> LONG_NUMBER
{
		//Console.WriteLine("Found rule term -> INT_NUMBER");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.Long};
		}
        break;
      case 36: // term -> DOUBLE_NUMBER
{
		//Console.WriteLine("Found rule term -> FLOAT_NUMBER");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.Double};
	}
        break;
      case 37: // term -> UNANALIZED_TERM
{
		//Console.WriteLine("Found rule term -> UNANALIZED_TERM");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.UnAnalyzed};
	}
        break;
      case 38: // term -> DATETIME
{
		//Console.WriteLine("Found rule term -> DATETIME");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.DateTime};
	}
        break;
      case 39: // term -> NULL
{
		//Console.WriteLine("Found rule term -> NULL");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.Null};
	}
        break;
      case 40: // term -> QUOTED_WILDCARD_TERM
{
		//Console.WriteLine("Found rule term -> QUOTED_WILDCARD_TERM");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.QuotedWildcard};
	}
        break;
      case 41: // term -> WILDCARD_TERM
{
		//Console.WriteLine("Found rule term -> WILDCARD_TERM");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.WildCardTerm};
	}
        break;
      case 42: // term -> PREFIX_TERM
{
		//Console.WriteLine("Found rule term -> PREFIX_TERM");
		CurrentSemanticValue.tn = new TermLuceneASTNode(){Term=ValueStack[ValueStack.Depth-1].s, Type=TermLuceneASTNode.TermType.PrefixTerm};
	}
        break;
      case 43: // postfix_modifier -> proximity_modifier, boost_modifier
{
		CurrentSemanticValue.pm = new PostfixModifiers(){Boost = ValueStack[ValueStack.Depth-1].s, Similerity = null, Proximity = ValueStack[ValueStack.Depth-2].s};
	}
        break;
      case 44: // postfix_modifier -> fuzzy_modifier, boost_modifier
{
		CurrentSemanticValue.pm = new PostfixModifiers(){Boost = ValueStack[ValueStack.Depth-1].s, Similerity = ValueStack[ValueStack.Depth-2].s, Proximity = null};
	}
        break;
      case 45: // postfix_modifier -> boost_modifier
{
		CurrentSemanticValue.pm = new PostfixModifiers(){Boost = ValueStack[ValueStack.Depth-1].s,Similerity = null, Proximity = null};
	}
        break;
      case 46: // postfix_modifier -> fuzzy_modifier
{
		CurrentSemanticValue.pm = new PostfixModifiers(){Boost = null, Similerity = ValueStack[ValueStack.Depth-1].s, Proximity = null};
	}
        break;
      case 47: // postfix_modifier -> proximity_modifier
{
		CurrentSemanticValue.pm = new PostfixModifiers(){Boost = null, Similerity = null, Proximity = ValueStack[ValueStack.Depth-1].s};
	}
        break;
      case 48: // proximity_modifier -> TILDA, INT_NUMBER
{
	//Console.WriteLine("Found rule proximity_modifier -> TILDA INT_NUMBER");
	CurrentSemanticValue.s = ValueStack[ValueStack.Depth-1].s;
	}
        break;
      case 49: // boost_modifier -> BOOST, INT_NUMBER
{
	//Console.WriteLine("Found rule boost_modifier -> BOOST INT_NUMBER");
	CurrentSemanticValue.s = ValueStack[ValueStack.Depth-1].s;
	}
        break;
      case 50: // boost_modifier -> BOOST, FLOAT_NUMBER
{
	//Console.WriteLine("Found rule boost_modifier -> BOOST FLOAT_NUMBER");
	CurrentSemanticValue.s = ValueStack[ValueStack.Depth-1].s;
	}
        break;
      case 51: // fuzzy_modifier -> TILDA, FLOAT_NUMBER
{
	//Console.WriteLine("Found rule fuzzy_modifier ->  TILDA FLOAT_NUMBER");
	CurrentSemanticValue.s = ValueStack[ValueStack.Depth-1].s;
	}
        break;
      case 52: // fuzzy_modifier -> TILDA
{
		//Console.WriteLine("Found rule fuzzy_modifier ->  TILDA");
		CurrentSemanticValue.s = "0.5";
	}
        break;
      case 53: // range_operator_exp -> OPEN_CURLY_BRACKET, term, TO, term, CLOSE_CURLY_BRACKET
{
		//Console.WriteLine("Found rule range_operator_exp -> OPEN_CURLY_BRACKET term TO term CLOSE_CURLY_BRACKET");
		CurrentSemanticValue.rn = new RangeLuceneASTNode(){RangeMin = ValueStack[ValueStack.Depth-4].tn, RangeMax = ValueStack[ValueStack.Depth-2].tn, InclusiveMin = false, InclusiveMax = false};
		}
        break;
      case 54: // range_operator_exp -> OPEN_SQUARE_BRACKET, term, TO, term, CLOSE_CURLY_BRACKET
{
		//Console.WriteLine("Found rule range_operator_exp -> OPEN_SQUARE_BRACKET term TO term CLOSE_CURLY_BRACKET");
		CurrentSemanticValue.rn = new RangeLuceneASTNode(){RangeMin = ValueStack[ValueStack.Depth-4].tn, RangeMax = ValueStack[ValueStack.Depth-2].tn, InclusiveMin = true, InclusiveMax = false};
		}
        break;
      case 55: // range_operator_exp -> OPEN_CURLY_BRACKET, term, TO, term, CLOSE_SQUARE_BRACKET
{
		//Console.WriteLine("Found rule range_operator_exp -> OPEN_CURLY_BRACKET term TO term CLOSE_SQUARE_BRACKET");
		CurrentSemanticValue.rn = new RangeLuceneASTNode(){RangeMin = ValueStack[ValueStack.Depth-4].tn, RangeMax = ValueStack[ValueStack.Depth-2].tn, InclusiveMin = false, InclusiveMax = true};
		}
        break;
      case 56: // range_operator_exp -> OPEN_SQUARE_BRACKET, term, TO, term, CLOSE_SQUARE_BRACKET
{
		//Console.WriteLine("Found rule range_operator_exp -> OPEN_SQUARE_BRACKET term TO term CLOSE_SQUARE_BRACKET");
		CurrentSemanticValue.rn = new RangeLuceneASTNode(){RangeMin = ValueStack[ValueStack.Depth-4].tn, RangeMax = ValueStack[ValueStack.Depth-2].tn, InclusiveMin = true, InclusiveMax = true};
		}
        break;
      case 57: // operator -> OR
{
		//Console.WriteLine("Found rule operator -> OR");
		CurrentSemanticValue.o = OperatorLuceneASTNode.Operator.OR;
		}
        break;
      case 58: // operator -> AND
{
		//Console.WriteLine("Found rule operator -> AND");
		CurrentSemanticValue.o = OperatorLuceneASTNode.Operator.AND;
		}
        break;
      case 59: // operator -> INTERSECT
{
		//Console.WriteLine("Found rule operator -> INTERSECT");
		CurrentSemanticValue.o = OperatorLuceneASTNode.Operator.INTERSECT;
	}
        break;
      case 60: // prefix_operator -> PLUS
{
		//Console.WriteLine("Found rule prefix_operator -> PLUS");
		CurrentSemanticValue.npo = LuceneASTNodeBase.PrefixOperator.Plus;
		}
        break;
      case 61: // prefix_operator -> MINUS
{
		//Console.WriteLine("Found rule prefix_operator -> MINUS");
		CurrentSemanticValue.npo = LuceneASTNodeBase.PrefixOperator.Minus;
		}
        break;
      case 62: // prefix_operator -> NOT
{
        //Console.WriteLine("Found rule prefix_operator -> NOT");
		CurrentSemanticValue.npo = LuceneASTNodeBase.PrefixOperator.Minus;
    }
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
