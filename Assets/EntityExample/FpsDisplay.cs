// i yanked this class from the internet... stackoverflow or something.
// i added the ability to change color and font size
// i added the ability to set target frame rate and vsync count.
//--------------------------------------------------------------------------------------------------//

using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
	[SerializeField, Tooltip("an integer number for font size that scales with screen size, 1..n (default 2)")] private int fontSize = 2;
	[SerializeField, Tooltip("Color of the text to use (default null)")] private Color textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
	[SerializeField, Tooltip("The target frame-rate for the app. If set it to zero, the rate won't be set.  (default 0)")] private int appTargetFrameRate = 0;
	[SerializeField, Tooltip("The vsync count for the app. If set to zero, the count won't be set. (default 0)")] private int vSyncCount = 0;

	float deltaTime = 0.0f;

	private void Awake()
    {
		if (appTargetFrameRate >= 0) { Application.targetFrameRate = appTargetFrameRate; }
		if (vSyncCount >= 0) { QualitySettings.vSyncCount = vSyncCount; }
	}

    void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		GUIStyle style = new GUIStyle();
		Rect rect = new Rect(0, 0, w, (h * fontSize) / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = (h * fontSize) / 100;
		style.normal.textColor = textColor;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms, {1:0.} fps", msec, fps);
		GUI.Label(rect, text, style);
	}
}