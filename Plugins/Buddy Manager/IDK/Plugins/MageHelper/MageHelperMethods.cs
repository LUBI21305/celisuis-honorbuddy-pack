using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Combat;
using Styx.Logic.BehaviorTree;
using Styx.Logic.POI;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;
using Styx.Plugins.PluginClass;
using Styx.Combat.CombatRoutine;
using Styx.Logic;
using System.Windows.Forms;
using Tripper.Navigation;

namespace MageHelper
{
    public partial class MageHelper
    {
        private bool LOSCheck = false;
        private bool MadeItToOrg = false;
        private bool DidRepair = false;
        private bool CheckRunes = false;
        private bool MovedOutside = false;
        private bool DidMail = false;
        private bool MovedToPortals = false;
        private bool UsedPortal = false;
        private bool ReturnedToXYZ = false;
        private bool IsStarted = false;
        private bool IsFinished = false;
        private WoWPoint OldLocation;
        private uint OldMapId;
        private string OldZoneName;
        private Styx.Logic.POI.BotPoi CurrentPOI { get { return Styx.Logic.POI.BotPoi.Current; } }
        private List<string> BadBotBases = new List<string> { "Instancebuddy", "PartyBot", "LazyRaider", "BG Bot [Beta]", "PvP", "Mixed Mode" };

        private void SlowFall()
        {
            if (UseSlowFall && Me.IsFalling & SpellManager.CanCast("Slow Fall") && !Me.Auras.ContainsKey("Slow Fall"))
            {
                Thread.Sleep(1000);
                if (Me.IsFalling) { Log("I am Falling!  Casting Slow Fall!", null); SpellManager.Cast("Slow Fall", Me); }
            }
        }

        public void Invisibility()
        {
            string numberofMobs;
            if (UseInvis)
            {
                List<WoWUnit> AddList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
                unit.Guid != Me.Guid &&
                unit.Aggro &&
                !unit.IsFriendly &&
                !unit.IsTotem);

                if (AddList.Count >= InvisAdds && Me.HealthPercent <= InvisHP && SpellManager.CanCast("Invisibility"))
                {
                    numberofMobs = AddList.Count.ToString();
                    Log(string.Format("Have {0} mobs, and am about to die!", numberofMobs));
                    Log("Casting Invisibilty to exit combat.", null);
                    if (Me.GotAlivePet) { Lua.DoString(string.Format("RunMacroText(\"{0}\")", "/PetAbandon()")); }
                    if (Me.IsCasting) { SpellManager.StopCasting(); StyxWoW.SleepForLagDuration(); }
                    SpellManager.Cast("Invisibility");
                    Me.ClearTarget();
                    AddList = null;
                    Styx.Logic.POI.BotPoi.Clear("MageHelper exiting combat to save a life.");
                }
            }
        }

        private bool LockItUp(string method, List<WoWUnit> objList)
        {
            ObjectManager.Update();
            bool _Return = false;
            if (method == "moreMobs")
            {
                /*Checks for mobs within 15 yards of the loot we want to blink to... This way you won't be blinking into an agro field*/
                foreach (WoWUnit z in objList)
                {
                    if (z.IsValid && !z.IsMe && z.Guid != LastPOI.Guid && z.Location.Distance(LastPOI.Location) <= 15 && !z.Dead && z.IsHostile) { _Return = true; }
                }
                if (_Return) { /*Log("Found Hostiles near", LastPOI.Name);*/ }
                return _Return;
            }
            if (method == "closerLoot")
            {
                /*Check is there is closer what our POI is...  The bot is sometimes stupid, and could possibly set a POI different than what it is actually moving to loot
                 * This would result in blinking to the wrong loot*/
                foreach (WoWUnit z in objList)
                {
                    if (z.IsValid && !z.IsMe && z.Guid != LastPOI.Guid && z.Lootable && z.Location.Distance(Me.Location) < LastPOI.Location.Distance(Me.Location)) { _Return = true; }
                }
                if (_Return) { /*Log("Found Loot closer than", LastPOI.Name);*/ }
                return _Return;
            }
            return true;
        }

