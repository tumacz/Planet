using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GizmoUV
{
    private const float OverlapTolerance = 0.001f;
    private const float GridSpacing = 0.08f;

    public static void DrawSphereGrid(Transform sphereTransform, bool showUVs, int uvStep, Vector3 labelOffset, Color gizCol,
        int latitudeLines = 18, int longitudeLines = 36, float radius = 1f)
    {
        Gizmos.color = gizCol;

#if UNITY_EDITOR
        Dictionary<Vector3, List<string>> labelMap = new Dictionary<Vector3, List<string>>();
#endif

        //LATITUDE
        for (int i = 1; i < latitudeLines; i++)
        {
            float lat = Mathf.PI * i / latitudeLines;
            float y = Mathf.Cos(lat);
            float r = Mathf.Sin(lat);

            Vector3 prevPoint = Vector3.zero;
            for (int j = 0; j <= longitudeLines; j++)
            {
                float lon = 2 * Mathf.PI * j / longitudeLines;
                float x = Mathf.Cos(lon) * r;
                float z = Mathf.Sin(lon) * r;

                Vector3 point = new Vector3(x, y, z).normalized * radius;
                point = sphereTransform.TransformPoint(point);

                if (j > 0) Gizmos.DrawLine(prevPoint, point);

#if UNITY_EDITOR
                if (showUVs && i % uvStep == 0 && j % uvStep == 0)
                {
                    Vector2 uv = new Vector2((float)j / longitudeLines, (float)i / latitudeLines);
                    Vector3 labelPos = point + labelOffset;
                    labelPos = RoundVector(labelPos);

                    if (!labelMap.ContainsKey(labelPos))
                        labelMap[labelPos] = new List<string>();

                    labelMap[labelPos].Add($"UV: {uv:F2}");
                }
#endif
                prevPoint = point;
            }
        }

        //LONGITUDE
        for (int i = 0; i < longitudeLines; i++)
        {
            float lon = 2 * Mathf.PI * i / longitudeLines;
            Vector3 prevPoint = Vector3.zero;
            for (int j = 0; j <= latitudeLines; j++)
            {
                float lat = Mathf.PI * j / latitudeLines;
                float y = Mathf.Cos(lat);
                float r = Mathf.Sin(lat);
                float x = Mathf.Cos(lon) * r;
                float z = Mathf.Sin(lon) * r;

                Vector3 point = new Vector3(x, y, z).normalized * radius;
                point = sphereTransform.TransformPoint(point);

                if (j > 0) Gizmos.DrawLine(prevPoint, point);

#if UNITY_EDITOR
                if (showUVs && i % uvStep == 0 && j % uvStep == 0)
                {
                    Vector2 uv = new Vector2((float)i / longitudeLines, (float)j / latitudeLines);
                    Vector3 labelPos = point + labelOffset;
                    labelPos = RoundVector(labelPos);

                    if (!labelMap.ContainsKey(labelPos))
                        labelMap[labelPos] = new List<string>();

                    labelMap[labelPos].Add($"UV: {uv:F2}");
                }
#endif
                prevPoint = point;
            }
        }

#if UNITY_EDITOR
        foreach (var kvp in labelMap)
        {
            Vector3 basePos = kvp.Key;
            List<string> labels = kvp.Value.Distinct().ToList();

            int cols = Mathf.CeilToInt(Mathf.Sqrt(labels.Count));
            for (int i = 0; i < labels.Count; i++)
            {
                int row = i / cols;
                int col = i % cols;
                Vector3 offset = new Vector3(col * GridSpacing, row * GridSpacing, 0f);
                Handles.Label(basePos + offset, labels[i]);
            }
        }
#endif
    }

    private static Vector3 RoundVector(Vector3 v, float precision = 0.01f)
    {
        return new Vector3(
            Mathf.Round(v.x / precision) * precision,
            Mathf.Round(v.y / precision) * precision,
            Mathf.Round(v.z / precision) * precision
        );
    }
}
