using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Engine.Utils.Utils;

namespace Engine.Model
{
    public class HavenaProdutos: BaseModel
    {
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        private static List<HavenaProdutos> GetProdutos()
        {
            List<HavenaProdutos> havenaProdutos = new List<HavenaProdutos>();
            havenaProdutos.Add(new HavenaProdutos { Nome = "PRODUTO " });
            havenaProdutos.Add(new HavenaProdutos { Nome = "CODIGO " });
            return havenaProdutos;
        }
        public static List<HavenaProdutos> IdentificarProdutos(string frase)
        {
            var produtos = GetProdutos();

            List<HavenaProdutos> havenaProdutos = new List<HavenaProdutos>();

            produtos.ForEach(produto =>
            {
                var produtosFrase = frase.Split(produto.Nome);
                for ( int k = 0; k < produtosFrase.Length - 1; k ++ )
                {
                    var posicao = frase.IndexOf(produto.Nome); // Tenta encontra a palavra na frase
                    var posicaoInicial = posicao;
                    var posicaoFinal = 0;
                    var caracter = "";

                    var codigoProduto = "";
                    var descricaoProduto = "";

                    if( posicao >= 0 ) // Encontrou
                    {
                        posicaoFinal = posicao += produto.Nome.Length;
                    }

                    while( posicao > 0 && posicao < frase.Length )
                    {
                        posicaoFinal = posicao;
                        caracter = frase.Substring(posicao, 1); // pega o caracter

                        if ("012345678".Contains(caracter))
                        {
                            // Atualiza as duas variaveis com o novo caracter
                            codigoProduto = descricaoProduto += caracter;
                        }
                        else
                        {
                            descricaoProduto += caracter;
                            // indica que existe um código e então encerra o código

                            if (codigoProduto.Length >= 4) break;

                            var encontrouPChave = false;

                        
                            string[] palavrasChave = { "R$", "VALOR", "QUANTIDADE", "DESCONTO" };
                            // Se o primeiro caracter começar com o caracter encontrado então retorna true
                            var palavrasChaveIniciaCaracter = palavrasChave.FirstOrDefault( p => p.First().ToString() == caracter );
                            var encontrouPalavraChave = frase.Substring(posicao);

                            if (!encontrouPChave && palavrasChaveIniciaCaracter != null)
                            {
                                if ((posicao + 2) <= frase.Length && frase.Substring(posicao, 2) == palavrasChave[0]) // R$
                                    encontrouPChave = true;

                                if ((posicao + 5) <= frase.Length && frase.Substring(posicao, 5) == palavrasChave[1]) // VALOR
                                    encontrouPChave = true;

                                if ((posicao + 8) <= frase.Length && frase.Substring(posicao, 8) == palavrasChave[2]) // QUANTIDADE
                                    encontrouPChave = true;

                                if ((posicao + 10) <= frase.Length && frase.Substring(posicao, 10) == palavrasChave[3]) // DESCONTO
                                    encontrouPChave = true;
                            }

                            if (encontrouPChave) break;
                        }

                        posicao++;
                    }

                    if(!String.IsNullOrWhiteSpace(descricaoProduto))
                    {
                        frase = Stuff(frase, posicaoInicial, posicaoFinal - posicaoInicial + 1, $"{produto.Nome.Replace("O", "0")}{descricaoProduto}");

                        descricaoProduto = descricaoProduto.Trim().Substring(0, descricaoProduto.Length - 1);
                        // Pega os ultimos 3 caracteres da direita
                        if (descricaoProduto.Substring(descricaoProduto.Length - 3, 3) == " DE")
                        {
                            descricaoProduto = descricaoProduto.Substring(descricaoProduto.Length - 3);
                        }
                        if(codigoProduto.Length < 4)
                        {
                            codigoProduto = "";
                        }

                        havenaProdutos.Add(new HavenaProdutos
                        {
                            Index = posicao,
                            CodigoProduto = codigoProduto.PadRight(15, ' '),
                            DescricaoProduto = descricaoProduto.PadRight(40, ' ')
                        });
                    }
                }

            });
            return havenaProdutos;
        }
    }
}