        private bool IsLastPOItheCurrentPOI()
        {
            if (LastPOI.Guid == CurrentPOI.Guid) return true;
            else return false;
        }

        private bool BlinkOnlyWithPerfection()
        {

            while (Navigator.GeneratePath(Me.Location, LastPOI.Location).Length > 2) { LOSCheck = true; Navigator.MoveTo(LastPOI.Location); Thread.Sleep(ranNum(250, 500)); }
            if (LOSCheck) { Log("Moved into LOS/1 CTM left to: ", LastPOI.Name); }
            return LOSCheck;
        }

        private void BlinkToPOI()
        {
            if (Me.CurrentTarget == null && UseBlink && Styx.Helpers.CharacterSettings.Instance.LootMobs.Equals(true))
            {

                if (Styx.Logic.POI.BotPoi.Current != null && CurrentPOI.AsObject is WoWUnit)
                {
                    LastPOI = CurrentPOI.AsObject;//Sets the GUID of the Current/Last POI

                    if (true)
                    {
                        bool closerLoot = false;
                        bool moreMobs = false;
                        LOSCheck = false;
                        ObjectManager.Update();

                        List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                        .Where(o => !o.Dead)
                        .OrderBy(o => o.Distance).ToList();

                        moreMobs = LockItUp("moreMobs", objList);


                        closerLoot = LockItUp("closerLoot", objList);

                        if (LastPOI.ToUnit().Lootable && SpellManager.CanCast("Blink") && LastPOI.Distance >= 19 && LastPOI.Distance <= 40 && !moreMobs && !closerLoot)
                        {
                            //Log("Checking LOS", null);
                            /*if (!LastPOI.InLineOfSight) { bool done = false; Log("Moving into LoS", null); LOSCheck = true; while (!done) { Navigator.MoveTo(LastPOI.Location); Thread.Sleep(ranNum(750, 1000)); if (LastPOI.InLineOfSight) { done = true; } } }*/

                            BlinkOnlyWithPerfection();

                            if (!Me.Mounted)
                            {
                                if (LOSCheck == true && SpellManager.CanCast("Blink") && LastPOI.Distance >= 19 && LastPOI.Distance <= 50)
                                { LastPOI.ToUnit().Face(); StyxWoW.SleepForLagDuration(); SpellManager.Cast("Blink"); Log("Blinked(LoS) to", LastPOI.Name); Navigator.Clear(); Navigator.MoveTo(LastPOI.Location); }

                                if (LOSCheck == false)
                                { LastPOI.ToUnit().Face(); StyxWoW.SleepForLagDuration(); SpellManager.Cast("Blink"); Log("Blinked(N-LoS) to", LastPOI.Name); Navigator.Clear(); Navigator.MoveTo(LastPOI.Location); }
                                LOSCheck = false;
                                checkedMobs = false;
                            }
                            else
                            {
                                Log("I am mounted. Not Blinking to: ", LastPOI.Name);
                            }
                        }
                    }
                }
            }
        }


        private void PortalsRule()
        {
            while (IsStarted && !StyxWoW.IsInWorld)
            {
                Thread.Sleep(1000);
            }
            //BotPoi.Current.Type == PoiType.Repair
            //Check for BG, Instance, Zone we can't get back to
            if (!IsStarted && (CurrentPOI.Type == PoiType.Mail || CurrentPOI.Type == PoiType.Repair || CurrentPOI.Type == PoiType.Sell) && !Styx.BotManager.Current.Name.ContainsAny(BadBotBases))
            {
                Log(CurrentPOI.Type.ToString(), "detected.  Using Portalling Option");
                IsStarted = true;
                IsFinished = false;
                return;
            }
            while (!Me.IsActuallyInCombat && !MadeItToOrg && IsStarted)
            {
                OldLocation = Me.Location;
                OldMapId = Me.MapId;
                Log("Old MapId is: ", Me.MapId.ToString());
                OldZoneName = Me.ZoneText;
                Log("Old ZoneName is: ", Me.ZoneText);
                if (ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.IsHostile && o.IsAlive && o.Distance < 5).Count() == 0)
                {
                    SpellManager.StopCasting();
                    Styx.Logic.Mount.Dismount();
                    Navigator.PlayerMover.MoveStop();
                    StyxWoW.SleepForLagDuration();
                    if (SpellManager.CanCast(3567))
                    {
                        IsStarted = true;
                        SpellManager.Cast(3567);
                        Thread.Sleep(2500);
                        while (Me.IsCasting || !StyxWoW.IsInWorld)
                        {
                            Log("Sleeping while casting", null);
                            Thread.Sleep(5000);
                        }
                        Log("End of Sleep 1, Sleep additional 5 seconds.", null);
                        Thread.Sleep(5000);
                        if (/*Me.MapId == Org*/true)
                        {
                            MadeItToOrg = true;
                        }
                        return;
                    }
                    else
                    {
                        IsStarted = false;
                    }
                }
            }


