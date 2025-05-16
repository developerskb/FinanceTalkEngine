using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class TipoPessoa: BaseModel
    {

        public static List<TipoPessoa> GetTiposPessoas()
        {
            List<TipoPessoa> result = new List<TipoPessoa>();

            result.Add(new TipoPessoa { Nome = "USUARIO", Id = 1 }); // Usado para indentificar que e o usuario do sistema
            /*result.Add(new TipoPessoa { Nome = "PARA MIM", Id = 2 });*/
            result.Add(new TipoPessoa { Nome = "COMIGO", Id = 3 });
            result.Add(new TipoPessoa { Nome = "AMIG", Id = 4 });
            result.Add(new TipoPessoa { Nome = "IRM", Id = 5 });
            result.Add(new TipoPessoa { Nome = "MINHA MAE", Id = 6 });
            result.Add(new TipoPessoa { Nome = "MEU TIO", Id = 7 });
            result.Add(new TipoPessoa { Nome = "MEU PAI", Id = 8 });
            result.Add(new TipoPessoa { Nome = "MINHA TIA", Id = 9 });
            result.Add(new TipoPessoa { Nome = "MINHA VO", Id = 10 });
            result.Add(new TipoPessoa { Nome = "MEU AVO", Id = 11 });
            result.Add(new TipoPessoa { Nome = "FILH", Id = 12 });
            result.Add(new TipoPessoa { Nome = "PRIM", Id = 13 });

            return result;
        }

        public static List<TipoPessoa> IdentificarTipoPessoa(string frase) {
            List<TipoPessoa> tipoPessoa = GetTiposPessoas();
            var pessoaFrase = new List<TipoPessoa>();
            var posicao = 0;

            foreach (var pessoa in tipoPessoa)
            {
                posicao = frase.IndexOf(pessoa.Nome);

                while (posicao >= 0)
                {
                    pessoaFrase.Add(new TipoPessoa
                    {
                        Index = posicao,
                        Nome = pessoa.Nome,
                        Id = pessoa.Id
                    });
                    break;
                }
            }

            if(pessoaFrase.Count == 0 )
            {
                pessoaFrase.Add(new TipoPessoa
                {
                    Index = 0,
                    Nome = tipoPessoa[0].Nome,
                    Id = tipoPessoa[0].Id
                });
            }

            pessoaFrase = pessoaFrase.OrderBy(p => p.Nome).ToList();

            return pessoaFrase;
        }
    }
}
