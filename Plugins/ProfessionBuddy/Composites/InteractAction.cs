using System;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Styx;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using System.Diagnostics;
using ObjectManager = Styx.WoWInternals.ObjectManager;
using System.Drawing.Design;

namespace HighVoltz.Composites
{
    #region InteractAction
    class InteractionAction : PBAction
    {
        public enum InteractActionType
        {
            NPC,
            GameObject,
        }
        public InteractActionType InteractType
        {
            get { return (InteractActionType)Properties["InteractType"].Value; }
            set { Properties["InteractType"].Value = value; }
        }
        /// <summary>
        /// NPC/Object Entry ID
        /// </summary>
        public uint Entry
        {
            get { return (uint)Properties["Entry"].Value; }
            set { Properties["Entry"].Value = value; }
        }
        public uint InteractDelay
        {
            get { return (uint)Properties["InteractDelay"].Value; }
            set { Properties["InteractDelay"].Value = value; }
        }
        public WoWGameObjectType GameObjectType
        {
            get { return (WoWGameObjectType)Properties["GameObjectType"].Value; }
            set { Properties["GameObjectType"].Value = value; }
        }
        public WoWSpellFocus SpellFocus
        {
            get { return (WoWSpellFocus)Properties["SpellFocus"].Value; }
            set { Properties["SpellFocus"].Value = value; }
        }
        public InteractionAction()
            : base()
        {
            Properties["Entry"] = new MetaProp("Entry", typeof(uint), new EditorAttribute(typeof(PropertyBag.EntryEditor), typeof(UITypeEditor)));
            Properties["InteractDelay"] = new MetaProp("InteractDelay", typeof(uint));
            Properties["InteractType"] = new MetaProp("InteractType", typeof(InteractActionType), new DisplayNameAttribute("Interact Type"));
            Properties["GameObjectType"] = new MetaProp("GameObjectType", typeof(WoWGameObjectType), new DisplayNameAttribute("GameObject Type"));
            Properties["SpellFocus"] = new MetaProp("SpellFocus", typeof(WoWSpellFocus), new DisplayNameAttribute("SpellFocus"));

            InteractDelay=Entry = 0u;
            InteractType = InteractActionType.GameObject;
            GameObjectType = WoWGameObjectType.Mailbox;
            SpellFocus = WoWSpellFocus.Anvil;
            
            Properties["SpellFocus"].Show = false;
            Properties["InteractType"].PropertyChanged += new EventHandler(InteractionAction_PropertyChanged);
            Properties["GameObjectType"].PropertyChanged += InteractionAction_PropertyChanged;
        }

        Stopwatch interactSw = new Stopwatch();
        protected override RunStatus Run(object context)
        {
            if (!IsDone)
            {
                WoWObject obj = null;
                if (InteractType == InteractActionType.NPC)
                {
                    if (Entry != 0)
                        obj = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == Entry).OrderBy(u => u.Distance).FirstOrDefault();
                    else if (ObjectManager.Me.GotTarget)
                        obj = ObjectManager.Me.CurrentTarget;
                }
                else if (InteractType == InteractActionType.GameObject)
                    obj = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => (Entry > 0 && u.Entry == Entry) || (u.SubType == GameObjectType &&
                        (GameObjectType != WoWGameObjectType.SpellFocus || (GameObjectType == WoWGameObjectType.SpellFocus && u.SpellFocus == SpellFocus))))
                        .OrderBy(u => u.Distance).FirstOrDefault();
                if (obj != null)
                {
                    if (!obj.WithinInteractRange)
                        Util.MoveTo(WoWMathHelper.CalculatePointFrom(me.Location,obj.Location,3));
                    else
                    {
                        if (InteractDelay >0 && (!interactSw.IsRunning || interactSw.ElapsedMilliseconds < InteractDelay))
                        {
                            interactSw.Start();
                        }
                        else
                        {
                            interactSw.Reset();
                            obj.Interact();
                            IsDone = true;
                        }
                    }
                }
                if (!IsDone)
                    return RunStatus.Running;
            }
            return RunStatus.Failure;
        }
        void InteractionAction_PropertyChanged(object sender, EventArgs e)
        {
            switch (GameObjectType)
            {
                case WoWGameObjectType.SpellFocus:
                    Properties["SpellFocus"].Show = true;
                    break;
                default:
                    Properties["SpellFocus"].Show = false;
                    break;
            }
            switch (InteractType)
            {
                case InteractActionType.GameObject:
                    Properties["GameObjectType"].Show = true;
                    break;
                case InteractActionType.NPC:
                    Properties["GameObjectType"].Show = false;
                    break;
                default:
                    Properties["GameObjectType"].Show = true;
                    break;
            }
            RefreshPropertyGrid();
        }
        public override string Name
        {
            get { return "Interact"; }
        }
        public override string Title
        {
            get { return string.Format("{0}: {1} " + (Entry != 0 ? Entry.ToString() : ""), Name, InteractType); }
        }
        public override string Help
        {
            get
            {
                return "This action will cause your character to interact with a specific target. InteractDelay is a time in milliseconds to way before interacting with target.This can be also used to goto objects like forge/anvil or NPCs that are within 100 feet";
            }
        }
        public override object Clone()
        {
            return new InteractionAction()
            {
                InteractType = this.InteractType,
                Entry = this.Entry,
                GameObjectType = this.GameObjectType,
                SpellFocus = this.SpellFocus,
                InteractDelay = this.InteractDelay,
            };
        }
        #region XmlSerializer
        public override void ReadXml(XmlReader reader)
        {
            uint num;
            uint.TryParse(reader["Entry"], out num);
            Entry = num;
            if (reader.MoveToAttribute("InteractDelay"))
            {
                uint.TryParse(reader["InteractDelay"], out num);
                InteractDelay = num;
            }
            InteractType = (InteractActionType)Enum.Parse(typeof(InteractActionType), reader["InteractType"]);
            GameObjectType = (WoWGameObjectType)Enum.Parse(typeof(WoWGameObjectType), reader["GameObjectType"]);
            SpellFocus = (WoWSpellFocus)Enum.Parse(typeof(WoWSpellFocus), reader["SpellFocus"]);
            reader.ReadStartElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Entry", Entry.ToString());
            writer.WriteAttributeString("InteractDelay", InteractDelay.ToString());
            writer.WriteAttributeString("InteractType", InteractType.ToString());
            writer.WriteAttributeString("GameObjectType", GameObjectType.ToString());
            writer.WriteAttributeString("SpellFocus", SpellFocus.ToString());
        }
        #endregion
    }
    #endregion
}
