using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.todos
{
    public class WindowMain : EditorWindow
    {
        #region FIELDS AND PROPERTIES

        // Constants.
        private const int _BUTTON_WIDTH = 70;
        private const int _BUTTON_HEIGHT = 18;

        // Fields.
        public static List<QQQ> AddedQQQs = new List<QQQ>();
        public static List<string> RemovedScripts = new List<string>();
        public static List<string> MovedFromScripts = new List<string>();
        public static List<string> MovedToScripts = new List<string>();
        public static List<string> ImportedScripts = new List<string>();

        public static bool WasHiddenByReimport;
        public static bool QQQsChanged;

        [SerializeField]
        public TODO TODO;

        private GUISkin _skin;
        private GUIStyle _taskStyle, _scriptStyle, _buttonTextStyle;

        private int _qqqWidth, _buttonsWidth;
        private float _totalQQQHeight;

        private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        private Rect _scrollAreaRect, _scrollViewRect, _qqqRect, _editAndCompleteRect, _pinRect;
        private bool _showingScrollbar;


        // Properties.
        public static WindowMain Instance { get; private set; }

        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        #endregion


        #region MONOBEHAVIOUR METHODS

        public void OnEnable()
        {
        #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
            titleContent = new GUIContent("TODOs");
        #else
            title = "TODOs";
        #endif

            Instance = this;

            /* Load current preferences (like colours, etc.).
             * We do this here so that most preferences are updated as soon as they're changed.
             */
            Preferences.GetAllPrefValues();

            LoadSkin();
            LoadStyles();

            if (TODO == null)
            {
                TODO = TODO.Create();
            }
        }


        private void OnLostFocus()
        {
            IO.PersistCompletions(TODO);
            IO.WriteQQQsToFile(TODO.QQQs);
        }


        private void OnDisable()
        {
            IO.PersistCompletions(TODO);
            IO.WriteQQQsToFile(TODO.QQQs);
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            GUI.skin = _skin; // Without this, almost everything will work aside from the scrollbar.

            DrawWindowBackground();

            // If we have added a QQQ through the "Add window", we add it to the QQQ list.
            if (AddedQQQs.Count > 0)
            {
                foreach (var qqq in AddedQQQs)
                {
                    TODO.QQQs.Add(qqq);
                }
                AddedQQQs.Clear();
                QQQsChanged = true;
            }

            // If any script was moved, we change the reference of any QQQ from it.
            if (MovedFromScripts.Count > 0 && MovedFromScripts.Count == MovedToScripts.Count)
            {
                for (var i = 0; i < MovedFromScripts.Count; i++)
                {
                    TODO.QQQs = QQQOps.ChangeScriptOfQQQ(TODO.QQQs, MovedFromScripts[i], MovedToScripts[i]);
                }
                MovedFromScripts.Clear();
                MovedToScripts.Clear();
                QQQsChanged = true;
            }

            // If any script was removed, we remove any QQQs referencing it.
            if (RemovedScripts.Count > 0)
            {
                foreach (var script in RemovedScripts)
                {
                    TODO.QQQs = QQQOps.RemoveScript(TODO.QQQs, script);
                }
                RemovedScripts.Clear();
                QQQsChanged = true;
            }

            // If we (re)imported any scripts, we first remove the existing ones, and then reimport them.
            if (ImportedScripts.Count > 0)
            {
                foreach (var script in ImportedScripts)
                {
                    for(var i = 0; i < TODO.QQQs.Count; i++)
                    {
                        TODO.QQQs.RemoveAll(x => x.Script == script);
                    }
                    TODO.QQQs.AddRange(QQQOps.GetQQQsFromScript(script));
                }
                ImportedScripts.Clear();
                QQQsChanged = true;
            }

            if (TODO.QQQs.Count == 0)
            {
                DrawNoQQQsMessage();
            }

            // If the list has changed in some way, we reorder it.
            if (QQQsChanged)
            {
                TODO.QQQs = QQQOps.ReorderQQQs(TODO.QQQs);
                QQQsChanged = false;
            }

            TODO.CurrentQQQs = TODO.QQQs;

            DrawQQQs();
            DrawSeparator();
            DrawBottomButtons();
        }


        private void Update()
        {
            // Unfortunately, IMGUI is not really responsive to events, e.g. changing the style of a button
            // (like when you press it) shows some pretty abysmal delays in the GUI, the button will light up
            // and down too late after the actual click. We force the UI to update more often instead.
            Repaint();
        }

        #endregion

        #region METHODS

        [MenuItem("Window/Gamedev Toolbelt/TODOs/Open TODO %&w", false, 1)]
        public static void Init()
        {
            // If TODOs has not been initialized, or EditorPrefs have been lost for some reason, reset them to default, and show the first start window.
            if (!EditorPrefs.HasKey("GDTB_TODOs_firsttime") ||
                EditorPrefs.GetBool("GDTB_TODOs_firsttime", false) == false)
            {
                Preferences.InitExtension();
            }

            // Get existing open window or if none, make a new one.
            var window = (WindowMain) GetWindow(typeof(WindowMain));
            window.SetMinSize();
            window.LoadSkin();
            window.LoadStyles();
            window.UpdateLayoutingSizes();

            IO.LoadScripts();

            AddedQQQs.AddRange(IO.LoadStoredQQQs());

            window.Show();

            if (Preferences.ShowWelcome)
            {
                WindowWelcome.Init();
            }
        }


        /// Load custom skin.
        public void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Load custom styles and apply colors from preferences.
        public void LoadStyles()
        {
            _scriptStyle = _skin.GetStyle("GDTB_TODOs_script");
            _scriptStyle.active.textColor = Preferences.Tertiary;
            _scriptStyle.normal.textColor = Preferences.Tertiary;
            _taskStyle = _skin.GetStyle("GDTB_TODOs_task");
            _taskStyle.active.textColor = Preferences.Secondary;
            _taskStyle.normal.textColor = Preferences.Secondary;
            _buttonTextStyle = _skin.GetStyle("GDTB_TODOs_buttonText");
            _buttonTextStyle.active.textColor = Preferences.Tertiary;
            _buttonTextStyle.normal.textColor = Preferences.Tertiary;

            _skin.settings.selectionColor = Preferences.Secondary;

            // Change scrollbar color.
            var scrollbar = Resources.Load(Constants.TEX_SCROLLBAR, typeof(Texture2D)) as Texture2D;
            if (scrollbar != null)
            {
                scrollbar.SetPixel(0, 0, Preferences.Secondary);
                scrollbar.Apply();
                _skin.verticalScrollbarThumb.normal.background = scrollbar;
                _skin.verticalScrollbarThumb.active.background = scrollbar;
            }
            _skin.verticalScrollbarThumb.fixedWidth = 6;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowMain)) as WindowMain;
            if (window != null)
            {
                window.minSize = new Vector2(322f, 150f);
            }
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Primary);
        }


        /// If there are no QQQs, tell the user.
        private void DrawNoQQQsMessage()
        {
            var label = "There are currently no tasks.\nAdd one by writing a comment with " + Preferences.TODOToken +
                        " in it.\n\nIf it's the first time you open TODOs,\npress the 'Process scripts' button.\n\nIf you just reimported some files,\npress the 'Refresh tasks' button.";
            var labelContent = new GUIContent(label);

            Vector2 labelSize;
        #if UNITY_5_3_OR_NEWER
            labelSize = EditorStyles.centeredGreyMiniLabel.CalcSize(labelContent);
        #else
                labelSize = EditorStyles.wordWrappedMiniLabel.CalcSize(labelContent);
        #endif

            var labelRect = new Rect(position.width / 2 - labelSize.x / 2,
                position.height / 2 - labelSize.y / 2 - Constants.OFFSET * 2.5f, labelSize.x, labelSize.y);
        #if UNITY_5_3_OR_NEWER
            EditorGUI.LabelField(labelRect, labelContent, EditorStyles.centeredGreyMiniLabel);
        #else
            EditorGUI.LabelField(labelRect, labelContent, EditorStyles.wordWrappedMiniLabel);
        #endif
        }


        /// Draw the list of QQQs.
        private void DrawQQQs()
        {
            _scrollViewRect.height = _totalQQQHeight - Constants.OFFSET;

            // Diminish the width of scrollview and scroll area so that the scollbar is offset from the right edge of the window.
            _scrollAreaRect.width += Constants.ICON_SIZE - Constants.OFFSET;
            _scrollViewRect.width -= Constants.OFFSET;

            // Change size of the scroll area so that it fills the window when there's no scrollbar.
            if (_showingScrollbar == false)
            {
                _scrollViewRect.width += Constants.ICON_SIZE;
            }

            _scrollPosition = GUI.BeginScrollView(_scrollAreaRect, _scrollPosition, _scrollViewRect);

            _totalQQQHeight = Constants.OFFSET; // This includes all prefs, not just a single one.

            for (var i = 0; i < TODO.CurrentQQQs.Count; i++)
            {
                var taskContent = new GUIContent(TODO.CurrentQQQs[i].Task);
                var scriptContent = new GUIContent(CreateScriptLabelText(TODO.CurrentQQQs[i]));
                var taskHeight = _taskStyle.CalcHeight(taskContent, _qqqWidth);
                var scriptHeight = _scriptStyle.CalcHeight(scriptContent, _qqqWidth);
                var pinHeight = Constants.LINE_HEIGHT;

                var qqqBackgroundHeight = taskHeight + scriptHeight + pinHeight + Constants.OFFSET * 2;
                qqqBackgroundHeight = qqqBackgroundHeight < Constants.ICON_SIZE * 2.7f ? Constants.ICON_SIZE * 2.7f : qqqBackgroundHeight;

                _qqqRect = new Rect(0, _totalQQQHeight, _qqqWidth, qqqBackgroundHeight);
                _pinRect = new Rect(Constants.OFFSET * 2, _totalQQQHeight + taskHeight + scriptHeight + Constants.OFFSET * 2, 70,
                    Constants.LINE_HEIGHT);
                _editAndCompleteRect = new Rect(_qqqWidth + (Constants.OFFSET * 2), _qqqRect.y, _buttonsWidth,
                    qqqBackgroundHeight);

                var qqqBackgroundRect = _qqqRect;
                qqqBackgroundRect.height = qqqBackgroundHeight + Constants.OFFSET / 2;

                if (_showingScrollbar) // If we're not showing the scrollbar, QQQs need to be larger too.
                {
                    qqqBackgroundRect.width = position.width - Constants.OFFSET - Constants.ICON_SIZE;
                }
                else
                {
                    qqqBackgroundRect.width = position.width - Constants.OFFSET * 2.5f;
                }

                qqqBackgroundRect.x += Constants.OFFSET;

                _totalQQQHeight += qqqBackgroundRect.height + Constants.OFFSET;

                // If the user removes a QQQ from the list in the middle of a draw call, the index in the for loop stays the same but QQQs.Count diminishes.
                // I couldn't find a way around it, so what we do is swallow the exception and wait for the next draw call.
//                try
//                {
                    DrawQQQBackground(qqqBackgroundRect, GetQQQPriorityColor((int) TODO.CurrentQQQs[i].Priority));
                    DrawTaskAndScript(_qqqRect, i, taskHeight, scriptHeight);
                    DrawPin(_pinRect, i);
                    DrawEditAndComplete(_editAndCompleteRect, i);
//                }
//                catch (System.Exception)
//                {
//                }
            }


            // Are we showing the scrollbar?
            _showingScrollbar = _scrollAreaRect.height < _scrollViewRect.height;

            GUI.EndScrollView();
        }


        /// Draw the background that separates the QQQs visually.
        private void DrawQQQBackground(Rect aRect, Color aColor)
        {
            var borderThickness = Preferences.BorderThickness;
            EditorGUI.DrawRect(aRect, aColor);
            EditorGUI.DrawRect(new Rect(
                    aRect.x + borderThickness,
                    aRect.y + borderThickness,
                    aRect.width - borderThickness * 2,
                    aRect.height - borderThickness * 2),
                Preferences.Quaternary);
        }


        /// Draws the "Task" and "Script" texts for QQQs.
        private void DrawTaskAndScript(Rect aRect, int qqqIndex, float aTaskHeight, float aScriptHeight)
        {
            // Task.
            var taskRect = aRect;
            taskRect.x = Constants.OFFSET * 2;
            taskRect.y += Constants.OFFSET;
            taskRect.height = aTaskHeight;
            EditorGUI.LabelField(taskRect, TODO.CurrentQQQs[qqqIndex].Task, _taskStyle);

            // Script.
            var scriptRect = aRect;
            scriptRect.x = Constants.OFFSET * 2;
            scriptRect.y += (taskRect.height + 8);
            scriptRect.height = aScriptHeight;
            var scriptLabel = CreateScriptLabelText(TODO.CurrentQQQs[qqqIndex]);

            EditorGUI.LabelField(scriptRect, scriptLabel, _scriptStyle);

            // Open editor on click.
            EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp && scriptRect.Contains(Event.current.mousePosition))
            {
                QQQOps.OpenScript(TODO.CurrentQQQs[qqqIndex]);
            }
        }


        /// Draws the "Pin to top" checkbox.
        private void DrawPin(Rect aRect, int qqqIndex)
        {
            var wasPinned = TODO.CurrentQQQs[qqqIndex].IsPinned;
            var isPinned = EditorGUI.ToggleLeft(aRect, "Pin to top", wasPinned, _scriptStyle);

            if (isPinned != wasPinned)
            {
                TODO.QQQs[qqqIndex].IsPinned = isPinned;
                QQQsChanged = true;
            }
        }


        /// Draw Edit and Complete buttons.
        private void DrawEditAndComplete(Rect aRect, int qqqIndex)
        {
            Rect editRect, completeRect;
            GUIContent editContent, completeContent;

            aRect.x = position.width - _BUTTON_WIDTH - Constants.OFFSET * 2.5f;

            if (_showingScrollbar)
            {
                aRect.x -= Constants.OFFSET * 2.5f;
            }

            SetupButton_Edit(aRect, out editRect, out editContent);
            SetupButton_Complete(aRect, out completeRect, out completeContent);

            if (Controls.Button(editRect, editContent))
            {
                WindowEdit.Init(TODO.QQQs[qqqIndex]);
            }


            if (Controls.Button(completeRect, completeContent))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs)
                {
                    var token = Preferences.TODOToken;
                    if (EditorUtility.DisplayDialog("Complete " + token,
                        "Are you sure you're done with this " + token + "?\nIt will be removed from the code too.",
                        "Complete " + token, "Cancel"))
                    {
                        canExecute = true;
                    }
                }
                else
                {
                    canExecute = true;
                }

                // Actually do the thing.
                if (canExecute)
                {
                    TODO = QQQOps.CompleteQQQ(TODO, TODO.QQQs[qqqIndex]);
                }
            }
        }


        /// Draw Process, Add, Refresh and Settings.
        private void DrawBottomButtons()
        {
            Rect processRect, addRect, refreshRect, settingsRect;
            GUIContent processContent, addContent, refreshContent, settingsContent;

            SetupButton_Process(out processRect, out processContent);
            SetupButton_Add(out addRect, out addContent);
            SetupButton_Refresh(out refreshRect, out refreshContent);
            SetupButton_Settings(out settingsRect, out settingsContent);

            // Process scripts.
            if (Controls.Button(processRect, processContent))
            {
                QQQOps.FindAllScripts();
                TODO.QQQs = QQQOps.GetQQQsFromAllScripts();
            }

            // Add new QQQ.
            if (Controls.Button(addRect, addContent))
            {
                WindowAdd.Init();
            }

            // Refresh list of QQQs.
            if (Controls.Button(refreshRect, refreshContent))
            {
                TODO.QQQs = QQQOps.RefreshQQQs(TODO.QQQs);
            }

            // Open settings.
            if (Controls.Button(settingsRect, settingsContent))
            {
                CloseOtherWindows();

                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }
        }


        /// Draw a line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (Constants.OFFSET * 7), position.width, 1);
            EditorGUI.DrawRect(separator, Preferences.Secondary);
        }


        private void SetupButton_Edit(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += Constants.OFFSET + 2;
            anEditRect.width = _BUTTON_WIDTH;
            anEditRect.height = _BUTTON_HEIGHT;

            anEditContent = new GUIContent("Edit", "Edit this task");
        }


        private void SetupButton_Complete(Rect aRect, out Rect aCompleteRect, out GUIContent aCompleteContent)
        {
            aCompleteRect = aRect;
            aCompleteRect.y += _BUTTON_HEIGHT + Constants.OFFSET + 8;
            aCompleteRect.width = _BUTTON_WIDTH;
            aCompleteRect.height = _BUTTON_HEIGHT;

            aCompleteContent = new GUIContent("Complete", "Complete this task");
        }


        private void SetupButton_Process(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - _BUTTON_WIDTH * 2 - Constants.OFFSET * 3, position.height - (_BUTTON_HEIGHT * 1.4f),
                _BUTTON_WIDTH, _BUTTON_HEIGHT);
            aContent = new GUIContent("Process", "Process scripts");
        }


        private void SetupButton_Add(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - _BUTTON_WIDTH - Constants.OFFSET, position.height - (_BUTTON_HEIGHT * 1.4f),
                _BUTTON_WIDTH, _BUTTON_HEIGHT);
            aContent = new GUIContent("Add", "Add a new QQQ");
        }


        private void SetupButton_Refresh(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 + Constants.OFFSET, position.height - (_BUTTON_HEIGHT * 1.4f), _BUTTON_WIDTH,
                _BUTTON_HEIGHT);
            aContent = new GUIContent("Refresh", "Refresh list");
        }


        private void SetupButton_Settings(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 + _BUTTON_WIDTH + Constants.OFFSET * 3, position.height - (_BUTTON_HEIGHT * 1.4f),
                _BUTTON_WIDTH, _BUTTON_HEIGHT);
            aContent = new GUIContent("Settings", "Open Settings");
        }


        /// Create the text that indicates where the task is.
        private string CreateScriptLabelText(QQQ aQQQ)
        {
            return "Line " + (aQQQ.LineNumber + 1) + " in \"" + aQQQ.Script + "\"";
        }


        /// Get the correct color for a priority rectangle.
        private Color GetQQQPriorityColor(int aPriority)
        {
            Color col;
            switch (aPriority)
            {
                case 1:
                    col = Preferences.PriorityUrgent;
                    break;
                case 3:
                    col = Preferences.PriorityMinor;
                    break;
                default:
                    col = Preferences.PriorityNormal;
                    break;
            }
            return col;
        }


        /// Update sizes used in layouting based on the window size.
        private void UpdateLayoutingSizes()
        {
            var width = position.width - Constants.OFFSET * 2;
            _scrollAreaRect = new Rect(Constants.OFFSET, Constants.OFFSET, width - Constants.OFFSET * 2, position.height - Constants.ICON_SIZE - Constants.OFFSET * 4);
            _scrollViewRect = _scrollAreaRect;

            if (_showingScrollbar)
            {
                _buttonsWidth = _BUTTON_WIDTH + Constants.OFFSET * 3;
            }
            else
            {
                _buttonsWidth = _BUTTON_WIDTH + Constants.OFFSET * 1;
            }

            _qqqWidth = (int) width - _buttonsWidth - Constants.OFFSET * 3;
        }


        /// Close open sub-windows (add, edit) when opening prefs.
        private void CloseOtherWindows()
        {
            if (WindowAdd.IsOpen)
            {
                GetWindow(typeof(WindowAdd)).Close();
            }
            if (WindowEdit.IsOpen)
            {
                GetWindow(typeof(WindowEdit)).Close();
            }
            if (WindowWelcome.IsOpen)
            {
                GetWindow(typeof(WindowWelcome)).Close();
            }
        }

        #endregion
    }
}