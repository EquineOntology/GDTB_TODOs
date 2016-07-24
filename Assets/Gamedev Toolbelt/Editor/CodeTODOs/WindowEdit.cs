using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.codetodos
{
    public class WindowEdit : EditorWindow
    {
        public static WindowEdit Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }

        }

        private static QQQ _oldQQQ;
        private static QQQ _newQQQ;

        private string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };
        private bool _prioritySetOnce = false;


        private GUISkin _GDTBSkin, _defaultSkin;
        private const int IconSize = 16;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;

        public static void Init(QQQ aQQQ)
        {
            // Get existing open window or if none, make a new one.
            WindowEdit window = (WindowEdit)EditorWindow.GetWindow(typeof(WindowEdit));
            window.minSize = new Vector2(208f, 140f);
            window.titleContent = new GUIContent("Edit task");
            _oldQQQ = aQQQ;
            _newQQQ = new QQQ((int)aQQQ.Priority, aQQQ.Task, aQQQ.Script, aQQQ.LineNumber);
            window.Show();
        }


        public void OnEnable()
        {
            Instance = this;
            _GDTBSkin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        private void OnGUI()
        {
            if (_defaultSkin == null)
            {
                _defaultSkin = GUI.skin;
            }
            GUI.skin = _GDTBSkin;
            DrawPriority();
            DrawTask();
            DrawEdit();
        }


        /// Draw priority.
        private void DrawPriority()
        {
            var labelRect = new Rect(10, 10, position.width, 16);
            EditorGUI.LabelField(labelRect, "Choose a priority:", EditorStyles.boldLabel);

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
            EditorGUI.LabelField(labelRect, "Task:", EditorStyles.boldLabel);

            // The task itself.
            var fieldRect = new Rect(10, 71, position.width - 20, 32);
            _newQQQ.Task = EditorGUI.TextField(fieldRect, _newQQQ.Task);
        }


        /// Draw Edit based of preferences.
        private void DrawEdit()
        {
            Rect buttonRect;
            GUIContent buttonContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    DrawEdit_Default(out buttonRect, out buttonContent);
                    break;
                default:
                    DrawEdit_Icon(out buttonRect, out buttonContent);
                    break;
            }

            if (GUI.Button(buttonRect, buttonContent))
            {
                // Get confirmation (through confirmation dialog or automatically if conf. is off).
                var execute = false;

                if (Preferences.ShowConfirmationDialogs == true)
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
                if (execute == true)
                {
                    QQQOps.UpdateTask(_oldQQQ, _newQQQ);
                    if (WindowMain.IsOpen)
                    {
                        EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
                    }
                    EditorWindow.GetWindow(typeof(WindowEdit)).Close();
                }
            }
        }


        /// Draw default-style Edit.
        private void DrawEdit_Default(out Rect aRect, out GUIContent aContent)
        {
            GUI.skin = _defaultSkin;

            aRect = new Rect((position.width / 2) - ButtonWidth/2, position.height - ButtonHeight * 1.5f, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Save", "Save edits");
        }


        /// Draw icon Edit.
        private void DrawEdit_Icon(out Rect aRect, out GUIContent aContent)
        {
            GUI.skin = _GDTBSkin;
            aRect = new Rect((position.width / 2) - IconSize/2, position.height - IconSize * 1.5f, IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_QQQ_EDIT, typeof(Texture2D)) as Texture2D, "Save edits");
        }
    }
}