using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GDTB.CodeTODOs
{
    public class WindowMain : EditorWindow
    {
        public static List<QQQ> QQQs = new List<QQQ>();
        private GUISkin _gdtbSkin;
        private GUIStyle _priorityStyle, _taskStyle, _scriptStyle;

        // ========================= Editor layouting =========================
        private const int IconSize = 16;

        private int _unit, _priorityWidth, _qqqWidth, _editAndDoneWidth;
        private int _offset = 5;

        private int _priorityLabelWidth;

        private float _heightIndex = 0;
        private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
        private Rect _scrollRect, _scrollViewRect, _qqqRect, _priorityRect, _rightButtonsRect;


        // ====================================================================
        [MenuItem("Window/Gamedev Toolbelt/CodeTODOs %q")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.titleContent = new GUIContent(Constants.TEXT_WINDOW_TITLE);
            window.minSize = new Vector2(250f, 100f);

            PreferencesManager.RefreshAllPrefs();

            window.UpdateLayoutingSizes();
            window._priorityLabelWidth = (int)window._priorityStyle.CalcSize(new GUIContent("URGENT")).x; // Not with the other layouting sizes because it only needs to be done once.

            if (QQQs.Count == 0 && PreferencesManager.AutoRefresh)
            {
                Helper.GetQQQsFromAllScripts();
                Helper.ReorderQQQs();
            }
            else if (PreferencesManager.AutoRefresh == false)
            {
                QQQs.Clear();
                QQQs.AddRange(IO.LoadStoredQQQs());
            }
            window.Show();
        }


        public void OnEnable()
        {

            PreferencesManager.RefreshAllPrefs();
            LoadSkin();
            LoadStyles();
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            GUI.skin = _gdtbSkin;

            DrawQQQs();

            DrawAddButton();
            DrawRefreshButton();
            DrawSettingsButton();
        }


        /// Draw the list of QQQs.
        private void DrawQQQs()
        {
            _scrollViewRect.height = _heightIndex;
            _scrollRect.width += IconSize + 2;
            _scrollPosition = GUI.BeginScrollView(_scrollRect, _scrollPosition, _scrollViewRect);
            _heightIndex = _offset;
            for (var i = 0; i < QQQs.Count; i++)
            {
                var taskContent = new GUIContent(QQQs[i].Task);
                var taskHeight = _taskStyle.CalcHeight(taskContent, _qqqWidth);

                var helpBoxHeight = taskHeight + Constants.LINE_HEIGHT + 5;
                helpBoxHeight = helpBoxHeight < IconSize * 2.5f ? IconSize * 2.5f : helpBoxHeight;

                _qqqRect = new Rect(_priorityWidth, _heightIndex, _qqqWidth, helpBoxHeight);
                _priorityRect = new Rect(0, _qqqRect.y, _priorityWidth, helpBoxHeight);
                _rightButtonsRect = new Rect(_priorityWidth + _qqqWidth + (_offset * 2), _qqqRect.y, _editAndDoneWidth, helpBoxHeight);

                var helpBoxRect = _priorityRect;
                helpBoxRect.height = helpBoxHeight;
                helpBoxRect.width = position.width - (_offset * 2) - IconSize;
                helpBoxRect.x += _offset;

                _heightIndex += (int)helpBoxHeight + _offset;
                _scrollViewRect.height = _heightIndex;

                DrawHelpBox(helpBoxRect);
                DrawPriority(_priorityRect, QQQs[i], helpBoxHeight);
                DrawTaskAndScriptLabels(_qqqRect, QQQs[i], taskHeight);
                DrawEditAndCompleteButtons(_rightButtonsRect, QQQs[i]);
            }
            GUI.EndScrollView();
        }


        #region QQQPriorityMethods

        /// Select which priority format to use based on the user preference.
        private void DrawPriority(Rect aRect, QQQ aQQQ, float helpBoxHeight = 30)
        {
            switch (PreferencesManager.QQQPriorityDisplay)
            {
                case PriorityDisplayFormat.TEXT_ONLY:
                    DrawPriorityText(aRect, aQQQ);
                    break;
                case PriorityDisplayFormat.ICON_ONLY:
                    DrawPriorityIcon(aRect, aQQQ);
                    break;
                case PriorityDisplayFormat.ICON_AND_TEXT:
                    DrawPriorityIconAndText(aRect, aQQQ);
                    break;
                case PriorityDisplayFormat.BARS:
                default:
                    DrawPriorityBars(aRect, aQQQ, helpBoxHeight);
                    break;
            }
        }

        /// Draw priority for the "Bars" setting.
        private void DrawPriorityBars(Rect aRect, QQQ aQQQ, float helpBoxHeight)
        {
            var borderWidth = 1;
            var priorityRect = aRect;
            var newY = 0;
            var newX = 0;

            newX = (int)priorityRect.x + (_offset * 2);
            newY = (int)priorityRect.y + _offset;

            priorityRect.width = IconSize;
            priorityRect.height = helpBoxHeight - (_offset * 2);
            priorityRect.position = new Vector2(newX, newY);

            var color = GetQQQPriorityColor((int)aQQQ.Priority);
            EditorGUI.DrawRect(priorityRect, color);

            // Draw bodrers
            var topBorder = new Rect (priorityRect.x, priorityRect.y, priorityRect.width, borderWidth);
            var bottomBorder = new Rect (priorityRect.x, (priorityRect.y + priorityRect.height - borderWidth), priorityRect.width, borderWidth);
            var leftBorder = new Rect (priorityRect.x, priorityRect.y, borderWidth, priorityRect.height);
            var rightBorder = new Rect ((priorityRect.x + priorityRect.width - borderWidth), priorityRect.y, borderWidth, priorityRect.height);

            EditorGUI.DrawRect(topBorder, Color.gray);
            EditorGUI.DrawRect(bottomBorder, Color.gray);
            EditorGUI.DrawRect(leftBorder, Color.gray);
            EditorGUI.DrawRect(rightBorder, Color.gray);
        }

        /// Draw priority for the "Icon only" setting.
        private void DrawPriorityIcon(Rect aRect, QQQ aQQQ)
        {
            // Prepare the rectangle for layouting. The layout is "space-icon-space".
            var priorityRect = aRect;
            var newY = 0;
            var newX = 0;
            if (aRect.width > IconSize + (_offset * 2))
            {
                newX = (int)priorityRect.x + IconSize / 2 + _offset;
                newY = (int)priorityRect.y + IconSize / 2 + _offset;
            }
            else
            {
                newX = (int)priorityRect.x + IconSize / 2;
                newY = (int)priorityRect.y + IconSize / 2;
            }

            priorityRect.width = IconSize;
            priorityRect.height = IconSize;
            priorityRect.position = new Vector2(newX, newY);

            Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
            EditorGUI.DrawPreviewTexture(priorityRect, tex);
        }


        /// Draw priority for the "Text only" setting.
        private void DrawPriorityText(Rect aRect, QQQ aQQQ)
        {
            var priorityRect = aRect;
            priorityRect.width -= _offset;
            priorityRect.height -= (IconSize / 2 + _offset);

            var newX = (int)priorityRect.x + _offset;
            var newY = (int)priorityRect.y + _offset;
            priorityRect.position = new Vector2(newX, newY);

            EditorGUI.LabelField(priorityRect, aQQQ.Priority.ToString());
        }


        /// Draw priority for the "Icon and Text" setting.
        private void DrawPriorityIconAndText(Rect aRect, QQQ aQQQ)
        {
            // Draw the Icon.
            var iconRect = aRect;
            iconRect.width = IconSize;
            iconRect.height = IconSize;

            var iconNewY = iconRect.y + _offset;
            var iconNewX = iconRect.x + Mathf.Clamp(_unit, 1, IconSize + (int)(_priorityLabelWidth / 2));
            iconRect.position = new Vector2(iconNewX, iconNewY);

            Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
            EditorGUI.DrawPreviewTexture(iconRect, tex);

            // Draw the label.
            var labelRect = aRect;
            labelRect.width -= _offset;

            var labelNewX = labelRect.x + _offset;
            var labelNewY = (int)(iconRect.y + iconRect.height);
            labelRect.position = new Vector2(labelNewX, labelNewY);

            EditorGUI.LabelField(labelRect, aQQQ.Priority.ToString());
        }


        /// Get the correct texture for a priority.
        private Texture2D GetQQQPriorityTexture(int aPriority)
        {
            Texture2D tex;
            switch (aPriority)
            {
                case 1:
                tex = Resources.Load<Texture2D>(Constants.FILE_QQQ_URGENT);
                    break;
                case 3:
                tex = Resources.Load<Texture2D>(Constants.FILE_QQQ_MINOR);
                    break;
                case 2:
                default:
                tex = Resources.Load<Texture2D>(Constants.FILE_QQQ_NORMAL);
                    break;
            }
            return tex;
        }


        /// Get the correct color for a priority rectangle.
        private Color GetQQQPriorityColor(int aPriority)
        {
            Color col;
            switch (aPriority)
            {
                case 1:
                    col = PreferencesManager.PriColor1;
                    break;
                case 3:
                    col = PreferencesManager.PriColor3;
                    break;
                case 2:
                default:
                    col = PreferencesManager.PriColor2;
                    break;
            }
            return col;
        }

        #endregion


        /// Draws the "Task" and "Script" texts for QQQs.
        private void DrawTaskAndScriptLabels(Rect aRect, QQQ aQQQ, float aHeight)
        {
            // Task.
            var taskRect = aRect;
            taskRect.x = _priorityWidth;
            taskRect.y += _offset;
            taskRect.height = aHeight;
            EditorGUI.LabelField(taskRect, aQQQ.Task, _taskStyle);

            // Script.
            var scriptRect = aRect;
            scriptRect.x = _priorityWidth;
            scriptRect.y += (taskRect.height + 5);
            scriptRect.height = Constants.LINE_HEIGHT;

            //var scriptLabel = aQQQ.Script;
            var scriptLabel = Helper.CreateScriptLabel(aQQQ, scriptRect.width, _scriptStyle);
            EditorGUI.LabelField(scriptRect, scriptLabel, _scriptStyle);

            // Open editor on click.
            EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp && scriptRect.Contains(Event.current.mousePosition))
            {
                Helper.OpenScript(aQQQ);
            }
        }


        /// Draw the "Help box" style rectangle that separates the QQQs visually.
        private void DrawHelpBox(Rect aRect)
        {
            EditorGUI.LabelField(aRect, "", EditorStyles.helpBox);
        }


        /// Draw the "Edit" and "Complete" buttons.
        private void DrawEditAndCompleteButtons(Rect aRect, QQQ aQQQ)
        {
            // "Edit" button.
            var editRect = aRect;
            editRect.x = position.width - (IconSize * 2) - (_offset * 2);
            editRect.y += 3;
            editRect.width = IconSize;
            editRect.height = IconSize;
            var editButton = new GUIContent(Resources.Load(Constants.FILE_QQQ_EDIT, typeof(Texture2D)) as Texture2D, "Edit this task");

            // Open edit window on click.
            if (GUI.Button(editRect, editButton))
            {
                WindowEdit.Init(aQQQ);
            }

            // "Complete" button.
            var completeRect = editRect;
            completeRect.y = editRect.y + editRect.height + 2;
            var completeButton = new GUIContent(Resources.Load(Constants.FILE_QQQ_DONE, typeof(Texture2D)) as Texture2D, "Complete this task");

            // Complete QQQ on click.
            if (GUI.Button(completeRect, completeButton))
            {
                // Confirmation dialog.
                if (EditorUtility.DisplayDialog("Mark task as complete", "Are you sure you want to mark this task as done?\nThis will IRREVERSIBLY remove the comment from the script!", "Complete task", "Cancel"))
                {
                    Helper.CompleteQQQ(aQQQ);
                }
            }
        }


        /// Draw the "Refresh" button.
        private void DrawRefreshButton()
        {
            // "Refresh" button.
            var refreshRect = new Rect((position.width / 2) - (IconSize * 0.5f), position.height - (IconSize * 1.5f), IconSize, IconSize);
            var refreshButton = new GUIContent(Resources.Load(Constants.FILE_QQQ_REFRESH, typeof(Texture2D)) as Texture2D, "Refresh list of tasks");

            // Refresh on click.
            if (GUI.Button(refreshRect, refreshButton))
            {
                QQQs.Clear();
                Helper.GetQQQsFromAllScripts();
                Helper.ReorderQQQs();
            }
        }


        /// Draw the "Add" button.
        private void DrawAddButton()
        {
            // "Add" button.
            var addRect = new Rect((position.width / 2) - (IconSize * 1.5f) - _offset, position.height - (IconSize * 1.5f), IconSize, IconSize);
            var addButton = new GUIContent(Resources.Load(Constants.FILE_QQQ_ADD, typeof(Texture2D)) as Texture2D, "Add a new task");

            // Add QQQ on click.
            if (GUI.Button(addRect, addButton))
            {
                WindowAdd.Init();
            }
        }


        /// Draw the "Settings" button.
        private void DrawSettingsButton()
        {
            // "Settings" button.
            var settingsRect = new Rect((position.width / 2) + (IconSize * 0.5f) + _offset, position.height - (IconSize * 1.5f), IconSize, IconSize);
            var settingsButton = new GUIContent(Resources.Load(Constants.FILE_SETTINGS, typeof(Texture2D)) as Texture2D, "Open settings window");

            // Open settings on click.
            if (GUI.Button(settingsRect, settingsButton))
            {
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var asm = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var T = asm.GetType("UnityEditor.PreferencesWindow");
                var M = T.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                M.Invoke(null, null);
            }
        }


        /// Update sizes used in layouting based on the window size.
        private void UpdateLayoutingSizes()
        {
            var width = position.width - IconSize;

            _scrollRect = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize * 2.5f);

            _scrollViewRect = _scrollRect;

            _unit = (int)(width / 28) == 0 ? 1 : (int)(width / 28); // If the unit would be 0, set it to 1.

            // Priority rect width has different size based on preferences.
            if (PreferencesManager.QQQPriorityDisplay.ToString() == "ICON_ONLY")
            {
                _priorityWidth = Mathf.Clamp((_unit * 2) + IconSize, IconSize, (IconSize + _offset) * 2);
            }
            else if (PreferencesManager.QQQPriorityDisplay.ToString() == "BARS")
            {
                _priorityWidth = IconSize * 2;
            }
            else
            {
                _priorityWidth = Mathf.Clamp((_unit * 2) + IconSize, IconSize, (IconSize * 2) + _priorityLabelWidth);
            }
            _editAndDoneWidth = (IconSize * 2) + 5;
            _qqqWidth = (int)width - _priorityWidth - _editAndDoneWidth - (_offset * 2);
        }


        /// Load the CodeTODOs skin.
        private void LoadSkin()
        {
            _gdtbSkin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Assign the GUI Styles
        private void LoadStyles()
        {
            _priorityStyle = _gdtbSkin.GetStyle("label");
            _taskStyle = _gdtbSkin.GetStyle("task");
            _scriptStyle = _gdtbSkin.GetStyle("script");
        }


        /// Called when the window is closed.
        private void OnDestroy()
        {
            if (PreferencesManager.AutoRefresh == false)
            {
                IO.WriteQQQsToFile();
            }
        }
    }
}