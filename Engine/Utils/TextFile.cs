using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    public class TextFile
    {
        static readonly string textFile = System.IO.File.ReadAllText(@"C:\Users\user\source\repos\FinanceEngine\TalkEngine\Engine\Assets\Text\FALA.txt");
        public static string[] GetTxt()
        {
            var txt = textFile.Split("\n").ToArray();
            return txt;
        }
    }
}
