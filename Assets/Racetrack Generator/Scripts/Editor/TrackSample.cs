using UnityEngine;

namespace Ivankarez.RacetrackGenerator.Editor
{
    public struct TrackSample
    {
        public float x;
        public float y;
        public float leftWidth;
        public float rightWidth;
        public Vector3 position;

        public TrackSample(float x, float y, float rightWidth, float leftWidth)
        {
            this.x = x;
            this.y = y;
            this.leftWidth = leftWidth;
            this.rightWidth = rightWidth;
            position = new Vector3(x, 0, y);
        }
    }
}
