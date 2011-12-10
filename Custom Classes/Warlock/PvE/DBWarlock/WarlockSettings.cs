using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.WoWInternals;

namespace DBWarlock
{
    public class WarlockSettings
    {
        string _cfgFile = "Settings";
        public WarlockSettings()
        {
            if (ObjectManager.Me.Class == WoWClass.Warlock)
            try
            {
                //            _cfgFile = "DBWarlock.settings";
                Load();
            }
            catch (Exception e)
            { Logging.Write(e.Message); }
        }
        #region variables

        #region status
        public bool showDebug = false;
        #endregion

        #region Resting & Combat

        //Resting Logic
        public int restManaPercent = 30;
        public int restHealthPercent = 30;

        //Shards And Stones
        public int maxSoulShards = 10;
        public int healthStoneHealthPercent = 45;
        public bool useFirestone = true;
        public bool useSpellstone = false;
        public bool useHealthStone = true;
        public bool useSoulstone = true;
        public bool useDrainSoul = true;

        //Metamorphosis
        public bool useMetamorphosis = true;
        public int metamorphosisMinimumAggros = 2;
        public bool useShadowCleave = true;
        public bool useImmolationAura = true;
        public bool useShadowFlame = true;

        //Lag Delay Average
        public int myLag = 100;

        //Pet Attack Delay
        public int petAttackDelay = 500;

        //Armor Buff

        public bool useArmor = true;
        public string armorName = "Auto";

        #endregion

        #region Spell
        // ATTACK
        public bool useShadowBolt = true;
        public bool useSoulFire = true;
        public bool useIncinerate = true;
        public bool useWand = true;
        public bool useSearingOfPain = true;

        //DOT
        public bool useImmolate = true;
        public bool useCorruption = true;
        public bool dotAdds = true;

        //CURSE
        public bool useCurse = true;
        public string curseName = "Auto";

        //Life Tap
        public bool useLifeTap = true;
        public int lfTapMinHealth = 60;
        public int lfTapMaxMana = 70;

        //Drain Life
        public bool useDrainLife = true;
        public int drainLifeMaxHealth = 40;
        public int drainLifeStopHealth = 95;

        //Drain Mana
        public bool useDrainMana = true;
//        public int drainLifeMaxHealth = 40;
//        public int drainLifeStopHealth = 95;

        //Death Coil
        public bool useDeathCoil = true;
        public int dcMaxHealth = 40;

        //HealthFunnel
        public bool useHealthFunnel = true;
        public int hfMinPlayerHealth = 40;
        public int hfPetMinHealth = 40;
        public int hfPetMaxHealth = 95;

        //PVP
        public bool useFear = true;
        public bool useHowlOfTerror = true;

        //General
        public bool useHaunt = true;
        public bool useUnstableAffliction = true;


        #endregion

        #region Summon
        public bool useSummon = true;
        public int summonEntry = 0; // 0 = Auto / Sumon Entry ID
        public bool useFelDomination = true;

        //Enslave Demon
        public bool useEnslaveDemon = false;
        public bool useDemonicEmpowerment = true;
        public bool useDarkPact = true;
        public bool useSoulLink = true;


        #endregion



        #endregion

