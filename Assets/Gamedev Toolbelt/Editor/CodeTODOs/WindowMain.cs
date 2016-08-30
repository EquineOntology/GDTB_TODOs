using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalhydra.gdtb.codetodos
{
    public class WindowMain : EditorWindow
    {
        public static List<QQQ> QQQs = new List<QQQ>();

        public static WindowMain Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        private GUISkin _skin;
        private GUIStyle _style_task, _style_script, _style_buttonText;

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;

        private int _width_qqq, _width_buttons;
        private float _height_totalQQQHeight = 0;
        private int _offset = 5;

        private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        private Rect _rect_scrollArea, _rect_scrollView, _rect_qqq, _rect_editAndComplete;
        private bool _showingScrollbar = false;


        // ====================================================================
        [MenuItem("Window/Gamedev Toolbelt/CodeTODOs %q")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.SetMinSize();
            window.LoadSkin();
            window.LoadStyles();
            window.UpdateLayoutingSizes();

            if (QQQs.Count == 0 && Preferences.AutoRefresh == true)
            {
                QQQOps.RefreshQQQs();
            }
            else if (Preferences.AutoRefresh == false)
            {
                QQQs.Clear();
                QQQs.AddRange(IO.LoadStoredQQQs());
            }
            window.Show();
        }


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


        /// Called when the window is closed.
        private void OnDestroy()
        {
            if (Preferences.AutoRefresh == false)
            {
                IO.WriteQQQsToFile();
            }
            Resources.UnloadUnusedAssets();
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            GUI.skin = _skin; // Without this, almost everything will work aside from the scrollbar.

            // If the list is clean (for instance because we just recompiled) load QQQs based on preferences.
            if (QQQs.Count == 0)
            {
                if (Preferences.AutoRefresh == true)
                {
                    QQQOps.RefreshQQQs();
                }
                else
                {
                    QQQs.Clear();
                    QQQs.AddRange(IO.LoadStoredQQQs());
                }
            }

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


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        /// If there are no QQQs, tell the user.
        private void DrawNoQQQsMessage()
        {
            var label = "There are currently no tasks.\nAdd one by writing a comment with " + Preferences.TODOToken + " in it.\n\nIf you see this after the project recompiled,\ntry refreshing the window!\nYour tasks should come back just fine.";
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

                if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
                {
                    height_qqqBackground += 4;
                }

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


        /// Create the text that indicates where the task is.
        private string CreateScriptLabelText (QQQ aQQQ)
        {
            return "Line " + (aQQQ.LineNumber + 1) + " in \"" + aQQQ.Script + "\"";
        }


        #region EditAndComplete
        /// Select which format to use based on the user preference.
        private void DrawEditAndComplete(Rect aRect, QQQ aQQQ)
        {
            Rect editRect, completeRect;
            GUIContent editContent, completeContent;

            aRect.x = position.width - _offset * 2;
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
            {
                aRect.x = aRect.x - ButtonWidth - _offset * 0.5f;
            }
            else
            {
                aRect.x = aRect.x - IconSize - _offset * 0.5f;
            }

            if (_showingScrollbar == true)
            {
                aRect.x -= _offset * 2.5f;
            }

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Edit_default(aRect, out editRect, out editContent);
                    Button_Complete_default(aRect, out completeRect, out completeContent);
                    break;
                default:
                    Button_Edit_icon(aRect, out editRect, out editContent);
                    Button_Complete_icon(aRect, out completeRect, out completeContent);
                    break;
            }

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
        private void Button_Edit_default(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset + 2;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this task");
        }
        private void Button_Complete_default(Rect aRect, out Rect aCompleteRect, out GUIContent aCompleteContent)
        {
            aCompleteRect = aRect;
            aCompleteRect.y += ButtonHeight + _offset + 8;
            aCompleteRect.width = ButtonWidth;
            aCompleteRect.height = ButtonHeight;

            aCompleteContent = new GUIContent("Complete", "Complete this task");
        }

        private void Button_Edit_icon(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset + 2;
            anEditRect.width = IconSize;
            anEditRect.height = IconSize;
            anEditContent = new GUIContent(DrawingUtils.Texture_Edit, "Edit this EditorPref");
        }
        private void Button_Complete_icon(Rect aRect, out Rect aCompleteRect, out GUIContent aCompleteContent)
        {
            aCompleteRect = aRect;
            aCompleteRect.y += IconSize + _offset + 8;
            aCompleteRect.width = IconSize;
            aCompleteRect.height = IconSize;

            aCompleteContent = new GUIContent(DrawingUtils.Texture_Complete, "Complete this task");
        }
        #endregion


        #region A-R-S buttons
        /// Draw Add, Refresh and Settings based on preferences.
        private void DrawBottomButtons()
        {
            Rect addRect, refreshRect, settingsRect;
            GUIContent addContent, refreshContent, settingsContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Add_default(out addRect, out addContent);
                    Button_Refresh_default(out refreshRect, out refreshContent);
                    Button_Settings_default(out settingsRect, out settingsContent);
                    break;
				case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Add_icon(out addRect, out addContent);
                    Button_Refresh_icon(out refreshRect, out refreshContent);
                    Button_Settings_icon(out settingsRect, out settingsContent);
                    break;
            }

             // Add new QQQ.
            if (Controls.Button(addRect, addContent))
            {
                WindowAdd.Init();
            }
            //DrawingUtils.DrawButton(addRect, Preferences.ButtonsDisplay, DrawingUtils.Texture_Add, addContent.text, _style_buttonText);


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


        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth - IconSize * 2), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new QQQ");
        }
        private void Button_Refresh_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth/2), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }
        private void Button_Settings_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth / 2 + _offset), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }


        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 2), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent(DrawingUtils.Texture_Add, "Add a new QQQ");
        }
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize/2), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent(DrawingUtils.Texture_Refresh, "Refresh list");
        }
        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent(DrawingUtils.Texture_Settings, "Open Settings");
        }
        #endregion


        /// Draw a line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (_offset * 7), position.width, 1);
            EditorGUI.DrawRect(separator, Preferences.Color_Secondary);
        }


        /// Update sizes used in layouting based on the window size.
        private void UpdateLayoutingSizes()
        {
            var width = position.width - _offset * 2;
            _rect_scrollArea = new Rect(_offset, _offset, width - _offset * 2, position.height - IconSize - _offset * 4);
            _rect_scrollView = _rect_scrollArea;

            // Buttons have different sizes based on preferences.
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                if (_showingScrollbar)
                {
                    _width_buttons = IconSize + _offset * 3;
                }
                else
                {
                    _width_buttons = IconSize + _offset;
                }
            }
            else
            {
                if (_showingScrollbar)
                {
                    _width_buttons = ButtonWidth + _offset * 3;
                }
                else
                {
                    _width_buttons = ButtonWidth + _offset * 1;
                }
            }

            _width_qqq = (int)width - _width_buttons - _offset * 3;
        }


        /// Load CodeTODOs custom skin.
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
            _skin.verticalScrollbarThumb.fixedWidth = 6;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowMain)) as WindowMain;
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                window.minSize = new Vector2(222f, 150f);
            }
            else
            {
                window.minSize = new Vector2(322f, 150f);
            }
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
        }


        private void Update()
        {
            // Unfortunately, IMGUI is not really responsive to events, e.g. changing the style of a button
            // (like when you press it) shows some pretty abysmal delays in the GUI, the button will light up
            // and down too late after the actual click. We force the UI to update more often instead.
            Repaint();
        }
    }
}