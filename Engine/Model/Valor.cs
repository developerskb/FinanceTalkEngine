using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Engine.Utils.Utils;

namespace Engine.Model
{
    public class Valor: BaseModel
    {
        public Classificacao Classificacao { get; set; }
        public decimal ValorLancamento { get; set; }        

        static List<string> GetTagsCentavos()
        {
            return new List<string> { "CENTAVOS" };
        }

        static int NumeroExtensoToInt( string numeroExtenso )
        {
            numeroExtenso = numeroExtenso.Trim();
            List<string> numeroExtesoDecimal = new List<string> { 
                "ZERO",
                "UM",
                "DOIS",
                "TRES",
                "QUATRO",
                "CINCO",
                "SEIS",
                "SETE",
                "OITO",
                "NOVE" };
            
            if(numeroExtesoDecimal.Contains(numeroExtenso))
            {
                // Se o número for menor que 10 então vai retornar a posição dele do index
                // Que é exatamente o número dele
                return numeroExtesoDecimal.IndexOf(numeroExtenso);
            }
            List<string> numeroExtesoDezenaAMilhar = new List<string> {
                // 0-9
                "DEZ", "ONZE", "DOZE", "TREZE", "CATORZE", "QUINZE", "DEZESSEIS", "DEZESSETE", "DEZEOITO", "DEZENOVE",
                // 10-17
                "VINTE", "TRINTA", "QUARENTA", "CINQUENTA", "SESSENTA", "SETENTA", "OITENTA", "NOVENTA",
                // 18 - 27
                "CENTO", "DUZENTOS", "TREZENTOS", "QUATROCENTOS", "QUINHENTOS", "SEISCENTOS", "SETECENTOS", "OITOCENTOS", "NOVECENTOS",
                "CEM", "MIL"
            };

            if(numeroExtesoDezenaAMilhar.Contains(numeroExtenso))
            {
                var indexExtenso = numeroExtesoDezenaAMilhar.IndexOf(numeroExtenso);
                // Se o index for inferior a 10 entedesse que é inferior a 20 então soma 10
                // para retornar o index certo
                if (indexExtenso < 10) return indexExtenso + 10;
                else if (indexExtenso == 10) return indexExtenso + 10;
                // pega o index e soma + 2 para corrigir o index
                // apartir daqui.
                // pega o resto de 10 ou seja se for 10 sobra 3
                // sobrando 3 multiplica por 10 assim retornando 30 
                else if (indexExtenso <= 17) return (indexExtenso + 2) % 10 * 10;
                else if (indexExtenso == 18) return 100;
                else if (indexExtenso <= 26) return (indexExtenso - 17 ) * 100;
                else if (indexExtenso <= 27) return 100;
                else return 1000;
            }

            return 0;
        }

