using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using static Engine.Utils.Utils;

namespace Engine.Model
{
    public class HavenaDescontos: BaseModel
    {
        public decimal Desconto { get; set; }
        private static List<HavenaDescontos> GetDescontos()
        {
            List<HavenaDescontos> havenasDescontos = new List<HavenaDescontos>();
            havenasDescontos.Add(new HavenaDescontos { Nome = "DESCONTO DE ", Id = 1 });
            havenasDescontos.Add(new HavenaDescontos { Nome = "DESCONTO ", Id = 1 });
            return havenasDescontos;
        }
        public static List<HavenaDescontos> IdentificarDescontos(string frase)
        {
            var descontos = GetDescontos();
            List<HavenaDescontos> havenaDescontos = new List<HavenaDescontos>();
            descontos.ForEach(descont => {
                var posicao = 0;// frase.IndexOf(descont.Nome);
                var posicaoInicial = posicao;
                var posicaoFinal = posicao;

                var desconto = "";
                var parteLida = "";

                var quantidadePalavrasFrase = frase.Split(descont.Nome);

                for (int i = 0; i < quantidadePalavrasFrase.Count() - 1; i++) {
                    posicaoInicial = posicao = frase.IndexOf(descont.Nome);
                    if ( posicao >= 0 )
                    {
                        posicaoFinal = posicao += descont.Nome.Length;
                    }

                    while(posicao >= 0 && posicao < frase.Length)
                    {
                        posicaoFinal = posicao;

                        var caracter = frase.Substring(posicao, 1);
                        parteLida = caracter;
                        if("0123456789R$%,. ".Contains(caracter) )
                        {
                            desconto += caracter;
                        }
                        else
                        {
                            break;
                        }
                        posicao++;
                    }

                    frase = Stuff(frase, posicaoInicial, descont.Nome.Length, descont.Nome.Replace("O", "0"));
                    if( !String.IsNullOrWhiteSpace(desconto))
                    {
                        decimal descontoValor = 0;
                        var culture = CultureInfo.GetCultureInfo("en-US");
                        if (desconto.Contains("R$") && decimal.TryParse(desconto.Replace("R$", " ").Replace(",", ".").Trim(), NumberStyles.Number, culture, out descontoValor)){
                           havenaDescontos.Add(new HavenaDescontos {
                               Index = posicao,
                               Desconto = descontoValor,
                               Tipo = "V"
                           });
                        }
                        if (desconto.Contains("%") && decimal.TryParse(desconto.Replace("%", " ").Trim(), NumberStyles.Number, culture, out descontoValor))
                        {
                            havenaDescontos.Add(new HavenaDescontos
                            {
                                Index = posicao,
                                Desconto = descontoValor,
                                Tipo = "P"
                            });
                        }
                        desconto = "";
                    }

                }
            });

            return havenaDescontos;
        }
    }
}
