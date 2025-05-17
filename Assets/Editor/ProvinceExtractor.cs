using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json; //todo: use Newtonsoft.Json, not Unity.Plastic

public class ProvinceExtractor : EditorWindow
{
    private Texture2D provinceMap;
    private string outputFileName = "Provinces.json";

    [MenuItem("Tools/Generate Provinces JSON")]
    public static void ShowWindow()
    {
        GetWindow<ProvinceExtractor>("Province JSON Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Province Map", EditorStyles.boldLabel);
        provinceMap = (Texture2D)EditorGUILayout.ObjectField("Province PNG", provinceMap, typeof(Texture2D), false);
        outputFileName = EditorGUILayout.TextField("Output File Name", outputFileName);

        if (GUILayout.Button("Generate JSON"))
        {
            if (provinceMap != null)
                GenerateJSON();
            else
                Debug.LogError("No texture selected!");
        }
    }

    void GenerateJSON()
    {
        if (provinceMap == null) return;

        var pixels = provinceMap.GetPixels32();
        int width = provinceMap.width;
        int height = provinceMap.height;

        var colorPoints = new Dictionary<Color32, List<Vector2>>();
        int step = 4;

        for (int y = 0; y < height; y += step)
        {
            for (int x = 0; x < width; x += step)
            {
                var color = pixels[y * width + x];
                if (color.a < 255) continue;

                if (!colorPoints.ContainsKey(color))
                    colorPoints[color] = new List<Vector2>();

                float u = (float)x / width;
                float v = (float)y / height;
                colorPoints[color].Add(new Vector2(u, v));
            }
        }

        var provinceList = new List<JsonProvince>();

        foreach (var kvp in colorPoints)
        {
            var color = kvp.Key;
            var points = kvp.Value;

            Vector2 uvSum = Vector2.zero;
            foreach (var pt in points)
                uvSum += pt;

            Vector2 avgUV = points.Count > 0 ? uvSum / points.Count : Vector2.zero;

            provinceList.Add(new JsonProvince
            {
                Name = "",
                Color = $"#{color.r:X2}{color.g:X2}{color.b:X2}",
                ID = (color.r << 16) + (color.g << 8) + color.b,
                OwnerEmpire = "",
                PointUV = new float[] { avgUV.x, avgUV.y }
            });
        }

        string json = JsonConvert.SerializeObject(provinceList, Formatting.Indented);
        string path = Path.Combine(Application.dataPath, outputFileName);
        File.WriteAllText(path, json);

        Debug.Log($"✅ Province JSON saved to: {path}");
        AssetDatabase.Refresh();
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