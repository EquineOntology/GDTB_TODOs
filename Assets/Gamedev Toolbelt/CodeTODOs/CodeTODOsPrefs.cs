#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CodeTODOsPrefs
{

    // TODO token (QQQ)
    public const string PREFS_CODETODOS_TOKEN = "GDTB_CodeTODOs_TODOToken";
    private static string _todoToken = "QQQ";
    public static string TODOToken
    {
        get { return _todoToken; }
    }


    // Priority displayed as text, icon, text + icon
    public const string PREFS_CODETODOS_PRIORITY_DISPLAY = "GDTB_CodeTODOs_PriorityDisplay";
    private static PriorityDisplayFormat _priorityDisplay = PriorityDisplayFormat.ICON_ONLY;
    public static PriorityDisplayFormat QQQPriorityDisplay
    {
        get { return _priorityDisplay; }
    }
    private static string[] _displayFormatsString = { "Text only", "Icon only", "Icon and Text" };


    // Auto-update QQQs
    public const string PREFS_CODETODOS_AUTO_REFRESH = "GDTB_CodeTODOs_AutoUpdate";
    private static bool _autoRefresh = true;
    public static bool AutoRefresh
    {
        get { return _autoRefresh; }
    }

    [PreferenceItem("Code TODOs")]
    public static void PreferencesGUI()
    {
        RefreshPrefs();
        EditorGUILayout.BeginVertical();
        _todoToken = EditorGUILayout.TextField("TODO token", _todoToken);
        _priorityDisplay = (PriorityDisplayFormat) EditorGUILayout.Popup("Priority format", System.Convert.ToInt16(_priorityDisplay), _displayFormatsString);
        _autoRefresh = EditorGUILayout.Toggle("Auto refresh", _autoRefresh);

        if (GUI.changed)
        {
            UpdatePreferences();
            RefreshPrefs();
        }
    }

    private static void UpdatePreferences()
    {
        UpdateQQQTemplate(_todoToken);
        UpdatePriorityDisplay(_priorityDisplay);
        UpdateAutoRefresh(_autoRefresh);
    }


    private static void UpdateQQQTemplate(string aNewToken)
    {
        EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, aNewToken);
    }


    private static void UpdatePriorityDisplay(PriorityDisplayFormat aDisplayFormat)
    {
        EditorPrefs.SetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, System.Convert.ToInt16(aDisplayFormat));
    }


    private static void UpdateAutoRefresh(bool aToggle)
    {
        EditorPrefs.SetBool(PREFS_CODETODOS_AUTO_REFRESH, aToggle);
    }


    /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
    public static void RefreshPrefs()
    {
        // TODO token.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_TOKEN))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, "QQQ");
            _todoToken = "QQQ";
        }
        else
        {
            _todoToken = EditorPrefs.GetString(PREFS_CODETODOS_TOKEN, "QQQ");
        }

        // QQQ Priority display
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_PRIORITY_DISPLAY))
        {
            EditorPrefs.SetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, 2);
            _priorityDisplay = PriorityDisplayFormat.ICON_ONLY;
        }
        else
        {
            _priorityDisplay = (PriorityDisplayFormat)EditorPrefs.GetInt(PREFS_CODETODOS_PRIORITY_DISPLAY, 2);
        }

        // Auto refresh
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_AUTO_REFRESH))
        {
            EditorPrefs.SetBool(PREFS_CODETODOS_AUTO_REFRESH, true);
            _autoRefresh = true;
        }
        else
        {
            _autoRefresh = EditorPrefs.GetBool(PREFS_CODETODOS_AUTO_REFRESH, true);
        }
    }
}


public enum PriorityDisplayFormat
{
    TEXT_ONLY,
    ICON_ONLY,
    ICON_AND_TEXT
}
#endif