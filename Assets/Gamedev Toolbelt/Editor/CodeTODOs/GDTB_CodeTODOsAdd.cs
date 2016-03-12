#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class GDTB_CodeTODOsAdd : EditorWindow
{
    private GUISkin _GDTBSkin;

    private string[] _qqqPriorities = { "Urgent", "Normal", "Minor" };

    private string _task;
    private MonoScript _script;
    private int _priority = 2;
    private int _lineNumber = 0;

    private GUISkin _defaultSkin;


    public static void Init()
    {
        GDTB_CodeTODOsAdd window = (GDTB_CodeTODOsAdd)EditorWindow.GetWindow(typeof(GDTB_CodeTODOsAdd));
        window.ShowUtility();
    }

    public void OnEnable()
    {
        _defaultSkin = GUI.skin;
		_GDTBSkin = Resources.Load(GDTB_CodeTODOsConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        _script = new MonoScript();
    }

    public void OnGUI()
    {
        DrawScriptPicker();
        DrawTaskField();
        DrawPriorityPopup();
        DrawLineField();
        DrawButton();
    }


    /// Draw script picker.
    private void DrawScriptPicker()
    {
        var labelRect = new Rect(10, 10, 100, 16);
        EditorGUI.LabelField(labelRect, "Pick a script:", EditorStyles.boldLabel);

        var pickerRect = new Rect(10, 28, Mathf.Clamp(position.width - 20, 80, 500), 16);
        _script = (MonoScript)EditorGUI.ObjectField(pickerRect, _script, typeof(MonoScript), false);
    }


    /// Draw Task input field.
    private void DrawTaskField()
    {
        var labelRect = new Rect(10, 53, 100, 16);
        EditorGUI.LabelField(labelRect, "Write a task:", EditorStyles.boldLabel);

        var taskRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, 500), 32);
        _task = EditorGUI.TextField(taskRect, _task);
    }


    /// Draw priority popup.
    private void DrawPriorityPopup()
    {
        var labelRect = new Rect(10, 112, 150, 16);
        EditorGUI.LabelField(labelRect, "Choose a priority:", EditorStyles.boldLabel);

        var priorityRect = new Rect(10, 130, 70, 16);
        _priority = EditorGUI.Popup(priorityRect, _priority - 1, _qqqPriorities) + 1;
    }


    /// Draw line number field.
    private void DrawLineField()
    {
        var labelRect = new Rect(10, 155, 200, 32);
        EditorGUI.LabelField(labelRect, "Choose the line number:", EditorStyles.boldLabel);

        var lineRect = new Rect(10, 176, Mathf.Clamp(position.width - 20, 80, 500), 16);
        _lineNumber = EditorGUI.IntField(lineRect, _lineNumber);

        if (_lineNumber < 0)
        {
            _lineNumber = 0;
        }
    }


    /// Draw "Add task" button.
    private void DrawButton()
    {
        GUI.skin = _GDTBSkin;

        var buttonRect = new Rect((Screen.width / 2) - 37, 210, 74, 20);

        if (GUI.Button(buttonRect, "Add task"))
        {
            if (_script.name == "")
            {
                EditorUtility.DisplayDialog("No script selected", "Please select a script.", "Ok");
            }
            else if (_task == "")
            {
                EditorUtility.DisplayDialog("No task to add", "Please create a task.", "Ok");
            }
            else
            {
                if (EditorUtility.DisplayDialog("Add task?", "Are you sure you want to add this task to the specified script?", "Add task", "Cancel"))
                {
                    var path = AssetDatabase.GetAssetPath(_script);
                    var newQQQ = new GDTB_QQQ(_priority, _task, path, _lineNumber);
                    GDTB_CodeTODOsHelper.AddQQQ(newQQQ);
                    EditorWindow.GetWindow(typeof(GDTB_CodeTODOsAdd)).Close();
                }
            }
        }
        GUI.skin = _defaultSkin;
    }
}
#endif