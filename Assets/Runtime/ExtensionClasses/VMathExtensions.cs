using UnityEngine;

namespace VCustomComponents
{
    public static class VMathExtensions
    {
        public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
        {
            vector.x = Mathf.Clamp(vector.x, min.x, max.x);
            vector.y = Mathf.Clamp(vector.y, min.y, max.y);
            
            return vector;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }
        
        public static int Clamp(this int value, int min, int max)
        {
            return Mathf.Clamp(value, min, max);
        }
        
        public static Vector2 GetCircumferencePoint(float angleDegrees, float radius, Vector2 center)
        {
            var angleRadians = angleDegrees * Mathf.Deg2Rad;
            
            var x = center.x + radius * Mathf.Cos(angleRadians);
            var y = center.y + radius * Mathf.Sin(angleRadians);
            
            return new Vector2(x, y);
        }
    }
}
