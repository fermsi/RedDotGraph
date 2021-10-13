namespace RedDot.Editor.Util
{
    public static class PathUtil
    {
        public static string CurEditorPath;

        public static string GetEditorFilePath(string fileName)
        {
            return $"{CurEditorPath}{fileName}";
        }
    }
}