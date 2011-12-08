using Styx;
using Styx.Combat.CombatRoutine;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Hera.Helpers
{
    public static class CLC
    {
        //*********************************************************************************************************
        // 
        // This is what I call my "Common Language Configuration" system.
        // It takes common language terms and makes a TRUE/FALSE setting from it.
        // I'm sure there are more elegant ways of undertaking this but it suites my purposes perfectly.
        //
        // You pass a RAW line of string and simply check if a phrase or keyword is present
        // Its a lot more intuitive for the user and it gives me more control over the UI
        // This way I don't need hundreds of tick boxes or controls
        // 
        //*********************************************************************************************************
       

        /// <summary>
        /// This is the property you assign the raw 'setting' to. The raw setting is the value passed from the Settings.XXXX property
        /// Eg CLC.RawSetting = Settings.Cleanse; Checking CLC.AfterCombatEnds you will see it returns TRUE
        /// </summary>
        public static string RawSetting { get; set; }
        private static LocalPlayer Me { get { return ObjectManager.Me; } }

        // Call these properties from the CC in order to check if a condition is met. 
        public static bool OnAdds { get { return RawSetting.Contains("only on adds"); } }
        public static bool NoAdds { get { return RawSetting.Contains("only when no adds"); } }
        public static bool Always { get { return RawSetting.Contains("always"); } }
        public static bool Never { get { return RawSetting.Contains("never"); } }
        public static bool OnRunners { get { return RawSetting.Contains("on runners") || RawSetting.Contains("and runners"); } }
        public static bool IfCasting { get { return RawSetting.Contains("on casters") || RawSetting.Contains("casters and"); } }
        public static bool IfCastingOrRunning { get { return RawSetting.Contains("casters and runners"); } }
        public static bool OutOfCombat { get { return RawSetting.Contains("out of combat") || RawSetting.Contains("during pull"); } }
        public static bool Immediately { get { return RawSetting.Contains("immediately"); } }
        public static bool InBGs { get { return RawSetting.Contains("only in battleground"); } }
        public static bool InInstances { get { return RawSetting.Contains("only in instance"); } }
        public static bool NotInInstances { get { return RawSetting.Contains("not in instance"); } }
        public static bool OnHumanoids { get { return RawSetting.Contains("on humanoids"); } }
        public static bool OnFleeing { get { return RawSetting.Contains("on fleeing"); } }
        public static bool NotLowLevel { get { return RawSetting.Contains("not low level"); } }
        public static bool LowHealth { get { return RawSetting.Contains("on low health"); } }

        // Adds
        public static bool Adds2OrMore { get { return RawSetting.Contains("on 2+ adds"); } }
        public static bool Adds3OrMore { get { return RawSetting.Contains("on 3+ adds"); } }
        public static bool Adds4OrMore { get { return RawSetting.Contains("on 4+ adds"); } }


        // Combo points - Rogues and Druids (Cat)
        public static bool ComboPoints1OrMore { get { return RawSetting.Contains("1+ combo"); } }
        public static bool ComboPoints2OrMore { get { return RawSetting.Contains("2+ combo"); } }
        public static bool ComboPoints3OrMore { get { return RawSetting.Contains("3+ combo"); } }
        public static bool ComboPoints4OrMore { get { return RawSetting.Contains("4+ combo"); } }
        public static bool ComboPoints5OrMore { get { return RawSetting.Contains("5+ combo"); } }

        // Holy Power - Paladin only
        public static bool HolyPower1OrMore { get { return RawSetting.Contains("1+ Holy Power"); } }
        public static bool HolyPower2OrMore { get { return RawSetting.Contains("2+ Holy Power"); } }
        public static bool HolyPower3OrMore { get { return RawSetting.Contains("3+ Holy Power"); } }

        // Hunter Focus Fire Frenzy Stacks
        public static bool FocusFire1OrMore { get { return RawSetting.Contains("1+ Frenzy"); } }
        public static bool FocusFire2OrMore { get { return RawSetting.Contains("2+ Frenzy"); } }
        public static bool FocusFire3OrMore { get { return RawSetting.Contains("3+ Frenzy"); } }
        public static bool FocusFire4OrMore { get { return RawSetting.Contains("4+ Frenzy"); } }
        public static bool FocusFire5OrMore { get { return RawSetting.Contains("5+ Frenzy"); } }

        // Hunter Focus 
        public static bool Focus2OorMore { get { return RawSetting.Contains("focus 20+"); } }
        public static bool Focus25orMore { get { return RawSetting.Contains("focus 25+"); } }
        public static bool Focus3OorMore { get { return RawSetting.Contains("focus 30+"); } }
        public static bool Focus35orMore { get { return RawSetting.Contains("focus 35+"); } }
        public static bool Focus4OorMore { get { return RawSetting.Contains("focus 40+"); } }
        public static bool Focus45orMore { get { return RawSetting.Contains("focus 45+"); } }
        public static bool Focus5OorMore { get { return RawSetting.Contains("focus 50+"); } }
        public static bool Focus55orMore { get { return RawSetting.Contains("focus 55+"); } }
        public static bool Focus6OorMore { get { return RawSetting.Contains("focus 60+"); } }
        public static bool Focus65orMore { get { return RawSetting.Contains("focus 65+"); } }
        public static bool Focus7OorMore { get { return RawSetting.Contains("focus 70+"); } }
        public static bool Focus75orMore { get { return RawSetting.Contains("focus 75+"); } }
        public static bool Focus8OorMore { get { return RawSetting.Contains("focus 80+"); } }
        public static bool Focus85orMore { get { return RawSetting.Contains("focus 85+"); } }
        public static bool Focus9OorMore { get { return RawSetting.Contains("focus 90+"); } }
        public static bool Focus95orMore { get { return RawSetting.Contains("focus 95+"); } }

        // Health
        public static bool HealthLessThan90 { get { return RawSetting.Contains("health below 90"); } }
        public static bool HealthLessThan80 { get { return RawSetting.Contains("health below 80"); } }
        public static bool HealthLessThan70 { get { return RawSetting.Contains("health below 70"); } }
        public static bool HealthLessThan60 { get { return RawSetting.Contains("health below 60"); } }
        public static bool HealthLessThan50 { get { return RawSetting.Contains("health below 50"); } }
        public static bool HealthLessThan40 { get { return RawSetting.Contains("health below 40"); } }
        public static bool HealthLessThan30 { get { return RawSetting.Contains("health below 30"); } }
        public static bool HealthLessThan20 { get { return RawSetting.Contains("health below 20"); } }
        
        public static bool IsOkToRun
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(RawSetting)) return false;                                                 // No string passed so nothing to check
                    if (Always || Immediately) return true;                                                             // Always means always
                    if (Never) return false;                                                                            // No means no! You men are all the same

                    if (OutOfCombat && !Me.Combat) return true;                                                         // Only if we're not in combat
                    if (OnAdds && Utils.Adds && Me.Combat) return true;                                                               // Only if we have adds
                    if (NoAdds && !Utils.Adds && Me.Combat) return true;                                                               // Only if we DON'T have adds
                    if (Me.GotTarget && IfCastingOrRunning && (Me.CurrentTarget.IsCasting || Me.CurrentTarget.Fleeing)) return true;    // Only if the target is casting or running
                    if (OnRunners && Me.GotTarget && Me.CurrentTarget.Fleeing) return true;                                              // Only if target (NPC) is running away
                    if (IfCasting && Me.GotTarget && Me.CurrentTarget.IsCasting) return true;                                            // Only if target is casting
                    if (InInstances && Me.IsInInstance) return true;                                                    // Only if you are inside an instance
                    if (InBGs && Utils.IsBattleground) return true;                                                     // Only if you are inside a battleground
                    if (OnHumanoids && Me.GotTarget && Me.CurrentTarget.CreatureType == WoWCreatureType.Humanoid) return true;  // Humanoids only. Mostly for runners
                    if (OnFleeing && Me.GotTarget && Me.CurrentTarget.Fleeing) return true;  // Is target fleeing
                    if (NotInInstances && !Me.IsInInstance) return true;
                    if (NotLowLevel && Me.GotTarget && !Target.IsLowLevel) return true;
                    
                    // Adds 2+
                    if (Adds2OrMore && Utils.AddsCount >= 2) return true;
                    if (Adds3OrMore && Utils.AddsCount >= 3) return true;
                    if (Adds4OrMore && Utils.AddsCount >= 4) return true;
                    

                    // Health < X
                    if (RawSetting.Contains("health below"))
                    {
                        if (HealthLessThan90 && Me.HealthPercent < 90) return true;
                        if (HealthLessThan80 && Me.HealthPercent < 80) return true;
                        if (HealthLessThan70 && Me.HealthPercent < 70) return true;
                        if (HealthLessThan60 && Me.HealthPercent < 60) return true;
                        if (HealthLessThan50 && Me.HealthPercent < 50) return true;
                        if (HealthLessThan40 && Me.HealthPercent < 40) return true;
                        if (HealthLessThan30 && Me.HealthPercent < 30) return true;
                        if (HealthLessThan20 && Me.HealthPercent < 20) return true;
                    }

                    if (Me.Class == WoWClass.Druid)
                    {
                        
                    }

                    
                    // If you are not a Rogue or a Druid (Cat) then don't do these checks
                    if (Me.Class == WoWClass.Rogue || (Me.Class == WoWClass.Druid && Me.Shapeshift == ShapeshiftForm.Cat))
                    {
                        if (Me.ComboPoints <= 0) return false;
                        if (ComboPoints5OrMore && Me.ComboPoints >= 5) return true;
                        if (ComboPoints4OrMore && Me.ComboPoints >= 4) return true;
                        if (ComboPoints3OrMore && Me.ComboPoints >= 3) return true;
                        if (ComboPoints2OrMore && Me.ComboPoints >= 2) return true;
                        if (ComboPoints1OrMore && Me.ComboPoints >= 1) return true;
                        
                    }

                    if (Me.Class == WoWClass.Paladin)
                    {
                        //if (Me.CurrentHolyPower <= 0) return false;
                        if (HolyPower1OrMore && Me.CurrentHolyPower >= 1) return true;
                        if (HolyPower2OrMore && Me.CurrentHolyPower >= 2) return true;
                        if (HolyPower3OrMore && Me.CurrentHolyPower >= 3) return true;

                        // Other Misc
                        if (RawSetting.Contains("Sacred Duty") && IsBuffPresent("Sacred Duty")) return true;
                    }

                    if (Me.Class == WoWClass.Hunter)
                    {
                        if (Focus2OorMore && Me.FocusPercent >= 20) return true;
                        if (Focus25orMore && Me.FocusPercent >= 25) return true;
                        if (Focus3OorMore && Me.FocusPercent >= 30) return true;
                        if (Focus35orMore && Me.FocusPercent >= 35) return true;
                        if (Focus4OorMore && Me.FocusPercent >= 40) return true;
                        if (Focus45orMore && Me.FocusPercent >= 45) return true;
                        if (Focus5OorMore && Me.FocusPercent >= 50) return true;
                        if (Focus55orMore && Me.FocusPercent >= 55) return true;
                        if (Focus6OorMore && Me.FocusPercent >= 60) return true;
                        if (Focus65orMore && Me.FocusPercent >= 65) return true;
                        if (Focus7OorMore && Me.FocusPercent >= 70) return true;
                        if (Focus75orMore && Me.FocusPercent >= 75) return true;
                        if (Focus8OorMore && Me.FocusPercent >= 80) return true;
                        if (Focus85orMore && Me.FocusPercent >= 85) return true;
                        if (Focus9OorMore && Me.FocusPercent >= 90) return true;
                        if (Focus95orMore && Me.FocusPercent >= 95) return true;

                        if (FocusFire1OrMore && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy Effect") && Me.Pet.Auras["Frenzy Effect"].StackCount >= 1) return true;
                        if (FocusFire2OrMore && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy Effect") && Me.Pet.Auras["Frenzy Effect"].StackCount >= 2) return true;
                        if (FocusFire3OrMore && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy Effect") && Me.Pet.Auras["Frenzy Effect"].StackCount >= 3) return true;
                        if (FocusFire4OrMore && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy Effect") && Me.Pet.Auras["Frenzy Effect"].StackCount >= 4) return true;
                        if (FocusFire5OrMore && Me.GotAlivePet && Me.Pet.Auras.ContainsKey("Frenzy Effect") && Me.Pet.Auras["Frenzy Effect"].StackCount >= 5) return true;
                    }
                }

                catch (System.Exception e)
                {
                    System.Reflection.MethodBase currMethod = System.Reflection.MethodBase.GetCurrentMethod();
                    Debug.ModuleName = currMethod.Name;
                    Debug.Catch(e);
                }

                return false;   // Otherwise its not going to happen
            }

        }

        public static bool ResultOK(string clcSettingString)
        {
            RawSetting = clcSettingString;
            bool result = IsOkToRun;
            return result;
        }


        private static bool IsBuffPresent(string buffToCheck)
        {
            Lua.DoString(string.Format("buffName,_,_,stackCount,_,_,_,_,_=UnitBuff(\"player\",\"{0}\")", buffToCheck));
            string buffName = Lua.GetLocalizedText("buffName", Me.BaseAddress);
            return (buffName == buffToCheck);
        }
    }
}
