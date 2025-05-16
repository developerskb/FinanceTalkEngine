using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class Lancamento
    {
        public string Frase { get; set; }
        public string FraseConvertida { get; set; }
        public string FraseHavena { get; set; }

        public List<Valor> Valores { get; set; } = new List<Valor>();
        public List<ReceitaDespesa> ReceitasDespesas { get; set; } = new List<ReceitaDespesa>();
        public List<Pessoa> Pessoas { get; set; } = new List<Pessoa>();
        public List<TipoFormas> Formas { get; set; }
        public List<TipoPessoa> TipoPessoas { get; set; }
        public List<Cartao> Cartoes { get; set; }
        public List<Operacao> Operacoes { get; set; }
        public List<TipoData> Datas { get; set; } = new List<TipoData>();
        public List<TipoRateio> Rateio { get; set; } = new List<TipoRateio>();
        public List<TipoParcelas> Parcelas { get; set; } = new List<TipoParcelas>();
        public string Usuario { get; set; }
        # region Havena
        public decimal Sinal { get; set; }
        public DateTime DataSinal { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime PrevisaoEntrega { get; set; }
        public string Cliente { get; set; }
        public List<HavenaProdutos> Produtos { get; set; }
        public List<HavenaQuantidades> Quantidades { get; set; }
        public List<HavenaDescontos> Descontos { get; set; }
        public List<HavenaItens> Itens { get; set; }
        # endregion
        public LancamentoData LancamentoData { get; set; }

        private bool eHavena { get; set; }

        public Lancamento()
        {

        }

        public Lancamento(string frase, string usuario)
        {
            Frase = frase;
            Usuario = $"{usuario.ToUpper()[0]}{usuario.Substring(1).ToLower()}";
            FraseConvertida = frase.ToUpper();
        }

        public void ProcessarFrase()
        {
            //fazer as conversões da frase
            eHavena = FraseConvertida.IndexOf("RAVENA") >= 0;
            
            Frase = Frase;
            FraseConvertida = Valor.ResolveFraseCentavos(Frase);

            Valores = Valor.BuscarValores(FraseConvertida);
            ReceitasDespesas = ReceitaDespesa.IdentificarDespesas(FraseConvertida);
            // Passa o usuário caso não encontre pessoas na frase
            Pessoas = Pessoa.IdentificarPessoas(FraseConvertida, Usuario, eHavena);
            Datas = TipoData.IdentificarDatas(FraseConvertida);

            DataSinal = Datas.Max(data => data.Data);

            Usuario = Usuario;
            LancamentoData = LancamentoData.BuscarData();
            Formas = TipoFormas.IdentificarFormas(FraseConvertida);
            TipoPessoas = TipoPessoa.IdentificarTipoPessoa(FraseConvertida);
            Cartoes = Cartao.IdentificarCartoes(FraseConvertida);
            Operacoes = Operacao.IdentificarOperacoes(FraseConvertida);
            Rateio = TipoRateio.EncontrarRateio(FraseConvertida);

            if (eHavena)
            {
                // A palavra resto nao podera ser utilizada no Havena porque sempre existira apenas um valor (final da venda)
                // o sistema vai se perder em caso de parcelamento, porque os valores "resto" sera repetido de acordo com 
                // a quantidade de parcelas, por isso que e necessario tirar essa palavra
                FraseHavena = FraseConvertida.Replace("RESTO", "#ESTO");


                Sinal = HavenaSinal.IndentificarSinal(FraseHavena, Valores);
                Produtos = HavenaProdutos.IdentificarProdutos(FraseHavena);
                Quantidades = HavenaQuantidades.IdentificarQuantidades(FraseHavena);
                Descontos = HavenaDescontos.IdentificarDescontos(FraseHavena);

                // não entendi quantidade de eliminados
                // ver o que é necessário aplica aqui posteriormente

                while (Quantidades.Count < Produtos.Count)
                {
                    Quantidades.Add(new HavenaQuantidades
                    {
                        // Quantidades.Last().Quantidade,
                        Quantidade = 1, //
                        Index = 0
                    });
                }
                while (Descontos.Count < Produtos.Count)
                {
                    Descontos.Add(new HavenaDescontos
                    {
                        Desconto = Descontos.Count > 0 ? Descontos.Last().Desconto : 0,
                        Index = 0,
                        Tipo = "V"
                    });
                }
                while (Valores.Count < Produtos.Count)
                {
                    Valores.Add(new Valor
                    {
                        Tipo = "G",
                        ValorLancamento = Valores.Last().ValorLancamento + 1,
                        Index = 0,
                        Id = 1
                    });
                }

                Cliente = Pessoas.First().Nome;
                DataCadastro = Datas.Min(d => d.Data); // A data de Cadastro é menor
                PrevisaoEntrega = Datas.Max(d => d.Data);
                DataSinal = PrevisaoEntrega;

                Itens = HavenaItens.IdentificarItens(Produtos, Descontos, Quantidades, Valores);

                Datas.Clear();
                Datas.Add(new TipoData {
                    Index = 1,
                    Data = PrevisaoEntrega,
                    DiaDaSemana = Convert.ToInt32(PrevisaoEntrega.DayOfWeek),
                    Nome = "PREV.ENTREGA",
                    Id = 0
                });


                if (Sinal > 0 || FraseHavena.Contains("PARCEL"))
                {
                    var valorTotal = Valores.Sum(valor => valor.ValorLancamento) - Sinal;
                    Valores.Clear();
                    Valores.Add(new Valor
                    {
                        Index = 1 + Sinal > 0 ? 1 : 0,
                        ValorLancamento = valorTotal - Sinal,
                        Tipo = "G",
                        Id = 1
                    });
                }
                // Aplica o Desconto
                if (Valores.Count == 1 && Descontos.Count == 1)
                {
                    Valores.First().ValorLancamento -= Descontos.First().Desconto;
                }
            }

            if (Valores.Count != Pessoas.Count && !eHavena){ // .and. ! eHavena
                decimal maiorValor = 0;
                var pMaiorValor = 0;
                decimal somaValores = 0;
                int i = 0; // Index

                foreach (var valor in Valores)
                {
                    if (valor.ValorLancamento > maiorValor)
                    {
                        maiorValor = valor.ValorLancamento;
                        pMaiorValor = i;
                    }
                    somaValores += valor.ValorLancamento;
                    i++;
                    }
                    if (somaValores > 0) {
                        somaValores -= maiorValor;
                    }
                    var roundSoma = Math.Round(somaValores, 2);
                    var roundMaior = Math.Round(maiorValor, 2);
                    if ( roundSoma == roundMaior ) {
                        if (pMaiorValor == 0) {
                            Pessoas.Insert(0, new Pessoa {
                                Id = 0,
                                Nome = Usuario
                            });
                        if(Valores.Count > 0)
                            {
                                Valores.FirstOrDefault().Tipo = "N";
                            }
                        }
                        if(pMaiorValor == Valores.Count - 1)
                        {
                             if (Valores.Count > 0)
                             {
                                 Valores.Last().Tipo = "N";
                                }
                            }
                        }
                        else
                        {
                            var valorDiferencia = Math.Round(maiorValor, 2) - Math.Round(somaValores, 2);

                            if( pMaiorValor == 0 )
                            {
                                Pessoas.Insert(0, new Pessoa { 
                                    Id = 0,
                                    Nome = Usuario
                                });
                                Valores.First().ValorLancamento = valorDiferencia;
                                Valores.First().Tipo = "N";
                            }
                            if(pMaiorValor + 1 == Valores.Count)
                            {
                                Valores.Last().ValorLancamento = valorDiferencia;
                            Valores.Last().Tipo = "N"; // para ele ignorar
                            }
                        }
            }
            var index = 0;
            Parcelas = TipoParcelas.GetParcelas(FraseConvertida, Rateio, Pessoas, TipoPessoas, Datas, Valores, Formas, Cartoes, ReceitasDespesas);
            // Remove o primeiro valor, pois seram acrescentados novos valores
            // do list de Parcelas
            // É necessário remover o inicial
            if( Parcelas.Count > 1)
            {
                if(Valores.Count == 1) Valores.Clear();
                if(Datas.Count == 1 || Datas.Count == Parcelas.Count)  Datas.Clear();
            }
            Parcelas.ForEach(parcela =>
            {
                Valores.Add(new Valor
                {
                    ValorLancamento = parcela.Valor,
                    Index = index,
                });
                Datas.Add(
                    new TipoData
                    {
                        Index = Datas.Count,
                        Data = parcela.Data.Data,
                        Tipo = "S",
                        Nome = parcela.Data.Nome,
                        DiaDaSemana = parcela.Data.DiaDaSemana
                    }
                );
                index++;
            });

            // Adiciona o Sinal no inicio do indice
            if ( Sinal > 0 )
            {
                Valores.Insert(0, new Valor {
                    Index = -1,
                    ValorLancamento = Math.Abs(Sinal) * -1,
                });

                Datas.Insert(0, new TipoData {
                    Index = -1,
                    Id = 0,
                    Data = DataSinal,
                    DiaDaSemana = Convert.ToInt32(DataSinal.DayOfWeek),
                    Nome = "HOJE"
                });
            }

        }

        public override string ToString()
        {
            var result = ""; // $"{Frase}\n{FraseConvertida}";
            result += $"\nUsuario.....: {Usuario}"; // Deixa apenas a primeira letra me maiúsculo
            result += $"\nLancamento..: {LancamentoData.Data}({LancamentoData.Nome})";
            result += "\nFrase.......: " + Frase;
            result += "\nDatas.......: " + String.Join(", ", Datas.Select(d => $"P({d.Index}) {d.Nome.Trim()} ({d.Data.ToString("dd/MM/yyyy")})").ToArray());
            result += "\nDespesas/Rec: " + String.Join(", ", ReceitasDespesas.Select(d => $"P({d.Index}) {d.Nome.Trim()}"));
            result += "\nPessoas.....: " + String.Join(", ", Pessoas.Select(p => $"P({p.Index}) {p.Nome.Trim()}"));
            result += "\nFormas......: " + String.Join(", ", Formas.Select(f => $"P({f.Index}) {f.Nome.Trim()}").ToArray());
            if (Valores.Count > 0) { 
                result += "\nValores.....: " + String.Join(", ", Valores.Select(v => $"P({v.Index}) {v.ValorLancamento.ToString("0.00")}").ToArray());
            }
            result += "\nTipo Pessoas: " + String.Join(", ",TipoPessoas.Select(p => $"P({p.Index}) {p.Nome.Trim()}").ToArray()); ;
            
            if( Cartoes.Count >= 1 )
                result += "\nCartoes.....: " + String.Join(", ", Cartoes.Select(c => $"P({c.Index}) {c.Nome.Trim()}").ToArray());
            
            result += "\nOperacoes...: " + String.Join(", ", Operacoes.Select(d => $"P({d.Index}) {d.Nome.Trim()}({d.Tipo})").ToArray());

            if (Parcelas.Count > 1) {
                var max = (Parcelas.Count > 0 ? Parcelas.Sum(p => p.Valor) : (Valores.Sum(v => v.ValorLancamento)));
                result += $"\nMaior Valor.: { String.Format($"{max:0.00}") }";
            }
            // Esse tipo N era pra funcionar como um NÃO - o robson adicionou no script dele
            // Basicamente ele ignora o primeiro registro quando for inserido
            if (Valores.Count > 1 || Parcelas.Count > 1)
            {
                var sum = (Parcelas.Count > 0 ? Parcelas.Where(p => p.Tipo != "N").Sum(p => p.Valor) : ( Valores.Where( v => v.Tipo != "N").Sum( v => v.ValorLancamento ) ));
                result += $"\nSoma........: { String.Format($"{sum:0.00}") }";
            }
            if(Rateio.Count >= 1)
                result += $"\nRateio/Parc.: " + String.Join(", ",Rateio.Select(rateio => $"({rateio.Index}) {rateio.Nome}({rateio.Tipo})").ToArray());

            if (Parcelas.Count >= 1)
                result += "\nQtd.Parcelas: " + Convert.ToString(Parcelas.Count).PadLeft(2, '0');
            result += String.Format("\n{0, 10} {1, 11} {2, -16} {3, -16} {4, -16} {5, 6} {6, -11}", "DATA", "USUARIO", "PESSOA", "FORMA PGTO.", "CARTAO", "VALOR", "DESP/REC.");
            if (Parcelas.Count >= 1)
            {
                result += String.Join("",
                    Parcelas.Select(p =>
                       String.Format("\n{0, 10:dd/MM/yyyy} {1, 11} {2, -16} {3, -16} {4, -16} {5, 6:0.00} {6, -11}",
                       p.Data.Data, p.TipoPessoaNome, p.Nome, p.FormaNome, p.CartaoNome, p.Valor * -1, p.DespesaNome)
                    ).ToArray());
            }
            else
            {
                List<string> dicValores = new List<string>();
                List<List<string>> dic = new List<List<string>>();
                var i = 0;
                Valores.ForEach(val => {
                    dicValores.Add(Datas.Count > i ? Datas[i].Data.ToString("dd/MM/yyyy") : Datas.Last().Data.ToString("dd/MM/yyyy"));
                    dicValores.Add(Convert.ToString(TipoPessoas.Count       > i ? TipoPessoas[i].Nome : TipoPessoas.Last().Nome ));
                    dicValores.Add(Convert.ToString(Pessoas.Count           > i ? Pessoas[i].Nome : Pessoas.Last().Nome ));
                    dicValores.Add(Convert.ToString(Formas.Count            > i ? Formas[i].Nome : Formas.Last().Nome ));
                    dicValores.Add(Convert.ToString(Cartoes.Count           > i ? Cartoes[i].Nome : ( Cartoes.Count == 0 ? "" : Cartoes.Last().Nome) ));
                    dicValores.Add(Convert.ToString(val.ValorLancamento * -1));
                    dicValores.Add(Convert.ToString(ReceitasDespesas.Count > i ? ReceitasDespesas[i].Nome : ReceitasDespesas.Last().Nome ));
                    i++;
                    dic.Add(dicValores);
                    dicValores = new List<string>();
                });

                var valor =
                result += String.Join(", ", dic.Select(val =>
                    String.Format("\n{0, 10:dd/MM/yyyy} {1, 11} {2, -16} {3, -16} {4, -16} {5, 6:0.00} {6, -11}",
                        val[0], val[1], val[2], val[3], val[4], val[5], val[6] )
                    )
                );


               /* result += String.Join("",
                    Valores.Select(v =>
                       String.Format("\n{0, 10:dd/MM/yyyy} {1, 11} {2, -16} {3, -16} {4, -16} {5, 6:0.00} {6, -11}",
                       p.Data.Data, p.TipoPessoaNome, p.Nome, p.FormaNome, p.CartaoNome, p.Valor * -1, p.DespesaNome)
                    ).ToArray());*/
            }

            if(eHavena)
            {
                if (!String.IsNullOrWhiteSpace(FraseHavena))
                {
                    result += "\n\n\nFrase.......: " + FraseConvertida;
                    result += "\nNova Frase..: " + FraseHavena;
                }
                if (Itens.Count > 0)
                {
                    result += String.Format("\n {0,3} {1,-16} {2, -42} {3, 3} {4, 12} {5,15} {6, 9} {7, 13} {8, 2}", "POS", "CODIGO", "DESCRICAO", "QTD", "VALOR UNITAR", "VR. DESCONTO", "DESCONTO%", "TOTAL DO ITEM", "TP");
                    result += String.Join("",
                    Itens.Select(h =>
                       String.Format("\n {0,3} {1,-16} {2, -42} {3, 3} {4, 12} {5,15} {6, 9} {7, 13} {8, 2}", h.Index + 1, h.CodigoProduto, h.DescricaoProduto, h.Quantidade, h.Valor, h.DescontoValor, h.DescontoPerc, h.TotalItem, h.Tipo)
                    ).ToArray());
                }

                result += "\n";

                if( !String.IsNullOrWhiteSpace(Cliente)) result += $"\nCLIENTE........... : {Cliente}";
                if (!String.IsNullOrWhiteSpace(DataCadastro.ToString("dd/MM/yyyy"))) result += $"\nDATA DE CADASTRO...: {DataCadastro.ToString("dd/MM/yyyy")}";
                if (!String.IsNullOrWhiteSpace(PrevisaoEntrega.ToString("dd/MM/yyyy"))) result += $"\nPREVISAO DE ENTREGA: {PrevisaoEntrega.ToString("dd/MM/yyyy")}";
                if (Sinal > 0)
                {
                    result += $"\nVALOR DO SINAL.....: {Sinal}";
                    result += $"\nDATA DO SINAL......: {DataSinal.ToString("dd/MM/yyyy")}";
                }
            }

            return result;  
        }
    }
}
    