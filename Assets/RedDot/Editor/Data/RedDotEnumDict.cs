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
        public int id;
        public string key;
        public string desc;
    }
    [Serializable]
    public class RedDotEnumDict
    {
        private Dictionary<int, string> _enumDict;
        private Dictionary<int, string> _descDict;
        public List<RedDotEnumInfo> enumInfos = new List<RedDotEnumInfo>();

        public string classTemplate =
            "namespace RedDotDemo\n{\n\tpublic enum RedDotKeyEnum\n\t{\n[[key&values]]\n\t}\n}";
        public string keyValueReg = "\t\t[[key]] = [[value]],//[[desc]]";
        public string exportFileName = "RedDotKeyEnum.cs";
        private int _maxId = 0;
        [NonSerialized]
        private RedDotGraphData _graphData;
        private bool _isChanged = false;
        public bool IsChanged => _isChanged;
        private bool _isCached = true;
        public bool IsCached => _isCached;
        public void ClearChangedTag()
        {
            _isChanged = false;
        }

        public List<int> GetIds()
        {
            return _enumDict.Keys.ToList();
        }

        public List<string> GetNodeKeys()
        {
            var keys = new List<string>();
            keys.Add("null");
            foreach (var info in _enumDict)
            {
                keys.Add($"{info.Key}:{info.Value}");
            }
            return keys;
        }

        public string GetNodeKey(int id)
        {
            string key;
            if (_enumDict.TryGetValue(id, out key))
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
            _isCached = true;
            if (_enumDict == null)
            {
                _enumDict = new Dictionary<int, string>();
            }
            if (_descDict == null)
            {
                _descDict = new Dictionary<int, string>();
            }
            foreach (var info in _enumDict)
            {
                enumInfos.Add(new RedDotEnumInfo(){desc = GetDesc(info.Key), id = info.Key, key = info.Value});
            }
        }

        public void ValidateData(RedDotGraphData graphData)
        {
            _graphData = graphData;
            _enumDict = new Dictionary<int, string>();
            _descDict = new Dictionary<int, string>();
            _maxId = 0;
            foreach (var info in enumInfos)
            {
                var id = info.id;
                _enumDict[id] = info.key;
                _descDict[id] = info.desc;
                if (id > _maxId)
                {
                    _maxId = id;
                }
            }
        }

        public int Add()
        {
            var id = ++_maxId;
            _enumDict[id] = "Default";
            _descDict[id] = "加一点描述吧";
            _graphData.Dirty();
            _isChanged = true;
            _isCached = false;
            return id;
        }

        public void Remove(int id)
        {
            _enumDict.Remove(id);
            _descDict.Remove(id);
            _isChanged = true;
            _isCached = false;
            _graphData.Dirty();
        }

        public void ChangeKey(int id, string info)
        {
            if (_enumDict.ContainsKey(id))
            {
                _enumDict[id] = info;
                _graphData.Dirty();
                _isCached = false;
            }
        }

        public string GetKey(int id)
        {
            string key;
            if (!_enumDict.TryGetValue(id, out key))
            {
                key = "null";
            }

            return key;
        }

        public void ChangeDesc(int id, string info)
        {
            if (_descDict.ContainsKey(id))
            {
                _descDict[id] = info;
                _graphData.Dirty();
                _isCached = false;
            }
        }

        public string GetDesc(int id)
        {
            string key;
            if (!_descDict.TryGetValue(id, out key))
            {
                key = "null";
            }

            return key;
        }

        public void ChangeKeyAndValueReg(string newReg)
        {
            keyValueReg = newReg;
            _isChanged = true;
            _graphData.Dirty();
        }

        public void ChangeClassTemplate(string newTemplate)
        {
            classTemplate = newTemplate;
            _isChanged = true;
            _graphData.Dirty();
        }

        public void ChangeExportFileName(string fileName)
        {
            exportFileName = fileName;
            _isChanged = true;
            _graphData.Dirty();
        }

        public void ExportTo(string filePath)
        {
            //[[key]] = [[value]];//[[desc]]
            var keyAndValuesStr = "";
            var endingSymble = "";
            foreach (var info in _enumDict)
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