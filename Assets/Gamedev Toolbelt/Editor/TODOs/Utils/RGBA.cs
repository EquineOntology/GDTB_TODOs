using UnityEngine;

namespace com.immortalhydra.gdtb.todos
{
    public class RGBA : MonoBehaviour
    {

#region METHODS

        public static string ColorToString(Color aColor)
        {
            var colorString = aColor.r.ToString() + '/' + aColor.g.ToString() + '/' + aColor.b.ToString() + '/' + aColor.a.ToString();
            return colorString;
        }


        public static Color StringToColor(string anRGBAString)
        {
            var color = new Color();
            var values = anRGBAString.Split('/');
            color.r = float.Parse(values[0]);
            color.g = float.Parse(values[1]);
            color.b = float.Parse(values[2]);
            color.a = float.Parse(values[3]);

            return color;
        }


        // Return a color with rgba values between 0 and 1.
        public static Color GetNormalizedColor(Color aColor)
        {
            return new Color(aColor.r / 255.0f, aColor.g / 255.0f, aColor.b / 255.0f, aColor.a);
        }

#endregion

    }
}