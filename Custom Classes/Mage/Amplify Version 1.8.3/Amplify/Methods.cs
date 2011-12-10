using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = System.Action;
using Sequence = Styx.Logic;
using Styx.WoWInternals.World;


namespace Amplify
{
    public partial class Amplify
    {
      
            public static bool GotSheep
            {
                get
                {
                    List<WoWUnit> sheepList =
                        (from o in ObjectManager.ObjectList
                         where o is WoWUnit
                         let p = o.ToUnit()
                         where p.Distance < 40
                               && p.HasAura("Polymorph")
                         select p).ToList();

                    return sheepList.Count > 0;
                }
            }

            public string CurrentPullSpell
            {
                get
                {
                    return AmplifySettings.Instance.PullSpellSelect;
                }
            }
            public string CurrentSpamSpell
            {
                get
                {
                    return AmplifySettings.Instance.FrostSpamSpell;
                }
            }

            public string FireSpamSpell
            {
                get
                {
                    return AmplifySettings.Instance.FireSpamSpell;
                }
            }
      public static bool NeedToSheep()
      {
          List<WoWUnit> AddList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
unit.Guid != Me.Guid &&
unit.IsTargetingMeOrPet &&
!unit.IsFriendly &&
!unit.IsTotem &&
!unit.IsPet &&
(unit.CreatureType == WoWCreatureType.Humanoid || unit.CreatureType == WoWCreatureType.Beast) &&
unit != Me.CurrentTarget &&
!Styx.Logic.Blacklist.Contains(unit.Guid));
          if(AddList.Count > 0 && !GotSheep && Me.CurrentTarget != null)
          {
              return true;
          }
          return false;
    
      }


        public static void SheepLogic()
        {
            List<WoWUnit> AddList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
unit.Guid != Me.Guid &&
unit.IsTargetingMeOrPet &&
!unit.IsFriendly &&
!unit.IsPet &&
!unit.IsTotem &&
(unit.CreatureType == WoWCreatureType.Humanoid || unit.CreatureType == WoWCreatureType.Beast) &&
unit != Me.CurrentTarget &&
!Styx.Logic.Blacklist.Contains(unit.Guid));

