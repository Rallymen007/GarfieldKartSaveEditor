using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GarfieldKartSaveLibrary {
    public class GarfieldKartSaveLoader {
        public GarfieldKartSave LoadSave(string path) {
            GarfieldKartSave save = new GarfieldKartSave();

            foreach(string filename in Directory.GetFiles(path)) {
                if (filename.EndsWith("b3B0aW9ucw==")) {
                    // Options file
                    save.OptionsFilename = filename;
                    save.Options = ParseOptionFile(filename);
                    File.Copy(filename, filename + ".bak", true);
                    continue;
                }
                if (filename.EndsWith("cHJvZ2Vzc2lvbg==")) {
                    // Progression
                    save.ProgressionFilename = filename;
                    save.Progression = ParseProgressionFile(filename);
                    File.Copy(filename, filename + ".bak", true);
                    continue;
                }
            }

            return save;
        }

        public void Save(GarfieldKartSave save) {
            File.WriteAllLines(save.OptionsFilename, SerializeOptions(save));
            File.WriteAllLines(save.ProgressionFilename, SerializeProgression(save));
        }

        public string[] SerializeOptions(GarfieldKartSave save) {
            List<string> ret = new List<string>();
            foreach(var opair in save.Options) {
                ret.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(opair.Key))
                    + ";"
                    + Convert.ToBase64String(Encoding.UTF8.GetBytes(opair.Value)));
            }
            return ret.ToArray();
        }

        public string[] SerializeProgression(GarfieldKartSave save) {
            List<string> ret = new List<string>();
            foreach (var opair in save.Progression) {
                ret.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(opair.Key))
                    + ";"
                    + Convert.ToBase64String(Encoding.UTF8.GetBytes(opair.Value)));
            }
            return ret.ToArray();
        }

        public Dictionary<string, string> ParseOptionFile(string filename) {
            Dictionary<string, string> options = new Dictionary<string, string>();
            foreach(string line in File.ReadAllLines(filename)) {
                string[] split = line.Split(';');

                byte[] data = Convert.FromBase64String(split[0]);
                string param = Encoding.UTF8.GetString(data);
                data = Convert.FromBase64String(split[1]);
                string value = Encoding.UTF8.GetString(data);

                options.Add(param, value);
            }
            return options;
        }

        public Dictionary<string, string> ParseProgressionFile(string filename) {
            Dictionary<string, string> progression = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(filename)) {
                string[] split = line.Split(';');

                byte[] data = Convert.FromBase64String(split[0]);
                string param = Encoding.UTF8.GetString(data);
                data = Convert.FromBase64String(split[1]);
                string value = Encoding.UTF8.GetString(data);

                progression.Add(param, value);
            }
            return progression;
        }
    }

    public class GarfieldKartSave {
        public string OptionsFilename { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public string ProgressionFilename { get; set; }
        public Dictionary<string, string> Progression { get; set; }

        public GarfieldKartSave() {
            Options = new Dictionary<string, string>();
            Progression = new Dictionary<string, string>();
        }

        public override string ToString() {
            string str = "GarfieldKartSave::\nOptions:\n";
            foreach(var opair in Options) {
                str += opair.Key + "::" + opair.Value + "\n";
            }
            str += "Progression:\n";
            foreach (var ppair in Progression) {
                str += ppair.Key + "::" + ppair.Value + "\n";
            }
            str += "-----";
            return str;
        }
    }
}
