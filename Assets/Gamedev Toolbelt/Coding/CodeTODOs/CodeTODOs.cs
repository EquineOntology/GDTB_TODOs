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
    private string _skinPath;
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

    // ====================================================================
    // ======================= Class functionality ========================
    // ====================================================================

    [MenuItem("Gamedev Toolbelt/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        //window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
        window.titleContent = new GUIContent(GUIConstants.TEXT_WINDOW_TITLE);
        window.Show();
    }

    public void OnEnable()
    {
        // If the path to the asset folder has been saved, get it (without having to go searching for it again).
        // Otherwise, find out where it is.
        if (EditorPrefs.HasKey("GDTB_Path"))
        {
            _GDTBAssetPath = EditorPrefs.GetString("GDTB_Path");
        }
        else
        {
            _GDTBAssetPath = GDTB_IOUtils.GetGDTBPath();
            EditorPrefs.SetString("GDTB_Path", _GDTBAssetPath);
        }

        // Try to load the skin. If there's an error loading it (i.e. if the user changed the asset folder),
        // search for the folder again and retry.
        try
        {
            _GDTBSkin = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }
        catch (System.Exception)
        {
            _GDTBAssetPath = GDTB_IOUtils.GetGDTBPath();
            EditorPrefs.SetString("GDTB_Path", _GDTBAssetPath);
            _GDTBSkin = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }

        if (QQQs.Count == 0)
        {
            CodeTODOsHelper.GetQQQsFromAllScripts();
            CodeTODOsHelper.ReorderQQQs();
        }
    }

    private void OnGUI()
    {
        GUI.skin = _GDTBSkin;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        DrawQQQs();
        EditorGUILayout.Space();
        DrawListButton();
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    // The horizontal and vertical space reserved for each character in a label.
    private float _characterHeightCoefficient = 15.0f;
    private Vector2 _scrollPosition = new Vector2(Screen.width - 5,Screen.height);
    private void DrawQQQs()
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
            DrawCompleteTaskButton(QQQs[i]);
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

    // Draw priority for the "Text only" setting.
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

    // Draw priority for the "Icon and Text" setting.
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

    // Draw priority for the "Icon only" setting.
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

    // Get the correct texture for a priority.
    private Texture2D GetQQQPriorityTexture(int priority)
    {
        Texture2D tex;
        switch(priority)
        {
            case 0:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.ICON_QQQ_URGENT, typeof(Texture2D)) as Texture2D;
                break;
            case 1:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.ICON_QQQ_NORMAL, typeof(Texture2D)) as Texture2D;
                break;
            case 2:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.ICON_QQQ_MINOR, typeof(Texture2D)) as Texture2D;
                break;
            default:
                tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.ICON_QQQ_NORMAL, typeof(Texture2D)) as Texture2D;
                break;
        }
        return tex;
    }
#endregion

    // Draws the "Task" and "Script" texts for QQQs.
    private void DrawTaskAndScriptLabels(QQQ qqq, float taskHeight, float scriptHeight)
    {
        var labels = CodeTODOsHelper.FormatTaskAndScriptLabels(qqq);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        qqq.Task = EditorGUILayout.TextField(labels[0], EditorStyles.boldLabel, GUILayout.Height(taskHeight));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(labels[1], GUILayout.Height(scriptHeight));
        EditorGUILayout.EndVertical();
    }

    // Draw the "Complete task" button on the right of a QQQ.
    private void DrawCompleteTaskButton(QQQ qqq)
    {
        var tex = AssetDatabase.LoadAssetAtPath(_GDTBAssetPath + GUIConstants.ICON_QQQ_DONE, typeof(Texture2D)) as Texture2D;
        var content = new GUIContent(GUIConstants.TEXT_QQQ_DONE, tex);
        var propertyRect = EditorGUILayout.GetControlRect(GUILayout.Width(20));
        propertyRect.width = tex.width + GUIConstants.TEXT_QQQ_DONE.Length*5.5f;
        propertyRect.height = tex.height;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        if(GUI.Button(propertyRect, content))
        {
            CodeTODOsHelper.CompleteQQQ(qqq);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    // Draw the "Refresh list" button.
    private void DrawListButton()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button(GUIConstants.TEXT_REFRESH_LIST, GUILayout.Width(BUTTON_WIDTH)))
        {
            QQQs.Clear();
            CodeTODOsHelper.GetQQQsFromAllScripts();
            CodeTODOsHelper.ReorderQQQs();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
    }
}