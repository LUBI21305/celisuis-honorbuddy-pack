using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace HighVoltz
{
    public partial class MaterialListForm : Form
    {
        Professionbuddy PB;
        public MaterialListForm()
        {
            InitializeComponent();
            PB = Professionbuddy.Instance;
        }

        private void MaterialListForm_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<uint,int> kv in PB.MaterialList)
            {
                foreach (TradeSkill ts in PB.TradeSkillList)
                {
                    if (ts.Ingredients.ContainsKey(kv.Key))
                    {
                        uint bankCnt = Util.GetBankItemCount(kv.Key);
                        int gBankOrAltsCnt = GetAltsItemCount(kv.Key) - (int)(bankCnt + ts.Ingredients[kv.Key].InBagsCount);
                        MaterialGridView.Rows.Add(ts.Ingredients[kv.Key].Name, kv.Value,
                            ts.Ingredients[kv.Key].InBagsCount, bankCnt, PB.HasDataStoreAddon ? gBankOrAltsCnt:0);
                    }
                }
            }
        }

        int GetAltsItemCount (uint itemID)
        {
            return PB.DataStore.ContainsKey(itemID) ? PB.DataStore[itemID] : 0;
        }
    }
}
