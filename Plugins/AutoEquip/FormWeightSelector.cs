using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Styx.Helpers;
using Styx.Logic.Inventory;

namespace AutoEquip
{
    public partial class FormWeightSelector : Form
    {
        public FormWeightSelector()
        {
            InitializeComponent();
            RefreshWeightSets();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshWeightSets();
        }

        private readonly List<WeightSet> _weightSets = new List<WeightSet>();

        private void RefreshWeightSets()
        {
            lbxWeightSets.Items.Clear();
            _weightSets.Clear();
            var weightSetPath = Path.Combine(AutoEquipSettings.PluginFolderPath, "Weight Sets");
            string[] files = Directory.GetFiles(weightSetPath, "*.xml", SearchOption.AllDirectories);
            int selectIndex = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string name = Path.GetFileNameWithoutExtension(file);
                try
                {
                    WeightSet weightSet = AutoEquipSettings.LoadWeightSetFromXML(name, XElement.Load(file));
                    _weightSets.Add(weightSet);
                    lbxWeightSets.Items.Add(name);

                    if (AutoEquipSettings.ChosenWeightSet != null && AutoEquipSettings.ChosenWeightSet.Name == weightSet.Name)
                        selectIndex = i;
                }
                catch (XmlException ex)
                {
                    Logging.Write("[AutoEquip]: Could not load weight set {0} - {1}", name, ex.Message);
                }
            }

            if (selectIndex < 0 || selectIndex >= lbxWeightSets.Items.Count)
                return;

            lbxWeightSets.SelectedIndex = selectIndex;
        }

        private void ShowSelectedWeightSetInfo()
        {
            lbxWeights.Items.Clear();
            if (lbxWeightSets.SelectedIndex < 0 || lbxWeightSets.SelectedIndex >= _weightSets.Count)
                return;

            try
            {
                var selectedSet = _weightSets[lbxWeightSets.SelectedIndex];
                string[] parts = selectedSet.Name.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                lblClass.Text = "Class: " + parts[0];
                if (parts.Length > 1)
                    lblSpec.Text = "Spec: " + parts[1];
                else
                    lblSpec.Text = "Spec:";
                if (parts.Length > 2)
                    lblMisc.Text = "Misc: " + parts[2];
                else
                    lblMisc.Text = "Misc:";

                foreach (var kvp in selectedSet.Weights)
                    lbxWeights.Items.Add(kvp.Key + ": " + kvp.Value);
            }
            catch (KeyNotFoundException)
            {
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lbxWeightSets.SelectedIndex < 0 || lbxWeightSets.SelectedIndex >= _weightSets.Count)
            {
                Close();
                return;
            }

            AutoEquipSettings.ChosenWeightSet = _weightSets[lbxWeightSets.SelectedIndex];
            AutoEquipSettings.SaveDefaultAutoEquipSet();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lbxWeightSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedWeightSetInfo();
        }
    }
}