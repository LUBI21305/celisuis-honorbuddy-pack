using System;
using System.Diagnostics;
using System.Windows.Forms;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Profiles;
using Styx.Plugins.PluginClass;
using Styx.WoWInternals;


namespace fnav
{
    public class FNav : HBPlugin
    {
        private Stopwatch _pulseTimer = new Stopwatch();
        private Form _uiForm;

        public override string Name { get { return "FNav - The lazy mans taxi"; } }
        public override string Author { get { return "Fpsware"; } }
        public override Version Version { get { return _version; } }
        private readonly Version _version = new Version(0, 0, 0, 1);
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            _uiForm = new UIForm1();
            _uiForm.Show();

            //UIForm1.Show();
        }
        
        
        public Form UIForm
        {
            get
            {
                return _uiForm ?? (_uiForm = new UIForm1());
            }
        }

        public override void Pulse()
        {
            /*
            //if (_pulseTimer.IsRunning)

            if (!TreeRoot.IsRunning && ObjectManager.Me.IsMoving)
            {
                if (ProfileManager.CurrentProfile.Name.Contains("FNavGoTo"))
                {
                    _uiForm.Controls["distanceBar"]
                }
                
            }
             */
             


        }
         


    }
}
