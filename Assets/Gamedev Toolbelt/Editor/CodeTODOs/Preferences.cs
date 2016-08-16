using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.codetodos
{
    public class Preferences
    {
        #region fields
        // TODO token (QQQ).
        private const string PREFS_CODETODOS_TOKEN = "GDTB_CodeTODOs_TODOToken";
        private static string _todoToken = "QQQ";
        private static string _todoToken_default = "QQQ";
        public static string TODOToken
        {
            get { return _todoToken; }
        }


        // Buttons displayed as normal buttons or smaller icons.
        private const string PREFS_CODETODOS_BUTTONS_DISPLAY = "GDTB_CodeTODOs_ButtonDisplay";
        private static ButtonsDisplayFormat _buttonsDisplay = ButtonsDisplayFormat.COOL_ICONS;
        private static int _buttonsDisplay_default = 1;
        private static ButtonsDisplayFormat _oldDisplayFormat;
        public static ButtonsDisplayFormat ButtonsDisplay
        {
            get { return _buttonsDisplay; }
        }
        private static string[] _buttonsFormatsString = { "Cool icons", "Regular buttons" };


        // Confirmation dialogs
        private const string PREFS_CODETODOS_CONFIRMATION_DIALOGS = "GDTB_CodeTODOs_ConfirmationDialogs";
        private static bool _confirmationDialogs = true;
        private static bool _confirmationDialogs_default = true;
        public static bool ShowConfirmationDialogs
        {
            get { return _confirmationDialogs; }
        }

        #region Colors
        // Style of icons (light or dark).
        private const string PREFS_CODETODOS_ICON_STYLE = "GDTB_CodeTODOs_IconStyle";
        private static IconStyle _iconStyle = IconStyle.LIGHT;
        private static int _iconStyle_default = 1;
        private static IconStyle _oldIconStyle;
        public static IconStyle IconStyle
        {
            get { return _iconStyle; }
        }
        private static string[] arr_iconStyle = { "Dark", "Light" };

        // Primary color.
        private const string PREFS_CODETODOS_COLOR_PRIMARY = "GDTB_CodeTODOs_Primary";
        private static Color _primary = new Color(56, 56, 56, 1);
        private static Color _primary_dark = new Color(56, 56, 56, 1);
        private static Color _primary_light = new Color(233, 233, 233, 1);
        private static Color _primary_default = new Color(56, 56, 56, 1);
        public static Color Color_Primary
        {
            get { return _primary; }
        }

        // Secondary color.
        private const string PREFS_CODETODOS_COLOR_SECONDARY = "GDTB_CodeTODOs_Secondary";
        private static Color _secondary = new Color(255, 90, 90, 1);
        private static Color _secondary_dark = new Color(255, 90, 90, 1);
        private static Color _secondary_light = new Color(165, 0, 0, 1);
        private static Color _secondary_default = new Color(255, 90, 90, 1);
        public static Color Color_Secondary
        {
            get { return _secondary; }
        }

        // Tertiary color.
        private const string PREFS_CODETODOS_COLOR_TERTIARY = "GDTB_CodeTODOs_Tertiary";
        private static Color _tertiary = new Color(255, 248, 248, 1);
        private static Color _tertiary_dark = new Color(255, 248, 248, 1);
        private static Color _tertiary_light = new Color(56, 56, 56, 1);
        private static Color _tertiary_default = new Color(255, 248, 248, 1);
        public static Color Color_Tertiary
        {
            get { return _tertiary; }
        }

        // Quaretnary color.
        private const string PREFS_CODETODOS_COLOR_QUATERNARY = "GDTB_CodeTODOs_Quaternary";
        private static Color _quaternary = new Color(70, 70, 70, 1);
        private static Color _quaternary_dark = new Color(70, 70, 70, 1);
        private static Color _quaternary_light = new Color(220, 220, 220, 1);
        private static Color _quaternary_default = new Color(70, 70, 70, 1);
        public static Color Color_Quaternary
        {
            get { return _quaternary; }
        }

        // Color of URGENT tasks.
        private const string PREFS_CODETODOS_COLOR_PRI1 = "GDTB_CodeTODOs_Urgent";
        private static Color _priColor1 = new Color(246, 71, 71, 1);  // Sunset orange http://www.flatuicolorpicker.com/pink
        private static Color _priColor1_default = new Color(246, 71, 71, 1);
        private static Color _priColor1_dark = new Color(246, 71, 71, 1);
        private static Color _priColor1_light = new Color(246, 71, 71, 1);
        public static Color PriColor1
        {
            get { return _priColor1; }
        }

        // Color of NORMAL tasks
        private const string PREFS_CODETODOS_COLOR_PRI2 = "GDTB_CodeTODOs_Normal";
        private static Color _priColor2 = new Color(244, 208, 63, 1); // Saffron http://www.flatuicolorpicker.com/yellow
        private static Color _priColor2_default = new Color(244, 208, 63, 1);
        private static Color _priColor2_dark = new Color(244, 208, 63, 1);
        private static Color _priColor2_light = new Color(244, 208, 63, 1);
        public static Color PriColor2
        {
            get { return _priColor2; }
        }

        // Color of MINOR tasks
        private const string PREFS_CODETODOS_COLOR_PRI3 = "GDTB_CodeTODOs_Minor";
        private static Color _priColor3 = new Color(46, 204, 113, 1); // Shamrock http://www.flatuicolorpicker.com/green
        private static Color _priColor3_default = new Color(46, 204, 113, 1);
        private static Color _priColor3_dark = new Color(46, 204, 113, 1);
        private static Color _priColor3_light = new Color(46, 204, 113, 1);
        public static Color PriColor3
        {
            get { return _priColor3; }
        }

        #endregion


        // Auto-update QQQs.
        private const string PREFS_CODETODOS_AUTO_REFRESH = "GDTB_CodeTODOs_AutoUpdate";
        private static bool _autoRefresh = true;
        private static bool _autoRefresh_default = true;
        private static bool _oldAutoRefresh;
        public static bool AutoRefresh
        {
            get { return _autoRefresh; }
        }


        // Custom shortcut
        private const string PREFS_CODETODOS_SHORTCUT = "GDTB_CodeTODOs_Shortcut";
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


        private static Vector2 _scrollPosition = new Vector2(-1, 0);
        [PreferenceItem("Code TODOs")]
        public static void PreferencesGUI()
        {
            GetAllPrefValues();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            _todoToken = EditorGUILayout.TextField("TODO token", _todoToken);
            _autoRefresh = EditorGUILayout.Toggle("Auto refresh", _autoRefresh);
            _confirmationDialogs = EditorGUILayout.Toggle("Show confirmation dialogs", _confirmationDialogs);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
            _buttonsDisplay = (ButtonsDisplayFormat)EditorGUILayout.Popup("Button style", System.Convert.ToInt16(_buttonsDisplay), _buttonsFormatsString);
            _iconStyle = (IconStyle)EditorGUILayout.Popup("Icon style", (int)_iconStyle, arr_iconStyle);
            _priColor1 = EditorGUILayout.ColorField("Urgent priority", _priColor1);
            _priColor2 = EditorGUILayout.ColorField("Normal priority", _priColor2);
            _priColor3 = EditorGUILayout.ColorField("Minor priority", _priColor3);
            EditorGUILayout.Separator();
            _primary = EditorGUILayout.ColorField("Background and button color", _primary);
            _secondary = EditorGUILayout.ColorField("Accent color", _secondary);
            _tertiary = EditorGUILayout.ColorField("Text color", _tertiary);
            _quaternary = EditorGUILayout.ColorField("Element background color", _quaternary);
            EditorGUILayout.Separator();
            DrawThemeButtons();
            GUILayout.Space(20);
            _newShortcut = DrawShortcutSelector();
            GUILayout.Space(20);
            DrawResetButton();
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                // If buttons display changed we want to open and close the window, so that the new minsize is applied.
                var shouldReopenWindowMain = false;
                if (_buttonsDisplay != _oldDisplayFormat || _iconStyle != _oldIconStyle)
                {
                    _oldDisplayFormat = _buttonsDisplay;
                    _oldIconStyle = _iconStyle;
                    shouldReopenWindowMain = true;
                }

                // Save QQQs when switching off autorefresh.
                if (_autoRefresh != _oldAutoRefresh)
                {
                    _oldAutoRefresh = _autoRefresh;
                    IO.WriteQQQsToFile();
                }

                SetPrefValues();

                if (shouldReopenWindowMain)
                {
                    if (WindowMain.IsOpen)
                    {
                        EditorWindow.GetWindow(typeof(WindowMain)).Close();
                        var window = EditorWindow.GetWindow(typeof(WindowMain)) as WindowMain;
                        window.SetMinSize();
                        window.Show();
                    }
                }
            }
        }


        /// Set the value of all preferences.
        private static void SetPrefValues()
        {
            EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, _todoToken);
            EditorPrefs.SetInt(PREFS_CODETODOS_BUTTONS_DISPLAY, System.Convert.ToInt16(_buttonsDisplay));
            EditorPrefs.SetBool(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh);
            EditorPrefs.SetBool(PREFS_CODETODOS_CONFIRMATION_DIALOGS, _confirmationDialogs);
            SetIconStyle();
            SetColorPrefs();
            SetShortcutPrefs();
        }


        /// Set the value of IconStyle.
        private static void SetIconStyle()
        {
            EditorPrefs.SetInt(PREFS_CODETODOS_ICON_STYLE, (int)_iconStyle);
            DrawingUtils.LoadTextures(_iconStyle);
        }


        /// Set the value of a Color preference.
        private static void SetColorPrefs()
        {
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRIMARY, RGBA.ColorToString(_primary));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_SECONDARY, RGBA.ColorToString(_secondary));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_TERTIARY, RGBA.ColorToString(_tertiary));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_QUATERNARY, RGBA.ColorToString(_quaternary));

            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI1, RGBA.ColorToString(_priColor1));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI2, RGBA.ColorToString(_priColor2));
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI3, RGBA.ColorToString(_priColor3));
        }


        /// Draw Apply colors - Load dark theme - load light theme.
        private static void DrawThemeButtons()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply new colors"))
            {
                ReloadSkins();
                RepaintOpenWindows();
            }
            if (GUILayout.Button("Load dark theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Change to dark theme?", "Are you sure you want to change the color scheme to the dark (default) theme?", "Change color scheme", "Cancel"))
                    {
                        canExecute = true;
                    }
                }
                else
                {
                    canExecute = true;
                }

                // Do it if we have permission.
                if (canExecute == true)
                {
                    _primary = new Color(_primary_dark.r / 255.0f, _primary_dark.g / 255.0f, _primary_dark.b / 255.0f, 1.0f);
                    _secondary = new Color(_secondary_dark.r / 255.0f, _secondary_dark.g / 255.0f, _secondary_dark.b / 255.0f, 1.0f);
                    _tertiary = new Color(_tertiary_dark.r / 255.0f, _tertiary_dark.g / 255.0f, _tertiary_dark.b / 255.0f, 1.0f);
                    _quaternary = new Color(_quaternary_dark.r / 255.0f, _quaternary_dark.g / 255.0f, _quaternary_dark.b / 255.0f, 1.0f);
                    _priColor1 = new Color(_priColor1_dark.r / 255.0f, _priColor1_dark.g / 255.0f, _priColor1_dark.b / 255.0f, 1.0f);
                    _priColor2 = new Color(_priColor2_dark.r / 255.0f, _priColor2_dark.g / 255.0f, _priColor2_dark.b / 255.0f, 1.0f);
                    _priColor3 = new Color(_priColor3_dark.r / 255.0f, _priColor3_dark.g / 255.0f, _priColor3_dark.b / 255.0f, 1.0f);
                    SetColorPrefs();
                    GetColorPrefs();

                    _iconStyle = IconStyle.LIGHT;
                    SetIconStyle();
                    GetIconStyle();
                    ReloadSkins();

                    RepaintOpenWindows();
                }
            }
            if (GUILayout.Button("Load light theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Change to light theme?", "Are you sure you want to change the color scheme to the light theme?", "Change color scheme", "Cancel"))
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
                    _primary = new Color(_primary_light.r / 255.0f, _primary_light.g / 255.0f, _primary_light.b / 255.0f, 1.0f);
                    _secondary = new Color(_secondary_light.r / 255.0f, _secondary_light.g / 255.0f, _secondary_light.b / 255.0f, 1.0f);
                    _tertiary = new Color(_tertiary_light.r / 255.0f, _tertiary_light.g / 255.0f, _tertiary_light.b / 255.0f, 1.0f);
                    _quaternary = new Color(_quaternary_light.r / 255.0f, _quaternary_light.g / 255.0f, _quaternary_light.b / 255.0f, 1.0f);
                    _priColor1 = new Color(_priColor1_light.r / 255.0f, _priColor1_light.g / 255.0f, _priColor1_light.b / 255.0f, 1.0f);
                    _priColor2 = new Color(_priColor2_light.r / 255.0f, _priColor2_light.g / 255.0f, _priColor2_light.b / 255.0f, 1.0f);
                    _priColor3 = new Color(_priColor3_light.r / 255.0f, _priColor3_light.g / 255.0f, _priColor3_light.b / 255.0f, 1.0f);
                    SetColorPrefs();
                    GetColorPrefs();

                    _iconStyle = IconStyle.DARK;
                    SetIconStyle();
                    GetIconStyle();
                    ReloadSkins();

                    RepaintOpenWindows();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
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
            _buttonsDisplay = (ButtonsDisplayFormat)EditorPrefs.GetInt(PREFS_CODETODOS_BUTTONS_DISPLAY, _buttonsDisplay_default); // Buttons display.
            _oldDisplayFormat = _buttonsDisplay;
            GetIconStyle();
            _autoRefresh = GetPrefValue(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh_default); // Auto refresh.
            _oldAutoRefresh = _autoRefresh;
            _confirmationDialogs = GetPrefValue(PREFS_CODETODOS_CONFIRMATION_DIALOGS, _confirmationDialogs_default);
            GetColorPrefs();
            _shortcut = GetPrefValue(PREFS_CODETODOS_SHORTCUT, _shortcut_default); // Shortcut.
            ParseShortcutValues();
        }


        /// Get IconStyle.
        private static void GetIconStyle()
        {
            _iconStyle = (IconStyle)EditorPrefs.GetInt(PREFS_CODETODOS_ICON_STYLE, _iconStyle_default); // Icon style.
            _oldIconStyle = _iconStyle;
            DrawingUtils.LoadTextures(_iconStyle);
        }


        /// Load color preferences.
        private static void GetColorPrefs()
        {
            _primary = GetPrefValue(PREFS_CODETODOS_COLOR_PRIMARY, _primary_default); // PRIMARY color.
            _secondary = GetPrefValue(PREFS_CODETODOS_COLOR_SECONDARY, _secondary_default); // SECONDARY color.
            _tertiary = GetPrefValue(PREFS_CODETODOS_COLOR_TERTIARY, _tertiary_default); // TERTIARY color.
            _quaternary = GetPrefValue(PREFS_CODETODOS_COLOR_QUATERNARY, _quaternary_default); // QUATERNARY color.

            _priColor1 = GetPrefValue(PREFS_CODETODOS_COLOR_PRI1, _priColor1_default); // URGENT priority color.
            _priColor2 = GetPrefValue(PREFS_CODETODOS_COLOR_PRI2, _priColor2_default); // NORMAL priority color.
            _priColor3 = GetPrefValue(PREFS_CODETODOS_COLOR_PRI3, _priColor3_default); // MINOR priority color.
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
            _buttonsDisplay = (ButtonsDisplayFormat)_buttonsDisplay_default;
            _autoRefresh = _autoRefresh_default;
            _primary = new Color(_primary_default.r / 255, _primary_default.g / 255, _primary_default.b / 255, _primary_default.a);
            _secondary = new Color(_secondary_default.r / 255, _secondary_default.g / 255, _secondary_default.b / 255, _secondary_default.a);
            _tertiary = new Color(_tertiary_default.r / 255, _tertiary_default.g / 255, _tertiary_default.b / 255, _tertiary_default.a);
            _quaternary = new Color(_quaternary_default.r / 255, _quaternary_default.g / 255, _quaternary_default.b / 255, _quaternary_default.a);
            _priColor1 = new Color(_priColor1_default.r / 255, _priColor1_default.g / 255, _priColor1_default.b / 255, _priColor1_default.a);
            _priColor2 = new Color(_priColor2_default.r / 255, _priColor2_default.g / 255, _priColor2_default.b / 255, _priColor2_default.a);
            _priColor3 = new Color(_priColor3_default.r / 255, _priColor3_default.g / 255, _priColor3_default.b / 255, _priColor3_default.a);
            _shortcut = _shortcut_default;

            SetPrefValues();
            GetAllPrefValues();
        }


        /// Reload skins of open windows.
        private static void ReloadSkins()
        {
            if (WindowMain.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowMain)) as WindowMain;
                window.LoadStyles();
            }
            if (WindowAdd.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowAdd)) as WindowMain;
                window.LoadStyles();
            }
            if (WindowEdit.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowEdit)) as WindowMain;
                window.LoadStyles();
            }
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


    public enum ButtonsDisplayFormat
    {
        COOL_ICONS,
        REGULAR_BUTTONS
    }


    public enum IconStyle
    {
        DARK,
        LIGHT
    }
}