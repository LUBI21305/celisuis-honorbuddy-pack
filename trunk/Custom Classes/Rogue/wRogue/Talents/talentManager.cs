using System;
using System.Linq;
using System.Collections.Generic;

using Styx;
using Styx.Helpers;
using Styx.WoWInternals;

namespace wRogue.Talents
{
    /// <summary>
    ///   This nifty little class will allow us to grab all the talents currently spec'd on the toon
    ///   and tally them up later so we can figure out the exact spec of the toon
    /// </summary>
    internal class TalentManager
    {
        public TalentManager()
        {
            TalentCount = new Dictionary<string, int>();

            // First things first, grab every single talent we can. :)
            UpdateTalentCounts();
        }

        public Dictionary<string, int> TalentCount { get; private set; }

        public event EventHandler OnTalentsUpdated;

        private void UpdateTalentCounts()
        {
            // We will safely assume we only have 3 talent tabs (hunters can go to hell!)

            // Do this 1 based to make it valid Lua indexes
            for (int tab = 1; tab <= 3; tab++)
            {
                var numTalents = Lua.GetReturnVal<int>(string.Format("return GetNumTalents({0})", tab), 0);
                for (int index = 1; index <= numTalents; index++)
                {
                    List<string> talentInfo = Lua.GetReturnValues(string.Format("return GetTalentInfo({0}, {1})", tab, index), "lua.lua");

                    string name = talentInfo[0];
                    int rank = Convert.ToInt32(talentInfo[4]);

                    TalentCount.Add(name, rank);
                }
            }

            if (OnTalentsUpdated != null)
                OnTalentsUpdated(this, EventArgs.Empty);
        }

