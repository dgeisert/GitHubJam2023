using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using System.IO;

[ExecuteInEditMode]
public class QuickArt : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private string artStyle;
    [SerializeField] private string artRequest;
    [SerializeField] private string fileName;
    [SerializeField] private bool doIt;
    private AIManager aiManager;

    private void Update()
    {
        if(doIt)
        {
            doIt = false;
            aiManager = GetComponent<AIManager>();
            aiManager.GetImage(artRequest + ". " + artStyle, SaveArt, new AIPassthroughData());
            Debug.Log("Making art for " + fileName);
        }
    }

    private void SaveArt(Texture texture, AIPassthroughData data)
    {
        string path = "Assets/Art";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        byte[] pngData = ((Texture2D)texture).EncodeToPNG();
        string filePath = Path.Combine(path, fileName + ".png");
        File.WriteAllBytes(filePath, pngData);

        AssetDatabase.ImportAsset(filePath);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(filePath);
        importer.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(filePath);
    }
#endif
}
