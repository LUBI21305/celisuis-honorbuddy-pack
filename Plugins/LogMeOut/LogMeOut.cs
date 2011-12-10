/*----------------------------------------------------
 * LogMeOut! - Developped by ZenLulz (on thebuddyforum.com)
 * WoW Version Supported : 4.2.2.14545
 * SVN : https://zenlulzdev.googlecode.com/svn/trunk/HonorBuddy/Plugins/LogMeOut/
 * Note : This is a free plugin, and could not be sold, or repackaged.
 * Version : 1.1.0 (Release)
 ----------------------------------------------------*/

using Styx.Logic;
using System;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using System.Threading;
using System.Diagnostics;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Net;
using Styx.Plugins.PluginClass;
using Styx;
using System.Drawing;
using System.Windows.Forms;

namespace LogMeOut
{
    public class LogMeOut : HBPlugin
    {
        //Normal Stuff.
        public override string Name { get { return "LogMeOut!"; } }
        public override string Author { get { return "ZenLulz"; } }
        public override Version Version { get { return Classes.Updater.Version; } }
        public override bool WantButton { get { return true; } }
        public override string ButtonText { get { return "Settings"; } }

        /// <summary>
        /// Représente l'instance de cette classe.
        /// </summary>
        public static LogMeOut Instance;

        /// <summary>
        /// Objet pointant sur le les propriétés du bot.
        /// </summary>
        private static readonly LocalPlayer Me = ObjectManager.Me;
        /// <summary>
        /// Crée une variable qui définit si le module a déjà été chargé.
        /// </summary>
        private bool blnInit = false;
        /// <summary>
        /// Indique si le personnage a déjà été dans un champ de bataille ou instance dans la phase de déconnexion.
        /// </summary>
        private bool blnBattlegroundOrInstance = false;
        /// <summary>
        /// Indique si le personnage a déjà été mort dans la phase de déconnexion.
        /// </summary>
        private bool blnIsDead = false;
        /// <summary>
        /// Indique si le personnage a déjà été en combat dans la phase de déconnexion.
        /// </summary>
        private bool blnCombat = false;
        /// <summary>
        /// Indique si le personnage a déjà été en monture volante ou sur un trajet aérien dans la phase de déconnexion.
        /// </summary>
        private bool blnFlying = false;
        /// <summary>
        /// Indique si l'objet ou le sort paramétré dans l'Action Before a été utilisé dans la phase de déconnexion.
        /// </summary>
        private bool blnSpellOrItemUsed = false;
        /// <summary>
        /// Indique si le message du force logout a déjà été affiché.
        /// </summary>
        private bool blnForceLogOutMessageShown = false;
        /// <summary>
        /// Indique si une alerte est effectuée à cause d'une action avant déconnexion dans la phase de déconnexion.
        /// </summary>
        private bool blnAlertActionBefore = false;
        /// <summary>
        /// Crée une variable qui définit si la procédure de déconnexion a déjà été engagée.
        /// </summary>
        private bool blnStartingLogOutProcess = false;
        /// <summary>
        /// Indique depuis combien de temps la procédure de déconnexion a été entamée.
        /// </summary>
        private DateTime dtStartingLogOutProcess = DateTime.MinValue;
        /// <summary>
        /// Indique si le processus de déconnexion est arrivé à terme.
        /// </summary>
        private bool blnDisconnected = false;
        /// <summary>
        /// Indique lorsque le timer est lancé.
        /// </summary>
        private DateTime dtStartingTimer;
        /// <summary>
        /// Indique lorsque le temps écoulé a été indiqué pour la dernière fois dans la console Log.
        /// </summary>
        private DateTime dtLastLoggingTime;
        /// <summary>
        /// Indique la dernière fois que le bot est resté stuck.
        /// </summary>
        private DateTime dtLastStuck;
        /// <summary>
        /// Indique le nombre de stucks consécutifs.
        /// </summary>
        private int intNbStucks = 0;
        /// <summary>
        /// Indique le nombre de whispes reçus.
        /// </summary>
        private int intNbWhispes = 0;
        /// <summary>
        /// Indique le nombre de fois que le bot est mort.
        /// </summary>
        private int intNbDeaths = 0;
        /// <summary>
        /// Indique si le joueur est vivant.
        /// </summary>
        private bool blnIsAlive = true;
        /// <summary>
        /// Indique la dernière fois que le bot était en chute libre.
        /// </summary>
        private DateTime dtLastIsFalling = new DateTime();
        /// <summary>
        /// Indique depuis combien de temps le bot est en combat.
        /// </summary>
        private DateTime dtInCombat;
        /// <summary>
        /// Contient le nom des unités et joueurs ciblant le bot.
        /// </summary>
        private String strTargetingUnitsAndPlayersNames;
        /// <summary>
        /// Contient les joueurs/unités qui entoure le joueur avec le temps lorsque celui-ci a été ajouté
        /// </summary>
        private Classes.PeopleAroundMe objPeopleAroundMe;

        /// <summary>
        /// Méthode écrivant du texte dans le console Log.
        /// </summary>
        public static void wLog(string format, params object[] args)
        {
            Logging.Write(Color.FromName(LogMeOutSettings.Instance.ColorLogs), "[LogMeOut!]: " + format, args);
        }