        public static string ResolveFraseCentavos(string frase)
        {            
            string aFrase = frase.ToUpper();

            // Alteracoes de palavras para facilitar a leitura/interpretacao da frase
            aFrase = aFrase
            .Replace("Á", "A").Replace("À", "A").Replace("Ã", "A").Replace("Â", "A")
            .Replace("É", "E").Replace("È", "E").Replace("Ê", "E")
            .Replace("Í", "I").Replace("Ì", "I").Replace("Î", "I")
            .Replace("Ó", "O").Replace("Ò", "O").Replace("Õ", "O").Replace("Ô", "O")
            .Replace("Ù", "U").Replace("Ú", "U").Replace("Û", "U")
            .Replace("Ç", "C")
            .Replace("GOSTEI", "GASTEI")
            .Replace("DIA PRIMEIRO.DE","DIA 01 DE")
            .Replace("LANCAR DEBITO","LANCAR DEBIT0")
            .Replace("GERAR DEBITO","GERAR DEBIT0")
            .Replace("CRIAR DEBITO","CRIAR DEBIT0")
            .Replace(":",",");


            // var regexMilharPorExtenso = @"[A-Z]*(\b MIL)(\b E ([A-Z]{1,7}(ENTOS|ENTO)(\b E ([A-Z]{1,6}(NTE|NTA)(\b E [A-Z]*)|[A-Z]{2,6})|)|[A-Z]{3,9}|[A-Z]{2,6}(NTE|NTA))(\b E [A-Z]*|)|)";
            // var matchMilharExtenso = Regex.Matches(aFrase, regexMilharPorExtenso);

            // Número por extenso

            // Esse código regex irá procurar padrões em frases que iniciem
            // com centenas ou milhares.
            var regexMilharCentenaPorExtenso = @"([A-Z]*(\b MIL)|[A-Z]{1,7}(ENTOS|ENTO))(\b E ([A-Z]{1,7}(ENTOS|ENTO)(\b E ([A-Z]{1,6}(NTE|NTA)(\b E [A-Z]*)|[A-Z]{2,6})|)|[A-Z]{3,9}|[A-Z]{2,6}(NTE|NTA))(\b E [A-Z]*|)|)";
            var matchMilharCentenaExtenso = Regex.Matches(aFrase, regexMilharCentenaPorExtenso);
            if (matchMilharCentenaExtenso.Count > 0)
            {
                // Vai dividir a string pelo "E" dependendo da quantidade vai aplicar a lógica
                // para retornar o valor...
                foreach( Match matchMilhar in matchMilharCentenaExtenso)
                {
                    var milharExtenso = matchMilhar.Value;
                    var milharExtensoSplitted = milharExtenso.Split(" E ");
                    var posicaoNaFrase = aFrase.IndexOf(milharExtenso);

                    // inicio e final da frase para depois concatenar com o novo valor
                    var inicio = aFrase.Substring(0, posicaoNaFrase);
                    var final = aFrase.Substring(posicaoNaFrase + milharExtenso.Length);
                    
                    decimal total = 0;

                    var primeiroCentenaMilhar = milharExtensoSplitted.First().Trim();
                    var isCentenaMilhar = primeiroCentenaMilhar.Contains("ENTO") || primeiroCentenaMilhar.Contains("MIL");
                    // Essa parte do código servira para descobrir se a primeira palavra
                    // é milhar, centena, dezena ou unidade
                    if(milharExtensoSplitted.Length >= 1)
                    {   
                        if(isCentenaMilhar) // Se caso as primeiras palavras iniciarem com uma centena ou milhar
                        {
                            if (primeiroCentenaMilhar.Contains("MIL"))
                            {   // Se a primeira palavra conter a palavra mil então sabesse que é um milhar
                                // Vai pegar a palavra antes do mil o prefixo no caso
                                var numeroExtensoPrefixo = primeiroCentenaMilhar.Replace("MIL", "");
                                var milharPrefixo = NumeroExtensoToInt(numeroExtensoPrefixo) * 1000;
                                total += milharPrefixo;
                            }
                            if (primeiroCentenaMilhar.Contains("ENTOS") || primeiroCentenaMilhar.Contains("ENTO"))
                            {   // Se a primeira palavra conter a palavra mil então sabesse que é um milhar
                                // Vai pegar a palavra antes do mil o prefixo no caso
                                var centena = NumeroExtensoToInt(primeiroCentenaMilhar);
                                total += centena;
                            }
                        }
                        else
                        {
                            // Se a primeirar palavra não for centena ou milhar, resta apenas dezena
                            // e unidade
                            var dezenaOuUnidade = NumeroExtensoToInt(primeiroCentenaMilhar);
                            total += dezenaOuUnidade;
                        }
                    }
                    if (milharExtensoSplitted.Length >= 2) // até 20
                    {

                        // Nesse caso se for igual a 2 o valor pode variar entre
                        // 1-20 ou dezenas inteiras ou centenas inteiras
                        var segundaPalavara = milharExtensoSplitted[1];
                        var centenaDezenaUnidade = NumeroExtensoToInt(segundaPalavara);
                        total += centenaDezenaUnidade;

                        if(milharExtensoSplitted.Length >= 3)
                        {
                            // A 3° palavra podera ser a unidade ex: vinte e *um* ou duzentos e *nove*
                            // ou a dezena
                            var terceiraPalavra = milharExtensoSplitted[2];
                            var unidadeOuDezena = NumeroExtensoToInt(terceiraPalavra);
                            total += unidadeOuDezena;
                            if(milharExtensoSplitted.Length == 4)
                            {
                                var ultimaPalavra = milharExtensoSplitted[3];
                                var unidade = NumeroExtensoToInt(ultimaPalavra);
                                total += unidade;
                            }
                        }
                    }
                    aFrase = $"{inicio} R$ {total} {final}";
                }
            }

            // Esse código regex irá procurar padrões em palavras que sejam dezenas ou unidades
            var regexDezenaUnidade = @"(([A-Z]{1,6}(NTE|NTA)((\b E [A-Z]*)|)|(DEZ)[A-Z]{4,6}|[A-Z]{2,6}\w*ZE)|([ACOINTRQUSE]{3,5}(O\b)|(DEZ\b|UM\b)|[DNVOITRES]{3}((S\b)|(E\b))))";
            var matchDezenaUnidadeExtenso = Regex.Matches(aFrase, regexDezenaUnidade);
            if (matchDezenaUnidadeExtenso.Count > 0)
            {
                foreach(Match match in matchDezenaUnidadeExtenso) {
                    var matchDezenaUnidade = match.Value.ToString().Trim();
                    var matchDezenaUnidadeSplitted = matchDezenaUnidade.Split(" E ");

                    var posicaoInicial = aFrase.IndexOf(matchDezenaUnidade);
                    var inicio = aFrase.Substring(0, posicaoInicial);
                    var final = aFrase.Substring(posicaoInicial + matchDezenaUnidade.Length);

                    var total = 0;
                    // Vai somar todas as palavras que encontrarem
                    // Caso a palavra não seja encontrada na função então
                    // a função retorna 0 ( não ire mudar os cálculos )
                    foreach( string dezenaUnidade in matchDezenaUnidadeSplitted)
                    {
                        total += NumeroExtensoToInt(dezenaUnidade);
                    }
                    if( total > 0 ) // Só troca caso for maior que 0
                    {               // para evitar problemas
                        aFrase = $"{inicio} R$ {total} {final}";
                    }
                }
            }

            // Get tags
            var tagsCentavos = GetTagsCentavos();

            // trocar o "," caso os proximos número forem números
            var regexCentavosFrase = @"( ,)[0-9]{2}";
            var matchCentavosVirgula = Regex.Matches(aFrase, regexCentavosFrase);
            if(matchCentavosVirgula.Count > 0)
            {
                foreach( var match in matchCentavosVirgula)
                {
                    var centavosEncontrado = match.ToString();
                    var centavosPalavraNova = centavosEncontrado.Replace(",", "") + " CENTAVOS ";
                    var indexMatch = aFrase.IndexOf(match.ToString()); // encontro na frase
                    var inicio = aFrase.Substring(0, indexMatch);
                    var final = aFrase.Substring(indexMatch + centavosEncontrado.Length);
                    aFrase = inicio + centavosPalavraNova + final;
                }
            }
            var regexECentavos = @"[1-9]{1}[0-9]+ (COM|E) +[0-9]{1,3}";
            // Busca valores de 1 a 9 em apenas 1 caractere
            // Busca valores de 0 a 9
            // Busca a palavra E ou COM no meio da frase
            // Busca valores de 0 a 9 até 3 digitos
            var matchCentavosE = Regex.Matches(aFrase, regexECentavos);
            if (matchCentavosE.Count > 0)
            {
                foreach (var match in matchCentavosE)
                {
                    var centavosEncontrado = match.ToString();
                    var centavosPalavraNova = "R$ " + centavosEncontrado.Replace("E", ".").Replace("COM",".").Replace(" ","");
                    var indexMatch = aFrase.IndexOf(match.ToString()); // encontro na frase
                    var inicio = aFrase.Substring(0, indexMatch);
                    var final = aFrase.Substring(indexMatch + centavosEncontrado.Length);
                    aFrase = inicio + centavosPalavraNova + final;
                }
            }

            //padroes de pesquisa
            string regExpCents = "[1-9]+[0-9]*";
            string regExpValor = @"(\d+|\d{1,3}(.\d{3})*)(\,\d+)";
            //converter decimal com ','
            System.Globalization.CultureInfo fp = new System.Globalization.CultureInfo("pt-BR");

            foreach (string tag in tagsCentavos)
            {
                //pegar a substring de centavos
                int indexTag = aFrase.IndexOf(tag);
                int indexReal = aFrase.IndexOf("R$");
                if (indexTag > -1)
                {
                    string fraseReal = aFrase.Substring(0, indexTag + tag.Length);
                    fraseReal = fraseReal.Substring(fraseReal.LastIndexOf("R$") > -1 ? fraseReal.LastIndexOf("R$") : fraseReal.IndexOf(" ")).Trim();

                    decimal valorCents = 0;
                    decimal valor = 0;

                    var matchCents = Regex.Matches(fraseReal, regExpCents);
                    if (matchCents.Count > 0)
                    {
                        // o último valor é centavos
                        Match matchCentavos = matchCents.LastOrDefault();
                        {
                            var cents = Convert.ToInt32(matchCentavos.Value);
                            valorCents += Convert.ToDecimal(cents) / 100m;
                        }
                        if (matchCents.Count > 1)
                        {
                            //se tiver mais de um o primeiro é reais
                            Match matchValor = matchCents.FirstOrDefault();
                            var reais = Convert.ToInt32(matchValor.Value);
                            valor += reais;
                        }
                    }
                    var matchReais = Regex.Matches(fraseReal, regExpValor);
                    if (matchReais.Count > 0)
                    {
                        var reais = Convert.ToDecimal(matchReais[0].Value, fp);
                        valor += reais;
                    }
                    valor += valorCents;
                    aFrase = aFrase.Replace(fraseReal, "R$ " + valor.ToString());
                }
            }

            return aFrase;
        }

