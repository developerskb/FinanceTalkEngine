using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Engine.Utils.Utils;

namespace Engine.Model
{
    public class HavenaQuantidades : BaseModel
    {
        public decimal Quantidade { get; set; }
        public static List<HavenaQuantidades> GetQuantidades()
        {
            List<HavenaQuantidades> havenaQuantidades = new List<HavenaQuantidades>();
            havenaQuantidades.Add(new HavenaQuantidades { Nome = "QUANTIDADE ", Id = 1 });
            havenaQuantidades.Add(new HavenaQuantidades { Nome = "QTD ", Id = 2 });
            havenaQuantidades.Add(new HavenaQuantidades { Nome = "QTD.", Id = 3 });
            return havenaQuantidades;
        }
        public static List<HavenaQuantidades> IdentificarQuantidades(string frase) {

            var quantidades = GetQuantidades();
            List<HavenaQuantidades> havenaQuantidades = new List<HavenaQuantidades>();

            quantidades.ForEach(quantd =>
            {
                var palavrasNaFrase = frase.Split(quantd.Nome);
                for (int l = 0; l < palavrasNaFrase.Length - 1; l++ ){
                    var posicao = frase.IndexOf(quantd.Nome);
                    var posicaoInicial = posicao;
                    var posicaoFinal = posicao;

                    var quantidade = "";
                    var parteLida = "";

                    if (posicaoFinal >= 0)
                    {
                        // seta a posição final da palavra para as duas variáveis
                        posicaoFinal = posicao += quantd.Nome.Length;
                    
                        while( posicao >= 0 && posicao < frase.Length )
                        {
                            posicaoFinal = posicao;

                            var caracter = frase.Substring(posicao, 1);
                            parteLida = caracter;
                            if ("0123456789".Contains(caracter))
                            {
                                quantidade += caracter;
                            }
                            else
                            {
                                break;
                            }
                            posicao++;
                        }

                    }

                    frase = Stuff(frase, posicaoInicial, quantd.Nome.Length, quantd.Nome.Replace("Q", "K"));

                    decimal quantidadeDecimal = 0;
                    // Verifica se a string não está vazia e se ele consegue converter para decimal
                    if (!String.IsNullOrEmpty(quantidade) && decimal.TryParse(quantidade, out quantidadeDecimal))
                    {
                        havenaQuantidades.Add(new HavenaQuantidades
                        {
                            Index = posicaoInicial,
                            Quantidade = quantidadeDecimal
                        });
                    }
                }
            });

            return havenaQuantidades;
        }
    }
}
