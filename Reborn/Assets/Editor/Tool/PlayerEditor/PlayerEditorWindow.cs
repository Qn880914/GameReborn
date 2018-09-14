using UnityEditor;

namespace FrameWork.Editor.Tool
{
    public class PlayerEditorWindow : EditorWindow
    {
        [MenuItem("Player/EditorPlayer")]
        private static void ShowPlayerEditorWindow()
        {
            PlayerEditorWindow window = EditorWindow.GetWindow<PlayerEditorWindow>("Player Editor");
            window.minSize = new UnityEngine.Vector2(800, 600);
        }
    }
}
