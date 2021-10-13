using System.Collections.Generic;
using RedDot.Runtime.RedDot;
using UnityEngine;

namespace RedDotDemo
{
    public class TestProxy
    {
        private static TestProxy s_ins;

        private TestProxy()
        {
        }

        public static TestProxy Ins()
        {
            return s_ins ?? (s_ins = new TestProxy());
        }

        public void RegVisitors()
        {
            RedDotExternalLogicVisitor.Ins().RegExternalLogic((int)RedDotExternalLogicEnum.Bag, RedDotFullBag);
            RedDotExternalLogicVisitor.Ins().RegExternalLogic((int)RedDotExternalLogicEnum.Equip, RedDotNewEquip);
            RedDotExternalLogicVisitor.Ins().RegExternalLogic((int)RedDotExternalLogicEnum.Email, RedDotNumberEmail);
            RedDotExternalLogicVisitor.Ins().RegExternalLogic((int)RedDotExternalLogicEnum.Task, RedDotNormalTask_ID);
            RedDotExternalLogicVisitor.Ins().RegExternalLogic((int)RedDotExternalLogicEnum.Task, RedDotNormalTask);
        }

        public void InitData()
        {
            IsBagFull = true;
            IsEquipNew = true;
            EmailNum = 2;
            TaskStausChange(1001, false);
            TaskStausChange(1002, true);
            
        }

        private bool _isBagFull = false;

        public bool IsBagFull
        {
            get => _isBagFull;
            set
            { 
                _isBagFull = value;
                RedDotManager.Ins().DataChange(RedDotKeyEnum.Bag.ToString()); 
            }
        }

        private void RedDotFullBag(out RedDotStatus status, out int num)
        {
            status = _isBagFull ? RedDotStatus.RedDotStatusFull : RedDotStatus.RedDotStatusNull;
            num = 0;
        }

        private bool _isEquipNew = false;
        public bool IsEquipNew
        {
            get => _isEquipNew;
            set
            {
                _isEquipNew = value;
                RedDotManager.Ins().DataChange(RedDotKeyEnum.Equip.ToString());
            }
        }
        private void RedDotNewEquip(out RedDotStatus status, out int num)
        {
            status = _isEquipNew ? RedDotStatus.RedDotStatusNew : RedDotStatus.RedDotStatusNull;
            num = 0;
        }

        private int _emailNum;

        public int EmailNum
        {
            get => _emailNum;
            set
            {
                _emailNum = value;
                RedDotManager.Ins().DataChange(RedDotKeyEnum.Email.ToString());
            }
        }

        private void RedDotNumberEmail(out RedDotStatus status, out int num)
        {
            status = _emailNum > 0 ? RedDotStatus.RedDotStatusNumber : RedDotStatus.RedDotStatusNull;
            num = _emailNum;
        }

        private Dictionary<int, bool> _taskNumDict = new Dictionary<int, bool>();

        public void TaskStausChange(int taskId, bool hasRedDot)
        {
            Debug.Log($"TaskStausChange taskId:{taskId}, hasRedDot:{hasRedDot}");
            _taskNumDict[taskId] = hasRedDot;
            RedDotManager.Ins().DataChange(RedDotKeyEnum.Task.ToString(), (uint)taskId);
        }

        public bool GetTaskHasRedDot(int taskId)
        {
            return _taskNumDict[taskId];
        }

        private void RedDotNormalTask_ID(uint id, out RedDotStatus status, out int num)
        {
            status = _taskNumDict[(int)id] ? RedDotStatus.RedDotStatusNormal : RedDotStatus.RedDotStatusNull;
            num = 0;
        }

        private void RedDotNormalTask(out RedDotStatus status, out int num)
        {
            status = RedDotStatus.RedDotStatusNull;
            num = 0;
            foreach (var taskInfo in _taskNumDict)
            {
                if (taskInfo.Value)
                {
                    status = RedDotStatus.RedDotStatusNormal;
                    break;
                }
            }
        }
    }
}