            //Move to Vendor (Sell & Repair), Move to Mailbox (Mail Items)
            while (MadeItToOrg && !DidRepair && IsStarted)
            {
                ObjectManager.Update();
                if (Me.Location.Distance(new WoWPoint(1776.5, -4338.8, -7.508744)) < 10)
                {
                    Navigator.MoveTo(new WoWPoint(1823.608, -4304.775, -12.16005));
                    foreach (WoWUnit Muragas in ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == 3330))
                    {
                        while (Muragas.Distance > Muragas.InteractRange) { Navigator.MoveTo(Muragas.Location); Log("Moving to Repair."); }
                        Navigator.PlayerMover.MoveStop();
                        Muragas.Interact();
                        StyxWoW.SleepForLagDuration();
                        Styx.Logic.Vendors.RepairAllItems();
                        StyxWoW.SleepForLagDuration();
                        Styx.Logic.Vendors.SellAllItems();
                        StyxWoW.SleepForLagDuration();
                    }
                }
                DidRepair = true;
                return;
            }



            while (DidRepair && !CheckRunes && IsStarted)
            {
                //Check for Runes of Teleport, buy if I have less than 5
                uint NumberOfRunes = 0;
                ObjectManager.Update();
                foreach (WoWItem Runes in ObjectManager.GetObjectsOfType<WoWItem>().Where(o => o.Entry == 17031))
                {
                    if (Runes.BagSlot != -1)
                    {
                        NumberOfRunes += Runes.StackCount;
                    }
                }
                Log("Found # Runes:", NumberOfRunes.ToString());
                if (NumberOfRunes < 5)
                {
                    while (new WoWPoint(1819.77, -4305.931, -12.17886).Distance(Me.Location) > 3)
                    {
                        Navigator.MoveTo(new WoWPoint(1819.77, -4305.931, -12.17886));
                    }
                    while (new WoWPoint(1812.137, -4318.922, -11.23538).Distance(Me.Location) > 3)
                    {
                        Navigator.MoveTo(new WoWPoint(1812.137, -4318.922, -11.23538));
                    }
                    while (new WoWPoint(1831.635, -4333.173, -15.48243).Distance(Me.Location) > 3)
                    {
                        Navigator.MoveTo(new WoWPoint(1831.635, -4333.173, -15.48243));
                    }
                    foreach (WoWUnit ReagentVendor in ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == 3335))
                    {
                        Log("Inside ReagVend Foreach.", null);
                        while (ReagentVendor.Location.Distance(Me.Location) > ReagentVendor.InteractRange) { Navigator.MoveTo(ReagentVendor.Location); }
                        Navigator.PlayerMover.MoveStop();
                        ReagentVendor.Interact();
                        while (!MerchantFrame.Instance.IsVisible) { Thread.Sleep(100); }
                        MerchantFrame.Instance.BuyItem("Rune of Teleportation", 5);
                        Log("Bought 5 'Rune of Teleportation'", null);
                        StyxWoW.SleepForLagDuration();
                    }
                    NumberOfRunes = 0;
                    while (new WoWPoint(1831.958, -4331.217, -15.48225).Distance(Me.Location) > 2)
                    {
                        Navigator.MoveTo(new WoWPoint(1831.958, -4331.217, -15.48225));
                    }
                }
                CheckRunes = true;
                return;
            }


            while (CheckRunes && !MovedOutside && IsStarted)
            {
                while (new WoWPoint(1798.782, -4325.472, -11.27483).Distance(Me.Location) > 2)
                {
                    Navigator.MoveTo(new WoWPoint(1798.782, -4325.472, -11.27483));
                    Log("Moving Outside. Step 1.", null);
                }
                while (new WoWPoint(1778.542, -4295.233, 6.407692).Distance(Me.Location) > 2)
                {
                    Navigator.MoveTo(new WoWPoint(1778.542, -4295.233, 6.407692));
                    Log("Moving Outside. Step 2", null);
                }
                while (new WoWPoint(1756.049, -4306.153, 6.61089).Distance(Me.Location) > 3)
                {
                    Navigator.MoveTo(new WoWPoint(1756.049, -4306.153, 6.61089));
                    Log("Moving Outside. Step 3", null);
                }
                while (new WoWPoint(1786.761, -4239.646, 40.70471).Distance(Me.Location) > 4)
                {
                    Navigator.MoveTo(new WoWPoint(1786.761, -4239.646, 40.70471));
                    Log("Moving Outside. Step 4", null);
                }
                MovedOutside = true;
                return;
            }


            while (MovedOutside && !DidMail && IsStarted)
            {
                if (Styx.Logic.Inventory.InventoryManager.HaveItemsToMail && Styx.Helpers.CharacterSettings.Instance.MailRecipient != null)
                {
                    ObjectManager.Update();
                    Log("Mailing Items");
                    foreach (WoWGameObject Mailbox in ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 206732 && o.SubType == WoWGameObjectType.Mailbox))
                    {
                        Log("Inside Mailbox Foreach", null);
                        while (Mailbox.Location.Distance(Me.Location) > Mailbox.InteractRange) { Navigator.MoveTo(Mailbox.Location); }
                        Navigator.PlayerMover.MoveStop();
                        Thread.Sleep(1000);
                        Mailbox.Interact();
                        StyxWoW.SleepForLagDuration();
                        Styx.Logic.Vendors.MailAllItems();
                        StyxWoW.SleepForLagDuration();
                    }
                }
                else
                {
                    Log("No Items to Mail.  Continue!");
                    DidMail = true;
                    return;
                }
            }


            while (DidMail && !MovedToPortals && IsStarted)
            {
                //Move to Portal Area
                Thread.Sleep(1000);
                if (!Me.Mounted) { Flightor.MountHelper.MountUp(); }
                Thread.Sleep(1000);
                while (Me.IsCasting) { Thread.Sleep(100); }
                Log("Flightor moving to Portals");
                while (new WoWPoint(2048.148, -4376.259, 98.84513).Distance(Me.Location) > 4)
                {
                    Flightor.MoveTo(new WoWPoint(2048.148, -4376.259, 98.84513));
                    Thread.Sleep(100);
                }
                Log("Made it to Portals");
                Thread.Sleep(500);
                Flightor.MountHelper.Dismount();
                MovedToPortals = true;
                return;
            }


            while (MovedToPortals && !UsedPortal && IsStarted)
            {
                while (Me.IsCasting) { Thread.Sleep(50); }
                Thread.Sleep(500);
                if (Me.Mounted) { Flightor.MountHelper.Dismount(); }
                Thread.Sleep(500);
                ObjectManager.Update();
                foreach (WoWGameObject Portal in ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 206595))
                {
                    while (Portal.Location.Distance(Me.Location) > Portal.InteractRange)
                    {
                        Navigator.MoveTo(Portal.Location);
                        Log("Moving towards ", Portal.Name);
                        Thread.Sleep(100);
                    }
                    Navigator.PlayerMover.MoveStop();
                    Thread.Sleep(500);
                    Portal.Interact();
                    Thread.Sleep(5000);
                }
                //Insert Logic to use correct portal, or just fuck off!
                if (OldMapId == 599999)
                {
                    //I don't fucking know
                }

                if (OldZoneName == "ShitYea")
                {
                    //Fuck off
                }
                UsedPortal = true;
                return;
            }


            while (MadeItToOrg && DidRepair && CheckRunes && DidMail && MovedToPortals && UsedPortal && !ReturnedToXYZ && IsStarted)
            {
                Thread.Sleep(10000);
                if (Me.MapId == OldMapId)
                {
                    if (BotManager.Current.Name == "Grind Bot")
                    {
                        Log("Using 'Grind Bot'.");
                        /*if(Styx.Logic.Profiles.Quest.ProfileHelperFunctionsBase.CanFly())
                        {
                            Log("Flying back to Original XYZ.");
                            Flightor.MountHelper.MountUp();
                            Thread.Sleep(1000);
                            while (Me.IsCasting) { Thread.Sleep(100); }
                            while (OldLocation.Distance(Me.Location) > 5)
                            {
                                Flightor.MoveTo(OldLocation);
                                Thread.Sleep(100);
                            }
                        }*/
                        //Doesn't fackin work!
                        Thread.Sleep(5000);
                        Log("Returning to Orig XYZ");
                        while (OldLocation.Distance(Me.Location) > 5)
                        {
                            Navigator.MoveTo(OldLocation);
                            Thread.Sleep(100);
                        }
                        Log("At Original XYZ.");
                        IsFinished = true;
                    }
                    /*
                    if (BotManager.Current.Name == "Gatherbuddy2")
                    {
                        Flightor.MountHelper.MountUp();
                        IsFinished = true;
                    }
                    if (BotManager.Current.Name == "Questing")
                    {
                        Flightor.MountHelper.MountUp();
                        Thread.Sleep(500);
                        while (Me.IsCasting) { Thread.Sleep(100); }
                        StyxWoW.SleepForLagDuration();
                        Flightor.MoveTo(OldLocation);
                        IsFinished = true;
                    }
                    if (BotManager.Current.Name == "ArchaeologyBuddy")
                    {
                        Flightor.MountHelper.MountUp();
                        IsFinished = true;
                    }*/
                }
                IsFinished = true;
                ReturnedToXYZ = true;
                return;
            }

            if (IsStarted && IsFinished)
            {
                Log("Successful internention of Sell/Mail/Repair");
                Log("Now waiting for another opportunity...");
                MadeItToOrg = false;
                DidRepair = false;
                CheckRunes = false;
                DidMail = false;
                MovedOutside = false;
                MovedToPortals = false;
                UsedPortal = false;
                ReturnedToXYZ = false;
                IsStarted = false;
                IsFinished = false;
            }
            if (IsStarted && !IsFinished)
            {
                Log("Well I messed up somewhere.", null);
                Log("You're character is now stuck, and I don't have the logic to get it back to where it was apparently, so please attach this log in the MageHelper thread and explain where you were farming.", null);
                IsStarted = true;
                IsFinished = true;
            }

            /* ZONE ID LIST
             * 0 = Kalimdor
             * 1 = Eastern Kingdoms
             * 8 = Outlands
             * 10 = Northrend
             * 11 = Maelstrom
             * 5042 = Deepholm
             * 5389 = Tol Barad Peninsula
             * 
             */

        }//End of PortalsRule()


    }





    /*These are just down here for past references to what I used prior.
     * Dont worry about them
     * /
    /*foreach (WoWUnit z in objList)
                    {
                        if (z.IsValid && !z.IsMe && z.Guid != LastPOI.Guid && z.Lootable && z.Location.Distance(Me.Location) < LastPOI.Location.Distance(Me.Location)) { closerLoot++; }
                    }
                    if (closerLoot > 0) { Log("Found Loot closer than", LastPOI.Name); }*/

    /*foreach (WoWUnit z in objList)
    {
        if (z.IsValid && !z.IsMe && z.Guid != LastPOI.Guid && z.Location.Distance(LastPOI.Location) <= 15 && !z.Dead && z.IsHostile) { moremobs = true; }
    }
    if (moremobs) { Log("Found Hostiles near", LastPOI.Name); }*/
}
