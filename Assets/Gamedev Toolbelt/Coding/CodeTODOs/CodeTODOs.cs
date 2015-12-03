#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
{
    public static List<QQQ> QQQs = new List<QQQ>();
    private GUISkin _GDTBSkin;
    private GUIStyle _labelStyle;

    // ========================= Editor layouting =========================
    private const int BUTTON_WIDTH = 150;
    private const int ICON_SIZE = 16;

    private int _unit, _qqqWidth, _priorityWidth, _editAndDoneWidth;
    private int _helpBoxOffset = 5;

    private int _priorityLabelWidth;

    // ======================= Class functionality ========================
    [MenuItem("Window/CodeTODOs %q")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        CodeTODOs window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.titleContent = new GUIContent(GUIConstants.TEXT_WINDOW_TITLE);

        window.UpdateLayoutingSizes(window.position.width);
        window._priorityLabelWidth = (int)window._labelStyle.CalcSize(new GUIContent("URGENT")).x; // Not with the other layouting sizes because it only needs to be done once.

        if (QQQs.Count == 0)
        {
            CodeTODOsHelper.GetQQQsFromAllScripts();
            CodeTODOsHelper.ReorderQQQs();
        }

        window.Show();
    }


    public void OnEnable()
    {
        LoadSkin();
    }


    private void OnGUI()
    {
        UpdateLayoutingSizes(position.width);
        GUI.skin = _GDTBSkin;

        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        DrawQQQs();
        EditorGUILayout.Space();
        DrawListButton();
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
    }


    /// Draw the list of QQQs.
    private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
    private Rect _qqqRect, _priorityRect, _rightButtonsRect;
    private void DrawQQQs()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, _GDTBSkin.scrollView);
        var heightIndex = _helpBoxOffset;
        for (int i = 0; i < QQQs.Count; i++)
        {
            var helpBoxHeight = CalculateNumberOfLines(QQQs[i].Task, _qqqWidth) * GUIConstants.LINE_HEIGHT + GUIConstants.LINE_HEIGHT;
            helpBoxHeight = helpBoxHeight < ICON_SIZE * 2 ? ICON_SIZE * 2.5f : helpBoxHeight; // Minimum vertical size is ICON_SIZE * 2.5f.

            _qqqRect = new Rect(_priorityWidth, heightIndex, _qqqWidth, helpBoxHeight);
            //EditorGUI.DrawRect(_qqqRect, Color.green);
            _priorityRect = new Rect(0, _qqqRect.y, _priorityWidth, helpBoxHeight);
            //EditorGUI.DrawRect(_priorityRect, Color.red);
            _rightButtonsRect = new Rect(_priorityWidth + _qqqWidth, _qqqRect.y, _editAndDoneWidth, helpBoxHeight);
            //EditorGUI.DrawRect(_rightButtonsRect, Color.blue);

            var _helpBoxRect = _priorityRect;
            _helpBoxRect.width = position.width - Mathf.Clamp(_unit * 2, ICON_SIZE, (ICON_SIZE * 2) + _helpBoxOffset);
            _helpBoxRect.x += 3;
            DrawHelpBox(_helpBoxRect);

            DrawPriority(_priorityRect, QQQs[i]);
            DrawTaskAndScriptLabels(_qqqRect, QQQs[i]);
            DrawEditAndCompleteButtons(_rightButtonsRect, QQQs[i]);

            heightIndex += (int)helpBoxHeight + _helpBoxOffset;
        }
        EditorGUILayout.EndScrollView();
    }


    #region QQQPriorityMethods
    /// Select which priority format to use based on the user preference.
    private void DrawPriority(Rect aRect, QQQ aQQQ)
    {
        if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.TEXT_ONLY)
        {
            DrawPriorityText(aRect, aQQQ);
        }
        else if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.ICON_ONLY)
        {
            DrawPriorityIcon(aRect, aQQQ);
        }
        else if (CodeTODOsPrefs.QQQPriorityDisplay == PriorityDisplayFormat.ICON_AND_TEXT)
        {
            DrawPriorityIconAndText(aRect, aQQQ);
        }
    }


    /// Draw priority for the "Icon only" setting.
    private void DrawPriorityIcon(Rect aRect, QQQ aQQQ)
    {
        // Prepare the rectangle for layouting. The layout is "space-icon-space".
        var priorityRect = aRect;
        var newY = 0;
        var newX = 0;
        if (aRect.width > ICON_SIZE + (_helpBoxOffset * 2))
        {
            newX = (int)priorityRect.x + ICON_SIZE / 2 + _helpBoxOffset;
            newY = (int)priorityRect.y + ICON_SIZE / 2 + _helpBoxOffset;
        }
        else
        {
            newX = (int)priorityRect.x + ICON_SIZE / 2;
            newY = (int)priorityRect.y + ICON_SIZE / 2;
        }

        priorityRect.width = ICON_SIZE;
        priorityRect.height = ICON_SIZE;
        priorityRect.position = new Vector2(newX, newY);

        Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
        EditorGUI.DrawPreviewTexture(priorityRect, tex);

        //EditorGUI.DrawPreviewTexture(priorityRect, EditorGUIUtility.whiteTexture);
    }


    /// Draw priority for the "Text only" setting.
    private void DrawPriorityText(Rect aRect, QQQ aQQQ)
    {
        var priorityRect = aRect;
        priorityRect.height -= (ICON_SIZE / 2 + _helpBoxOffset);

        var newX = (int)priorityRect.x + _helpBoxOffset;
        var newY = (int)priorityRect.y + _helpBoxOffset;
        priorityRect.position = new Vector2(newX, newY);

        //EditorGUI.DrawPreviewTexture(priorityRect, EditorGUIUtility.whiteTexture);
        EditorGUI.LabelField(priorityRect, aQQQ.Priority.ToString());
    }


    /// Draw priority for the "Icon and Text" setting.
    private void DrawPriorityIconAndText(Rect aRect, QQQ aQQQ)
    {
        //EditorGUI.DrawPreviewTexture(aRect, EditorGUIUtility.whiteTexture);

        // Draw the Icon.
        var iconRect = aRect;
        iconRect.width = ICON_SIZE;
        iconRect.height = ICON_SIZE;

        var iconNewY = iconRect.y + _helpBoxOffset;
        var iconNewX = iconRect.x + Mathf.Clamp(_unit, 1, ICON_SIZE + (int)(_priorityLabelWidth / 2));
        iconRect.position = new Vector2(iconNewX, iconNewY);

        Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
        EditorGUI.DrawPreviewTexture(iconRect, tex);
        //EditorGUI.DrawPreviewTexture(iconRect, EditorGUIUtility.whiteTexture);

        // Draw the label.
        var labelRect = aRect;
        labelRect.width = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, (ICON_SIZE * 2) + _priorityLabelWidth);

        var labelNewX = labelRect.x + _helpBoxOffset;
        var labelNewY = (int)(iconRect.y + iconRect.height);
        labelRect.position = new Vector2(labelNewX, labelNewY);

        EditorGUI.LabelField(labelRect, aQQQ.Priority.ToString());
        //EditorGUI.DrawPreviewTexture(labelRect, EditorGUIUtility.whiteTexture);
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
    private void DrawTaskAndScriptLabels(Rect aRect, QQQ aQQQ)
    {
        var labelsRect = aRect;// EditorGUILayout.GetControlRect(GUILayout.Width(_qqqWidth));
        labelsRect.x = _priorityWidth;
        var labels = CodeTODOsHelper.FormatTaskAndScriptLabels(aQQQ, _qqqWidth);

        // Task.
        var taskWidth = aQQQ.Task.Length * GUIConstants.BOLD_CHAR_WIDTH;
        var charsInLine = _qqqWidth / GUIConstants.BOLD_CHAR_WIDTH;
        var taskHeight = (aQQQ.Task.Length / charsInLine) * GUIConstants.LINE_HEIGHT;

        var taskRect = labelsRect;
        taskRect.height = taskHeight;

        EditorGUI.LabelField(taskRect, labels[0], EditorStyles.boldLabel);

        //EditorGUI.DrawPreviewTexture(taskRect, EditorGUIUtility.whiteTexture);

        // Script.
        var scriptRect = taskRect;
        scriptRect.height = GUIConstants.LINE_HEIGHT;
        scriptRect.y = scriptRect.y + taskRect.height + 5;

        GUIStyle link = new GUIStyle(EditorStyles.label); // "Blue link" style
        link.normal.textColor = Color.blue;

        // Open editor on click.
        EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && scriptRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsHelper.OpenScript(aQQQ);
        }
        EditorGUI.LabelField(scriptRect, labels[1], link);

        var helpBoxRect = labelsRect;
        helpBoxRect.height = taskRect.height + 5 + scriptRect.height;
        helpBoxRect.width = _qqqWidth + _priorityWidth + _editAndDoneWidth;
    }


    /// Draw the "Help box" style rectangle that separates the QQQs visually.
    private void DrawHelpBox(Rect aRect)
    {
        EditorGUI.LabelField(aRect, "", EditorStyles.helpBox);
    }


    /// Draw the "Edit" and "Complete" buttons.
    private void DrawEditAndCompleteButtons(Rect aRect, QQQ aQQQ)
    {
        var maxRectWidth = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, ICON_SIZE * 3);
        var buttonsRect = EditorGUILayout.GetControlRect(GUILayout.Width(maxRectWidth));
        //EditorGUI.DrawPreviewTexture(buttonsRect, EditorGUIUtility.whiteTexture);

        // "Edit" button.
        var editRect = aRect;
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
        _unit = (int)(aWidth / 28) == 0 ? 1 : (int)(aWidth / 28); // If the unit would be 0, set it to 1.
        _priorityWidth = Mathf.Clamp((_unit * 2) + ICON_SIZE, 1, (ICON_SIZE * 2) + _priorityLabelWidth);
        _editAndDoneWidth = Mathf.Clamp((_unit * 2) + ICON_SIZE + 5, 1, (ICON_SIZE * 3) + 5);
        _qqqWidth = (int)aWidth - _priorityWidth - _editAndDoneWidth; // Size of this is "everything else", i.e. whatever is left after the other elements.
    }


    /// Load the CodeTODOs skin.
    private void LoadSkin()
    {
        _GDTBSkin = Resources.Load(GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        _labelStyle = _GDTBSkin.GetStyle("label");
    }


    /// Calculate how many lines a task will fill given a max width.
    private int CalculateNumberOfLines(string aString, int aMaxWidth)
    {
        var charactersInLine = aMaxWidth / GUIConstants.BOLD_CHAR_WIDTH;
        return aString.Length / charactersInLine;
    }
}
#endif