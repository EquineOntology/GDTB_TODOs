using UnityEngine;
using UnityEditor;

namespace GDTB.CodeTODOs
{
    public class Preferences
    {
        #region fields
        // TODO token (QQQ).
        public const string PREFS_CODETODOS_TOKEN = "GDTB_CodeTODOs_TODOToken";
        private static string _todoToken = "QQQ";
        private static string _todoToken_default = "QQQ";
        public static string TODOToken
        {
            get { return _todoToken; }
        }


        // Priority displayed as text, icon, text + icon.
        public const string PREFS_CODETODOS_PRIORITY_DISPLAY = "GDTB_CodeTODOs_PriorityDisplay";
        private static PriorityDisplayFormat _priorityDisplay = PriorityDisplayFormat.BARS;
        private static int _priorityDisplay_default = 3;
        public static PriorityDisplayFormat PriorityDisplay
        {
            get { return _priorityDisplay; }
        }
        private static string[] _displayFormatsString = { "Text only", "Icon only", "Icon and Text", "Bars" };


        // Buttons displayed as normal buttons or smaller icons.
        public const string PREFS_CODETODOS_BUTTONS_DISPLAY = "GDTB_CodeTODOs_ButtonDisplay";
        private static ButtonsDisplayFormat _buttonsDisplay = ButtonsDisplayFormat.COOL_ICONS;
        private static int _buttonsDisplay_default = 1;
        public static ButtonsDisplayFormat ButtonsDisplay
        {
            get { return _buttonsDisplay; }
        }
        private static string[] _buttonsFormatsString = { "Cool icons", "Regular buttons" };


        // Auto-update QQQs.
        public const string PREFS_CODETODOS_AUTO_REFRESH = "GDTB_CodeTODOs_AutoUpdate";
        private static bool _autoRefresh = true;
        private static bool _autoRefresh_default = true;
        private static bool _oldAutoRefresh;
        public static bool AutoRefresh
        {
            get { return _autoRefresh; }
        }


        // Color of URGENT tasks.
        public const string PREFS_CODETODOS_COLOR_PRI1 = "GDTB_CodeTODOs_Color1";
        private static Color _priColor1 = new Color(246, 71, 71, 1);  // Sunset orange http://www.flatuicolorpicker.com/pink
        private static Color _priColor1_default = new Color(246, 71, 71, 1);
        public static Color PriColor1
        {
            get { return _priColor1; }
        }

        // Color of NORMAL tasks
        public const string PREFS_CODETODOS_COLOR_PRI2 = "GDTB_CodeTODOs_Color2";
        private static Color _priColor2 = new Color(244, 208, 63, 1); // Saffron http://www.flatuicolorpicker.com/yellow
        private static Color _priColor2_default = new Color(244, 208, 63, 1);
        public static Color PriColor2
        {
            get { return _priColor2; }
        }

        // Color of MINOR tasks
        public const string PREFS_CODETODOS_COLOR_PRI3 = "GDTB_CodeTODOs_Color3";
        private static Color _priColor3 = new Color(46, 204, 113, 1); // Shamrock http://www.flatuicolorpicker.com/green
        private static Color _priColor3_default = new Color(46, 204, 113, 1);
        public static Color PriColor3
        {
            get { return _priColor3; }
        }

        // Color of bar borders
        public const string PREFS_CODETODOS_COLOR_BORDER = "GDTB_CodeTODOs_Border";
        private static Color _borderColor = Color.gray;
        private static Color _borderColor_default = Color.gray;
        public static Color BorderColor
        {
            get { return _borderColor; }
        }

        // Custom shortcut
        public const string PREFS_CODETODOS_SHORTCUT = "GDTB_CodeTODOs_Shortcut";
        private static string _shortcut = "%|q";
        private static string _newShortcut;
        private static string _shortcut_default = "%|q";
        public static string Shortcut
        {
            get { return _shortcut; }
        }
        private static bool[] _modifierKeys = new bool[] { false, false, false }; // Ctrl/Cmd, Alt, Shift.
        private static int _mainShortcutKeyIndex = 0;
        // Want absolute control over values.
        private static string[] _shortcutKeys = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "LEFT", "RIGHT", "UP", "DOWN", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "HOME", "END", "PGUP", "PGDN" };
        #endregion fields