        /// <summary>
        /// Méthode écrivant du texte dans le console Log en tant qu'erreur.
        /// </summary>
        public static void wLogError(string format, params object[] args)
        {
            Logging.Write(Color.Red, "[LogMeOut!]: " + format, args);
        }

        /// <summary>
        /// Méthode écrivant du texte dans le console Debug.
        /// </summary>
        public static void wLogDebug(string format, params object[] args)
        {
            Logging.WriteDebug(Color.FromName(LogMeOutSettings.Instance.ColorLogs), "[LogMeOut!]: " + format, args);
        }

        /// <summary>
        /// Méthode s'exécutant lors de l'appui du bouton Settings dans la fenêtre Plugin
        /// </summary>
        public override void OnButtonPress()
        {
            //Ouverture de la fenêtre de configuration
            Forms.frmSettings winSettings = new Forms.frmSettings();
            //Affiche la fenêtre
            winSettings.ShowDialog();
        }

        /// <summary>
        /// Réécriture de la méthode Initialize().
        /// </summary>
        public override void Initialize()
        {
            //Exécute le code par défaut
            base.Initialize();
            if (!blnInit)
            {
                //Ajout d'un texte qui affiche que le plugin a correctement été chargé
                wLog("Loaded (Ver.{0})", Version.ToString());
                blnInit = true;
                //Affiche la raison de la dernière déconnexion
                checkLastLogOut();
                //Vérifie si il y a une nouvelle version du plugin
                if (Classes.Updater.isAvailable)
                    wLog("UPDATE AVAILABLE : A new version of LogMeOut! is available (Ver." + Classes.Updater.getLatestVersion() + "). To update it, open the settings form of this plugin.");
                //Bind l'événement lors de l'appui sur le bouton Start du bot
                BotEvents.OnBotStarted += onBotStarted;
                //Bind l'événement lors de l'écriture dans la console Debug pour récupérer le nombre de stuck
                Logging.OnDebug += new Logging.DebugDelegate(Logging_OnDebug);
                //Bind les événements lors de la réception de whispes
                Lua.Events.AttachEvent("CHAT_MSG_WHISPER", handleWhispes);
                Lua.Events.AttachEvent("CHAT_MSG_BN_WHISPER", handleWhispes);
                //Lie cette instance avec la variable correspondante
                LogMeOut.Instance = this;
                //Instancie l'objet gérant les unités et joueurs environnants
                objPeopleAroundMe = new Classes.PeopleAroundMe();
                //Set le temps de démarrage du bot à maintenant (sinon le bot s'arrête s'il est démarré en cours de route)
                dtStartingTimer = DateTime.Now;
                //Vérifie que le dossier du plugin n'a pas été renommé
                checkPluginPath();
            }
        }

        public override void Dispose()
        {
            //Unbind les événements de réception des whispes
            Lua.Events.DetachEvent("CHAT_MSG_WHISPER", handleWhispes);
            Lua.Events.DetachEvent("CHAT_MSG_BN_WHISPER", handleWhispes);
            //Effectue le Dispose() de base
            base.Dispose();
        }

        public void onBotStarted(EventArgs args)
        {
            if (isEnabled())
            {
                //Crée un pointeur sur l'instance des données sauvegardées
                LogMeOutSettings settings = LogMeOutSettings.Instance;
                //Charge les derniers paramètres du plugin
                settings.Load();
                //Réinitialise les variables vitales du plugin
                dtStartingLogOutProcess = DateTime.MinValue;
                blnDisconnected = false;
                blnSpellOrItemUsed = false;
                blnAlertActionBefore = false;
                dtInCombat = DateTime.Now;
                intNbWhispes = 0;
                intNbDeaths = 0;
                intNbStucks = 0;
                //Vide le cahce contant les unités et joueurs proches
                objPeopleAroundMe.Clear();
                //Set l'heure actuel pour le début du timer
                dtStartingTimer = DateTime.Now;
                //Ajout d'un texte
                wLog("Started successfully !");
                wLog("-------------------------------------------------");
                wLog(" The bot will stop on the following triggers :");
                //Affiche les triggers activés
                if (settings.alertOnBagsFull)
                    wLog(" + On Bags full");
                if (settings.alertOnTimeElapsed)
                    wLog(" + On Time Elapsed (" + settings.hoursElapsed.ToString("00") + "h" + settings.minutesElapsed.ToString("00") + ")");
                if (settings.alertOnDeaths)
                    wLog(" + On number of deaths (" + settings.nbDeaths + " max)");
                if (settings.alertOnStucks)
                    wLog(" + On Stucks (" + settings.nbStucks + " max)");
                if (settings.alertOnMinutesInCombat)
                    wLog(" + On minutes in combat (" + settings.nbMinutesInCombat + " max)");
                if (settings.alertOnMobsKilled)
                    wLog(" + On number of mobs killed (" + settings.nbMobsKilled + " max)");
                if (settings.alertOnWhispesReceived)
                    wLog(" + On whispes received (" + settings.nbWhispesReceived + " max)");
                if (settings.alertOnPoints)
                    wLog(" + On " + settings.nbPoints + " " + Enum.Arrays.NamesPoints[settings.typePoints] + " points reached");
                if (settings.alertOnLevelReached)
                    wLog(" + On level reached (" + settings.nbLevel + " max)");
                if(settings.alertOnPlayerFollows)
                    wLog(" + On " + settings.minutesPlayerFollows + " minutes after a player follows the bot");
                if (settings.alertOnPlayerTargets)
                    wLog(" + On " + settings.minutesPlayerTargets + " minutes after a player targets the bot");
                wLog("-------------------------------------------------");
            }
        }

