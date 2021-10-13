using System;
using System.Collections.Generic;
using RedDot.Editor.Config;
using UnityEngine;

namespace RedDot.Editor.Data
{
    [Serializable]
    public class RedDotNodeContext
    {
        public int id;
        public Vector2 position;
        public List<int> outNodeIds = new List<int>();
        public int keyId;
        public int funcId;
        public int redDotType;

        public void RemoveOutNode(int id)
        {
            outNodeIds.Remove(id);
        }

        public void AddOutNode(int id)
        {
            outNodeIds.Add(id);
        }
    }
}