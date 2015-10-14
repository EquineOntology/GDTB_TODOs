using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
{
    // ====================================================================
    // ============== Variables used in more than one method ==============
    // ====================================================================
    public static List<QQQ> QQQs = new List<QQQ>();

    // ====================================================================
    // =========================== Editor stuff ===========================
    // ====================================================================
    private const int BUTTON_WIDTH = 150;
    private const int BOX_WIDTH = 400;
    private const int EDITOR_WINDOW_MINSIZE_X = 300;
    private const int EDITOR_WINDOW_MINSIZE_Y = 250;
    private const string WINDOW_TITLE = "CodeTODOs";

    // ====================================================================
    // ======================= Class functionality ========================
    // ====================================================================
    private GUISkin _GDTBSkin;
    private bool _GUISkinWasAssigned = false;

    [MenuItem("Gamedev Toolbelt/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
        window.titleContent = new GUIContent(WINDOW_TITLE);
        window.Show();
    }

    public void OnEnable()
    {
        if(_GDTBSkin == null)
        {
            _GDTBSkin = GDTB_IOUtils.GetGUISkin();
            _GUISkinWasAssigned = true;
        }
        else
        {
            _GUISkinWasAssigned = true;
        }
        if (QQQs.Count == 0)
        {
            QQQs = CodeTODOsHelper.GetQQQsFromAllScripts();
        }
    }

    private void OnGUI()
    {
        if (_GUISkinWasAssigned == true)
        {
            GUI.skin = _GDTBSkin;
        }
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        DrawQQQList();
        EditorGUILayout.Space();
        DrawListButton();
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
    }

    // The horizontal and vertical space reserved for each character in a label.
    private float _characterHeightCoefficient = 15.0f;
    private Vector2 _scrollPosition = new Vector2(Screen.width - 5,Screen.height);
    private void DrawQQQList()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, _GDTBSkin.scrollView);

        for (int i = 0; i < QQQs.Count; i++)
        {
            // Calculate how high the box must be to accomodate the task & script.
            var taskLines = ((QQQs[i].Task.Length / CodeTODOsPrefs.CharsBeforeNewline) > 0) && CodeTODOsPrefs.CutoffSwitch == false ? (QQQs[i].Task.Length / CodeTODOsPrefs.CharsBeforeNewline) + 1 : 1;
            var taskHeight = taskLines * _characterHeightCoefficient * 1.1f;
            var scriptLines = ((QQQs[i].Script.Length / CodeTODOsPrefs.CharsBeforeNewline) > 0) && CodeTODOsPrefs.CutoffSwitch == false ? (QQQs[i].Script.Length / CodeTODOsPrefs.CharsBeforeNewline) + 1 : 1;
            var scriptHeight = scriptLines * _characterHeightCoefficient;
            var boxHeight = taskHeight + scriptHeight;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(boxHeight));
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            // Build correctly formatted "task" label.
            // The number of characters before a newline is reduced to account for the bold label style.
            string taskLabel;
            if (CodeTODOsPrefs.CutoffSwitch == true)
            {
                taskLabel = CodeTODOsHelper.GetStringEnd(QQQs[i].Task, CodeTODOsPrefs.CharsBeforeNewline - 8);
            }
            else
            {
                taskLabel = CodeTODOsHelper.DivideStringWithNewlines(QQQs[i].Task, CodeTODOsPrefs.CharsBeforeNewline - 8);
            }
            EditorGUILayout.LabelField(taskLabel, EditorStyles.boldLabel, GUILayout.Height(taskHeight));
            EditorGUILayout.EndHorizontal();

            // Build correctly formatted "script" label.
            string scriptLabel;
            if (CodeTODOsPrefs.CutoffSwitch == true)
            {
                scriptLabel = "In \"..." + CodeTODOsHelper.GetStringEnd(QQQs[i].Script, CodeTODOsPrefs.CharsBeforeNewline) + "\"";
            }
            else
            {
                scriptLabel = "In \"" + CodeTODOsHelper.DivideStringWithNewlines(QQQs[i].Script, CodeTODOsPrefs.CharsBeforeNewline) + "\"";
            }
            EditorGUILayout.LabelField(scriptLabel, GUILayout.Height(scriptHeight));

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private const string LIST_QQQS = "Force list refresh";
    private void DrawListButton()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button(LIST_QQQS, GUILayout.Width(BUTTON_WIDTH)))
        {
            QQQs.Clear();
            CodeTODOsHelper.GetQQQsFromAllScripts();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
}