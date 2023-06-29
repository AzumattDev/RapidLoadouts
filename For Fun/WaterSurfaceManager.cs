// Decompiled with JetBrains decompiler
// Type: ValheimMod.Monobehaviours.WaterSurfaceManager
// Assembly: RuneMagic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E587902C-11D3-47DC-9EB3-21B3033B45AD
// Assembly location: C:\Users\crypt\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\Jewelcrafting_RecycleTesting\BepInEx\plugins\hyleanlegend-Rune_Magic\RuneMagic.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace RapidLoadouts.For_Fun
{
    internal class WaterSurfaceManager : MonoBehaviour
    {
        public const int OCEAN_WATERVOLUME_VERTICES = 1025;
        private static WaterVolume waterVolume;
        private MeshFilter waterSurface;
        private HashSet<DryLandRune> knownDryLandRunes;
        private static HashSet<CalmWatersRune> knownCalmWatersRunes;
        private float localToWorldDistanceScaleFactor;
        private static float surfaceSideLength;
        private bool animatingSurface;
        private static bool animatingDepth;
        private Vector3[] meshFinalState;
        private static float[] originalNormalizedDepths;
        private static float[] cornerNormalizedDepthFinalState;
        private static float calmWatersAnimationSpeed;

        private void Awake()
        {
            waterVolume = gameObject.GetComponent<WaterVolume>();
            waterSurface = waterVolume.m_waterSurface.gameObject.GetComponent<MeshFilter>();
            knownDryLandRunes = new HashSet<DryLandRune>();
            knownCalmWatersRunes = new HashSet<CalmWatersRune>();
            Vector3[] vertices = waterSurface.sharedMesh.vertices;
            Vector3 vertexForCoord1 = WaterVolumePatch.getVertexForCoord(0, 0, vertices);
            Vector3 vertexForCoord2 = WaterVolumePatch.getVertexForCoord(0, 31, vertices);
            surfaceSideLength = (waterSurface.gameObject.transform.TransformPoint(vertexForCoord1) - waterSurface.gameObject.transform.TransformPoint(vertexForCoord2)).magnitude;
            localToWorldDistanceScaleFactor = surfaceSideLength / (vertexForCoord1 - vertexForCoord2).magnitude;
            calmWatersAnimationSpeed = 2f;
            meshFinalState = waterSurface.sharedMesh.vertices;
        }

        private void Start()
        {
            waterVolume.GetType().GetMethod("DetectWaterDepth", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(waterVolume, new object[0]);
            originalNormalizedDepths = new float[4];
            cornerNormalizedDepthFinalState = new float[4];
            float[] numArray = Traverse.Create(waterVolume).Field("m_normalizedDepth").GetValue() as float[];
            for (int index = 0; index < 4; ++index)
            {
                originalNormalizedDepths[index] = numArray[index];
                cornerNormalizedDepthFinalState[index] = numArray[index];
            }

            findInitialNearbyRunes();
        }

        public void registerRune(DryLandRune rune)
        {
            if (knownDryLandRunes.Contains(rune))
                return;
            knownDryLandRunes.Add(rune);
            recalculateMeshFinalState();
        }

        public void unregisterRune(DryLandRune rune)
        {
            if (!knownDryLandRunes.Contains(rune))
                return;
            knownDryLandRunes.Remove(rune);
            recalculateMeshFinalState();
        }

        public void registerRune(CalmWatersRune rune, bool wasPlacedByPlayer)
        {
            if (knownCalmWatersRunes.Contains(rune))
                return;
            knownCalmWatersRunes.Add(rune);
            if (!wasPlacedByPlayer && !animatingDepth)
            {
                recalculateNormalizedDepthsFinalState();
                calmWatersAnimationSpeed = 5f;
            }
            else
                recalculateNormalizedDepthsFinalState();
        }

        public void unregisterRune(CalmWatersRune rune)
        {
            if (!knownCalmWatersRunes.Contains(rune))
                return;
            knownCalmWatersRunes.Remove(rune);
            recalculateNormalizedDepthsFinalState();
        }

        public float getSurfaceSideLength() => surfaceSideLength;

        private void findInitialNearbyRunes()
        {
            foreach (DryLandRune allInstance in DryLandRune.allInstances)
            {
                if (Utils.DistanceXZ(allInstance.transform.position, transform.position) <= allInstance.getMaxEffectRadius() + (double)surfaceSideLength)
                    knownDryLandRunes.Add(allInstance);
            }

            foreach (CalmWatersRune allInstance in CalmWatersRune.allInstances)
            {
                Vector3 vector3 = allInstance.transform.position - transform.position;
                if (Math.Abs(vector3.x) <= surfaceSideLength * 2.0 && Math.Abs(vector3.z) <= surfaceSideLength * 2.0)
                    knownCalmWatersRunes.Add(allInstance);
            }

            if (knownDryLandRunes.Count > 0)
            {
                Vector3[] vertices = waterSurface.mesh.vertices;
                resetVertices(vertices);
                foreach (DryLandRune knownDryLandRune in knownDryLandRunes)
                    calculateRuneEffect(knownDryLandRune, vertices);
                waterSurface.mesh.vertices = vertices;
                waterSurface.mesh.RecalculateNormals();
            }

            if (knownCalmWatersRunes.Count <= 0)
                return;
            recalculateNormalizedDepthsFinalState();
            animateDepth(1000f);
        }

        private void Update()
        {
            if (animatingSurface)
                animateSurface(Time.deltaTime);
            if (!animatingDepth)
                return;
            animateDepth(Time.deltaTime);
        }

        private void animateSurface(float dt)
        {
            Vector3[] vertices = waterSurface.mesh.vertices;
            animatingSurface = false;
            float num1 = 2f * dt;
            float val2 = 4f;
            for (int index = 0; index < vertices.Length; ++index)
            {
                Vector3 vector3 = meshFinalState[index];
                float num2 = Math.Min(Math.Max(Math.Abs(vector3.y - vertices[index].y) * num1, val2), Math.Abs(vector3.y - vertices[index].y));
                if (vector3.y > (double)vertices[index].y)
                    vertices[index].y += num2;
                else
                    vertices[index].y -= num2;
                if (num2 > 1E-05)
                    animatingSurface = true;
            }

            waterSurface.mesh.vertices = vertices;
            waterSurface.mesh.RecalculateNormals();
        }

        private void recalculateMeshFinalState()
        {
            resetVertices(meshFinalState);
            foreach (DryLandRune knownDryLandRune in knownDryLandRunes)
                calculateRuneEffect(knownDryLandRune, meshFinalState);
            animatingSurface = true;
        }

        private void resetVertices(Vector3[] vertices)
        {
            for (int index = 0; index < vertices.Length; ++index)
                vertices[index].y = 0.0f;
        }

        private void calculateRuneEffect(DryLandRune rune, Vector3[] vertices)
        {
            if (rune == null || Utils.DistanceXZ(rune.transform.position, transform.position) > rune.getMaxEffectRadius() + (double)surfaceSideLength)
                return;
            Vector3 vector3 = waterSurface.gameObject.transform.InverseTransformPoint(rune.transform.position);
            for (int index = 0; index < vertices.Length; ++index)
            {
                float distance = Utils.DistanceXZ(vector3, vertices[index]) * localToWorldDistanceScaleFactor;
                if (distance < (double)rune.getMaxEffectRadius())
                    vertices[index].y = Math.Min(vertices[index].y, rune.calculateVerticalOffset(distance));
            }
        }

        internal static void recalculateNormalizedDepthsFinalState()
        {
            for (int index = 0; index < originalNormalizedDepths.Length; ++index)
                cornerNormalizedDepthFinalState[index] = originalNormalizedDepths[index];
            float val1 = 5f;
            BoxCollider component = waterVolume.gameObject.GetComponent<BoxCollider>();
            Vector3[] vector3Array1 = new Vector3[4];
            Vector3[] vector3Array2 = vector3Array1;
            Transform transform1 = waterVolume.transform;
            Bounds bounds = component.bounds;
            double x1 = -(double)bounds.size.x / 2.0;
            bounds = component.bounds;
            double z1 = bounds.size.z / 2.0;
            Vector3 vector3_1 = transform1.TransformPoint((float)x1, 0.0f, (float)z1);
            vector3Array2[0] = vector3_1;
            Vector3[] vector3Array3 = vector3Array1;
            Transform transform2 = waterVolume.transform;
            bounds = component.bounds;
            double x2 = bounds.size.x / 2.0;
            bounds = component.bounds;
            double z2 = bounds.size.z / 2.0;
            Vector3 vector3_2 = transform2.TransformPoint((float)x2, 0.0f, (float)z2);
            vector3Array3[1] = vector3_2;
            Vector3[] vector3Array4 = vector3Array1;
            Transform transform3 = waterVolume.transform;
            bounds = component.bounds;
            double x3 = bounds.size.x / 2.0;
            bounds = component.bounds;
            double z3 = -(double)bounds.size.z / 2.0;
            Vector3 vector3_3 = transform3.TransformPoint((float)x3, 0.0f, (float)z3);
            vector3Array4[2] = vector3_3;
            Vector3[] vector3Array5 = vector3Array1;
            Transform transform4 = waterVolume.transform;
            bounds = component.bounds;
            double x4 = -(double)bounds.size.x / 2.0;
            bounds = component.bounds;
            double z4 = -(double)bounds.size.z / 2.0;
            Vector3 vector3_4 = transform4.TransformPoint((float)x4, 0.0f, (float)z4);
            vector3Array5[3] = vector3_4;
            foreach (CalmWatersRune knownCalmWatersRune in knownCalmWatersRunes)
            {
                Vector3 position = knownCalmWatersRune.transform.position;
                for (int index = 0; index < vector3Array1.Length; ++index)
                {
                    Vector3 vector3_5 = vector3Array1[index];
                    if (Math.Abs(vector3_5.x - position.x) <= (double)surfaceSideLength && Math.Abs(vector3_5.z - position.z) <= (double)surfaceSideLength)
                        cornerNormalizedDepthFinalState[index] = Math.Min(val1, cornerNormalizedDepthFinalState[index]);
                }
            }

            calmWatersAnimationSpeed = 2f;
            animatingDepth = true;
        }

        private void animateDepth(float dt)
        {
            animatingDepth = false;
            float num1 = calmWatersAnimationSpeed * dt;
            float val2 = 4f;
            float[] numArray = Traverse.Create(waterVolume).Field("m_normalizedDepth").GetValue() as float[];
            for (int index = 0; index < cornerNormalizedDepthFinalState.Length; ++index)
            {
                float num2 = cornerNormalizedDepthFinalState[index];
                float num3 = Math.Min(Math.Max(Math.Abs(num2 - numArray[index]) * num1, val2), Math.Abs(num2 - numArray[index]));
                if (num2 > (double)numArray[index])
                    numArray[index] += num3;
                else
                    numArray[index] -= num3;
                if (num3 > 1E-05)
                    animatingDepth = true;
            }

            waterVolume.GetType().GetMethod("SetupMaterial", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(waterVolume, new object[0]);
        }

        public static HashSet<WaterSurfaceManager> getManagers(
            Vector3 point,
            float radius)
        {
            HashSet<WaterSurfaceManager> managers = new();
            WaterSurfaceManager managerAtPoint1 = getManagerAtPoint(point);
            if (managerAtPoint1 == null)
                return managers;
            float surfaceSideLength = managerAtPoint1.getSurfaceSideLength();
            int num = (int)Math.Ceiling(radius / (double)surfaceSideLength);
            for (int index1 = -num; index1 <= num; ++index1)
            {
                for (int index2 = -num; index2 <= num; ++index2)
                {
                    WaterSurfaceManager managerAtPoint2 = getManagerAtPoint(point + new Vector3(index1 * surfaceSideLength, 0.0f, index2 * surfaceSideLength));
                    if (managerAtPoint2 != null)
                        managers.Add(managerAtPoint2);
                }
            }

            return managers;
        }

        public static WaterSurfaceManager getManagerAtPoint(Vector3 point)
        {
            Heightmap heightmap = Heightmap.FindHeightmap(point);
            return heightmap == null ? null : heightmap.transform.root.gameObject.GetComponentInChildren<WaterSurfaceManager>();
        }

        public static void addToWaterVolumes()
        {
            foreach (WaterVolume waterVolume in Resources.FindObjectsOfTypeAll<WaterVolume>())
            {
                if (waterVolume != null && waterVolume.m_waterSurface != null && waterVolume.GetComponentInParent<WaterSurfaceManager>() == null)
                {
                    MeshFilter component = waterVolume.m_waterSurface.gameObject.GetComponent<MeshFilter>();
                    if (component != null && component.sharedMesh != null && component.sharedMesh.vertexCount == 1025)
                    {
                        WaterVolumePatch.precalculateVertices(component);
                        waterVolume.gameObject.AddComponent<WaterSurfaceManager>();
                    }
                }
            }
        }
    }
}