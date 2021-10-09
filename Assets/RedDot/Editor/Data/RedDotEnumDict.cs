using System;
using System.Collections.Generic;
using System.Linq;
using RedDot.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace RedDot.Editor.Data
{
    [Serializable]
    public class RedDotEnumInfo
    {
        public int Id;
        public string Key;
        public string Desc;
    }
    [Serializable]
    public class RedDotEnumDict
    {
        private Dictionary<int, string> enumDict;
        private Dictionary<int, string> descDict;
        public List<RedDotEnumInfo> enumInfos = new List<RedDotEnumInfo>();

        public string classTemplate =
            "namespace RedDotDemo\n{\n\tpublic enum RedDotKeyEnum\n\t{\n[[key&values]]\n\t}\n}";
        public string keyValueReg = "\t\t[[key]] = [[value]],//[[desc]]";
        public string exportFileName = "RedDotKeyEnum.cs";
        private int maxId = 0;
        [NonSerialized]
        private RedDotGraphData graphData;
        private bool isChanged = false;
        public bool IsChanged => isChanged;
        private bool isCached = true;
        public bool IsCached => isCached;
        public void ClearChangedTag()
        {
            isChanged = false;
        }

        public List<int> GetIds()
        {
            return enumDict.Keys.ToList();
        }

        public List<string> GetNodeKeys()
        {
            var keys = new List<string>();
            keys.Add("null");
            foreach (var info in enumDict)
            {
                keys.Add($"{info.Key}:{info.Value}");
            }
            return keys;
        }

        public string GetNodeKey(int id)
        {
            string key;
            if (enumDict.TryGetValue(id, out key))
            {
                key = $"{id}:{key}";
            }
            else
            {
                key = "null";
            }

            return key;
        }

        public int GetIdByNodeKey(string key)
        {
            if (key == "null") return 0;
            return Convert.ToInt32(key.Split(':')[0]);
        }

        public void PrepareExport()
        {
            enumInfos.Clear();
            isCached = true;
            if (enumDict == null)
            {
                enumDict = new Dictionary<int, string>();
            }
            if (descDict == null)
            {
                descDict = new Dictionary<int, string>();
            }
            foreach (var info in enumDict)
            {
                enumInfos.Add(new RedDotEnumInfo(){Desc = GetDesc(info.Key), Id = info.Key, Key = info.Value});
            }
        }

        public void ValidateData(RedDotGraphData graphData)
        {
            this.graphData = graphData;
            enumDict = new Dictionary<int, string>();
            descDict = new Dictionary<int, string>();
            maxId = 0;
            foreach (var info in enumInfos)
            {
                var id = info.Id;
                enumDict[id] = info.Key;
                descDict[id] = info.Desc;
                if (id > maxId)
                {
                    maxId = id;
                }
            }
        }

        public int Add()
        {
            var id = ++maxId;
            enumDict[id] = "Default";
            descDict[id] = "加一点描述吧";
            graphData.Dirty();
            isChanged = true;
            isCached = false;
            return id;
        }

        public void Remove(int id)
        {
            enumDict.Remove(id);
            descDict.Remove(id);
            isChanged = true;
            isCached = false;
            graphData.Dirty();
        }

        public void ChangeKey(int id, string info)
        {
            if (enumDict.ContainsKey(id))
            {
                enumDict[id] = info;
                graphData.Dirty();
                isCached = false;
            }
        }

        public string GetKey(int id)
        {
            string key;
            if (!enumDict.TryGetValue(id, out key))
            {
                key = "null";
            }

            return key;
        }

        public void ChangeDesc(int id, string info)
        {
            if (descDict.ContainsKey(id))
            {
                descDict[id] = info;
                graphData.Dirty();
                isCached = false;
            }
        }

        public string GetDesc(int id)
        {
            string key;
            if (!descDict.TryGetValue(id, out key))
            {
                key = "null";
            }

            return key;
        }

        public void ChangeKeyAndValueReg(string newReg)
        {
            keyValueReg = newReg;
            isChanged = true;
            graphData.Dirty();
        }

        public void ChangeClassTemplate(string newTemplate)
        {
            classTemplate = newTemplate;
            isChanged = true;
            graphData.Dirty();
        }

        public void ChangeExportFileName(string fileName)
        {
            exportFileName = fileName;
            isChanged = true;
            graphData.Dirty();
        }

        public void ExportTo(string filePath)
        {
            //[[key]] = [[value]];//[[desc]]
            var keyAndValuesStr = "";
            var endingSymble = "";
            foreach (var info in enumDict)
            {
                var id = info.Key;
                var desc = GetDesc(id);
                keyAndValuesStr += endingSymble + keyValueReg.Replace("[[key]]", info.Value).
                    Replace("[[value]]", id.ToString()).
                    Replace("[[desc]]", desc);
                endingSymble = "\n";
            }
            //[[key&values]]
            var code = classTemplate.Replace("[[key&values]]", keyAndValuesStr);
            Debug.Log(code);
            FileUtilities.WriteToDisk(filePath, code);
            AssetDatabase.Refresh();
        }
    }
}