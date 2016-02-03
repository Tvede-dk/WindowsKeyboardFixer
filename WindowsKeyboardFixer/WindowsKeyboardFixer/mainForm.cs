using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsKeyboardFixer {
    public partial class mainForm : Form {


        /// <summary>
        /// app settings that are stored in the registry
        /// </summary>
        private Settings appSettings = new Settings() { appName = "KeyboardFixer" };

        /// <summary>
        /// handler for the keyboard (layout) interaction.
        /// </summary>
        private KeyboardHandler keyboardHandler = new KeyboardHandler();
        /// <summary>
        /// simple indicator that we are ready to respond to ui events. 
        /// </summary>
        private bool isDoneInitalizing = false;
        /// <summary>
        /// the cultures we are working with. 
        /// </summary>
        private List<CultureInfo> allInfos = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));

        private List<CultureInfo> displayingInfos = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));


        private string filterText = "";

        private CultureInfo selectedInfo;

        public mainForm() {
            InitializeComponent();
            filterCultureInfos();
            checkBox1.Checked = appSettings.HaveRegistered();
            isDoneInitalizing = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            if (keyboardHandler.IsListening) {
                //unregister
                keyboardHandler.UnRegister();
            }
            Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            new MainAbout().ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (selectedInfo != null) {
                keyboardHandler.Register(selectedInfo);
                handleUI(false);
                HideToTray();
            }
        }

        private void handleUI(bool enableSelection) {
            button1.Enabled = enableSelection;
            button2.Enabled = !enableSelection;
            textBox1.Enabled = enableSelection;
            listBox1.Enabled = enableSelection;
        }

        private void button2_Click(object sender, EventArgs e) {
            keyboardHandler.UnRegister();
            handleUI(true);
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            //filter the list
            filterText = textBox1.Text;
            filterCultureInfos();
        }

        private void filterCultureInfos() {
            listBox1.Items.Clear();
            displayingInfos.Clear();
            foreach (var item in allInfos) {
                if (filterText.Length == 0 || item.NativeName.ToLower().StartsWith(filterText.ToLower()) && item.LCID > 1000) {
                    displayingInfos.Add(item);
                    listBox1.Items.Add(item.NativeName);
                }
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e) {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < displayingInfos.Count) {
                selectedInfo = displayingInfos[listBox1.SelectedIndex];
            }
        }
        /// <summary>
        /// when changing the boot settings. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (!isDoneInitalizing) {
                return;
            }
            appSettings.RegiterOrUnRegister(checkBox1.Checked);
        }

        private void HideToTray() {
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(1000, "Keyboard layout forced", "The layout has been forced to: " + selectedInfo.NativeName, ToolTipIcon.Info);
            Hide();
        }

        private void notifyIcon1_Click(object sender, EventArgs e) {
            if (!Visible) { 
                Show();
            } else {
                Hide();
            }
        }
    }
}
