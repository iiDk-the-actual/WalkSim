using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace WalkSim.WalkSim.Tools
{
    public class DebugPoint : MonoBehaviour
    {
        private static readonly Dictionary<string, DebugPoint> Points = new Dictionary<string, DebugPoint>();

        public float size = 0.1f;

        public Color color = Color.white;

        private Material material;

        private void Awake()
        {
            material = Instantiate(Plugin.Plugin.instance.bundle.LoadAsset<Material>("m_xRay"));
            material.color = color;
            GetComponent<MeshRenderer>().material = material;
        }

        private void FixedUpdate()
        {
            material.color = color;
            transform.localScale = Vector3.one * (size * GTPlayer.Instance.scale);
        }

        private void OnDestroy()
        {
            Points.Remove(name);
        }

        public static Transform Get(string name, Vector3 position, Color color = default, float size = 0.1f)
        {
            var flag = Points.ContainsKey(name);
            var flag2 = flag;
            Transform transform;
            if (flag2)
            {
                Points[name].color = color;
                Points[name].transform.position = position;
                Points[name].size = size;
                transform = Points[name].transform;
            }
            else
            {
                transform = Create(name, position, color);
            }

            return transform;
        }

        private static Transform Create(string name, Vector3 position, Color color)
        {
            var transform = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            transform.name = "Cipher Debugger (" + name + ")";
            transform.localScale = Vector3.one * 0.2f;
            transform.position = position;
            transform.GetComponent<Collider>().enabled = false;
            var material = Instantiate(GorillaTagger.Instance.offlineVRRig.mainSkin.material);
            transform.GetComponent<Renderer>().material = material;
            var debugPoint = transform.gameObject.AddComponent<DebugPoint>();
            debugPoint.color = color;
            Points.Add(name, debugPoint);
            return transform.transform;
        }
    }
}