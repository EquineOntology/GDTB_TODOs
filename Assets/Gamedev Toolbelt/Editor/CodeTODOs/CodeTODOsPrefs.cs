#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CodeTODOsPrefs
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
    private static PriorityDisplayFormat _priorityDisplay = PriorityDisplayFormat.ICON_ONLY;
    private static int _priorityDisplay_default = 2;
    public static PriorityDisplayFormat QQQPriorityDisplay
    {
        get { return _priorityDisplay; }
    }
    private static string[] _displayFormatsString = { "Text only", "Icon only", "Icon and Text", "Bars" };


    // Auto-update QQQs.
    public const string PREFS_CODETODOS_AUTO_REFRESH = "GDTB_CodeTODOs_AutoUpdate";
    private static bool _autoRefresh = true;
    private static bool _autoRefresh_default = true;
    public static bool AutoRefresh
    {
        get { return _autoRefresh; }
    }


    // Colour of URGENT tasks.
    public const string PREFS_CODETODOS_COLOR_PRI1 = "GDTB_CodeTODOs_Color1";
    private static Color _priColor1 = Color.red;
    private static Color _priColor1_default = Color.red;
    public static Color PriColor1
    {
        get { return _priColor1; }
    }

    // Colour of NORMAL tasks
    public const string PREFS_CODETODOS_COLOR_PRI2 = "GDTB_CodeTODOs_Color2";
    private static Color _priColor2 = Color.yellow;
    private static Color _priColor2_default = Color.yellow;
    public static Color PriColor2
    {
        get { return _priColor2; }
    }

    // Colour of MINOR tasks
    public const string PREFS_CODETODOS_COLOR_PRI3 = "GDTB_CodeTODOs_Color3";
    private static Color _priColor3 = Color.green;
    private static Color _priColor3_default = Color.green;
    public static Color PriColor3
    {
        get { return _priColor3; }
    }
    #endregion fields

    [PreferenceItem("Code TODOs")]
    public static void PreferencesGUI()
    {
        RefreshPrefs();
        EditorGUILayout.BeginVertical();
        _todoToken = EditorGUILayout.TextField("TODO token", _todoToken);
        _priorityDisplay = (PriorityDisplayFormat) EditorGUILayout.Popup("Priority format", System.Convert.ToInt16(_priorityDisplay), _displayFormatsString);
        _autoRefresh = EditorGUILayout.Toggle("Auto refresh", _autoRefresh);
        _priColor1 = EditorGUILayout.ColorField("High priority color", _priColor1);
        _priColor2 = EditorGUILayout.ColorField("Normal priority color", _priColor2);
        _priColor3 = EditorGUILayout.ColorField("Minor priority color", _priColor3);

        if (GUI.changed)
        {
            UpdatePreferences();
            RefreshPrefs();
        }
    }

    private static void UpdatePreferences()
    {
        UpdateQQQTemplate();
        UpdatePriorityDisplay(_priorityDisplay);
        UpdateAutoRefresh();
        UpdateColours();
    }

    private static void UpdateQQQTemplate()
    {
        EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, _todoToken);
    }

    private static void UpdatePriorityDisplay(PriorityDisplayFormat aDisplayFormat)
    {
        EditorPrefs.SetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, System.Convert.ToInt16(aDisplayFormat));
    }

    private static void UpdateAutoRefresh()
    {
        EditorPrefs.SetBool(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh);
    }

    private static void UpdateColours()
    {
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI1, RGBA.ColorToString(_priColor1));
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI2, RGBA.ColorToString(_priColor2));
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI3, RGBA.ColorToString(_priColor3));
    }


    /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
    public static void RefreshPrefs()
    {
        // TODO token.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_TOKEN))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, _todoToken_default);
            _todoToken = _todoToken_default;
        }
        else
        {
            _todoToken = EditorPrefs.GetString(PREFS_CODETODOS_TOKEN, _todoToken_default);
        }

        // QQQ Priority display
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_PRIORITY_DISPLAY))
        {
            EditorPrefs.SetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, _priorityDisplay_default);
            _priorityDisplay = PriorityDisplayFormat.ICON_ONLY;
        }
        else
        {
            _priorityDisplay = (PriorityDisplayFormat)EditorPrefs.GetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, _priorityDisplay_default);
        }

        // Auto refresh
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_AUTO_REFRESH))
        {
            EditorPrefs.SetBool(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh_default);
            _autoRefresh = _autoRefresh_default;
        }
        else
        {
            _autoRefresh = EditorPrefs.GetBool(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh_default);
        }


        // URGENT priority color.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_COLOR_PRI1))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI1, RGBA.ColorToString(_priColor1_default));
            _priColor1 = _priColor1_default;
        }
        else
        {
            _priColor1 = RGBA.StringToColor(EditorPrefs.GetString(PREFS_CODETODOS_COLOR_PRI1, RGBA.ColorToString(_priColor1_default)));
        }

        // NORMAL priority color.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_COLOR_PRI2))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI2, RGBA.ColorToString(_priColor2_default));
            _priColor2 = _priColor2_default;
        }
        else
        {
            _priColor2 = RGBA.StringToColor(EditorPrefs.GetString(PREFS_CODETODOS_COLOR_PRI2, RGBA.ColorToString(_priColor2_default)));
        }

        // MINOR priority color.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_COLOR_PRI3))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI3, _priColor3_default.ToString());
            _priColor3 = _priColor3_default;
        }
        else
        {
            _priColor3 = RGBA.StringToColor(EditorPrefs.GetString(PREFS_CODETODOS_COLOR_PRI3, RGBA.ColorToString(_priColor3_default)));
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
#endif