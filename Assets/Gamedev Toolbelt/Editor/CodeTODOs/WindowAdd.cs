
using UnityEngine;
using UnityEditor;

namespace GDTB.CodeTODOs
{
    public class WindowAdd : EditorWindow
    {
        public static WindowAdd Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        private GUISkin _GDTBSkin;

        private string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };

        private string _task;
        private MonoScript _script;
        private int _priority = 2;
        private int _lineNumber = 0;

        private GUISkin _defaultSkin;

        private const int IconSize = 16;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;
        private const int FieldsWidth = 70;


        public static void Init()
        {
            WindowAdd window = (WindowAdd)EditorWindow.GetWindow(typeof(WindowAdd));
            window.titleContent = new GUIContent("Add task");
            window.minSize = new Vector2(208f, 230f);
            window.ShowUtility();
        }


        public void OnEnable()
        {
            Instance = this;
            _GDTBSkin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            _script = new MonoScript();
        }


        public void OnGUI()
        {
            if (_defaultSkin == null)
            {
                _defaultSkin = GUI.skin;
            }
            DrawScriptPicker();
            DrawTaskField();
            DrawPriorityPopup();
            DrawLineNumberField();
            DrawAdd();
        }


        /// Draw script picker.
        private void DrawScriptPicker()
        {
            var labelRect = new Rect(10, 10, position.width, 16);
            EditorGUI.LabelField(labelRect, "Pick a script:", EditorStyles.boldLabel);

            var pickerRect = new Rect(10, 28, FieldsWidth, 16);
            _script = (MonoScript)EditorGUI.ObjectField(pickerRect, _script, typeof(MonoScript), false);
        }


        /// Draw Task input field.
        private void DrawTaskField()
        {
            var labelRect = new Rect(10, 53, position.width, 16);
            EditorGUI.LabelField(labelRect, "Write a task:", EditorStyles.boldLabel);

            var taskRect = new Rect(10, 71, position.width - 20, 32);
            _task = EditorGUI.TextField(taskRect, _task);
        }


        /// Draw priority popup.
        private void DrawPriorityPopup()
        {
            var labelRect = new Rect(10, 112, position.width, 16);
            EditorGUI.LabelField(labelRect, "Choose a priority:", EditorStyles.boldLabel);

            var priorityRect = new Rect(10, 130, FieldsWidth, 16);
            _priority = EditorGUI.Popup(priorityRect, _priority - 1, _qqqPriorities) + 1;
        }


        /// Draw line number field.
        private void DrawLineNumberField()
        {
            var labelRect = new Rect(10, 155, position.width, 32);
            EditorGUI.LabelField(labelRect, "Choose the line number:", EditorStyles.boldLabel);

            var lineRect = new Rect(10, 176, FieldsWidth, 16);
            _lineNumber = EditorGUI.IntField(lineRect, _lineNumber);

            if (_lineNumber < 1)
            {
                _lineNumber = 1;
            }
        }


        /// Draw Add based of preferences.
        private void DrawAdd()
        {
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    DrawAdd_Default();
                    break;
                default:
                    DrawAdd_Icon();
                    break;
            }
        }


        /// Draw icon Add.
        private void DrawAdd_Icon()
        {
            GUI.skin = _GDTBSkin;

            var buttonRect = new Rect((position.width / 2) - IconSize / 2, position.height - IconSize * 1.5f, IconSize, IconSize);
            var buttonContent = new GUIContent(Resources.Load(Constants.FILE_QQQ_ADD, typeof(Texture2D)) as Texture2D, "Add task");
            if (GUI.Button(buttonRect, buttonContent))
            {
                if (_script.name == "")
                {
                    EditorUtility.DisplayDialog("No script selected", "Please select a script.", "Ok");
                }
                else if (_task == "")
                {
                    EditorUtility.DisplayDialog("No task to add", "Please create a task.", "Ok");
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Add task?", "Are you sure you want to add this task to the specified script?", "Add task", "Cancel"))
                    {
                        var path = AssetDatabase.GetAssetPath(_script);
                        var newQQQ = new QQQ(_priority, _task, path, _lineNumber);
                        QQQOps.AddQQQ(newQQQ);
                        EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                    }
                }
            }
            GUI.skin = _defaultSkin;
        }


        /// Draw default-button Add.
        private void DrawAdd_Default()
        {
            GUI.skin = _defaultSkin;

            var buttonRect = new Rect((position.width / 2) - ButtonWidth / 2, position.height - ButtonHeight * 1.5f, ButtonWidth, ButtonHeight);

            var buttonContent = new GUIContent("Add task", "Add task");
            if (GUI.Button(buttonRect, buttonContent))
            {
                if (_script.name == "")
                {
                    EditorUtility.DisplayDialog("No script selected", "Please select a script.", "Ok");
                }
                else if (_task == "")
                {
                    EditorUtility.DisplayDialog("No task to add", "Please create a task.", "Ok");
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Add task?", "Are you sure you want to add this task to the specified script?", "Add task", "Cancel"))
                    {
                        var path = AssetDatabase.GetAssetPath(_script);
                        var newQQQ = new QQQ(_priority, _task, path, _lineNumber);
                        QQQOps.AddQQQ(newQQQ);
                        EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                    }
                }
            }
        }
    }
}