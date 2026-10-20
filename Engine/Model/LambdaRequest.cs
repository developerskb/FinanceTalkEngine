using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class LambdaRequest
    {
        public string Usuario { get; init; }
        public string Frase { get; init; }
        public List<Cartao> Cartoes { get; init; }
        public List<ContaBancaria> Contas { get; init; }
        public List<Pessoa> Pessoas { get; init; }
        public List<CategoriaCadastro> Categorias { get; init; }
        public List<SubCategoriaCadastro> SubCategorias { get; init; }
        // public Datetime Data { get; init; }

    }
}
