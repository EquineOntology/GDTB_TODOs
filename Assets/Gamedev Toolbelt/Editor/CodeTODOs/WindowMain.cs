using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.codetodos
{
    public class WindowMain : EditorWindow
    {
#region FIELDS AND PROPERTIES

        // Fields.
        public static List<QQQ> QQQs = new List<QQQ>();

        private GUISkin _skin;
        private GUIStyle _style_task, _style_script, _style_buttonText;

        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;

        private int _width_qqq, _width_buttons;
        private float _height_totalQQQHeight = 0;
        private int _offset = 5;

        private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        private Rect _rect_scrollArea, _rect_scrollView, _rect_qqq, _rect_editAndComplete;
        private bool _showingScrollbar = false;

        // Properties.
        public static WindowMain Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

#endregion


#region MONOBEHAVIOUR METHODS

        public void OnEnable()
        {
        #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                titleContent = new GUIContent("CodeTODOs");
        #else
                title = "CodeTODOs";
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
            Resources.UnloadUnusedAssets();
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

        [MenuItem("Window/Gamedev Toolbelt/CodeTODOs/Open CodeTODOs %w", false, 1)]
        public static void Init()
        {
            // If CodeTODOs has not been initialized, or EditorPrefs have been lost for some reason, reset them to default, and show the first start window.
            if(!EditorPrefs.HasKey("GDTB_CodeTODOs_firsttime") || EditorPrefs.GetBool("GDTB_CodeTODOs_firsttime", false) == false)
            {
                Preferences.InitExtension();
            }

            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.SetMinSize();
            window.LoadSkin();
            window.LoadStyles();
            window.UpdateLayoutingSizes();

            IO.LoadScripts();

            QQQs.Clear();
            QQQs.AddRange(IO.LoadStoredQQQs());

            window.Show();

            if(Preferences.ShowWelcome == true)
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
            _style_script = _skin.GetStyle("GDTB_CodeTODOs_script");
            _style_script.active.textColor = Preferences.Color_Tertiary;
            _style_script.normal.textColor = Preferences.Color_Tertiary;
            _style_task = _skin.GetStyle("GDTB_CodeTODOs_task");
            _style_task.active.textColor = Preferences.Color_Secondary;
            _style_task.normal.textColor = Preferences.Color_Secondary;
            _style_buttonText = _skin.GetStyle("GDTB_CodeTODOs_buttonText");
            _style_buttonText.active.textColor = Preferences.Color_Tertiary;
            _style_buttonText.normal.textColor = Preferences.Color_Tertiary;

            _skin.settings.selectionColor = Preferences.Color_Secondary;

            // Change scrollbar color.
            var scrollbar = Resources.Load(Constants.TEX_SCROLLBAR, typeof(Texture2D)) as Texture2D;
        #if UNITY_5 || UNITY_5_3_OR_NEWER
            scrollbar.SetPixel(0,0, Preferences.Color_Secondary);
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
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        /// If there are no QQQs, tell the user.
        private void DrawNoQQQsMessage()
        {
            var label = "There are currently no tasks.\nAdd one by writing a comment with " + Preferences.TODOToken + " in it.\n\nIf it's the first time you open CodeTODOs,\npress the 'Process scripts' button.\n\nIf you want to refresh the list,\npress the 'Refresh tasks' button.";
            var labelContent = new GUIContent(label);

            Vector2 labelSize;
            #if UNITY_5_3_OR_NEWER
                labelSize = EditorStyles.centeredGreyMiniLabel.CalcSize(labelContent);
            #else
                labelSize = EditorStyles.wordWrappedMiniLabel.CalcSize(labelContent);
            #endif

            var labelRect = new Rect(position.width / 2 - labelSize.x / 2, position.height / 2 - labelSize.y / 2 - _offset * 2.5f, labelSize.x, labelSize.y);
            #if UNITY_5_3_OR_NEWER
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.centeredGreyMiniLabel);
            #else
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.wordWrappedMiniLabel);
            #endif
        }


        /// Draw the list of QQQs.
        private void DrawQQQs()
        {
            _rect_scrollView.height = _height_totalQQQHeight - _offset;

            // Diminish the width of scrollview and scroll area so that the scollbar is offset from the right edge of the window.
            _rect_scrollArea.width += IconSize - _offset;
            _rect_scrollView.width -= _offset;

            // Change size of the scroll area so that it fills the window when there's no scrollbar.
            if (_showingScrollbar == false)
            {
                _rect_scrollView.width += IconSize;
            }

            _scrollPosition = GUI.BeginScrollView(_rect_scrollArea, _scrollPosition, _rect_scrollView);

            _height_totalQQQHeight = _offset; // This includes all prefs, not just a single one.

            for (var i = 0; i < QQQs.Count; i++)
            {
                var taskContent = new GUIContent(QQQs[i].Task);
                var scriptContent = new GUIContent(CreateScriptLabelText(QQQs[i]));
                var taskHeight = _style_task.CalcHeight(taskContent, _width_qqq);
                var scriptHeight = _style_script.CalcHeight(scriptContent, _width_qqq);

                var height_qqqBackground = taskHeight + scriptHeight + _offset * 2;
                height_qqqBackground = height_qqqBackground < IconSize * 2.7f ? IconSize * 2.7f : height_qqqBackground;

                _rect_qqq = new Rect(0, _height_totalQQQHeight, _width_qqq, height_qqqBackground);
                _rect_editAndComplete = new Rect(_width_qqq + (_offset * 2), _rect_qqq.y, _width_buttons, height_qqqBackground);

                var rect_qqqBackground = _rect_qqq;
                rect_qqqBackground.height = height_qqqBackground + _offset / 2;

                if (_showingScrollbar == true) // If we're not showing the scrollbar, QQQs need to be larger too.
                {
                    rect_qqqBackground.width = position.width - _offset - IconSize;
                }
                else
                {
                    rect_qqqBackground.width = position.width - _offset * 2.5f;
                }

                rect_qqqBackground.x += _offset;

                _height_totalQQQHeight += rect_qqqBackground.height + _offset;

                // If the user removes a QQQ from the list in the middle of a draw call, the index in the for loop stays the same but QQQs.Count diminishes.
                // I couldn't find a way around it, so what we do is swallow the exception and wait for the next draw call.
                try
                {
                    DrawQQQBackground(rect_qqqBackground,  GetQQQPriorityColor((int)QQQs[i].Priority));
                    DrawTaskAndScript(_rect_qqq, QQQs[i], taskHeight, scriptHeight);
                    DrawEditAndComplete(_rect_editAndComplete, QQQs[i]);
                }
                catch (System.Exception) { }
            }

            // Are we showing the scrollbar?
            if (_rect_scrollArea.height < _rect_scrollView.height)
            {
                _showingScrollbar = true;
            }
            else
            {
                _showingScrollbar = false;
            }

            GUI.EndScrollView();
        }


        /// Draw the background that separates the QQQs visually.
        private void DrawQQQBackground(Rect aRect, Color aColor)
        {
            EditorGUI.DrawRect(aRect, aColor);
            EditorGUI.DrawRect(new Rect(
                    aRect.x + Constants.BUTTON_BORDER_THICKNESS,
                    aRect.y + Constants.BUTTON_BORDER_THICKNESS,
                    aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2,
                    aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2),
                Preferences.Color_Quaternary);
        }


        /// Draws the "Task" and "Script" texts for QQQs.
        private void DrawTaskAndScript(Rect aRect, QQQ aQQQ, float aTaskHeight, float aScriptHeight)
        {
            // Task.
            var taskRect = aRect;
            taskRect.x = _offset * 2;
            taskRect.y += _offset;
            taskRect.height = aTaskHeight;
            EditorGUI.LabelField(taskRect, aQQQ.Task, _style_task);

            // Script.
            var scriptRect = aRect;
            scriptRect.x = _offset * 2;
            scriptRect.y += (taskRect.height + 8);
            scriptRect.height = aScriptHeight;
            var scriptLabel = CreateScriptLabelText(aQQQ);

            EditorGUI.LabelField(scriptRect, scriptLabel, _style_script);

            // Open editor on click.
            EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp && scriptRect.Contains(Event.current.mousePosition))
            {
                QQQOps.OpenScript(aQQQ);
            }
        }


        /// Draw Edit and Complete buttons.
        private void DrawEditAndComplete(Rect aRect, QQQ aQQQ)
        {
            Rect editRect, completeRect;
            GUIContent editContent, completeContent;

            aRect.x = position.width - ButtonWidth - _offset * 2.5f;

            if (_showingScrollbar == true)
            {
                aRect.x -= _offset * 2.5f;
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
                if (Preferences.ShowConfirmationDialogs == true)
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
                if (canExecute == true)
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
            var separator = new Rect(0, position.height - (_offset * 7), position.width, 1);
            EditorGUI.DrawRect(separator, Preferences.Color_Secondary);
        }


        private void SetupButton_Edit(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset + 2;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this task");
        }


        private void SetupButton_Complete(Rect aRect, out Rect aCompleteRect, out GUIContent aCompleteContent)
        {
            aCompleteRect = aRect;
            aCompleteRect.y += ButtonHeight + _offset + 8;
            aCompleteRect.width = ButtonWidth;
            aCompleteRect.height = ButtonHeight;

            aCompleteContent = new GUIContent("Complete", "Complete this task");
        }


        private void SetupButton_Process(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - ButtonWidth * 2 - _offset * 3, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Process", "Process scripts");
        }


        private void SetupButton_Add(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 - ButtonWidth - _offset, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new QQQ");
        }


        private void SetupButton_Refresh(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 + _offset, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }


        private void SetupButton_Settings(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(position.width / 2 + ButtonWidth + _offset * 3, position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
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
                    col = Preferences.PriColor1;
                    break;
                case 3:
                    col = Preferences.PriColor3;
                    break;
                case 2:
                default:
                    col = Preferences.PriColor2;
                    break;
            }
            return col;
        }


        /// Update sizes used in layouting based on the window size.
        private void UpdateLayoutingSizes()
        {
            var width = position.width - _offset * 2;
            _rect_scrollArea = new Rect(_offset, _offset, width - _offset * 2, position.height - IconSize - _offset * 4);
            _rect_scrollView = _rect_scrollArea;

            if (_showingScrollbar)
            {
                _width_buttons = ButtonWidth + _offset * 3;
            }
            else
            {
                _width_buttons = ButtonWidth + _offset * 1;
            }

            _width_qqq = (int)width - _width_buttons - _offset * 3;
        }


        /// Close open sub-windows (add, edit) when opening prefs.
        private void CloseOtherWindows()
        {
            if (WindowAdd.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowAdd)).Close();
            }
            if (WindowEdit.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowEdit)).Close();
            }
            if (WindowWelcome.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowWelcome)).Close();
            }
        }

#endregion

    }
}
