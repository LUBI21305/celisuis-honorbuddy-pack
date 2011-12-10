using System.Collections.Generic;

namespace PalaHealerPVP
{
    partial class UltimatePalaHealer
    {

        private readonly List<uint> ToDispellPVE = new List<uint>
        {
            87619,  //Static Cling?
            87618,
                    //Flame shock lynx boss ZG?
            51783,  //Static Discharge?
            95173,  //Consuming Darkness 1
            88954,  //Consuming Darkness 2
            96958,  //Lash of anguish 1
            96423   //Lash of anguish 2
        };
        private readonly List<uint> ToDispellPVP = new List<uint>
        {
            5782,   //Fear
            118,    //Polymorph
            61305,  //Polymorph Black Cat
            28272,  //Polymorph pig
            61721,  //Polymorph rabbit
            61780,  //Polymorph Turkey O_o
            28271,  //Polymorph turtle
            1499,   //Freezeng trap
            60192,  //Freezeng trap - trap launcher
            24131,  //Wyvern sting
            19386,  //Wyvern sting
            6358,   //Seducton
            605,    //Mind control
            20066,  //Repetance
            51514,  //Hex
            8122,   //Psychic scream
            853,    //Hammer of Justice
            5246,   //Intimidating shout
            5484    //Howl of terror
        };
        private readonly List<uint> NOTToDispellPVE = new List<uint>
        {
            92878,  //Blackout1
            92877,  //Blackout2
            92876,  //Blackout3
            86788,  //Blackout4
            96328,  //Toxic Torment
            96325,  //Frostburn Formula
            96326   //Burning Blood
                    //3° boss Bastion of Twilight
        };
        private readonly List<uint> NOTToDispellPVP = new List<uint>
        {
            30108 //Unstable Affliction
        };
    }
}
