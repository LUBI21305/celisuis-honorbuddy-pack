using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Logic;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using System.Threading;
using System.Diagnostics;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Net;
using Styx.Plugins.PluginClass;
using Styx;

namespace LogMeOut.Classes
{
    /// <summary>
    /// Classe gérant les unités et joueurs autour du bot.
    /// </summary>
    class PeopleAroundMe
    {
        /// <summary>
        /// Contient toutes les unités et joueurs aux alentours et les stockent pendant un certain temps.
        /// </summary>
        private List<WoWObject> listUnitsAndPlayers;
        /// <summary>
        /// Contient le moment d'ajout des unités et joueurs dans la liste "listUnitsAndPeople".
        /// </summary>
        private List<DateTime> listAddingTime;
        /// <summary>
        /// Contient le moment lorsque des unités et joueurs cible le bot.
        /// </summary>
        private List<DateTime> listTargetingTime;

        /// <summary>
        /// Constructeur initialisant les variables.
        /// </summary>
        public PeopleAroundMe()
        {
            //Instanciation des listes
            listUnitsAndPlayers = new List<WoWObject>();
            listAddingTime = new List<DateTime>();
            listTargetingTime = new List<DateTime>();
        }

        /// <summary>
        /// Met à jour la capture des unités et joueurs aux alentours.
        /// </summary>
        public void Update()
        {
            //Ajout les nouveaux joueurs et unités
            this.Dump();
            //Supprime les joueurs et unités inexistants
            this.Clean();
            //Rafraichit le ciblage des unités et joueurs
            this.Refresh();
        }

