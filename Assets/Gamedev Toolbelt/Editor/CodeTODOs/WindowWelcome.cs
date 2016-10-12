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
        private GUIStyle _wordWrappedColoredLabel;
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
            LoadStyle();
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

            var label1Content = new GUIContent("Hello!\n\nUsing CodeTODOs is easy. The first time you run it, press the 'Process scripts' button.\n\nDepending on your settings, it will look like one of these two:");
            var label1Height = _wordWrappedColoredLabel.CalcHeight(label1Content, position.width - _offset * 2);
            var label1Rect = new Rect(_offset * 2, _offset * 2, position.width - _offset * 4, label1Height);
            EditorGUI.LabelField(label1Rect, label1Content, _wordWrappedColoredLabel);

            DrawProcessButtons();

            var label2Content = new GUIContent("That will analyse your project and locate all scripts.\n\nAfter that, any time you want to update your list of tasks you can click on the 'Refresh' button, which will look through the scripts and pick up on pending tasks.");
            var label2Height = _wordWrappedColoredLabel.CalcHeight(label2Content, position.width - _offset * 2);
            var label2Rect = new Rect(_offset * 2, _offset * 2 + 40, position.width - _offset * 4, label2Height);
            label2Rect.y += 100;
            EditorGUI.LabelField(label2Rect, label2Content, _wordWrappedColoredLabel);

            DrawRefreshButtons();

            var label3Content = new GUIContent("To define a task, you just need to start a comment with the appropriate token (your currently set token is '" + Preferences.TODOToken + ")'.\n\nRemeber to take a look at the Preferences, a section has been added for CodeTODOs, and to the README for additional features and info!");
            var label3Rect = new Rect(_offset * 2, _offset * 2 + 260, position.width - _offset * 4, label2Height);;
            label3Rect.height = _wordWrappedColoredLabel.CalcHeight(label3Content, position.width - _offset * 2);
            EditorGUI.LabelField(label3Rect, label3Content, _wordWrappedColoredLabel);

            DrawToggle();
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        private void DrawProcessButtons()
        {
            // Text.
            var rect_text = new Rect(60, 110, 100, 20);
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
            var rect_text = new Rect(60, 230, 100, 20);
            var style = new GUIStyle();
            style.active.textColor = style.onActive.textColor = style.normal.textColor = style.onNormal.textColor = Preferences.Color_Tertiary;
			style.imagePosition = ImagePosition.TextOnly;
			style.alignment = TextAnchor.MiddleCenter;
			// Label inside the button.
			EditorGUI.LabelField(rect_text, "Refresh", style);

            // Icon
            GUI.DrawTexture(new Rect(160, 229, Constants.BUTTON_TEXTURE_SIZE, Constants.BUTTON_TEXTURE_SIZE), DrawingUtils.Texture_Refresh);
        }


        private void DrawToggle()
        {
            var rect = new Rect(_offset * 2, position.height - 20 - _offset, position.width, 20);
            Preferences.ShowWelcome = EditorGUI.ToggleLeft(rect, " Show this window every time CodeTODOs is opened", Preferences.ShowWelcome, _wordWrappedColoredLabel);
            //TODO: Why is the window showing up every time codetodos is closed? Is Preferences.ShowWelcome not being set correctly or reset somehow?
        }


        /// Load CodeTODOs custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load label styles.
        public void LoadStyle()
        {
            _wordWrappedColoredLabel = _skin.GetStyle("GDTB_CodeTODOs_script");
            _wordWrappedColoredLabel.active.textColor = Preferences.Color_Tertiary;
            _wordWrappedColoredLabel.normal.textColor = Preferences.Color_Tertiary;
            _wordWrappedColoredLabel.wordWrap = true;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowWelcome)) as WindowWelcome;
            window.minSize = new Vector2(450f, 380f);
        }
    }
}
