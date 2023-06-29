using System;
using System.Collections.Generic;
using UnityEngine;

namespace RapidLoadouts.For_Fun
{
    public class CalmWatersRune : MonoBehaviour, ZNetViewHook
    {
        public static HashSet<CalmWatersRune> allInstances = new HashSet<CalmWatersRune>();
        private const string CreationTimeKey = "creationTime";
        private float effectRadius = 141.24f;
        private float minGlow;
        private float maxGlow;
        private ZNetView netView;

        private void Awake()
        {
            // if (PlayerPatch.PlayerCreatingGhost)
            //     UnityEngine.Object.Destroy((UnityEngine.Object)this);
            CalmWatersRune.allInstances.Add(this);
            this.minGlow = 1f;
            this.maxGlow = 5f;
        }

        public void PostZNetViewAwake(ZNetView view)
        {
            this.netView = this.GetComponent<ZNetView>();
            // if (!PlayerPatch.PlayerPlacing)
            //     return;
            this.netView.GetZDO().Set("creationTime", (long)ZNet.instance.GetTimeSeconds());
        }

        private void Start()
        {
            HashSet<WaterSurfaceManager> managers = WaterSurfaceManager.getManagers(this.transform.position, this.effectRadius);
            bool wasPlacedByPlayer = Math.Abs((long)ZNet.instance.GetTimeSeconds() - this.netView.GetZDO().GetLong("creationTime")) < 10L;
            foreach (WaterSurfaceManager waterSurfaceManager in managers)
                waterSurfaceManager.registerRune(this, wasPlacedByPlayer);
        }

        private void OnDestroy()
        {
            CalmWatersRune.allInstances.Remove(this);
            foreach (WaterSurfaceManager manager in WaterSurfaceManager.getManagers(this.transform.position, this.effectRadius))
                manager.unregisterRune(this);
        }

        /*private void Update()
        {
            float windIntensity = EnvMan.instance.GetWindIntensity();
            this.gameObject.GetComponentInChildren<RuneProjector>().setIntensity(Mathf.Lerp(this.minGlow, this.maxGlow, windIntensity));
        }*/
    }
}