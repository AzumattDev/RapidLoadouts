/*// Decompiled with JetBrains decompiler
// Type: ValheimMod.PlayerPatch
// Assembly: RuneMagic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E587902C-11D3-47DC-9EB3-21B3033B45AD
// Assembly location: C:\Users\crypt\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\Jewelcrafting_RecycleTesting\BepInEx\plugins\hyleanlegend-Rune_Magic\RuneMagic.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using ValheimMod.Monobehaviours;
using ValheimMod.ScriptableObjects;
using ValheimMod.Setup;

namespace ValheimMod
{
    internal class PlayerPatch
    {
        public static readonly int PLACEMENT_STATUS_VALID = 0;
        public static readonly int PLACEMENT_STATUS_INVALID = 1;
        public static readonly int PLACEMENT_STATUS_BLOCKED_BY_PLAYER = 2;
        public static readonly int PLACEMENT_STATUS_TOO_CLOSE = 5;
        public static readonly string PLAYER_EYE_GLOW_KEY = "playerEyeGlow";
        public static bool PlayerPlacing = false;
        public static bool PlayerCreatingGhost = false;
        private static float MaxPlaceDistanceMinimum = 5f;

        private static bool isRuneFocusPieceTable(PieceTable table) => !((UnityEngine.Object)table == (UnityEngine.Object)null) && table.gameObject.name.Equals("_RuneFocusPieceTable");

        private static bool isPieceInTable(PieceTable table, Piece piece)
        {
            if ((UnityEngine.Object)table == (UnityEngine.Object)null || (UnityEngine.Object)piece == (UnityEngine.Object)null)
                return false;
            foreach (GameObject piece1 in table.m_pieces)
            {
                Piece component = piece1.GetComponent<Piece>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.m_name == piece.m_name)
                    return true;
            }

            return false;
        }

        private static bool isRunemagicShatterSelected(Player __instance)
        {
            PieceTable table = Traverse.Create((object)__instance).Field("m_buildPieces").GetValue() as PieceTable;
            return !((UnityEngine.Object)table == (UnityEngine.Object)null) && PlayerPatch.isRuneFocusPieceTable(table) && !((UnityEngine.Object)table.GetSelectedPiece() == (UnityEngine.Object)null) && table.GetSelectedPiece().gameObject.name.Equals("runemagic_ShatteringRune");
        }

        private static bool isResizableEffectSelected(Player __instance)
        {
            PieceTable table = Traverse.Create((object)__instance).Field("m_buildPieces").GetValue() as PieceTable;
            return !((UnityEngine.Object)table == (UnityEngine.Object)null) && PlayerPatch.isRuneFocusPieceTable(table) && !((UnityEngine.Object)table.GetSelectedPiece() == (UnityEngine.Object)null) && ValheimMod.ValheimMod.resizableTerrainModifierNames.Contains(table.GetSelectedPiece().gameObject.name);
        }

        public static void ExpendEnergy(Player player, float amount)
        {
            if (player.NoCostCheat())
                return;
            PieceTable table = Traverse.Create((object)player).Field("m_buildPieces").GetValue() as PieceTable;
            if ((UnityEngine.Object)table == (UnityEngine.Object)null || !PlayerPatch.isRuneFocusPieceTable(table))
                return;
            ItemDrop.ItemData playerRightItem = PlayerPatch.getPlayerRightItem(player);
            playerRightItem.m_durability -= amount;
            playerRightItem.m_durability = Mathf.Clamp(playerRightItem.m_durability, 0.0f, playerRightItem.GetMaxDurability());
        }

        public static bool HasEnoughEnergy(Player player, float amount)
        {
            PieceTable table = Traverse.Create((object)player).Field("m_buildPieces").GetValue() as PieceTable;
            return !((UnityEngine.Object)table == (UnityEngine.Object)null) && PlayerPatch.isRuneFocusPieceTable(table) && (double)PlayerPatch.getPlayerRightItem(player).m_durability >= (double)amount;
        }

        public static float getAvailableEnergy(Player player)
        {
            PieceTable table = Traverse.Create((object)player).Field("m_buildPieces").GetValue() as PieceTable;
            return (UnityEngine.Object)table == (UnityEngine.Object)null || !PlayerPatch.isRuneFocusPieceTable(table) ? 0.0f : PlayerPatch.getPlayerRightItem(player).m_durability;
        }

        public static ItemDrop.ItemData getPlayerRightItem(Player player) => Traverse.Create((object)player).Field("m_rightItem").GetValue() as ItemDrop.ItemData;

        [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
        private class UpdatePlacementGhost_Patch
        {
            private static float savedMaxPlaceDistance = 5f;

            private static void Prefix(Player __instance)
            {
                if (ConfigLoader.getBool("disableRangeExtensionForLargePieces"))
                    return;
                PlayerPatch.UpdatePlacementGhost_Patch.savedMaxPlaceDistance = __instance.m_maxPlaceDistance;
                __instance.m_maxPlaceDistance = Mathf.Max(PlayerPatch.MaxPlaceDistanceMinimum, PlayerPatch.UpdatePlacementGhost_Patch.savedMaxPlaceDistance);
            }

            private static Exception Finalizer(
                Player __instance,
                GameObject ___m_placementGhost,
                int ___m_placeRotation,
                int ___m_placementStatus)
            {
                PlayerPatch.UpdatePlacementGhost_Patch.DoRuneSpecialBehaviors(__instance, ___m_placementGhost, ___m_placeRotation, ___m_placementStatus);
                if (!ConfigLoader.getBool("disableRangeExtensionForLargePieces"))
                    __instance.m_maxPlaceDistance = PlayerPatch.UpdatePlacementGhost_Patch.savedMaxPlaceDistance;
                return (Exception)null;
            }

            private static void DoRuneSpecialBehaviors(
                Player __instance,
                GameObject ___m_placementGhost,
                int ___m_placeRotation,
                int ___m_placementStatus)
            {
                if ((UnityEngine.Object)___m_placementGhost == (UnityEngine.Object)null || !UnlockManager.isUnlockable(__instance.GetSelectedPiece()))
                    return;
                PlayerRaycast ray = new PlayerRaycast(__instance);
                if (!ray.hit)
                    return;
                PlacementGhostBehavior[] componentsInChildren = ___m_placementGhost.GetComponentsInChildren<PlacementGhostBehavior>();
                int index = 0;
                while (index < componentsInChildren.Length && componentsInChildren[index].AdjustPlacementGhost(__instance, ___m_placementGhost, ray))
                    ++index;
                if (PlayerPatch.isRunemagicShatterSelected(__instance))
                    PlayerPatch.UpdatePlacementGhost_Patch.RunemagicShatterPatch(__instance, ___m_placementGhost);
                else if (PlayerPatch.isResizableEffectSelected(__instance))
                    PlayerPatch.UpdatePlacementGhost_Patch.ResizablePiecePatch(__instance, ___m_placementGhost, ___m_placeRotation);
                ___m_placementStatus = (int)Traverse.Create((object)__instance).Field("m_placementStatus").GetValue();
                ___m_placementGhost.GetComponent<Piece>().SetInvalidPlacementHeightlight(___m_placementStatus != PlayerPatch.PLACEMENT_STATUS_VALID);
            }

            private static void RunemagicShatterPatch(Player __instance, GameObject ___m_placementGhost)
            {
                int mask = LayerMask.GetMask("Default", "static_solid", "Default_small", "terrain", "vehicle");
                PlayerRaycast playerRaycast = new PlayerRaycast(__instance, mask);
                if (!playerRaycast.hit || !RockDestroyer.isDestructibleStone(playerRaycast.hitInfo.collider.gameObject))
                    return;
                if (RockDestroyer.hasDestructibleHull(playerRaycast.hitInfo.collider.gameObject))
                {
                    RockDestroyer.create().destroyRockWithRaycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, mask, true);
                }
                else
                {
                    MeshFilter component = playerRaycast.hitInfo.collider.gameObject.GetComponent<MeshFilter>();
                    if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
                        return;
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(AssetLoader.GetPrefabFromAssetBundle("vfx_ShatteringRuneHighlight"));
                    gameObject.transform.parent = component.gameObject.transform;
                    gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    ParticleSystem.ShapeModule shape = gameObject.GetComponent<ParticleSystem>().shape with
                    {
                        mesh = component.sharedMesh,
                        scale = component.gameObject.transform.lossyScale
                    };
                }
            }

            private static void ResizablePiecePatch(
                Player __instance,
                GameObject ___m_placementGhost,
                int ___m_placeRotation)
            {
                float num1 = ConfigLoader.getFloat("resizablePieceMaxRadius");
                float num2 = ConfigLoader.getFloat("resizablePieceMinRadius");
                int num3 = ConfigLoader.getInt("resizablePieceGranularity");
                ___m_placementGhost.transform.rotation = Quaternion.identity;
                float radius = (float)(Mathf.Abs(___m_placeRotation) % num3) / (float)num3 * (num1 - num2) + num2;
                PlayerPatch.UpdatePlacementGhost_Patch.setRadius(___m_placementGhost, radius);
                PlayerPatch.UpdatePlacementGhost_Patch.setRadius((Traverse.Create((object)__instance).Field("m_buildPieces").GetValue() as PieceTable).GetSelectedPiece().gameObject, radius);
            }

            private static void setRadius(GameObject resizable, float radius)
            {
                TerrainModifier componentInChildrenAll1 = ValheimMod.ValheimMod.getComponentInChildrenAll<TerrainModifier>(resizable);
                if ((UnityEngine.Object)componentInChildrenAll1 != (UnityEngine.Object)null)
                {
                    componentInChildrenAll1.m_levelRadius = radius;
                    componentInChildrenAll1.m_smoothRadius = radius;
                    componentInChildrenAll1.m_paintRadius = radius;
                }

                foreach (ParticleSystem componentsInChild in resizable.GetComponentsInChildren<ParticleSystem>(true))
                {
                    ParticleSystem.ShapeModule shape = componentsInChild.shape;
                    if ((double)shape.radius != (double)radius)
                        shape.radius = radius;
                }

                TerrainOp componentInChildrenAll2 = ValheimMod.ValheimMod.getComponentInChildrenAll<TerrainOp>(resizable);
                if ((UnityEngine.Object)componentInChildrenAll2 != (UnityEngine.Object)null)
                {
                    componentInChildrenAll2.m_settings.m_levelRadius = radius;
                    componentInChildrenAll2.m_settings.m_raiseRadius = radius;
                    componentInChildrenAll2.m_settings.m_smoothRadius = radius;
                    componentInChildrenAll2.m_settings.m_paintRadius = radius;
                }

                ValheimMod.ValheimMod.getComponentInChildrenAll<ResizableRune>(resizable)?.setSize(radius);
                if (!((UnityEngine.Object)ValheimMod.ValheimMod.getComponentInChildrenAll<RuneProjector>(resizable) != (UnityEngine.Object)null))
                    return;
                resizable.transform.localScale = new Vector3(radius, 1f, radius);
            }
        }

        [HarmonyPatch(typeof(Player), "CheckPlacementGhostVSPlayers")]
        private class CheckPlacementGhostVSPlayers_Patch
        {
            private static bool Postfix(bool __result, Player __instance, GameObject ___m_placementGhost) => !((UnityEngine.Object)___m_placementGhost == (UnityEngine.Object)null) && !((UnityEngine.Object)___m_placementGhost.GetComponent<RuneProjector>() != (UnityEngine.Object)null) && __result;
        }

        [HarmonyPatch(typeof(Player), "ConsumeResources")]
        private class ConsumeResources_Patch
        {
            private static void Postfix(Player __instance)
            {
                PieceTable table = Traverse.Create((object)__instance).Field("m_buildPieces").GetValue() as PieceTable;
                if ((UnityEngine.Object)table == (UnityEngine.Object)null || !PlayerPatch.isRuneFocusPieceTable(table) || (UnityEngine.Object)table.GetSelectedPiece() == (UnityEngine.Object)null)
                    return;
                PlayerPatch.ExpendEnergy(__instance, RunemagicPieceConfig.getEnergyCost(table.GetSelectedPiece()));
            }
        }

        [HarmonyPatch(typeof(Player), "SetupPlacementGhost")]
        private class SetupPlacementGhost_Patch
        {
            private static bool Prefix()
            {
                PlayerPatch.PlayerCreatingGhost = true;
                return true;
            }

            private static void Postfix(Player __instance, GameObject ___m_placementGhost)
            {
                PlayerPatch.PlayerCreatingGhost = false;
                if ((UnityEngine.Object)___m_placementGhost == (UnityEngine.Object)null)
                    return;
                BoundingSphere boundingSphere = BoundingSphere.Calculate(___m_placementGhost);
                PlayerPatch.MaxPlaceDistanceMinimum = boundingSphere == null ? 5f : Math.Max(5f, 1f + boundingSphere.radius);
                foreach (System.Type type in new List<System.Type>()
                         {
                             typeof(DryLandRune),
                             typeof(RockDestroyer),
                             typeof(RockAnimator),
                             typeof(CanopyRune)
                         })
                {
                    foreach (UnityEngine.Object componentsInChild in ___m_placementGhost.GetComponentsInChildren(type))
                        UnityEngine.Object.Destroy(componentsInChild);
                }

                if (!PlaceableBoulderConfig.isPlaceableBoulder(Utils.GetPrefabName(___m_placementGhost)))
                    return;
                Debug.Log((object)"Adding magic circle");
                BoulderSetupHandler.addRuneCircle(___m_placementGhost);
                Transform[] componentsInChildren = ___m_placementGhost.GetComponentsInChildren<Transform>();
                int layer = LayerMask.NameToLayer("ghost");
                foreach (Component component in componentsInChildren)
                    component.gameObject.layer = layer;
            }
        }

        [HarmonyPatch(typeof(Player), "HaveRequirements", new System.Type[] { typeof(Piece), typeof(Player.RequirementMode) })]
        private class HaveRequirements_Patch
        {
            private static bool Postfix(
                bool __result,
                Player __instance,
                HashSet<string> ___m_knownRecipes,
                Piece piece,
                Player.RequirementMode mode)
            {
                if ((UnityEngine.Object)piece == (UnityEngine.Object)null || !UnlockManager.isUnlockable(piece))
                    return __result;
                return mode == Player.RequirementMode.IsKnown || mode == Player.RequirementMode.CanAlmostBuild ? __result && UnlockManager.isUnlocked(__instance, piece) : __result && PlayerPatch.HasEnoughEnergy(__instance, RunemagicPieceConfig.getEnergyCost(piece));
            }
        }

        [HarmonyPatch(typeof(Player), "UpdateAvailablePiecesList")]
        private class UpdateAvailablePiecesList_Patch
        {
            private static void Prefix(
                Player __instance,
                HashSet<string> ___m_knownRecipes,
                PieceTable ___m_buildPieces)
            {
                if (!((UnityEngine.Object)___m_buildPieces != (UnityEngine.Object)null) || !PlayerPatch.isRuneFocusPieceTable(___m_buildPieces))
                    return;
                foreach (GameObject piece in ___m_buildPieces.m_pieces)
                {
                    Piece component = piece.GetComponent<Piece>();
                    if (UnlockManager.isUnlockable(component) && !UnlockManager.isUnlocked(__instance, component) && ___m_knownRecipes.Contains(component.m_name))
                    {
                        Debug.Log((object)(component.m_name + " isn't unlocked, removing from known recipes list"));
                        ___m_knownRecipes.Remove(component.m_name);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), "SetPlaceMode")]
        private class SetPlaceMode_Patch
        {
            private static void Postfix(Player __instance, PieceTable buildPieces)
            {
                ConfigLoader.cacheConfig();
                List<string> stringList = Traverse.Create((object)Hud.instance).Field("m_buildCategoryNames").GetValue() as List<string>;
                if (PlayerPatch.isRuneFocusPieceTable(buildPieces))
                {
                    stringList[0] = Localization.instance.Localize("$hudcategory_runemagic_misc");
                    stringList[1] = Localization.instance.Localize("$hudcategory_runemagic_crafting");
                    stringList[2] = Localization.instance.Localize("$hudcategory_runemagic_building");
                    stringList[3] = Localization.instance.Localize("$hudcategory_runemagic_furniture");
                }
                else
                {
                    stringList[0] = Localization.instance.Localize("$hud_misc");
                    stringList[1] = Localization.instance.Localize("$hud_crafting");
                    stringList[2] = Localization.instance.Localize("$hud_building");
                    stringList[3] = Localization.instance.Localize("$hud_furniture");
                }

                if (ConfigLoader.getBool("RuneFocusEyeGlowEnabled") && PlayerPatch.isRuneFocusPieceTable(buildPieces))
                {
                    Vector4 vector4 = ConfigLoader.getVector4("RuneFocusEyeGlowColor");
                    Color color = ColorUtils.applyIntensity(new Color(vector4.x, vector4.y, vector4.z), vector4.w);
                    PlayerVFXQueue.enableLocalPlayerVFX((PlayerVFXQueue.PlayerVFX)new PlayerVFXQueue.PlayerEyeVFX(new Vector3(color.r, color.g, color.b)), "RuneFocusEyeGlow", 1);
                }
                else
                    PlayerVFXQueue.disableLocalPlayerVFX("RuneFocusEyeGlow");
            }
        }

        [HarmonyPatch(typeof(Player), "PlacePiece")]
        private class PlacePiece_Patch
        {
            private static void Prefix() => PlayerPatch.PlayerPlacing = true;

            private static bool Postfix(bool __result, Player __instance, Piece piece)
            {
                if (__result && piece.gameObject.name.Equals("runemagic_ShatteringRune"))
                {
                    int mask = LayerMask.GetMask("Default", "static_solid", "Default_small", "terrain", "vehicle");
                    Transform eye = __instance.m_eye;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out hitInfo, 50f, mask) && (bool)(UnityEngine.Object)hitInfo.collider && !(bool)(UnityEngine.Object)hitInfo.collider.attachedRigidbody)
                        RockDestroyer.create().destroyRockWithRaycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, mask);
                    else
                        Debug.Log((object)"Shatter didn't hit anything");
                }

                if (__result && piece.gameObject.name.Equals("runemagic_debugdelete"))
                {
                    int layerMask = (int)Traverse.Create((object)__instance).Field("m_placeRayMask").GetValue();
                    Transform eye = __instance.m_eye;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out hitInfo, 50f, layerMask) && (bool)(UnityEngine.Object)hitInfo.collider && !(bool)(UnityEngine.Object)hitInfo.collider.attachedRigidbody)
                        RockDestroyer.create().deleteRockWithRaycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, layerMask);
                }

                if (__result)
                {
                    try
                    {
                        ValheimMod.ValheimMod.analytics.piecePlaced(Utils.GetPrefabName(piece.gameObject));
                    }
                    catch (Exception ex)
                    {
                        ValheimMod.ValheimMod.DevModeLog(ex);
                    }
                }

                return __result;
            }

            private static Exception Finalizer(Exception __exception)
            {
                PlayerPatch.PlayerPlacing = false;
                return __exception;
            }
        }

        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        private class FixedUpdate_Patch
        {
            private static void Postfix(Player __instance, PieceTable ___m_buildPieces)
            {
                GracePeriodManager.update(Time.fixedDeltaTime);
                if ((UnityEngine.Object)__instance == (UnityEngine.Object)Player.m_localPlayer)
                {
                    if (!Hud.IsPieceSelectionVisible() && (UnityEngine.Object)___m_buildPieces != (UnityEngine.Object)null && (UnityEngine.Object)___m_buildPieces.GetSelectedPiece() != (UnityEngine.Object)null && ___m_buildPieces.GetSelectedPiece().name.Equals("runemagic_passive_WaterWalking"))
                        __instance.GetSEMan().AddStatusEffect(ObjectDB.instance.GetStatusEffect("SE_WaterWalking"), true, 0, 0.0f);
                    else
                        __instance.GetSEMan().RemoveStatusEffect(ObjectDB.instance.GetStatusEffect("SE_WaterWalking"));
                    if (!Hud.IsPieceSelectionVisible() && (UnityEngine.Object)___m_buildPieces != (UnityEngine.Object)null && (UnityEngine.Object)___m_buildPieces.GetSelectedPiece() != (UnityEngine.Object)null && ___m_buildPieces.GetSelectedPiece().name.Equals("runemagic_passive_Decumberance"))
                        __instance.GetSEMan().AddStatusEffect(ObjectDB.instance.GetStatusEffect("SE_Decumberance"), true, 0, 0.0f);
                    else
                        __instance.GetSEMan().RemoveStatusEffect(ObjectDB.instance.GetStatusEffect("SE_Decumberance"));
                    if (!Hud.IsPieceSelectionVisible() && (UnityEngine.Object)___m_buildPieces != (UnityEngine.Object)null && (UnityEngine.Object)___m_buildPieces.GetSelectedPiece() != (UnityEngine.Object)null && ___m_buildPieces.GetSelectedPiece().name.Equals("runemagic_passive_Alertness"))
                    {
                        __instance.GetSEMan().AddStatusEffect(ObjectDB.instance.GetStatusEffect("SE_Alertness"), true, 0, 0.0f);
                        if (ConfigLoader.getBool("AlertnessEyeGlowEnabled"))
                        {
                            Vector4 vector4 = ConfigLoader.getVector4("AlertnessEyeGlowColor");
                            Color color = ColorUtils.applyIntensity(new Color(vector4.x, vector4.y, vector4.z), vector4.w);
                            PlayerVFXQueue.enableLocalPlayerVFX((PlayerVFXQueue.PlayerVFX)new PlayerVFXQueue.PlayerEyeVFX(new Vector3(color.r, color.g, color.b)), "AlertnessRune", 10);
                        }
                    }
                    else
                    {
                        __instance.GetSEMan().RemoveStatusEffect(ObjectDB.instance.GetStatusEffect("SE_Alertness"));
                        if (ConfigLoader.getBool("AlertnessEyeGlowEnabled"))
                            PlayerVFXQueue.disableLocalPlayerVFX("AlertnessRune");
                    }

                    if (!Hud.IsPieceSelectionVisible() && (UnityEngine.Object)___m_buildPieces != (UnityEngine.Object)null && (UnityEngine.Object)___m_buildPieces.GetSelectedPiece() != (UnityEngine.Object)null && ___m_buildPieces.GetSelectedPiece().name.Equals("runemagic_passive_BoatPropulsion") && (UnityEngine.Object)__instance.GetControlledShip() != (UnityEngine.Object)null)
                        __instance.GetSEMan().AddStatusEffect(ObjectDB.instance.GetStatusEffect("SE_BoatPropulsion"), true, 0, 0.0f);
                    else
                        __instance.GetSEMan().RemoveStatusEffect(ObjectDB.instance.GetStatusEffect("SE_BoatPropulsion"));
                }

                if (!ConfigLoader.getBool("AlertnessEyeGlowEnabled"))
                    return;
                PlayerVFXQueue.handlePlayerVFX(__instance);
            }
        }

        [HarmonyPatch(typeof(Player), "OnInventoryChanged")]
        private class OnInventoryChanged_Patch
        {
            private static void Postfix(Player __instance, bool ___m_isLoading, Inventory ___m_inventory)
            {
                if (___m_isLoading)
                    return;
                foreach (ItemDrop.ItemData allItem in ___m_inventory.GetAllItems())
                {
                    if (ValheimMod.ValheimMod.isRuneFocus(allItem))
                    {
                        if (!Tutorial.instance.m_texts.Contains(ValheimMod.ValheimMod.RuneFocusTutorial))
                            Tutorial.instance.m_texts.Add(ValheimMod.ValheimMod.RuneFocusTutorial);
                        __instance.ShowTutorial(ValheimMod.ValheimMod.RuneFocusTutorial.m_name);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), "OnDamaged")]
        private class OnDamaged_Patch
        {
            private static void Postfix(Player __instance, HitData hit)
            {
                try
                {
                    if (!((UnityEngine.Object)__instance == (UnityEngine.Object)Player.m_localPlayer) || hit == null || !((UnityEngine.Object)hit.GetAttacker() != (UnityEngine.Object)null) || !hit.GetAttacker().IsPlayer())
                        return;
                    ValheimMod.ValheimMod.analytics.wasPvpDamageTaken = true;
                    ValheimMod.ValheimMod.DevModeLog("Pvp damage taken");
                }
                catch (Exception ex)
                {
                    ValheimMod.ValheimMod.DevModeLog(ex);
                }
            }
        }

        [HarmonyPatch(typeof(Player), "CheckCanRemovePiece")]
        private class CheckCanRemovePiece_Patch
        {
            private static bool Postfix(
                bool __result,
                Player __instance,
                PieceTable ___m_buildPieces,
                Piece piece)
            {
                if ((UnityEngine.Object)piece == (UnityEngine.Object)null || (UnityEngine.Object)___m_buildPieces == (UnityEngine.Object)null)
                    return __result;
                if (!__result)
                    return false;
                bool flag1 = PlayerPatch.isRuneFocusPieceTable(___m_buildPieces);
                bool flag2 = ValheimMod.ValheimMod.isPiecePlaceableByRuneFocus(piece);
                return flag1 ? flag2 : !flag2;
            }
        }
    }
}*/

using HarmonyLib;
using RapidLoadouts.For_Fun;
using UnityEngine;

[HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
static class PlayerOnSpawnedPatch
{
    static void Postfix(Player __instance)
    {
        __instance.gameObject.AddComponent<DryLandRune>();
    }
}

[HarmonyPatch(typeof (ZoneSystem), "Start")]
public static class ZoneSystem_Start_Patch
{
    public static void Postfix()
    {
        Debug.Log((object) "ZoneSystem_Start_Patch ran");
        //RuneStonePatch.setAccumulatorOnRuneStones(ValheimMod.isModEnabled);
        WaterSurfaceManager.addToWaterVolumes();
        //MaterialReplacer.RegisterMaterials();
        foreach (Piece piece in Resources.FindObjectsOfTypeAll<Piece>())
        {
            if (piece.gameObject.name.Equals("Karve") || piece.gameObject.name.Equals("Raft") || piece.gameObject.name.Equals("VikingShip"))
                piece.m_canBeRemoved = true;
            piece.gameObject.AddComponent<DryLandRune>();
        }
        //TerrainHolder.determineTerrainHeightBounds();
    }
}