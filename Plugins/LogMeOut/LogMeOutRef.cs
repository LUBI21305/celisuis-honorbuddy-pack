using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogMeOut.Enum
{
    public enum HearthItems
    {
        PierreDeFoyer = 6948,
        PortailEtherien = 54452,
        Archeologie = 64488
    }

    public enum IdPoints : int
    {
        Conquete = 390,
        Honneur = 392,
        Justice = 395,
        Vaillance = 396
    }

    public class Arrays
    {
        //Contient les noms des points
        public static string[] NamesPoints = {"Justice", "Valor", "Honor", "Conquest"};
    }
}