        // Buscando todos os valores da frase
        public static List<Valor> BuscarValores(string frase)
        {
            var verbos = string.Join("|", Operacao.GetOperacoes().Select(o => o.Nome.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().OrderByDescending(v => v.Length).Select(Regex.Escape));
            const string numero = @"(?:\d{1,3}(?:\.\d{3})+(?:,\d{1,2})?|\d+(?:[,.]\d{1,2})?)";
            // Só extrai valores com contexto monetário: moeda, verbo financeiro ou sufixo reais.
            var padrao = @"(?<!\w)(?:(?<desconto>DESCONTO(?: DE)?\s+)?(?:R\$\s*|\$\s*|(?:" + verbos +
                @")\s+(?:R\$\s*|\$\s*)?))(?<valor>" + numero + @")(?!\d|[.,]\d|/)(?:\s+REA(?:IS|L)\b)?" +
                @"|(?<![\d.,/])(?<valor>" + numero + @")\s+REA(?:IS|L)\b";
            var resultado = new List<Valor>();
            foreach (Match match in Regex.Matches(frase ?? "", padrao, RegexOptions.IgnoreCase)) {
                var antes = (frase ?? "").Substring(0, match.Index);
                if (match.Groups["desconto"].Success ||
                    Regex.IsMatch(antes, @"\bDESCONTO(?:\s+DE)?\s*$", RegexOptions.IgnoreCase)) continue;
                var texto = match.Groups["valor"].Value;
                var cultura = texto.Contains(",") ? System.Globalization.CultureInfo.GetCultureInfo("pt-BR")
                    : System.Globalization.CultureInfo.InvariantCulture;
                if (!texto.Contains(",") && Regex.IsMatch(texto, @"^\d{1,3}(?:\.\d{3})+$")) texto = texto.Replace(".", "");
                if (!decimal.TryParse(texto, System.Globalization.NumberStyles.Number, cultura, out var valor) ||
                    valor > 9999999999999.99m) continue;
                resultado.Add(new Valor { ValorLancamento = valor, Index = match.Index });
            }
            return resultado.OrderBy(v => v.Index).ToList();
        }
    }
}
