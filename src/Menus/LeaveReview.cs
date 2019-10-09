using UnityEngine;
using UnityEditor;

namespace com.immortalhydra.gdtb.todos
{
	public class LeaveReview : MonoBehaviour
	{

#region METHODS

	[MenuItem("Window/Gamedev Toolbelt/TODOs/❤ Leave a review ❤")]
	private static void GoToAssetStorePage()
	{
		Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/69589");
	}

#endregion

	}
}
