using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Engine.Utils.Utils;

namespace Engine.Model
{
    public class TipoData: BaseModel
    {
        public string DataOriginal { get; set; }
        public DateTime Data { get; set; }
		public int DiaDaSemana { get; set; }

		public static List<TipoData> GetTiposData()
        {
            List<TipoData> result = new List<TipoData>();

            result.Add(new TipoData { Nome = "DIA", Id = 0, Tipo = "E" }); // Data especifica
            result.Add(new TipoData { Nome = "DATA", Id = 0, Tipo = "E" }); // Data especifica

            result.Add(new TipoData { Nome = "AGORA", Id = 0, Tipo = "" });
            result.Add(new TipoData { Nome = "ACABEI DE", Id = 0, Tipo = "" });
            result.Add(new TipoData { Nome = "HOJE", Id = 0, Tipo = "" });
            result.Add(new TipoData { Nome = "ANTEONTEM", Id = -2, Tipo = "" });
            result.Add(new TipoData { Nome = " ONTEM", Id = -1, Tipo = "" });
            result.Add(new TipoData { Nome = "AMANHA", Id = 1, Tipo = "" });
            result.Add(new TipoData { Nome = "DEPOIS DE AMANHA", Id = 2, Tipo = "" });

            result.Add(new TipoData { Nome = "DOMINGO", Id = 1, Tipo = "S" });
            result.Add(new TipoData { Nome = "SEGUNDA", Id = 2, Tipo = "S" });
            result.Add(new TipoData { Nome = "TERCA", Id = 3, Tipo = "S" });
            result.Add(new TipoData { Nome = "QUARTA", Id = 4, Tipo = "S" });
            result.Add(new TipoData { Nome = "QUINTA", Id = 5, Tipo = "S" });
            result.Add(new TipoData { Nome = "SEXTA", Id = 6, Tipo = "S" });
            result.Add(new TipoData { Nome = "SABADO", Id = 7, Tipo = "S" });
            result.Add(new TipoData { Nome = "SEMANA PASSADA", Id = 7, Tipo = "S" });

            return result;
        }

        // Identificando a data por extenso
        public static List<TipoData> DataPorExtenso(string frase)
        {
			var DatasFrase = new List<TipoData>();
			var numeros = "0123456789";

			// Filtra só os tipos Datas iguais a "E"
			List<TipoData> tipoData = GetTiposData();
			// .FindAll(tipos => tipos.Tipo == "E");

			//tipoData.ForEach( (tipos =>
			for (int d = 0; d < tipoData.Count; d++)
			{
				if(tipoData[d].Tipo != "E") break;

				var dataNome = tipoData[d].Nome;
				var posicaoDataFrase = frase.IndexOf(dataNome);
				var posicaoInicial = posicaoDataFrase;

				// Se encontrar a palavra data dentro da frase
				while (posicaoDataFrase >= 0)
				{
					while (posicaoDataFrase <= frase.Length)
					{
						var caracter = frase.Substring(posicaoDataFrase, 1);
						if (!numeros.Contains(caracter) || String.IsNullOrWhiteSpace(caracter))
							posicaoDataFrase++;
                        else
                            break;
                    }

					var dia = "";

					while (posicaoDataFrase <= frase.Length)
					{
						var caracter = frase.Substring(posicaoDataFrase, 1);
						if (!numeros.Contains(caracter) || String.IsNullOrWhiteSpace(caracter))
							break;
						dia += caracter;
						posicaoDataFrase++;
					}

					posicaoDataFrase += 4;
					var mes = "";

					while (posicaoDataFrase <= frase.Length)
					{
						var caracter = frase.Substring(posicaoDataFrase, 1);
						if (String.IsNullOrWhiteSpace(caracter))
							break;
						mes += caracter;
						posicaoDataFrase++;
					}

					posicaoDataFrase += 4;
					var ano = "";
					while (posicaoDataFrase <= frase.Length)
					{
						var caracter = frase.Substring(posicaoDataFrase, 1);
						if (!numeros.Contains(caracter) || String.IsNullOrWhiteSpace(caracter))
							break;
						ano += caracter;
						posicaoDataFrase++;
					}

					if (ano.Length == 2)
						ano = Convert.ToInt32(ano) >= 50 ? $"19{ano.Trim()}" : $"20{ano.Trim()}";

					var DiaFormatado = String.Format("{0:00}", Convert.ToInt32(String.IsNullOrWhiteSpace(dia) ? "01" : dia ));
					var MesFormatado = String.Format("{0:00}", GetMesByName(mes));

					if(String.IsNullOrWhiteSpace(mes))
                    {
						mes = "01";
                    }

					if( String.IsNullOrWhiteSpace(ano))
                    {
						ano = DateTime.Now.ToString("yyyy");
                    }

					var novaData = Convert.ToDateTime($"{ano}/{mes}/{dia}");

					var posicaoData = posicaoDataFrase - posicaoInicial;
					DatasFrase.Add(new TipoData{
						Index = posicaoDataFrase,
						Nome = tipoData[d].Nome,
						Tipo = tipoData[d].Tipo,
						Id = tipoData[d].Id,
						Data = novaData,
						DiaDaSemana = Convert.ToInt32(novaData.DayOfWeek)
					});
					frase = Stuff(frase, posicaoInicial, posicaoData, new String('.', posicaoData));

					d--;
					break;

				}
			};
			return DatasFrase;
        }

