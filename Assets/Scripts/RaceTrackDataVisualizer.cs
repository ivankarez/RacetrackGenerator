using Ivankarez.RacetrackGenerator;
using UnityEngine;

/// <summary>
/// Visualise track data in the Unity Editor
/// </summary>
public class RaceTrackDataVisualizer : MonoBehaviour
{
    [SerializeField] private TrackData raceTrackData;
    [SerializeField] private Gradient speedColorGradient;

    private void OnDrawGizmos()
    {
        if (raceTrackData == null)
        {
            return;
        }

        DrawLine(raceTrackData.centerLine, Color.white);
        DrawLine(raceTrackData.leftLine, Color.red);
        DrawLine(raceTrackData.rightLine, Color.blue);
        DrawLine(raceTrackData.racingLine, Color.green);

        for (int i = 0; i < raceTrackData.racingLineSuggestedSpeeds.Length; i++)
        {
            var speed = raceTrackData.racingLineSuggestedSpeeds[i];
            var color = speedColorGradient.Evaluate(speed / 360);
            Gizmos.color = color;
            Gizmos.DrawSphere(raceTrackData.racingLine[i], 1f);
        }

        for (int i = 0; i < raceTrackData.centerLineSuggestedSpeeds.Length; i++)
        {
            var speed = raceTrackData.centerLineSuggestedSpeeds[i];
            var color = speedColorGradient.Evaluate(speed / 360);
            Gizmos.color = color;
            Gizmos.DrawSphere(raceTrackData.centerLine[i], 1f);
        }
    }

    private void DrawLine(Vector3[] points, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLineList(points);
    }
}
