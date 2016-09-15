using UnityEngine;
using UnityEditor;
using System.Collections;

namespace com.immortalhydra.gdtb.codetodos
{
	public static class Controls
	{
		public static float TooltipTime = 0.3f;
		public static bool Button(Rect controlRect, GUIContent controlContent)
		{
			var shouldFire = false;
			var controlID = GUIUtility.GetControlID(FocusType.Passive);

			switch (Event.current.GetTypeForControl(controlID))
			{
				case EventType.Repaint:
				{
					// Calc the rectangle for the content.
					var contentRect = new Rect(
						controlRect.x,
						controlRect.y,
						controlRect.width,
						controlRect.height
					);

					// If mouse over button
					if(controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
					{
						DrawPressedButton(contentRect, controlContent);
					}
					else
					{
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
			EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
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