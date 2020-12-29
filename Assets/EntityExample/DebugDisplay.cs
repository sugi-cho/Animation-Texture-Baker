// This class will dump output to the screen
// To use it, add an empty gameobject to the scene and attach this script to it as a component.
// you can use the parameters in the Inspector to specify the location and width/height.
//--------------------------------------------------------------------------------------------------//

using UnityEngine;
using System.Collections;
 
 public class DebugDisplay : MonoBehaviour
 {
     private string m_log;
     private Queue m_logQueue = new Queue();

    [SerializeField] private int m_x = 0; // the x coordinate of the top left
    [SerializeField] private int m_y = 0; // the y coordinate of the top left
    [SerializeField] private int m_width = -1; // the width - if this is < 0, then the screen width will be used
    [SerializeField] private int m_height = -1; // the height - if this is < 0, then the screen width will be used

    void OnEnable()
	{
         Application.logMessageReceived += HandleLog;
    }
     
     void OnDisable()
	 {
         Application.logMessageReceived -= HandleLog;
     }
 
     void HandleLog(string logString, string stackTrace, LogType type)
	 {
         m_log = logString;
         string newString = "\n [" + type + "] : " + m_log;
         m_logQueue.Enqueue(newString);
         if (type == LogType.Exception) {
             newString = "\n" + stackTrace;
             m_logQueue.Enqueue(newString);
         }
         m_log = string.Empty;
         foreach(string log in m_logQueue) { m_log += log; }
     }
 
     void OnGUI ()
	 {
        int w = (m_width < 0) ? Screen.width : m_width;
        int h = (m_height < 0) ? Screen.height : m_height;
        //RectTransform rect = GetComponent<RectTransform>();
        GUILayout.BeginArea(new Rect(m_x, m_y, w, h));
        GUILayout.Label(m_log);
        GUILayout.EndArea();
     }
 }