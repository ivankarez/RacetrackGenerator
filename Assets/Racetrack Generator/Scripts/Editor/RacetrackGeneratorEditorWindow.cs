using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ivankarez.RacetrackGenerator.Editor
{
    public class RacetrackGeneratorEditorWindow : EditorWindow
    {
        private TextAsset trackCsv;
        private TextAsset racingLineCsv;
        private Mesh targetMesh;
        private TrackData targetTrackData;
        private float maxSpeed = 180;
        private float agility = 25;
        private float decelerationRate = 10;

        [MenuItem("Window/Racetrack Generator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(RacetrackGeneratorEditorWindow), false, "Racetrack Generator");
        }

        private void OnGUI()
        {
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("Racetrack Generator", titleStyle);
            GUILayout.Space(10);

            GUILayout.Label("Suggested Speed Parameters:", EditorStyles.boldLabel);
            maxSpeed = EditorGUILayout.FloatField("Max Speed:", maxSpeed);
            agility = EditorGUILayout.FloatField("Agility:", agility);
            decelerationRate = EditorGUILayout.FloatField("Deceleration Rate:", decelerationRate);

            GUILayout.Space(10);
            GUILayout.Label("Input Data:", EditorStyles.boldLabel);
            trackCsv = (TextAsset)EditorGUILayout.ObjectField("Track CSV:", trackCsv, typeof(TextAsset), false);
            racingLineCsv = (TextAsset)EditorGUILayout.ObjectField("Racing Line CSV:", racingLineCsv, typeof(TextAsset), false);

            GUILayout.Space(10);
            GUILayout.Label("Output Assets:", EditorStyles.boldLabel);
            GUILayout.Label("A new asset will be created in the Asset folder if no instance is specified", EditorStyles.helpBox);
            targetMesh = (Mesh)EditorGUILayout.ObjectField("Target Mesh:", targetMesh, typeof(Mesh), false);
            targetTrackData = (TrackData)EditorGUILayout.ObjectField("Target Track Data:", targetTrackData, typeof(TrackData), false);

            GUILayout.Space(10);
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 25,
            };
            if (GUILayout.Button("Generate mesh", buttonStyle))
            {
                if (trackCsv == null)
                {
                    Debug.LogError("Track csv is null");
                    return;
                }
                Debug.Log("Parsing CSV");
                var samples = ParseTrackCsv();
                Debug.Log("Generating track data");
                var track = GenerateTrack(samples);
                Debug.Log("Saving track data");
                if (targetMesh != null)
                {
                    EditorUtility.SetDirty(targetMesh);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    AssetDatabase.CreateAsset(track.Item1, $"Assets/{trackCsv.name}.asset");
                }
                if (targetTrackData != null)
                {
                    EditorUtility.SetDirty(targetTrackData);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    AssetDatabase.CreateAsset(track.Item2, $"Assets/{trackCsv.name}_track_data.asset");
                }
            }

            GUILayout.Label("created with <3 by ivankarez", EditorStyles.centeredGreyMiniLabel);
        }

        private TrackSample[] ParseTrackCsv()
        {
            return CollectCSV(trackCsv, values => new TrackSample(float.Parse(values[0], CultureInfo.InvariantCulture),
                float.Parse(values[1], CultureInfo.InvariantCulture),
                float.Parse(values[2], CultureInfo.InvariantCulture),
                float.Parse(values[3], CultureInfo.InvariantCulture)));
        }

        private Vector3[] ParseRacingLineCsv()
        {
            if (racingLineCsv == null)
            {
                return new Vector3[0];
            }

            return CollectCSV(racingLineCsv, values => new Vector3(float.Parse(values[0], CultureInfo.InvariantCulture),
                    0,
                    float.Parse(values[1], CultureInfo.InvariantCulture)));
        }

        private T[] CollectCSV<T>(TextAsset csv, Func<string[], T> collector)
        {
            var lines = csv.text.Split('\n');
            var samples = new List<T>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;
                var values = line.Split(',')
                    .Select(s => s.Trim())
                    .ToArray();
                samples.Add(collector(values));
            }

            return samples.ToArray();
        }

        private Tuple<Mesh, TrackData> GenerateTrack(TrackSample[] samples)
        {
            var mesh = targetMesh == null ? new Mesh() : targetMesh;
            var trackData = this.targetTrackData == null ? CreateInstance<TrackData>() : this.targetTrackData;
            mesh.Clear();

            var vertices = new Vector3[samples.Length * 2];
            var triangles = new int[(samples.Length + 1) * 6];
            var uvs = new Vector2[samples.Length * 2];

            var centerLine = new Vector3[samples.Length];
            var leftLine = new Vector3[samples.Length];
            var rightLine = new Vector3[samples.Length];

            for (int i = 0; i < samples.Length; i++)
            {
                var sample = samples[i];
                var next = samples[(i + 1) % samples.Length];
                var direction = (next.position - sample.position).normalized;
                var leftPoint = sample.position + Quaternion.Euler(0, 90, 0) * direction * sample.leftWidth;
                var rightPoint = sample.position + Quaternion.Euler(0, -90, 0) * direction * sample.rightWidth;
                vertices[i * 2] = leftPoint;
                vertices[i * 2 + 1] = rightPoint;
                centerLine[i] = sample.position;
                leftLine[i] = leftPoint;
                rightLine[i] = rightPoint;
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

            trackData.centerLine = centerLine;
            trackData.leftLine = leftLine;
            trackData.rightLine = rightLine;
            trackData.racingLine = ParseRacingLineCsv();
            if (trackData.racingLine.Length != 0)
            {
                trackData.racingLineSuggestedSpeeds = SuggestedSpeedCalculator.Calulcate(trackData.racingLine, decelerationRate, agility, maxSpeed);
            }
            trackData.centerLineSuggestedSpeeds = SuggestedSpeedCalculator.Calulcate(trackData.centerLine, decelerationRate, agility, maxSpeed);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return new(mesh, trackData);
        }
    }
}
