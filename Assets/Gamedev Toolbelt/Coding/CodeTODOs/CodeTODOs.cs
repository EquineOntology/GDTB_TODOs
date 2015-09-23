﻿using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
{
    // QQQ variables used in more than one method.
    //public ReorderableList _reorderableQQQs;
    public List<QQQ> _qqqs = new List<QQQ>();
    //private List<int> _priorities = new List<int>();
    private List<string> _qqqScripts = new List<string>();
    private List<string> _qqqTasks = new List<string>();

    // Constants used in more than one method.
    private const int BUTTON_WIDTH = 100;    

    [MenuItem("Gamedev toolbelt/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.Show();
    }

    public void OnEnable()
    {
        UpdateEditorPrefs();
        _showOptions = new AnimBool(false);
        _qqqs = CodeTODOsHelper.CheckAllScriptsForQQQs(out _qqqTasks, out _qqqScripts);
        /*_reorderableQQQs = new ReorderableList(_qqqs, typeof(QQQ), true, true, true, true);

        _reorderableQQQs.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = _reorderableQQQs.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Task"));
            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Script"));
        };*/
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        DrawQQQList();
        EditorGUILayout.Space();
        DrawListButton();
        EditorGUILayout.Space();
        DrawOptions();
        EditorGUILayout.EndVertical();
    }
    
    private const int BOX_WIDTH = 400;    
    private void DrawQQQList()
    {
        EditorGUILayout.BeginVertical();
        
        for(int i= 0; i< _qqqs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(BOX_WIDTH));            
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();            
            GUILayout.Space(10);
            EditorGUILayout.LabelField(_qqqs[i].Task, EditorStyles.boldLabel, GUILayout.Width(BOX_WIDTH - 20));            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("In \"" + _qqqs[i].Script + "\".", GUILayout.Width(BOX_WIDTH - 20));

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();

        //_reorderableQQQs.DoLayoutList();
        //_reorderableQQQs.DoList(new Rect(20, 20, 200, 200));
    }

    private const string LIST_QQQS = "Refresh list";
    private void DrawListButton()
    {
        EditorGUILayout.BeginHorizontal();    
        EditorGUILayout.Space();
        if (GUILayout.Button(LIST_QQQS, GUILayout.Width(BUTTON_WIDTH)))
        {
            CodeTODOsHelper.FindAllScripts();
            CodeTODOsHelper.CheckAllScriptsForQQQs(out _qqqTasks, out _qqqScripts);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    // ===========================================================
    // =======================   OPTIONS   =======================
    // ===========================================================    
	
    // TODO token (QQQ)
    public const string PREFS_CODETODOS_TOKEN = "GDTB_CodeTODOs_TODOToken";    
    public static string TODOToken = "QQQ";
    private string _oldTODOTokenValue = "QQQ";

    // Cutoff or newline
    public const string PREFS_CODETODOS_CUTOFF = "GDTB_CodeTODOs_Cutoff";
    private bool _cutoffSwitch = false;
    private bool _oldCutoffSwitchValue = false;

    // Characters in QQQs before newline/cutoff
    public const string PREFS_CODETODOS_CHARS_BEFORE_NEWLINE = "GDTB_CodeTODOs_CharsBeforeNewline";
    private int _charsBeforeNewline = 60;
    private int _oldCharsBeforeNewlineValue = 60;
    
    private AnimBool _showOptions;
    
    private void DrawOptions()
    {
        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        _showOptions.target = GUILayout.Toggle(_showOptions.value, "Show Options", "Button", GUILayout.Width(BUTTON_WIDTH));
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        
        /*  ===================================================
         * | Options                                           |
         * |   TODO token ____________________                 |
         * |   Cutoff TODOs _                                  |
         * |   Characters before newline __                    |
         *  =================================================== */
        if (EditorGUILayout.BeginFadeGroup(_showOptions.faded))
		{
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
			
			// TODO token.
            TODOToken = EditorGUILayout.TextField("TODO token", TODOToken);
            if (TODOToken != _oldTODOTokenValue)
            {
                UpdateQQQTemplate(TODOToken);
            }
            // Cutoff TODOs
            _cutoffSwitch = EditorGUILayout.Toggle("Cutoff TODOs", _cutoffSwitch);
            if(_cutoffSwitch != _oldCutoffSwitchValue)
            {
                UpdateCutoffSwitch(_cutoffSwitch);
            }
            // Chars before newline.
            _charsBeforeNewline = EditorGUILayout.IntField("Characters on line", _charsBeforeNewline);
            if (_charsBeforeNewline != _oldCharsBeforeNewlineValue)
            {
                UpdateCharsBeforeNewline(_charsBeforeNewline);
            }
            EditorGUI.indentLevel--;
        }
		EditorGUILayout.EndFadeGroup();        
        EditorGUILayout.EndVertical();
    }
    
    private void UpdateQQQTemplate(string newToken)
    {
        EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, newToken);
        _oldTODOTokenValue = newToken;
    }
    
    private void UpdateCharsBeforeNewline(int newCharLimit)
    {
        EditorPrefs.SetInt(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE, newCharLimit);
        _oldCharsBeforeNewlineValue = newCharLimit;
    }
    
    private void UpdateCutoffSwitch(bool newSwitchValue)
    {
        EditorPrefs.SetBool(PREFS_CODETODOS_CUTOFF, newSwitchValue);
        _oldCutoffSwitchValue = newSwitchValue;
    }
    
    private void UpdateEditorPrefs()
    {        
        /// TODO token.
        if(!EditorPrefs.HasKey(PREFS_CODETODOS_TOKEN))
        {
            EditorPrefs.SetString(PREFS_CODETODOS_TOKEN, "QQQ");
        }
        else
        {
            TODOToken = EditorPrefs.GetString(PREFS_CODETODOS_TOKEN, "QQQ");
        }
        
        // Chars before newline.
        if(!EditorPrefs.HasKey(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE))
        {
            EditorPrefs.SetInt(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE, 60);
        }
        else
        {
            _charsBeforeNewline = EditorPrefs.GetInt(PREFS_CODETODOS_CHARS_BEFORE_NEWLINE, 60);
        }
        
        // Cutoff or newline.
        if(!EditorPrefs.HasKey(PREFS_CODETODOS_CUTOFF))
        {
            EditorPrefs.SetBool(PREFS_CODETODOS_CUTOFF, false);
        }
        else
        {
            _cutoffSwitch = EditorPrefs.GetBool(PREFS_CODETODOS_CUTOFF, false);
        }
    }
}