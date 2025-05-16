using System;
using System.Diagnostics;
using Engine.Model;
using Engine.Utils;

namespace EngineConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //testar valor
            var c = 0;
            //var txt = TextFile.GetTxt();
            string frase = "Ola Ravena, venda de hoje cliente VITOR, produto 55555 R$ 100 quantidade 1 com sinal de R$ 10,50 e o resto parcelado em R$ 3 vezes com entrega para depois de amanha";
            Lancamento lancamento = new Lancamento(frase, "robson");
            lancamento.ProcessarFrase();
            var lancamentoTexto = lancamento.ToString();
            Console.WriteLine(lancamentoTexto);
            /*
            foreach( string frase in txt )
            {
                c++;
                Lancamento lancamento = new Lancamento(frase, "robson" );
                lancamento.ProcessarFrase();
                var lancamentoTexto = lancamento.ToString();
                Console.WriteLine(lancamentoTexto);



                //if( txt.Length - 3 == c  || c == 3) break;
                //break;
                //if (Console.ReadKey().Key.ToString().ToLower() == "numpad0") break;
            }
            */

        }
    }
}
