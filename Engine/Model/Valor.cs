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
            var result = new List<Valor>();

            string regExpInteiro = @"(DESCONTO (DE |)|)(\b(R\$ |R\$|(GASTEI) )|[A-Z]{0}[$])\b[1-9]+[0-9]*";
            string regExpValor = @"(DESCONTO (DE |)|)(\b(R\$ |R\$|(GASTEI) )|[A-Z]{0}[$])\b(\d+|\d{1,3}(.\d{3})*)(\,\d+)";
            string regExpIntDecimal = @"\d+,?\d{0,2}";
            //converter decimal com ','
            System.Globalization.CultureInfo fp = new System.Globalization.CultureInfo("pt-BR");

            /*var matchDecimalInt = Regex.Matches(frase, regExpInteiro);

            if(matchDecimalInt.Count > 0)
            {
                foreach(Match match in matchDecimalInt)
                {
                    var reais = Convert.ToDecimal(match.Value.Replace("R$", "").Trim(), fp);
                    result.Add(new Valor { ValorLancamento = reais, Index = match.Index });
                }
            }*/

            var matchReais = Regex.Matches(frase, regExpValor);
            if (matchReais.Count > 0)
            {
                foreach (Match match in matchReais)
                {
                    if (!(match.Value.Contains("DESCONTO") || match.Value.Contains("DESCONTO DE")))
                    {
                        var reais = Convert.ToDecimal(match.Value.Replace("R$", "").Replace("GASTEI", "").Replace("$","").Trim(), fp);
                        // aqui ele vai substituir o valor para uma mascara
                        // para não ter problemas de repetir o mesmo valor
                        var index = frase.IndexOf(match.Value);
                        var len = match.Value.Length;
                        frase = Stuff(frase, index, len, new String('.', len));
                        // frase.Substring(index, match.Value.Length);

                        result.Add(new Valor { ValorLancamento = reais, Index = match.Index });
                    }
                }
            }

            var matchInts = Regex.Matches(frase, regExpInteiro);
            if (matchInts.Count > 0)
            {
                foreach (Match match in matchInts)
                {
                    var t = match.Value;
                    // Não consegui implementar com o regex para que ele ignorasse a frase caso começasse
                    // com DESCONTO OU DESCONTO DE então faço a verificação manual
                    if(!(t.Contains("DESCONTO") || t.Contains("DESCONTO DE")))
                    {
                        var reais = Convert.ToDecimal(match.Value.Replace("R$", "").Replace("GASTEI", "").Replace("$", "").Trim(), fp);
                        result.Add(new Valor { ValorLancamento = reais, Index = match.Index });
                    }
                }
            }

            result = result.OrderBy(r => r.Index).ToList();

            return result;
        }
    }
}