        public wRogueSpec GetSpec()
        {
            if (TalentCount.Count == 0)
                return wRogueSpec.Lowbie;

            if (StyxWoW.Me.Level < 10)
                return wRogueSpec.Lowbie;

            int ass = 0, com = 0, sub = 0;
            foreach (var t in TalentCount)
            {
                int count = t.Value;
                switch (t.Key)
                {
                    // Russian Support provided by divinerock
                    // German Support provided by Laria
                    // Spanish Support provided by BAJONDIL

                    //English
                    case "Deadly Momentum":
                    case "Coup de Grace":
                    case "Leathality":
                    case "Ruthlessness":
                    case "Quickening":
                    case "Puncturing Wounds":
                    case "Blackjack":
                    case "Deadly Brew":
                    case "Cold Blood":
                    case "Vile Poisons":
                    case "Deadened Nerves":
                    case "Seal Fate":
                    case "Murderous Intent":
                    case "Overkill":
                    case "Master Poisoner":
                    case "Improved Expose Armor":
                    case "Cut to the Chase":
                    case "Venomous Wounds":
                    case "Vendetta":
                    //Russian
                    case "Смертельный импульс":
                    case "Удар милосердия":
                    case "Смертоносность":
                    case "Жестокость":
                    case "Активизация":
                    case "Колотые раны":
                    case "Червленый вяз":
                    case "Смертельное варево":
                    case "Хладнокровие":
                    case "Тлетворные яды":
                    case "Омертвевшие нервы":
                    case "Печать судьбы":
                    case "Жестокие намерения":
                    case "Бойня":
                    case "Мастер ядоварения":
                    case "Сильное ослабление доспеха":
                    case "Сразу к делу":
                    case "Незаживающие раны":
                    case "Вендетта":
                    //German
                    case "Tödliche Dynmaik":
                    case "Gnadenstoß":
                    case "Tödlichkeit":
                    case "Skrupellosigkeit":
                    case "Elan":
                    case "Stichwunden":
                    case "Nackenschlag":
                    case "Tödliche Mischung":
                    case "Kaltblütigkeit":
                    case "Üble Gifte":
                    case "Abgestumpfte Nerven":
                    case "Schicksal besiegeln":
                    case "Mörderische Absicht":
                    case "Amok":
                    case "Meister der Gifte":
                    case "Verbessertes Rüstung schwächen":
                    case "In Stücke schneiden":
                    case "Vergiftende Wunden":
                    //Spanish
                    case "Ímpetu mortal":
                    case "Golpe de gracia":
                    case "Letalidad":
                    case "Crueldad":
                    case "Aceleración":
                    case "Heridas perforadoras":
                    case "Porra":
                    case "Brebaje letal":
                    case "Sangre fría":
                    case "Venenos inmundos":
                    case "Impasibilidad":
                    case "Sellar destino":
                    case "Propósito asesino":
                    case "Exterminar":
                    case "Maestro envenenador":
                    case "Exponer armadura mejorado":
                    case "Acortar":
                    case "Heridas envenenadas":
                        ass += count;
                        break;

                    //English
                    case "Improved Recuperate":
                    case "Improved Sinister Strike":
                    case "Precision":
                    case "Improved Slice and Dice":
                    case "Improved Sprint":
                    case "Aggression":
                    case "Improved Kick":
                    case "Lightning Reflexes":
                    case "Revealing Strike":
                    case "Reinforced Leather":
                    case "Improved Gouge":
                    case "Combat Potency":
                    case "Blade Twisting":
                    case "Throwing Specialization":
                    case "Andrenaline Rush":
                    case "Savage Combat":
                    case "Bandit's Guile":
                    case "Restless Blades":
                    case "Killing Spree":
                    //Russian
                    case "Ускоренное заживление ран":
                    case "Усиленный коварный удар":
                    case "Точность":
                    case "Жестокая мясорубка":
                    case "Улучшенный спринт":
                    case "Агрессивность":
                    case "Подлый пинок":
                    case "Молниеносные рефлексы":
                    case "Пробивающий удар":
                    case "Укрепленная кожаная броня":
                    case "Мощный парализующий удар":
                    case "Боевой потенциал":
                    case "Вращение лезвий":
                    case "Специализация на метательном оружии":
                    case "Выброс адреналина":
                    case "Жар битвы":
                    case "Коварство бандита":
                    case "Не знающие устали клинки":
                    case "Череда убийств":
                    //German
                    case "Verbesserte Gesundung":
                    case "Verbesserter finsterer Stoß":
                    case "Präzision":
                    case "Verbessertes Zerhäckseln":
                    case "Verbessertes Sprinten":
                    case "Verbesserter Tritt":
                    case "Blitzartige Reflexe":
                    case "Enthüllender Stoß":
                    case "Verstärktes Leder":
                    case "Verbesserter Solarplexus":
                    case "Kampfkraft":
                    case "Klingenwendung":
                    case "Wurfwaffenspezialisierung":
                    case "Adrenalinrausch":
                    case "Grausamer Kampf":
                    case "Arglist des Banditen":
                    case "Ruhlose Klingen":
                    case "Mordlust":
                    //Spanish
                    case "Reponerse mejorado":
                    case "Golpe siniestro mejorado":
                    case "Precisión":
                    case "Hacer picadillo mejorado":
                    case "Sprint mejorado":
                    case "Agresión":
                    case "Patada mejorada":
                    case "Reflejos de relámpago":
                    case "Golpe revelador":
                    case "Cuero reforzado":
                    case "Gubia mejorada":
                    case "Potencia de combate":
                    case "Malabares cortantes":
                    case "Especialización en lanzamiento":
                    case "Subidón de adrenalina":
                    case "Combate salvaje":
                    case "Astucia del bandido":
                    case "Hojas inquietas":
                    case "Asesinato múltiple":
                        com += count;
                        break;

                    //English
                    case "Nightstalker":
                    case "Improved Ambush":
                    case "Relentless Strikes":
                    case "Elusiveness":
                    case "Waylay":
                    case "Opportunity":
                    case "Initiative":
                    case "Energetic Recovery":
                    case "Find Weakness":
                    case "Hemorrhage":
                    case "Honor Among Thieves":
                    case "Premeditation":
                    case "Enveloping Shadows":
                    case "Cheat Death":
                    case "Preparation":
                    case "Sanguinary Vein":
                    case "Slaughter from the Shadows":
                    case "Serrated Blades":
                    case "Shadow Dance":
                    //Russian
                    case "Ночной ловец":
                    case "Разрушительный подлый удар":
                    case "Неослабевающие удары":
                    case "Неуловимость":
                    case "Засада":
                    case "Правильный момент":
                    case "Инициатива":
                    case "Восстановление энергии":
                    case "Поиск слабости":
                    case "Кровоизлияние":
                    case "Воровская честь":
                    case "Умысел":
                    case "Окутывающие тени":
                    case "Обман смерти":
                    case "Подготовка":
                    case "Вспарывание вен":
                    case "Резня во тьме":
                    case "Зазубренные клинки":
                    case "Танец теней":
                    //German
                    case "Nachtpirscher":
                    case "Verbesserter Hinterhalt":
                    case "Unerbittliche Stöße":
                    case "Flüchtigkeit":
                    case "Wegelagerei":
                    case "Günstige Gelegenheit":
                    case "Energetische Erholung":
                    case "Schwächen aufspüren":
                    case "Blutsturz":
                    case "Ehre unter Dieben":
                    case "Konzentration":
                    case "Verhüllende Schatten":
                    case "Von der Schippe springen":
                    case "Vorbereitung":
                    case "Venenschnitt":
                    case "Hinterhältiger Mord":
                    case "Gezahnte Klingen":
                    case "Schattentanz":
                    //Spanish
                    case "Acechador nocturno":
                    case "Emboscada mejorada":
                    case "Golpes despiadados":
                    case "Elusión":
                    case "Acecho":
                    case "Oportunidad":
                    case "Iniciativa":
                    case "Recuperación enérgica":
                    case "Descubrir debilidad":
                    case "Hemorragia":
                    case "Premeditación":
                    case "Sombras envolventes":
                    case "Burlar a la muerte":
                    case "Preparación":
                    case "Vena sangrienta":
                    case "Matanza desde las Sombras":
                    case "Espadas dentadas":
                    case "Danza de las Sombras":
                        sub += count;
                        break;
                }
            }
            Dictionary<wRogueSpec, int> counts = new Dictionary<wRogueSpec, int>
                                                          {
                                                              {wRogueSpec.Assassination, ass},
                                                              {wRogueSpec.Combat, com},
                                                              {wRogueSpec.Subtlety, sub},
                                                          };
            return counts.OrderByDescending(k => k.Value).First().Key;
        }
    }
}