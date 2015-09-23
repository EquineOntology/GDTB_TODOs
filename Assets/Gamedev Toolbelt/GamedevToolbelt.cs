/*using UnityEngine;
using UnityEditor;

public class GamedevToolbelt : EditorWindow
{
    // GENERAL
    private const string PREFS_EDITORPREFS = "GDTB_Editor_Enable";
    private string[] _GDTBFiles = {"CodeTODOs.cs",
                                   "AnimationTester.cs"
                                   };    

    [MenuItem("Gamedev toolbelt/Options")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        GamedevToolbelt window = (GamedevToolbelt)EditorWindow.GetWindow(typeof(GamedevToolbelt));
        window.Show();
        window.title = "GDTB Options";
    }

    public void OnEnable()
    {   
        // Only go through the entire list of files once.
        if (EditorPrefs.GetBool(PREFS_EDITORPREFS, false) == true)
        {
            CheckPrefs();
        }
        else
        {
            UpdateGDTBEditorPrefs();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        DrawCodeTODOs();
		GUILayout.Space(10);
        DrawAnimationTester();
        GUILayout.Space(10);
        DrawRefreshButton();
        EditorGUILayout.EndVertical();
    }	
    
    private void CheckPrefs()
    {
        UpdateCodeTODOsPrefs();
        UpdateAnimationTesterPrefs();
    }
    
    private void UpdateGDTBEditorPrefs()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths();
        
        // For each asset, check if it's one of the extension's files,
        // and add the relevant keys to EditorPrefs.
        foreach(var asset in allAssets)
        {
            for(int i = 0; i < _GDTBFiles.Length; i++)
            {
                if(asset.EndsWith(_GDTBFiles[i]))
                {
                    switch(_GDTBFiles[i])
                    {
                        case "CodeTODOs.cs":
                            EnableCodeTODOs();
                            break;
                        case "AnimationTester.cs":
                            EnableAnimationTester();
                            break;
                    }
                }
            }
        }
        EditorPrefs.SetBool(PREFS_EDITORPREFS, true);
    }

    private const int BUTTON_WIDTH = 50;
    public void DrawRefreshButton()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if(GUILayout.Button("Refresh", GUILayout.Width(BUTTON_WIDTH)))
        {
            UpdateGDTBEditorPrefs();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
    
    // =============================================================
    // =======================   CodeTODOs   =======================
    // =============================================================
    private bool _codeTODOsIsInstalled = false;
    public const string PREFS_CODINGTODOS = "GDTB_CodeTODOs_Enable";
	public const string PREFS_TODO_TOKEN = "GDTB_CodeTODOs_TODOToken";
    public const string PREFS_CHARS_BEFORE_NEWLINE = "GDTB_CodeTODOs_CharsBeforeNewline";
    private string _todoToken;
    private string _currentTODOToken;
    private int _charsBeforeNewline;
    private int _currentCharsBeforeNewline;

    
    
    
    private void UpdateQQQTemplate(string newTemplate)
    {
        EditorPrefs.SetString(PREFS_TODO_TOKEN, newTemplate);
        _currentTODOToken = newTemplate;
    }
    
    private void UpdateCharsBeforeNewline(int newCharLimit)
    {
        EditorPrefs.SetInt(PREFS_CHARS_BEFORE_NEWLINE, newCharLimit);
        _currentCharsBeforeNewline = newCharLimit;
    }
    
    private void EnableCodeTODOs()
    {
        EditorPrefs.SetBool(PREFS_CODINGTODOS, true);
        _codeTODOsIsInstalled = true;
    }
    
    private void UpdateCodeTODOsPrefs()
    {
        if (_codeTODOsIsInstalled == true)
        {
            CodeTODOs.TODOToken = EditorPrefs.GetString(PREFS_TODO_TOKEN, "QQQ");
            CodeTODOs.CharsBeforeNewline = EditorPrefs.GetInt(PREFS_CHARS_BEFORE_NEWLINE, 60);
        }
    }

    // =============================================================
    // ====================   AnimationTester   ====================
    // =============================================================
    
    private bool _animationTesterIsInstalled = false;
    private const string PREFS_ANIMATIONTESTER = "GDTB_AnimationTester_Enable";
    
    private void DrawAnimationTester()
    {
        if (_animationTesterIsInstalled == true)
        {
        }
    }

    private void EnableAnimationTester()
    {
        EditorPrefs.SetBool(PREFS_ANIMATIONTESTER, true);
        _animationTesterIsInstalled = true;
    }
    
    private void UpdateAnimationTesterPrefs()
    {    
    }
}*/