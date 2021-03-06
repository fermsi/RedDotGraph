using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedDot.Editor.Util
{
    static class FileUtilities
    {
        public static string WriteRedDotGraphDataToDisk(string path, object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var text = JsonUtility.ToJson(data, true);
            if (WriteToDisk(path, text))
                return text;
            else
                return null;
        }

        // returns true if successfully written to disk
        public static bool WriteToDisk(string path, string text)
        {
            while (true)
            {
                try
                {
                    File.WriteAllText(path, text);
                }
                catch (Exception e)
                {
                    if (e.GetBaseException() is UnauthorizedAccessException &&
                        (File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        if (EditorUtility.DisplayDialog("File is Read-Only", path, "Make Writeable", "Cancel Save"))
                        {
                            // make writeable
                            FileInfo fileInfo = new FileInfo(path);
                            fileInfo.IsReadOnly = false;
                            continue; // retry save
                        }
                        else
                            return false;
                    }

                    Debug.LogException(e);

                    if (EditorUtility.DisplayDialog("Exception While Saving", e.ToString(), "Retry", "Cancel"))
                        continue; // retry save
                    else
                        return false;
                }
                break; // no exception, file save success!
            }

            return true;
        }
    }
}
