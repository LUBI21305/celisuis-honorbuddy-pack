using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Timers;
using System.Xml.Linq;
using System.Xml;

using Styx;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins.PluginClass;
using Styx.Logic.Pathing;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using System.Text.RegularExpressions;
using Styx.Logic.Profiles;

namespace Allrounder
{
    class Mail
    {

        public static MailFrame mailFrame = new MailFrame();
        public static WoWPoint mailboxLoc = new WoWPoint();
        public static void Mailspell()
        {
            Logging.Write(Color.Blue, "Moving to Mailbox");
            if (mailboxLoc.X != 0f && mailboxLoc.Y != 0f && mailboxLoc.Z != 0f)
            {
                Styx.Logic.Mount.MountUp();
                while (!ObjectManager.Me.Combat && mailboxLoc.Distance(ObjectManager.Me.Location) > 5)
                {
                    Navigator.MoveTo(mailboxLoc);
                    Thread.Sleep(100);
                }
            }
            ObjectManager.Update();
            List<WoWGameObject> gObjList = ObjectManager.GetObjectsOfType<WoWGameObject>();
            List<WoWGameObject> mailboxList = new List<WoWGameObject>();
            foreach (WoWGameObject o in gObjList)
            {
                if (o.SubType == WoWGameObjectType.Mailbox)
                    mailboxList.Add(o);
            }
            if (mailboxList.Count != 0)
            {

                mailboxList.Sort((p1, p2) => p1.Location.Distance(ObjectManager.Me.Location).CompareTo(p2.Location.Distance(ObjectManager.Me.Location)));
                if (mailboxList[0].Distance > 50) Styx.Logic.Mount.MountUp();
                while (!ObjectManager.Me.Combat && mailboxList[0].Distance > 5)
                {
                    Navigator.MoveTo(mailboxList[0].Location);
                    Thread.Sleep(100);
                }
                mailboxList[0].Interact();
                Thread.Sleep(2500);
                int mailCount = mailFrame.MailCount;
                int i = mailCount;
                Logging.Write("[Mail]:I Have {0} Mail", i);
                Logging.Write("[Mail]:Looting Mail");
                while (ObjectManager.Me.FreeNormalBagSlots >= 10 && i >= 1)
	        
                {
                    StyxWoW.SleepForLagDuration();
                    Lua.DoString("TakeInboxItem(" + i.ToString() + ")");
                    i--;

                    if (ObjectManager.Me.FreeNormalBagSlots < 10 || mailCount < 1)
                    {
                        mailFrame.Close();
                        break;
                    }
                }
                      
            }
        }

    }
}

   
    

