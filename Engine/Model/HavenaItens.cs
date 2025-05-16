using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class HavenaItens : BaseModel
    {
        public string CodigoProduto{ get; set;}
        public string DescricaoProduto { get; set;}
        public decimal Quantidade{ get; set;}
        public decimal Valor{ get; set;}
        public decimal DescontoValor{ get; set;}
        public int DescontoPerc{ get; set;}
        public decimal TotalItem{ get; set;}
        /*private static List<HavenaItens> GetItens()
        {
            List<HavenaItens> havenaItens = new List<HavenaItens>();
            return havenaItens;
        }*/
        public static List<HavenaItens> IdentificarItens(List<HavenaProdutos> produtos, List<HavenaDescontos> descontos, List<HavenaQuantidades> quantidades,List<Valor> valores)
        {
            /*var itens = GetItens();*/
            List<HavenaItens> havenaItens = new List<HavenaItens> { };

            for(int i = 0; i < produtos.Count; i++)
            {
                var descontoPer = 0;
                var descontoValor = descontos.Count == 0 ? 0: (descontos.Count > i ? descontos[i].Desconto : descontos.Last().Desconto);
                if( descontos[i].Tipo == "P")
                {
                    descontoPer = descontos[i].Id;
                    descontoValor = Math.Round( valores[i].ValorLancamento * (descontos[i].Desconto / 100), 2);
                }
                var totalItem = ( quantidades[i].Quantidade * valores[i].ValorLancamento) - descontoValor;
                havenaItens.Add(new HavenaItens { 
                    Index = i,
                    CodigoProduto = produtos[i].CodigoProduto,
                    DescricaoProduto = produtos[i].DescricaoProduto,
                    Quantidade = quantidades[i].Quantidade,
                    Valor = valores[i].ValorLancamento,
                    DescontoValor = descontoValor,
                    DescontoPerc = descontoPer,
                    TotalItem = totalItem,
                    Tipo = descontos[i].Tipo
                });
            }

            return havenaItens;
        }
    }
}
