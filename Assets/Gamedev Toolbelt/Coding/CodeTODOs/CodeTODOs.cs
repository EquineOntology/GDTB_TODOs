using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
{
    // ====================================================================
    // ============== Variables used in more than one method ==============
    // ====================================================================
    //public ReorderableList _reorderableQQQs;
    public static List<QQQ> QQQs = new List<QQQ>();
    //private List<int> _priorities = new List<int>();
    private List<string> _qqqScripts = new List<string>();
    private List<string> _qqqTasks = new List<string>();

    // ====================================================================
    // =========================== Editor stuff ===========================
    // ====================================================================
    private const int BUTTON_WIDTH = 100;
    private const int BOX_WIDTH = 400;
    private const int EDITOR_WINDOW_MINSIZE_X = 300;
    private const int EDITOR_WINDOW_MINSIZE_Y = 300;
    private const string WINDOW_TITLE = "CodeTODOs";
    private GUISkin _GDTBSkin;
    private bool _GUISkinWasAssigned = false;

    // ====================================================================
    // ======================= Class functionality ========================
    // ====================================================================
    [MenuItem("Gamedev Toolbelt/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.titleContent = new GUIContent(WINDOW_TITLE);
        window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
        window.Show();
    }

    public void OnEnable()
    {
        //UpdateEditorPrefs();
        if(_GDTBSkin == null)
        {
            _GDTBSkin = GDTB_IOUtils.GetGUISkin();
            _GUISkinWasAssigned = true;
        }
        else
        {
            _GUISkinWasAssigned = true;
        }

        QQQs = CodeTODOsHelper.GetQQQsFromAllScripts(out _qqqTasks, out _qqqScripts);
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
        if (_GUISkinWasAssigned == true)
        {
            GUI.skin = _GDTBSkin;
        }
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        DrawQQQList();
        EditorGUILayout.Space();
        DrawListButton();
        EditorGUILayout.EndVertical();
    }

    // The horizontal and vertical space reserved for each character in a label.
    private float _characterHeightCoefficient = 15.0f;
    private void DrawQQQList()
    {
        EditorGUILayout.BeginVertical();

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
            CodeTODOsHelper.GetQQQsFromAllScripts(out _qqqTasks, out _qqqScripts);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
}