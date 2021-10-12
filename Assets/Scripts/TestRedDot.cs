using RedDotDemo;
using UnityEngine;
using RedDot.Runtime.RedDot;

public class TestRedDot : MonoBehaviour
{
    public GameObject panelContainer;
    void Start()
    {
        TestProxy.Ins().RegVisitors();//注入所有红点逻辑方法
        RedDotManager.Ins().AddJsonFile("Assets/Resources/test.json");//加载红点图配置
        TestProxy.Ins().InitData();//模拟正式数据初始化

        RedDotManager.Ins().AddFrame("TestPanel");//切换面板
        var go = Resources.Load<GameObject>("Prefabs/TestPanel");
        go = Instantiate(go, panelContainer.transform);
        go.transform.localPosition = Vector3.zero;
    }
}
