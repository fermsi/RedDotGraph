using System.Collections.Generic;

namespace RedDot.Editor.Config
{
    public enum RedDotNodeContextStatus
    {
        NORMAL = 1,
        NUMBER = 2,
        NEW = 4,
        FULL = 8,
    }
    
    public class RedDotStatusConfig
    {
        public readonly string StatusDesc;
        public readonly string Icon;

        public RedDotStatusConfig(string statusDesc, string icon)
        {
            StatusDesc = statusDesc;
            Icon = icon;
        }
    }
    
    public class RedDotEditorConfig
    {
        public const string Extension = "reddotgraph";
        public const string ExportExtension = "json";
        public const string KEY_WINDOW_TITLE = "Key设置";
        public const string EXTERNAL_ID_WINDOW_TITLE = "逻辑ID设置";

        private static Dictionary<RedDotNodeContextStatus, RedDotStatusConfig> statusConfig;

        private static void InitConfig()
        {
            if (statusConfig != null) return;
            statusConfig = new Dictionary<RedDotNodeContextStatus, RedDotStatusConfig>();
            statusConfig.Add(RedDotNodeContextStatus.NORMAL, new RedDotStatusConfig(
                "红点",
                "Icons/icon_normal"
                ));
            statusConfig.Add(RedDotNodeContextStatus.FULL, new RedDotStatusConfig(
                "满",
                "Icons/icon_full"
            ));
            statusConfig.Add(RedDotNodeContextStatus.NEW, new RedDotStatusConfig(
                "新",
                "Icons/icon_new"
            ));
            statusConfig.Add(RedDotNodeContextStatus.NUMBER, new RedDotStatusConfig(
                "数字",
                "Icons/icon_number"
            ));
        }

        public static RedDotStatusConfig GetStatusConfig(RedDotNodeContextStatus status)
        {
            InitConfig();
            return statusConfig[status];
        }
    }
}