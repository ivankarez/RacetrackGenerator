using Ivankarez.RacetrackGenerator;
using UnityEngine;

/// <summary>
/// Visualise track data in the Unity Editor
/// </summary>
public class RaceTrackDataVisualizer : MonoBehaviour
{
    [SerializeField] private TrackData raceTrackData;

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
    }

    private void DrawLine(Vector3[] points, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLineList(points);
    }
}
