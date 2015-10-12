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

    // Cutoff or newline
    public const string PREFS_CODETODOS_CUTOFF = "GDTB_CodeTODOs_Cutoff";
    private static bool _cutoffSwitch = true;
    public static bool CutoffSwitch
    {
        get { return _cutoffSwitch; }
    }

    // Characters in QQQs before newline/cutoff
    public const string PREFS_CODETODOS_CHARS_BEFORE_NEWLINE = "GDTB_CodeTODOs_CharsBeforeNewline";
    private static int _charsBeforeNewline = 40;
    public static int CharsBeforeNewline
    {
        get { return _charsBeforeNewline; }
    }

    [PreferenceItem("Code TODOs")]
    public static void PreferencesGUI()
    {
        /*  ===================================================
         * | Options                                           |
         * |   TODO token ____________________                 |
         * |   Cutoff TODOs _                                  |
         * |   Characters before newline __                    |
         *  =================================================== */
        EditorGUILayout.BeginVertical();
        _todoToken = EditorGUILayout.TextField("TODO token", _todoToken);
        _cutoffSwitch = EditorGUILayout.Toggle("Cutoff TODOs", _cutoffSwitch);
        _charsBeforeNewline = EditorGUILayout.IntField("Characters on line", _charsBeforeNewline);

        if(GUI.changed)
        {
            UpdatePreferences();
        }
    }

    private static void UpdatePreferences()
    {
        UpdateQQQTemplate(_todoToken);
        UpdateCutoffSwitch(_cutoffSwitch);
        UpdateCharsBeforeNewline(_charsBeforeNewline);
    }

    private static void UpdateQQQTemplate(string newToken)
    {
        EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, newToken);
    }

    private static void UpdateCharsBeforeNewline(int newCharLimit)
    {
        EditorPrefs.SetInt(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE, newCharLimit);
    }

    private static void UpdateCutoffSwitch(bool newSwitchValue)
    {
        EditorPrefs.SetBool(PREFS_CODETODOS_CUTOFF, newSwitchValue);
    }

    private void UpdateEditorPrefs()
    {
        /// TODO token.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_TOKEN))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, "QQQ");
        }
        else
        {
            _todoToken = EditorPrefs.GetString(PREFS_CODETODOS_TOKEN, "QQQ");
        }

        // Chars before newline.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE))
        {
            EditorPrefs.SetInt(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE, 60);
        }
        else
        {
            _charsBeforeNewline = EditorPrefs.GetInt(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE, 60);
        }

        // Cutoff or newline.
        if (!EditorPrefs.HasKey(PREFS_CODETODOS_CUTOFF))
        {
            EditorPrefs.SetBool(PREFS_CODETODOS_CUTOFF, false);
        }
        else
        {
            _cutoffSwitch = EditorPrefs.GetBool(PREFS_CODETODOS_CUTOFF, false);
        }
    }
}