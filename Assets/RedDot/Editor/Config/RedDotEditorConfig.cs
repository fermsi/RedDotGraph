using System.Collections.Generic;

namespace RedDot.Editor.Config
{
    public enum RedDotNodeContextStatus
    {
        Normal = 1,
        Number = 2,
        New = 4,
        Full = 8,
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
        public const string EXTENSION = "reddotgraph";
        public const string EXPORT_EXTENSION = "json";
        public const string KEY_WINDOW_TITLE = "Key设置";
        public const string EXTERNAL_ID_WINDOW_TITLE = "逻辑ID设置";

        private static Dictionary<RedDotNodeContextStatus, RedDotStatusConfig> s_statusConfig;

        private static void InitConfig()
        {
            if (s_statusConfig != null) return;
            s_statusConfig = new Dictionary<RedDotNodeContextStatus, RedDotStatusConfig>();
            s_statusConfig.Add(RedDotNodeContextStatus.Normal, new RedDotStatusConfig(
                "红点",
                "Icons/icon_normal"
                ));
            s_statusConfig.Add(RedDotNodeContextStatus.Full, new RedDotStatusConfig(
                "满",
                "Icons/icon_full"
            ));
            s_statusConfig.Add(RedDotNodeContextStatus.New, new RedDotStatusConfig(
                "新",
                "Icons/icon_new"
            ));
            s_statusConfig.Add(RedDotNodeContextStatus.Number, new RedDotStatusConfig(
                "数字",
                "Icons/icon_number"
            ));
        }

        public static RedDotStatusConfig GetStatusConfig(RedDotNodeContextStatus status)
        {
            InitConfig();
            return s_statusConfig[status];
        }
    }
}