using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogMeOut.Forms
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            //Rempli la liste déroulante des type de points
            foreach (string strNomPoint in Enum.Arrays.NamesPoints)
            {
                this.cmbPoints.Items.Add(strNomPoint);
            }
            //Importe les données de la fenêtre
            loadSettings();
            //Vérifie si une nouvelle mise à jour est disponible
            if (Classes.Updater.isAvailable)
            {
                this.labUpdateCurrentVersion.Text = Classes.Updater.Version.ToString();
                this.labUpdateNewVersion.Text = Classes.Updater.getLatestVersion();
                this.tabControl.SelectedTab = tabUpdate;
            }
            else
            {
                //Supprime l'onglet Update
                this.tabControl.TabPages.Remove(tabUpdate);
            }
            //Applique le focus sur le bouton Cancel
            this.btnCancel.Focus();
        }

        /// <summary>
        /// Charge toutes les variables de la fenêtre depuis un fichier XML.
        /// </summary>
        private void loadSettings()
        {
            //Crée un pointeur sur l'instance des données sauvegardées
            LogMeOutSettings settings = LogMeOutSettings.Instance;
            //Importe les données
            settings.Load();
            //Rempli les champs du tab Triggers
            this.chkBagsFull.Checked = settings.alertOnBagsFull;
            this.chkTimeElapsed.Checked = settings.alertOnTimeElapsed;
            this.numHoursElapsed.Value = settings.hoursElapsed;
            this.numMinutesElapsed.Value = settings.minutesElapsed;
            this.chkDeaths.Checked = settings.alertOnDeaths;
            this.numDeaths.Value = settings.nbDeaths;
            this.chkStucks.Checked = settings.alertOnStucks;
            this.numStucks.Value = settings.nbStucks;
            this.chkMinutesInCombat.Checked = settings.alertOnMinutesInCombat;
            this.numMinutesInCombat.Value = settings.nbMinutesInCombat;
            this.chkMobsKilled.Checked = settings.alertOnMobsKilled;
            this.numMobsKilled.Value = settings.nbMobsKilled;
            this.chkWhispesReceived.Checked = settings.alertOnWhispesReceived;
            this.numWhispesReceived.Value = settings.nbWhispesReceived;
            this.chkPoints.Checked = settings.alertOnPoints;
            this.numPoints.Value = settings.nbPoints;
            this.cmbPoints.SelectedIndex = settings.typePoints;
            this.chkLevelReached.Checked = settings.alertOnLevelReached;
            this.numLevelReached.Value = settings.nbLevel;
            this.chkPlayerFollows.Checked = settings.alertOnPlayerFollows;
            this.numPlayerFollows.Value = settings.minutesPlayerFollows;
            this.chkPlayerTargets.Checked = settings.alertOnPlayerTargets;
            this.numPlayerTargets.Value = settings.minutesPlayerTargets;
            this.chkBeepWhenFire.Checked = settings.beepWhenFire;

            //Rempli les champs du tab Action Before
            switch (settings.ActionBefore)
            {
                case 0:
                    this.radBeforeNothing.Checked = true;
                    break;
                case 1:
                    this.radBeforeHearthstone.Checked = true;
                    break;
                case 2:
                    this.radBeforeSpell.Checked = true;
                    break;
                case 3:
                    this.radBeforeItem.Checked = true;
                    break;
            }
            this.txtSpellName.Text = settings.spellName;
            this.numItemID.Value = settings.itemID;

            //Rempli les champs du tab Action After
            switch (settings.ActionAfter)
            {
                case 0 :
                    this.radAfterNothing.Checked = true;
                    break;
                case 1:
                    this.radAfterShutdown.Checked = true;
                    break;
                case 2:
                    this.radAfterBatchLigne.Checked = true;
                    break;
            }
            this.txtAfterBatchCommand.Text = settings.BatchCommand;
            this.txtAfterBatchArgument.Text = settings.BatchArgument;

            //Rempli les champs du tab Logging
            this.cmbColorLogs.SelectedItem = settings.ColorLogs;
            this.chkLoggingTime.Checked = settings.LoggingTime;
            this.numLoggingTime.Value = settings.LoggingTimeEvery;

            //Rempli les champd du tab Exceptions
            this.chkExceptionBG.Checked = settings.exceptionBG;
            this.chkExceptionInstance.Checked = settings.exceptionInstance;
            this.chkExceptionCountDeathsBG.Checked = settings.exceptionCountDeathsBG;
        }

        /// <summary>
        /// Sauvegarde toutes les variables de la fenêtre dans un fichier XML.
        /// </summary>
        private void saveSettings()
        {
            //Crée un pointeur sur l'instance des données sauvegardées
            LogMeOutSettings settings = LogMeOutSettings.Instance;
            //Sauvegarde les champs du tab Triggers
            settings.alertOnBagsFull = this.chkBagsFull.Checked;
            settings.alertOnTimeElapsed = this.chkTimeElapsed.Checked;
            settings.hoursElapsed = (int)this.numHoursElapsed.Value;
            settings.minutesElapsed = (int)this.numMinutesElapsed.Value;
            settings.alertOnDeaths = this.chkDeaths.Checked;
            settings.nbDeaths = (int)this.numDeaths.Value;
            settings.alertOnStucks = this.chkStucks.Checked;
            settings.nbStucks = (int)this.numStucks.Value;
            settings.alertOnMinutesInCombat = this.chkMinutesInCombat.Checked;
            settings.nbMinutesInCombat = (int)this.numMinutesInCombat.Value;
            settings.alertOnMobsKilled = this.chkMobsKilled.Checked;
            settings.nbMobsKilled = (int)this.numMobsKilled.Value;
            settings.alertOnWhispesReceived = this.chkWhispesReceived.Checked;
            settings.nbWhispesReceived = (int)this.numWhispesReceived.Value;
            settings.alertOnPoints = this.chkPoints.Checked;
            settings.nbPoints = (int)this.numPoints.Value;
            settings.typePoints = this.cmbPoints.SelectedIndex;
            settings.alertOnLevelReached = this.chkLevelReached.Checked;
            settings.nbLevel = (int)this.numLevelReached.Value;
            settings.alertOnPlayerFollows = this.chkPlayerFollows.Checked;
            settings.minutesPlayerFollows = (int)this.numPlayerFollows.Value;
            settings.alertOnPlayerTargets = this.chkPlayerTargets.Checked;
            settings.minutesPlayerTargets = (int)this.numPlayerTargets.Value;
            settings.beepWhenFire = this.chkBeepWhenFire.Checked;

            //Sauvegarde les champs du tab Action Before
            if(this.radBeforeNothing.Checked)
                settings.ActionBefore = 0;
            else if(this.radBeforeHearthstone.Checked)
                settings.ActionBefore = 1;
            else if(this.radBeforeSpell.Checked)
                settings.ActionBefore = 2;
            else if (this.radBeforeItem.Checked)
                settings.ActionBefore = 3;
            settings.spellName = this.txtSpellName.Text;
            settings.itemID = (int)this.numItemID.Value;

            //Sauvegarde les champs Action After
            if (this.radAfterNothing.Checked)
                settings.ActionAfter = 0;
            else if (this.radAfterShutdown.Checked)
                settings.ActionAfter = 1;
            else if (this.radAfterBatchLigne.Checked)
                settings.ActionAfter = 2;
            settings.BatchCommand = this.txtAfterBatchCommand.Text;
            settings.BatchArgument = this.txtAfterBatchArgument.Text;

            //Sauvegarde les champs du tab Logging
            settings.ColorLogs = this.cmbColorLogs.SelectedItem.ToString();
            settings.LoggingTime = this.chkLoggingTime.Checked;
            settings.LoggingTimeEvery = (int)this.numLoggingTime.Value;

            //Sauvegarde les champs du tab Exceptions
            settings.exceptionBG = this.chkExceptionBG.Checked;
            settings.exceptionInstance = this.chkExceptionInstance.Checked;
            settings.exceptionCountDeathsBG = this.chkExceptionCountDeathsBG.Checked;

            //Sauvegarde les données
            settings.Save();
        }

        private void chkHoursElapsed_CheckedChanged(object sender, EventArgs e)
        {
            this.numHoursElapsed.Enabled = this.chkTimeElapsed.Checked;
            this.numMinutesElapsed.Enabled = this.chkTimeElapsed.Checked;
        }

        private void chkDeaths_CheckedChanged(object sender, EventArgs e)
        {
            this.numDeaths.Enabled = this.chkDeaths.Checked;
        }

        private void chkStucks_CheckedChanged(object sender, EventArgs e)
        {
            this.numStucks.Enabled = this.chkStucks.Checked;
        }

        private void chkMinutesInCombat_CheckedChanged(object sender, EventArgs e)
        {
            this.numMinutesInCombat.Enabled = this.chkMinutesInCombat.Checked;
        }

        private void chkMobsKilled_CheckedChanged(object sender, EventArgs e)
        {
            this.numMobsKilled.Enabled = this.chkMobsKilled.Checked;
        }

        private void chkPoints_CheckedChanged(object sender, EventArgs e)
        {
            this.numPoints.Enabled = this.chkPoints.Checked;
            this.cmbPoints.Enabled = this.chkPoints.Checked;
        }

        private void chkLevelReached_CheckedChanged(object sender, EventArgs e)
        {
            this.numLevelReached.Enabled = this.chkLevelReached.Checked;
        }

        private void radSpell_CheckedChanged(object sender, EventArgs e)
        {
            this.txtSpellName.Enabled = this.radBeforeSpell.Checked;
        }

        private void radItem_CheckedChanged(object sender, EventArgs e)
        {
            this.numItemID.Enabled = this.radBeforeItem.Checked;
        }

        private void chkLoggingHours_CheckedChanged(object sender, EventArgs e)
        {
            this.numLoggingTime.Enabled = this.chkLoggingTime.Checked;
        }

        private void radAfterBatchLigne_CheckedChanged(object sender, EventArgs e)
        {
            this.txtAfterBatchCommand.Enabled = this.radAfterBatchLigne.Checked;
            this.txtAfterBatchArgument.Enabled = this.radAfterBatchLigne.Checked;
        }

        private void chkWhispesReceived_CheckedChanged(object sender, EventArgs e)
        {
            this.numWhispesReceived.Enabled = this.chkWhispesReceived.Checked;
        }

        private void chkPlayerFollows_CheckedChanged(object sender, EventArgs e)
        {
            this.numPlayerFollows.Enabled = this.chkPlayerFollows.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.numPlayerTargets.Enabled = this.chkPlayerTargets.Checked;
        }

        private void txtSpellName_Enter(object sender, EventArgs e)
        {
            //Si le contrôle contient le texte d'informations, on le vide
            if (this.txtSpellName.ForeColor != System.Drawing.Color.Black)
            {
                this.txtSpellName.Text = "";
                this.txtSpellName.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtSpellName_TextChanged(object sender, EventArgs e)
        {
            //Si le champ est vide, on remet le texte d'informations
            if (this.txtSpellName.Text == "Enter the spell name (in english) here")
                this.txtSpellName.ForeColor = System.Drawing.Color.Gray;
        }

        private void txtSpellName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSpellName.Text))
                this.txtSpellName.Text = "Enter the spell name (in english) here";
        }

        private void btnShowTimer_Click(object sender, EventArgs e)
        {
            //Création de la fenêtre timer
            Forms.frmTimer frmTimerWindow = new frmTimer();
            frmTimerWindow.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Ferme la fenêtre
            this.Close();
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            //Sauvegarde les données de la fenêtre
            saveSettings();
            //Ferme la fenêtre
            this.Close();
        }

        private void btnUpdateWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.Updater.strWebSite);
        }

        private void btnUpdateUpdate_Click(object sender, EventArgs e)
        {
            //Désactive le bouton durant la mise à jour
            this.btnUpdateUpdate.Enabled = false;
            this.btnUpdateUpdate.Text = "Updating...";
            //Rafraichit la fenêtre
            Application.DoEvents();
            //Effectue la mise à jour
            if (Classes.Updater.installLatestVersion())
                this.btnUpdateUpdate.Text = "Updated !";
            else
            {
                this.btnUpdateUpdate.Enabled = true;
                this.btnUpdateUpdate.Text = "Update !";
            }
        }
    }
}