		// Identificando a data (dia da semana)
		public static Tuple<List<TipoData>, string> DataDiaDaSemana(DateTime data, string frase)
        {
			var DatasFrase = new List<TipoData>();
			// Filtra só os tipos Datas iguais a "E"
			List<TipoData> tipoData = GetTiposData();

			for (int d = 0; d < tipoData.Count; d++)
			{
				if (tipoData[d].Tipo == "E") continue; // vai pularr se o tipo for igual a E
				var dataNome = tipoData[d].Nome;
				var posicaoData= frase.IndexOf(dataNome);
				var novaData = data;
				while (posicaoData >= 0)
				{
					// Calculando nova data	para o dia da semana
					if (tipoData[d].Tipo == "S")
					{
						// Dia da SEMANA da ocorrencia - dia informado
						var diaSemana = Convert.ToInt32(data.DayOfWeek);
						var dias = diaSemana - tipoData[d].Id;
						if (dias == 0) // Se ZERO Sinal que o dia da SEMANA informado e o mesmo dia da semana passada 
							dias = 7;
						else if (dias < 0) // Sinal que o dia da SEMANA informado e maior do que o dia atual subtraio o dia atual
							   dias = 7 - Math.Abs( (DatasFrase.Count >= d ? DatasFrase[d].Id : 0) - diaSemana);
						novaData = data.AddDays(dias * -1) ;
					}
					else
						// Calculando nova data	para o dia que NAO E especifico
						novaData = data.AddDays(tipoData[d].Id);

					DatasFrase.Add(new TipoData
					{
						Index = posicaoData,
						Nome = tipoData[d].Nome,
						Id = tipoData[d].Id,
						Tipo = tipoData[d].Tipo,
						Data = novaData,
						DiaDaSemana = Convert.ToInt32(tipoData[d].Data.DayOfWeek)
					});

					/*frase = frase.Substring(posicaoData + dataNome.Length);
					posicaoData = frase.IndexOf(dataNome);*/
					break;
				}
			}
			// Se nao encontrar nenhuma data subentende que e hoje
			if ( DatasFrase.Count == 0 )
            {
				DatasFrase.Add( new TipoData
                {
					Index = 0,
					Nome = tipoData[2].Nome,
					Id = tipoData[2].Id,
					Tipo = tipoData[2].Tipo,
					Data = data,
					DiaDaSemana = Convert.ToInt32(data.DayOfWeek)
				});
            }

            /*frase = null;*/
            return Tuple.Create(DatasFrase, frase);
        }
		public static List<TipoData> IdentificarDatas(string frase)
        {
			// NÃO TROCAR A SEQUÊNCIA.

			var tipoData = new List<TipoData>();

			var dataDiaSemana = DataDiaDaSemana(DateTime.Now, frase);
			dataDiaSemana.Item1.ForEach( data => { tipoData.Add(data); });

            var dataPorExtenso = DataPorExtenso(dataDiaSemana.Item2);
			dataPorExtenso.ForEach(data => { tipoData.Add(data); });

			return tipoData;
        }

		public static int GetMesByName(string mesNome)
        {
			List<string> meses = new()
            {
				"JANEIRO",
				"FEVEREIRO",
				"MARCO",
				"ABRIL",
				"MAIO",
				"JUNHO",
				"JULHO",
				"AGOSTO",
				"SETEMBRO",
				"OUTUBRO",
				"NOVEMBRO",
				"DEZEMBRO"
			};
			var mesIndex = meses.IndexOf(mesNome) + 1;
			return mesIndex;
        }
		
	}
}
