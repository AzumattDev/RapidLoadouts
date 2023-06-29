using System;
using System.Collections.Generic;
using UnityEngine;

namespace RapidLoadouts.For_Fun
{
    internal class DryLandRune : MonoBehaviour
    {
        private float effectRadius;
        private float radius = 10f;
        private float depth = 5f;
        private float slope = 1f;
        public static HashSet<DryLandRune> allInstances = new();

        private void Awake()
        {
            allInstances.Add(this);
            radius = 10f;
            depth = 5f;
            slope = 2f;
            effectRadius = (float)(5.0 + 0.100000001490116 * depth + radius * (double)slope) / slope;
        }

        private void Update()
        {
            WaterSurfaceManager.recalculateNormalizedDepthsFinalState();
        }

        private void Start()
        {
            foreach (WaterSurfaceManager manager in WaterSurfaceManager.getManagers(transform.position, effectRadius))
                manager.registerRune(this);
            float a = 2f;
            float b = 3f;
            float oceanDepthAll = GetOceanDepthAll(transform.position);
            //gameObject.GetComponentInChildren<RuneProjector>().setIntensity(Mathf.Lerp(a, b, oceanDepthAll / depth));
        }

        private void OnDestroy()
        {
            allInstances.Remove(this);
            foreach (WaterSurfaceManager manager in WaterSurfaceManager.getManagers(transform.position, effectRadius))
                manager.unregisterRune(this);
        }

        public float calculateVerticalOffset(float distance) => (float)Math.Tanh((distance - (double)radius) * slope) * depth - depth;

        public float getMaxEffectRadius() => effectRadius;

        public static float GetOceanDepthAll(Vector3 worldPos)
        {
            Heightmap heightmap = Heightmap.FindHeightmap(worldPos);
            return (bool)(UnityEngine.Object)heightmap ? heightmap.GetOceanDepth(worldPos) : 0.0f;
        }
    }
}