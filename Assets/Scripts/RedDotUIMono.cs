using RedDotDemo;
using RedDot.Runtime.RedDot;
using UnityEngine;
using UnityEngine.UI;

public class RedDotUIMono : MonoBehaviour
{
    public Image img;
    public Text numTxt;

    private string _key;
    public RedDotKeyEnum keyEnum;
    public uint id;

    // Start is called before the first frame update
    void Start()
    {
        _key = keyEnum.ToString();
        Debug.Log($"RedDotUIMono Start key:{_key}");
        RedDotManager.Ins().AddUI(_key, RedDotStatusUpdate, id);
        RedDotStatusUpdate();
    }

    private void RedDotStatusUpdate()
    {
        RedDotManager.Ins().GetRedDotStatus(_key, id, out var status, out var num);
        Debug.Log($"RedDotUIMono RedDotStatusUpdate key:{_key}, id:{id}, status:{status}, num:{num}");
        string icon = "redDotNormal";
        switch (status)
        {
            case RedDotStatus.RedDotStatusNull:
                img.enabled = false;
                numTxt.text = "";
                return;
            case RedDotStatus.RedDotStatusNormal:
                img.enabled = true;
                numTxt.text = "";
//                icon = "redDotNormal";
                break;
            case RedDotStatus.RedDotStatusNumber:
                img.enabled = true;
                numTxt.text = num.ToString();
//                icon = "redDotNormal";
                break;
            case RedDotStatus.RedDotStatusNew:
                img.enabled = true;
                numTxt.text = "";
                icon = "redDotNew";
                break;
            case RedDotStatus.RedDotStatusFull:
                img.enabled = true;
                numTxt.text = "";
                icon = "redDotFull";
                break;
            default:
                Debug.LogError($"未处理的红点状态：{status}");
                break;
        }

        img.sprite = Resources.Load<Sprite>($"Icon/RedDot/{icon}");
    }

    // Update is called once per frame
    void OnDestroy()
    {
        RedDotManager.Ins().RemoveUI(_key, RedDotStatusUpdate, id);
    }
}