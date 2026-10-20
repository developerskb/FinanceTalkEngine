using System.Collections.Generic;
using System.Linq;

namespace Engine.Model
{
    public class ContaBancaria : BaseModel
    {
        public int ContaBancariaId { get; set; }
        public string BancoNome { get; set; }

        public static List<ContaBancaria> IdentificarContas(string frase, List<ContaBancaria> cadastradas)
        {
            if (cadastradas == null) return new List<ContaBancaria>();
            // Reutiliza a comparação de nomes/bancos e a resolução de ambiguidades.
            var candidatos = cadastradas.Where(c => c != null).Select(c => new Cartao {
                CartaoId = c.ContaBancariaId, Nome = c.Nome, BancoNome = c.BancoNome
            }).ToList();
            return Cartao.IdentificarCartoes(frase, candidatos).Select(c => new ContaBancaria {
                ContaBancariaId = c.CartaoId.Value, Id = c.CartaoId.Value,
                Nome = c.Nome, BancoNome = c.BancoNome, Index = c.Index
            }).ToList();
        }
    }
}