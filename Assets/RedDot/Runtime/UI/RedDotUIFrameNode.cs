using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedDot.Runtime.RedDot
{
    /// <summary>
    /// 红点UI节点
    /// </summary>
    public class RedDotUIInfo
    {
        private readonly Action _cb;
        public uint LastChangeTime = 0;
        public string Key { private set; get; }
        public uint Id { private set; get; }

        public RedDotUIInfo(string key, uint id, Action cb)
        {
            _cb = cb;
            Key = key;
            Id = id;
        }

        public void DoCallBack()
        {
            _cb.Invoke();
        }

        public bool IsSame(uint id, Action cb)
        {
            return Id == id && _cb == cb;
        }

    }
    /// <summary>
    /// 帧节点，一个全屏面板对应一个帧，刷新红点的时候，只刷新当前帧的红点即可
    /// </summary>
    public class FrameNode : IEnumerable
    {
        private readonly Dictionary<string, List<RedDotUIInfo>> _redDotUIInfosDict = new Dictionary<string, List<RedDotUIInfo>>();

        public bool TryAddRedDotUIInfo(string redDotKey, Action cb, uint id = 0)
        {
            if (!_redDotUIInfosDict.TryGetValue(redDotKey, out var infos))
            {
                infos = new List<RedDotUIInfo>();
                _redDotUIInfosDict[redDotKey] = infos;
            }

            foreach (var info in infos)
            {
                if (info.IsSame(id, cb))
                {
                    Debug.LogWarning($"add same UI, redDotKey:{redDotKey}, id:{id}");
                    return false;
                }
            }
            infos.Add(new RedDotUIInfo(redDotKey, id, cb));
            return true;
        }

        public RedDotUIInfo RemoveRedDotUIInfo(string redDotKey, Action cb, uint id = 0)
        {
            List<RedDotUIInfo> infos = _redDotUIInfosDict[redDotKey];
            if (infos != null)
            {
                for (int i = infos.Count - 1; i >= 0; i--)
                {
                    var info = infos[i];
                    if (info.IsSame(id, cb))
                    {
                        infos.RemoveAt(i);
                        return info;
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
                
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var infos in _redDotUIInfosDict)
            {
                foreach (var info in infos.Value)
                {
                    yield return info;
                }
            }
        }

        public void DataChanged(List<string> keys, uint id)
        {
            Debug.Log($"DataChanged keys:{string.Join(", ", keys)}, id:{id}");
            foreach (var key in keys)
            {
                if (_redDotUIInfosDict.TryGetValue(key, out var infos))
                {
                    foreach (var info in infos)
                    {
                        if (info.Id == id)
                        {
                            info.DoCallBack();
                        }
                    }
                }
            }
        }
    }
}