        /// <summary>
        /// Méthode s'exécutant lors de l'écriture dans la console debug.
        /// </summary>
        public void Logging_OnDebug(string message, Color col)
        {
            //Vérifie que le plugin soit activé
            if (isEnabled())
            {
                //Récupère le nombre de stucks consécutifs si le trigger est actif
                if (LogMeOutSettings.Instance.alertOnStucks)
                {
                    //Si un stuck est détecté
                    if (message.Contains("[STUCK] Got stuck at"))
                    {
                        //Si c'est notre premier stuck (pas eu d'autre il y a 20 secondes)
                        if ((DateTime.Now - dtLastStuck).TotalSeconds > 20)
                        {
                            intNbStucks = 1;
                        }
                        //Si ce n'est pas le premier de la série
                        else
                        {
                            intNbStucks++;
                        }
                        //On enregistre à quel moment il a été effectué
                        dtLastStuck = DateTime.Now;
                        //On inscrit un message dans la console Log
                        WoWPoint maPosition = Styx.WoWInternals.ObjectManager.Me.Location;
                        wLog("Stuck detected (X=\"" + maPosition.X + "\" Y=\"" + maPosition.Y + "\" Z=\"" + maPosition.Z + "\"). Number of consecutive stuck : " +
                                intNbStucks + "/" + LogMeOutSettings.Instance.nbStucks);
                        //Emet un bip si l'option a été activée
                        if (LogMeOutSettings.Instance.beepWhenFire)
                            beep();
                    }
                }
            }
        }

        /// <summary>
        /// Méthode s'exécutant lors d'un whispe.
        /// </summary>
        public void handleWhispes(object sender, LuaEventArgs e)
        {
            //Vérifie que le plugin soit activé
            if (isEnabled())
            {
                //Incrémente le compteur
                intNbWhispes++;
                //Indique que nous avons reçu un whispe
                wLog("Got a whisp from " + e.Args[1] + ". " + intNbWhispes + "/" + LogMeOutSettings.Instance.nbWhispesReceived + " (message: \"" + e.Args[0] + "\")");
                //Emet un bip si l'option a été activée
                if (LogMeOutSettings.Instance.beepWhenFire)
                    beep();
            }
        }

