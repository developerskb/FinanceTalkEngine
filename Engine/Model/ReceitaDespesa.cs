using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class ReceitaDespesa: BaseModel
    {        
        public static List<ReceitaDespesa> GetReceitasDespesas()
        {
            List<ReceitaDespesa> result = new List<ReceitaDespesa>();

            result.Add(new ReceitaDespesa { Nome = "N.IDENT.", Id = 1, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "COMIDA", Id = 2, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "LANCHE", Id = 3, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "BEBIDA", Id = 4, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "COMPRAS", Id = 5, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "ENERGIA", Id = 6, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "LUZ", Id = 7, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "CARRO", Id = 8, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "MOTO", Id = 9, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "COMBUSTIVEL", Id = 10, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "ALCOOL", Id = 11, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "CAFE", Id = 12, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "ALMOCO", Id = 13, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "JANTA", Id = 14, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "DOCE", Id = 15, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "MANUTENCAO", Id = 16, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "MANUTENCOES", Id = 17, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "LOTERIA", Id = 18, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "JOGOS", Id = 19, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "APOSTA", Id = 20, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "MERCADO", Id = 21, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "GASOLINA", Id = 22, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "ENERGIA", Id = 23, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "SABESP", Id = 24, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "ELETROPAULO", Id = 25, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "FARMACIA", Id = 26, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "BEBE", Id = 27, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "SACOLAO", Id = 28, Tipo = "D" });
            result.Add(new ReceitaDespesa { Nome = "PIZZA", Id = 29, Tipo = "D" });

            return result;
        }

        // Identificando Despesas (Gastos)
        public static List<ReceitaDespesa> IdentificarDespesas(string frase)
        {
            List<ReceitaDespesa> tipoDespesas = GetReceitasDespesas();
            List<ReceitaDespesa> despesasFrase = new List<ReceitaDespesa>();

            tipoDespesas.ForEach(despesa =>
            {
                var posicao = frase.IndexOf(despesa.Nome);
                while( posicao >= 0 )
                {

                    despesasFrase.Add(new ReceitaDespesa
                    {
                        Index = posicao,
                        Nome = despesa.Nome,
                        Id = despesa.Id,
                        Tipo = despesa.Tipo
                    });

                    break;
                }
            });

            if( despesasFrase.Count == 0 )
            {
                despesasFrase.Add(new ReceitaDespesa
                {
                    Index = 0,
                    Nome = tipoDespesas[0].Nome,
                    Id = tipoDespesas[0].Id,
                    Tipo = tipoDespesas[0].Tipo
                });
            }


            return despesasFrase;
        }
    }
}