        #region Méthodes de récupération d'informations
        /// <summary>
        /// Récupère tous les joueurs et unités aux alentours du bot.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllUnitsAndPlayers()
        {
            return listUnitsAndPlayers.Where(obj => !(obj is LocalPlayer)) .ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs et unités ciblant le bot aux alentours du bot.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllTargetingUnitsAndPlayers()
        {
            return listUnitsAndPlayers.Where(obj => !(obj is LocalPlayer) &&
                                                    (obj is WoWUnit && obj.ToUnit().IsTargetingMeOrPet) ||
                                                    (obj is WoWPlayer && obj.ToPlayer().IsTargetingMeOrPet)
                                                    ).ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs et unités non alliés ciblant le bot aux alentours du bot.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllTargetingNonFriendlyUnitAndPlayers()
        {
            return GetAllTargetingUnitsAndPlayers().Where(obj => (obj is WoWUnit && !obj.ToUnit().IsFriendly) || (obj is WoWPlayer && !obj.ToPlayer().IsFriendly)).ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs ciblant le bot aux alentours du bot.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllTargetingPlayers()
        {
            return GetAllTargetingUnitsAndPlayers().Where(obj => obj is WoWPlayer).ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs ciblant le bot aux alentours du bot depuis plus du temps spécifié.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllTargetingPlayers(TimeSpan tsPresentSinceAtLeast)
        {
            //Crée une liste qui contiendra les objets en question
            List<WoWObject> WoWObj = new List<WoWObject>();
            //Pou chaque objets
            for (int intIndex = 0; intIndex < listUnitsAndPlayers.Count; intIndex++)
            {
                //Vérifie que le cet objet soit bien un joueur et qu'il cible bien notre bot depuis plus du temps spécifié
                if (listUnitsAndPlayers[intIndex] is WoWPlayer && !(listUnitsAndPlayers[intIndex] is LocalPlayer) &&
                        listUnitsAndPlayers[intIndex].ToPlayer().IsTargetingMeOrPet && (DateTime.Now - listTargetingTime[intIndex]) >= tsPresentSinceAtLeast)
                    WoWObj.Add(listUnitsAndPlayers[intIndex]);
            }
            //Retourne les objets
            return WoWObj.ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs aux alentours du bot.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllPlayers()
        {
            return GetAllUnitsAndPlayers().Where(obj => obj is WoWPlayer).ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs aux alentours du bot qui sont là depuis un certain temps.
        /// </summary>
        /// <param name="tsPresentSinceAtLeast">Temps de présence minimum dans le cache.</param>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllPlayers(TimeSpan tsPresentSinceAtLeast)
        {
            //Crée une liste qui contiendra les objets en question
            List<WoWObject> WoWObj = new List<WoWObject>();
            //Pou chaque objets
            for (int intIndex = 0; intIndex < listUnitsAndPlayers.Count; intIndex++)
            {
                //Vérifie que le cet objet soit bien un joueur et qu'il est été ajouté depuis plus du temps spécifié
                if (listUnitsAndPlayers[intIndex] is WoWPlayer && !(listUnitsAndPlayers[intIndex] is LocalPlayer) && 
                        (DateTime.Now - listAddingTime[intIndex]) >= tsPresentSinceAtLeast)
                    WoWObj.Add(listUnitsAndPlayers[intIndex]);
            }
            //Retourne les objets
            return WoWObj.ToArray();
        }

        /// <summary>
        /// Récupère tous les joueurs non alliés aux alentours du bot.
        /// </summary>
        /// <returns>Retourne un tableau de WoWObject.</returns>
        public WoWObject[] GetAllNonFriendlyPlayers()
        {
            return GetAllPlayers().Where(obj => !obj.ToPlayer().IsFriendly).ToArray();
        }

        public WoWObject[] GetAllNonFriendlyPlayers(TimeSpan tsPresentSinceAtLeast)
        {
            return GetAllPlayers(tsPresentSinceAtLeast).Where(obj => !obj.ToPlayer().IsFriendly).ToArray();
        }
        #endregion

        /// <summary>
        /// Vide le cache entier contenant les unités et joueurs.
        /// </summary>
        public void Clear()
        {
            listUnitsAndPlayers.Clear();
            listAddingTime.Clear();
        }

        /// <summary>
        /// Affiche tous les unités et joueurs avec leur heure d'apparition dans la console.
        /// </summary>
        public void ShowAll()
        {
            //Header
            Logging.WriteDebug(System.Drawing.Color.Orange, "=======Units & Players==" + DateTime.Now.ToLongTimeString() + "========");
            //Affiche les objets
            for (int intIndex = 0; intIndex < listUnitsAndPlayers.Count; intIndex++)
            {
                Logging.WriteDebug(System.Drawing.Color.Orange, listUnitsAndPlayers[intIndex].Name + " (" + 
                              listUnitsAndPlayers[intIndex].GetType().ToString().Substring(listUnitsAndPlayers[intIndex].GetType().ToString().LastIndexOf(".")) + 
                              ") " + listAddingTime[intIndex].ToLongTimeString());
            }
            //Footer
            Logging.WriteDebug(System.Drawing.Color.Orange, "================================");
        }

        /// <summary>
        /// Ajoute les nouveaux joueurs et unités au cache.
        /// </summary>
        private void Dump()
        {
            //Rafraicit l'ObjetManager de HonorBuddy
            ObjectManager.Update();
            //Récupère les nouveaux joueurs et unités
            foreach (WoWObject WoWObj in ObjectManager.ObjectList.Where(obj => ((obj is WoWUnit && !listUnitsAndPlayers.Contains(obj.ToUnit())) || (obj is WoWPlayer && !listUnitsAndPlayers.Contains(obj.ToPlayer())))))
            {
                //Ajoute l'unité ou le joueur à la liste
                listUnitsAndPlayers.Add(WoWObj);
                //Ajoute son heure d'ajout
                listAddingTime.Add(DateTime.Now);
                //Ajoute l'heure de ciblage
                listTargetingTime.Add(DateTime.Now);
            }
        }

        /// <summary>
        /// Rafraichit l'heure de ciblage des unités et joueurs dans le cache.
        /// </summary>
        private void Refresh()
        {
            //Boucle sur chaque unité/joueur
            for (int intIndex = 0; intIndex < listUnitsAndPlayers.Count; intIndex++)
            {
                //Si l'unité ou le joueur ne cible pas le bot, on rafraichit l'heure
                if ((listUnitsAndPlayers[intIndex] is WoWUnit && !listUnitsAndPlayers[intIndex].ToUnit().IsTargetingMeOrPet) ||
                        (listUnitsAndPlayers[intIndex] is WoWPlayer && !listUnitsAndPlayers[intIndex].ToPlayer().IsTargetingMeOrPet))
                    listTargetingTime[intIndex] = DateTime.Now;
            }
        }

        /// <summary>
        /// Nettoie la liste des unités/joueurs
        /// </summary>
        private void Clean()
        {
            for (int intIndex = listUnitsAndPlayers.Count - 1; intIndex >= 0 ; intIndex--)
            {
                //Si l'unité ou le joueur n'est plus valide
                if (!listUnitsAndPlayers[intIndex].IsValid)
                {
                    //Supprime l'occurrence de la liste des unités/joueurs
                    listUnitsAndPlayers.RemoveAt(intIndex);
                    //Supprime l'occurrence de temps
                    listAddingTime.RemoveAt(intIndex);
                    //Supprime l'heure de ciblage
                    listTargetingTime.RemoveAt(intIndex);
                }
            }
        }
    }
}
