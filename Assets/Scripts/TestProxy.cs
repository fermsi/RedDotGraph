using System.Collections.Generic;
using RedDot.Runtime.RedDot;
using UnityEngine;

namespace RedDotDemo
{
    public class TestProxy
    {
        private static TestProxy _ins;

        private TestProxy()
        {
        }

        public static TestProxy Ins()
        {
            if (_ins == null)
            {
                _ins = new TestProxy();
            }

            return _ins;
        }

        public void RegVisitors()
        {
            RedDotExternalFuncVisitor.Ins().RegVisitor((int)RedDotVisitorEnum.Bag, RedDotFullBag);
            RedDotExternalFuncVisitor.Ins().RegVisitor((int)RedDotVisitorEnum.Equip, RedDotNewEquip);
            RedDotExternalFuncVisitor.Ins().RegVisitor((int)RedDotVisitorEnum.Email, RedDotNumberEmail);
            RedDotExternalFuncVisitor.Ins().RegVisitor((int)RedDotVisitorEnum.Task, RedDotNormalTask_ID);
        }

        public void InitData()
        {
            IsBagFull = true;
            IsEquipNew = true;
            EmailNum = 2;
            TaskStausChange(1001, false);
            TaskStausChange(1002, true);
            
        }

        private bool isBagFull = false;

        public bool IsBagFull
        {
            get => isBagFull;
            set
            { 
                isBagFull = value;
                RedDotManager.Ins().DataChange(RedDotKeyEnum.Bag.ToString()); 
            }
        }

        private void RedDotFullBag(uint id, out RedDotStatus status, out int num)
        {
            status = isBagFull ? RedDotStatus.RED_DOT_TYPE_FULL : RedDotStatus.RED_DOT_TYPE_NULL;
            num = 0;
        }

        private bool isEquipNew = false;
        public bool IsEquipNew
        {
            get => isEquipNew;
            set
            {
                isEquipNew = value;
                RedDotManager.Ins().DataChange(RedDotKeyEnum.Equip.ToString());
            }
        }
        private void RedDotNewEquip(uint id, out RedDotStatus status, out int num)
        {
            status = isEquipNew ? RedDotStatus.RED_DOT_TYPE_NEW : RedDotStatus.RED_DOT_TYPE_NULL;
            num = 0;
        }

        private int emailNum;

        public int EmailNum
        {
            get => emailNum;
            set
            {
                emailNum = value;
                RedDotManager.Ins().DataChange(RedDotKeyEnum.Email.ToString());
            }
        }

        private void RedDotNumberEmail(uint id, out RedDotStatus status, out int num)
        {
            status = emailNum > 0 ? RedDotStatus.RED_DOT_TYPE_NUMBER : RedDotStatus.RED_DOT_TYPE_NULL;
            num = emailNum;
        }

        private Dictionary<int, bool> taskNumDict = new Dictionary<int, bool>();

        public void TaskStausChange(int taskId, bool hasRedDot)
        {
            Debug.Log($"===taskId:{taskId}===hasRedDot:{hasRedDot}");
            taskNumDict[taskId] = hasRedDot;
            RedDotManager.Ins().DataChange(RedDotKeyEnum.Task.ToString(), (uint)taskId);
        }

        public bool GetTaskHasRedDot(int taskId)
        {
            return taskNumDict[taskId];
        }

        private void RedDotNormalTask_ID(uint id, out RedDotStatus status, out int num)
        {
            if (id == 0)
            {
                status = RedDotStatus.RED_DOT_TYPE_NULL;
                foreach (var taskInfo in taskNumDict)
                {
                    if (taskInfo.Value)
                    {
                        status = RedDotStatus.RED_DOT_TYPE_NORMAL;
                        break;
                    }
                }
            }
            else
            {
                status = taskNumDict[(int)id] ? RedDotStatus.RED_DOT_TYPE_NORMAL : RedDotStatus.RED_DOT_TYPE_NULL;
            }
            num = 0;
        }
    }
}