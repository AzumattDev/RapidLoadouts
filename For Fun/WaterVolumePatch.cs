using System;
using HarmonyLib;
using UnityEngine;

namespace RapidLoadouts.For_Fun
{
    internal class WaterVolumePatch
    {
        private static int[][] coordMap;

        public static Vector3 getVertexForCoord(int x, int z, Vector3[] vertices)
        {
            x = Mathf.Clamp(x, 0, 31);
            z = Mathf.Clamp(z, 0, 31);
            return vertices[coordMap[x][z]];
        }

        public static void precalculateVertices(MeshFilter waterSurface)
        {
            if (coordMap != null)
                return;
            Vector3[] vertices = waterSurface.sharedMesh.vertices;
            coordMap = new int[32][];
            for (int index = 0; index < coordMap.Length; ++index)
                coordMap[index] = new int[32];
            for (int index = 0; index < vertices.Length; ++index)
            {
                if (vertices[index].y == 0.0)
                {
                    float num1 = vertices[index].x + 1f;
                    float num2 = vertices[index].z + 1f;
                    float f1 = (float)(num1 * 31.0 / 2.0);
                    float f2 = (float)(num2 * 31.0 / 2.0);
                    coordMap[Mathf.RoundToInt(f1)][Mathf.RoundToInt(f2)] = index;
                }
            }

            Debug.Log("Cached water surface vertex order");
        }

        [HarmonyPatch(typeof(WaterVolume), nameof(WaterVolume.GetWaterSurface))]
        private class GetWaterSurface_Patch
        {
            private static float Postfix(float __result, WaterVolume __instance, Vector3 point)
            {
                __result += getSurfaceDeviation(__instance, point);
                return __result;
            }

            private static float getSurfaceDeviation(WaterVolume volume, Vector3 point)
            {
                MeshFilter component = volume.m_waterSurface.gameObject.GetComponent<MeshFilter>();
                if (component == null)
                    return 0.0f;
                Vector3[] vertices = component.mesh.vertices;
                if (vertices.Length != 1025)
                    return 0.0f;
                Vector3 pos = component.gameObject.transform.InverseTransformPoint(point);
                Vector3 vector3 = pos * 31f / 2f + new Vector3(15.5f, 0.0f, 15.5f);
                Vector3 vertexForCoord1 = getVertexForCoord((int)Math.Floor(vector3.x), (int)Math.Floor(vector3.z), vertices);
                Vector3 vertexForCoord2 = getVertexForCoord((int)Math.Ceiling(vector3.x), (int)Math.Floor(vector3.z), vertices);
                Vector3 vertexForCoord3 = getVertexForCoord((int)Math.Ceiling(vector3.x), (int)Math.Ceiling(vector3.z), vertices);
                Vector3 vertexForCoord4 = getVertexForCoord((int)Math.Floor(vector3.x), (int)Math.Ceiling(vector3.z), vertices);
                Vector3 b = interpolateX(vertexForCoord2, vertexForCoord1, pos);
                return interpolateZ(interpolateX(vertexForCoord3, vertexForCoord4, pos), b, pos).y;
            }

            private static Vector3 interpolateX(Vector3 a, Vector3 b, Vector3 pos)
            {
                float t = Mathf.InverseLerp(a.x, b.x, pos.x);
                float y = Mathf.Lerp(a.y, b.y, t);
                if (a.z != (double)b.z)
                    Debug.Log("interpolateX inputs have mismatched Z");
                return new Vector3(pos.x, y, a.z);
            }

            private static Vector3 interpolateZ(Vector3 a, Vector3 b, Vector3 pos)
            {
                float t = Mathf.InverseLerp(a.z, b.z, pos.z);
                float y = Mathf.Lerp(a.y, b.y, t);
                if (a.x != (double)b.x)
                    Debug.Log("interpolateZ inputs have mismatched X");
                return new Vector3(a.x, y, pos.z);
            }
        }
    }
}