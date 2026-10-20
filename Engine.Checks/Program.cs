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