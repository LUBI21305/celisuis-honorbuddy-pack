using Styx.Helpers;
namespace RoutesBuddy
{
    public class RoutesBuddySettings : Settings
    {
        public RoutesBuddySettings(string settingsPath)
            : base(settingsPath)
        {
            Load();
        }
        [Setting, DefaultValue(4)]
        public int exampleInteger { get; set; }

        [Setting, DefaultValue(true)]
        public bool exampleBool { get; set; }

        [Setting, DefaultValue("WHAZUP")]
        public string exampleString { get; set; }
    }
}
