using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ScriptCreator : MonoBehaviour
{

#if UNITY_EDITOR
    private AIManager aiManager;
    [SerializeField] private List<TextAsset> fliesToInclude;
    [SerializeField] private string scriptName = "NewScript";
    [TextAreaAttribute]
    [SerializeField] private string scriptDescription;
    [SerializeField] private bool doIt = false;

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
        string toAI = "Write a Unity script that can perform the following action." +
            "Only return the script.  Do not return any text other than the script. \n" +
            "The name of the script is " + scriptName + "\n" +
            scriptDescription;

        if(fliesToInclude.Count > 0)
        {
            toAI += "\n\nThe following files are included as reference when making " + scriptName + ".";
        }

        foreach (TextAsset fly in fliesToInclude)
        {
            toAI += "\n\n" + fly.text;
        }

        aiManager.GetText(toAI, CreateScript, new AIPassthroughData());
    }

    private void CreateScript(string result, AIPassthroughData data)
    {
        string path = "Assets/Scripts/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path += scriptName + ".cs";
        if (File.Exists(path))
        {
            EditorUtility.DisplayDialog("Error", "A script with that name already exists!", "OK");
            return;
        }

        using(StreamWriter writer = new StreamWriter(path))
        {
            foreach (string line in result.Split('\n'))
            {
                writer.WriteLine(line);
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
#endif
}
