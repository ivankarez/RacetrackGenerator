using UnityEngine;

namespace Ivankarez.RacetrackGenerator
{
    public static class SuggestedSpeedCalculator
    {
        public static float[] Calulcate(Vector3[] racingLine, float decelerationRate, float agility, float maxSpeed)
        {
            var angles = new float[racingLine.Length];
            var maxSpeeds = new float[racingLine.Length];
            var turningRate = maxSpeed / agility;
            for (int i = 0; i < racingLine.Length; i++)
            {
                var prev = racingLine[(i - 1 + racingLine.Length) % racingLine.Length];
                var current = racingLine[i];
                var next = racingLine[(i + 1) % racingLine.Length];

                angles[i] = CalculateAngle(prev, current, next);
                maxSpeeds[i] = Mathf.Max(-angles[i] * turningRate + maxSpeed, 0);
            }

            for (int i = racingLine.Length - 1; i >= 0; i--)
            {
                var prevIndex = (i - 1 + racingLine.Length) % racingLine.Length;
                var currentSpeed = maxSpeeds[i];
                var previousSpeed = maxSpeeds[prevIndex];
                if (currentSpeed < previousSpeed)
                {
                    var currentPoint = racingLine[i];
                    var prevPoint = racingLine[prevIndex];
                    var distance = Vector3.Distance(prevPoint, currentPoint);
                    var possibleSpeedDifference = distance * decelerationRate;
                    var expectedSpeedDifference = previousSpeed - currentSpeed;
                    if (expectedSpeedDifference > possibleSpeedDifference)
                    {
                        maxSpeeds[prevIndex] = currentSpeed + possibleSpeedDifference;
                    }
                }
            }

            return maxSpeeds;
        }

        private static float CalculateAngle(Vector3 prev, Vector3 current, Vector3 next)
        {
            var prevToCurrent = (current - prev).normalized;
            var currentToNext = (next - current).normalized;

            return Vector3.Angle(prevToCurrent, currentToNext);
        }
    }
}
