using System;
using System.Collections.Generic;
using Engine.Model;
void Check(bool ok, string nome) { if (!ok) throw new Exception(nome); Console.WriteLine("OK: " + nome); }
var c = new List<Cartao> { new Cartao { CartaoId = 12, Nome = "Nubank Croma", BancoNome = "Nubank" } };
Check(Cartao.IdentificarCartoes("gastei 250 no cartão nubank", c)[0].CartaoId == 12, "ID real por banco");
Check(Cartao.IdentificarCartoes("gastei no cartão núbank croma", c)[0].CartaoId == 12, "acentos e nome completo");
c.Add(new Cartao { CartaoId = 13, Nome = "Reserva", BancoNome = "Nubank" });
Check(Cartao.IdentificarCartoes("cartão nubank", c).Count == 0, "banco ambíguo");
Check(Cartao.IdentificarCartoes("cartão nubank croma", c).Count == 1, "nome completo resolve banco");
Check(Cartao.IdentificarCartoes("cartão caixa", new List<Cartao>()).Count == 0, "lista vazia não usa fixos");
Check(Cartao.IdentificarCartoes("CAIXA", null).Count == 1, "chamada antiga");
Check(Cartao.IdentificarCartoes("cartão nubank", new List<Cartao> { new Cartao { CartaoId = 14, Nome = "Nubank Croma" } }).Count == 1, "nome parcial");
var contas = new List<ContaBancaria> { new ContaBancaria { ContaBancariaId = 5, Nome = "Principal", BancoNome = "Itaú" } };
Check(ContaBancaria.IdentificarContas("paguei pelo pix do itau", contas)[0].ContaBancariaId == 5, "conta por banco sem acento");
Check(ContaBancaria.IdentificarContas("usei a principal", contas)[0].ContaBancariaId == 5, "conta pelo nome");
contas.Add(new ContaBancaria { ContaBancariaId = 6, Nome = "Reserva", BancoNome = "Itaú" });
Check(ContaBancaria.IdentificarContas("pix do itau", contas).Count == 0, "contas ambíguas");
Check(ContaBancaria.IdentificarContas("pix da principal", contas).Count == 1, "nome resolve contas");
Check(ContaBancaria.IdentificarContas("pix itau", null).Count == 0, "compatibilidade sem contas");
foreach (var forma in new [] { "pix", "débito", "transferência" }) {
    var lancamento = new Lancamento("gastei 50 reais no " + forma + " do itau", "Ana",
        new List<Cartao> { new Cartao { CartaoId = 12, Nome = "Itaú" } }, contas.GetRange(0, 1));
    lancamento.ProcessarFrase();
    Check(lancamento.Contas.Count == 1 && lancamento.Cartoes.Count == 0, "fluxo completo " + forma);
}
var credito = new Lancamento("gastei 50 reais no cartão itau", "Ana",
    new List<Cartao> { new Cartao { CartaoId = 12, Nome = "Itaú" } }, contas);
credito.ProcessarFrase();
Check(credito.Contas.Count == 0 && credito.Cartoes.Count == 1, "cartão não seleciona conta");
var fraseRobson = new Lancamento("O Robson gastou 150 reais com alimentação usando minha conta Nubank",
    "Leandro Nascimento de Oliveira",
    new List<Cartao> { new Cartao { CartaoId = 39, Nome = "Nubank Croma", BancoNome = "Nubank" } },
    new List<ContaBancaria> { new ContaBancaria { ContaBancariaId = 43, Nome = "Nubank Croma", BancoNome = "Nubank" } },
    new List<Pessoa> { new Pessoa { PessoaId = 17, Nome = "Robson" } });