        /// <summary>
        /// Méthode exécutée lors des pulsations du plugin.
        /// </summary>
        public override void Pulse()
        {
            //Crée un pointeur sur l'instance des données sauvegardées
            LogMeOutSettings settings = LogMeOutSettings.Instance;
            //Charge les derniers paramètres du plugin
            settings.Load();

            //Vérifie si le joueur est en phase de déconnexion
            if(this.IsLoggingOut)
                disconnect("");

            //Rafraichit les joueurs environnant dans le cache de LogMeOut!
            objPeopleAroundMe.Update();

            //Vérifie si les sacs du joueur sont plein
            if (settings.alertOnBagsFull)
            {
                //Tente de laisser au moins un slot libre dans les sacs
                if (Me.FreeBagSlots <= 1)
                {
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect("The character's bags are full !");
                }
            }
            
            //Vérifie le temps passé
            if (settings.alertOnTimeElapsed)
            {
                //Détermine le temps restant
                TimeSpan tsRemaining = (dtStartingTimer.AddHours(settings.hoursElapsed).AddMinutes(settings.minutesElapsed) - DateTime.Now);

                //Vérifie s'il est temps de se déconnecter
                if (tsRemaining <= new TimeSpan(0))
                {
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect("Logout time reached");
                }
                //Vérifie si l'option de log est activée
                if (settings.LoggingTime)
                {
                    //S'il est temps d'écrire le temps avant la déconnexion
                    if ((DateTime.Now - dtLastLoggingTime).TotalMinutes >= settings.LoggingTimeEvery)
                    {
                        //Enregistrement du moment présent
                        dtLastLoggingTime = DateTime.Now;
                        //Ecriture du log
                        wLog("Remaining time before log out : " + tsRemaining.Hours.ToString("00") + "h" + tsRemaining.Minutes.ToString("00") + "m" + tsRemaining.Seconds.ToString("00") + "s");
                    }
                }
            }
            else
            {
                //Sinon l'heure du début du timer est égal à maintenant
                dtStartingTimer = DateTime.Now;
            }

            //Vérifie le nombre de mort du joueur
            if (settings.alertOnDeaths)
            {
                //Si l'exception du nombre de mort pendant les BGs est activée
                if (!(Battlegrounds.IsInsideBattleground && settings.exceptionCountDeathsBG))
                {
                    //Si le plugin n'a pas encore traité la mort du joueur
                    if (!Me.IsAlive)
                    {
                        if (blnIsAlive)
                        {
                            //Incrémente le nombre de mort
                            intNbDeaths++;
                            //Affiche comment le joueur est mort
                            wLog("Death detected " + intNbDeaths + "/" + settings.nbDeaths + ". Zone name : " + Me.RealZoneText + " (X=\"" + Me.X + "\" Y=\"" + Me.Y + "\" Z=\"" + Me.Z + "\").");
                            //Affiche qui attaquait le joueur ou si le joueur est mort suite à une chute
                            if (!string.IsNullOrEmpty(strTargetingUnitsAndPlayersNames))
                                wLog("The following unit(s) killed you : " + strTargetingUnitsAndPlayersNames);
                            else if ((DateTime.Now - dtLastIsFalling).TotalSeconds <= 1)
                                wLog("I died because I fell too far !");
                            //Si le nombre de mort a atteint le quota désigné par le joueur
                            if (intNbDeaths >= settings.nbDeaths)
                                disconnect("The character is dead " + intNbDeaths + " times ! (" + settings.nbDeaths + " max)");
                            //Annonce que le bot est mort
                            blnIsAlive = false;
                            //Emet un bip si l'option a été activée
                            if (settings.beepWhenFire)
                                beep();
                        }
                    }
                    else
                    {
                        //Annonce que le bot est vivant
                        blnIsAlive = true;
                        //Si le bot est en chute libre, nous enregistrons le moment
                        if (Me.IsFalling)
                            dtLastIsFalling = DateTime.Now;

                        //Récupère tous les joueurs et unités non alliés qui ciblent le joueur (pour les afficher si le bot meurt par la suite)
                        strTargetingUnitsAndPlayersNames = "";
                        foreach (WoWObject WoWObj in objPeopleAroundMe.GetAllTargetingNonFriendlyUnitAndPlayers())
                        {
                            //Ajout le nom de l'unité ou du joueur dans la chaîne de caractères
                            //Si c'est un joueur
                            if (WoWObj is WoWPlayer)
                                strTargetingUnitsAndPlayersNames += "[Player] " + WoWObj.ToPlayer().Name + "(" + WoWObj.ToPlayer().Level + "); ";
                            //Si c'est un MJ
                            else if (WoWObj is WoWPlayer && WoWObj.ToPlayer().IsGM)
                                strTargetingUnitsAndPlayersNames += "[GM] " + WoWObj.ToPlayer().Name;
                            //Si c'est un pet
                            else if (WoWObj is WoWUnit && WoWObj.ToUnit().IsPet)
                                strTargetingUnitsAndPlayersNames += "[Pet] " + WoWObj.ToUnit().Name + "(of " + WoWObj.ToUnit().OwnedByUnit.Name + "); ";
                            //Si c'est un mob élite
                            else if (WoWObj is WoWUnit && WoWObj.ToUnit().Elite)
                                strTargetingUnitsAndPlayersNames += "[Elite] " + WoWObj.ToUnit().Name + "(" + WoWObj.ToUnit().Level + "); ";
                            //Si c'est un mob
                            else
                                strTargetingUnitsAndPlayersNames += "[Mob] " + WoWObj.ToUnit().Name + "(" + WoWObj.ToUnit().Level + "); ";
                        }
                    }
                }
            }

            //Vérifie le nombre de stucks
            if (settings.alertOnStucks)
            {
                //Si le nombre de stuck maximum a été dépassé, nous déconnectons le bot
                if (intNbStucks >= settings.nbStucks)
                    disconnect("The character was stuck " + intNbStucks + " times !");
            }

            //Vérifie si le temps de combat ne dépasse pas le quota
            if (settings.alertOnMinutesInCombat)
            {
                //Il faut que le bot soit en combat
                if (Me.Combat)
                {
                    //Vérifie le temps passé en combat
                    if ((DateTime.Now - dtInCombat).TotalMinutes >= settings.nbMinutesInCombat)
                    {
                        //Emet un bip si l'option a été activée
                        if (settings.beepWhenFire)
                            beep();
                        //Déconnecte le bot
                        disconnect("The fight lasts more than " + settings.nbMinutesInCombat + " minutes.");
                    }
                }
                else
                {
                    //Durée du combat ramenée à maintenant
                    dtInCombat = DateTime.Now;
                }
            }

            //Vérifie le nombre de monstres tués
            if (settings.alertOnMobsKilled)
            {
                if (Styx.Helpers.InfoPanel.MobsKilled >= settings.nbMobsKilled)
                {
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect("The character killed " + Styx.Helpers.InfoPanel.MobsKilled + " mobs. (" + settings.nbMobsKilled + " max)");
                }
            }

            //Vérifie le nombre de whispes reçus
            if (settings.alertOnWhispesReceived)
            {
                if (intNbWhispes >= settings.nbWhispesReceived)
                    disconnect("The character received " + intNbWhispes + " whispes.");
            }

            //Vérifie le nombre de points atteints
            if (settings.alertOnPoints)
            {
                int intMonnaie = 0;

                switch (settings.typePoints)
                {
                    //Alerte sur les points de justice
                    case 0:
                        intMonnaie = getCurrency((int)Enum.IdPoints.Justice);
                        break;
                    case 1:
                        intMonnaie = getCurrency((int)Enum.IdPoints.Vaillance);
                        break;
                    case 2:
                        intMonnaie = getCurrency((int)Enum.IdPoints.Honneur);
                        break;
                    case 3:
                        intMonnaie = getCurrency((int)Enum.IdPoints.Conquete);
                        break;
                }
                //Déconnexion si le nombre de points dépasse le seuil limite
                if (intMonnaie >= settings.nbPoints)
                {
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect("The character reached " + intMonnaie + " " + Enum.Arrays.NamesPoints[settings.typePoints] + " points ! (" + settings.nbPoints + " max)");
                }
            }

            //Vérification du niveau du joueur
            if (settings.alertOnLevelReached)
            {
                if (Me.Level >= settings.nbLevel)
                {
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect("The character reached the level " + settings.nbLevel + " !");
                }
            }

            //Vérifie si des joueurs suivent le joueur
            if (settings.alertOnPlayerFollows)
            {
                //Obtient les joueurs qui sont autour du bot plus longtemps que le temps spécifié par l'utilisateur
                WoWObject[] followers = objPeopleAroundMe.GetAllPlayers(new TimeSpan(0, settings.minutesPlayerFollows, 0));
                if (followers.Length > 0)
                {
                    //Raison de la déconnexion
                    String strRaison = "These players follow the bot during more than " + settings.minutesPlayerFollows + " minutes : ";
                    foreach (WoWObject WoWObj in followers)
                    {
                        strRaison += WoWObj.Name + " ";
                    }
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect(strRaison);
                }
            }

            //Vérifie si des joueurs ciblent le joueur
            if (settings.alertOnPlayerTargets)
            {
                //Obtient les joueurs qui ciblent le bot plus longtemps que le temps spécifié par l'utilisateur
                WoWObject[] targeters = objPeopleAroundMe.GetAllTargetingPlayers(new TimeSpan(0, settings.minutesPlayerTargets, 0));
                if (targeters.Length > 0)
                {
                    //Raison de la déconnexion
                    String strRaison = "These players target the bot during more than " + settings.minutesPlayerTargets + " minutes : ";
                    foreach (WoWObject WoWObj in targeters)
                    {
                        strRaison += WoWObj.Name + " ";
                    }
                    //Emet un bip si l'option a été activée
                    if (settings.beepWhenFire)
                        beep();
                    //Déconnecte le bot
                    disconnect(strRaison);
                }
            }
        }

