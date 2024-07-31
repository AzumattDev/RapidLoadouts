using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RapidLoadouts.YAMLStuff;

public static class ItemSetHelper
{
    public static void AddCreateLoadout(ItemSets? __instance)
    {
        if (__instance == null) return;
        // Convert customSets to game's Loadout
        List<ItemSets.ItemSet> customSets = ItemSetHelper.ConvertToGameItemSets(RapidLoadoutsPlugin.RL_yamlData);

        // Create a dictionary for the fast lookup
        Dictionary<string, ItemSets.ItemSet> customSetDict = customSets.ToDictionary(set => set.m_name, set => set);

        // Iterate through m_sets and replace or add
        for (int i = 0; i < __instance.m_sets.Count; ++i)
        {
            ItemSets.ItemSet set = __instance.m_sets[i];
            if (!customSetDict.ContainsKey(set.m_name)) continue;
            __instance.m_sets[i] = customSetDict[set.m_name];
            customSetDict.Remove(set.m_name);
        }

        // Add the remaining item sets in customSetDict to m_sets
        __instance.m_sets.AddRange(customSetDict.Values);
    }

    public static List<ItemSets.ItemSet> ConvertToGameItemSets(List<ItemSet?> customLoadouts)
    {
        List<ItemSets.ItemSet> gameLoadouts = new List<ItemSets.ItemSet>();

        foreach (ItemSet? customSet in customLoadouts)
        {
            if (customSet != null)
            {
                ItemSets.ItemSet gameSet = new ItemSets.ItemSet
                {
                    m_name = RegexUtilities.TrimInvalidCharacters(customSet.m_name),
                    m_items = new List<ItemSets.SetItem>(),
                    m_skills = new List<ItemSets.SetSkill>()
                };

                foreach (SetItem? customItem in customSet.m_items)
                {
                    ItemSets.SetItem gameItem = new ItemSets.SetItem
                    {
                        m_item = ConvertToItemDrop(customItem.m_item), // This method should convert string to ItemDrop
                        m_quality = customItem.m_quality,
                        m_stack = customItem.m_stack,
                        m_use = customItem.m_use,
                        m_hotbarSlot = customItem.m_hotbarSlot
                    };

                    gameSet.m_items.Add(gameItem);
                }

                foreach (SetSkill? customSkill in customSet.m_skills)
                {
                    ItemSets.SetSkill gameSkill = new ItemSets.SetSkill
                    {
                        m_skill = ConvertToSkillType(customSkill.m_skill), // This method should convert string to Skills.SkillType
                        m_level = customSkill.m_level
                    };

                    gameSet.m_skills.Add(gameSkill);
                }

                gameLoadouts.Add(gameSet);
            }
        }

        return gameLoadouts;
    }

    internal static ItemDrop? ConvertToItemDrop(string itemName)
    {
        if (ObjectDB.instance == null) return null;
        GameObject? fabby = ObjectDB.instance.GetItemPrefab(itemName);
        if (fabby != null)
        {
            return fabby.GetComponent<ItemDrop>();
        }

        // Log error
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError($"ItemSetHelper.ConvertToItemDrop: Prefab for {itemName} not found in the ObjectDB! Can't convert to ItemDrop!");
        return null;
    }

    internal static Skills.SkillType ConvertToSkillType(string skillName)
    {
        if (Enum.TryParse(skillName, true, out Skills.SkillType skillType))
        {
            return skillType;
        }

        // Log error
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError($"ItemSetHelper.ConvertToSkillType: {skillName} is not a valid skill name or isn't a vanilla skill! Can't convert to Skills.SkillType!");
        return Skills.SkillType.None;
    }

    public static bool TryGetSet(string name, bool dropCurrentItems = false)
    {
        if (Player.m_localPlayer == null || !ItemSets.instance.GetSetDictionary().TryGetValue(name, out ItemSets.ItemSet itemSet))
            return false;

        if (dropCurrentItems)
            DropCurrentItems();

        PopulateInventoryWithSetItems(itemSet);
        ResetAndRaiseSkills(itemSet);

        return true;
    }

    private static void DropCurrentItems()
    {
        Skills skills = Player.m_localPlayer.GetSkills();

        Player.m_localPlayer.CreateTombStone();
        Player.m_localPlayer.ClearFood();
        Player.m_localPlayer.ClearHardDeath();
        Player.m_localPlayer.GetSEMan().RemoveAllStatusEffects();

        foreach (Skills.SkillDef skill in skills.m_skills)
            skills.CheatResetSkill(skill.m_skill.ToString());
    }

    private static void PopulateInventoryWithSetItems(ItemSets.ItemSet itemSet)
    {
        Inventory inventory = Player.m_localPlayer.GetInventory();
        InventoryGui.instance.m_playerGrid.UpdateInventory(inventory, Player.m_localPlayer, null);

        foreach (ItemSets.SetItem setItem in itemSet.m_items)
        {
            if (setItem.m_item == null)
                continue;
            if (setItem.m_item.m_itemData.m_shared.m_dlc.Length > 0 && !DLCMan.instance.IsDLCInstalled(setItem.m_item.m_itemData.m_shared.m_dlc))
            {
                continue;
            }

            int amount = Math.Max(1, setItem.m_stack);
            ItemDrop.ItemData itemData = inventory.AddItem(setItem.m_item.gameObject.name, amount, Math.Max(1, setItem.m_quality), 0, 0L, "Thor");

            if (itemData != null)
            {
                if (setItem.m_use)
                    Player.m_localPlayer.UseItem(inventory, itemData, false);

                if (setItem.m_hotbarSlot > 0)
                    InventoryGui.instance.m_playerGrid.DropItem(inventory, itemData, amount, new Vector2i(setItem.m_hotbarSlot - 1, 0));
            }
        }
    }

    internal static void SetAnyGuardianPower(string setEffect)
    {
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug($"Setting effect to {setEffect}");
        Player.m_localPlayer.SetGuardianPower(setEffect);
        Player.m_localPlayer.m_guardianPowerCooldown = 0.0f;
    }

    private static void ResetAndRaiseSkills(ItemSets.ItemSet itemSet)
    {
        Skills skills = Player.m_localPlayer.GetSkills();

        foreach (ItemSets.SetSkill skill in itemSet.m_skills)
        {
            skills.CheatResetSkill(skill.m_skill.ToString());
            Player.m_localPlayer.GetSkills().CheatRaiseSkill(skill.m_skill.ToString(), skill.m_level);
        }
    }

    public static string Repeat(string value, int count)
    {
        return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
    }
}