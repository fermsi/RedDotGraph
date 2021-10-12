using System;
using System.Collections.Generic;

namespace RedDot.Runtime.RedDot
{
    /// <summary>
    /// 红点外部方法访问器
    /// </summary>
    public partial class RedDotExternalLogicVisitor
    {
        public delegate void RedDotExternalLogic(out RedDotStatus status, out int num);
        public delegate void RedDotExternalLogicWithId(uint id, out RedDotStatus status, out int num);
        private static RedDotExternalLogicVisitor _ins = null;
        public static RedDotExternalLogicVisitor Ins()
        {
            if (_ins == null)
            {
                _ins = new RedDotExternalLogicVisitor();
            }
            return _ins;
        }

        private RedDotExternalLogicVisitor()
        {
        }

        private Dictionary<int, RedDotExternalLogic> visitors = new Dictionary<int, RedDotExternalLogic>();
        private Dictionary<int, RedDotExternalLogicWithId> visitorsWithId = new Dictionary<int, RedDotExternalLogicWithId>();

        public void RegExternalLogic(int externalId, RedDotExternalLogic externalLogic)
        {
            visitors[externalId] = externalLogic;
        }

        public void RegExternalLogic(int externalId, RedDotExternalLogicWithId externalLogic)
        {
            visitorsWithId[externalId] = externalLogic;
        }

        public void Visit(int externalId, uint id, out RedDotStatus status, out int num)
        {
            if (id == 0)
            {
                if (visitorsWithId.TryGetValue(externalId, out RedDotExternalLogicWithId visitor))
                {
                    visitor(id, out status, out num);
                    return;
                }
            }
            else if (visitors.TryGetValue(externalId, out RedDotExternalLogic visitor))
            {
                visitor(out status, out num);
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