using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Amplify.Talents;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;
using Color = System.Drawing.Color;
using Sequence = TreeSharp.Sequence;

namespace Amplify
{
    public partial class Amplify : CombatRoutine
    {
        private readonly Version _version = new Version(1, 8, 3);
        private readonly TalentManager _talentManager = new TalentManager();
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        public override string Name { get { return "Amplify Elite" + _version; } }
        public override WoWClass Class { get { return WoWClass.Mage; } }
        public new double? PullDistance
        {
            get
            {
                if (SpellManager.CanCast("Arcane Missiles") && Me.Auras.ContainsKey("Arcane Missiles!") && AmplifySettings.Instance.Pull_ProcArcaneMissles)
                {
                    return SpellManager.Spells["Arcane Missiles"].MaxRange;
                }
                else
                if (!SpellManager.HasSpell("Frostbolt"))
                {
                    return SpellManager.Spells["Fireball"].MaxRange;
                }
                else
                if (SpellManager.HasSpell(CurrentPullSpell))
                {
                    return SpellManager.Spells[CurrentPullSpell].MaxRange;
                }
                else
               return 30;
 
            }
        }
 
        public void DisablePlugin(string name)
        {
            foreach (Styx.Plugins.PluginContainer plugin in Styx.Plugins.PluginManager.Plugins)
            {
                if (plugin.Name == name)
                {
                    plugin.Enabled = false;
                    Logging.Write("{0} Disabled", plugin.Name);
                }
            }
        }
        public Stopwatch StopSpellTimer = new Stopwatch();

        public override void Pulse()
        {
 
            if (IsBattleGround() && Me.IsActuallyInCombat && Me.Mounted)
            {
                Log("Im in Combat Dismouting");
                Mount.Dismount();
                FindClostestPlayer(30);
            }
            if (!AmplifySettings.Instance.MoveDisable && Me.CurrentTarget == null)
            {
               
                if (Me.GotAlivePet && Me.CurrentTarget == null && Me.Pet.CurrentTarget != null)
                {

                        Logging.Write("Pet Has Agro From {0}", Me.Pet.CurrentTarget.Name);
                        Logging.Write("Trying to Initiate Combat with Pets Current Target");
                        Me.Pet.CurrentTarget.Target();
                        CreatePullBehavior();
                    
                }
            }

         
        }

        public override void Initialize()
        {
            Targeting.Instance.IncludeTargetsFilter += IncludeTargetsFilter;
            AmplifySettings.Instance.Load();
            Log("Loading Saved Setttings");
            Lua.Events.AttachEvent("COMBAT_LOG_EVENT", CombatLogEventHander);
            Lua.Events.AttachEvent("UI_ERROR_MESSAGE", UIErrorEventHander);
            if(AmplifySettings.Instance.IsConfigured == false)
            {
                Logging.Write(Color.Yellow,
                              "You have not configured Amplify, Configuration is needed for this CC to work, Please Configure your Settings now");
            }
            Log("Charactor is a level {0}, {1}, Mage.", Me.Level.ToString(), Me.Race.ToString());
            switch (_talentManager.Spec)
            {
                case MageTalentSpec.Frost:
                Logging.Write(Color.LightBlue,"Current Spec is FROST");
                break;

                case MageTalentSpec.Arcane:
                Logging.Write(Color.LightPink, "Current Spec is Arcane!");
                break;

                case MageTalentSpec.Fire:
                Logging.Write(Color.LightCyan, "Current Spec is FIRE!");
                break;

                case MageTalentSpec.Lowbie:
                Logging.Write(Color.LightGreen, "Mage is Less then Level 10, No Current Spec");
                break;

            }
                
        }
        private static void UIErrorEventHander(object sender, LuaEventArgs args)
        {
            string Error = args.Args[0].ToString();

            if (Error == "Target not in line of sight" || Error == "Ziel ist nicht im Sichtfeld")
            {
                if (!AmplifySettings.Instance.MoveDisable && Me.CurrentTarget != null && Navigator.CanNavigateFully(Me.Location, Me.CurrentTarget.Location))
                {
                    Logging.Write("Target not in Line of Sight to {0} Blacklisting for 10 sec to allow Repositioning. ", Me.CurrentTarget.Name);
                    Blacklist.Add(StyxWoW.Me.CurrentTargetGuid, TimeSpan.FromSeconds(10));
                    StyxWoW.Me.ClearTarget();
                }
                if (!AmplifySettings.Instance.MoveDisable && Me.CurrentTarget != null && !Navigator.CanNavigateFully(Me.Location, Me.CurrentTarget.Location))
                {
                    Logging.Write("Cant Navigate to {0} Blacklisting for 3min", Me.CurrentTarget.Name);
                    Blacklist.Add(StyxWoW.Me.CurrentTargetGuid, TimeSpan.FromMinutes(3));
                    StyxWoW.Me.ClearTarget();

                }
            }
        }

