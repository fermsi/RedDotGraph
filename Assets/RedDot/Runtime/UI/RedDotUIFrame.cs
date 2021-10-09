using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedDot.Runtime.RedDot
{
    /// <summary>
    /// 1、RedDotUIMaps是基于帧+脏标记的方法实现的，不支持没有显示（或者已经隐藏的，不在当前帧下）的页面添加红点，
    /// 因为添加红点的话，一定是在当前最上层帧里添加的。
    /// 只要帧一还原，之前的帧数据会全部删除。
    /// 2、如果想要解决这个问题，可以吧脏标记功能禁用，is_use_dirty_state = false即可，禁用后，每次删除UI，都会修改cbDict，所以看项目需求
    /// 
    /// </summary>
    public class RedDotUIFrames
    {
        private Dictionary<string, FrameNode> frameNodeDict = new Dictionary<string, FrameNode>();
        private FrameNode curFrameNode;

        public void AddUI(string redDotKey, Action cb, uint id = 0)
        {
            var frameNode = curFrameNode;
            frameNode.TryAddRedDotUIInfo(redDotKey, cb, id);
        }

        /// <summary>
        /// 某个UI删除了
        /// </summary>
        /// <param name="redDotKey"></param>
        /// <param name="cb"></param>
        /// <param name="id"></param>
        public void RemoveUI(string redDotKey, Action cb, uint id = 0)
        {
            var frameNode = curFrameNode;
            frameNode.RemoveRedDotUIInfo(redDotKey, cb, id);
        }

        public void AddFrame(string name)
        {
            if (!frameNodeDict.ContainsKey(name))
            {
                var node = new FrameNode();
                curFrameNode = node;
                frameNodeDict[name] = node;
            }
            else
            {
                Debug.LogWarning($"已经存在了此frame:{name}");
            }
        }

        public void RemoveFrame(string name)
        {
            if (frameNodeDict.TryGetValue(name, out FrameNode node))
            {
                frameNodeDict.Remove(name);
                node.Dispose();
            }
            else
            {
                Debug.LogWarning($"不存在此frame:{name}");
            }
        }

        public void ChangeToFrame(string name)
        {
            if (frameNodeDict.TryGetValue(name, out FrameNode node))
            {
                curFrameNode = node;
            }
            else
            {
                Debug.LogWarning($"不存在此frame:{name}");
            }
        }

        public FrameNode GetCurrentFrameNode()
        {
            return curFrameNode;
        }
    }
}