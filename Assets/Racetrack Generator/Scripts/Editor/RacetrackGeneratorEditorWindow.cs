using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ivankarez.RacetrackGenerator
{
    public class RacetrackGeneratorEditorWindow : EditorWindow
    {
        private TextAsset trackCsv;
        private Mesh targetMesh;

        [MenuItem("Ivankarez/Racetrack Generator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(RacetrackGeneratorEditorWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("Input Data", EditorStyles.boldLabel);
            trackCsv = (TextAsset)EditorGUILayout.ObjectField("Track csv", trackCsv, typeof(TextAsset), false);
            targetMesh = (Mesh)EditorGUILayout.ObjectField("Target Mesh", targetMesh, typeof(Mesh), false);

            if (GUILayout.Button("Generate mesh"))
            {
                if (trackCsv == null)
                {
                    Debug.LogError("Track csv is null");
                    return;
                }
                Debug.Log("Parsing CSV");
                var samples = ParseCsv();
                Debug.Log("Generating mesh");
                var mesh = GenerateMesh(samples);
                Debug.Log("Saving mesh");
                if (targetMesh != null)
                {
                    EditorUtility.SetDirty(targetMesh);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    AssetDatabase.CreateAsset(mesh, $"Assets/{trackCsv.name}.asset");
                }
            }
        }

        private TrackSample[] ParseCsv()
        {
            var lines = trackCsv.text.Split('\n');
            var samples = new List<TrackSample>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;

                var values = line.Split(',')
                    .Select(s => s.Trim())
                    .ToArray();
                samples.Add(new TrackSample(float.Parse(values[0], CultureInfo.InvariantCulture),
                    float.Parse(values[1], CultureInfo.InvariantCulture),
                    float.Parse(values[2], CultureInfo.InvariantCulture),
                    float.Parse(values[3], CultureInfo.InvariantCulture)));
            }

            return samples.ToArray();
        }

        private Mesh GenerateMesh(TrackSample[] samples)
        {
            var mesh = targetMesh == null ? new Mesh() : targetMesh;
            mesh.Clear();
            var vertices = new Vector3[samples.Length * 2];
            var triangles = new int[(samples.Length + 1) * 6];
            var uvs = new Vector2[samples.Length * 2];

            for (int i = 0; i < samples.Length; i++)
            {
                var sample = samples[i];
                var next = samples[(i + 1) % samples.Length];
                var direction = (next.position - sample.position).normalized;
                var leftPoint = sample.position + Quaternion.Euler(0, 90, 0) * direction * sample.leftWidth;
                var rightPoint = sample.position + Quaternion.Euler(0, -90, 0) * direction * sample.rightWidth;
                vertices[i * 2] = leftPoint;
                vertices[i * 2 + 1] = rightPoint;
                uvs[i * 2] = new Vector2(i % 2, 0);
                uvs[i * 2 + 1] = new Vector2(i % 2, 1);
            }

            for (int i = 0; i < samples.Length - 1; i++)
            {
                triangles[i * 6] = i * 2;
                triangles[i * 6 + 1] = i * 2 + 1;
                triangles[i * 6 + 2] = i * 2 + 2;
                triangles[i * 6 + 3] = i * 2 + 2;
                triangles[i * 6 + 4] = i * 2 + 1;
                triangles[i * 6 + 5] = i * 2 + 3;
            }
            triangles[triangles.Length - 1] = vertices.Length - 1;
            triangles[triangles.Length - 2] = vertices.Length - 2;
            triangles[triangles.Length - 3] = 0;
            triangles[triangles.Length - 4] = 0;
            triangles[triangles.Length - 5] = 1;
            triangles[triangles.Length - 6] = vertices.Length - 1;


            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
