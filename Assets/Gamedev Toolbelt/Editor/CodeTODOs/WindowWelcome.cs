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
        private GUIStyle _wordWrappedColoredLabel, _headerLabel;
        private int _offset = 5;
        private bool _welcomeValue;
        private float _usableWidth = 0;


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

            LoadSkin();
            LoadStyle();

            _welcomeValue = Preferences.ShowWelcome;
        }


        private void Update()
        {
            Repaint();
        }


        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }


        private void OnGUI()
        {
            _usableWidth = position.width - _offset * 2;
            GUI.skin = _skin;

            DrawWindowBackground();
            var label1Content = new GUIContent("Hello! Using CodeTODOs is easy.");
            var label1Height = _wordWrappedColoredLabel.CalcHeight(label1Content, _usableWidth);
            var label1Rect = new Rect(_offset * 2, _offset * 2, _usableWidth - _offset * 2, label1Height);
            EditorGUI.LabelField(label1Rect, label1Content, _wordWrappedColoredLabel);

            var header1Content = new GUIContent("1. Press the 'Process scripts' button to analyze your project and find the script files.");
            var header1Height = _headerLabel.CalcHeight(header1Content, _usableWidth);
            var header1Rect = new Rect(_offset * 2, _offset * 2 + 30, _usableWidth - _offset * 2, header1Height);
            EditorGUI.LabelField(header1Rect, header1Content, _headerLabel);

            DrawProcessButtons();

            var header2Content = new GUIContent("2. Click on the 'Refresh tasks' button, which will look through the scripts and find pending tasks.");
            var header2Height = _headerLabel.CalcHeight(header2Content, _usableWidth);
            var header2Rect = new Rect(_offset * 2, _offset * 2 + 80, _usableWidth - _offset * 2, header2Height);
            header2Rect.y += 25;
            EditorGUI.LabelField(header2Rect, header2Content, _headerLabel);

            DrawRefreshButtons();

            var header3Content = new GUIContent("3. Define a task by starting a comment with your token.");
            var header3Rect = new Rect(_offset * 2, _offset * 2 + 200, _usableWidth - _offset * 2, 0);;
            header3Rect.height = _headerLabel.CalcHeight(header3Content, _usableWidth);
            EditorGUI.LabelField(header3Rect, header3Content, _headerLabel);

            var label2Content =  new GUIContent("Your currently set token is '" + Preferences.TODOToken + "'.\nYou can change that and much more in the Preferences, where a section has been added for CodeTODOs.\n\nDon't forget to check the README for advanced features and info!");
            var label2Rect = new Rect(_offset * 2, _offset + 2 + 220, _usableWidth - _offset * 2, 0);
            label2Rect.height = _wordWrappedColoredLabel.CalcHeight(label2Content, _usableWidth);
            EditorGUI.LabelField(label2Rect, label2Content, _wordWrappedColoredLabel);

            DrawToggle();
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        private void DrawProcessButtons()
        {
            var processRect = new Rect(60, 80, 80, 20);
            var processContent = new GUIContent("Process", "Process all scripts");

            Controls.Button(processRect, processContent);
        }



        private void DrawRefreshButtons()
        {
            var refreshRect = new Rect(60, 160, 80, 20);
            var refreshContent = new GUIContent("Refresh", "Refresh task list");

            Controls.Button(refreshRect, refreshContent);
        }


        private void DrawToggle()
        {
            var rect = new Rect(_offset * 2, position.height - 20 - _offset, position.width, 20);
            _welcomeValue = EditorGUI.ToggleLeft(rect, " Show this window every time CodeTODOs is opened", _welcomeValue, _wordWrappedColoredLabel);
            if (_welcomeValue != Preferences.ShowWelcome)
            {
                Preferences.SetWelcome(_welcomeValue);
            }
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

            _headerLabel = _skin.GetStyle("GDTB_CodeTODOs_task");
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowWelcome)) as WindowWelcome;
            window.minSize = new Vector2(450f, 350f);
        }
    }
}
