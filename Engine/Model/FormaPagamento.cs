using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class FormaPagamento: BaseModel
    {

        public List<FormaPagamento> GetFormasPagamento()
        {
            List<FormaPagamento> result = new List<FormaPagamento>();

            result.Add(new FormaPagamento { Nome = "DINHEIRO", Id = 1 });
            result.Add(new FormaPagamento { Nome = "CARTAO", Id = 2 });
            result.Add(new FormaPagamento { Nome = "DEBITO", Id = 2 });
            result.Add(new FormaPagamento { Nome = "PIX", Id = 2 });
            result.Add(new FormaPagamento { Nome = "TRANSFERENCIA", Id = 2 });

            return result;
        }

    }
}
