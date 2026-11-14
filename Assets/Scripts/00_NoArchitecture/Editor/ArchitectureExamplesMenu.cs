using UnityEditor;
using UnityEngine;

namespace ArchitectureExamples.NoArchitecture.Editor
{
    /// <summary>可选编辑器便捷入口；只创建带 Launcher 的物体，不自动保存或覆盖场景。</summary>
    public static class ArchitectureExamplesMenu
    {
        [MenuItem("Tools/Architecture Examples/Create Launcher", false, 100)]
        private static void CreateLauncher()
        {
            var existing = Object.FindAnyObjectByType<ArchitectureExamplesLauncher>();
            if (existing != null) { Selection.activeGameObject = existing.gameObject; return; }
            var go = new GameObject("Architecture Examples");
            Undo.RegisterCreatedObjectUndo(go, "Create Architecture Examples");
            Undo.AddComponent<ArchitectureExamplesLauncher>(go);
            Selection.activeGameObject = go;
        }
        [MenuItem("Tools/Architecture Examples/Create Launcher", true)]
        private static bool CanCreateLauncher() { return !EditorApplication.isPlayingOrWillChangePlaymode; }
    }
}
