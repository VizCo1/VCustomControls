using UnityEngine;

namespace VCustomComponents.Runtime
{
    public static class VMathExtensions
    {
        public static Vector2 GetCircumferencePoint(float angleDegrees, float radius, Vector2 center)
        {
            var angleRadians = angleDegrees * Mathf.Deg2Rad;
            
            var x = center.x + radius * Mathf.Cos(angleRadians);
            var y = center.y + radius * Mathf.Sin(angleRadians);
            
            return new Vector2(x, y);
        }
    }
}