fraseRobson.ProcessarFrase();
Check(fraseRobson.Valores.Count == 1 && fraseRobson.Valores[0].ValorLancamento == 150m, "frase exata de Robson retorna 150");
Check(fraseRobson.Pessoas.Count == 1 && fraseRobson.Pessoas[0].PessoaId == 17, "Robson identificado pelo cadastro");
Check(fraseRobson.Contas.Count == 1 && fraseRobson.Contas[0].ContaBancariaId == 43 && fraseRobson.Cartoes.Count == 0, "minha conta não seleciona cartão do mesmo banco");
foreach (var verbo in new [] { "gastou", "paguei", "pagou", "gastei" }) {
    var valores = Valor.BuscarValores(Valor.ResolveFraseCentavos("Robson " + verbo + " 12,50 reais"));
    Check(valores.Count == 1 && valores[0].ValorLancamento == 12.50m, "valor decimal com " + verbo);
}
Check(Pessoa.IdentificarPessoas("ROBSON GASTOU", "Leandro", false,
    new List<Pessoa> { new Pessoa { PessoaId = 17, Nome = "Robson da Silva" } })[0].PessoaId == 17, "primeiro nome do cadastro");
Check(Pessoa.IdentificarPessoas("GASTEI 50 REAIS", "Leandro", false, new List<Pessoa>())[0].Nome == "Leandro", "frase pessoal mantém usuário");
Console.WriteLine("Resposta de diagnóstico: " + System.Text.Json.JsonSerializer.Serialize(new {
    Valores = fraseRobson.Valores.Count, PessoaId = fraseRobson.Pessoas[0].PessoaId,
    ContaId = fraseRobson.Contas[0].ContaBancariaId, Cartoes = fraseRobson.Cartoes.Count }));
var categorias = new List<CategoriaCadastro> {
    new CategoriaCadastro { CategoriaId = 18, Nome = "Alimentação", CategoriaTipo = "D" },
    new CategoriaCadastro { CategoriaId = 19, Nome = "Transporte", CategoriaTipo = "D" },
    new CategoriaCadastro { CategoriaId = 20, Nome = "Salário", CategoriaTipo = "R" }
};
var subs = new List<SubCategoriaCadastro> {
    new SubCategoriaCadastro { SubCategoriaId = 71, CategoriaId = 18, Nome = "Restaurante", PalavrasChaves = "almoço; jantar" },
    new SubCategoriaCadastro { SubCategoriaId = 72, CategoriaId = 19, Nome = "Combustível", PalavrasChaves = "gasolina; etanol" },
    new SubCategoriaCadastro { SubCategoriaId = 73, CategoriaId = 20, Nome = "Salário mensal", PalavrasChaves = "pagamento mensal" },
    new SubCategoriaCadastro { SubCategoriaId = 99, CategoriaId = 999, Nome = "Inacessível", PalavrasChaves = "secreto" }
};
var clas = Classificacao.Identificar("O Robson gastou 150 reais com alimentação", categorias, subs);
Check(clas.CategoriaId == 18 && clas.SubCategoriaId == null, "categoria cadastrada sem inventar subcategoria");
clas = Classificacao.Identificar("gastei 50 no almoço", categorias, subs);
Check(clas.CategoriaId == 18 && clas.SubCategoriaId == 71, "palavra-chave identifica subcategoria e pai");
clas = Classificacao.Identificar("paguei 50 com gasolina", categorias, subs);
Check(clas.CategoriaId == 19 && clas.SubCategoriaId == 72, "outra categoria por palavras-chave");
clas = Classificacao.Identificar("recebi 1500 de salário mensal", categorias, subs);
Check(clas.CategoriaId == 20 && clas.SubCategoriaId == 73 && clas.Tipo == "R", "receita classificada");
Check(Classificacao.Identificar("gastei no secreto", categorias, subs).CategoriaId == null, "subcategoria fora do catálogo ignorada");
Check(Classificacao.Identificar("gastei com alimentação e transporte", categorias, subs).CategoriaId == null, "categorias ambíguas");
var duplicadas = new List<SubCategoriaCadastro>(subs) {
    new SubCategoriaCadastro { SubCategoriaId = 74, CategoriaId = 18, Nome = "Delivery", PalavrasChaves = "almoço" }
};
clas = Classificacao.Identificar("gastei no almoço", categorias, duplicadas);
Check(clas.CategoriaId == 18 && clas.SubCategoriaId == null, "mantém pai mas não escolhe subcategoria ambígua");
Check(Classificacao.Identificar("gastei com alimentação", new List<CategoriaCadastro>(), subs).CategoriaId == null, "lista vazia não usa catálogo fixo");
Check(Pessoa.IdentificarPessoas("ROBSON GASTOU 50", "Leandro", false, new List<Pessoa> {
    new Pessoa { PessoaId = 1, Nome = "Robson Silva" }, new Pessoa { PessoaId = 2, Nome = "Robson Souza" }
})[0].IdentificacaoPendente, "pessoa ambígua exige revisão");
foreach (var frase in new[] { "recebi 1.500 reais", "ganhei R$ 1.500", "Robson pagou 1500 reais", "fiz uma compra de 1500 reais" }) {
    var valores = Valor.BuscarValores(Valor.ResolveFraseCentavos(frase));
    Check(valores.Count == 1 && valores[0].ValorLancamento == 1500m, "valor financeiro: " + frase);
}
var decimais = Valor.BuscarValores("R$ 1.500,50 E R$ 0,50");
Check(decimais.Count == 2 && decimais[0].ValorLancamento == 1500.50m && decimais[1].ValorLancamento == .5m, "milhar e centavos");
Check(Valor.BuscarValores("DIA 20/10/2026 EM 3 PARCELAS").Count == 0, "não interpreta data e parcela como valor");
Check(Valor.BuscarValores("GASTEI 100 REAIS COM DESCONTO DE R$ 10").Count == 1, "desconto não vira outro gasto");
var completo = new Lancamento("Robson gastou 150 reais com alimentação usando minha conta Nubank", "Leandro",
    new List<Cartao>(), new List<ContaBancaria> { new ContaBancaria { ContaBancariaId = 43, Nome = "Principal", BancoNome = "Nubank" } },
    new List<Pessoa> { new Pessoa { PessoaId = 17, Nome = "Robson" } }, categorias, subs);
