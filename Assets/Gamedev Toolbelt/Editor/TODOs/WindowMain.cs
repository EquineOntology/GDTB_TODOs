using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.todos
{
    public class WindowMain : EditorWindow
    {
#region FIELDS AND PROPERTIES

        // Constants.
        private const int IconSize = Constants.ICON_SIZE;
        private const int Offset = Constants.OFFSET;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;

        // Fields.
        public static List<QQQ> QQQs = new List<QQQ>();

        public static bool WasHiddenByReimport = false;

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
        }


        private void OnDestroy()
        {
            IO.WriteQQQsToFile();
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            GUI.skin = _skin; // Without this, almost everything will work aside from the scrollbar.

            DrawWindowBackground();

            // If the list is still clean after the above, then we really have no QQQs.
            if (QQQs.Count == 0)
            {
                DrawNoQQQsMessage();
            }

            if (WasHiddenByReimport)
            {
                QQQs.Clear();
                QQQs.AddRange(IO.LoadStoredQQQs());
                GetWindow(typeof(WindowMain)).Show();
                WasHiddenByReimport = false;
            }

            QQQOps.ReorderQQQs();
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

        [MenuItem("Window/Gamedev Toolbelt/TODOs/Open TODOs %&w", false, 1)]
        public static void Init()
        {
            // If TODOs has not been initialized, or EditorPrefs have been lost for some reason, reset them to default, and show the first start window.
            if(!EditorPrefs.HasKey("GDTB_TODOs_firsttime") || EditorPrefs.GetBool("GDTB_TODOs_firsttime", false) == false)
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

            QQQs.Clear();
            QQQs.AddRange(IO.LoadStoredQQQs());

            window.Show();

            if(Preferences.ShowWelcome)
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
        #if UNITY_5 || UNITY_5_3_OR_NEWER
            scrollbar.SetPixel(0,0, Preferences.Secondary);
        #else
			var pixels = scrollbar.GetPixels();
			// We do it like this because minimum texture size in older versions of Unity is 2x2.
			for(var i = 0; i < pixels.GetLength(0); i++)
			{
				scrollbar.SetPixel(i, 0, Preferences.Color_Secondary);
				scrollbar.SetPixel(i, 1, Preferences.Color_Secondary);
			}
        #endif

            scrollbar.Apply();
            _skin.verticalScrollbarThumb.normal.background = scrollbar;
            _skin.verticalScrollbarThumb.active.background = scrollbar;
            _skin.verticalScrollbarThumb.fixedWidth = 6;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowMain)) as WindowMain;
            window.minSize = new Vector2(322f, 150f);
        }




        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Primary);
        }


        /// If there are no QQQs, tell the user.
        private void DrawNoQQQsMessage()
        {
            var label = "There are currently no tasks.\nAdd one by writing a comment with " + Preferences.TODOToken + " in it.\n\nIf it's the first time you open TODOs,\npress the 'Process scripts' button.\n\nIf you just reimported some files,\npress the 'Refresh tasks' button.";
            var labelContent = new GUIContent(label);

            Vector2 labelSize;
            #if UNITY_5_3_OR_NEWER
                labelSize = EditorStyles.centeredGreyMiniLabel.CalcSize(labelContent);
            #else
                labelSize = EditorStyles.wordWrappedMiniLabel.CalcSize(labelContent);
            #endif

            var labelRect = new Rect(position.width / 2 - labelSize.x / 2, position.height / 2 - labelSize.y / 2 - Offset * 2.5f, labelSize.x, labelSize.y);
            #if UNITY_5_3_OR_NEWER
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.centeredGreyMiniLabel);
            #else
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.wordWrappedMiniLabel);
            #endif
        }


        /// Draw the list of QQQs.
        private void DrawQQQs()
        {
            _scrollViewRect.height = _totalQQQHeight - Offset;

            // Diminish the width of scrollview and scroll area so that the scollbar is offset from the right edge of the window.
            _scrollAreaRect.width += IconSize - Offset;
            _scrollViewRect.width -= Offset;

            // Change size of the scroll area so that it fills the window when there's no scrollbar.
            if (_showingScrollbar == false)
            {
                _scrollViewRect.width += IconSize;
            }

            _scrollPosition = GUI.BeginScrollView(_scrollAreaRect, _scrollPosition, _scrollViewRect);

            _totalQQQHeight = Offset; // This includes all prefs, not just a single one.

            foreach (var qqq in QQQs)
            {
                var taskContent = new GUIContent(qqq.Task);
                var scriptContent = new GUIContent(CreateScriptLabelText(qqq));
                var taskHeight = _taskStyle.CalcHeight(taskContent, _qqqWidth);
                var scriptHeight = _scriptStyle.CalcHeight(scriptContent, _qqqWidth);
                var pinHeight = Constants.LINE_HEIGHT;

                var qqqBackgroundHeight = taskHeight + scriptHeight + pinHeight + Offset * 2;
                qqqBackgroundHeight = qqqBackgroundHeight < IconSize * 2.7f ? IconSize * 2.7f : qqqBackgroundHeight;

                _qqqRect = new Rect(0, _totalQQQHeight, _qqqWidth, qqqBackgroundHeight);
                _pinRect = new Rect(Offset * 2, _totalQQQHeight + taskHeight + scriptHeight + Offset * 2, 70, Constants.LINE_HEIGHT);
                _editAndCompleteRect = new Rect(_qqqWidth + (Offset * 2), _qqqRect.y, _buttonsWidth, qqqBackgroundHeight);

                var qqqBackgroundRect = _qqqRect;
                qqqBackgroundRect.height = qqqBackgroundHeight + Offset / 2;

                if (_showingScrollbar) // If we're not showing the scrollbar, QQQs need to be larger too.
                {
                    qqqBackgroundRect.width = position.width - Offset - IconSize;
                }
                else
                {
                    qqqBackgroundRect.width = position.width - Offset * 2.5f;
                }

                qqqBackgroundRect.x += Offset;

                _totalQQQHeight += qqqBackgroundRect.height + Offset;

                // If the user removes a QQQ from the list in the middle of a draw call, the index in the for loop stays the same but QQQs.Count diminishes.
                // I couldn't find a way around it, so what we do is swallow the exception and wait for the next draw call.
                try
                {
                    DrawQQQBackground(qqqBackgroundRect,  GetQQQPriorityColor((int)qqq.Priority));
                    DrawTaskAndScript(_qqqRect, qqq, taskHeight, scriptHeight);
                    DrawPin(_pinRect, qqq);
                    DrawEditAndComplete(_editAndCompleteRect, qqq);
                }
                catch (System.Exception) { }
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
        private void DrawTaskAndScript(Rect aRect, QQQ aQQQ, float aTaskHeight, float aScriptHeight)
        {
            // Task.
            var taskRect = aRect;
            taskRect.x = Offset * 2;
            taskRect.y += Offset;
            taskRect.height = aTaskHeight;
            EditorGUI.LabelField(taskRect, aQQQ.Task, _taskStyle);

            // Script.
            var scriptRect = aRect;
            scriptRect.x = Offset * 2;
            scriptRect.y += (taskRect.height + 8);
            scriptRect.height = aScriptHeight;
            var scriptLabel = CreateScriptLabelText(aQQQ);

            EditorGUI.LabelField(scriptRect, scriptLabel, _scriptStyle);

            // Open editor on click.
            EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp && scriptRect.Contains(Event.current.mousePosition))
            {
                QQQOps.OpenScript(aQQQ);
            }
        }


        /// Draws the "Pin to top" checkbox.
        private void DrawPin(Rect aRect, QQQ qqq)
        {
            qqq.IsPinned = EditorGUI.ToggleLeft(aRect, "Pin to top", qqq.IsPinned, _scriptStyle);
        }


        /// Draw Edit and Complete buttons.
        private void DrawEditAndComplete(Rect aRect, QQQ aQQQ)
        {
            Rect editRect, completeRect;
            GUIContent editContent, completeContent;

            aRect.x = position.width - ButtonWidth - Offset * 2.5f;

            if (_showingScrollbar)
            {
                aRect.x -= Offset * 2.5f;
            }

            SetupButton_Edit(aRect, out editRect, out editContent);
            SetupButton_Complete(aRect, out completeRect, out completeContent);

            if (Controls.Button(editRect, editContent))
            {
                WindowEdit.Init(aQQQ);
            }


            if (Controls.Button(completeRect, completeContent))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs)
                {
                    var token = Preferences.TODOToken;
                    if (EditorUtility.DisplayDialog("Complete " + token, "Are you sure you're done with this " + token + "?\nIt will be removed from the code too.", "Complete " + token, "Cancel"))
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
                    QQQOps.CompleteQQQ(aQQQ);
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
            if(Controls.Button(processRect, processContent))
            {
                QQQOps.FindAllScripts();
                QQQOps.GetQQQsFromAllScripts();
            }

            // Add new QQQ.
            if (Controls.Button(addRect, addContent))
            {
                WindowAdd.Init();
            }

            // Refresh list of QQQs.
            if (Controls.Button(refreshRect, refreshContent))
            {
                QQQOps.RefreshQQQs();
            }

            // Open settings.
            if (Controls.Button(settingsRect, settingsContent))
            {
                CloseOtherWindows();

                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }
        }


        /// Draw a line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (Offset * 7), position.width, 1);
            EditorGUI.DrawRect(separator, Preferences.Secondary);
        }


        private void SetupButton_Edit(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += Offset + 2;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this task");
        }


        private void SetupButton_Complete(Rect aRect, out Rect aCompleteRect, out GUIContent aCompleteContent)
        {
            aCompleteRect = aRect;
            aCompleteRect.y += ButtonHeight + Offset + 8;
            aCompleteRect.width = ButtonWidth;
            aCompleteRect.height = ButtonHeight;

            aCompleteContent = new GUIContent("Complete", "Complete this task");
        }


        private void SetupButton_Process(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - ButtonWidth * 2 - Offset * 3, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Process", "Process scripts");
        }


        private void SetupButton_Add(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - ButtonWidth - Offset, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new QQQ");
        }


        private void SetupButton_Refresh(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 + Offset, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }


        private void SetupButton_Settings(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 + ButtonWidth + Offset * 3, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }


        /// Create the text that indicates where the task is.
        private string CreateScriptLabelText (QQQ aQQQ)
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
            var width = position.width - Offset * 2;
            _scrollAreaRect = new Rect(Offset, Offset, width - Offset * 2, position.height - IconSize - Offset * 4);
            _scrollViewRect = _scrollAreaRect;

            if (_showingScrollbar)
            {
                _buttonsWidth = ButtonWidth + Offset * 3;
            }
            else
            {
                _buttonsWidth = ButtonWidth + Offset * 1;
            }

            _qqqWidth = (int)width - _buttonsWidth - Offset * 3;
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
