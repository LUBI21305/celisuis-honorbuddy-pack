using System.IO;
using Styx;
using Styx.Helpers;

namespace LogMeOut
{
    public class LogMeOutSettings : Settings
    {
        public static readonly LogMeOutSettings Instance = new LogMeOutSettings();

        public LogMeOutSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"Settings/LogMeOut/LogMeOut-Settings-{0}.xml", StyxWoW.Me.Name))) // Plugins/LogMeOut/Config/LogMeOut-Settings-{0}.xml
        {
        }

        #region Variables du tab Triggers
        //Bags full
        [Setting, DefaultValue(false)]
        public bool alertOnBagsFull { get; set; }

        //Time elapsed
        [Setting, DefaultValue(false)]
        public bool alertOnTimeElapsed { get; set; }
        [Setting, DefaultValue(1)]
        public int hoursElapsed { get; set; }
        [Setting, DefaultValue(0)]
        public int minutesElapsed { get; set; }

        //Deaths
        [Setting, DefaultValue(false)]
        public bool alertOnDeaths { get; set; }
        [Setting, DefaultValue(10)]
        public int nbDeaths { get; set; }

        //Stucks
        [Setting, DefaultValue(false)]
        public bool alertOnStucks { get; set; }
        [Setting, DefaultValue(10)]
        public int nbStucks { get; set; }

        //Rest in combat
        [Setting, DefaultValue(false)]
        public bool alertOnMinutesInCombat { get; set; }
        [Setting, DefaultValue(5)]
        public int nbMinutesInCombat { get; set; }

        //Mobs killed
        [Setting, DefaultValue(false)]
        public bool alertOnMobsKilled { get; set; }
        [Setting, DefaultValue(200)]
        public int nbMobsKilled { get; set; }

        //Whispes Received
        [Setting, DefaultValue(false)]
        public bool alertOnWhispesReceived { get; set; }
        [Setting, DefaultValue(3)]
        public int nbWhispesReceived { get; set; }

        //Points
        [Setting, DefaultValue(false)]
        public bool alertOnPoints { get; set; }
        [Setting, DefaultValue(4000)]
        public int nbPoints { get; set; }
        [Setting, DefaultValue(0)]
        public int typePoints { get; set; }

        //Level Reached
        [Setting, DefaultValue(false)]
        public bool alertOnLevelReached { get; set; }
        [Setting, DefaultValue(85)]
        public int nbLevel { get; set; }

        //Player follows
        [Setting, DefaultValue(false)]
        public bool alertOnPlayerFollows { get; set; }
        [Setting, DefaultValue(10)]
        public int minutesPlayerFollows { get; set; }

        //Player targets
        [Setting, DefaultValue(false)]
        public bool alertOnPlayerTargets { get; set; }
        [Setting, DefaultValue(5)]
        public int minutesPlayerTargets { get; set; }

        //Beep when fire
        [Setting, DefaultValue(false)]
        public bool beepWhenFire { get; set; }
        #endregion
        #region Variables du tab Action Before
        //Action before
        [Setting, DefaultValue(0)]
        public int ActionBefore { get; set; }

        //Spell Name
        [Setting, DefaultValue("Enter the spell name (in english) here")]
        public string spellName { get; set; }

        //Item ID
        [Setting, DefaultValue(1)]
        public int itemID { get; set; }
        #endregion
        #region Variables du tab Action After
        //Action after
        [Setting, DefaultValue(0)]
        public int ActionAfter { get; set; }

        //Batch Command
        [Setting, DefaultValue("")]
        public string BatchCommand { get; set; }

        //Batch Ligne
        [Setting, DefaultValue("")]
        public string BatchArgument { get; set; }
        #endregion
        #region Variables du tab Logging
        //Couleur du texte de log
        [Setting, DefaultValue("Orange")]
        public string ColorLogs { get; set; }

        //Ecrit le temps restant
        [Setting, DefaultValue(true)]
        public bool LoggingTime { get; set; }

        //Every
        [Setting, DefaultValue(15)]
        public int LoggingTimeEvery { get; set; }
        #endregion
        #region Variables du tab Exceptions
        //Exception sur les BGs
        [Setting, DefaultValue(true)]
        public bool exceptionBG { get; set; }
        //Exception sur les instances
        [Setting, DefaultValue(true)]
        public bool exceptionInstance { get; set; }

        //Exception sur le compteur de mort en champs de bataille
        [Setting, DefaultValue(false)]
        public bool exceptionCountDeathsBG { get; set; }
        #endregion
        #region Autres variables
        //Contient la raison de la dernière déconnexion du bot
        [Setting, DefaultValue("")]
        public string reasonLastLogOut { get; set; }
        #endregion
    }
}