        [PreferenceItem("Code TODOs")]
        public static void PreferencesGUI()
        {
            GetAllPrefValues();
            EditorGUILayout.BeginVertical();
            _todoToken = EditorGUILayout.TextField("TODO token", _todoToken);
            _priorityDisplay = (PriorityDisplayFormat)EditorGUILayout.Popup("Priority style", System.Convert.ToInt16(_priorityDisplay), _displayFormatsString);
            _buttonsDisplay = (ButtonsDisplayFormat)EditorGUILayout.Popup("Button style", System.Convert.ToInt16(_buttonsDisplay), _buttonsFormatsString);
            _autoRefresh = EditorGUILayout.Toggle("Auto refresh", _autoRefresh);
            EditorGUILayout.Separator();
            _priColor1 = EditorGUILayout.ColorField("Urgent priority", _priColor1);
            _priColor2 = EditorGUILayout.ColorField("Normal priority", _priColor2);
            _priColor3 = EditorGUILayout.ColorField("Minor priority", _priColor3);
            _borderColor = EditorGUILayout.ColorField("Borders", _borderColor);
            EditorGUILayout.Separator();
            _newShortcut = DrawShortcutSelector();
            GUILayout.Space(20);
            DrawResetButton();
            EditorGUILayout.EndVertical();
            if (GUI.changed)
            {
                // Save QQQs when switching off autorefresh.
                if (_autoRefresh != _oldAutoRefresh)
                {
                    _oldAutoRefresh = _autoRefresh;
                    IO.WriteQQQsToFile();
                }

                SetPrefValues();
                GetAllPrefValues();
                RepaintOpenWindows();
            }
        }


