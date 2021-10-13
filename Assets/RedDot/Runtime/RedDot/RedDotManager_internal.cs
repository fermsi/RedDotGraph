namespace RedDot.Runtime.RedDot
{
    public partial class RedDotManager
    {
        private RedDotUIFrames _uiFramesNoId;
        private RedDotUIFrames _uiFramesHasId;
        protected RedDotGraph RedDotGrapic = new RedDotGraph();

        private RedDotManager()
        {
            _uiFramesNoId = new RedDotUIFrames();
            _uiFramesHasId = new RedDotUIFrames();
        }

        /// <summary>
        /// 添加红点顶点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redDotType"></param>
        /// <param name="externalId"></param>
        /// <returns></returns>
        protected int AddRedDotVertex(string key, int redDotType, int externalId = -1)
        {
            return RedDotGrapic.Insert(new RedDotVertex(key, redDotType, externalId));
        }

        /// <summary>
        /// 添加边
        /// </summary>
        /// <param name="vex1"></param>
        /// <param name="vex2"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        protected bool AddRedDotEdge(string vex1, string vex2, int weight = 0)
        {
            return RedDotGrapic.InsertEdge(vex1, vex2, weight);
        }

        private void TravelUIFrames(RedDotUIFrames redDotUiFrames)
        {
            foreach (var info in redDotUiFrames.GetCurrentFrameNode())
            {
                VisitRedDotUIStruct(info as RedDotUIInfo);
            }
//            RedDotUIInfo info = redDotUiFrames.FirstRedDotStruct();
//            while (info != null)
//            {
//                VisitRedDotUIStruct(info);
//                info = redDotUiFrames.NextRedDotStruct();
//            }
        }

        /// <summary>
        /// 节点访问，就是具体的刷红点数据
        /// </summary>
        /// <param name="v"></param>
        private void VisitRedDotVertex(RedDotVertex v, uint id)
        {
            var info = v.GetRuntimeInfo(id);
            RedDotExternalLogicVisitor.Ins().Visit(v.ExternalId, id, out info.RedDotStatus, out info.Value);
        }

        /// <summary>
        /// visit某个红点
        /// </summary>
        /// <param name="redDotUiInfo"></param>
        private void VisitRedDotUIStruct(RedDotUIInfo redDotUiInfo)
        {
            //key的红点解决了，但带ID的红点也解决了
            var v = RedDotGrapic.GetRedDotByKey(redDotUiInfo.Key);
            var info = v.GetRuntimeInfo(redDotUiInfo.Id);
            if (info.IsDataChange)
            {
                RedDotGrapic.RefreshRedDotStatus(v.Key, redDotUiInfo.Id, VisitRedDotVertex);
            }
            if (info.StatusChangeTIme > redDotUiInfo.LastChangeTime)
            {
                redDotUiInfo.LastChangeTime = info.StatusChangeTIme;
                RedDotStatusChange(redDotUiInfo);
            }
        }

        /// <summary>
        /// 红点状态发生了变化，需要发消息出去，变更状态
        /// </summary>
        /// <param name="redDotUiInfo"></param>
        private void RedDotStatusChange(RedDotUIInfo redDotUiInfo)
        {
            redDotUiInfo.DoCallBack();
        }
    }
}