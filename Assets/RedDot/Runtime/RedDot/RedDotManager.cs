using System;
using UnityEngine;

namespace RedDot.Runtime.RedDot
{
    public partial class RedDotManager
    {
        private static RedDotManager _ins = null;
        public static RedDotManager Ins()
        {
            if (_ins == null)
            {
                _ins = new RedDotManager();
            }
            return _ins;
        }

        /// <summary>
        /// 是否是由GetRedDotStatus来启动刷新红点
        /// GetRedDotStatus里的刷新红点和RefreshRedDot方法是有些性能的差别的，
        /// 最好是统一调用RefreshRedDot来刷新红点
        /// </summary>
        public bool IsUseRefreshByGetRedDotStatus = false;
        
        /// <summary>
        /// UI绑定，目前还没有想好是用回调还是发消息的机制
        /// </summary>
        /// <param name="redDotKey">红点关键码</param>
        /// <param name="id">红点ID</param>
        /// <param name="cb"></param>
        public void AddUI(string redDotKey, Action cb, uint id = 0)
        {
            if (id == 0)
            {
                uiFramesNoId.AddUI(redDotKey, cb);
            }
            else
            {
                uiFramesHasId.AddUI(redDotKey, cb, id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redDotKey"></param>
        /// <param name="id"></param>
        /// <param name="cb"></param>
        public void RemoveUI(string redDotKey, Action cb, uint id = 0)
        {
            if (redDotKey == null || redDotKey == "")
            {
                Debug.LogWarning($"解绑定的红点key为空");
                return;
            }

            if (cb == null)
            {
                Debug.LogWarning($"解绑定的红点cb为空");
                return;
            }
            if (id == 0)
            {
                uiFramesNoId.RemoveUI(redDotKey, cb);
            }
            else
            {
                uiFramesHasId.RemoveUI(redDotKey, cb, id);
            }
        }

        /// <summary>
        /// 数据有变化
        /// </summary>
        /// <param name="key"></param>
        public void DataChange(string key)
        {
            var changedList = redDotGraph.RedDotDataChange(key);
            uiFramesNoId.GetCurrentFrameNode()?.DataChanged(changedList, 0);
        }

        /// <summary>
        /// 带id的数据有变化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        public void DataChange(string key, uint id)
        {
            var changedList = redDotGraph.RedDotDataChange(key, id);
            uiFramesHasId.GetCurrentFrameNode()?.DataChanged(changedList, id);
            if (id != 0) DataChange(key);
        }

        /// <summary>
        /// 刷新红点
        /// 和GetRedDotStatus的及时刷新有功能冲突，需要决策调用那个
        /// </summary>
        public void RefreshRedDot()
        {
            TravelUIFrames(uiFramesNoId);
            TravelUIFrames(uiFramesHasId);
        }

        /// <summary>
        /// 切换到某个全屏UI上
        /// </summary>
        /// <param name="name"></param>
        public void ChangeToFrame(string name)
        {
            uiFramesNoId.ChangeToFrame(name);
            uiFramesHasId.ChangeToFrame(name);
        }

        public void AddFrame(string name)
        {
            uiFramesNoId.AddFrame(name);
            uiFramesHasId.AddFrame(name);
        }

        public void RemoveFrame(string name)
        {
            uiFramesNoId.RemoveFrame(name);
            uiFramesHasId.RemoveFrame(name);
        }

        /// <summary>
        /// 获取红点的状态
        /// 如果数据有刷新，则需要及时的刷一下，这里采用的是及时刷。
        /// 要不要及时刷由框架决定，如果调用RefreshRedDot，就不需要及时刷新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="num"></param>
        public void GetRedDotStatus(string key, uint id, out RedDotStatus status, out int num)
        {
            var v = redDotGraph.GetRedDotByKey(key);
            var info = v.GetRuntimeInfo(id);
            //如果数据有刷新，则需要及时的刷一下，这里采用的是及时刷。
            //要不要及时刷由框架决定，如果调用RefreshRedDot，就不需要及时刷新//todo
//            if (IsUseRefreshByGetRedDotStatus && info.isDataChange)
            if (!IsUseRefreshByGetRedDotStatus && info.isDataChange)
            {
                redDotGraph.RefreshRedDotStatus(key, id, VisitLeafRedDotVertex);
            }
            status = info.redDotStatus;
            num = info.value;
        }
    }
}