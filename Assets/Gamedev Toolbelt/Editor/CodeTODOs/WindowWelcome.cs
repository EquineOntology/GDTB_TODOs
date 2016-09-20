using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.codetodos
{
    public class WindowWelcome : EditorWindow
    {
        public static WindowWelcome Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }
        private GUISkin _skin;
        private int _offset = 5;


        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowWelcome)EditorWindow.GetWindow(typeof(WindowWelcome));
            window.SetMinSize();
            window.LoadSkin();
            window.Show();
        }


        public void OnEnable()
        {
            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                titleContent = new GUIContent("Hello!");
            #else
                title = "Hello!";
            #endif

            Instance = this;

            Preferences.GetAllPrefValues();

            LoadSkin();
        }


        /// Called when the window is closed.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }


        private void OnGUI()
        {
            GUI.skin = _skin; // Without this, almost everything will work aside from the scrollbar.

            DrawWindowBackground();

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Hello!\n\nUsing CodeTODOs is easy. The first time you run it, press the 'Process scripts' button.\n\nDepending on your settings, it will look like one of these two:", GUILayout.ExpandWidth(true));
            DrawProcessButtons();
            GUILayout.Space(50);
            GUILayout.Label("That will analyse your project and locate all scripts.\n\nAfter that, any time you want to update your list of tasks you can click on the 'Refresh' button, which will look through the scripts and pick up on pending tasks.", GUILayout.ExpandWidth(true));
            GUILayout.Space(50);
            DrawRefreshButtons();
            GUILayout.Label("To define a task, you just need to start a comment with the appropriate token (your currently set token is '" + Preferences.TODOToken + "'.\n\nRemeber to take a look at the Preferences, a section has been added for CodeTODOs!", GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();

            

        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        private void DrawProcessButtons()
        {
            // Text.
            var rect_text = new Rect(60, 105, 100, 20);
            var style = new GUIStyle();
            style.active.textColor = style.onActive.textColor = style.normal.textColor = style.onNormal.textColor = Preferences.Color_Tertiary;
			style.imagePosition = ImagePosition.TextOnly;
			style.alignment = TextAnchor.MiddleCenter;
			// Label inside the button.
			EditorGUI.LabelField(rect_text, "Process", style);

            // Icon
            GUI.DrawTexture(new Rect(160, 109, Constants.BUTTON_TEXTURE_SIZE, Constants.BUTTON_TEXTURE_SIZE), DrawingUtils.Texture_Process);
        }


        private void DrawRefreshButtons()
        {
            // Text.
            var rect_text = new Rect(60, 225, 100, 20);
            var style = new GUIStyle();
            style.active.textColor = style.onActive.textColor = style.normal.textColor = style.onNormal.textColor = Preferences.Color_Tertiary;
			style.imagePosition = ImagePosition.TextOnly;
			style.alignment = TextAnchor.MiddleCenter;
			// Label inside the button.
			EditorGUI.LabelField(rect_text, "Refresh", style);

            // Icon
            GUI.DrawTexture(new Rect(160, 229, Constants.BUTTON_TEXTURE_SIZE, Constants.BUTTON_TEXTURE_SIZE), DrawingUtils.Texture_Refresh);
        }


        /// Load CodeTODOs custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowWelcome)) as WindowWelcome;
            window.minSize = new Vector2(450f, 380f);
        }
    }
}
