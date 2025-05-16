using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class TipoRateio: BaseModel
    {
        public static List<TipoRateio> GetTipoRateios()
        {

            var result = new List<TipoRateio>();
            result.Add(new TipoRateio { Nome = "PARCELE", Id = 1, Tipo = "P" });
            result.Add(new TipoRateio { Nome = "PARCELA", Id = 2, Tipo = "P" });
            result.Add(new TipoRateio { Nome = "DIVIDI", Id = 5, Tipo = "P" });
            result.Add(new TipoRateio { Nome = "DIVIDA", Id = 3, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "DIVISAO", Id = 4, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "SEPARAR EM ", Id = 6, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "SEPARE", Id = 7, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "REPARTA", Id = 8, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "REPARTI EM ", Id = 7, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "REPARTIR EM ", Id = 8, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "RATEIO", Id = 9, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "RATEEI", Id = 10, Tipo = "R" });
            result.Add(new TipoRateio { Nome = "RATEAR", Id = 11, Tipo = "R" });
            return result;
        }
    
        public static List<TipoRateio> EncontrarRateio(string frase)
        {
            List<TipoRateio> tipoRateios = GetTipoRateios();
            List<TipoRateio> rateiosFrase = new List<TipoRateio>();

            tipoRateios.ForEach(rateio =>
            {
                var posicao = frase.IndexOf(rateio.Nome);
                // adiciona apenas uma vez.
                if (posicao > 0 && rateiosFrase.Count == 0)
                {
                    rateiosFrase.Add(new TipoRateio {
                        Index = 0,
                        Nome = rateio.Nome,
                        Id = rateio.Id,
                        Tipo = rateio.Tipo,
                    });
                }
            });

            return rateiosFrase;
        }
    }
}