        /// Set the value of all preferences.
        private static void SetPrefValues()
        {
            EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, _todoToken);
            EditorPrefs.SetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, System.Convert.ToInt16(_priorityDisplay));
            EditorPrefs.SetInt(PREFS_CODETODOS_BUTTONS_DISPLAY, System.Convert.ToInt16(_buttonsDisplay));
            EditorPrefs.SetBool(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh);

            SetColorPrefs();
            SetShortcutPrefs();
        }


        /// Set the value of a Color preference.
        private static void SetColorPrefs()
        {
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI1, RGBA.ColorToString(_priColor1));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI2, RGBA.ColorToString(_priColor2));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI3, RGBA.ColorToString(_priColor3));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_BORDER, RGBA.ColorToString(_borderColor));
        }


        /// Set the value of the shortcut preference.
        private static void SetShortcutPrefs()
        {
            if (_newShortcut != _shortcut)
            {
                _shortcut = _newShortcut;
                EditorPrefs.SetString(PREFS_CODETODOS_SHORTCUT, _shortcut);
                var formattedShortcut = _shortcut.Replace("|", "");
                IO.OverwriteShortcut(formattedShortcut);
            }
        }


        /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
        public static void GetAllPrefValues()
        {
            _todoToken = GetPrefValue(PREFS_CODETODOS_TOKEN, _todoToken_default); // TODO token.
            _priorityDisplay = (PriorityDisplayFormat)EditorPrefs.GetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, _priorityDisplay_default); // QQQ Priority display.
            _buttonsDisplay = (ButtonsDisplayFormat)EditorPrefs.GetInt(PREFS_CODETODOS_BUTTONS_DISPLAY, _buttonsDisplay_default); // Buttons display.
            _autoRefresh = GetPrefValue(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh_default); // Auto refresh.
            _oldAutoRefresh = _autoRefresh;
            _priColor1 = GetPrefValue(PREFS_CODETODOS_COLOR_PRI1, _priColor1_default); // URGENT priority color.
            _priColor2 = GetPrefValue(PREFS_CODETODOS_COLOR_PRI2, _priColor2_default); // NORMAL priority color.
            _priColor3 = GetPrefValue(PREFS_CODETODOS_COLOR_PRI3, _priColor3_default); // MINOR priority color.
            _borderColor = GetPrefValue(PREFS_CODETODOS_COLOR_BORDER, _borderColor_default); // Priority bar border color.
            _shortcut = GetPrefValue(PREFS_CODETODOS_SHORTCUT, _shortcut_default); // Shortcut.
            ParseShortcutValues();
        }


        /// Get the value of a bool preference.
        private static bool GetPrefValue(string aKey, bool aDefault)
        {
            bool val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetBool(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetBool(aKey, aDefault);
            }

            return val;
        }


        /// Get the value of a string preference.
        private static string GetPrefValue(string aKey, string aDefault)
        {
            string val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetString(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetString(aKey, aDefault);
            }

            return val;
        }


        /// Get the value of a Color preference.
        private static Color GetPrefValue(string aKey, Color aDefault)
        {
            Color val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetString(aKey, RGBA.ColorToString(aDefault));
                val = aDefault;
            }
            else
            {
                val = RGBA.StringToColor(EditorPrefs.GetString(aKey, RGBA.ColorToString(aDefault)));
            }

            return val;
        }


        /// Draw the shortcut selector.
        private static string DrawShortcutSelector()
        {
            // Differentiate between Mac Editor (CMD) and Win editor (CTRL).
            var platformKey = Application.platform == RuntimePlatform.OSXEditor ? "CMD" : "CTRL";
            var shortcut = "";
            ParseShortcutValues();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Shortcut ");
            GUILayout.Space(20);
            _modifierKeys[0] = GUILayout.Toggle(_modifierKeys[0], platformKey, EditorStyles.miniButton, GUILayout.Width(50));
            _modifierKeys[1] = GUILayout.Toggle(_modifierKeys[1], "ALT", EditorStyles.miniButton, GUILayout.Width(40));
            _modifierKeys[2] = GUILayout.Toggle(_modifierKeys[2], "SHIFT", EditorStyles.miniButton, GUILayout.Width(60));
            _mainShortcutKeyIndex = EditorGUILayout.Popup(_mainShortcutKeyIndex, _shortcutKeys, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            // Generate shortcut string.
            if (_modifierKeys[0] == true)
            {
                shortcut += "%|";
            }
            if (_modifierKeys[1] == true)
            {
                shortcut += "&|";
            }
            if (_modifierKeys[2] == true)
            {
                shortcut += "#|";
            }
            shortcut += _shortcutKeys[_mainShortcutKeyIndex];

            return shortcut;
        }


        /// Get usable values from the shortcut string pref.
        private static void ParseShortcutValues()
        {
            var foundCmd = false;
            var foundAlt = false;
            var foundShift = false;

            var keys = _shortcut.Split('|');
            for (var i = 0; i < keys.Length; i++)
            {
                switch (keys[i])
                {
                    case "%":
                        foundCmd = true;
                        break;
                    case "&":
                        foundAlt = true;
                        break;
                    case "#":
                        foundShift = true;
                        break;
                    default:
                        _mainShortcutKeyIndex = System.Array.IndexOf(_shortcutKeys, keys[i]);
                        break;
                }
            }
            _modifierKeys[0] = foundCmd; // Ctrl/Cmd.
            _modifierKeys[1] = foundAlt; // Alt.
            _modifierKeys[2] = foundShift; // Shift.
        }


        /// Draw reset button.
        private static void DrawResetButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset preferences", GUILayout.Width(120)))
            {
                ResetPrefsToDefault();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Reset all preferences to default.
        private static void ResetPrefsToDefault()
        {
            _todoToken = _todoToken_default;
            _priorityDisplay = (PriorityDisplayFormat)_priorityDisplay_default;
            _buttonsDisplay = (ButtonsDisplayFormat)_buttonsDisplay_default;
            _autoRefresh = _autoRefresh_default;
            _priColor1 = new Color(_priColor1_default.r / 255, _priColor1_default.g / 255, _priColor1_default.b / 255, _priColor1_default.a);
            _priColor2 = new Color(_priColor2_default.r / 255, _priColor2_default.g / 255, _priColor2_default.b / 255, _priColor2_default.a);
            _priColor3 = new Color(_priColor3_default.r / 255, _priColor3_default.g / 255, _priColor3_default.b / 255, _priColor3_default.a);
            _borderColor = _borderColor_default;
            _shortcut = _shortcut_default;

            SetPrefValues();
            GetAllPrefValues();
        }


        /// Repaint all open CodeTODOs windows.
        private static void RepaintOpenWindows()
        {
            if (WindowMain.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
            }
            if (WindowAdd.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowAdd)).Repaint();
            }
            if (WindowEdit.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowEdit)).Repaint();
            }
        }
    }


    public enum PriorityDisplayFormat
    {
        TEXT_ONLY,
        ICON_ONLY,
        ICON_AND_TEXT,
        BARS
    }


    public enum ButtonsDisplayFormat
    {
        COOL_ICONS,
        REGULAR_BUTTONS
    }
}