        /// <summary>
        /// Méthode de déconnexion du joueur.
        /// </summary>
        /// <param name="strRaison">Raison de la déconnexion</param>
        public void disconnect(string strRaison)
        {
            //Crée un pointeur sur l'instance des données sauvegardées
            LogMeOutSettings settings = LogMeOutSettings.Instance;
            //Ecriture de textes dans le log si cela n'a pas déjà été fait auparavant
            if (!this.IsLoggingOut)
            {
                wLog("Starting logout process...");
                wLog("Reason : " + strRaison);
                //Indique que la procédure de déconnexion a été lancé
                blnStartingLogOutProcess = true;
            }

            #region "Phase de vérification du personnage"
            //Vérifie si le processus ets déjà arrivé à terme
            if (blnDisconnected)
            {
                //Termine la séquence de déconnexion
                return;
            }
            //Vérifie que le joueur est connecté
            if (!StyxWoW.IsInGame)
            {
                //Termine la séquence de déconnexion
                return;
            }
            //Vérifie si le joueur se trouve dans un champ de bataille
            if (Battlegrounds.IsInsideBattleground)
            {
                //Si l'exception est activée, nous restons jusqu'à la fin
                if (settings.exceptionBG)
                {
                    if (!blnBattlegroundOrInstance)
                        wLog("The character is in the battleground " + Battlegrounds.Current.ToString() + ". Waiting the end...");
                    blnBattlegroundOrInstance = true;
                    //Reset le temps du timeout
                    dtStartingLogOutProcess = DateTime.MinValue;
                    //Termine la séquence de déconnexion
                    return;
                }
                else
                {
                    //Sinon nous quittons
                    Battlegrounds.LeaveBattlefield();
                    //Attente de l'écran de chargement
                    waitLoadingScreens();
                    //Termine la séquence de déconnexion
                    return;
                }
            }
            //Vérifie si le joueur se trouve dans une instance
            if (Me.IsInInstance)
            {
                if (settings.exceptionInstance)
                {
                    if (!blnBattlegroundOrInstance)
                        wLog("The character is in the instance (name: " + Me.RealZoneText + "). Waiting the end...");
                    blnBattlegroundOrInstance = true;
                    //Reset le temps du timeout
                    dtStartingLogOutProcess = DateTime.MinValue;
                    //Termine la séquence de déconnexion
                    return;
                }
            }
            //Réinitialise l'avertissement des champs de bataille et instances
            blnBattlegroundOrInstance = false;
            //Vérifie que le joueur ne soit pas en combat
            if (Me.Combat && !settings.alertOnMinutesInCombat)
            {
                if (!blnCombat)
                    wLog("Waiting the end of the fight...");
                blnCombat = true;
                //Reset le temps du timeout
                dtStartingLogOutProcess = DateTime.MinValue;
                //Termine la séquence de déconnexion
                return;
            }
            //Réinitialise l'avertissement de combat
            blnCombat = false;

            //Une fois les vérifications obligatoires, on démarre le timer du timeout
            if(dtStartingLogOutProcess == DateTime.MinValue)
                dtStartingLogOutProcess = DateTime.Now;
            //Détermine si le bot doit se déconnecter sans exécuter certains contrôles à cause du timeout
            if (!this.MustLogOut)
            {
                //Vérifie si le joueur n'est pas mort
                if (Me.Dead || Me.IsGhost)
                {
                    if (!blnIsDead)
                        wLog("The character is dead. Waiting respawn...");
                    blnIsDead = true;
                    //Termine la séquence de déconnexion
                    return;
                }
                //Réinitialise l'avertissement de mort
                blnIsDead = false;
                //Vérifie si le joueur boit ou mange
                if (Me.Buffs.ContainsKey("Food"))
                {
                    wLog("The character is eating. Waiting...");
                    //Termine la séquence de déconnexion
                    return;
                }
                //Reset la variable de combat car il ne l'est plus
                blnCombat = false;
                //Si le joueur veut lancer une action avant la déconnexion
                if (settings.ActionBefore != 0 && !blnSpellOrItemUsed)
                {
                    //Vérifie si le joueur est sur une monture volante ou sur un trajet aérien si nous souhaitons utiliser une objet/sort avant la déconnexion
                    if (Me.IsFlying || Me.IsOnTransport)
                    {
                        if (!blnFlying)
                            wLog("The character is flying. Waiting...");
                        blnFlying = true;
                        //Termine la séquence de déconnexion
                        return;
                    }
                    //Vérifie que le joueur ne soit pas en train de se déplacer
                    if (Me.IsMoving || Me.IsFalling)
                    {
                        //Termine la séquence de déconnexion
                        return;
                    }
                    //Réinitialise l'avertissement de vol
                    blnFlying = false;

                    if (Me.IsCasting)
                    {
                        wLog("Waiting the end of your actual cast (" + Me.CastingSpell.Name + ")");
                        //Termine la séquence de déconnexion
                        return;
                    }
                    switch (settings.ActionBefore)
                    {
                        //Si nous voulons exécuter une pierre de foyer
                        case 1:
                            //Récupère les éventuelles objets faisant office de pierre de foyer
                            IEnumerable<WoWItem> HearthItems = GetItemsByIdFromBags(HearthItemIDs);
                            //Vérifie que nous possédions au moins une pierre de foyer
                            if (HearthItems == null || HearthItems.Count() == 0)
                            {
                                if (!blnAlertActionBefore)
                                    wLog("No hearth items on the character.");
                                blnAlertActionBefore = true;
                            }
                            else
                            {
                                //Tente d'obtenir une pierre de foyer sans cooldown
                                WoWItem Hearth = HearthItems.FirstOrDefault(Item => Item.Cooldown == 0);
                                if (Hearth == null)
                                {
                                    if (!blnAlertActionBefore)
                                        wLog("Your hearth item is not ready yet.");
                                    blnAlertActionBefore = true;
                                    //Termine la séquence de déconnexion
                                    return;
                                }
                                else
                                {
                                    //Utilise la pierre de foyer ou l'objet équivalent
                                    Hearth.Use();
                                    wLog("Using " + Hearth.Name + "...");
                                    //Attend une seconde (le temps que le personnage cast dans WoW)
                                    Thread.Sleep(1000);
                                    //Vérification de l'incantation du sort
                                    blnSpellOrItemUsed = spellChecker((int)Hearth.ItemSpells[0].ActualSpell.CastTime, Hearth.Name);
                                    if (!blnSpellOrItemUsed)
                                    {
                                        //Termine la séquence de déconnexion
                                        return;
                                    }
                                }
                            }
                            break;
                        //Si nous voulons lancer un sort
                        case 2:
                            //Vérifie que le joueur possède ce sort
                            if (!SpellManager.HasSpell(settings.spellName))
                            {
                                if (!blnAlertActionBefore)
                                    wLog("The character does not have the spell : " + settings.spellName);
                                blnAlertActionBefore = true;
                            }
                            else
                            {
                                //Vérifie que le joueur peut lancer ce sort
                                if (!SpellManager.CanCast(settings.spellName))
                                {
                                    if (!blnAlertActionBefore)
                                        wLog("The character cannot cast the spell : " + settings.spellName);
                                    blnAlertActionBefore = true;
                                }
                                else
                                {
                                    //Tente de lancer le sort sur sois-même
                                    SpellManager.Cast(settings.spellName, Me.ToUnit());
                                    wLog("Casting spell  : " + settings.spellName + "...");
                                    //Attend une seconde (le temps que le personnage cast dans WoW)
                                    Thread.Sleep(1000);
                                    //Vérification de l'incantation du sort
                                    blnSpellOrItemUsed = spellChecker((int)SpellManager.Spells[settings.spellName].CastTime, settings.spellName);
                                    if (!blnSpellOrItemUsed)
                                    {
                                        //Termine la séquence de déconnexion
                                        return;
                                    }
                                }
                            }
                            break;
                        //Si nous voulons utiliser un objet
                        case 3:
                            ///Récupère l'objet dans les sacs
                            IEnumerable<WoWItem> SpecificItems = GetItemsByIdFromBags((new uint[] { (uint)settings.itemID }));
                            //Vérifie que cet objet existe bien dans notre inventaire
                            if (SpecificItems == null || SpecificItems.Count() == 0)
                            {
                                if (!blnAlertActionBefore)
                                    wLog("The character does not have the item ID : " + settings.itemID);
                            }
                            else
                            {
                                WoWItem SpecificItem = SpecificItems.First();
                                //Vérifie si celui-ci possède un cooldown actif
                                if (SpecificItem.Cooldown != 0)
                                {
                                    if (!blnAlertActionBefore)
                                        wLog("Cannot use \"" + SpecificItem.Name + "\" because this item is not ready.");
                                    //Termine la séquence de déconnexion
                                    return;
                                }
                                else
                                {
                                    //Tente d'utiliser l'objet proposé
                                    if (!SpecificItem.Usable)
                                    {
                                        if (!blnAlertActionBefore)
                                            wLog("The item \"" + SpecificItem.Name + "\" is not usable.");
                                    }
                                    else
                                    {
                                        SpecificItem.Use();
                                        wLog("Using " + SpecificItem.Name + "...");
                                        //Attend une seconde (le temps que le personnage cast dans WoW)
                                        Thread.Sleep(1000);
                                        //Vérification de l'incantation du sort
                                        blnSpellOrItemUsed = spellChecker((int)SpecificItem.ItemSpells[0].ActualSpell.CastTime, SpecificItem.Name);
                                        if (!blnSpellOrItemUsed)
                                        {
                                            //Termine la séquence de déconnexion
                                            return;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                //Si le bot doit forcer la déconnexion, affiche un message la première fois
                if (!blnForceLogOutMessageShown)
                {
                    wLog("Timeout of the logout reached. Force quit now.");
                    blnForceLogOutMessageShown = true;
                }
            }
            #endregion

            //Inscrit la raison de la déconnexion
            settings.reasonLastLogOut = strRaison;
            settings.Save();
            //Processus de déconnexion OK
            blnDisconnected = true;
            //Déconnexion de World of Warcraft !
            wLog("Log out in 20 secondes...");
            Thread.Sleep(1000 * 20);

            #region Action After
            //Effectue l'éventuelle action après la déconnexion
            switch (settings.ActionAfter)
            {
                //Eteint l'ordinateur
                case 1:
                    wLog("Shutdown the computer.");
                    Process.Start("shutdown", "/s /t 5 /f");
                    break;
                case 2:
                    wLog("Executes \"" + settings.BatchCommand + "\" with the argument(s) \"" + settings.BatchArgument + "\"");
                    Process.Start(settings.BatchCommand, settings.BatchArgument);
                    break;
            }
            #endregion

            //Arrêt du processus
            wLog("Goodbye");
            Lua.DoString("ForceQuit()");
            //Si le processus ne s'est pas arrêté après 5 secondes, nous le tuons
            int intProcessId = ObjectManager.WoWProcess.Id;
            int intTickCount = Environment.TickCount;
            while (Environment.TickCount - intTickCount < 5000 && Process.GetProcessById(intProcessId) != null)
                Thread.Sleep(200);
            if (Process.GetProcessById(intProcessId) != null)
                Process.GetProcessById(intProcessId).Kill();

            //Arrêt du bot
            wLog("Stopping HonorBuddy...");
            Application.Exit();
        }
        
        /// <summary>
        /// IDs des différents objets faisant office de pierre de foyer.
        /// </summary>
        static uint[] HearthItemIDs = 
        {
            (uint)Enum.HearthItems.PierreDeFoyer,
            (uint)Enum.HearthItems.PortailEtherien,
            (uint)Enum.HearthItems.Archeologie
        };

        /// <summary>
        /// Vérifie s'il existe un/des objet(s) faisant référence à un tableau d'IDs.
        /// </summary>
        /// <param name="uintItems">Tableau IDs d'objets à rechercher.</param>
        /// <returns></returns>
        IEnumerable<WoWItem> GetItemsByIdFromBags(uint[] uintItems)
        {
            return from WoWItem Item in Me.BagItems
                   where uintItems.Contains(Item.Entry)
                   select Item;
        }

        /// <summary>
        /// Méthode qui met en pause le bot durant les écrans de chargement.
        /// </summary>
        public void waitLoadingScreens()
        {
            do
            {
                Thread.Sleep(100);
            } while (!StyxWoW.IsInWorld);
        }

        /// <summary>
        /// Détermine le nombre de monnaie que possède le joueur.
        /// </summary>
        /// <param name="intCurrencyId">ID de la monnaie.</param>
        /// <returns>Nombre de monnaie que possède le joueur.</returns>
        public int getCurrency(int intCurrencyId)
        {
            return Lua.GetReturnVal<int>("return GetCurrencyInfo(" + intCurrencyId + ")", 1);
        }

        /// <summary>
        /// Vérifie qu ele plugin soit bien activé.
        /// </summary>
        /// <returns>Retourne l'état d'activation du plugin.</returns>
        public bool isEnabled()
        {
            //Vérifie qu ele plugin soit bien activé
            IEnumerable<bool> blnIsEnabled = from Styx.Plugins.PluginContainer plugin in Styx.Plugins.PluginManager.Plugins
                                             where plugin.Name == this.Name
                                             select plugin.Enabled;
            return blnIsEnabled.First();
        }

        /// <summary>
        /// Vérifie que le dossier de LogMeOut! existe.
        /// </summary>
        public void checkPluginPath()
        {
            if (!Directory.Exists(Path.Combine(Logging.ApplicationPath, Classes.Updater.strPluginPath)))
                wLog("WARNING: The folder of LogMeOut! has been renamed ! Some features won't be able to work. Please, restore the name to \"LogMeOut\".");
        }

        /// <summary>
        /// Emet un bip.
        /// </summary>
        public void beep()
        {
            //Il faut que le bot ne soit pas en train de se déconnecter pour émettre un bip
            if (!this.IsLoggingOut)
            {
                System.Media.SoundPlayer spBeep = new System.Media.SoundPlayer(Path.Combine(Logging.ApplicationPath, Classes.Updater.strPluginPath + "Sounds\\beep.wav"));
                spBeep.PlaySync();
                spBeep.PlaySync();
            }

        }

        /// <summary>
        /// Affiche pourquoi le bot s'est déconnecté la dernière fois (si la déconnexion est dû grâce à LogMeOut!).
        /// </summary>
        public void checkLastLogOut()
        {
            //Vérifie que cette information soit disponible
            if (!string.IsNullOrEmpty(LogMeOutSettings.Instance.reasonLastLogOut))
            {
                //Affiche la raison de la précédente déconnexion
                wLog("Reason of the last log out : {0}", LogMeOutSettings.Instance.reasonLastLogOut);
                //Efface cette raison
                LogMeOutSettings.Instance.reasonLastLogOut = "";
                LogMeOutSettings.Instance.Save();
            }
        }

        /// <summary>
        /// Valide si le sort a correctement été utilisé.
        /// </summary>
        /// <param name="intCastTime">Indique le temps du sort ou de l'objet utilisé.</param>
        /// <param name="strSpellName">Définit le nom du sort incanté.</param>
        /// <returns>Renvoie un booléen en fonction de si le sort/objet a bien été incanté.</returns>
        public bool spellChecker(int intCastTime, string strSpellName)
        {
            //Message de debuggage
            wLogDebug("SpellChecker for " + strSpellName + " (CastTime : " + intCastTime + "ms)");
            //Enregistre le temps lors du début du cast
            DateTime dtCastBegin = DateTime.Now;
            //Tant que le joueur incante
            do
            {
                Thread.Sleep(50);
            } while (Me.IsCasting);
            //Enregistre le temps lors de la fin du cast
            DateTime dtCastEnd = DateTime.Now;
            //Détermine le temps de cast effectué
            double dblCastedTime = (dtCastEnd - dtCastBegin).TotalMilliseconds + 1000;
            //Si le temps d'attente est égal ou plus grand que le temps de cast normal du sort, c'est qu'il a fonctionné !
            if (dblCastedTime >= intCastTime)
            {
                //Le sort a correctement été incanté
                wLog(strSpellName + " has been correctly casted.");
                wLogDebug("SpellChecker : " + strSpellName + " casted in " + dblCastedTime + "ms");
                return true;
            }
            else
            {
                //Le sort n'a pas correctement été incanté
                wLogError(strSpellName + " has NOT been correctly casted !");
                wLogDebug("SpellChecker : " + strSpellName + " interrupted after " + dblCastedTime + "ms");
                return false;
            }
        }

        #region "Propriétés"
        /// <summary>
        /// Renvoi l'heure de démarrage du bot.
        /// </summary>
        public DateTime StartingBot
        {
            get
            {
                return dtStartingTimer;
            }
            set
            {
                dtStartingTimer = value;
            }
        }
        /// <summary>
        /// Renvoi l'état du processus d'arrêt.
        /// </summary>
        public bool IsLoggingOut
        {
            get
            {
                return blnStartingLogOutProcess;
            }
            set
            {
                blnStartingLogOutProcess = value;
            }
        }
        /// <summary>
        /// Indique si le bot DOIT se déconnecter.
        /// </summary>
        /// <remarks>Cela signifie que la procédure de déconnexion a été lancée il y a au moins 5 minutes.</remarks>
        public bool MustLogOut
        {
            get
            {
                if ((DateTime.Now - dtStartingLogOutProcess).TotalMinutes >= 5)
                    return true;
                else
                    return false;
            }
        }
        #endregion
    }
}

