using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CampusCats.Editor {
public static class CampusProject {
    const string Scene = "Assets/CampusCats/Generated/Campus.unity";
    [MenuItem("校园猫猫/打开校园场景")]
    public static void Open() { EditorSceneManager.OpenScene(Scene); }
    [MenuItem("校园猫猫/构建 Windows 试玩版")]
    public static void Build() {
        string folder = EditorUtility.OpenFolderPanel("选择输出目录", "", "");
        if (string.IsNullOrEmpty(folder)) return;
        var result = BuildPipeline.BuildPlayer(new[] {Scene}, folder + "/CampusCats.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new System.Exception("构建失败：" + result.summary.result);
        Debug.Log("Windows 构建完成");
    }
}
}