            if (AddList.Count > 0 && !GotSheep)
            {
                Log("Got adds Lets Polymorph a Target");
                WoWUnit SheepAdd = AddList[0].ToUnit();
                Log("Casting Poly on {0}", SheepAdd.Name);
                SpellManager.Cast("Polymorph", SheepAdd);
                if (Styx.StyxWoW.GlobalCooldown)
                {
                    while (Styx.StyxWoW.GlobalCooldown)
                    {
                        Thread.Sleep(10);
                    }
                }
                SheepTimer.Reset();
                SheepTimer.Start();
            }
        }

            
        public static void retargetSheep()
        {
            List<WoWUnit> mobList = ObjectManager.GetObjectsOfType<WoWUnit>(false);

            foreach (WoWUnit sheep in mobList)
            {
                if (sheep.Auras.ContainsKey("Polymorph"))
                {
                    Log("Only Sheep Exists Re-Targeting Sheep");
                    sheep.Target();
                 
                }
            }
        }
        public static void retargetTotem()
        {
            List<WoWUnit> mobList = ObjectManager.GetObjectsOfType<WoWUnit>(false);

            foreach (WoWUnit Totem in mobList)
            {
                if (Totem.CreatureType == WoWCreatureType.Totem && Totem.CurrentHealth > 1 && Totem.Attackable)
                {
                    Log("Totem on the Feild Targeting Totem!");
                    Totem.Target();

                }
            }
        }
        private static void IncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
        {
            using (new FrameLock())
            {
                if (!StyxWoW.Me.GotAlivePet || (StyxWoW.Me.GotAlivePet && !StyxWoW.Me.Pet.Combat))
                    return;

                for (int i = 0; i < incomingUnits.Count; i++)
                {
                    if (incomingUnits[i] is WoWUnit)
                    {
                        WoWUnit u = incomingUnits[i].ToUnit();
                        if (u.Combat && (u.IsTargetingMeOrPet || u.PetAggro))
                            outgoingUnits.Add(u);
                    }
                }
            }
        }
        public bool SpellToSteal()
        {
            using (new FrameLock())
            {
                foreach (KeyValuePair<string, WoWAura> pair in Me.CurrentTarget.ActiveAuras)
                {
                    WoWAura curAura = pair.Value;
                    if (curAura.Spell.SpellEffect1.EffectType == WoWSpellEffectType.Heal)
                    {
                        Logging.Write("CurrentTarget's Buff " + curAura.Name + " is healing him");
                        return true;
                    }
                }
                return false;
            }
        }
        public static bool IsInPartyOrRaid()
        {
            if (Me.PartyMembers.Count > 0)
                return true;

            return false;
        }
        public static List<WoWUnit> getAdds()
        {
            List<WoWUnit> AddList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
           unit.Guid != Me.Guid &&
           unit.IsTargetingMeOrPet &&
           !unit.IsFriendly &&
           !unit.IsPet &&
           !unit.IsTotem &&
           unit != Me.CurrentTarget &&
           !Styx.Logic.Blacklist.Contains(unit.Guid));

            return AddList;

        }
        public static List<WoWUnit> getallunits()
        {
            List<WoWUnit> AllUnitsList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
           unit.Guid != Me.Guid &&
           !unit.IsFriendly &&
           !unit.Combat &&
           !Styx.Logic.Blacklist.Contains(unit.Guid));

            return AllUnitsList;

        }
        public static List<WoWUnit> getTotems()
        {
            List<WoWUnit> TotemsList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
           unit.Guid != Me.Guid &&
           !unit.IsFriendly &&
           !unit.IsPet &&
           unit.IsTotem &&
           !Styx.Logic.Blacklist.Contains(unit.Guid));

            return TotemsList;

        }
        public static void FindClostestPlayer(int Range)
        {
            List<WoWPlayer> PlrNearList2 = (from p in ObjectManager.GetObjectsOfType<WoWPlayer>() let d = p.Distance where d <= Range && !p.Dead && !p.IsFriendly orderby d ascending select p).ToList();

            if (IsBattleGround() && PlrNearList2.Count > 0)
            {
                Log("My CurrentTarget died or got removed, finding new.");
                PlrNearList2[0].Target();
            }
        }
        public static List<WoWUnit> getAdds2()
        {
            List<WoWUnit> AddList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
           unit.Guid != Me.Guid &&
           (unit.IsTargetingMeOrPet || unit.IsTargetingMyPartyMember) &&
           !unit.IsFriendly &&
           !unit.IsPet &&
           unit != Me.CurrentTarget &&
           !Styx.Logic.Blacklist.Contains(unit.Guid));

            return AddList;

        }
  
        public static bool Adds()
        {
          
            List<WoWUnit> mobList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
                unit.Guid != Me.Guid &&
                unit.IsTargetingMeOrPet &&
                !unit.IsFriendly &&
                !Styx.Logic.Blacklist.Contains(unit.Guid));

            if (mobList.Count > 0)
            {
                return true;
            }
                return false;

        }
        public static bool HasSheeped()
        {
         
            List<WoWUnit> SheepedList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
unit.Guid != Me.Guid &&
unit.Auras.ContainsKey("Polymorph"));
            if (SheepedList.Count > 0)
            {
                Log("Sheeped Target Detected disabling Frostnova");
                return true;
            }
          
                return false;

        }
        private static bool IsNotWanding
        {
            get
            { 
          
               
                if (Lua.GetReturnVal<int>("return IsAutoRepeatSpell(\"Shoot\")", 0) == 1) { return false; }
                if (Lua.GetReturnVal<int>("return HasWandEquipped()", 0) == 0) { return false; }
                return true;
            }
        }
        private static bool PetActionReady
        {
            get
            {
                
                if (Lua.GetReturnVal<string>("return GetPetActionCooldown(4)", 0) == "0") 
                { 
                    return true; 
                }
                return false;
            }
        }
        
        private void FrostNova()
        {
            switch (AmplifySettings.Instance.FrostNova)
            {
                case "Automatic":
                    BlinkBack();
                    break;

                case "Blink":
                    BlinkBack();
                    break;

                case "Strife":
                    Strafe();
                    break;

                case "BackPeddle":
                    BackPeddle();
                    break;
            }
        }
        public static WoWPoint NavMe
        {
            get
            {
                if (Me.CurrentTarget != null)
                {
                    double Distance = Me.CurrentTarget.Distance + 10;
                    return WoWMathHelper.CalculatePointFrom(Me.Location, Me.CurrentTarget.Location, (float)Distance);
                }
                return Me.Location;
            }
        }

        private void FaceAway()
        {
            float rotation = Me.RotationDegrees + 180;
            Me.SetFacing(WoWMathHelper.DegreesToRadians(rotation));
            Log("Facing Away");
        }
        private void BlinkBack()
        {
        

            if (Me.CurrentTarget != null)
            {
                if (Styx.StyxWoW.GlobalCooldown)
                {
                    while (Styx.StyxWoW.GlobalCooldown)
                    {
                        Thread.Sleep(10);
                    }
                }
                if (IsBattleGround())
                {
                    List<int> IsFacing = new List<int>();
                    List<int> IsNotFacing = new List<int>();
                    IsFacing.Clear();
                    IsNotFacing.Clear();
                    foreach (WoWPlayer Player in ObjectManager.GetObjectsOfType<WoWPlayer>(false))
                    {
                        if (Player.IsAlive && Player.IsHostile && Player.Distance < 50)
                        {
                            if (Me.IsFacing(Player.Location))
                            {
                                IsFacing.Add(1);
                            }
                            else
                            {
                                IsNotFacing.Add(1);
                            }
                        }
                    }
                    if (SpellManager.CanCast("Blink", null))
                    {
                        if (IsFacing.Count >= IsNotFacing.Count)
                        {

                            Log("{0} Players in front of me, {1} Players behind, Blinking Back!", IsFacing.Count.ToString(), IsNotFacing.Count.ToString());
                            float rotation = Me.RotationDegrees + 180;
                            Me.SetFacing(WoWMathHelper.DegreesToRadians(rotation));
                            SpellManager.Cast("Frost Nova");
                            if (Styx.StyxWoW.GlobalCooldown)
                            {
                                while (Styx.StyxWoW.GlobalCooldown)
                                {
                                    Thread.Sleep(10);
                                }
                            }
                            //Thread.Sleep(50);
                            Log("Blink!");
                            SpellManager.Cast("Blink");
                            Styx.StyxWoW.SleepForLagDuration();
                            Me.CurrentTarget.Face();
                            IsFacing.Clear();
                            IsNotFacing.Clear();
                        }
                        else
                        {
                            SpellManager.Cast("Frost Nova");
                            if (Styx.StyxWoW.GlobalCooldown)
                            {
                                while (Styx.StyxWoW.GlobalCooldown)
                                {
                                    Thread.Sleep(10);
                                }
                            }
                            Thread.Sleep(60);
                            Log("More Player behind us, Better Blink Forward!");
                            SpellManager.Cast("Blink");
                            IsFacing.Clear();
                            IsNotFacing.Clear();
                        }
                    }


                }
                if (SpellManager.CanCast("Blink", null))
                {
                    Log("Target Frozen Blinking Away.");
                    float rotation = Me.RotationDegrees + 180;
                    Me.SetFacing(WoWMathHelper.DegreesToRadians(rotation));
                    SpellManager.Cast("Frost Nova");
                    if (Styx.StyxWoW.GlobalCooldown)
                    {
                        while (Styx.StyxWoW.GlobalCooldown)
                        {
                            Thread.Sleep(10);
                        }
                    }
                    //Thread.Sleep(50);
                    Log("Blink!");
                    SpellManager.Cast("Blink");
                    Styx.StyxWoW.SleepForLagDuration();
                    Me.CurrentTarget.Face();
                }

            }
        }

        private void WalkAway()
        {
            if (Me.CurrentTarget != null)
            {
                
                WoWMovement.ClickToMove(WoWMathHelper.CalculatePointBehind(Me.Location, Me.RotationDegrees, 10));
                Log("Waliking away.");
                float rotation = Me.RotationDegrees + 180;
                Me.SetFacing(WoWMathHelper.DegreesToRadians(rotation));
                Thread.Sleep(250);
                WoWMovement.Move(WoWMovement.MovementDirection.Forward);
                Thread.Sleep(1500);
                WoWMovement.MoveStop(WoWMovement.MovementDirection.Forward);
                Me.CurrentTarget.Face();
            }
        }
        private void BackPeddle()
        {
            if (Me.CurrentTarget != null)
            {
                Log("Backpeddle!");
                WoWMovement.Move(WoWMovement.MovementDirection.Backwards);
                Thread.Sleep(1500);
                WoWMovement.MoveStop(WoWMovement.MovementDirection.Backwards);
                Me.CurrentTarget.Face();
            }
        }
        private void Strafe()
        {
            
            WoWMovement.Move(WoWMovement.MovementDirection.StrafeRight);
            WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
            WoWMovement.Face();
            ConeOfCold();
            WoWMovement.MoveStop(WoWMovement.MovementDirection.StrafeRight);
            WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
            Log("Strife Jump");
            if (Me.CurrentTarget != null)
            {
                if (Me.CurrentTarget.BoundingRadius < Me.CurrentTarget.Distance)
                {
                    Log("Im still in hitting Range strife jumping again.");
                    WoWMovement.Move(WoWMovement.MovementDirection.StrafeRight);
                    WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
                    Thread.Sleep(50);
                    WoWMovement.MoveStop(WoWMovement.MovementDirection.StrafeRight);
                    WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
                }
            }
            WoWMovement.Face();
        }
        public bool ColdSnapCheck()
        {
            using (new FrameLock())
            {
                List<int> SpellList = new List<int>();
                SpellList.Clear();
                //Check for Spells on Cooldown And Add to the List if they Are.
                if (SpellManager.HasSpell("Cone of Cold") && SpellManager.Spells["Cone of Cold"].Cooldown)
                {
                    SpellList.Add(1);
                }
                if (SpellManager.HasSpell("Frost Nova") && SpellManager.Spells["Frost Nova"].Cooldown)
                {
                    SpellList.Add(1);
                }
                if (SpellManager.HasSpell("Icy Veins") && SpellManager.Spells["Icy Veins"].Cooldown)
                {
                    SpellList.Add(1);
                }
                if (SpellManager.HasSpell("Ice Barrier") && SpellManager.Spells["Ice Barrier"].Cooldown)
                {
                    SpellList.Add(1);
                }
                if (SpellList.Count > 1)
                {
                    return true;
                }
                return false;
            }
        }


        private void ConeOfCold()
        {
            if (Me.CurrentTarget != null)
            {
                if (Me.GotTarget && SpellManager.CanCast("Cone of Cold"))
                {
                    if (Me.CurrentTarget.Distance < 10)
                    {
                        WoWMovement.Face();
                        Log("Cone of Cold.");
                        SpellManager.Cast("Cone of Cold");
                    }
                }
            }
        }
        public bool HasQuest(int QuestID)
        {
            foreach (int QuestIDs in QuestFrame.Instance.ActiveQuests)
            {
                if (QuestIDs == QuestID)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool PartyNeedInt()
        {
            if (IsBattleGround())
            {
                return false;
            }
            List<int> playerBuffs = new List<int>();
            foreach (WoWPlayer Player in Me.PartyMembers)
            {
                if (!Player.ActiveAuras.ContainsKey("Arcane Brilliance"))
                {
                    playerBuffs.Add(1); 
                }
            }
            if (playerBuffs.Count > 2)
            {
                playerBuffs.Clear();
                return true;
            }
            else
            {
                playerBuffs.Clear();
                return false;
            }
        }

        public static bool PartyHaveCurse()
        {
           
                List<int> playerBuffs = new List<int>();
                foreach (WoWPlayer p in Me.PartyMembers)
                {
                    using (new FrameLock())
            { 
                    if (p.Debuffs.Values.ToList().Exists(aura => aura.Spell.DispelType == WoWDispelType.Curse))
                    {
                        playerBuffs.Add(1);
                    }
                        }
                }
                if (playerBuffs.Count > 0)
                {
                    playerBuffs.Clear();
                    return true;
                }
                else
                {
                    playerBuffs.Clear();
                    return false;
                }
            
        }
        public static void PartyDeCurse()
        {
           
            foreach (WoWPlayer p in Me.PartyMembers)
            {
                using (new FrameLock())
                {
                    if (p.Debuffs.Values.ToList().Exists(aura => aura.Spell.DispelType == WoWDispelType.Curse))
                    {
                        Log("Removing Curse from {0}", p.Name);
                        SpellManager.Cast("Remove Curse", p);
                    }
                }
            }
 
        }


        public static bool MeIsStunned()
        {
            using (new FrameLock())
            {
                if (Me.Debuffs.Values.ToList().Exists(aura => aura.Spell.Mechanic == WoWSpellMechanic.Dazed) ||
                    Me.Debuffs.Values.ToList().Exists(aura => aura.Spell.Mechanic == WoWSpellMechanic.Stunned) ||
                    Me.Debuffs.Values.ToList().Exists(aura => aura.Spell.Mechanic == WoWSpellMechanic.Fleeing) ||
                    Me.Debuffs.Values.ToList().Exists(aura => aura.Spell.Mechanic == WoWSpellMechanic.Charmed) ||
                    Me.Debuffs.Values.ToList().Exists(aura => aura.Spell.Mechanic == WoWSpellMechanic.Asleep))
                {
                    Log("Under Movement Imparing Effect");
                    return true;
                }
                return false;
            }
        }   
        public bool isItemInCooldown(WoWItem item)
        {
            using (new FrameLock())
            {
                if (Equals(null, item))
                    return true;

                string cd_st;
                Lua.DoString("s=GetItemCooldown(" + item.Entry + ")");
                cd_st = Lua.GetLocalizedText("s", ObjectManager.Me.BaseAddress);
                if (cd_st == "0")
                    return false;

                return true;
            }
        }
       
     private WoWItem HaveItemCheck(List<int> listId)
        {
            foreach (WoWItem item in ObjectManager.GetObjectsOfType<WoWItem>(false))
            {
                if (listId.Contains(Convert.ToInt32(item.Entry)))
                {
                   
                    return item;
                }
            }
            return null;
        }

       private WoWItem ManaGem;
       public bool HaveManaGem()
       {
           if (!SpellManager.HasSpell("Conjure Mana Gem"))
           {
               //to help eleaviate low level number crunching by returning false of i dont have the spell.
               return false;
           }
           
           if (ManaGem == null)
           {
               foreach (WoWItem item in Me.BagItems)
               {
                   if (item.Entry == 36799)
                   {
                       ManaGem = item;
                       return true;
                   }

               }
               return false;
           }
           else
           {
               return true;
           }
       }
       
       public bool ManaGemNotCooldown()
       {
           if (ManaGem != null)
           {
               if (ManaGem.Cooldown == 0)
               {
                   return true;
               }

           }
           return false;
       }
        public void UseManaGem()
        {
            if (ManaGem != null && ManaGem.BaseAddress != 0 && ManaGemNotCooldown())
            {
                Log("Popping ManaGem");
                ManaGem.Use();
            }
        }

        private static bool IsBattleGround()
        {
            return Styx.Logic.Battlegrounds.IsInsideBattleground;
        }
        
        public RunStatus PetLogic()
        {
            
                Log("Freezing the Target");
                Lua.DoString("CastPetAction(4)");
                Thread.Sleep(600);
                LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location);
                FreezeTimer.Reset();
                FreezeTimer.Start();
            
                return RunStatus.Success;  
        }
        public RunStatus AoE()
        {
            switch (AmplifySettings.Instance.AoE)
            {
                case "Blizzard":
                    Log("Casting Blizzard");
                    SpellManager.Cast("Blizzard");;
                    break;

                case "Flamestrike":
                    Log("Casting Flamestrike");
                    SpellManager.Cast("Flamestrike");;
                    break;
            }
            Thread.Sleep(500);
            LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location);

            
            return RunStatus.Success;
        }
        /*
         public bool ArcaneBurn()
        {
            if (!SpellManager.Spells["Evocation"].Cooldown && !SpellManager.Spells["Arcane Power"].Cooldown && !SpellManager.Spells["Mirror Image"] && !SpellManager.Item["Mana Gem"].Cooldown)//However you would do that check for mana gem//
            {
                return true;
            }
         }
        */

        //only works with mobs, not players, need a new one for pvp. 
        public static bool WillChain(int Hits, WoWPoint TargetLocal)
        {
            List<WoWUnit> hitList = new List<WoWUnit>();
          
        
                List<WoWUnit> enemyList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
                        unit.Attackable &&
                        !unit.IsFriendly &&
                        !unit.Combat &&
                        unit != Me &&
                        unit != Me.CurrentTarget);
                foreach (WoWUnit Plr in enemyList)
                {
                    if (TargetLocal.Distance(Plr.Location) < 8)
                    {
                        hitList.Add(Plr);
                    }

                }

            if (hitList.Count > Hits)
            {
                Log("Found Targets {0} near CurrentTarget", hitList.Count.ToString());
                hitList.Clear();
                return true;
    


            }
            else
            {
                hitList.Clear();
                return false;
            }
        }






        WoWItem CurrentManaPotion;
        public static ulong LastTargetManaPot;
        public bool HaveManaPotion()
        {
            //whole idea is to make sure CurrentHealthPotion is not null, and to check once every battle. 
            if (CurrentManaPotion == null)
            {
                if (LastTargetManaPot == null || Me.CurrentTarget.Guid != LastTargetManaPot) //Meaning they are not the same. 
                {
                    LastTargetManaPot = Me.CurrentTarget.Guid; // set guid to current target. 
                    List<WoWItem> ManaPot =
                    (from obj in
                         Me.BagItems.Where(
                             ret => ret != null && ret.BaseAddress != 0 &&
                             (ret.ItemInfo.ItemClass == WoWItemClass.Consumable) &&
                             (ret.ItemInfo.ContainerClass == WoWItemContainerClass.Potion) &&
                             (ret.ItemSpells[0].ActualSpell.SpellEffect1.EffectType == WoWSpellEffectType.Energize))
                     select obj).ToList();
                    if (ManaPot.Count > 0)
                    {

                        //on first check, set CurrentHealthPotion so we dont keep running the list looking for one, 
                        CurrentManaPotion = ManaPot.FirstOrDefault();
                        Log("Potion Found {0}", ManaPot.FirstOrDefault().Name);
                        return true;

                    }
                }


                return false;
            }
            else
            {
                return true;
            }
        }
        public bool ManaPotionReady()
        {
            if (CurrentManaPotion != null && CurrentManaPotion.BaseAddress != 0)
            {
                if (CurrentManaPotion.Cooldown == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void UseManaPotion()
        {
            if (CurrentManaPotion != null && CurrentManaPotion.BaseAddress != 0)
            {
                if (CurrentManaPotion.Cooldown == 0)
                {
                    Log("Using {0} to Regen Mana", CurrentManaPotion.Name.ToString());
                    CurrentManaPotion.Use();
                }
            }
        }







        public static ulong LastTargetHPPot;
        WoWItem CurrentHealthPotion;
        public bool HaveHealthPotion()
        {
            //whole idea is to make sure CurrentHealthPotion is not null, and to check once every battle. 
            if (CurrentHealthPotion == null)
            {
                if (LastTargetHPPot == null || Me.CurrentTarget.Guid != LastTargetHPPot) //Meaning they are not the same. 
                {
                    LastTarget = Me.CurrentTarget.Guid; // set guid to current target. 
                    List<WoWItem> HPPot =
                    (from obj in
                         Me.BagItems.Where(
                             ret => ret != null && ret.BaseAddress != 0 &&
                             (ret.ItemInfo.ItemClass == WoWItemClass.Consumable) &&
                             (ret.ItemInfo.ContainerClass == WoWItemContainerClass.Potion) &&
                             (ret.ItemSpells[0].ActualSpell.SpellEffect1.EffectType == WoWSpellEffectType.Heal))
                     select obj).ToList();
                    if (HPPot.Count > 0)
                    {

                        //on first check, set CurrentHealthPotion so we dont keep running the list looking for one, 
                        CurrentHealthPotion = HPPot.FirstOrDefault();
                        Log("Potion Found {0}", HPPot.FirstOrDefault().Name);
                        return true;

                    }
                }


                return false;
            }
            else
            {
                return true;
            }
        }
        public bool HealthPotionReady()
        {
            if (CurrentHealthPotion != null && CurrentHealthPotion.BaseAddress != 0)
            {
                if (CurrentHealthPotion.Cooldown == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void UseHealthPotion()
        {
            if (CurrentHealthPotion != null && CurrentHealthPotion.BaseAddress != 0)
            {
                if (CurrentHealthPotion.Cooldown == 0)
                {
                    Log("Using {0} to Regen Health", CurrentHealthPotion.Name.ToString());
                    CurrentHealthPotion.Use();
                }
            }
        }

        private static WoWPoint _FrostNovaLocal;



    }


    }

