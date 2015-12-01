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
    private const int ICON_SIZE = 16;

    private int _unit, _qqqWidth;

    // ====================================================================
    // ======================= Class functionality ========================
    // ====================================================================

    [MenuItem("Window/CodeTODOs")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.titleContent = new GUIContent(GUIConstants.TEXT_WINDOW_TITLE);
        window.UpdateLayoutingSizes(window.position.width);
        window.Show();
    }


    public void OnEnable()
    {
        UpdateLayoutingSizes(position.width);
        LoadSkin();
        if (QQQs.Count == 0)
        {
            CodeTODOsHelper.GetQQQsFromAllScripts();
            CodeTODOsHelper.ReorderQQQs();
        }
    }


    private void OnGUI()
    {
        UpdateLayoutingSizes(position.width);
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


    /// Draw the list of QQQs.
    private Vector2 _scrollPosition = new Vector2(Screen.width - (Screen.width/28), Screen.height);
    private void DrawQQQs()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, _GDTBSkin.scrollView);
        for (int i = 0; i < QQQs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            DrawPriority(QQQs[i]);
            DrawTaskAndScriptLabels(QQQs[i]);
            DrawEditAndCompleteButtons(QQQs[i]);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }


    #region QQQPriorityMethods
    /// Select which priority format to use based on the user preference.
    private void DrawPriority(QQQ aQQQ)
    {
        if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.TEXT_ONLY)
        {
            DrawPriorityText(aQQQ);
        }
        else if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.ICON_ONLY)
        {
            DrawPriorityIcon(aQQQ);
        }
        else if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.ICON_AND_TEXT)
        {
            DrawPriorityIconAndText(aQQQ);
        }
    }


    /// Draw priority for the "Icon only" setting.
    private void DrawPriorityIcon(QQQ aQQQ)
    {
        // Prepare the rectangle for layouting. The layout is "space-icon-space".
        var priorityRect = EditorGUILayout.GetControlRect();
        var newY = priorityRect.y + (int)(priorityRect.height/2); // Center vertically.
        var newX = priorityRect.x + Mathf.Clamp(_unit/2, 1, ICON_SIZE); // Increase horizontal margin, but only to ICON_SIZE.

        priorityRect.width = ICON_SIZE;
        priorityRect.height = ICON_SIZE;
        priorityRect.position = new Vector2(newX, newY);

        Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
        EditorGUI.DrawPreviewTexture(priorityRect, tex);

        //EditorGUI.DrawRect(priorityRect, Color.green);
    }


    /// Draw priority for the "Text only" setting.
    int _priorityLabelWidth = "URGENT".Length * GUIConstants.NORMAL_CHAR_WIDTH;
    private void DrawPriorityText(QQQ aQQQ)
    {
        // Prepare the rectangle for layouting.
        var priorityRect = EditorGUILayout.GetControlRect();
        priorityRect.width = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, (ICON_SIZE * 2) + _priorityLabelWidth);
        var newY = priorityRect.y + (int)(priorityRect.height/2);

        // Only increase margin when the rectangle gets bigger than the label itself.
        var newX = priorityRect.x;
        if (_priorityLabelWidth < priorityRect.width)
        {
            newX = newX + Mathf.Clamp(_unit / 2, 1, ICON_SIZE);
        }
        priorityRect.position = new Vector2(newX, newY);

        //EditorGUI.DrawRect(priorityRect, Color.green);

        EditorGUI.LabelField(priorityRect, aQQQ.Priority.ToString());
    }


    /// Draw priority for the "Icon and Text" setting.
    private void DrawPriorityIconAndText(QQQ aQQQ)
    {
        var priorityRect = EditorGUILayout.GetControlRect();
        priorityRect.width = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, (ICON_SIZE * 2) + _priorityLabelWidth);

        // Draw the Icon.
        var iconRect = priorityRect;
        var newYIcon = iconRect.y + (int)(iconRect.height / 5); // Center at 1/4th vertically.
        var newXIcon = iconRect.x + Mathf.Clamp(_unit, 1, ICON_SIZE + (int)(_priorityLabelWidth / 2));

        iconRect.width = ICON_SIZE;
        iconRect.height = ICON_SIZE;
        iconRect.position = new Vector2(newXIcon, newYIcon);

        Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
        EditorGUI.DrawPreviewTexture(iconRect, tex);

        // Draw the text.
        var labelRect = priorityRect;
        labelRect.width = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, (ICON_SIZE * 2) + _priorityLabelWidth);
        var newYLabel = newYIcon + ICON_SIZE + (int)(labelRect.height / 4);
        var newXLabel = labelRect.x;

        if (_priorityLabelWidth < labelRect.width)
        {
            newXLabel = newXLabel + Mathf.Clamp(_unit / 2, 1, ICON_SIZE);
        }
        labelRect.position = new Vector2(newXLabel, newYLabel);
        EditorGUI.LabelField(labelRect, aQQQ.Priority.ToString());
    }


    /// Get the correct texture for a priority.
    private Texture2D GetQQQPriorityTexture(int aPriority)
    {
        Texture2D tex;
        switch (aPriority)
        {
            case 1:
                tex = Resources.Load<Texture2D>(GUIConstants.FILE_QQQ_URGENT);
                break;
            case 3:
                tex = Resources.Load<Texture2D>(GUIConstants.FILE_QQQ_MINOR);
                break;
            case 2:
            default:
                tex = Resources.Load<Texture2D>(GUIConstants.FILE_QQQ_NORMAL);
                break;
        }
        return tex;
    }
    #endregion


    /// Draws the "Task" and "Script" texts for QQQs.
    private void DrawTaskAndScriptLabels(QQQ aQQQ)//, float aTaskHeight, float aScriptHeight)
    {
        var windowWidth = EditorWindow.GetWindow<CodeTODOs>().position.width;
        var labels = CodeTODOsHelper.FormatTaskAndScriptLabels(aQQQ, windowWidth / 2);
        var taskHeight = CodeTODOsHelper.CalculateHeightOfString(aQQQ.Task, windowWidth, true);
        //Debug.Log(taskHeight);

        var taskWidth = aQQQ.Task.Length * GUIConstants.BOLD_CHAR_WIDTH;
        // To calculate the actual size of a script, we multiply the sum of the script characters,
        //the "fixed" ones (Line XXX in ""), and the the characters in lineNumber for the max width of a character in pixels.
        var scriptWidth = (aQQQ.Script.Length + 11 + aQQQ.LineNumber.ToString().Length) * GUIConstants.NORMAL_CHAR_WIDTH;
        //Debug.Log(aQQQ.Script.Length);

        var labelWidth = taskWidth > scriptWidth ? taskWidth : scriptWidth;

        EditorGUILayout.BeginVertical();

        var taskLabelRect = EditorGUILayout.GetControlRect(GUILayout.Height(taskHeight), GUILayout.Width(labelWidth));
        EditorGUI.LabelField(taskLabelRect, labels[0], EditorStyles.boldLabel);

        GUIStyle link = new GUIStyle(EditorStyles.label);
        link.normal.textColor = Color.blue;

        // Open editor on click.
        var scriptLabelRect = EditorGUILayout.GetControlRect(GUILayout.Height(GUIConstants.CHAR_HEIGHT * GUIConstants.VERTICAL_SPACING), GUILayout.Width(labelWidth));
        EditorGUIUtility.AddCursorRect(scriptLabelRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && scriptLabelRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsHelper.OpenScript(aQQQ);
        }
        EditorGUI.LabelField(scriptLabelRect, labels[1], link);

        EditorGUILayout.EndVertical();
    }


    /// Draw the "Edit" and "Complete" buttons.
    private void DrawEditAndCompleteButtons(QQQ aQQQ)
    {
        var maxRectWidth = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, ICON_SIZE * 3);
        var buttonsRect = EditorGUILayout.GetControlRect(GUILayout.Width(maxRectWidth));
        //EditorGUI.DrawRect(buttonsRect, Color.red);

        // "Edit" button.
        var editRect = buttonsRect;
        editRect.x = position.width - ICON_SIZE - Mathf.Clamp(_unit, 1, ICON_SIZE);
        editRect.y += 1;
        editRect.width = ICON_SIZE;
        editRect.height = ICON_SIZE;

        var editTex = Resources.Load(GUIConstants.FILE_QQQ_EDIT, typeof(Texture2D)) as Texture2D;
        EditorGUI.DrawPreviewTexture(editRect, editTex);

        // Open Edit window on Click.
        EditorGUIUtility.AddCursorRect(editRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && editRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsEdit.Init(aQQQ);
        }

        // "Complete" button.
        var completeRect = editRect;
        completeRect.y = editRect.y + editRect.height + 2;

        var completeTex = Resources.Load(GUIConstants.FILE_QQQ_DONE, typeof(Texture2D)) as Texture2D;
        EditorGUI.DrawPreviewTexture(completeRect, completeTex);

        // Complete QQQ on click.
        EditorGUIUtility.AddCursorRect(completeRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && completeRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsHelper.CompleteQQQ(aQQQ);
        }
    }


    /// Draw the "Refresh list" button.
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


    /// Update sizes used in layouting based on the window size.
    private void UpdateLayoutingSizes(float aWidth)
    {
        _unit = (int)(aWidth / 28) == 0? 1 : (int)(aWidth/28); // If the unit would be 0, set it to 1.
        _qqqWidth = (int)aWidth - (_unit * 5) - (ICON_SIZE * 2); // Size of this is "everything else", i.e. whatever is left after the other elements.
    }


    /// Load the CodeTODOs skin.
    private void LoadSkin()
    {
        _GDTBSkin = Resources.Load(GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
    }
}
#endif