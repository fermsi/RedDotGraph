using System;
using System.Collections.Generic;

namespace RedDot.Runtime.RedDot
{
    /// <summary>
    /// 红点外部方法访问器
    /// </summary>
    public partial class RedDotExternalFuncVisitor
    {
        public delegate void RedDotVisitor(uint id, out RedDotStatus status, out int num);
        private static RedDotExternalFuncVisitor _ins = null;
        public static RedDotExternalFuncVisitor Ins()
        {
            if (_ins == null)
            {
                _ins = new RedDotExternalFuncVisitor();
            }
            return _ins;
        }

        private RedDotExternalFuncVisitor()
        {
        }

        private Dictionary<int, RedDotVisitor> visitors = new Dictionary<int, RedDotVisitor>();

        public void RegVisitor(int externalId, RedDotVisitor visitor)
        {
            visitors[externalId] = visitor;
        }

        public void Visit(int externalId, uint id, out RedDotStatus status, out int num)
        {
            if (visitors.TryGetValue(externalId, out RedDotVisitor visitor))
            {
                visitor(id, out status, out num);
                return;
            }

            status = 0;
            num = 0;
            Console.WriteLine($"RedDotExternalFunVisitor.visitors里没有找到externalId : {externalId}");
        }

        public bool CheckHasExternalId(int externalId)
        {
            return visitors.ContainsKey(externalId);
        }
    }
}