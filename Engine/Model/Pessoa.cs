using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class Pessoa: BaseModel
    {
        public int TipoPessoaId { get; set; }

        public static List<Pessoa> GetPessoas(string usuario)
        {
            List<Pessoa> result = new List<Pessoa>();

            result.Add(new Pessoa { Nome = usuario, Id = 1, TipoPessoaId = 1 });
            result.Add(new Pessoa { Nome = " MEU ", Id = 2, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = " MEU.", Id = 3, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "COMIGO", Id = 4, TipoPessoaId = 1 });
            result.Add(new Pessoa { Nome = "PARA MIM", Id = 5, TipoPessoaId = 1 });
            result.Add(new Pessoa { Nome = " E EU ", Id = 6, TipoPessoaId = 1 });
            result.Add(new Pessoa { Nome = " EU E ", Id = 7, TipoPessoaId = 1 });

            result.Add(new Pessoa { Nome = "FULANO", Id = 8, TipoPessoaId = 1 });

            // Amigos
            result.Add(new Pessoa { Nome = "MARCELO", Id = 10, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "RAIMO", Id = 11, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "GABRIEL", Id = 8, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "VICTOR", Id = 9, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "THIAGO", Id = 10, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "VITOR", Id = 11, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "RAMON", Id = 12, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "RAIMUNDO", Id = 13, TipoPessoaId = 4 });

            // Irm?os
            result.Add(new Pessoa { Nome = "HUGO", Id = 22, TipoPessoaId = 5 });
            result.Add(new Pessoa { Nome = "ANA LUCIA", Id = 23, TipoPessoaId = 5 });
            result.Add(new Pessoa { Nome = "ANA PAULA", Id = 24, TipoPessoaId = 5 });

            // Parentes
            result.Add(new Pessoa { Nome = "LADIR Id = ", Id = 3, TipoPessoaId = 0 });
            result.Add(new Pessoa { Nome = "JOSE Id = ", Id = 3, TipoPessoaId = 1 });
            result.Add(new Pessoa { Nome = "FLAVIO Id = ", Id = 3, TipoPessoaId = 2 });
            result.Add(new Pessoa { Nome = "DIEGO Id = ", Id = 3, TipoPessoaId = 3 });
            result.Add(new Pessoa { Nome = "VIVIANE Id = ", Id = 3, TipoPessoaId = 4 });
            result.Add(new Pessoa { Nome = "MARCIA Id = ", Id = 3, TipoPessoaId = 5 });
            result.Add(new Pessoa { Nome = "FRANCISCO Id = ", Id = 3, TipoPessoaId = 6 });
            result.Add(new Pessoa { Nome = "VERA Id = ", Id = 3, TipoPessoaId = 7 });
            result.Add(new Pessoa { Nome = "LURDES Id = ", Id = 3, TipoPessoaId = 8 });
            result.Add(new Pessoa { Nome = "SILVANA Id = ", Id = 3, TipoPessoaId = 9 });

            // Testes
            result.Add(new Pessoa { Nome = "MARA Id = ", Id = 4, TipoPessoaId = 0 });

            return result;
        }

        // Identificando as pessoas
        public static List<Pessoa> IdentificarPessoas(string frase, string usuarioNome = "", bool eHavena = false)
        {

            List<Pessoa> tipoPessoa = GetPessoas(usuarioNome);
            var posicao = 1;

            var pessoasFrase = new List<Pessoa> { };

            foreach( var pessoa in tipoPessoa )
            {
                posicao = frase.IndexOf(pessoa.Nome);

                while( posicao > 0 )
                {
                    // Vai verificar se é a pessoa em questão se for
                    // então vai adicionar o própio nome do usuário
                    // e o Id
                    var pessoaNome = pessoa.Nome;
                    var pessoaId = pessoa.Id;
                    if (!eHavena)
                    {
                        string[] strs = { "COMIGO", "PARA MIM", " E EU ", " EU E ", " MEU ", " MEU.", " .EU ", " EU." };
                        // Se tiver alguns desses nomes significa que é o própio usuário
                        var isMe = strs.Contains(pessoa.Nome);
                        
                        if(isMe)
                        {
                            var usuario = tipoPessoa[0]; // Pega o usuário
                            pessoaNome = usuario.Nome;
                            pessoaId = usuario.Id;
                        }                        
                    }


                    pessoasFrase.Add(new Pessoa
                    {
                        Index = posicao,
                        Nome = pessoaNome,
                        Id = pessoaId
                    });
                    break;
                }
                // Organiza por nome
            }
            pessoasFrase = pessoasFrase.OrderBy(p => p.Index).ToList(); // testar
            
            if (pessoasFrase.Count == 0)
            {
                // Caso não encontre nada então vai adicionar
                // os dados do usuário
                pessoasFrase.Add(new Pessoa
                {
                    Index = 0,
                    Nome = tipoPessoa[0].Nome,
                    Id = tipoPessoa[0].Id
                });
            }
            
            return pessoasFrase;
        }
    }
}
