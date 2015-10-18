using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
{
    // ====================================================================
    // ============== Variables used in more than one method ==============
    // ====================================================================
    public static List<QQQ> QQQs = new List<QQQ>();
    private GUISkin _GDTBSkin;
    private bool _GUISkinWasAssigned = false;
    private string _GDTBAssetPath;
    private string[] _qqqPriorities = { "Urgent", "Normal", "Minor"};

    // ====================================================================
    // =========================== Editor stuff ===========================
    // ====================================================================
    private const int BUTTON_WIDTH = 150;
    private const int BOX_WIDTH = 400;
    private const int POPUP_WIDTH = 60;
    private const int EDITOR_WINDOW_MINSIZE_X = 300;
    private const int EDITOR_WINDOW_MINSIZE_Y = 250;
    private const string WINDOW_TITLE = "CodeTODOs";

    // ====================================================================
    // ======================= Class functionality ========================
    // ====================================================================

    [MenuItem("Gamedev Toolbelt/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        //window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
        window.titleContent = new GUIContent(WINDOW_TITLE);
        window.Show();
    }

    public void OnEnable()
    {
        if(!EditorPrefs.HasKey("GDTB_Path"))
        {
            _GDTBAssetPath = GDTB_IOUtils.GetGDTBPath();
            EditorPrefs.SetString("GDTB_Path", _GDTBAssetPath);
        }
        else
        {
            _GDTBAssetPath = EditorPrefs.GetString("GDTB_Path");
        }

        if (_GDTBSkin == null)
        {
            _GDTBSkin = GDTB_IOUtils.GetGUISkin();
        }
        _GUISkinWasAssigned = true;

        if (QQQs.Count == 0)
        {
            CodeTODOsHelper.GetQQQsFromAllScripts();
            CodeTODOsHelper.ReorderQQQs();
        }
    }

    private void OnGUI()
    {
        if (_GUISkinWasAssigned == true)
        {
            GUI.skin = _GDTBSkin;
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        DrawQQQList();
        EditorGUILayout.Space();
        DrawListButton();
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
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
            var taskHeight = taskLines * _characterHeightCoefficient * 1.2f;
            var scriptLines = ((QQQs[i].Script.Length / CodeTODOsPrefs.CharsBeforeNewline) > 0) && CodeTODOsPrefs.CutoffSwitch == false ? (QQQs[i].Script.Length / CodeTODOsPrefs.CharsBeforeNewline) + 1 : 1;
            var scriptHeight = scriptLines * _characterHeightCoefficient;
            var boxHeight = taskHeight + scriptHeight;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(boxHeight));
            DrawPriority(QQQs[i]);
            DrawTaskAndScriptLabels(QQQs[i], taskHeight, scriptHeight);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    #region QQQPriorityMethods
    // This method was added to improve readability of inside OnGUI.
    // It simply selects which priority format to use based on the user preference.
    private void DrawPriority(QQQ qqq)
    {
        if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.TEXT_ONLY)
        {
            DrawPriorityText(qqq);
        }
        else if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.ICON_ONLY)
        {
            DrawPriorityIcon(qqq);
        }
        else if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.ICON_AND_TEXT)
        {
            DrawPriorityIconAndText(qqq);
        }
    }

    private void DrawPriorityText(QQQ qqq)
    {
        var priorityIndex = System.Convert.ToInt16(qqq.Priority);

        EditorGUILayout.BeginHorizontal(GUILayout.Width(16));
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        qqq.Priority = (QQQPriority)EditorGUILayout.Popup(priorityIndex, _qqqPriorities, GUILayout.Width(POPUP_WIDTH));
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPriorityIconAndText(QQQ qqq)
    {
        var priorityIndex = System.Convert.ToInt16(qqq.Priority);
        Texture2D tex = GetQQQPriorityTexture(priorityIndex);

        EditorGUILayout.BeginHorizontal(GUILayout.Width(16));
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        var propertyRect = EditorGUILayout.GetControlRect(GUILayout.Width(20));
        propertyRect.width = 16;
        propertyRect.height = 16;
        EditorGUI.DrawPreviewTexture(propertyRect, tex);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        qqq.Priority = (QQQPriority)EditorGUILayout.Popup(priorityIndex, _qqqPriorities, GUILayout.Width(POPUP_WIDTH));
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPriorityIcon(QQQ qqq)
    {
        var priorityIndex = System.Convert.ToInt16(qqq.Priority);
        Texture2D tex = GetQQQPriorityTexture(priorityIndex);

        EditorGUILayout.BeginHorizontal(GUILayout.Width(24));
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        var propertyRect = EditorGUILayout.GetControlRect(GUILayout.Width(16));
        propertyRect.width = 16;
        propertyRect.height = 16;
        EditorGUI.DrawPreviewTexture(propertyRect, tex);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }

    private Texture2D GetQQQPriorityTexture(int priority)
    {
        Texture2D tex;
        switch(priority)
        {
            case 0:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + "/GUI/qqqPriority_URGENT.psd", typeof(Texture2D)) as Texture2D;
                break;
            case 1:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + "/GUI/qqqPriority_NORMAL.psd", typeof(Texture2D)) as Texture2D;
                break;
            case 2:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + "/GUI/qqqPriority_MINOR.psd", typeof(Texture2D)) as Texture2D;
                break;
            default:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + "/GUI/qqqPriority_NORMAL.psd", typeof(Texture2D)) as Texture2D;
                break;
        }
        return tex;
    }
    #endregion

    private void DrawTaskAndScriptLabels(QQQ qqq, float taskHeight, float scriptHeight)
    {
        var labels = CodeTODOsHelper.FormatTaskAndScriptLabels(qqq);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.LabelField(labels[0], EditorStyles.boldLabel, GUILayout.Height(taskHeight));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(labels[1], GUILayout.Height(scriptHeight));
        EditorGUILayout.EndVertical();

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
            CodeTODOsHelper.ReorderQQQs();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
}