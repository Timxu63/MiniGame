using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

[InitializeOnLoad]
public static class DataTableToolbarButton
{
    static DataTableToolbarButton()
    {
        // 注册到左侧工具栏
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        // 在工具栏绘制一个按钮
        if (GUILayout.Button(new GUIContent("导表", "运行 DataTables/gen.bat"), GUILayout.Width(60)))
        {
            RunGenBat();
        }
    }

    static void RunGenBat()
    {
        try
        {
            UnityEngine.Debug.Log("Start Update Table ...");
            string batPath = Path.Combine(Application.dataPath, "../DataTables/gen.bat");

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c \"" + batPath + "\"";
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(batPath); // 关键
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;

            process.Start();
            process.WaitForExit();
            process.Close();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"执行批处理失败: {ex.Message}");
        }
    }
}