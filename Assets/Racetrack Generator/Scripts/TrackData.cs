using UnityEngine;

namespace Ivankarez.RacetrackGenerator
{
    public class TrackData : ScriptableObject
    {
        public Vector3[] leftLine;
        public Vector3[] rightLine;
        public Vector3[] centerLine;
        public Vector3[] racingLine;
        public float[] racingLineSuggestedSpeeds;
        public float[] centerLineSuggestedSpeeds;
    }
}