        #region XmlUtils
        //Code Extracted from ShamWOW
        public void Load()
        {
        //    XmlTextReader reader;
            XmlDocument xml = new XmlDocument();
            XmlNode xvar;

            string sPath = Process.GetCurrentProcess().MainModule.FileName;
            sPath = Path.GetDirectoryName(sPath);
            sPath = Path.Combine(sPath, _cfgFile);

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDebug("Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            String serverName = Lua.GetReturnVal<string>("return GetRealmName()", 0);
            String toonName = ObjectManager.Me.Name;

            sPath = Path.Combine(sPath, "DBWarlock-" + serverName + "-" + toonName + ".config");


            //((Warlock)Global.CurrentCC).slog("");
            //Logging.Write("");

            Logging.WriteDebug("Loading config file: {0}", sPath);
            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            try
            {
                xml.Load(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.Write(e.Message);
                Logging.Write("[DBWCONFIG]Continuing with Default Config Values");
                fs.Close();
                try { Save(); }
                catch (Exception) { }
                return;
            }

//            xml = new XmlDocument();

            try
            {
//                xml.Load(reader);
                if (xml == null)
                    return;

                xvar = xml.SelectSingleNode("//DBWarlockSettings/showDebug");
                if (xvar != null)
                {
                    showDebug = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + showDebug.ToString());
                }

                //Resting Logic
                xvar = xml.SelectSingleNode("//DBWarlockSettings/restManaPercent");
                if (xvar != null)
                {
                    restManaPercent = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + restManaPercent.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/restHealthPercent");
                if (xvar != null)
                {
                    restHealthPercent = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + restHealthPercent.ToString());
                }

                //Shards And Stones
                xvar = xml.SelectSingleNode("//DBWarlockSettings/maxSoulShards");
                if (xvar != null)
                {
                    maxSoulShards = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + maxSoulShards.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/healthStoneHealthPercent");
                if (xvar != null)
                {
                    healthStoneHealthPercent = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + healthStoneHealthPercent.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useFirestone");
                if (xvar != null)
                {
                    useFirestone = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useFirestone.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useSpellstone");
                if (xvar != null)
                {
                    useSpellstone = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useSpellstone.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useHealthStone");
                if (xvar != null)
                {
                    useHealthStone = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useHealthStone.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useSoulstone");
                if (xvar != null)
                {
                    useSoulstone = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useSoulstone.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useDrainSoul");
                if (xvar != null)
                {
                    useDrainSoul = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useDrainSoul.ToString());
                }

                //Metamorphosis
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useMetamorphosis");
                if (xvar != null)
                {
                    useMetamorphosis = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useMetamorphosis.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/metamorphosisMinimumAggros");
                if (xvar != null)
                {
                    metamorphosisMinimumAggros = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + metamorphosisMinimumAggros.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useShadowCleave");
                if (xvar != null)
                {
                    useShadowCleave = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useShadowCleave.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useImmolationAura");
                if (xvar != null)
                {
                    useImmolationAura = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useImmolationAura.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useShadowFlame");
                if (xvar != null)
                {
                    useShadowFlame = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useShadowFlame.ToString());
                }

                //Lag Delay Average
                xvar = xml.SelectSingleNode("//DBWarlockSettings/myLag");
                if (xvar != null)
                {
                    myLag = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + myLag.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/myLag");
                if (xvar != null)
                {
                    myLag = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + myLag.ToString());
                }

                //Pet Attack Delay
                xvar = xml.SelectSingleNode("//DBWarlockSettings/petAttackDelay");
                if (xvar != null)
                {
                    petAttackDelay = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + petAttackDelay.ToString());
                }

                //Armor Buff
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useArmor");
                if (xvar != null)
                {
                    useArmor = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useArmor.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/armorName");
                if (xvar != null)
                {
                    armorName = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + armorName.ToString());
                }

                // ATTACK
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useShadowBolt");
                if (xvar != null)
                {
                    useShadowBolt = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useShadowBolt.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useSoulFire");
                if (xvar != null)
                {
                    useSoulFire = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useSoulFire.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useIncinerate");
                if (xvar != null)
                {
                    useIncinerate = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useIncinerate.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useWand");
                if (xvar != null)
                {
                    useWand = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useWand.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useSearingOfPain");
                if (xvar != null)
                {
                    useSearingOfPain = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useSearingOfPain.ToString());
                }

                //DOT
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useImmolate");
                if (xvar != null)
                {
                    useImmolate = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useImmolate.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useCorruption");
                if (xvar != null)
                {
                    useCorruption = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useCorruption.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/dotAdds");
                if (xvar != null)
                {
                    dotAdds = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + dotAdds.ToString());
                }

                //CURSE
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useCurse");
                if (xvar != null)
                {
                    useCurse = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useCurse.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/curseName");
                if (xvar != null)
                {
                    curseName = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + curseName.ToString());
                }

                //Life Tap
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useLifeTap");
                if (xvar != null)
                {
                    useLifeTap = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useLifeTap.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/lfTapMinHealth");
                if (xvar != null)
                {
                    lfTapMinHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + lfTapMinHealth.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/lfTapMaxMana");
                if (xvar != null)
                {
                    lfTapMaxMana = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + lfTapMaxMana.ToString());
                }

                //Drain Life
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useDrainLife");
                if (xvar != null)
                {
                    useDrainLife = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useDrainLife.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/drainLifeMaxHealth");
                if (xvar != null)
                {
                    drainLifeMaxHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + drainLifeMaxHealth.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/drainLifeStopHealth");
                if (xvar != null)
                {
                    drainLifeStopHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + drainLifeStopHealth.ToString());
                }

                //Drain Mana
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useDrainMana");
                if (xvar != null)
                {
                    useDrainMana = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useDrainMana.ToString());
                }

                //Death Coil
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useDeathCoil");
                if (xvar != null)
                {
                    useDeathCoil = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useDeathCoil.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/dcMaxHealth");
                if (xvar != null)
                {
                    dcMaxHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + dcMaxHealth.ToString());
                }

                //HealthFunnel
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useHealthFunnel");
                if (xvar != null)
                {
                    useHealthFunnel = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useHealthFunnel.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/hfMinPlayerHealth");
                if (xvar != null)
                {
                    hfMinPlayerHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + hfMinPlayerHealth.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/hfPetMinHealth");
                if (xvar != null)
                {
                    hfPetMinHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + hfPetMinHealth.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/hfPetMaxHealth");
                if (xvar != null)
                {
                    hfPetMaxHealth = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + hfPetMaxHealth.ToString());
                }

                //PVP
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useFear");
                if (xvar != null)
                {
                    useFear = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useFear.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useHowlOfTerror");
                if (xvar != null)
                {
                    useHowlOfTerror = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useHowlOfTerror.ToString());
                }

                //General
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useHaunt");
                if (xvar != null)
                {
                    useHaunt = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useHaunt.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useUnstableAffliction");
                if (xvar != null)
                {
                    useUnstableAffliction = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useUnstableAffliction.ToString());
                }

                //Summon
                xvar = xml.SelectSingleNode("//DBWarlockSettings/useSummon");
                if (xvar != null)
                {
                    useSummon = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useSummon.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/summonEntry");
                if (xvar != null)
                {
                    summonEntry = Convert.ToInt16(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + summonEntry.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useFelDomination");
                if (xvar != null)
                {
                    useFelDomination = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useFelDomination.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useEnslaveDemon");
                if (xvar != null)
                {
                    useEnslaveDemon = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useEnslaveDemon.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useDemonicEmpowerment");
                if (xvar != null)
                {
                    useDemonicEmpowerment = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useDemonicEmpowerment.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useDarkPact");
                if (xvar != null)
                {
                    useDarkPact = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useDarkPact.ToString());
                }

                xvar = xml.SelectSingleNode("//DBWarlockSettings/useSoulLink");
                if (xvar != null)
                {
                    useSoulLink = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("   cfg:  " + xvar.Name + "=" + useSoulLink.ToString());
                }


            }
            catch (Exception e)
            {
                Logging.WriteDebug("DBWARLOCK EXCEPTION, STACK=" + e.StackTrace);
                Logging.WriteDebug("DBWARLOCK EXCEPTION, SRC=" + e.Source);
                Logging.WriteDebug("DBWARLOCK : " + e.Message);
            }
        }


        public void Save()
        {
            XmlDocument xml;
            XmlElement root;
            XmlElement element;
            XmlText text;
            XmlComment xmlComment;

            string sPath = Process.GetCurrentProcess().MainModule.FileName;
            sPath = Path.GetDirectoryName(sPath);

            sPath = Path.Combine(sPath, _cfgFile);

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDebug("Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            String serverName = Lua.GetReturnVal<string>("return GetRealmName()", 0);
            String toonName = ObjectManager.Me.Name;

            sPath = Path.Combine(sPath, "DBWarlock-" + serverName + "-" + toonName + ".config");
            //((Warlock)Global.CurrentCC).slog("");
            //Logging.Write("");

            Logging.WriteDebug("Saving config file: {0}", sPath);
            xml = new XmlDocument();
            XmlDeclaration dc = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.AppendChild(dc);

            xmlComment = xml.CreateComment(
                "=======================================================================\n" +
                "Configuration file for the DBWarlock CC\n\n" + 
                "XML file containing settings to customize the the DBWarlock CC.\n" + 
                "It is STRONGLY recommended you use the Configuration UI to change this\n"+
                "file instead of direct changein it here.\n"+
                "========================================================================");

            //let's add the root element
            root = xml.CreateElement("DBWarlockSettings");
            root.AppendChild(xmlComment);

            //let's add another element (child of the root)
            element = xml.CreateElement("showDebug");
            text = xml.CreateTextNode(showDebug.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("restManaPercent");
            text = xml.CreateTextNode(restManaPercent.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("restHealthPercent");
            text = xml.CreateTextNode(restHealthPercent.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("maxSoulShards");
            text = xml.CreateTextNode(maxSoulShards.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("healthStoneHealthPercent");
            text = xml.CreateTextNode(healthStoneHealthPercent.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useFirestone");
            text = xml.CreateTextNode(useFirestone.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useSpellstone");
            text = xml.CreateTextNode(useSpellstone.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useHealthStone");
            text = xml.CreateTextNode(useHealthStone.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useSoulstone");
            text = xml.CreateTextNode(useSoulstone.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useDrainSoul");
            text = xml.CreateTextNode(useDrainSoul.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useMetamorphosis");
            text = xml.CreateTextNode(useMetamorphosis.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("metamorphosisMinimumAggros");
            text = xml.CreateTextNode(metamorphosisMinimumAggros.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useShadowCleave");
            text = xml.CreateTextNode(useShadowCleave.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useImmolationAura");
            text = xml.CreateTextNode(useImmolationAura.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useShadowFlame");
            text = xml.CreateTextNode(useShadowFlame.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("myLag");
            text = xml.CreateTextNode(myLag.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("petAttackDelay");
            text = xml.CreateTextNode(petAttackDelay.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useArmor");
            text = xml.CreateTextNode(useArmor.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("armorName");
            text = xml.CreateTextNode(armorName.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useShadowBolt");
            text = xml.CreateTextNode(useShadowBolt.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useSoulFire");
            text = xml.CreateTextNode(useSoulFire.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useIncinerate");
            text = xml.CreateTextNode(useIncinerate.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useWand");
            text = xml.CreateTextNode(useWand.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useSearingOfPain");
            text = xml.CreateTextNode(useSearingOfPain.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useImmolate");
            text = xml.CreateTextNode(useImmolate.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useCorruption");
            text = xml.CreateTextNode(useCorruption.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("dotAdds");
            text = xml.CreateTextNode(dotAdds.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useCurse");
            text = xml.CreateTextNode(useCurse.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("curseName");
            text = xml.CreateTextNode(curseName.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useLifeTap");
            text = xml.CreateTextNode(useLifeTap.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("lfTapMinHealth");
            text = xml.CreateTextNode(lfTapMinHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("lfTapMaxMana");
            text = xml.CreateTextNode(lfTapMaxMana.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useDrainLife");
            text = xml.CreateTextNode(useDrainLife.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("drainLifeMaxHealth");
            text = xml.CreateTextNode(drainLifeMaxHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("drainLifeStopHealth");
            text = xml.CreateTextNode(drainLifeStopHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useDrainMana");
            text = xml.CreateTextNode(useDrainMana.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useDeathCoil");
            text = xml.CreateTextNode(useDeathCoil.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("dcMaxHealth");
            text = xml.CreateTextNode(dcMaxHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useHealthFunnel");
            text = xml.CreateTextNode(useHealthFunnel.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("hfMinPlayerHealth");
            text = xml.CreateTextNode(hfMinPlayerHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("hfPetMinHealth");
            text = xml.CreateTextNode(hfPetMinHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("hfPetMaxHealth");
            text = xml.CreateTextNode(hfPetMaxHealth.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useFear");
            text = xml.CreateTextNode(useFear.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useHowlOfTerror");
            text = xml.CreateTextNode(useHowlOfTerror.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useHaunt");
            text = xml.CreateTextNode(useHaunt.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useUnstableAffliction");
            text = xml.CreateTextNode(useUnstableAffliction.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useSummon");
            text = xml.CreateTextNode(useSummon.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("summonEntry");
            text = xml.CreateTextNode(summonEntry.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useFelDomination");
            text = xml.CreateTextNode(useFelDomination.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useEnslaveDemon");
            text = xml.CreateTextNode(useEnslaveDemon.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useDemonicEmpowerment");
            text = xml.CreateTextNode(useDemonicEmpowerment.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useDarkPact");
            text = xml.CreateTextNode(useDarkPact.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("useSoulLink");
            text = xml.CreateTextNode(useSoulLink.ToString());
            element.AppendChild(text);
            root.AppendChild(element);


            xml.AppendChild(root);

            //let's try to save the XML document in a file: C:\pavel.xml
            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            try
            {
                xml.Save(fs); //I've chosen the c:\ for the resulting file pavel.xml
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.Write(e.Message);
            }



    }


        #endregion
    }
}
