using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class Operacao: BaseModel
    {        
        public static List<Operacao> GetOperacoes()
        {
            List<Operacao> result = new List<Operacao>();

            result.Add(new Operacao { Nome = "GASTO", Id = 1, Tipo = "D" });
            result.Add(new Operacao { Nome = "GASTEI", Id = 2, Tipo = "D" });
            result.Add(new Operacao { Nome = "GASTAR", Id = 3, Tipo = "D" });
            result.Add(new Operacao { Nome = "USEI", Id = 4, Tipo = "D" });
            result.Add(new Operacao { Nome = "COMPREI", Id = 5, Tipo = "D" });
            result.Add(new Operacao { Nome = "COMPRAS", Id = 6, Tipo = "D" });
            result.Add(new Operacao { Nome = "COMPRANDO", Id = 7, Tipo = "D" });
            result.Add(new Operacao { Nome = "COMPRAR", Id = 8, Tipo = "D" });
            result.Add(new Operacao { Nome = "DESPESA", Id = 9, Tipo = "D" });
            result.Add(new Operacao { Nome = "PERDI", Id = 10, Tipo = "D" });

            result.Add(new Operacao { Nome = "GANHEI", Id = 15, Tipo = "C" });
            result.Add(new Operacao { Nome = "RECEBI", Id = 16, Tipo = "C" });
            result.Add(new Operacao { Nome = "PREMIO", Id = 17, Tipo = "C" });
            result.Add(new Operacao { Nome = "MEU VALE", Id = 18, Tipo = "C" });
            result.Add(new Operacao { Nome = "SALARIO", Id = 19, Tipo = "C" });

            result.Add(new Operacao { Nome = "ABASTECI", Id = 25, Tipo = "D" });
            result.Add(new Operacao { Nome = "ABASTECER", Id = 26, Tipo = "D" });
            result.Add(new Operacao { Nome = "ENVIEI", Id = 27, Tipo = "D" });
            result.Add(new Operacao { Nome = "TRANSFERI", Id = 28, Tipo = "D" });
            result.Add(new Operacao { Nome = "SAQUEI", Id = 30, Tipo = "D" });
            result.Add(new Operacao { Nome = "VOU SACAR", Id = 31, Tipo = "D" });
            result.Add(new Operacao { Nome = "RETIREI", Id = 32, Tipo = "D" });

            result.Add(new Operacao { Nome = "LANCAR CREDITO", Id = 40, Tipo = "C" });
            result.Add(new Operacao { Nome = "GERAR CREDITO", Id = 41, Tipo = "C" });
            result.Add(new Operacao { Nome = "CRIAR CREDITO", Id = 42, Tipo = "C" });
            result.Add(new Operacao { Nome = "LANCAR DEBIT0", Id = 45, Tipo = "D" });
            result.Add(new Operacao { Nome = "GERAR DEBIT0", Id = 46, Tipo = "D" });
            result.Add(new Operacao { Nome = "CRIAR DEBIT0", Id = 47, Tipo = "D" });
            result.Add(new Operacao { Nome = "EMPRESTEI", Id = 50, Tipo = "D" }); // Operacao dupla
            result.Add(new Operacao { Nome = " ME DEVE ", Id = 51, Tipo = "D" }); // Operacao dupla
            result.Add(new Operacao { Nome = " DEVO  ", Id = 52, Tipo = "D" }); // Operacao dupla
            result.Add(new Operacao { Nome = "EMPRESTIMO", Id = 53, Tipo = "C" }); // Operacao dupla
            result.Add(new Operacao { Nome = "DEVENDO", Id = 54, Tipo = "D" }); // Operacao dupla

            return result;
        }
    
        public static List<Operacao> IdentificarOperacoes(string frase)
        {
            List<Operacao> operacoFrase = new List<Operacao> { };
            List<Operacao> tipoOperacao = GetOperacoes();

            tipoOperacao.ForEach(operacao =>
            {
                var posicao = frase.IndexOf(operacao.Nome);
                while (posicao >= 0)
                {
                    operacoFrase.Add(new Operacao
                    {
                        Index = posicao,
                        Nome = operacao.Nome,
                        Id = operacao.Id,
                        Tipo = operacao.Tipo
                    }) ;
                    break;
                }
            });

            if(operacoFrase.Count == 0)
            {
                operacoFrase.Add(new Operacao
                {
                    Index = 0,
                    Nome = tipoOperacao[0].Nome,
                    Id = tipoOperacao[0].Id,
                    Tipo = tipoOperacao[0].Tipo
                });
            }

            return operacoFrase;
        }
    }
}
