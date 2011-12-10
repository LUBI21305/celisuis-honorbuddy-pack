//CustomClass Template - Created by CodenameGamma
//Replace Layout with the CC name, 
//and WoWClass.Mage with the Class your Designing for.
//Created July, 3rd 2010
//For use with Honorbuddy
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Drawing;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

using TreeSharp;

namespace HandnavisTankadin
{
    class Classname : CombatRoutine
    {
        public override sealed string Name { get { return "Handnavis Tankadin CC V 1.9"; } }

        public override WoWClass Class { get { return WoWClass.Paladin; } }

        private static LocalPlayer Me { get { return ObjectManager.Me; } }




        private void slog(string format, params object[] args) //use for slogging
        {
            Logging.Write(format, args);
        }

        public override void Initialize()
        {
            Logging.Write(Color.Lime, "Handnavis Tankadin CC V 1.9");
        }

        public override bool WantButton
        {
            get
            {

                return true;
            }
        }

        public override void OnButtonPress()
        {

            HandnavisTankadin.HandnavisTankadinConfig f1 = new HandnavisTankadin.HandnavisTankadinConfig();
            f1.ShowDialog();
        }



        #region CC_Begin

        public int lvl = Me.Level;
        public bool kings = false;
        public bool sdm = false;

        public override bool NeedRest
        {
            get
            {
                if (Me.HealthPercent <= 30 || Me.ManaPercent <= 30)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public override void Rest()
        {
            if (Me.HealthPercent <= 30 || Me.ManaPercent <= 30)
            {
                Styx.Logic.Common.Rest.Feed();
            }
        }

        #endregion

        #region Pull

        public override void Pull()
        {
            if ((HandnavisTankadinSettings.Instance.Faceing == 1 && !StyxWoW.Me.IsInInstance) || HandnavisTankadinSettings.Instance.Faceing == 2)
            {
                StyxWoW.Me.CurrentTarget.Face();
            }

                AutoAttack();

                if (HandnavisTankadinSettings.Instance.UseExo)
                {
                    if (CastSpell("Exorcism") == true)
                        Logging.Write(Color.Lime, "Exorcism");
                }
                
                if (CastSpell("Avenger's Shield") == true)
                    Logging.Write(Color.Lime, "Avenger's Shield");

                if (CastSpell("Judgement") == true)
                    Logging.Write(Color.Lime, "Judgement");

                if (CastSpell("Hand of Reckoning") == true)
                    Logging.Write(Color.Lime, "Hand of Reckoning");

                if ((HandnavisTankadinSettings.Instance.Movement == 1 && !StyxWoW.Me.IsInInstance) || HandnavisTankadinSettings.Instance.Movement == 2)
                {
                    while (Me.CurrentTarget.Distance > Me.CurrentTarget.InteractRange)
                    {
                        Navigator.MoveTo(Me.CurrentTarget.Location + 0.9f);
                    }
                    Navigator.PlayerMover.MoveStop();
                }



            
        }


        #endregion

        #region Pull Buffs

        public override bool NeedPullBuffs { get { return false; } }

        public override void PullBuff() { }

        #endregion

        #region Pre Combat Buffs

        public override bool NeedPreCombatBuffs
        {
            get
            {
                if (!HandnavisTankadinSettings.Instance.UseAura)
                {
                    CrusaderAura();
                    DevotionAura();
                }

                RighteousFury();
                CheckSeals();
                BlessingofKings();
                BlessingofMight();
                CheckPartyBuffs();
                return false;
            }
        }

        public override void PreCombatBuff()
        {

        }

        #endregion

        #region Combat Buffs

        public override bool NeedCombatBuffs
        {
            get
            {
                LayonHands();
                DivineShield();
                HolyShield();
                GuardianoftheancientKings();
                ArdentDefender();
                DivineProtection();
                DivinePlea();
                DevotionAura();
                CheckSeals();
                return false;
            }
        }

        public override void CombatBuff()
        {

        }

        #endregion

        #region Heal

        public override bool NeedHeal
        {
            get
            {
                return false;
            }
        }

        public override void Heal()
        {

        }

        #endregion

        #region Falling

        public void HandleFalling() { }

        #endregion

        #region Combat

        public override void Combat()
        {

            if (!StyxWoW.Me.IsInInstance)
            {
                NeedEmergencyHeals();
            }
            if (HandnavisTankadinSettings.Instance.UseTaunt)
            {
                WoWUnit TauntTarget = FindTauntTarget();
               
                //Credits to fiftypence for basic taunt logic
                if (TauntTarget != null)
                {
                    WoWUnit AlliedTauntTarget = TauntTarget.CurrentTarget;
                    StyxWoW.SleepForLagDuration();
                    if (EnemyUnitsTauntable.Count == 1)
                    {
                        if (TauntTarget.Distance2D <= 30
                            && SpellManager.CanCast("Hand of Reckoning"))
                        {
                            SpellManager.Cast("Hand of Reckoning", TauntTarget);
                            Logging.Write(Color.Red, "Hand of Reckoning on " + TauntTarget.Name);
                            return;
                        }
                    }

                    else if (TauntTarget.Distance2D <= 40
                        && SpellManager.CanCast("Righteous Defense", AlliedTauntTarget)
                        && !TauntTarget.HasAura("Hand of Reckoning"))
                    {

                        if (AlliedTauntTarget != null)
                        {
                            SpellManager.Cast("Righteous Defense", AlliedTauntTarget);
                            Logging.Write(Color.Red, "Righteous Defense on " + AlliedTauntTarget.Name);
                            return;
                        }
                    } if (TauntTarget.Distance2D <= 30
                             && SpellManager.CanCast("Hand of Reckoning"))
                    {
                        SpellManager.Cast("Hand of Reckoning", TauntTarget);
                        Logging.Write(Color.Red, "Hand of Reckoning on " + TauntTarget.Name);
                        return;
                    }
                }
            }



            adds = detectAdds();
            saveLives();
            if ((Me.CurrentTarget != null && !Me.CurrentTarget.IsFriendly && Me.CurrentTarget.IsAlive == true && adds.Count == 1 && Me.Mounted == false) /*|| (Me.CurrentTarget != null && !Me.CurrentTarget.IsFriendly && Me.CurrentTarget.IsAlive == true && adds.Count == 2 && Me.Mounted == false)*/) //1-2 Mobs			
            {
                saveLives();

                if ((HandnavisTankadinSettings.Instance.Faceing == 1 && !StyxWoW.Me.IsInInstance) || HandnavisTankadinSettings.Instance.Faceing == 2)
                {
                    StyxWoW.Me.CurrentTarget.Face();
                }

                if (Me.HealthPercent < 40 && Me.CurrentHolyPower == 3)
                {
                    if (CastSpell("Word of Glory") == true)
                    {
                        Logging.Write(Color.Lime, "Word of Glory");
                    }
                }


                if (StyxWoW.Me.Rooted == true)
                {
                    if (CastSpell("Hand of Freedom") == true)
                    {
                        Logging.Write(Color.Lime, "Hand of Freedom");
                    }
                }


                if (Me.CurrentTarget.IsCasting)
                {
                    if (CastSpell("Rebuke") == true)
                    {
                        Logging.Write(Color.Lime, "Rebuke");
                    }
                }

                if (Me.CurrentTarget.Fleeing)
                {
                    if (CastSpell("Hammer of Justice") == true)
                    {
                        Logging.Write(Color.Lime, "Hammer of Justice");
                    }
                }

                if (Me.CurrentTarget.HealthPercent > 50 && (!Me.IsInInstance || Me.CurrentTarget.Elite))
                {
                    if (CastSpell("Avenging Wrath") == true)
                    {
                        Logging.Write(Color.Lime, "Avenging Wrath");
                    }
                }

                if (Me.CurrentHolyPower == 3 && Me.Auras.ContainsKey("Sacred Duty"))
                {
                    if (StyxWoW.Me.Auras["Sacred Duty"].Duration >= 1)
                    {
                        if (CastSpell("Shield of the Righteous") == true)
                        {
                            Logging.Write(Color.Lime, "Shield of Righteous");
                        }
                    }
                }
                if (Me.CurrentHolyPower == 3)
                {
                    if (CastSpell("Shield of the Righteous") == true)
                    {
                        Logging.Write(Color.Lime, "Shield of Righteous");
                    }
                }


                if (CastSpell("Crusader Strike") == true)
                {
                    Logging.Write(Color.Lime, "Crusader Strike");
                }

                if (Me.Auras.ContainsKey("Grand Crusader"))
                {
                    if (StyxWoW.Me.Auras["Grand Crusader"].Duration >= 1)
                    {
                        if (CastSpell("Avenger's Shield") == true)
                        {
                            Logging.Write(Color.Lime, "Avenger's Shield");
                        }
                    }
                }

                if (CastSpell("Judgement") == true)
                {
                    Logging.Write(Color.Lime, "Judgement");
                }

                if (CastSpell("Avenger's Shield") == true)
                {
                    Logging.Write(Color.Lime, "Avenger's Shield");
                }

                if (Me.CurrentTarget.HealthPercent < 20)
                {
                    if (CastSpell("Hammer of Wrath") == true)
                    {
                        Logging.Write(Color.Lime, "Hammer of Wrath");
                    }
                }
                if (Me.ManaPercent > 50 && Me.CurrentTarget.Distance < 10)
                {
                    if (CastSpell("Holy Wrath") == true)
                    {
                        Logging.Write(Color.Lime, "Holy Wrath");
                    }
                }

                if (Me.ManaPercent > 90 && Me.CurrentTarget.Distance < 5)
                {
                    if (CastSpell("Consecration") == true)
                    {
                        Logging.Write(Color.Lime, "Consecration");
                    }
                }

                if ((HandnavisTankadinSettings.Instance.Movement == 1 && !StyxWoW.Me.IsInInstance) || HandnavisTankadinSettings.Instance.Movement == 2)
                {
                    while (Me.CurrentTarget.Distance > Me.CurrentTarget.InteractRange)
                    {
                        Navigator.MoveTo(Me.CurrentTarget.Location + 0.9f);
                    }
                    Navigator.PlayerMover.MoveStop();
                }

            }

            if (Me.CurrentTarget != null && !Me.CurrentTarget.IsFriendly && Me.CurrentTarget.IsAlive == true && adds.Count > 1 && Me.Mounted == false) //3+ Mobs			
            {
                saveLives();
                if ((HandnavisTankadinSettings.Instance.Faceing == 1 && !StyxWoW.Me.IsInInstance) || HandnavisTankadinSettings.Instance.Faceing == 2)
                {
                    StyxWoW.Me.CurrentTarget.Face();
                }

                if (Me.HealthPercent < 40 && Me.CurrentHolyPower == 3)
                {
                    if (CastSpell("Word of Glory") == true)
                    {
                        Logging.Write(Color.Lime, "Word of Glory");
                    }
                }

                if (Me.CurrentTarget.IsCasting)
                {
                    if (CastSpell("Rebuke") == true)
                    {
                        Logging.Write(Color.Lime, "Rebuke");
                    }
                }

                if (Me.CurrentTarget.HealthPercent > 50 && (!Me.IsInInstance || Me.CurrentTarget.Elite))
                {
                    if (CastSpell("Avenging Wrath") == true)
                    {
                        Logging.Write(Color.Lime, "Avenging Wrath");
                    }
                }

                if (Me.CurrentTarget.Fleeing)
                {
                    if (CastSpell("Hammer of Justice") == true)
                    {
                        Logging.Write(Color.Lime, "Hammer of Justice");
                    }
                }

                if (CastSpell("Hammer of the Righteous") == true)
                {
                    Logging.Write(Color.Lime, "Hammer of the Righteous");
                }

                if (!Me.Auras.ContainsKey("Inquisition"))
                {
                    if (CastSpell("Inquisition") == true)
                    {
                        Logging.Write(Color.Lime, "Inquisition");
                    }
                }


                if (Me.ManaPercent > 75 && Me.CurrentTarget.Distance < 5)
                {
                    if (CastSpell("Consecration") == true)
                    {
                        Logging.Write(Color.Lime, "Consecration");
                    }
                }

                if (Me.ManaPercent > 50 && Me.CurrentTarget.Distance < 10)
                {
                    if (CastSpell("Holy Wrath") == true)
                    {
                        Logging.Write(Color.Lime, "Holy Wrath");
                    }
                }

                if (Me.CurrentHolyPower == 3 && Me.Auras.ContainsKey("Inquisition"))
                {
                    if (CastSpell("Shield of the Righteous") == true)
                    {
                        Logging.Write(Color.Lime, "Shield of Righteous");
                    }
                }

                if (CastSpell("Avenger's Shield") == true)
                {
                    Logging.Write(Color.Lime, "Avenger's");
                }

                if (Me.CurrentTarget.HealthPercent < 20)
                {
                    if (CastSpell("Hammer of Wrath") == true)
                    {
                        Logging.Write(Color.Lime, "Hammer of Wrath");
                    }
                }

                if ((HandnavisTankadinSettings.Instance.Movement == 1 && !StyxWoW.Me.IsInInstance) || HandnavisTankadinSettings.Instance.Movement == 2)
                {
                    while (Me.CurrentTarget.Distance > Me.CurrentTarget.InteractRange)
                    {
                        Navigator.MoveTo(Me.CurrentTarget.Location + 0.9f);
                    }
                    Navigator.PlayerMover.MoveStop();
                }
            }

        }

        #endregion



        #region Spells


        private void AutoAttack()
        {
            if (!Me.IsAutoAttacking)
            {
                Lua.DoString("StartAttack()");
            }

        }
        #endregion

        #region CastSpell Method
        // Credit to Apoc for the below CastSpell code
        // Used for calling CastSpell in the Combat Rotation


        //Credit to Wulf!
        public bool CastSpell(string spellName, WoWPlayer target)
        {
            if (SpellManager.CanCast(spellName, target) && !SpellManager.GlobalCooldown && !Me.IsCasting)
            {
                if (target == Me)
                {
                    //SpellManager.Cast(spellName, Me);
                    return false;
                }
                else
                {
                    SpellManager.Cast(spellName, target);
                    return true;
                }
            }
            return false;
        }


        public bool CastSpell(string spellName)
        {
            if (SpellManager.CanCast(spellName) && !SpellManager.GlobalCooldown && !Me.IsCasting)
            {
                SpellManager.Cast(spellName);
                // We managed to cast the spell, so return true, saying we were able to cast it.
                return true;
            }
            // Can't cast the spell right now, so return false.
            return false;
        }
        #endregion

        #region Add Detection
        //Credit to CodeNameGamma for detectAdds code
        private List<WoWUnit> adds = new List<WoWUnit>();

        private List<WoWUnit> detectAdds()
        {
            List<WoWUnit> addList = ObjectManager.GetObjectsOfType<WoWUnit>(false).FindAll(unit =>
                        unit.Guid != Me.Guid &&
                        unit.Distance < 10.00 &&
                        (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember || unit.IsTargetingMeOrPet) &&
                        !unit.IsFriendly &&
                        !unit.IsPet &&
                        !Styx.Logic.Blacklist.Contains(unit.Guid));

            if (addList.Count > 2)
            {
                //Logging.Write(Color.Orange, "Detected " + addList.Count.ToString() + " adds! Switchting to AoE Rank mode!");
            }
            return addList;
        }
        #endregion


        #region Crusader Aura

        public bool CrusaderAura()
        {
            if (!Me.Auras.ContainsKey("Crusader Aura") && StyxWoW.Me.Mounted)
            {
                if (CastSpell("Crusader Aura") == true)
                {
                    Logging.Write(Color.Lime, "Crusader Aura");
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region Devotion Aura

        public bool DevotionAura()
        {
            if (!Me.Auras.ContainsKey("Devotion Aura") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Devotion Aura") == true)
                {
                    Logging.Write(Color.Lime, "Devotion Aura");
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region Righteous Fury

        public bool RighteousFury()
        {
            if (!Me.Auras.ContainsKey("Righteous Fury") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Righteous Fury") == true)
                {
                    Logging.Write(Color.Lime, "Righteous Fury");
                    return true;
                }
            }
            return false;
        }
        #endregion





        #region Check Seals

        public bool CheckSeals()
        {
            if (HandnavisTankadinSettings.Instance.Seal == 0)
            {
                SealofTruth();
            }
            if (HandnavisTankadinSettings.Instance.Seal == 1)
            {
                SealofRighteousness();
            }
            if (HandnavisTankadinSettings.Instance.Seal == 2)
            {
                SealofInsight();
            }

            if (HandnavisTankadinSettings.Instance.Seal == 3)
            {
                if (adds.Count < 3)
                {
                    SealofTruth();
                }
                if (adds.Count >= 3)
                {
                    SealofRighteousness();
                }
            }

            return false;
        }
        #endregion



        #region Seal of Truth

        public bool SealofTruth()
        {
            if (!Me.Auras.ContainsKey("Seal of Truth") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Seal of Truth") == true)
                {
                    Logging.Write(Color.Lime, "Seal of Truth");
                    return true;
                }
            }
            return false;
        }
        #endregion



        #region Seal of Righteousness

        public bool SealofRighteousness()
        {
            if (!Me.Auras.ContainsKey("Seal of Righteousness") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Seal of Righteousness") == true)
                {
                    Logging.Write(Color.Lime, "Seal of Righteousness");
                    return true;
                }
            }
            return false;
        }
        #endregion



        #region Seal of Insight

        public bool SealofInsight()
        {
            if (!Me.Auras.ContainsKey("Seal of Insight") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Seal of Insight") == true)
                {
                    Logging.Write(Color.Lime, "Seal of Insight");
                    return true;
                }
            }
            return false;
        }
        #endregion



        #region Blessing of Kings

        public bool BlessingofKings()
        {
            if (Me.Level > lvl)
            {
                if (CastSpell("Blessing of Kings") == true)
                {
                    Logging.Write(Color.Lime, "Leveled up! Recast Blessing of Kings");
                    lvl = Me.Level;
                    return true;
                }
            }

            if (!Me.Auras.ContainsKey("Blessing of Kings") && !Me.Auras.ContainsKey("Mark of the Wild") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Blessing of Kings") == true)
                {
                    Logging.Write(Color.Lime, "Blessing of Kings");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Blessing of Might

        public bool BlessingofMight()
        {
            if (Me.Level > lvl)
            {
                if (CastSpell("Blessing of Might") == true)
                {
                    Logging.Write(Color.Lime, "Leveled up! Recast Blessing of Might");
                    lvl = Me.Level;
                    return true;
                }
            }
            if (!Me.Auras.ContainsKey("Blessing of Might") && Me.Auras.ContainsKey("Mark of the Wild") && !StyxWoW.Me.Mounted)
            {
                if (CastSpell("Blessing of Might") == true)
                {
                    Logging.Write(Color.Lime, "Blessing of Might");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region CheckPartyBuffs

        public bool CheckPartyBuffs()
        {
            foreach (WoWPlayer d in Me.PartyMembers)
            {
                double friendHp = d.HealthPercent;

                if (!d.Auras.ContainsKey("Blessing of Might") && d.IsAlive && d.Distance < 30)
                {
                    if (Me.Auras.ContainsKey("Blessing of Might") && Me.Auras["Blessing of Might"].CreatorGuid == Me.Guid)
                    {
                        if (CastSpell("Blessing of Might") == true)
                        {
                            Logging.Write(Color.Lime, "Blessing of Might on Partymember");
                            return true;
                        }

                    }
                }


                if (!d.Auras.ContainsKey("Blessing of Kings") && d.IsAlive && d.Distance < 30)
                {
                    if (Me.Auras.ContainsKey("Blessing of Kings") && Me.Auras["Blessing of Kings"].CreatorGuid == Me.Guid)
                    {
                        if (CastSpell("Blessing of Kings") == true)
                        {
                            Logging.Write(Color.Lime, "Blessing of Kings on Partymember");
                            return true;
                        }

                    }
                }

            } return false;

        }
        #endregion


        #region Holy Shield

        public bool HolyShield()
        {
            if (Me.HealthPercent < HandnavisTankadinSettings.Instance.HolyShieldPercent)
            {
                if (CastSpell("Holy Shield") == true)
                {
                    Logging.Write(Color.Lime, "Holy Shield");
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Lay on Hands
        public bool LayonHands()
        {
            if (Me.HealthPercent < HandnavisTankadinSettings.Instance.LayonHandsPercent && !StyxWoW.Me.HasAura("Forbearance") && !StyxWoW.Me.HasAura("Ardent Defender"))
            {
                if (CastSpell("Lay on Hands") == true)
                {
                    Logging.Write(Color.Lime, "Lay on Hands");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Divine Shield
        public bool DivineShield()
        {
            if (Me.HealthPercent < 15 && !StyxWoW.Me.HasAura("Forbearance") && !Me.IsInInstance)
            {
                if (CastSpell("Divine Shield") == true)
                {
                    Logging.Write(Color.Lime, "Divine Shield");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Divine Protection
        public bool DivineProtection()
        {
            if (Me.HealthPercent < HandnavisTankadinSettings.Instance.DivineProtectionPercent)
            {
                if (CastSpell("Divine Protection") == true)
                {
                    Logging.Write(Color.Lime, "Divine Protection");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Ardent Defender
        public bool ArdentDefender()
        {
            if (Me.HealthPercent < HandnavisTankadinSettings.Instance.ArdentDefenderPercent)
            {
                if (CastSpell("Ardent Defender") == true)
                {
                    Logging.Write(Color.Lime, "Ardent Defender");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Guardian of the ancient Kings
        public bool GuardianoftheancientKings()
        {
            if (Me.HealthPercent < HandnavisTankadinSettings.Instance.GuardianoftheancientKingsPercent)
            {
                if (CastSpell("Guardian of Ancient Kings") == true)
                {
                    Logging.Write(Color.Lime, "Guardian of Ancient Kings");
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region Divine Plea
        public bool DivinePlea()
        {
            if (Me.ManaPercent < HandnavisTankadinSettings.Instance.DivinePleaPercent && Me.HealthPercent > 50 || (HandnavisTankadinSettings.Instance.DivinePleaCD && adds.Count >= 3))
            {
                if (CastSpell("Divine Plea") == true)
                {
                    Logging.Write(Color.Lime, "Divine Plea");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Need Emergency Heal
        public bool NeedEmergencyHeals()
        {

            if (Me.HealthPercent < 50 && Me.CurrentHolyPower == 3)
            {
                if (CastSpell("Word of Glory") == true)
                {
                    Logging.Write(Color.Lime, "Word of Glory");
                    return true;
                }
            }
            if (Me.HealthPercent < 40)
            {
                if (CastSpell("Flash of Light") == true)
                {
                    if (CastSpell("Hammer of Justice") == true)
                    {
                        Logging.Write(Color.Lime, "Emergency! Stun Target");
                    }
                    Logging.Write(Color.Lime, "Flash of Light");
                    return true;
                }
            }
            return false;
        }
        #endregion

        //Credit to Wulf!
        #region saveLives
        private void saveLives()
        {
            foreach (WoWPartyMember e in Me.PartyMemberInfos)
            {
                foreach (WoWPlayer d in Me.PartyMembers)
                {
                    double friendHp = d.HealthPercent;
                    double friendMp = d.ManaPercent;
                    double friendMax = d.MaxMana;

                    if (HandnavisTankadinSettings.Instance.UseLay == true)
                    {
                        if (friendHp < 15 && friendMp > 25 && friendMax > Me.MaxMana * 3 && !d.Auras.ContainsKey("Forbearance") && Me.HealthPercent > 40)
                        {
                            if (CastSpell("Lay on Hands", d) == true)
                                Logging.Write(Color.Orange, "[+] Lay on Hands on " + d);
                        }
                    }

                    if (friendHp < 30 && Me.HealthPercent > 40 && d.Combat && d.Distance < 30)
                    {
                        if (CastSpell("Hand of Sacrifice", d) == true)
                            Logging.Write(Color.Orange, "[+] HoS on " + d);
                    }
                    if (friendHp < 20 && Me.HealthPercent > 40 && d.Combat && d.Distance < 30)
                    {
                        if (e.Role == WoWPartyMember.GroupRole.Tank)
                            if (!d.Auras.ContainsKey("Forbearance"))
                            {
                                if (CastSpell("Hand of Protection", d) == true)
                                    Logging.Write(Color.Orange, "[+] HoP on " + d);
                            }
                    }
                }
            }
        }
        #endregion



        //Credits to fiftypence for basic taunt logic
        public List<WoWUnit> EnemyUnitsTauntable
        {
            get
            {
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                    .Where(unit =>
                        !unit.IsFriendly
                        && unit.IsTargetingMyPartyMember
                        && unit.IsAlive
                        && unit.ThreatInfo.RawPercent < 100
                        && unit.MaxHealth < Me.MaxHealth * 3
                        && unit.Elite
                        && !unit.IsTargetingMeOrPet
                        && !unit.IsPet
                        && !unit.IsNonCombatPet
                        && !unit.IsCritter
                        && unit.DistanceSqr
                    <= 80 * 80).ToList();
            }
        }

        public WoWUnit FindTauntTarget()
        {
            return (from unit in EnemyUnitsTauntable
                    orderby unit.DistanceSqr descending
                    select unit).FirstOrDefault();
        }

/*        private bool IsHealer(WoWPlayer p)
        {
            if (p != null)
            {
                return Lua.GetReturnValues("return UnitGroupRolesAssigned('" + DeUnicodify(p.Name) + "')").First() == "HEALER";
            }
            else return false;
        }
        */


    }
}