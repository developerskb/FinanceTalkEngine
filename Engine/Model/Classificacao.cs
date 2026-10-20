using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Engine.Model
{
    public class CategoriaCadastro {
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string CategoriaTipo { get; set; }
    }
    public class SubCategoriaCadastro {
        public int SubCategoriaId { get; set; }
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string PalavrasChaves { get; set; }
    }
    public class Classificacao {
        public int? CategoriaId { get; set; }
        public int? SubCategoriaId { get; set; }
        public string CategoriaNome { get; set; }
        public string SubCategoriaNome { get; set; }
        public string Tipo { get; set; }

        public static Classificacao Identificar(string frase, List<CategoriaCadastro> categorias,
            List<SubCategoriaCadastro> subcategorias)
        {
            string Normalizar(string valor) => Regex.Replace(
                new string((valor ?? "").Normalize(NormalizationForm.FormD)
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray())
                    .ToUpperInvariant(), @"[^\p{L}\p{N}]+", " ").Trim();
            var texto = " " + Normalizar(frase) + " ";
            bool Contem(string valor) => !string.IsNullOrWhiteSpace(valor) && texto.Contains(" " + Normalizar(valor) + " ");
            var cats = (categorias ?? new List<CategoriaCadastro>())
                .Where(c => c != null && c.CategoriaId > 0 && !string.IsNullOrWhiteSpace(c.Nome))
                .GroupBy(c => c.CategoriaId).Select(g => g.First()).ToList();
            var subs = (subcategorias ?? new List<SubCategoriaCadastro>())
                .Where(s => s != null && s.SubCategoriaId > 0 && cats.Any(c => c.CategoriaId == s.CategoriaId))
                .GroupBy(s => s.SubCategoriaId).Select(g => g.First()).ToList();
            var operacoes = Operacao.IdentificarOperacoes(Valor.ResolveFraseCentavos(frase))
                .Where(o => texto.Contains(" " + Normalizar(o.Nome) + " "))
                .ToList();
            // O verbo explícito prevalece sobre termos como SALARIO e PREMIO.
            var verbosFinanceiros = operacoes.Where(o => Regex.IsMatch(o.Nome, @"(?:EI|OU|AMOS|EU)$")).ToList();
            var tipos = (verbosFinanceiros.Count > 0 ? verbosFinanceiros : operacoes)
                .Select(o => o.Tipo == "C" || o.Tipo == "R" ? "R" : "D").Distinct().ToList();
            string tipo = tipos.Count == 1 ? tipos[0] : null;
            var explicitas = cats.Where(c => Contem(c.Nome) &&
                (tipo == null || c.CategoriaTipo != "R" && c.CategoriaTipo != "D" || c.CategoriaTipo == tipo)).ToList();
            CategoriaCadastro categoria = explicitas.Count == 1 ? explicitas[0] : null;
            var candidatas = subs.Where(s => categoria == null || s.CategoriaId == categoria.CategoriaId).ToList();
            var nomes = candidatas.Where(s => Contem(s.Nome)).ToList();
            var palavras = nomes.Count > 0 ? nomes : candidatas.Where(s =>
                (s.PalavrasChaves ?? "").Split(new[] { ',', ';', '|', '\n' }, StringSplitOptions.RemoveEmptyEntries).Any(Contem)).ToList();
            if (categoria == null && explicitas.Count == 0) {
                var pais = palavras.Select(s => s.CategoriaId).Distinct().ToList();
                if (pais.Count == 1) categoria = cats.FirstOrDefault(c => c.CategoriaId == pais[0] &&
                    (tipo == null || c.CategoriaTipo != "R" && c.CategoriaTipo != "D" || c.CategoriaTipo == tipo));
            }
            // Não escolhe entre duas categorias explícitas nem entre duas subcategorias compatíveis.
            var sub = categoria != null && palavras.Count(s => s.CategoriaId == categoria.CategoriaId) == 1
                ? palavras.Single(s => s.CategoriaId == categoria.CategoriaId) : null;
            return new Classificacao {
                CategoriaId = categoria?.CategoriaId, CategoriaNome = categoria?.Nome,
                SubCategoriaId = sub?.SubCategoriaId, SubCategoriaNome = sub?.Nome,
                Tipo = tipo ?? (categoria?.CategoriaTipo == "R" ? "R" : "D")
            };
        }
    }
}
