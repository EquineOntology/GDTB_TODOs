
using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.codetodos
{
    public class WindowAdd : EditorWindow
    {
        public static WindowAdd Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        private GUISkin _skin;
        private GUIStyle _style_bold, _style_buttonText;

        private string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };

        private string _task;
        private MonoScript _script;
        private int _priority = 2;
        private int _lineNumber = 0;


        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;
        private const int FieldsWidth = 120;


        public static void Init()
        {
            WindowAdd window = (WindowAdd)EditorWindow.GetWindow(typeof(WindowAdd));
            window.minSize = new Vector2(208f, 230f);
            window.ShowUtility();
        }


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


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw script picker.
        private void DrawScriptPicker()
        {
            var labelRect = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Pick a script:",  _style_bold);

            var pickerRect = new Rect(10, 28, position.width - 20, 16);
            _script = (MonoScript)EditorGUI.ObjectField(pickerRect, _script, typeof(MonoScript), false);
        }


        /// Draw Task input field.
        private void DrawTaskField()
        {
            var labelRect = new Rect(10, 53, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Write a task:", _style_bold);

            var taskRect = new Rect(10, 71, position.width - 20, 32);
            _task = EditorGUI.TextField(taskRect, _task);
        }


        /// Draw priority popup.
        private void DrawPriorityPopup()
        {
            var labelRect = new Rect(10, 112, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Choose a priority:", _style_bold);

            var priorityRect = new Rect(10, 130, FieldsWidth, 16);
            _priority = EditorGUI.Popup(priorityRect, _priority - 1, _qqqPriorities) + 1;
        }


        /// Draw line number field.
        private void DrawLineNumberField()
        {
            var labelRect = new Rect(10, 155, position.width - 20, 32);
            EditorGUI.LabelField(labelRect, "Choose the line number:", _style_bold);

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
            Rect buttonRect;
            GUIContent buttonContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Add_default(out buttonRect, out buttonContent);
                    break;
                default:
                    Button_Add_icon(out buttonRect, out buttonContent);
                    break;
            }

            if (GUI.Button(buttonRect, buttonContent))
            {
                ButtonPressed();
            }
            DrawingUtils.DrawButton(buttonRect, Preferences.ButtonsDisplay, DrawingUtils.Texture_Add, buttonContent.text, _style_buttonText);
        }


        /// Draw default-button Add.
        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - ButtonWidth / 2, position.height - ButtonHeight * 1.5f, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add task", "Add task");
        }

        /// Draw icon Add.
        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2) - IconSize / 2, position.height - IconSize * 1.5f, IconSize, IconSize);
            aContent = new GUIContent("", "Add task");
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
                    if (Preferences.ShowConfirmationDialogs == true)
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
                    if (execute == true)
                    {
                        var path = AssetDatabase.GetAssetPath(_script);
                        var newQQQ = new QQQ(_priority, _task, path, _lineNumber);
                        QQQOps.AddQQQ(newQQQ);
                        EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                    }
                }
        }

        /// Load CodeTODOs custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load custom styles and apply colors from preferences.
        public void LoadStyles()
        {
            _style_bold = _skin.GetStyle("GDTB_CodeTODOs_task");
            _style_bold.active.textColor = Preferences.Color_Secondary;
            _style_bold.normal.textColor = Preferences.Color_Secondary;
            _style_buttonText = _skin.GetStyle("GDTB_CodeTODOs_buttonText");
            _style_buttonText.active.textColor = Preferences.Color_Tertiary;
            _style_buttonText.normal.textColor = Preferences.Color_Tertiary;

            _skin.settings.selectionColor = Preferences.Color_Secondary;
        }
    }
}