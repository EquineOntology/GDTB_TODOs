#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class GDTB_CodeTODOsPrefs
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


    // Color of URGENT tasks.
    public const string PREFS_CODETODOS_COLOR_PRI1 = "GDTB_CodeTODOs_Color1";
    private static Color _priColor1 = new Color(246,71,71,1);  // Sunset orange http://www.flatuicolorpicker.com/pink
    private static Color _priColor1_default = new Color(246,71,71,1);
    public static Color PriColor1
    {
        get { return _priColor1; }
    }

    // Color of NORMAL tasks
    public const string PREFS_CODETODOS_COLOR_PRI2 = "GDTB_CodeTODOs_Color2";
    private static Color _priColor2 = new Color (244, 208, 63, 1); // Saffron http://www.flatuicolorpicker.com/yellow
    private static Color _priColor2_default = new Color (244, 208, 63, 1);
    public static Color PriColor2
    {
        get { return _priColor2; }
    }

    // Color of MINOR tasks
    public const string PREFS_CODETODOS_COLOR_PRI3 = "GDTB_CodeTODOs_Color3";
    private static Color _priColor3 = new Color (46, 204, 113, 1); // Shamrock http://www.flatuicolorpicker.com/green
    private static Color _priColor3_default = new Color (46, 204, 113, 1);
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
    #endregion fields


    [PreferenceItem("Code TODOs")]
    public static void PreferencesGUI()
    {
        RefreshAllPrefs();
        EditorGUILayout.BeginVertical();
        _todoToken = EditorGUILayout.TextField("TODO token", _todoToken);
        _priorityDisplay = (PriorityDisplayFormat) EditorGUILayout.Popup("Priority format", System.Convert.ToInt16(_priorityDisplay), _displayFormatsString);
        _autoRefresh = EditorGUILayout.Toggle("Auto refresh", _autoRefresh);
        _priColor1 = EditorGUILayout.ColorField("High priority color", _priColor1);
        _priColor2 = EditorGUILayout.ColorField("Normal priority color", _priColor2);
        _priColor3 = EditorGUILayout.ColorField("Minor priority color", _priColor3);
        _borderColor = EditorGUILayout.ColorField("Borders", _borderColor);

        if (GUI.changed)
        {
            UpdatePreferences();
            RefreshAllPrefs();
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
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI1, GDTB_RGBA.ColorToString(_priColor1));
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI2, GDTB_RGBA.ColorToString(_priColor2));
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_PRI3, GDTB_RGBA.ColorToString(_priColor3));
        EditorPrefs.SetString(PREFS_CODETODOS_COLOR_BORDER, GDTB_RGBA.ColorToString(_borderColor));
    }


    /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
    public static void RefreshAllPrefs()
    {
        // TODO token.
        _todoToken = RefreshPref(PREFS_CODETODOS_TOKEN, _todoToken, _todoToken_default);

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
        _autoRefresh = RefreshPref(PREFS_CODETODOS_AUTO_REFRESH, _autoRefresh, _autoRefresh_default);

        // URGENT priority color.
        _priColor1 = RefreshPref(PREFS_CODETODOS_COLOR_PRI1, _priColor1, _priColor1_default);

        // NORMAL priority color.
        _priColor2 = RefreshPref(PREFS_CODETODOS_COLOR_PRI2, _priColor2, _priColor2_default);

        // MINOR priority color.
        _priColor3 = RefreshPref(PREFS_CODETODOS_COLOR_PRI3, _priColor3, _priColor3_default);

        // Priority bar border color.
        _borderColor = RefreshPref(PREFS_CODETODOS_COLOR_BORDER, _borderColor, _borderColor_default);
    }

    private static bool RefreshPref(string aKey, bool aValue, bool aDefault)
    {
        if (!EditorPrefs.HasKey(aKey))
        {
            EditorPrefs.SetBool(aKey, aDefault);
            aValue = aDefault;
        }
        else
        {
            aValue = EditorPrefs.GetBool(aKey, aDefault);
        }

        return aValue;
    }

    private static string RefreshPref(string aKey, string aValue, string aDefault)
    {
        if (!EditorPrefs.HasKey(aKey))
        {
            EditorPrefs.SetString(aKey, aDefault);
            aValue = aDefault;
        }
        else
        {
            aValue = EditorPrefs.GetString(aKey, aDefault);
        }

        return aValue;
    }

    private static Color RefreshPref(string aKey, Color aValue, Color aDefault)
    {
        if (!EditorPrefs.HasKey(aKey))
        {
            EditorPrefs.SetString(aKey, GDTB_RGBA.ColorToString(aDefault));
            aValue = aDefault;
        }
        else
        {
            aValue = GDTB_RGBA.StringToColor(EditorPrefs.GetString(aKey, GDTB_RGBA.ColorToString(aDefault)));
        }

        return aValue;
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