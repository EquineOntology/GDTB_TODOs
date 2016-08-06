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
        private GUIStyle _style_priority, _style_task, _style_script, _style_buttonText;

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 70;
        private const int ButtonHeight = 18;

        private int _unit, _width_priority, _width_priorityLabel, _width_qqq, _width_buttons;
        private float _height_totalQQQHeight = 0;
        private int _offset = 5;

        private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        private Rect _rect_scrollArea, _rect_scrollView, _rect_qqq, _rect_priority, _rect_editAndComplete;
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

            window._width_priorityLabel = (int)window._style_priority.CalcSize(new GUIContent("URGENT")).x; // Not with the other layouting sizes because it only needs to be done once.

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
            #if UNITY_UNITY_5_3_OR_NEWER
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

                _rect_qqq = new Rect(_width_priority, _height_totalQQQHeight, _width_qqq, height_qqqBackground);
                _rect_priority = new Rect(0, _rect_qqq.y, _width_priority, height_qqqBackground);
                _rect_editAndComplete = new Rect(_width_priority + _width_qqq + (_offset * 2), _rect_qqq.y, _width_buttons, height_qqqBackground);

                var rect_qqqBackground = _rect_priority;
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
                    DrawQQQBackground(rect_qqqBackground);
                    DrawPriority(_rect_priority, QQQs[i], height_qqqBackground);
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
        private void DrawQQQBackground(Rect aRect)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(
                    aRect.x + Constants.BUTTON_BORDER_THICKNESS,
                    aRect.y + Constants.BUTTON_BORDER_THICKNESS,
                    aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2,
                    aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2),
                Preferences.Color_Quaternary);
        }


        #region QQQPriorityMethods
        /// Select which priority format to use based on the user preference.
        private void DrawPriority(Rect aRect, QQQ aQQQ, float aBackgroundHeight = 30)
        {
            switch (Preferences.PriorityDisplay)
            {
                case PriorityDisplayFormat.TEXT_ONLY:
                    DrawPriority_Text(aRect, aQQQ);
                    break;
                case PriorityDisplayFormat.ICON_ONLY:
                    DrawPriority_Icon(aRect, aQQQ);
                    break;
                case PriorityDisplayFormat.ICON_AND_TEXT:
                    DrawPriority_IconAndText(aRect, aQQQ);
                    break;
                case PriorityDisplayFormat.BARS:
                default:
                    DrawPriority_Bars(aRect, aQQQ, aBackgroundHeight);
                    break;
            }
        }


        /// Draw priority for the "Bars" setting.
        private void DrawPriority_Bars(Rect aRect, QQQ aQQQ, float aBackgroundHeight)
        {
            var priorityRect = aRect;
            var newY = 0;
            var newX = 0;

            newX = (int)priorityRect.x + (_offset * 2);
            newY = (int)priorityRect.y + _offset;

            priorityRect.width = IconSize;
            priorityRect.height = aBackgroundHeight - (_offset * 2);
            priorityRect.position = new Vector2(newX, newY);

            var color = GetQQQPriorityColor((int)aQQQ.Priority);

            // Draw border rectangle.
            EditorGUI.DrawRect(priorityRect, Preferences.BorderColor);

            // Draw the priority bar.
            EditorGUI.DrawRect(new Rect(priorityRect.x + 1, priorityRect.y + 1, priorityRect.width - 2, priorityRect.height - 2), color);
        }


        /// Draw priority for the "Icon only" setting.
        private void DrawPriority_Icon(Rect aRect, QQQ aQQQ)
        {
            // Prepare the rectangle for layouting. The layout is "space-icon-space".
            var priorityRect = aRect;
            var newY = 0.0f;
            var newX = 0.0f;
            if (aRect.width > IconSize + (_offset * 2))
            {
                newX = priorityRect.x + IconSize / 2 + _offset;
                newY = priorityRect.y + IconSize / 2 + _offset;
            }
            else
            {
                newX = priorityRect.x + IconSize / 2;
                newY = priorityRect.y + IconSize / 2;
            }

            priorityRect.width = IconSize;
            priorityRect.height = IconSize;
            priorityRect.position = new Vector2(newX, newY);

            Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
            EditorGUI.DrawPreviewTexture(priorityRect, tex);
        }


        /// Draw priority for the "Text only" setting.
        private void DrawPriority_Text(Rect aRect, QQQ aQQQ)
        {
            var priorityRect = aRect;
            priorityRect.width -= _offset;
            priorityRect.height -= (IconSize / 2 + _offset);

            var newX = priorityRect.x + _offset * 2;
            var newY = priorityRect.y + _offset - 1;
            priorityRect.position = new Vector2(newX, newY);

            var priority = aQQQ.Priority.ToString();
            priority = priority == "MINOR" ? " MINOR" : priority;
            EditorGUI.LabelField(priorityRect, priority);
        }


        /// Draw priority for the "Icon and Text" setting.
        private void DrawPriority_IconAndText(Rect aRect, QQQ aQQQ)
        {
            // Draw the Icon.
            var iconRect = aRect;
            iconRect.width = IconSize;
            iconRect.height = IconSize;

            var iconNewX = iconRect.x + IconSize + _offset * 2;//iconRect.x + Mathf.Clamp(_unit, 1, IconSize + (int)(_priorityLabelWidth / 2));
            var iconNewY = iconRect.y + _offset;
            iconRect.position = new Vector2(iconNewX, iconNewY);

            Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
            EditorGUI.DrawPreviewTexture(iconRect, tex);

            // Draw the label.
            var labelRect = aRect;
            labelRect.width -= _offset;

            var labelNewX = labelRect.x + _offset * 2;
            var labelNewY = iconRect.y + iconRect.height;
            labelRect.position = new Vector2(labelNewX, labelNewY);

            var priority = aQQQ.Priority.ToString();
            priority = priority == "MINOR" ? " MINOR" : priority; // Label looks better with a space before.

            EditorGUI.LabelField(labelRect, priority);
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

        #endregion


        /// Draws the "Task" and "Script" texts for QQQs.
        private void DrawTaskAndScript(Rect aRect, QQQ aQQQ, float aTaskHeight, float aScriptHeight)
        {
            // Task.
            var taskRect = aRect;
            taskRect.x = _width_priority;
            taskRect.y += _offset;
            taskRect.height = aTaskHeight;
            EditorGUI.LabelField(taskRect, aQQQ.Task, _style_task);

            // Script.
            var scriptRect = aRect;
            scriptRect.x = _width_priority;
            scriptRect.y += (taskRect.height + 10);
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

            if (GUI.Button(editRect, editContent))
            {
                WindowEdit.Init(aQQQ);
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(editRect, DrawingUtils.Texture_Edit);
            }
            else
            {
                DrawingUtils.DrawTextButton(editRect, editContent.text, _style_buttonText);
            }

            if (GUI.Button(completeRect, completeContent))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs == true)
                {
                    var token = Preferences.TODOToken;
                    if (EditorUtility.DisplayDialog("Delete " + token, "Are you sure you want to delete this " + token + "?", "Delete " + token, "Cancel"))
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
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(completeRect, DrawingUtils.Texture_Delete);
            }
            else
            {
                DrawingUtils.DrawTextButton(completeRect, completeContent.text, _style_buttonText);
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
            anEditContent = new GUIContent("", "Edit this EditorPref");
        }
        private void Button_Complete_icon(Rect aRect, out Rect aCompleteRect, out GUIContent aCompleteContent)
        {
            aCompleteRect = aRect;
            aCompleteRect.y += IconSize + _offset + 8;
            aCompleteRect.width = IconSize;
            aCompleteRect.height = IconSize;

            aCompleteContent = new GUIContent("", "Complete this task");
        }
        #endregion


        #region A-R-S-N buttons
        /// Draw Add, Refresh, Settings and Nuke based on preferences.
        private void DrawBottomButtons()
        {
            Rect addRect, refreshRect, settingsRect, nukeRect;
            GUIContent addContent, refreshContent, settingsContent, nukeContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Add_default(out addRect, out addContent);
                    Button_Refresh_default(out refreshRect, out refreshContent);
                    Button_Settings_default(out settingsRect, out settingsContent);
                    Button_Nuke_default(out nukeRect, out nukeContent);
                    break;
				case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Add_icon(out addRect, out addContent);
                    Button_Refresh_icon(out refreshRect, out refreshContent);
                    Button_Settings_icon(out settingsRect, out settingsContent);
                    Button_Nuke_icon(out nukeRect, out nukeContent);
                    break;
            }

             // Add new QQQ.
            if (GUI.Button(addRect, addContent))
            {
                WindowAdd.Init();
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(addRect, DrawingUtils.Texture_Add);
            }
            else
            {
                DrawingUtils.DrawTextButton(addRect, addContent.text, _style_buttonText);
            }


            // Refresh list of QQQs.
            if (GUI.Button(refreshRect, refreshContent))
            {
                QQQOps.RefreshQQQs();
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(refreshRect, DrawingUtils.Texture_Refresh);
            }
            else
            {
                DrawingUtils.DrawTextButton(refreshRect, refreshContent.text, _style_buttonText);
            }

            // Open settings.
            if (GUI.Button(settingsRect, settingsContent))
            {
                CloseOtherWindows();
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }

            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(settingsRect, DrawingUtils.Texture_Settings);
            }
            else
            {
                DrawingUtils.DrawTextButton(settingsRect, settingsContent.text, _style_buttonText);
            }


            // Nuke QQQs.
            if (GUI.Button(nukeRect, nukeContent))
            {
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs == true)
                {
                    var token = Preferences.TODOToken;
                    if (EditorUtility.DisplayDialog("Remove ALL " + token + "s", "Are you sure ABSOLUTELY sure you want to remove ALL " + token + "s currently saved?\nThis is IRREVERSIBLE, only do this if you know what you're doing.", "Nuke " + token + "s", "Cancel"))
                    {
                        canExecute = true;
                    }
                }
                else
                {
                    canExecute = true;
                }

                if (canExecute == true)
                {
                    QQQOps.RemoveAllQQQs();
                }
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(nukeRect, DrawingUtils.Texture_Nuke);
            }
            else
            {
                DrawingUtils.DrawTextButton(nukeRect, nukeContent.text, _style_buttonText);
            }
        }


        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 2 - 6), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new QQQ");
        }
        private void Button_Refresh_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth - 2 ), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }
        private void Button_Settings_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + 2), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }
        private void Button_Nuke_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth + 6), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Nuke all", "Delete ALL prefs from EditorPrefs");
        }


        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 2 - 10), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Add a new QQQ");
        }
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize - 3), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Refresh list");
        }
        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + 3), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Open Settings");
        }
        private void Button_Nuke_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize + 10), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Delete ALL QQQs");
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

            _unit = (int)(width / 28) == 0 ? 1 : (int)(width / 28); // If the unit would be 0, set it to 1.

            // Priority rect width has different size based on preferences.
            if (Preferences.PriorityDisplay.ToString() == "ICON_ONLY")
            {
                _width_priority = Mathf.Clamp((_unit * 2) + IconSize, IconSize, (IconSize + _offset) * 2);
            }
            else if (Preferences.PriorityDisplay.ToString() == "BARS")
            {
                _width_priority = IconSize * 2;
            }
            else
            {
                _width_priority = _width_priorityLabel + _offset * 4;
            }

            // Same for buttons size
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

            _width_qqq = (int)width - _width_priority - _width_buttons - _offset * 3;
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
            _style_priority = _skin.GetStyle("GDTB_CodeTODOs_priority");
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

            /*
            style_bold = skin_custom.GetStyle("GDTB_CodeTODOs_key");
            style_bold.normal.textColor = Preferences.Color_Secondary;
            style_bold.active.textColor = Preferences.Color_Secondary;
            style_customGrid = skin_custom.GetStyle("GDTB_CodeTODOs_selectionGrid");
            */
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
    }
}