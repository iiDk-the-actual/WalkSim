using UnityEngine;

namespace WalkSim.WalkSim.Tools
{
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            var component = obj.GetComponent<T>();
            return component ? component : obj.AddComponent<T>();
        }

        public static void Obliterate(this GameObject self)
        {
            Object.Destroy(self);
        }

        public static void Obliterate(this Component self)
        {
            Object.Destroy(self);
        }

        public static float Distance(this Vector3 self, Vector3 other)
        {
            return Vector3.Distance(self, other);
        }
        
        public static float Map(float x, float a1, float a2, float b1, float b2) => b1 + (x - a1) / (a2 - a1) * (b2 - b1);
    }
}