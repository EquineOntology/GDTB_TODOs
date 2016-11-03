using UnityEditor;
using UnityEngine;

namespace com.immortalhydra.gdtb.codetodos
{
    public static class DrawingUtils
    {
        /// Draw the button, based on the type, not pressed.
        public static void DrawButton(Rect aRect, string aText, GUIStyle aStyle)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            var bgRect = new Rect(aRect.x + Constants.BUTTON_BORDER_THICKNESS, aRect.y + Constants.BUTTON_BORDER_THICKNESS, aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2, aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2);
            EditorGUI.DrawRect(bgRect, Preferences.Color_Primary);
            GUI.Label(new Rect(bgRect.x, bgRect.y - 1, bgRect.width, bgRect.height), aText, aStyle);
            aStyle.normal.textColor = Preferences.Color_Tertiary;
        }


        /// Draw "fake" text button in the pressed state.
        public static void DrawPressedButton(Rect aRect, string aText, GUIStyle aStyle)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            aStyle.normal.textColor = Preferences.Color_Primary;
            GUI.Label(new Rect(aRect.x, aRect.y - 1, aRect.width, aRect.height), aText, aStyle);
        }


        /// Draw custom selectionGrid.
        public static void DrawSelectionGrid(Rect aRect, string[] anElementArray, int aSelectedIndex, float anHorizontalSize, float aSpace, GUIStyle aNormalStyle, GUIStyle aPressedStyle)
        {
            var x = aRect.x;
            for (var i = 0; i < anElementArray.Length; i++)
            {
                var gridRect = aRect;
                x = i == 0 ? x : x + anHorizontalSize + aSpace; // Add parameters to horizontal pos., but not for the first rect.
                gridRect.x = x;
                gridRect.width = anHorizontalSize;

                if (i != aSelectedIndex)
                {
                    DrawButton(gridRect, anElementArray[i], aNormalStyle);
                }
                else
                {
                    DrawPressedButton(gridRect, anElementArray[i], aPressedStyle);
                }
            }
        }
    }
}
