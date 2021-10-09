using RedDotDemo;
using RedDot.Runtime.RedDot;
using UnityEngine;
using UnityEngine.UI;

public class RedDotUIMono : MonoBehaviour
{
    public Image img;
    public Text numTxt;

    private string key;
    public RedDotKeyEnum keyEnum;
    public uint id;
    
    // Start is called before the first frame update
    void Awake()
    {
        key = keyEnum.ToString();
        Debug.Log($"=======key:{key}");
        RedDotManager.Ins().AddUI(key, RedDotStatusUpdate, id);
        RedDotStatusUpdate();
    }

    private void RedDotStatusUpdate()
    {
        RedDotStatus status;
        int num;
        RedDotManager.Ins().GetRedDotStatus(key, id, out status, out num);
        Debug.Log($"----key:{key}----id:{id}------status:{status}-----num:{num}----");
        string icon = "redDotNormal";
        switch (status)
        {
            case RedDotStatus.RED_DOT_TYPE_NULL:
                img.enabled = false;
                numTxt.text = "";
                return;
            case RedDotStatus.RED_DOT_TYPE_NORMAL:
                img.enabled = true;
                numTxt.text = "";
//                icon = "redDotNormal";
                break;
            case RedDotStatus.RED_DOT_TYPE_NUMBER:
                img.enabled = true;
                numTxt.text = num.ToString();
//                icon = "redDotNormal";
                break;
            case RedDotStatus.RED_DOT_TYPE_NEW:
                img.enabled = true;
                numTxt.text = "";
                icon = "redDotNew";
                break;
            case RedDotStatus.RED_DOT_TYPE_FULL:
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
        RedDotManager.Ins().RemoveUI(key, RedDotStatusUpdate, id);
    }
}
