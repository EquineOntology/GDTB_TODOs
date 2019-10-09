using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.todos
{
    public class Preferences
    {
#region FIELDS AND PROPERTIES
        // TODO token (QQQ).
        private const string _PREFS_TODOS_TOKEN = "GDTB_TODOs_TODOToken";
        private static string _todoToken = "QQQ";
        private const string _TODO_TOKEN_DEFAULT = "QQQ";
        public static string TODOToken
        {
            get { return _todoToken; }
            set { _todoToken = value; }
        }


        // Confirmation dialogs.
        private const string _PREFS_TODOS_CONFIRMATION_DIALOGS = "GDTB_TODOs_ConfirmationDialogs";
        private static bool _confirmationDialogs = true;
        private const bool _CONFIRMATION_DIALOGS_DEFAULT = true;
        public static bool ShowConfirmationDialogs
        {
            get { return _confirmationDialogs; }
            set { _confirmationDialogs = value; }

        }


        // Welcome window.
        private const string _PREFS_TODOS_WELCOME = "GDTB_TODOs_Welcome";
        private static bool _showWelcome = true;
        private const bool _SHOW_WELCOME_DEFAULT = true;
        public static bool ShowWelcome
        {
            get { return _showWelcome; }
            set { _showWelcome = value; }
        }

        #region Colors
        // Primary color.
        private const string _PREFS_TODOS_COLOR_PRIMARY = "GDTB_TODOs_Primary";
        private static Color _primary = new Color(56,56,56,1);
        private static Color _primaryDark = new Color(56, 56, 56, 1);
        private static Color _primaryLight = new Color(255, 255, 255, 1);
        private static readonly Color PrimaryDefault = new Color(56, 56, 56, 1);

        public static Color Primary
        {
            get { return _primary; }
            set { _primary = value; }
        }

        // Secondary color.
        private const string _PREFS_TODOS_COLOR_SECONDARY = "GDTB_TODOs_Secondary";
        private static Color _secondary = new Color(255, 90, 90, 1);
        private static Color _secondaryDark = new Color(255, 90, 90, 1);
        private static Color _secondaryLight = new Color(165, 0, 0, 1);
        private static readonly Color SecondaryDefault = new Color(255, 90, 90, 1);

        public static Color Secondary
        {
            get { return _secondary; }
            set { _secondary = value; }
        }

        // Tertiary color.
        private const string _PREFS_TODOS_COLOR_TERTIARY = "GDTB_TODOs_Tertiary";
        private static Color _tertiary = new Color(255, 248, 248, 1);
        private static Color _tertiaryDark = new Color(255, 248, 248, 1);
        private static Color _tertiaryLight = new Color(56, 56, 56, 1);
        private static readonly Color TertiaryDefault = new Color(255, 248, 248, 1);

        public static Color Tertiary
        {
            get { return _tertiary; }
            set { _tertiary = value; }
        }

        // Quaternary color.
        private const string _PREFS_TODOS_COLOR_QUATERNARY = "GDTB_TODOs_Quaternary";
        private static Color _quaternary = new Color(70, 70, 70, 1);
        private static Color _quaternaryDark = new Color(70, 70, 70, 1);
        private static Color _quaternaryLight = new Color(242, 242, 242, 1);
        private static readonly Color QuaternaryDefault = new Color(70, 70, 70, 1);
        public static Color Quaternary
        {
            get { return _quaternary; }
            set { _quaternary = value; }
        }

        // Color of URGENT tasks.
        private const string _PREFS_TODOS_COLOR_PRI1 = "GDTB_TODOs_Urgent";
        private static Color _priorityUrgent = new Color(246, 71, 71, 1);
        private static readonly Color PriorityUrgentDefault = new Color(246, 71, 71, 1);
        private static Color _priorityUrgentDark = new Color(246, 71, 71, 1);
        private static Color _priorityUrgentLight = new Color(197, 0, 0, 1);
        public static Color PriorityUrgent
        {
            get { return _priorityUrgent; }
            set { _priorityUrgent = value; }
        }

        // Color of NORMAL tasks
        private const string _PREFS_TODOS_COLOR_PRI2 = "GDTB_TODOs_Normal";
        private static Color _priorityNormal = new Color(244, 208, 63, 1);
        private static readonly Color PriorityNormalDefault = new Color(244, 208, 63, 1);
        private static Color _priorityNormalDark = new Color(244, 208, 63, 1);
        private static Color _priorityNormalLight = new Color(234, 188, 0, 1);
        public static Color PriorityNormal
        {
            get { return _priorityNormal; }
            set { _priorityNormal = value; }
        }

        // Color of MINOR tasks
        private const string _PREFS_TODOS_COLOR_PRI3 = "GDTB_TODOs_Minor";
        private static Color _priorityMinor = new Color(46, 204, 113, 1);
        private static readonly Color PriorityMinorDefault = new Color(46, 204, 113, 1);
        private static Color _priorityMinorDark = new Color(46, 204, 113, 1);
        private static Color _priorityMinorLight = new Color(0, 189, 80, 1);
        public static Color PriorityMinor
        {
            get { return _priorityMinor; }
            set { _priorityMinor = value; }
        }

        #endregion

        // Custom shortcut
        private const string _PREFS_TODOS_SHORTCUT = "GDTB_TODOs_Shortcut";
        private static string _shortcut = "%|t";
        private static string _newShortcut;
        private const string _SHORTCUT_DEFAULT = "%|t";
        public static string Shortcut
        {
            get { return _shortcut; }
            set { _shortcut = value; }
        }

        private static readonly bool[] ModifierKeys = { false, false, false }; // Ctrl/Cmd, Alt, Shift.
        private static int _mainShortcutKeyIndex;
        // Want absolute control over values.
        private static readonly string[] ShortcutKeys = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "LEFT", "RIGHT", "UP", "DOWN", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "HOME", "END", "PGUP", "PGDN" };


        // Thick borders
        private const string _PREFS_TODOS_BORDER_THICKNESS = "GDTB_TODOs_BorderThickness";
        private static int _borderThickness;
        private const int _BORDER_THICKNESS_DEFAULT = 1;
        public static int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; }
        }

        private static Vector2 _scrollPosition = new Vector2(-1, 0);

