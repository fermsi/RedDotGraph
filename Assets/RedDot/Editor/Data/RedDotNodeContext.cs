using System;
using System.Collections.Generic;
using RedDot.Editor.Config;
using UnityEngine;

namespace RedDot.Editor.Data
{
    [Serializable]
    public class RedDotNodeContext
    {
        public int Id;
        public Vector2 Position;
        public List<int> OutNodeIds = new List<int>();
        public int KeyId;
        public int FuncId;
        public int RedDotType;

        public void RemoveOutNode(int id)
        {
            OutNodeIds.Remove(id);
        }

        public void AddOutNode(int id)
        {
            OutNodeIds.Add(id);
        }
    }
}