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
        private static RedDotExternalLogicVisitor s_ins;
        public static RedDotExternalLogicVisitor Ins()
        {
            if (s_ins == null)
            {
                s_ins = new RedDotExternalLogicVisitor();
            }
            return s_ins;
        }

        private RedDotExternalLogicVisitor()
        {
        }

        private Dictionary<int, RedDotExternalLogic> _visitors = new Dictionary<int, RedDotExternalLogic>();
        private Dictionary<int, RedDotExternalLogicWithId> _visitorsWithId = new Dictionary<int, RedDotExternalLogicWithId>();

        public void RegExternalLogic(int externalId, RedDotExternalLogic externalLogic)
        {
            _visitors[externalId] = externalLogic;
        }

        public void RegExternalLogic(int externalId, RedDotExternalLogicWithId externalLogic)
        {
            _visitorsWithId[externalId] = externalLogic;
        }

        public void Visit(int externalId, uint id, out RedDotStatus status, out int num)
        {
            if (id == 0)
            {
                if (_visitors.TryGetValue(externalId, out RedDotExternalLogic visitor))
                {
                    visitor(out status, out num);
                    return;
                }
            }
            if (_visitorsWithId.TryGetValue(externalId, out RedDotExternalLogicWithId visitorId))
            {
                visitorId(id, out status, out num);
                return;
            }

            status = 0;
            num = 0;
            Console.WriteLine($"RedDotExternalFunVisitor.visitors里没有找到externalId : {externalId}");
        }

        public bool CheckHasExternalId(int externalId)
        {
            return _visitors.ContainsKey(externalId);
        }
    }
}