        public static ulong LastTarget;
        public static int EvadeCount;
        private static void CombatLogEventHander(object sender, LuaEventArgs args)
        {
            foreach (object arg in args.Args)
            {
                if (arg is String)
                {
                    var s = (string)arg;
                    if (s.ToUpper() == "EVADE" || s.ToUpper() == "ENTKOMMEN")
                    {
                        if (!AmplifySettings.Instance.MoveDisable)
                        {
                            if (StyxWoW.Me.GotTarget && !IsBattleGround())
                            {
                                if (LastTarget == null || Me.CurrentTarget.Guid != LastTarget) //Meaning they are not the same. 
                                {
                                    LastTarget = Me.CurrentTarget.Guid; // set guid to current target. 
                                    EvadeCount = 1; //it didnt match last target and already evaded once. 
                                    Logging.Write("Target Evaded {0} Times", EvadeCount.ToString());
                                }
                                else
                                {
                                    EvadeCount++;
                                    Logging.Write("Target Evaded {0} Times", EvadeCount.ToString());
                                    if (EvadeCount >= 3)
                                    {
                                        Logging.Write("Target Evaded {0} Times", EvadeCount.ToString());
                                        Logging.Write("Target is Evade bugged.");
                                        Logging.Write("Blacklisting for 3 hours");
                                        Blacklist.Add(StyxWoW.Me.CurrentTargetGuid, TimeSpan.FromHours(3));
                                        StyxWoW.Me.ClearTarget();
                                    }
                                }

                            }
                            else
                                if (StyxWoW.Me.GotTarget && (IsBattleGround() || !Me.IsInInstance))
                                {
                                    Logging.Write("My target is Evade bugged.");
                                    Logging.Write("Blacklisting for 1 Minute");
                                    Blacklist.Add(StyxWoW.Me.CurrentTargetGuid, TimeSpan.FromMinutes(1));
                                    StyxWoW.Me.ClearTarget();
                                }
                        }
                    }
                }
            }
        }
        public override bool WantButton { get { return true; } }
    
        private Form _configForm;
        public override void OnButtonPress()
        {
            if (_configForm == null || _configForm.IsDisposed || _configForm.Disposing)
                _configForm = new AmpConfig();

            _configForm.ShowDialog();
        }

    }

    public partial class Amplify
    {
        private static int Christmas = 3;
        private static string _logSpam;
        private static void Log(string format, params object[] args)
        {
            string s = Utilities.FormatString(format, args);
           
            if (s != _logSpam)
            {

                Logging.Write(Color.LightBlue, "[Amplify]: {0}", string.Format(format, args));
                //in the future add color change per spec.
                
                _logSpam = s;
            }
        }
      
        private static void Log(string format)
        {
            Log(format, new object());
        }

        #region Behavior Tree Composite Helpers

        public delegate WoWUnit UnitSelectDelegate(object context);
        public Composite CreateBuffCheckAndCast(string name)
        {
            return new Decorator(ret => SpellManager.CanBuff(name),
                                 new Action(ret => SpellManager.Buff(name)));
        }

        public Composite CreateBuffCheckAndCast(string name, UnitSelectDelegate onUnit)
        {
            return new Decorator(ret => SpellManager.CanBuff(name, onUnit(ret)),
                                 new Action(ret => SpellManager.Buff(name, onUnit(ret))));
        }

