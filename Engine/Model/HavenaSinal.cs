using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class HavenaSinal: BaseModel
    {
        private static List<HavenaSinal> GetSinais()
        {
            List<HavenaSinal> sinais = new List<HavenaSinal>();
            sinais.Add(new HavenaSinal { Nome = "SINAL DE R$ ", Tipo = "D" });
            sinais.Add(new HavenaSinal { Nome = "DE SINAL ", Tipo = "A" });
            sinais.Add(new HavenaSinal { Nome = "COMO SINAL ", Tipo = "A" });   
            return sinais;
        }
        public static decimal IndentificarSinal(string frase, List<Valor> valores)
        {
            decimal valorSinal = 0;

            var sinais = GetSinais();
            sinais.ForEach( sinal =>
            {
                var palavrasNaFrase = frase.Split(sinal.Nome);
                for( int k = 0; k < palavrasNaFrase.Length - 1; k++ )
                {
                    var posicao = frase.IndexOf(sinal.Nome);

                    if( posicao >= 0 )
                    {
                        foreach(Valor valor in valores )
                        {
                            if( sinal.Tipo == "A" )
                            {
                                if(valor.Index < posicao)
                                {
                                    valorSinal = valor.ValorLancamento;
                                }
                                break;
                            }
                            if(sinal.Tipo == "D" && valor.Index > posicao )
                            {
                                valorSinal = valor.ValorLancamento;
                            }
                        };
                    }

                }
            });
            return valorSinal;
        }
    }
}
