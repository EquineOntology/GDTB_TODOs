#if UNITY_EDITOR
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

    // ====================================================================
    // =========================== Editor stuff ===========================
    // ====================================================================
    private const int BUTTON_WIDTH = 150;
    private const int BOX_WIDTH = 400;
    private const int POPUP_WIDTH = 60;
    private const int EDITOR_WINDOW_MINSIZE_X = 300;
    private const int EDITOR_WINDOW_MINSIZE_Y = 250;
    private const int ICON_BUTTON_SIZE = 16;

    // ====================================================================
    // ======================= Class functionality ========================
    // ====================================================================

    [MenuItem("Window/CodeTODOs")]
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
        _GDTBSkin = Resources.Load(GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;

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
    private float _characterHeightCoefficient = 16.0f;
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
            DrawEditTaskButton(QQQs[i]);
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
        var priorityRect = EditorGUILayout.GetControlRect(GUILayout.Width(50));
        EditorGUI.LabelField(priorityRect, qqq.Priority.ToString());
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
        var propertyRect = EditorGUILayout.GetControlRect(GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE));

        EditorGUI.DrawPreviewTexture(propertyRect, tex);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(qqq.Priority.ToString());
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
        var propertyRect = EditorGUILayout.GetControlRect(GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE));

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
                tex = Resources.Load(GUIConstants.FILE_QQQ_URGENT, typeof(Texture2D)) as Texture2D;
                break;
            case 1:
                tex = Resources.Load(GUIConstants.FILE_QQQ_NORMAL, typeof(Texture2D)) as Texture2D;
                break;
            case 2:
                tex = Resources.Load(GUIConstants.FILE_QQQ_MINOR, typeof(Texture2D)) as Texture2D;
                break;
            default:
                tex = Resources.Load(GUIConstants.FILE_QQQ_NORMAL, typeof(Texture2D)) as Texture2D;
                break;
        }
        return tex;
    }
#endregion

    // Draws the "Task" and "Script" texts for QQQs.
    private void DrawTaskAndScriptLabels(QQQ qqq, float taskHeight, float scriptHeight)
    {
        var labels = CodeTODOsHelper.FormatTaskAndScriptLabels(qqq);
        var taskWidth = qqq.Task.Length * 9.0f;
        var scriptWidth = qqq.Script.Length * 8.0f;

        var labelWidth = taskWidth > scriptWidth ? taskWidth : scriptWidth;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        var taskLabelRect = EditorGUILayout.GetControlRect(GUILayout.Height(taskHeight), GUILayout.Width(labelWidth));
        EditorGUI.LabelField(taskLabelRect, labels[0], EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        GUIStyle link = new GUIStyle(EditorStyles.label);
        link.normal.textColor = Color.blue;
        var scriptLabelRect = EditorGUILayout.GetControlRect(GUILayout.Height(scriptHeight), GUILayout.Width(labelWidth));
        EditorGUIUtility.AddCursorRect(scriptLabelRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && scriptLabelRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsHelper.OpenScript(qqq);
        }
        EditorGUI.LabelField(scriptLabelRect, labels[1], link);

        EditorGUILayout.EndVertical();
    }

    // Draw the "Edit task" button.
    private void DrawEditTaskButton(QQQ qqq)
    {
        var tex = Resources.Load(GUIConstants.FILE_QQQ_EDIT, typeof(Texture2D)) as Texture2D;
        var buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE));

        buttonRect.x += buttonRect.width + 4;
        buttonRect.y += 1;
        EditorGUI.DrawPreviewTexture(buttonRect, tex);
        EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsEdit.Init(qqq);
        }
    }

    // Draw the "Complete task" button.
    private void DrawCompleteTaskButton(QQQ qqq)
    {
        var tex = Resources.Load(GUIConstants.FILE_QQQ_DONE, typeof(Texture2D)) as Texture2D;
        var buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(ICON_BUTTON_SIZE), GUILayout.Height(ICON_BUTTON_SIZE));
        buttonRect.y += buttonRect.height + 4;
        EditorGUI.DrawPreviewTexture(buttonRect, tex);
        EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsHelper.CompleteQQQ(qqq);
        }
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
#endif