        public Composite CreateBuffCheckAndCast(string name, CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret) && SpellManager.CanBuff(name),
                                 new Action(ret => SpellManager.Buff(name)));
        }
        public Composite CreateBuffCheckAndCast(string name, bool OnMe)
        {
            return new Decorator(ret => !Me.Auras.ContainsKey(name),
                                  new Action(ret => SpellManager.Buff(name)));
        }
        public Composite CreateBuffCheckAndCast(string name, UnitSelectDelegate onUnit, CanRunDecoratorDelegate extra)
        {
            return CreateBuffCheckAndCast(name, onUnit, extra, false);
        }

        public Composite CreateBuffCheckAndCast(string name, UnitSelectDelegate onUnit, CanRunDecoratorDelegate extra, bool targetLast)
        {
            return new Decorator(ret => extra(ret) && SpellManager.CanBuff(name, onUnit(ret)),
                                 new Action(ret => SpellManager.Buff(name, onUnit(ret))));
        }

        public static Composite CreateSpellCheckAndCast(string name)
        {
           
            return new Decorator(ret => SpellManager.CanCast(name),
                                 new Sequence(
                                     new Action(ret => SpellManager.Cast(name)),
                                     new Action(ctx => Log("Casting {0}", name)))
                                 
                                 );
        }

        public Composite CreateSpellCheckAndCast(string name, WoWUnit onUnit)
        {
            return new Decorator(ret => SpellManager.CanCast(name),
                 new Sequence(
                                     new Action(ret => SpellManager.Cast(name, onUnit)),
                                     new Action(ctx => Log("Casting {0}", name)))
                                 );
        }

        public Composite CreateSpellCheckAndCast(string name, CanRunDecoratorDelegate extra)
        {
            return new Decorator(ret => extra(ret) && SpellManager.CanCast(name),
                 new Sequence(
                                     new Action(ret => SpellManager.Cast(name)),
                                     new Action(ctx => Log("Casting {0}", name)))                 
                );
        }

        public Composite CreateSpellCheckAndCast(string name, bool checkRange)
        {
            return new Decorator(ret => SpellManager.CanCast(name, checkRange),
                                  new Sequence(
                                     new Action(ret => SpellManager.Cast(name)),
                                     new Action(ctx => Log("Casting {0}", name)))
                                     );
        }

        public Composite CreateSpellCheckAndCast(string name, CanRunDecoratorDelegate extra, ActionDelegate extraAction)
        {
            return new Decorator(ret => extra(ret) && SpellManager.CanCast(name),
                                 new Action(delegate(object ctx)
                                 {
                                     SpellManager.Cast(name);
                                     extraAction(ctx);
                                     return RunStatus.Success;
                                 }));
        }
        public Composite CreateSpellCheckAndCast(string name, string Buffname)
        {
        
            return new Decorator(ret => SpellManager.CanCast(name),
                                 new Action(delegate
                                 {
                                     SpellManager.Cast(name);
                                     StyxWoW.SleepForLagDuration();
                                     if (Me.CurrentTarget.Auras.ContainsKey(Buffname))
                                     {
                                         SpellManager.StopCasting();
                                     }
                                     return RunStatus.Success;
                                 })
                                 );
        }
        public Composite CreateSpellCheckAndCast(string name, CanRunDecoratorDelegate extra, ActionDelegate extraAction, WoWUnit onUnit)
        {
            return new Decorator(ret => extra(ret) && SpellManager.CanCast(name),
                                 new Action(delegate(object ctx)
                                 {
                                     SpellManager.Cast(name, onUnit);
                                     extraAction(ctx);
                                     return RunStatus.Success;
                                 }));
        }
        public Composite Cast(string name)
        {
            return new Decorator(ret => SpellManager.CanCast(name),
                 new Sequence(
                                     new Action(ret => SpellManager.Cast(name)),
                                     new Action(ctx => Log("Casting {0}", name)))
                                 );
        }


        public Composite SummonPet(string PetSpellName)
        {

            return new Decorator(ret => SpellManager.CanCast(PetSpellName) && !Me.GotAlivePet,
                                 new Action(delegate
                                 {
                                     SpellManager.Cast(PetSpellName);
                                     StyxWoW.SleepForLagDuration();
                                     if (Me.GotAlivePet)
                                     {
                                         SpellManager.StopCasting();
                                     }
                                     return RunStatus.Success;
                                 })
                                 );
        }
 
        #endregion
    }
}
