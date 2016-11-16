using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.codetodos
{
    public class WindowEdit : EditorWindow
    {

#region FIELDS AND PROPERTIES

        // Fields.
        private static QQQ _oldQQQ;
        private static QQQ _newQQQ;

        private string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };
        private bool _prioritySetOnce;

        private GUISkin _skin;
        private GUIStyle _boldStyle, _buttonTextStyle;

        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;

        // Properties.
        public static WindowEdit Instance { get; private set; }
        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        #endregion

#region MONOBEHAVIOUR METHODS

        public void OnEnable()
        {
        #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
            titleContent = new GUIContent("Edit task");
        #else
            title = "Edit task";
        #endif
            Instance = this;
            LoadSkin();
            LoadStyles();
        }


        public void Update()
        {
            // We repaint every frame for the same reason we do so in WindowMain.
            Repaint();
        }




        private void OnGUI()
        {
            GUI.skin = _skin;
            DrawWindowBackground();
            DrawPriority();
            DrawTask();
            DrawEdit();
        }

#endregion


#region METHODS
        public static void Init(QQQ aQQQ)
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowEdit) GetWindow(typeof(WindowEdit));
            window.minSize = new Vector2(208f, 140f);
            _oldQQQ = aQQQ;
            _newQQQ = new QQQ((int)aQQQ.Priority, aQQQ.Task, aQQQ.Script, aQQQ.LineNumber);
            window.Show();
        }


        /// Load CodeTODOs custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load custom styles and apply colors from preferences.
        public void LoadStyles()
        {
            _boldStyle = _skin.GetStyle("GDTB_CodeTODOs_task");
            _boldStyle.active.textColor = Preferences.Secondary;
            _boldStyle.normal.textColor = Preferences.Secondary;
            _buttonTextStyle = _skin.GetStyle("GDTB_CodeTODOs_buttonText");
            _buttonTextStyle.active.textColor = Preferences.Tertiary;
            _buttonTextStyle.normal.textColor = Preferences.Tertiary;

            _skin.settings.selectionColor = Preferences.Secondary;
        }




        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Primary);
        }


        /// Draw priority.
        private void DrawPriority()
        {
            var labelRect = new Rect(10, 10, position.width, 16);
            EditorGUI.LabelField(labelRect, "Choose a priority:", _boldStyle);

            int priorityIndex;
            if (!_prioritySetOnce)
            {
                priorityIndex = (int)_newQQQ.Priority;
            }
            else
            {
                priorityIndex = (int)_oldQQQ.Priority;
                _prioritySetOnce = true;
            }
            var popupRect = new Rect(10, 28, 70, 16);
            priorityIndex = EditorGUI.Popup(popupRect, priorityIndex - 1, _qqqPriorities) + 1;

            _newQQQ.Priority = (QQQPriority)priorityIndex;
        }


        /// Draw task.
        private void DrawTask()
        {
            // Label.
            var labelRect = new Rect(10, 53, position.width, 16);
            EditorGUI.LabelField(labelRect, "Task:", _boldStyle);

            // The task itself.
            var fieldRect = new Rect(10, 71, position.width - 20, 32);
            _newQQQ.Task = EditorGUI.TextField(fieldRect, _newQQQ.Task);
        }


        /// Draw Edit based of preferences.
        private void DrawEdit()
        {
            Rect buttonRect;
            GUIContent buttonContent;

            SetupButton_Edit(out buttonRect, out buttonContent);

            if (Controls.Button(buttonRect, buttonContent))
            {
                PressedEdit();
            }
        }


        /// Setup the Edit button.
        private void SetupButton_Edit(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2) - ButtonWidth/2, position.height - ButtonHeight * 1.5f, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Save", "Save edits");
        }


        /// Action to take when the edit button is pressed.
        private void PressedEdit()
        {
            // Get confirmation (through confirmation dialog or automatically if conf. is off).
            var execute = false;

            if (Preferences.ShowConfirmationDialogs)
            {
                if (EditorUtility.DisplayDialog("Save changes to task?", "Are you sure you want to save the changes to the task?", "Save", "Cancel"))
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
                QQQOps.UpdateTask(_oldQQQ, _newQQQ);
                if (WindowMain.IsOpen)
                {
                    GetWindow(typeof(WindowMain)).Repaint();
                }
                GetWindow(typeof(WindowEdit)).Close();
            }
        }

#endregion

    }
}