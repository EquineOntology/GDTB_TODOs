using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.todos
{
    public class WindowAdd : EditorWindow
    {

#region FIELDS AND PROPERTIES

        // Constants.
        private const int _BUTTON_WIDTH = 70;
        private const int _BUTTON_HEIGHT = 18;
        private const int _FIELDS_WIDTH = 120;

        // Fields.
        private GUISkin _skin;
        private GUIStyle _boldStyle, _buttonTextStyle;
        private readonly string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };
        private string _task;
        private MonoScript _script;
        private int _priority = 2;
        private int _lineNumber;

        // Properties.
        public static WindowAdd Instance { get; private set; }
        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        #endregion

#region MONOBEHAVIOUR METHODS

        public void OnEnable()
        {
        #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
            titleContent = new GUIContent("Add task");
        #else
            title = "Add task";
        #endif

            Instance = this;
            LoadSkin();
            LoadStyles();
            _script = new MonoScript();
        }


        public void OnGUI()
        {
            DrawWindowBackground();
            DrawScriptPicker();
            DrawTaskField();
            DrawPriorityPopup();
            DrawLineNumberField();
            DrawAdd();
        }


        public void Update()
        {
            // We repaint every frame for the same reason we do so in WindowMain.
            Repaint();
        }

#endregion

#region METHODS

        public static void Init()
        {
            var window = (WindowAdd)GetWindow(typeof(WindowAdd));
            window.minSize = new Vector2(208f, 230f);
            window.ShowUtility();
        }


        /// Load TODOs custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load custom styles and apply colors from preferences.
        public void LoadStyles()
        {
            _boldStyle = _skin.GetStyle("GDTB_TODOs_task");
            _boldStyle.active.textColor = Preferences.Secondary;
            _boldStyle.normal.textColor = Preferences.Secondary;
            _buttonTextStyle = _skin.GetStyle("GDTB_TODOs_buttonText");
            _buttonTextStyle.active.textColor = Preferences.Tertiary;
            _buttonTextStyle.normal.textColor = Preferences.Tertiary;

            _skin.settings.selectionColor = Preferences.Secondary;
        }




        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Primary);
        }


        /// Draw script picker.
        private void DrawScriptPicker()
        {
            var labelRect = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Pick a script:",  _boldStyle);

            var pickerRect = new Rect(10, 28, position.width - 20, 16);
            _script = (MonoScript)EditorGUI.ObjectField(pickerRect, _script, typeof(MonoScript), false);
        }


        /// Draw Task input field.
        private void DrawTaskField()
        {
            var labelRect = new Rect(10, 53, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Write a task:", _boldStyle);

            var taskRect = new Rect(10, 71, position.width - 20, 32);
            _task = EditorGUI.TextField(taskRect, _task);
        }


        /// Draw priority popup.
        private void DrawPriorityPopup()
        {
            var labelRect = new Rect(10, 112, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Choose a priority:", _boldStyle);

            var priorityRect = new Rect(10, 130, _FIELDS_WIDTH, 16);
            _priority = EditorGUI.Popup(priorityRect, _priority - 1, _qqqPriorities) + 1;
        }


        /// Draw line number field.
        private void DrawLineNumberField()
        {
            var labelRect = new Rect(10, 155, position.width - 20, 32);
            EditorGUI.LabelField(labelRect, "Choose the line number:", _boldStyle);

            var lineRect = new Rect(10, 176, _FIELDS_WIDTH, 16);
            _lineNumber = EditorGUI.IntField(lineRect, _lineNumber);

            if (_lineNumber < 1)
            {
                _lineNumber = 1;
            }
        }


        /// Draw Add based of preferences.
        private void DrawAdd()
        {
            Rect buttonRect;
            GUIContent buttonContent;

            SetupButton_Add(out buttonRect, out buttonContent);

            if (Controls.Button(buttonRect, buttonContent))
            {
                ButtonPressed();
            }
        }


        /// Setup the Add button.
        private void SetupButton_Add(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - _BUTTON_WIDTH / 2, position.height - _BUTTON_HEIGHT * 1.5f, _BUTTON_WIDTH, _BUTTON_HEIGHT);
            aContent = new GUIContent("Add task", "Add task");
        }


        /// What to do when the add button is pressed.
        private void ButtonPressed()
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
                var execute = false;
                // Get confirmation (through confirmation dialog or automatically if conf. is off).
                if (Preferences.ShowConfirmationDialogs)
                {
                    if (EditorUtility.DisplayDialog("Add task?", "Are you sure you want to add this task to the specified script?", "Add task", "Cancel"))
                    {
                        execute = true;
                    }
                }
                else
                {
                    execute = true;
                }

                // Do the thing.
                if (execute)
                {
                    var path = AssetDatabase.GetAssetPath(_script);
                    var newQQQ = QQQ.Create(_priority, _task, path, _lineNumber);
                    QQQOps.AddQQQ(newQQQ);
                    GetWindow(typeof(WindowAdd)).Close();
                }
            }
        }

#endregion

    }
}