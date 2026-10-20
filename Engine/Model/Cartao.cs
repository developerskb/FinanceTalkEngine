using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class Cartao: BaseModel
    {

        public int? CartaoId { get; set; }
        public string BancoNome { get; set; }

        // Os IDs legados da lista fixa não são IDs de cartões do usuário.
        public static List<Cartao> IdentificarCartoes(string frase, List<Cartao> cadastrados)
        {
            if (cadastrados == null) return IdentificarCartoes(frase);
            string Normalizar(string texto) => System.Text.RegularExpressions.Regex.Replace(
                new string((texto ?? "").Normalize(NormalizationForm.FormD)
                    .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                        System.Globalization.UnicodeCategory.NonSpacingMark).ToArray()).ToUpperInvariant(),
                @"[^\p{L}\p{N}]+", " ").Trim();
            var texto = " " + Normalizar(frase) + " ";
            bool Contem(string nome) => !string.IsNullOrWhiteSpace(nome) &&
                texto.Contains(" " + Normalizar(nome) + " ");
            var validos = cadastrados.Where(c => c != null && c.CartaoId > 0 && !string.IsNullOrWhiteSpace(c.Nome))
                .GroupBy(c => c.CartaoId).Select(g => g.First()).ToList();
            var completos = validos.Where(c => Contem(c.Nome)).ToList();
            var encontrados = new List<Cartao>();
            foreach (var c in validos)
            {
                var nome = Normalizar(c.Nome);
                var banco = Normalizar(c.BancoNome);
                var apelido = nome.Split(' ').FirstOrDefault();
                string termo = Contem(c.Nome) ? nome : Contem(banco) ? banco :
                    apelido != null && apelido.Length >= 3 && Contem(apelido) ? apelido : "";
                if (termo == "") continue;
                if (completos.Contains(c) && completos.Count(outro => Normalizar(outro.Nome) == nome) > 1) continue;
                // Um nome completo explícito resolve a ambiguidade do banco.
                if (!completos.Contains(c) && completos.Any(outro =>
                    Normalizar(outro.BancoNome) == banco && banco != "")) continue;
                if (!completos.Contains(c) && validos.Count(outro =>
                    Contem(outro.Nome) || Normalizar(outro.BancoNome) == termo ||
                    Normalizar(outro.Nome).Split(' ').FirstOrDefault() == termo) > 1) continue;
                encontrados.Add(new Cartao { CartaoId = c.CartaoId, Id = c.CartaoId.Value,
                    Nome = c.Nome, BancoNome = c.BancoNome, Index = texto.IndexOf(" " + termo + " ", StringComparison.Ordinal) });
            }
            return encontrados.OrderBy(c => c.Index).ToList();
        }
        public static List<Cartao> GetCartoes()
        {
            List<Cartao> result = new List<Cartao>();

            /*result.Add(new Cartao { Nome = "DEBITO", Id = 1 });
            result.Add(new Cartao { Nome = "CAIXA", Id = 2 });
            result.Add(new Cartao { Nome = "BB", Id = 3 });
            result.Add(new Cartao { Nome = "MASTERCARD", Id = 4 });
            result.Add(new Cartao { Nome = "MINHA CONTA", Id = 5 });
            result.Add(new Cartao { Nome = "SUBMARINO", Id = 6 });*/
            result.Add(new Cartao { Nome = "CAIXA", Id = 2 });
            result.Add(new Cartao { Nome = "BB", Id = 3 });
            result.Add(new Cartao { Nome = "MASTERCARD", Id = 4 });
            result.Add(new Cartao { Nome = "MINHA  CONTA", Id = 5 });
            result.Add(new Cartao { Nome = "SUBMARINO", Id = 6 });
            result.Add(new Cartao { Nome = "CARREFOUR", Id = 7 });



            return result;
        }

        public static List<Cartao> IdentificarCartoes ( string frase )
        {
            List<Cartao> tipoCartoes = GetCartoes();
            var cartaoFrase = new List<Cartao>();

            tipoCartoes.ForEach(cartao =>
            {
                var posicao = frase.IndexOf(cartao.Nome);
                while( posicao >= 0)
                {
                    cartaoFrase.Add(new Cartao
                    {
                       Index = posicao,
                       Nome = cartao.Nome
                    });
                    break;
                }
            });

            cartaoFrase = cartaoFrase.OrderBy(c => c.Nome).ToList();

            return cartaoFrase;
        }

    }
}
