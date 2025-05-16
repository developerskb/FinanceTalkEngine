using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class LancamentoData: BaseModel
    {
        public string Data {  get; set; }
        public static LancamentoData BuscarData() {
            var data = DateTime.Now;
            var culture = new CultureInfo("pt-BR");
            var result = new LancamentoData
            {
                // Dia da semana
                Nome = data.ToString("dddd", culture)
                .ToUpper()
                .Replace("Ç", "C")
                .Replace("Á", "A"),

                Data = data.ToString("dd/MM/yyyy")
            };

            return result;

        }
    }
}
