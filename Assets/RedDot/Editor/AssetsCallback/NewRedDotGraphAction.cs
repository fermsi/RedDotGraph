using RedDot.Editor.Data;
using UnityEditor.ProjectWindowCallback;

namespace RedDot.Editor.AssetsCallback
{
    public class NewRedDotGraphAction: EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var redDotGraphData = new RedDotGraphData();
            redDotGraphData.SaveTo(pathName);

//            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<RedDotGraphData>(pathName);
//            Selection.activeObject = obj;
        }
    }
}