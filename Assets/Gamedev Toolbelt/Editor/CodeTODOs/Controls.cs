using UnityEngine;
using UnityEditor;
using System.Collections;

namespace com.immortalhydra.gdtb.codetodos
{
	public static class Controls
	{
		public static bool Button(Rect controlRect, GUIContent controlContent)
		{
			var shouldFire = false;
			var controlID = GUIUtility.GetControlID(FocusType.Passive);

			switch (Event.current.GetTypeForControl(controlID))
			{
				case EventType.Repaint:
				{
					// Draw base rectangle (will be visible as the "border").
					EditorGUI.DrawRect(controlRect, Preferences.Color_Secondary);

					// Calc the rectangle for the contentRectvar contentRect = new Rect (
					var contentRect = new Rect(
						controlRect.x +  Constants.BUTTON_BORDER_THICKNESS * 2,
						controlRect.y +  Constants.BUTTON_BORDER_THICKNESS * 2,
						controlRect.width - Constants.BUTTON_BORDER_THICKNESS * 4,
						controlRect.height - Constants.BUTTON_BORDER_THICKNESS * 4
					);

					// If mouse over button
					if(controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
					{
						DrawPressedButton(contentRect, controlContent);
					}
					else
					{
						DrawUnpressedBG(controlRect, controlContent);
						DrawUnpressedButton(contentRect, controlContent);
					}
					break;
				}
				case EventType.MouseUp:
                {
					if (controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
						GUI.changed = true;
						Event.current.Use();
						shouldFire = true;
					}
					break;
                }
			}
			return shouldFire;
		}


		public static int SelectionGrid(int selectedIndex, string[] options)
		{
			var controlID = GUIUtility.GetControlID (FocusType.Passive);

			switch (Event.current.GetTypeForControl(controlID))
			{
				case EventType.Repaint:
				break;
				case EventType.MouseDown:
				break;
				case EventType.MouseUp:
				break;
			}
			return selectedIndex;
		}


		private static void DrawUnpressedBG(Rect aRect, GUIContent aContent)
		{
			var primaryRect = new Rect (
				aRect.x + Constants.BUTTON_BORDER_THICKNESS,
				aRect.y + Constants.BUTTON_BORDER_THICKNESS,
				aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2,
				aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2
			);
			EditorGUI.DrawRect(primaryRect, Preferences.Color_Primary);
		}


		private static void DrawUnpressedButton(Rect aRect, GUIContent aContent)
		{
			var style = new GUIStyle();

			// If text buttons:
			if(Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
			{
				// Text formatting.
				style.active.textColor = style.onActive.textColor = style.normal.textColor = style.onNormal.textColor = Preferences.Color_Tertiary;
				style.imagePosition = ImagePosition.TextOnly;
				style.alignment = TextAnchor.MiddleCenter;

				// Label inside the button.
				EditorGUI.LabelField(aRect, aContent.text, style);
			}
			// If image buttons:
			else
			{
				style.imagePosition = ImagePosition.ImageOnly;

				// Icon inside the button.
				GUI.DrawTexture(aRect, aContent.image);
			}
		}


		private static void DrawPressedButton(Rect aRect, GUIContent aContent)
		{
			var style = new GUIStyle();
			// If text buttons:
			if(Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
			{
				// Text formatting.
				style.active.textColor = style.onActive.textColor = style.normal.textColor = style.onNormal.textColor = Preferences.Color_Primary;
				style.imagePosition = ImagePosition.TextOnly;
				style.alignment = TextAnchor.MiddleCenter;
				
				EditorGUI.LabelField(aRect, aContent.text, style);
			}
			// If image buttons:
			else
			{
				style.imagePosition = ImagePosition.ImageOnly;
				GUI.DrawTexture(aRect, aContent.image);
			}
		}
	}
}