using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExileLootDrop
{
    public class IniParser
    {
        private readonly string _iniFilePath;
        private readonly Hashtable _keyPairs = new Hashtable();
        private readonly List<SectionPair> _tmpList = new List<SectionPair>();

        public readonly string Name;

        public IniParser(string iniPath)
        {
            string str2 = null;
            _iniFilePath = iniPath;
            Name = Path.GetFileNameWithoutExtension(iniPath);

            if (!File.Exists(iniPath))
                throw new FileNotFoundException("Unable to locate " + iniPath);

            using (TextReader reader = new StreamReader(iniPath))
            {
                for (var str = reader.ReadLine(); str != null; str = reader.ReadLine())
                {
                    str = str.Trim();
                    if (str == "")
                        continue;

                    if (str.StartsWith("[") && str.EndsWith("]"))
                        str2 = str.Substring(1, str.Length - 2);
                    else
                    {
                        SectionPair pair;

                        if (str.StartsWith(";"))
                            str = str.Replace("=", "%eq%") + @"=%comment%";

                        var strArray = str.Split(new[] { '=' }, 2);
                        string str3 = null;
                        if (str2 == null)
                        {
                            str2 = "ROOT";
                        }
                        pair.Section = str2;
                        pair.Key = strArray[0];
                        if (strArray.Length > 1)
                        {
                            str3 = strArray[1];
                        }
                        _keyPairs.Add(pair, str3);
                        _tmpList.Add(pair);
                    }
                }
            }
        }

        public void AddSetting(string sectionName, string settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        public void AddSetting(string sectionName, string settingName, string settingValue)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (_keyPairs.ContainsKey(pair))
            {
                _keyPairs.Remove(pair);
            }
            if (_tmpList.Contains(pair))
            {
                _tmpList.Remove(pair);
            }
            _keyPairs.Add(pair, settingValue);
            _tmpList.Add(pair);
        }

        public int Count()
        {
            return Sections.Length;
        }

        public void DeleteSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (!_keyPairs.ContainsKey(pair)) return;
            _keyPairs.Remove(pair);
            _tmpList.Remove(pair);
        }

        public string[] EnumSection(string sectionName)
        {
            return (from pair in _tmpList where !pair.Key.StartsWith(";") where pair.Section == sectionName select pair.Key).ToArray();
        }

        public string[] Sections => (from pair in _tmpList
                                     group pair by pair.Section into bySection
                                     from IGrouping<string, SectionPair> g in bySection
                                     select g.Key).ToArray();

        public string GetSetting(string sectionName, string settingName, string defaultValue = "")
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            return !_keyPairs.ContainsKey(pair) ? defaultValue : ((string)_keyPairs[pair]).Trim();
        }

        public bool GetBoolSetting(string sectionName, string settingName, bool defaultValue = false)
        {
            if (defaultValue)
                return (GetSetting(sectionName, settingName).ToLower() != "false");
            return (GetSetting(sectionName, settingName).ToLower() == "true");
        }

        public void Save()
        {
            SaveSettings(_iniFilePath);
        }

        public void SaveSettings(string newFilePath)
        {
            var list = new ArrayList();
            var str2 = "";
            foreach (var pair in _tmpList.Where(pair => !list.Contains(pair.Section)))
            {
                list.Add(pair.Section);
            }
            foreach (string str3 in list)
            {
                str2 = str2 + "[" + str3 + "]\r\n";
                foreach (var pair2 in _tmpList)
                {
                    if (pair2.Section == str3)
                    {
                        var str = (string)_keyPairs[pair2];
                        if (str != null)
                        {
                            if (str == "%comment%")
                            {
                                str = "";
                            }
                            else
                            {
                                str = "=" + str;
                            }
                        }
                        str2 = str2 + pair2.Key.Replace("%eq%", "=") + str + "\r\n";
                    }
                }
                str2 = str2 + "\r\n";
            }

            using (TextWriter writer = new StreamWriter(newFilePath))
                writer.Write(str2);
        }

        public void SetSetting(string sectionName, string settingName, string value)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (_keyPairs.ContainsKey(pair))
            {
                _keyPairs[pair] = value;
            }
            else
            {
                AddSetting(sectionName, settingName, value);
            }
        }

        public bool ContainsSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            return _keyPairs.Contains(pair);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SectionPair
        {
            public string Section;
            public string Key;
        }
    }
}
