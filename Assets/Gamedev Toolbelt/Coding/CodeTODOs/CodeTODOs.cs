#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CodeTODOs : EditorWindow
{
    public static List<QQQ> QQQs = new List<QQQ>();
    private GUISkin _gdtbSkin;
    private GUIStyle _priorityStyle, _taskStyle, _scriptStyle;

    // ========================= Editor layouting =========================
    private const int IconSize = 16;

    private int _unit, _priorityWidth, _qqqWidth, _editAndDoneWidth;
    private int _helpBoxOffset = 5;

    private int _priorityLabelWidth;

    private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
    private Rect _scrollRect, _scrollViewRect, _qqqRect, _priorityRect, _rightButtonsRect;

    // ====================================================================
    [MenuItem("Window/CodeTODOs %q")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one.
        var window = (CodeTODOs)EditorWindow.GetWindow(typeof(CodeTODOs));
        window.titleContent = new GUIContent(GUIConstants.TEXT_WINDOW_TITLE);

        window.UpdateLayoutingSizes();
        window._priorityLabelWidth = (int)window._priorityStyle.CalcSize(new GUIContent("URGENT")).x; // Not with the other layouting sizes because it only needs to be done once.

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
        LoadStyles();
    }


    private void OnGUI()
    {
        UpdateLayoutingSizes();
        GUI.skin = _gdtbSkin;

        DrawQQQs();

        DrawAddButton();
        DrawRefreshButton();
        DrawSettingsButton();
    }


    /// Draw the list of QQQs.
    private void DrawQQQs()
    {
        _scrollPosition = GUI.BeginScrollView(_scrollRect, _scrollPosition, _scrollViewRect);
        var heightIndex = _helpBoxOffset;
        for (var i = 0; i < QQQs.Count; i++)
        {
            var taskContent = new GUIContent(QQQs[i].Task);
            var taskHeight = _taskStyle.CalcHeight(taskContent, _qqqWidth);

            var helpBoxHeight = taskHeight + GUIConstants.LINE_HEIGHT + 5;
            helpBoxHeight = helpBoxHeight < IconSize * 2.5f ? IconSize * 2.5f : helpBoxHeight;

            _qqqRect = new Rect(_priorityWidth, heightIndex, _qqqWidth, helpBoxHeight);
            _priorityRect = new Rect(0, _qqqRect.y, _priorityWidth, helpBoxHeight);
            _rightButtonsRect = new Rect(_priorityWidth + _qqqWidth + (_helpBoxOffset * 2), _qqqRect.y, _editAndDoneWidth, helpBoxHeight);

            var helpBoxRect = _priorityRect;
            helpBoxRect.height = helpBoxHeight;
            helpBoxRect.width = position.width - (_helpBoxOffset * 2) - IconSize;
            helpBoxRect.x += _helpBoxOffset;

            heightIndex += (int)helpBoxHeight + _helpBoxOffset;
            _scrollViewRect.height = heightIndex;

            DrawHelpBox(helpBoxRect);
            DrawPriority(_priorityRect, QQQs[i]);
            DrawTaskAndScriptLabels(_qqqRect, QQQs[i], taskHeight);
            DrawEditAndCompleteButtons(_rightButtonsRect, QQQs[i]);
        }
        GUI.EndScrollView();
    }


    #region QQQPriorityMethods

    /// Select which priority format to use based on the user preference.
    private void DrawPriority(Rect aRect, QQQ aQQQ)
    {
        switch (CodeTODOsPrefs.QQQPriorityDisplay)
        {
            case PriorityDisplayFormat.TEXT_ONLY:
                DrawPriorityText(aRect, aQQQ);
                break;
            case PriorityDisplayFormat.ICON_ONLY:
                DrawPriorityIcon(aRect, aQQQ);
                break;
            case PriorityDisplayFormat.ICON_AND_TEXT:
                DrawPriorityIconAndText(aRect, aQQQ);
                break;
        }
    }


    /// Draw priority for the "Icon only" setting.
    private void DrawPriorityIcon(Rect aRect, QQQ aQQQ)
    {
        // Prepare the rectangle for layouting. The layout is "space-icon-space".
        var priorityRect = aRect;
        var newY = 0;
        var newX = 0;
        if (aRect.width > IconSize + (_helpBoxOffset * 2))
        {
            newX = (int)priorityRect.x + IconSize / 2 + _helpBoxOffset;
            newY = (int)priorityRect.y + IconSize / 2 + _helpBoxOffset;
        }
        else
        {
            newX = (int)priorityRect.x + IconSize / 2;
            newY = (int)priorityRect.y + IconSize / 2;
        }

        priorityRect.width = IconSize;
        priorityRect.height = IconSize;
        priorityRect.position = new Vector2(newX, newY);

        Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
        EditorGUI.DrawPreviewTexture(priorityRect, tex);
    }


    /// Draw priority for the "Text only" setting.
    private void DrawPriorityText(Rect aRect, QQQ aQQQ)
    {
        var priorityRect = aRect;
        priorityRect.height -= (IconSize / 2 + _helpBoxOffset);

        var newX = (int)priorityRect.x + _helpBoxOffset;
        var newY = (int)priorityRect.y + _helpBoxOffset;
        priorityRect.position = new Vector2(newX, newY);

        EditorGUI.LabelField(priorityRect, aQQQ.Priority.ToString());
    }


    /// Draw priority for the "Icon and Text" setting.
    private void DrawPriorityIconAndText(Rect aRect, QQQ aQQQ)
    {
        // Draw the Icon.
        var iconRect = aRect;
        iconRect.width = IconSize;
        iconRect.height = IconSize;

        var iconNewY = iconRect.y + _helpBoxOffset;
        var iconNewX = iconRect.x + Mathf.Clamp(_unit, 1, IconSize + (int)(_priorityLabelWidth / 2));
        iconRect.position = new Vector2(iconNewX, iconNewY);

        Texture2D tex = GetQQQPriorityTexture((int)aQQQ.Priority);
        EditorGUI.DrawPreviewTexture(iconRect, tex);

        // Draw the label.
        var labelRect = aRect;
        labelRect.width = Mathf.Clamp((_unit * 2) + IconSize, 1, (IconSize * 2) + _priorityLabelWidth);

        var labelNewX = labelRect.x + _helpBoxOffset;
        var labelNewY = (int)(iconRect.y + iconRect.height);
        labelRect.position = new Vector2(labelNewX, labelNewY);

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
    private void DrawTaskAndScriptLabels(Rect aRect, QQQ aQQQ, float aHeight)
    {
        // Task.
        var taskRect = aRect;
        taskRect.x = _priorityWidth;
        taskRect.y += _helpBoxOffset;
        taskRect.height = aHeight;
        EditorGUI.LabelField(taskRect, aQQQ.Task, _taskStyle);

        // Script.
        var scriptRect = aRect;
        scriptRect.x = _priorityWidth;
        scriptRect.y += (taskRect.height + 5);
        scriptRect.height = GUIConstants.LINE_HEIGHT;

        var scriptLabel = CodeTODOsHelper.FormatScriptLabel(aQQQ, scriptRect.width, _scriptStyle);
        var scriptContent = new GUIContent(scriptLabel);
        scriptRect.width = _scriptStyle.CalcSize(scriptContent).x;
        EditorGUI.LabelField(scriptRect, scriptLabel, _scriptStyle);

        // Open editor on click.
        EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && scriptRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsHelper.OpenScript(aQQQ);
        }
    }


    /// Draw the "Help box" style rectangle that separates the QQQs visually.
    private void DrawHelpBox(Rect aRect)
    {
        EditorGUI.LabelField(aRect, "", EditorStyles.helpBox);
    }


    /// Draw the "Edit" and "Complete" buttons.
    private void DrawEditAndCompleteButtons(Rect aRect, QQQ aQQQ)
    {
        // "Edit" button.
        var editRect = aRect;
        editRect.x = position.width - (IconSize * 2) - (_helpBoxOffset * 2);
        editRect.y += 3;
        editRect.width = IconSize;
        editRect.height = IconSize;

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


    /// Draw the "Refresh" button.
    private void DrawRefreshButton()
    {
        // "Refresh" button.
        var refreshRect = new Rect((position.width / 2) - (IconSize * 0.5f), position.height - (IconSize * 1.5f), IconSize, IconSize);
        if (GUI.Button(refreshRect, GUIConstants.TEXT_REFRESH_LIST))
        {
            QQQs.Clear();
            CodeTODOsHelper.GetQQQsFromAllScripts();
            CodeTODOsHelper.ReorderQQQs();
        }
    }


    /// Draw the "Add" button.
    private void DrawAddButton()
    {
        // "Add" button.
        var addRect = new Rect((position.width / 2) - (IconSize * 1.5f) - _helpBoxOffset, position.height - (IconSize * 1.5f), IconSize, IconSize);

        var addTex = Resources.Load(GUIConstants.FILE_QQQ_ADD, typeof(Texture2D)) as Texture2D;
        EditorGUI.DrawPreviewTexture(addRect, addTex);

        // Add QQQ on click.
        EditorGUIUtility.AddCursorRect(addRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && addRect.Contains(Event.current.mousePosition))
        {
            CodeTODOsAdd.Init();
        }

    }


    /// Draw the "Settings" button.
    private void DrawSettingsButton()
    {
        // "Settings" button.
        var settingsRect = new Rect((position.width / 2) + (IconSize * 1.5f) + _helpBoxOffset, position.height - (IconSize * 1.5f), IconSize, IconSize);

        var settingsTex = Resources.Load(GUIConstants.FILE_SETTINGS, typeof(Texture2D)) as Texture2D;
        EditorGUI.DrawPreviewTexture(settingsRect, settingsTex);

        // Open settings on click.
        EditorGUIUtility.AddCursorRect(settingsRect, MouseCursor.Link);
        if (Event.current.type == EventType.MouseUp && settingsRect.Contains(Event.current.mousePosition))
        {
            // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
            var asm = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
            var T = asm.GetType("UnityEditor.PreferencesWindow");
            var M = T.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            M.Invoke(null, null);
        }
    }


    /// Update sizes used in layouting based on the window size.
    private void UpdateLayoutingSizes()
    {
        var width = position.width - IconSize;

        _scrollRect = new Rect(_helpBoxOffset, _helpBoxOffset, width - (_helpBoxOffset * 2), position.height - IconSize * 3);

        _scrollViewRect = _scrollRect;

        _unit = (int)(width / 28) == 0 ? 1 : (int)(width / 28); // If the unit would be 0, set it to 1.

        // Priority rect width has different size based on preferences.
        if (CodeTODOsPrefs.QQQPriorityDisplay.ToString() == "ICON_ONLY")
        {
            _priorityWidth = Mathf.Clamp((_unit * 2) + IconSize, IconSize, (IconSize + _helpBoxOffset) * 2);
        }
        else
        {
            _priorityWidth = Mathf.Clamp((_unit * 2) + IconSize, IconSize, (IconSize * 2) + _priorityLabelWidth);
        }

        _editAndDoneWidth = (IconSize * 2) + 5;
        _qqqWidth = (int)width - _priorityWidth - _editAndDoneWidth - (_helpBoxOffset * 2);
    }


    /// Load the CodeTODOs skin.
    private void LoadSkin()
    {
        _gdtbSkin = Resources.Load(GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
    }


    /// Assign the GUI Styles
    private void LoadStyles()
    {
        _priorityStyle = _gdtbSkin.GetStyle("label");
        _taskStyle = _gdtbSkin.GetStyle("task");
        _scriptStyle = _gdtbSkin.GetStyle("script");
    }
}
#endif