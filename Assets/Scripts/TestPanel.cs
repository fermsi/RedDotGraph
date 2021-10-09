using RedDotDemo;
using UnityEngine;

public class TestPanel : MonoBehaviour
{
    public void OnClickFullBag()
    {
        TestProxy.Ins().IsBagFull = !TestProxy.Ins().IsBagFull;
    }

    public void OnClickNewEquip()
    {
        TestProxy.Ins().IsEquipNew = !TestProxy.Ins().IsEquipNew;
    }

    public void OnClickAddMail()
    {
        TestProxy.Ins().EmailNum++;
    }

    public void OnClickSubMail()
    {
        TestProxy.Ins().EmailNum--;//可能小于0
    }

    public void OnClickTask1001()
    {
        TestProxy.Ins().TaskStausChange(1001, !TestProxy.Ins().GetTaskHasRedDot(1001));
    }

    public void OnClickTask1002()
    {
        TestProxy.Ins().TaskStausChange(1002, !TestProxy.Ins().GetTaskHasRedDot(1002));
    }
}
