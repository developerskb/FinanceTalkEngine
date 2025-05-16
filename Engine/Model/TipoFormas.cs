using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class TipoFormas: BaseModel
    {

        public static List<TipoFormas> GetTiposFormas()
        {
            List<TipoFormas> result = new List<TipoFormas>();
            result.Add(new TipoFormas { Nome = "DINHEIRO", Id = 1 });
            result.Add(new TipoFormas { Nome = "CARTAO", Id = 2 });
            result.Add(new TipoFormas { Nome = "DEBITO", Id = 2 });
            result.Add(new TipoFormas { Nome = "PIX", Id = 2 });
            result.Add(new TipoFormas { Nome = "TRANSFERENCIA", Id = 2 });
            return result;
        }

        public static List<TipoFormas> IdentificarFormas(string frase)
        {

            List<TipoFormas> tipoFormas = GetTiposFormas();
            var formasFrase = new List<TipoFormas>();
            var posicao = 0;

            foreach (var forma in tipoFormas)
            {
                posicao = frase.IndexOf(forma.Nome);

                while (posicao >= 0)
                {
                    formasFrase.Add(new TipoFormas
                    {
                        Index = posicao,
                        Nome = forma.Nome,
                        Id = forma.Id
                    });
                    break;
                }
            }

            if (formasFrase.Count == 0)
            {
                formasFrase.Add(new TipoFormas
                {
                    Index = 0,
                    Nome = tipoFormas[0].Nome,
                    Id = tipoFormas[0].Id
                });
            }

            formasFrase = formasFrase.OrderBy( forma => forma.Nome ).ToList();

            return formasFrase;
        }

    }
}
