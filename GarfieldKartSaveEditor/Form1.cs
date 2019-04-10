using GarfieldKartSaveLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GarfieldKartSaveEditor {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            locationPath.Text = GetPath();
        }

        static string GetPath() {
            Guid localLowId = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");
            return GetKnownFolderPath(localLowId) + "\\Anuman Interactive\\Garfield Kart";
        }

        static string GetKnownFolderPath(Guid knownFolderId) {
            IntPtr pszPath = IntPtr.Zero;
            try {
                int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0)
                    return Marshal.PtrToStringAuto(pszPath);
                throw Marshal.GetExceptionForHR(hr);
            } finally {
                if (pszPath != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pszPath);
            }
        }

        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

        GarfieldKartSaveLoader loader;
        GarfieldKartSave save;
        private void button1_Click(object sender, EventArgs e) {
            try {
                loader = new GarfieldKartSaveLoader();
                save = loader.LoadSave(locationPath.Text);

                toolStripStatusLabel1.Text = "Loaded";
            } catch(Exception ex) {
                MessageBox.Show("ERROR loading\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "Error";
            }
        }

        string listSelected;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (save == null) {
                toolStripStatusLabel2.Text = "Load First";
                return;
            }
            listSelected = comboBox1.SelectedItem.ToString();
            RefreshList();
        }

        private void RefreshList() {
            switch (listSelected) {
                case "Options":
                    listBox1.DataSource = new BindingSource(save.Options, null);
                    listBox1.DisplayMember = "Key";
                    toolStripStatusLabel2.Text = "Options Selected";
                    break;
                case "Progression":
                    listBox1.DataSource = new BindingSource(save.Progression, null);
                    listBox1.DisplayMember = "Key";
                    toolStripStatusLabel2.Text = "Progression Selected";
                    break;
                default:
                    toolStripStatusLabel2.Text = "Nothing Selected";
                    break;
            }
        }

        string selectedValue;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            KeyValuePair<string, string> sel = (KeyValuePair<string, string>) listBox1.SelectedItem;
            toolStripStatusLabel3.Text = "Nothing Set";
            valueBox.Text = sel.Value;
            selectedValue = sel.Key;
        }

        private void button3_Click(object sender, EventArgs e) {
            switch (listSelected) {
                case "Options":
                    save.Options[selectedValue] = valueBox.Text;
                    toolStripStatusLabel3.Text = "Set";
                    break;
                case "Progression":
                    save.Progression[selectedValue] = valueBox.Text;
                    toolStripStatusLabel3.Text = "Set";
                    break;
                default:
                    break;
            }
            RefreshList();
        }

        private void button2_Click(object sender, EventArgs e) {
            if (save == null) {
                toolStripStatusLabel4.Text = "Load First";
                return;
            }
            try {
                loader.Save(save);

                toolStripStatusLabel4.Text = "Saved";
            } catch (Exception ex) {
                MessageBox.Show("ERROR saving\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel4.Text = "Error";
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            List<string> keys = new List<string>();
            foreach(var pair in save.Progression) {
                if (pair.Key.StartsWith("tm_")) {
                    keys.Add(pair.Key);
                }
            }
            foreach(var key in keys) {
                save.Progression[key] = "4";
            }
            RefreshList();
        }
    }
}
