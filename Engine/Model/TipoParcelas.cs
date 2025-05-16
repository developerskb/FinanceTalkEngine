using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class TipoParcelas: BaseModel
    {

        public decimal Valor { get; set; }
        public TipoData Data { get; set; }
        public string CartaoNome { get; set; }
        public string FormaNome { get; set; }
        public string TipoPessoaNome { get; set; }
        public string DespesaNome { get; set; }
        public string OperacaoNome { get; set; }



        public static List<TipoParcelas> GetParcelas( 
            string frase,
            List<TipoRateio> tipoRateio,
            List<Pessoa> pessoas,
            List<TipoPessoa> tipoPessoa,
            List<TipoData> tipoData,
            List<Valor> valores,
            List<TipoFormas> tipoFormas,
            List<Cartao> cartoes,
            List<ReceitaDespesa> despesas)
        {
            List<TipoParcelas> parcelas = new List<TipoParcelas>();

            if( tipoRateio.Count == 0)
            {
                return parcelas;
            }
            else
            {
                int quantidadeDeParcelas = 0;
                var tipo = tipoRateio.First().Tipo;
                var culture = new CultureInfo("pt-BR");
                // Pegando os ultimos valores da lista
                /*var lastPessoas = tipoPessoa.Count;
                var lastData = tipoData.Count;
                var lastValores = valores.Count;
                var lastDespesas = despesas.Count;*/
                // Nesse bloco do código ele procura a quantidade de parcelas dentro da frase
                if( tipo == "P" )
                {
                    // começando pelo 2, pois já existe uma parcela
                    var posicao = 0;
                    /*var parcelas = 0;*/
                    for (int p = 2; p < 24; p++)
                    {
                        var extenso = NumeroExtenso(p).Replace("UM", "UMA").Replace("DOIS", "DUAS");
                        if (frase.IndexOf($" {extenso} VEZ") >= 0)
                            posicao = frase.IndexOf($"{extenso} VEZ");

                        if (posicao == 0 && frase.IndexOf($" {extenso} PARCELA") >= 0)
                            posicao = frase.IndexOf($"{extenso} PARCELA");


                        var numeroStr = Convert.ToString(p); // String.Format("{0:00}", p) ;//p.ToString("00");
                        if (frase.IndexOf($" {numeroStr} VEZ") >= 0)
                            posicao = frase.IndexOf($"{numeroStr} VEZ");

                        if (posicao == 0 && frase.IndexOf($" {numeroStr} PARCELA") >= 0)
                            posicao = frase.IndexOf($"{numeroStr} PARCELA");

                        if (posicao == 0 && frase.IndexOf($" {numeroStr} X") >= 0)
                            posicao = frase.IndexOf($"{numeroStr} X");

                        if (posicao == 0 && frase.IndexOf($" {numeroStr}X") >= 0)
                            posicao = frase.IndexOf($"{numeroStr}X");

                        if( posicao > 0 )
                        {
                            quantidadeDeParcelas = p;
                            break;
                        }

                    }
                }
                if (tipo == "R")
                    quantidadeDeParcelas = 1;
                decimal gastoValor = 0;
                decimal somaParcelas = 0;
                decimal valorParcela = 0;

                gastoValor = valores.Count > 0 ? valores[0].ValorLancamento : 0;
                
                if (quantidadeDeParcelas > 1 || tipoRateio.First().Tipo == "R")
                {
                    var qtdPessoas = tipoPessoa.Count; // Quantidade INICIAL de pessoas encontradas na frase 
                    var primeiraData = tipoData.FirstOrDefault().Data; // Data da primeira Parcela 
                    // Quando existe apenas UM valor o sistema devera dividir pela quantidade de parcelas colocando a diferenca na primeira parc.
                    if ( valores.Count == 1)
                    {
                        for( int p = 0; p < quantidadeDeParcelas; p++ )
                        {
                            if( tipoRateio[0].Tipo == "P" )
                                valorParcela = Math.Round(gastoValor / quantidadeDeParcelas, 2); // Valor da parcela
                            else
                                valorParcela = Math.Round(gastoValor / qtdPessoas, 2); // Valor do rateio

                            // Para cada parcela e necessario ter a mesma quantidade de pessoas
                            for ( int pe = 0; pe < qtdPessoas; pe++)
                            {
                                // Na primeira parcela o sistema so pode alterar data e valor para considerar o valor total como uma das parcelas
                                var proximaData = primeiraData;
                                var nomeDiaDaSemana = proximaData
                                    .ToString("dddd", culture).ToUpper()
                                    .Replace("Ç", "C")
                                    .Replace("Á", "A");
                                if ( p == 1 && pe == 1 )
                                {
                                    valores[valores.Count].ValorLancamento = valorParcela;
                                    valores[valores.Count].Tipo = "G";
                                    tipoData[tipoData.Count].Data = primeiraData; // AQUI TEM QUE ALTERAR O VENCIMENTO QUANDO FOR CARTAO DE CREDITO
                                }
                                else
                                {
                                    if (p >= 1) {
                                        proximaData = primeiraData.AddDays(p * 30); // ((p - 1) * 30);
                                        nomeDiaDaSemana = proximaData
                                        .ToString("dddd", culture).ToUpper()
                                        .Replace("Ç", "C")
                                        .Replace("Á", "A");
                                    }
                                    parcelas.Add(new TipoParcelas
                                    {
                                        Nome = p > 0 ? pessoas[pe].Nome : pessoas.Last().Nome,
                                        Valor = valorParcela,
                                        CartaoNome = cartoes.Count == 0 ? "" : cartoes.Last().Nome,
                                        FormaNome = tipoFormas.Last().Nome,
                                        OperacaoNome = despesas.Last().Nome,
                                        TipoPessoaNome = p > 1 ? tipoPessoa[pe].Nome : tipoPessoa.Last().Nome,
                                        DespesaNome = despesas.Last().Nome,
                                        Data =  new TipoData {
                                            Nome = nomeDiaDaSemana,
                                            Data = proximaData,
                                            DiaDaSemana = Convert.ToInt32(primeiraData.DayOfWeek)
                                        }
                                    });
                                }                                
                            }
                            somaParcelas += Math.Round(valorParcela, 2);
                        }

                        // Resolvendo o problema do arrendamento
                        if( somaParcelas != gastoValor)
                        {
                            var lancamento = valores.FirstOrDefault().ValorLancamento;
                            var gastoValorSomaParcela = Math.Round( gastoValor - somaParcelas , 2);
                            valores.First().ValorLancamento = lancamento + gastoValorSomaParcela;

                            parcelas.First().Valor += gastoValorSomaParcela;
                        }

                        if ( tipoPessoa.Count > 1 )
                        {
                            List<Valor> parcelasValores = new List<Valor>();
                            for( int p = 0; p < quantidadeDeParcelas; p++ )
                            {
                                // Criando uma matriz para armazenar PARCELA e VALOR para utilizar no novo rateio
                                parcelasValores.Add(new Valor {
                                    Index = p,
                                    ValorLancamento = valores[p].ValorLancamento
                                });
                            }

                            for (int k = 0; k < parcelasValores.Count; k++)
                            {
                                // Refazendo os valores das parcelas
                                /*var parcelaAtual = parcelasValores[k].Index;*/
                                var valorAtual = parcelasValores[k].ValorLancamento;

                                var novoValor = Math.Round( valorAtual / qtdPessoas, 2 );
                                var diffValor = Math.Round( Math.Round(valorAtual, 2) - Math.Round(novoValor * qtdPessoas, 2), 2 );

                                for( int v = 0; v < valores.Count; v++ )
                                {
                                    if( valores[v].Id == k)
                                    {
                                        valores[v].ValorLancamento = novoValor + ( diffValor != 0 ? diffValor : 0 );
                                        diffValor = 0;
                                        // somaValores += valores[v].ValorLancamento;
                                    }
                                }
                            }
                        }
                    }
                    else // Quando houver mais de um valor informado
                    {
                        var quantidadeValores = valores.Count;
                        // Deixando a matriz de datas com a mesma quantidade da matriz de valores
                        for (int d = 0; d < quantidadeValores; d++)
                        {
                            if( parcelas.Count < d + 1 )
                            {
                                parcelas.Add(new TipoParcelas
                                {
                                    Nome = pessoas.Count > d ? pessoas[d].Nome : pessoas.Last().Nome,
                                    Valor = valores[d].ValorLancamento,
                                    TipoPessoaNome = tipoPessoa.Count > d ? tipoPessoa[d].Nome : tipoPessoa.Last().Nome,
                                    CartaoNome = cartoes.Count == 0 ? "" : cartoes.Last().Nome,
                                    FormaNome = tipoFormas.FirstOrDefault().Nome,
                                    Data = tipoData.Count > d ? tipoData[d] : tipoData.Last(),
                                    DespesaNome = despesas.Count > d ? despesas[d].Nome : despesas.Last().Nome
                                });
                            }
                        }

                        for( int p = 0; p < quantidadeDeParcelas - 1; p++)
                        {
                            var proximaData = primeiraData.AddDays((p + 1) * 30);
                            var nomeDiaDaSemana = proximaData
                                    .ToString("dddd", culture).ToUpper()
                                    .Replace("Ç", "C")
                                    .Replace("Á", "A");

                            for (int v = 0; v < quantidadeValores; v++)
                            {
                                parcelas.Add(new TipoParcelas
                                {
                                    Data = new TipoData{
                                        Nome = nomeDiaDaSemana,
                                        Data = proximaData,
                                        DiaDaSemana = Convert.ToInt32(primeiraData.DayOfWeek)
                                    },
                                    Nome = pessoas.Count > v ? pessoas[v].Nome : pessoas.Last().Nome,
                                    TipoPessoaNome = tipoPessoa.Count > v ? tipoPessoa[v].Nome : tipoPessoa.Last().Nome,
                                    FormaNome = tipoFormas.Count > v ? tipoFormas[v].Nome : tipoFormas.Last().Nome,
                                    CartaoNome = cartoes.Count > v ? cartoes[v].Nome : (cartoes.Count == 0 ? "" : cartoes.Last().Nome),
                                    Valor = valores[v].ValorLancamento,
                                    DespesaNome = despesas.Count > v ? despesas[v].Nome : despesas.Last().Nome,
                                });
                            }
                        }
                    }
                }
                else
                {
                    parcelas.Add(new TipoParcelas
                    {
                        Nome = pessoas.FirstOrDefault().Nome,
                        Valor = valores.FirstOrDefault().ValorLancamento,
                        TipoPessoaNome = tipoPessoa.FirstOrDefault().Nome,
                        CartaoNome = cartoes.Count == 0 ? "" : cartoes.Last().Nome,
                        FormaNome = tipoFormas.FirstOrDefault().Nome,
                        Data = tipoData.FirstOrDefault(),
                        DespesaNome = despesas.FirstOrDefault().Nome
                    });
                }
                    return parcelas;
            }
        }

        public static string NumeroExtenso(int num)
        {
            // Poderia talvez aplicar mais lógica, porém
            // não será utilizado de forma complexa ... só
            // nessa situação
            List<string> numeroExtenso = new List<string> {
                "ZERO", "UM", "DOIS", "TRES", "QUATRO", "CINCO", "SEIS", "SETE", "OITO", "NOVE", 
                "DEZ", "ONZE", "DOZE", "TREZE", "QUATORZE OU CATORZE", "QUINZE", "DEZESSEIS", "DEZESSETE", "DEZOITO", "DEZENOVE", 
                "VINTE", "VINTE E UM", "VINTE E DOIS", "VINTE E TRÊS", "VINTE E QUATRO"
            };
            // não precisa fazer subtração, pois
            // a lista inicia com o "zero"
            return numeroExtenso[num]; 
        }
    }
}
