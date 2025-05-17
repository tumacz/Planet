using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProvinceImporter : EditorWindow
{
    private TextAsset jsonFile;
    private ProvinceDatabase targetDatabase;

    [MenuItem("Tools/Import Province JSON")]
    public static void ShowWindow()
    {
        GetWindow<ProvinceImporter>("Province JSON Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Province Data", EditorStyles.boldLabel);

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("Province JSON", jsonFile, typeof(TextAsset), false);
        targetDatabase = (ProvinceDatabase)EditorGUILayout.ObjectField("Target Database", targetDatabase, typeof(ProvinceDatabase), false);

        if (GUILayout.Button("Import") && jsonFile != null && targetDatabase != null)
        {
            ImportProvinces();
        }
    }

    private void ImportProvinces()
    {
        string wrappedJson = "{\"Provinces\":" + jsonFile.text + "}";

        ProvinceListWrapper wrapper = JsonUtility.FromJson<ProvinceListWrapper>(wrappedJson);
        targetDatabase.Provinces.Clear();

        foreach (var jsonProv in wrapper.Provinces)
        {
            ColorUtility.TryParseHtmlString(jsonProv.Color, out var color);
            Province p = new Province
            {
                Name = jsonProv.Name,
                Color = color,
                ID = jsonProv.ID,
                Owner = jsonProv.OwnerEmpire,
                UVCenter = new Vector2(jsonProv.PointUV[0], jsonProv.PointUV[1])
            };
            targetDatabase.Provinces.Add(p);
        }

        EditorUtility.SetDirty(targetDatabase);
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Zaimportowano {targetDatabase.Provinces.Count} prowincji.");
    }

    [System.Serializable]
    private class ProvinceListWrapper
    {
        public List<JsonProvince> Provinces;
    }

    [System.Serializable]
    private class JsonProvince
    {
        public string Name;
        public string Color;
        public int ID;
        public string OwnerEmpire;
        public float[] PointUV;
    }
}