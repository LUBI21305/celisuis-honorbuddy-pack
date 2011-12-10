using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;


using Styx;
using Styx.Plugins.PluginClass;
using Styx.Logic.Questing;
using Styx.Logic.Profiles;
using Styx.Helpers;
using Styx.WoWInternals;


namespace BeowulfePlugins
{
    class QuestProtector : HBPlugin
    {
        List<int>   m_lProtectedItems = new List<int>();

        private readonly Version m_verCurrent;
        private bool m_bHasInitialized = false;

        public override string Name { get { return "Quest Protector"; } }
        public override string Author { get { return "Beowulfe"; } }
        public override Version Version { get { return m_verCurrent; } }
        public override string ButtonText { get { return "Settings"; } }
        public override bool WantButton { get { return false; } }


        public QuestProtector()
        {
            m_verCurrent = new Version(1, 0, 1, 0);
            BotEvents.OnBotStart += BotStartup;
            BotEvents.OnBotStop += BotStop;
        }


        ~QuestProtector()
        {
            ClearProtectedItems();
            BotEvents.OnBotStart -= BotStartup;
            BotEvents.OnBotStart -= BotStop;
        }

        
        public override void Pulse()
        {
            if (!m_bHasInitialized)
            {
                Logging.Write("[Quest Protector] Initializing...");

                Lua.Events.AttachEvent("QUEST_ACCEPTED", HandleQuestUpdate);
                Lua.Events.AttachEvent("QUEST_ITEM_UPDATE", HandleQuestUpdate);
                Lua.Events.AttachEvent("QUEST_COMPLETE", HandleQuestUpdate);
                m_bHasInitialized = true;

                Logging.Write("[Quest Protector] Complete!");
            }
        }


        private void BotStartup(EventArgs args)
        {
            Logging.Write("[Quest Protector] Bot startup detected, ensuring newest item protection...");
            ParseQuests();
        }


        private void BotStop(EventArgs args)
        {
            Logging.Write("[Quest Protector] Bot shutdown detected, clearing item protection.");
            ClearProtectedItems();
        }


        private void HandleQuestUpdate(object sender, LuaEventArgs args)
        {
            Logging.Write("[Quest Protector] Quest update received, updating item protection.");
            ParseQuests();
        }


        public void ParseQuests()
        {
            List<PlayerQuest> lQuests;

            ClearProtectedItems();

            lQuests = ObjectManager.Me.QuestLog.GetAllQuests();

            for (int i = 0; i < lQuests.Count; i++)
                ProtectQuestItems(lQuests[i]);
        }


        private void ClearProtectedItems()
        {
            //Logging.Write("[Quest Protector] Clearing protected items...");

            foreach (int iID in m_lProtectedItems)
            {
                //Logging.Write("[Quest Protector] Item \"" + iID + "\" removed from protection.");
                ProfileManager.CurrentProfile.ProtectedItems.Remove((uint)iID);
            }

            m_lProtectedItems.Clear();
        }


        private void ProtectQuestItems(PlayerQuest quest)
        {
            //Logging.Write("[Quest Protector] Protecting quest items for quest \"" + quest.Name + "\"...");

            foreach (int iID in quest.CollectItemIDs)
            {
                if (m_lProtectedItems.Contains(iID))
                    continue;

                if (ProfileManager.CurrentProfile.ProtectedItems.Contains((uint)iID))
                {
                    //Logging.Write("[Quest Protector] Item \"" + iID + "\" is already protected.");
                    continue;
                }

                m_lProtectedItems.Add(iID);
                ProfileManager.CurrentProfile.ProtectedItems.Add((uint)iID);
                //Logging.Write("[Quest Protector] Protected item \"" + iID + "\".");
            }

            foreach (int iID in quest.CollectIntermediateItemIDs)
            {
                if (m_lProtectedItems.Contains(iID))
                    continue;

                if (ProfileManager.CurrentProfile.ProtectedItems.Contains((uint)iID))
                {
                    //Logging.Write("[Quest Protector] Item \"" + iID + "\" is already protected.");
                    continue;
                }

                m_lProtectedItems.Add(iID);
                ProfileManager.CurrentProfile.ProtectedItems.Add((uint)iID);
                //Logging.Write("[Quest Protector] Protected item \"" + iID + "\".");
            }
        }
    }
}
