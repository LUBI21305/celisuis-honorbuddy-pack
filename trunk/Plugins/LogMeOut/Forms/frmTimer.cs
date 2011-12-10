using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins.PluginClass;
using Styx;

namespace LogMeOut.Forms
{
    public partial class frmTimer : Form
    {
        /// <summary>
        /// Détermine si le timer est en pause.
        /// </summary>
        private bool blnIsInPause = false;
        /// <summary>
        /// Conserve le moment de la dernière mise à jour de l'heure du démarrage du bot.
        /// </summary>
        private DateTime dtLastUpdatePause;

        public frmTimer()
        {
            //Crée les contrôles
            InitializeComponent();
            //Bind l'événement lors de l'appui sur le bouton Start et Stop du bot
            BotEvents.OnBotStarted += new BotEvents.OnBotStartDelegate(BotEvents_OnBotStarted);
            BotEvents.OnBotStopped += new BotEvents.OnBotStopDelegate(BotEvents_OnBotStopped);
            //Vérifie que le plugin soit activé
            if (LogMeOut.Instance != null)
                this.timerRafraichissement.Enabled = true;
        }

        void BotEvents_OnBotStarted(EventArgs args)
        {
            //Démarre le timer
            this.timerRafraichissement.Start();
            //Active le bouton d'état
            this.btnChangeState.Enabled = true;
        }

        void BotEvents_OnBotStopped(EventArgs args)
        {
            //Arrête le timer
            this.timerRafraichissement.Stop();
            //Désactive le bouton d'état
            this.btnChangeState.Enabled = false;
            this.btnChangeState.Text = "Pause";
            blnIsInPause = false;
        }

        private void timerRafraichissement_Tick(object sender, EventArgs e)
        {
            //Si le timer est actif
            if (!blnIsInPause)
            {
                //Rafraichît le timer si le trigger est actif
                if (LogMeOutSettings.Instance.alertOnTimeElapsed && Styx.Logic.BehaviorTree.TreeRoot.IsRunning && !LogMeOut.Instance.IsLoggingOut)
                {
                    //Détermine le temps restant
                    TimeSpan tsRemaining = (LogMeOut.Instance.StartingBot.AddHours(LogMeOutSettings.Instance.hoursElapsed).AddMinutes(LogMeOutSettings.Instance.minutesElapsed) - DateTime.Now);
                    this.labTimer.Text = tsRemaining.Hours.ToString("00") + ":" + tsRemaining.Minutes.ToString("00") + ":" + tsRemaining.Seconds.ToString("00");
                    //Active le bouton pour mettre en pause/réactiver le timer
                    this.btnChangeState.Enabled = true;
                }
                else if (LogMeOut.Instance.IsLoggingOut)
                {
                    this.labTimer.Text = "Now";
                    //Désactive le bouton pour mettre en pause/réactiver le timer
                    this.btnChangeState.Enabled = false;
                }
                else
                {
                    this.labTimer.Text = "00:00:00";
                    //Désactive le bouton pour mettre en pause/réactiver le timer
                    this.btnChangeState.Enabled = false;
                }
            }
            else
            {
                //Si le timer est en pause
                //Rajoute le temps écoulé depuis le dernier cycle de ce timer à l'heure de début du bot
                LogMeOut.Instance.StartingBot = LogMeOut.Instance.StartingBot.AddMilliseconds((DateTime.Now - dtLastUpdatePause).TotalMilliseconds);
                //Met à jour l'heure du cycle
                dtLastUpdatePause = DateTime.Now;
            }
        }

        private void btnChangeState_Click(object sender, EventArgs e)
        {
            if (!blnIsInPause)
            {
                //Stop le timer
                this.btnChangeState.Text = "Resume";
                blnIsInPause = true;
                //Enregistre le moment où le bot a été arreté
                dtLastUpdatePause = DateTime.Now;
                //Ecriture dans la console Log
                //Détermine le temps restant
                TimeSpan tsRemaining = (LogMeOut.Instance.StartingBot.AddHours(LogMeOutSettings.Instance.hoursElapsed).AddMinutes(LogMeOutSettings.Instance.minutesElapsed) - DateTime.Now);
                LogMeOut.wLog("Timer paused on " + tsRemaining.Hours.ToString("00") + ":" + tsRemaining.Minutes.ToString("00") + ":" + tsRemaining.Seconds.ToString("00"));
            }
            else
            {
                //Démarre le timer
                this.btnChangeState.Text = "Pause";
                blnIsInPause = false;
                //Ecriture dans la console Log
                LogMeOut.wLog("Timer resumed");
            }
        }
    }
}