completo.ProcessarFrase();
Check(completo.Valores.Count == 1 && completo.Valores[0].Classificacao.CategoriaId == 18 &&
    completo.Pessoas[0].PessoaId == 17 && completo.Contas[0].ContaBancariaId == 43, "integração completa com cadastros");
var receitaCompleta = new Lancamento("Recebi 1.500 reais de salário mensal na minha conta Nubank", "Leandro",
    new List<Cartao>(), new List<ContaBancaria> { new ContaBancaria { ContaBancariaId = 43, Nome = "Principal", BancoNome = "Nubank" } },
    new List<Pessoa>(), categorias, subs);
receitaCompleta.ProcessarFrase();
Check(receitaCompleta.Valores.Count == 1 && receitaCompleta.Valores[0].ValorLancamento == 1500m &&
    receitaCompleta.Valores[0].Classificacao.Tipo == "R" && receitaCompleta.Valores[0].Classificacao.SubCategoriaId == 73,
    "fluxo de receita com salário e conta");
Check(Classificacao.Identificar("gastei com alimentação", new List<CategoriaCadastro> {
    new CategoriaCadastro { CategoriaId = 1, Nome = "Alimentação" },
    new CategoriaCadastro { CategoriaId = 2, Nome = "Alimentação" }
}, subs).CategoriaId == null, "nomes duplicados não escolhem ID arbitrário");
Check(Valor.BuscarValores("GASTEI 9999999999999999999999999999999999999999 REAIS").Count == 0, "valor fora do limite não derruba motor");
Check(Classificacao.Identificar("paguei 1500 reais de salário", categorias, subs).Tipo == "D",
    "pagar salário não vira receita por causa do substantivo");