#endregion

#region METHODS

        [PreferenceItem("TODOs")]
        public static void PreferencesGUI()
        {
            GetAllPrefValues();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            TODOToken = EditorGUILayout.TextField("TODO token", TODOToken);
            ShowConfirmationDialogs = EditorGUILayout.Toggle("Show confirmation dialogs", ShowConfirmationDialogs);
            ShowWelcome = EditorGUILayout.Toggle("Show Welcome window", ShowWelcome);
            BorderThickness = EditorGUILayout.IntSlider("Border thickness", BorderThickness, 1, 5);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
            PriorityUrgent = EditorGUILayout.ColorField("Urgent priority", PriorityUrgent);
            PriorityNormal = EditorGUILayout.ColorField("Normal priority", PriorityNormal);
            PriorityMinor = EditorGUILayout.ColorField("Minor priority", PriorityMinor);
            EditorGUILayout.Separator();
            Primary = EditorGUILayout.ColorField("Background and button color", Primary);
            Secondary = EditorGUILayout.ColorField("Accent color", Secondary);
            Tertiary = EditorGUILayout.ColorField("Text color", Tertiary);
            Quaternary = EditorGUILayout.ColorField("Element background color", Quaternary);
            EditorGUILayout.Separator();
            DrawThemeButtons();
            GUILayout.Space(20);
            _newShortcut = DrawShortcutSelector();
            GUILayout.Space(20);
            DrawResetButtons();
            GUILayout.Space(30);
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                SetPrefValues();
            }
        }


        /// If EditorPrefs have been lost or have never been initialized, we want to set them to their default values.
        public static void InitExtension()
        {
            ResetPrefsToDefault();
            QQQOps.FindAllScripts();
            EditorPrefs.SetBool("GDTB_TODOs_firsttime", true);
        }


        /// Set the value of ShowWelcome.
        public static void SetWelcome(bool val)
        {
            EditorPrefs.SetBool(_PREFS_TODOS_WELCOME, val);
        }


        /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
        public static void GetAllPrefValues()
        {
            TODOToken = GetPrefValue(_PREFS_TODOS_TOKEN, _TODO_TOKEN_DEFAULT); // TODO token.
            ShowConfirmationDialogs = GetPrefValue(_PREFS_TODOS_CONFIRMATION_DIALOGS, _CONFIRMATION_DIALOGS_DEFAULT);
            ShowWelcome = GetPrefValue(_PREFS_TODOS_WELCOME, _SHOW_WELCOME_DEFAULT);
            BorderThickness = GetPrefValue(_PREFS_TODOS_BORDER_THICKNESS, _BORDER_THICKNESS_DEFAULT);
            GetColorPrefs();
            Shortcut = GetPrefValue(_PREFS_TODOS_SHORTCUT, _SHORTCUT_DEFAULT); // Shortcut.
            ParseShortcutValues();
        }




        /// Set the value of all preferences.
        private static void SetPrefValues()
        {
            EditorPrefs.SetString(_PREFS_TODOS_TOKEN, TODOToken);
            EditorPrefs.SetBool(_PREFS_TODOS_CONFIRMATION_DIALOGS, ShowConfirmationDialogs);
            EditorPrefs.SetInt(_PREFS_TODOS_BORDER_THICKNESS, BorderThickness);
            SetWelcome(ShowWelcome);
            SetColorPrefs();
            SetShortcutPrefs();
        }


        /// Set the value of a Color preference.
        private static void SetColorPrefs()
        {
            EditorPrefs.SetString(_PREFS_TODOS_COLOR_PRIMARY, RGBA.ColorToString(Primary));
            EditorPrefs.SetString(_PREFS_TODOS_COLOR_SECONDARY, RGBA.ColorToString(Secondary));
            EditorPrefs.SetString(_PREFS_TODOS_COLOR_TERTIARY, RGBA.ColorToString(Tertiary));
            EditorPrefs.SetString(_PREFS_TODOS_COLOR_QUATERNARY, RGBA.ColorToString(Quaternary));

            EditorPrefs.SetString(_PREFS_TODOS_COLOR_PRI1, RGBA.ColorToString(PriorityUrgent));
            EditorPrefs.SetString(_PREFS_TODOS_COLOR_PRI2, RGBA.ColorToString(PriorityNormal));
            EditorPrefs.SetString(_PREFS_TODOS_COLOR_PRI3, RGBA.ColorToString(PriorityMinor));
        }


        /// Set the value of the shortcut preference.
        private static void SetShortcutPrefs()
        {
            if (_newShortcut != Shortcut && _newShortcut != null)
            {
                Shortcut = _newShortcut;
                EditorPrefs.SetString(_PREFS_TODOS_SHORTCUT, Shortcut);
                var formattedShortcut = Shortcut.Replace("|", "");
                IO.OverwriteShortcut(formattedShortcut);
            }
        }


        /// Load color preferences.
        private static void GetColorPrefs()
        {
            Primary = GetPrefValue(_PREFS_TODOS_COLOR_PRIMARY, RGBA.GetNormalizedColor(PrimaryDefault));
            Secondary = GetPrefValue(_PREFS_TODOS_COLOR_SECONDARY, RGBA.GetNormalizedColor(SecondaryDefault));
            Tertiary = GetPrefValue(_PREFS_TODOS_COLOR_TERTIARY, RGBA.GetNormalizedColor(TertiaryDefault));
            Quaternary = GetPrefValue(_PREFS_TODOS_COLOR_QUATERNARY, RGBA.GetNormalizedColor(QuaternaryDefault));

            PriorityUrgent = GetPrefValue(_PREFS_TODOS_COLOR_PRI1, RGBA.GetNormalizedColor(PriorityUrgentDefault));
            PriorityNormal = GetPrefValue(_PREFS_TODOS_COLOR_PRI2, RGBA.GetNormalizedColor(PriorityNormalDefault));
            PriorityMinor = GetPrefValue(_PREFS_TODOS_COLOR_PRI3, RGBA.GetNormalizedColor(PriorityMinorDefault));

            // If all colors are the same, there's been some issue. Revert to initial dark scheme.
            if(Primary == Secondary && Primary == Tertiary && Primary == Quaternary)
            {
                Primary = RGBA.GetNormalizedColor(PrimaryDefault);
                Secondary = RGBA.GetNormalizedColor(SecondaryDefault);
                Tertiary = RGBA.GetNormalizedColor(TertiaryDefault);
                Quaternary = RGBA.GetNormalizedColor(QuaternaryDefault);
            }
        }


        /// Reset all preferences to default.
        private static void ResetPrefsToDefault()
        {
            TODOToken = _TODO_TOKEN_DEFAULT;
            ShowConfirmationDialogs = _CONFIRMATION_DIALOGS_DEFAULT;
            ShowWelcome = _SHOW_WELCOME_DEFAULT;
            BorderThickness = _BORDER_THICKNESS_DEFAULT;
            Primary = RGBA.GetNormalizedColor(PrimaryDefault);
            Secondary = RGBA.GetNormalizedColor(SecondaryDefault);
            Tertiary = RGBA.GetNormalizedColor(TertiaryDefault);
            Quaternary = RGBA.GetNormalizedColor(QuaternaryDefault);
            PriorityUrgent = RGBA.GetNormalizedColor(PriorityUrgentDefault);
            PriorityNormal = RGBA.GetNormalizedColor(PriorityNormalDefault);
            PriorityMinor = RGBA.GetNormalizedColor(PriorityMinorDefault);
            Shortcut = _SHORTCUT_DEFAULT;

            SetPrefValues();
            GetAllPrefValues();
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
                if (ShowConfirmationDialogs)
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
                if (canExecute)
                {
                    Primary = new Color(_primaryDark.r / 255.0f, _primaryDark.g / 255.0f, _primaryDark.b / 255.0f, 1.0f);
                    Secondary = new Color(_secondaryDark.r / 255.0f, _secondaryDark.g / 255.0f, _secondaryDark.b / 255.0f, 1.0f);
                    Tertiary = new Color(_tertiaryDark.r / 255.0f, _tertiaryDark.g / 255.0f, _tertiaryDark.b / 255.0f, 1.0f);
                    Quaternary = new Color(_quaternaryDark.r / 255.0f, _quaternaryDark.g / 255.0f, _quaternaryDark.b / 255.0f, 1.0f);
                    PriorityUrgent = new Color(_priorityUrgentDark.r / 255.0f, _priorityUrgentDark.g / 255.0f, _priorityUrgentDark.b / 255.0f, 1.0f);
                    PriorityNormal = new Color(_priorityNormalDark.r / 255.0f, _priorityNormalDark.g / 255.0f, _priorityNormalDark.b / 255.0f, 1.0f);
                    PriorityMinor = new Color(_priorityMinorDark.r / 255.0f, _priorityMinorDark.g / 255.0f, _priorityMinorDark.b / 255.0f, 1.0f);
                    SetColorPrefs();
                    GetColorPrefs();

                    ReloadSkins();

                    RepaintOpenWindows();
                }
            }
            if (GUILayout.Button("Load light theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (ShowConfirmationDialogs)
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
                if (canExecute)
                {
                    Primary = new Color(_primaryLight.r / 255.0f, _primaryLight.g / 255.0f, _primaryLight.b / 255.0f, 1.0f);
                    Secondary = new Color(_secondaryLight.r / 255.0f, _secondaryLight.g / 255.0f, _secondaryLight.b / 255.0f, 1.0f);
                    Tertiary = new Color(_tertiaryLight.r / 255.0f, _tertiaryLight.g / 255.0f, _tertiaryLight.b / 255.0f, 1.0f);
                    Quaternary = new Color(_quaternaryLight.r / 255.0f, _quaternaryLight.g / 255.0f, _quaternaryLight.b / 255.0f, 1.0f);
                    PriorityUrgent = new Color(_priorityUrgentLight.r / 255.0f, _priorityUrgentLight.g / 255.0f, _priorityUrgentLight.b / 255.0f, 1.0f);
                    PriorityNormal = new Color(_priorityNormalLight.r / 255.0f, _priorityNormalLight.g / 255.0f, _priorityNormalLight.b / 255.0f, 1.0f);
                    PriorityMinor = new Color(_priorityMinorLight.r / 255.0f, _priorityMinorLight.g / 255.0f, _priorityMinorLight.b / 255.0f, 1.0f);
                    SetColorPrefs();
                    GetColorPrefs();

                    ReloadSkins();

                    RepaintOpenWindows();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
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
            ModifierKeys[0] = GUILayout.Toggle(ModifierKeys[0], platformKey, EditorStyles.miniButton, GUILayout.Width(50));
            ModifierKeys[1] = GUILayout.Toggle(ModifierKeys[1], "ALT", EditorStyles.miniButton, GUILayout.Width(40));
            ModifierKeys[2] = GUILayout.Toggle(ModifierKeys[2], "SHIFT", EditorStyles.miniButton, GUILayout.Width(60));
            _mainShortcutKeyIndex = EditorGUILayout.Popup(_mainShortcutKeyIndex, ShortcutKeys, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            // Generate shortcut string.
            if (ModifierKeys[0])
            {
                shortcut += "%|";
            }
            if (ModifierKeys[1])
            {
                shortcut += "&|";
            }
            if (ModifierKeys[2])
            {
                shortcut += "#|";
            }
            shortcut += ShortcutKeys[_mainShortcutKeyIndex];

            return shortcut;
        }


        /// Draw buttons to rebuild the scripts list and reset preferences.
        private static void DrawResetButtons()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            DrawRebuildScriptListButton();
            GUILayout.Space(10);
            DrawResetButton();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Draw button to rebuild the scripts list.
        private static void DrawRebuildScriptListButton()
        {
            if (GUILayout.Button("Rebuild list of scripts", GUILayout.Width(140)))
            {
                QQQOps.FindAllScripts();
            }
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


        /// Get the value of an int preference.
        private static int GetPrefValue(string aKey, int aDefault)
        {
            int val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetInt(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetInt(aKey, aDefault);
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


        /// Get usable values from the shortcut string pref.
        private static void ParseShortcutValues()
        {
            var foundCmd = false;
            var foundAlt = false;
            var foundShift = false;

            var keys = Shortcut.Split('|');
            foreach (var key in keys)
            {
                switch (key)
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
                        _mainShortcutKeyIndex = System.Array.IndexOf(ShortcutKeys, key);
                        break;
                }
            }
            ModifierKeys[0] = foundCmd; // Ctrl/Cmd.
            ModifierKeys[1] = foundAlt; // Alt.
            ModifierKeys[2] = foundShift; // Shift.
        }


        /// Draw reset button.
        private static void DrawResetButton()
        {
            if (GUILayout.Button("Reset preferences", GUILayout.Width(120)))
            {
                ResetPrefsToDefault();
            }
        }


        /// Reload skins of open windows.
        private static void ReloadSkins()
        {
            if (WindowMain.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowMain)) as WindowMain;
                if (window != null)
                {
                    window.LoadStyles();
                }
            }
            if (WindowAdd.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowAdd)) as WindowMain;
                if (window != null)
                {
                    window.LoadStyles();
                }
            }
            if (WindowEdit.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowEdit)) as WindowMain;
                if (window != null)
                {
                    window.LoadStyles();
                }
            }
            if (WindowWelcome.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowWelcome)) as WindowWelcome;
                if (window != null)
                {
                    window.LoadStyle();
                }
            }
        }


        /// Repaint all open TODOs windows.
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
            if (WindowWelcome.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowWelcome)).Repaint();
            }
        }

#endregion

    }
}