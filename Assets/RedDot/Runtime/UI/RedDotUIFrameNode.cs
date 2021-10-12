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
        private Action Cb;
        public uint LastChangeTime = 0;
        public string Key { private set; get; }
        public uint Id { private set; get; }

        public RedDotUIInfo(string key, uint id, Action cb)
        {
            Cb = cb;
            Key = key;
            Id = id;
        }

        public void DoCallBack()
        {
            Cb.Invoke();
        }

        public bool IsSame(uint id, Action cb)
        {
            return Id == id && Cb == cb;
        }

    }
    /// <summary>
    /// 帧节点，一个全屏面板对应一个帧，刷新红点的时候，只刷新当前帧的红点即可
    /// </summary>
    public class FrameNode : IEnumerable
    {
        public Dictionary<string, List<RedDotUIInfo>> RedDotUiInfosDict = new Dictionary<string, List<RedDotUIInfo>>();

        public bool TryAddRedDotUIInfo(string redDotKey, Action cb, uint id = 0)
        {
            List<RedDotUIInfo> infos;
            if (!RedDotUiInfosDict.TryGetValue(redDotKey, out infos))
            {
                infos = new List<RedDotUIInfo>();
                RedDotUiInfosDict[redDotKey] = infos;
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
            List<RedDotUIInfo> infos = RedDotUiInfosDict[redDotKey];
            if (infos != null)
            {
                RedDotUIInfo info;
                for (int i = infos.Count - 1; i >= 0; i--)
                {
                    info = infos[i];
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
            foreach (var infos in RedDotUiInfosDict)
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
            List<RedDotUIInfo> infos;
            foreach (var key in keys)
            {
                if (RedDotUiInfosDict.TryGetValue(key, out infos))
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