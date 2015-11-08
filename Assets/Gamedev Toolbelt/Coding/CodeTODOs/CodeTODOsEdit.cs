#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CodeTODOsEdit : EditorWindow
{
    private GUISkin _GDTBSkin;
    private string _skinPath;

    private static QQQ _oldQQQ;
    private static QQQ _newQQQ;

    private string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };

    public static void Init(QQQ qqq)
    {
        // Get existing open window or if none, make a new one.
        CodeTODOsEdit window = (CodeTODOsEdit)EditorWindow.GetWindow(typeof(CodeTODOsEdit));
        //window.minSize = new Vector2(EDITOR_WINDOW_MINSIZE_X, EDITOR_WINDOW_MINSIZE_Y);
        window.titleContent = new GUIContent(GUIConstants.TEXT_EDIT_WINDOW_TITLE);
        _oldQQQ = qqq;
        _newQQQ = new QQQ((int)qqq.Priority, qqq.Task, qqq.Script, qqq.LineNumber);
        window.Show();
    }

    public void OnEnable()
    {
        _GDTBSkin = Resources.Load(GUIConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
    }

    private void OnGUI()
    {
        GUI.skin = _GDTBSkin;
        DrawPriority();
        DrawTask();
        DrawButton();
    }

    // Draw the priority enum.
    private void DrawPriority()
    {
        var priorityIndex = (int)_oldQQQ.Priority;
        var popupRect = new Rect(10, 10, 60, 10);
        _newQQQ.Priority = (QQQPriority)EditorGUI.Popup(popupRect, priorityIndex, _qqqPriorities);
    }

    // Draw the textfield that enables the user to modify the QQQ's task.
    private void DrawTask()
    {
        // The "Task:" label.
        var labelRect = EditorGUILayout.GetControlRect();
        labelRect.width = 40;
        labelRect.x = 80;
        labelRect.y = 10;
        EditorGUI.LabelField(labelRect, "Task:", EditorStyles.boldLabel);

        // The task itself.
        var fieldRect = EditorGUILayout.GetControlRect();
        fieldRect.x = 130;
        fieldRect.y = 10;
        fieldRect.width = fieldRect.width - fieldRect.x - 10;
        _newQQQ.Task = EditorGUI.TextField(fieldRect, _newQQQ.Task);
    }

    // Draw "Save" button;
    private void DrawButton()
    {
        var buttonRect = new Rect((Screen.width / 2) - 25, 40, 50, 20);
        if(GUI.Button(buttonRect, "Save"))
        {
            CodeTODOsHelper.UpdateTask(_oldQQQ, _newQQQ);
            EditorWindow.GetWindow(typeof(CodeTODOsEdit)).Close();
        }
    }
}
#endif