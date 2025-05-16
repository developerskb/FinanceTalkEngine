using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    public class Utils
    {
		// Remove a quantidade de caracteres definida partindo da posição dada
		public static string Stuff(
			string frase,
			int posicaoInicial,
			int charsToDelete,
			string textoParaSubstituir)
		{
			// Pega do inicio da frase até a posição inicial
			string fraseAlterada = "";
			if (posicaoInicial >= 0)
			{
				if (posicaoInicial >= frase.Length) // se ele tiver o tamanho da frase ou maior 
					fraseAlterada += textoParaSubstituir; // então só acrescenta
				else
				{ // senão tiver o tamanho então "substitui"
					var inicioDaFrase = frase.Substring(0, posicaoInicial);

					if (charsToDelete > frase.Substring(posicaoInicial).Length)
						charsToDelete = frase.Substring(posicaoInicial).Length;

					var fraseASubstituir = frase.Substring(posicaoInicial, charsToDelete);

					// Se a frase for menor q o texto para substituir então substitui até o limite ideal
					if (fraseASubstituir.Length < textoParaSubstituir.Length)
						fraseASubstituir = textoParaSubstituir.Substring(0, fraseASubstituir.Length);
					// Se a frase for maior q o texto para substituir então substitui até apenas uma parte e não tudo
					else if (fraseASubstituir.Length > textoParaSubstituir.Length)
						fraseASubstituir = textoParaSubstituir + fraseASubstituir.Substring(textoParaSubstituir.Length);
					// Se tiverem o mesmo tamanho então substitui uma pela outra
					else
						fraseASubstituir = textoParaSubstituir;
					// Caso seja uma substituição no final da frase 
					var restoDaFrase = frase.Substring(posicaoInicial + charsToDelete);
					fraseAlterada = $"{inicioDaFrase}{fraseASubstituir}{restoDaFrase}"; // e volta o restante da frase
				}
				return fraseAlterada;
			}
			else
				return frase;
		}
	}
}
