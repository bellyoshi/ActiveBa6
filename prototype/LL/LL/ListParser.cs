using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LL.TokenTypes;
namespace LL
{
    public class ListParser:Parser
    {
        public ListParser(Lexer input) : base(input) { }

        public void List()
        {
            Console.WriteLine("List");
            Match(LBRACK);
            Elements();
            Match(RBLACK);
        }

        public void Assign()
        {
            Console.WriteLine("Assign");
            List();
            Match(EQUAL);
            List();
        }
        public void Elements()
        {
            Console.WriteLine("Elements");
            Element();
            while(LA(1) == COMMA)
            {
                Match(COMMA);
                Element();
            }
        }
        public void Element()
        {
            Console.WriteLine("Element");
            if (LA(1) == NAME && LA(2) == EQUAL)
            {
                Match(NAME);
                Match(EQUAL);
                Match(NAME);
            }
            else if (LA(1) == NAME)
            {
                Match(NAME);
            }
            else if (LA(1) == LBRACK)
            {
                List();
            }
            else
            {
                throw new Exception($"Expecting name or list; found {LT(1)}");
            }
        }

        public void Stat()
        {
            if (Speculate_stat_alt1())
            {
                List();
                Match(TokenTypes.EOF_TYPE);
            }else if (speculate_stat_alt2())
            {
                Assign();
                Match(TokenTypes.EOF_TYPE);
            }else
            {
                throw new NoViableAltException($"expecting stat found {LT(1)}");
            }
        }

        public bool Speculate_stat_alt1()
        {
            bool success = true;
            Mark();
            try
            {
                List();
                Match(TokenTypes.EOF_TYPE);
            }catch(RecognitionExeption e)
            {
                success = false;
            }
            Release();
            return success;
        }

        public bool speculate_stat_alt2()
        {
            bool success = true;
            Mark();
            try
            {
                Assign();
                Match(TokenTypes.EOF_TYPE);
            }
            catch (RecognitionExeption e)
            {
                success = false;
            }
            Release();
            return success;

        }


    }
}
