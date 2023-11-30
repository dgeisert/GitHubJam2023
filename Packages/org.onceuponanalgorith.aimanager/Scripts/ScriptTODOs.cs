using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ScriptTODOs : MonoBehaviour
{
#if UNITY_EDITOR
    private AIManager aiManager;
    private string scriptFolderPath = "Assets/Scripts/";
    [SerializeField] private TextAsset targetScript;
    [SerializeField] private bool doIt = false;
    private string targetScriptPath;

    private void Update()
    {
        if(doIt)
        {
            doIt = false;
            aiManager = GetComponent<AIManager>();
            SendToAI();
        }
    }

    private void SendToAI()
    {
        targetScriptPath = scriptFolderPath + targetScript.name + ".cs";
        string[] lines = File.ReadAllLines(targetScriptPath);
        string toAI = "In the following Unity C# script replace all the TODO comments with functioning code." +
            "Only return the script.  Do not return any text other than the script. \n\n";

        for (int i = 0; i < lines.Length; i++)
        {
            toAI += "\n" + lines[i];
        }

        aiManager.GetText(toAI, UpdateScript, new AIPassthroughData());
    }

    private void UpdateScript(string result, AIPassthroughData data)
    {
        string[] lines = result.Split('\n');
        File.WriteAllLines(targetScriptPath, lines);
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
#endif
}
