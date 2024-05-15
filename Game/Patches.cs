using System;
using System.Resources;
using HarmonyLib;
using RapidLoadouts.UI;
using RapidLoadouts.YAMLStuff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RapidLoadouts.Game;

[HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
static class ObjectDBAwakePatch
{
    static void Postfix(ObjectDB __instance)
    {
        // Funny enough, this happens *just* after ItemSets.Awake(), so we need to add here, not in ItemSets.Awake() otherwise we won't be able to find our items!
        ItemSetHelper.AddCreateItemSets(ItemSets.instance);
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
static class StoreGuiAwakePatch
{
    static void Postfix(Player __instance)
    {
        try
        {
            // Get the root panel
            GameObject rootPanel;
            GameObject ingameGui;
            Transform augaStoreScreen;

            if (RapidLoadoutsPlugin.HasAuga)
            {
                ingameGui = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)");

                if (ingameGui == null)
                {
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("IngameGui not found");
                    return;
                }

                augaStoreScreen = Utils.FindChild(ingameGui.transform, "AugaStoreScreen(Clone)");

                if (augaStoreScreen == null)
                {
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("AugaStoreScreen not found");
                    return;
                }

                rootPanel = augaStoreScreen.gameObject;
            }
            else
            {
                rootPanel = StoreGui.instance.gameObject;

                if (rootPanel == null)
                {
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("StoreGui.instance is null");
                    return;
                }
            }

            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("Root panel: " + rootPanel.name);

            // Clone the root panel
            GameObject? newRootPanel = Object.Instantiate(rootPanel, InventoryGui.instance.m_player.transform, false);
            newRootPanel.name = "AzuRapidLoadoutsRootPanel";
            // Set the new root panel position to be middle of the screen
            newRootPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 150);
            newRootPanel.GetComponent<RectTransform>().localPosition = RapidLoadoutsPlugin.ItemSetWindow.Value;
            // Add the ItemSetGui component to the newRootPanel
            ItemSetGui? itemsetGui = newRootPanel.AddComponent<ItemSetGui>();
            if (itemsetGui == null)
            {
                RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("ItemSetGui component not found on newRootPanel");
                return;
            }

            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("ItemSetGui component found on newRootPanel");
            // Update references and settings
            ItemSetGui.m_rootPanel = Utils.FindChild(newRootPanel.transform, "Store").gameObject;
            itemsetGui.m_chooseButton = Utils.FindChild(newRootPanel.transform, "BuyButton").GetComponent<Button>();
            itemsetGui.m_chooseButton.transform.Find(RapidLoadoutsPlugin.HasAuga ? "Label" : "Text").GetComponent<TMP_Text>().text = Localization.instance.Localize("$azu_rl_buyItemSet");

            itemsetGui.m_sellButton = Utils.FindChild(newRootPanel.transform, "SellButton").GetComponent<Button>();
            itemsetGui.m_sellButton.GetComponent<UITooltip>().m_text = RapidLoadoutsPlugin.HasAuga
                ? "Not implemented, " +
                  "couldn't hide this button without making the UI look unbalanced and ugly. " +
                  "Plus side? It makes a cool coin sound at least. Figured I'd leave the sound in."
                : Localization.instance.Localize("$azu_rl_equipSelected");

            itemsetGui.m_sellButton.transform.Find("Image").gameObject.SetActive(false);
            if (!RapidLoadoutsPlugin.HasAuga)
            {
                itemsetGui.m_sellButton.transform.Find("Image (1)").gameObject.SetActive(true);
                itemsetGui.m_sellButton.transform.parent.gameObject.SetActive(false);
            }

            ItemSetGui.m_listRoot = Utils.FindChild(newRootPanel.transform, "ListRoot").GetComponent<RectTransform>();
            ItemSetGui.m_listElement = Utils.FindChild(newRootPanel.transform, "ItemElement").gameObject;

            itemsetGui.m_listScroll = Utils.FindChild(newRootPanel.transform, RapidLoadoutsPlugin.HasAuga ? "ScrollBar" : "ItemScroll").GetComponent<Scrollbar>();
            ItemSetGui.m_itemEnsureVisible = Utils.FindChild(newRootPanel.transform, RapidLoadoutsPlugin.HasAuga ? "ItemListBkg" : "Items").GetComponent<ScrollRectEnsureVisible>();

            itemsetGui.m_coinText = newRootPanel.transform.Find($"Store/{(RapidLoadoutsPlugin.HasAuga ? "DividerMedium/" : "")}coins/coins").GetComponent<TMP_Text>();
            itemsetGui.m_coinIcon = newRootPanel.transform.Find($"Store/{(RapidLoadoutsPlugin.HasAuga ? "DividerMedium/" : "")}coins/coin icon{(RapidLoadoutsPlugin.HasAuga ? "/coin icon" : "")}").GetComponent<Image>();

            if (!RapidLoadoutsPlugin.HasAuga)
            {
                newRootPanel.transform.Find("Store/coins/coin icon").GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 5);
                newRootPanel.transform.Find("Store/coins").GetComponent<RectTransform>().anchoredPosition += new Vector2(35, 0);
            }

            itemsetGui.m_topicText = Utils.FindChild(newRootPanel.transform, RapidLoadoutsPlugin.HasAuga ? "Topic" : "topic").GetComponent<TMP_Text>();


            itemsetGui.m_buyEffects = newRootPanel.GetComponent<StoreGui>().m_buyEffects;
            itemsetGui.m_sellEffects = newRootPanel.GetComponent<StoreGui>().m_sellEffects;
            itemsetGui.m_hideDistance = newRootPanel.GetComponent<StoreGui>().m_hideDistance;
            ItemSetGui.m_itemSpacing = newRootPanel.GetComponent<StoreGui>().m_itemSpacing;
            ItemSetGui.m_coinPrefab = newRootPanel.GetComponent<StoreGui>().m_coinPrefab;
            ItemSetGui.m_itemlistBaseSize = newRootPanel.GetComponent<StoreGui>().m_itemlistBaseSize;

            // Destroy the StoreGui script component
            GameObject.DestroyImmediate(newRootPanel.GetComponent<StoreGui>());

            // Add UIDragger and event handler
            UIDragger? dragger = newRootPanel.AddComponent<UIDragger>();
            dragger.OnUIDropped += (source, position) => { RapidLoadoutsPlugin.ItemSetWindow.Value = position; };
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("ItemSetGui created successfully");
        }
        catch (Exception ex)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("Exception: " + ex);
        }
    }

    /*static void Postfix(Player __instance)
    {
        try
        {
            // Get the root panel
            GameObject rootPanel;
            if (RapidLoadoutsPlugin.HasAuga)
            {
                var ingameGui = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui(Clone)");
                if (ingameGui == null)
                {
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("IngameGui not found");
                    return;
                }

                var augaStoreScreen = Utils.FindChild(ingameGui.transform, "AugaStoreScreen(Clone)");
                if (augaStoreScreen == null)
                {
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("AugaStoreScreen not found");
                    return;
                }

                rootPanel = augaStoreScreen.gameObject;
            }
            else
            {
                rootPanel = StoreGui.instance.gameObject;
                if (rootPanel == null)
                {
                    RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("StoreGui.instance is null");
                    return;
                }
            }

            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("Root panel: " + rootPanel.name);

            // Clone the root panel
            var newRootPanel = Object.Instantiate(rootPanel, InventoryGui.instance.m_player.transform, false);
            newRootPanel.name = "AzuRapidLoadoutsRootPanel";
            // Set the new root panel position to be middle of the screen
            newRootPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 150);
            newRootPanel.GetComponent<RectTransform>().localPosition = RapidLoadoutsPlugin.ItemSetWindow.Value;
            // Add the ItemSetGui component to the newRootPanel
            var itemsetGui = newRootPanel.AddComponent<ItemSetGui>();
            if (itemsetGui == null)
            {
                RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("ItemSetGui component not found on newRootPanel");
                return;
            }

            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("ItemSetGui component found on newRootPanel");
            // Update references and settings
            ItemSetGui.m_rootPanel = Utils.FindChild(newRootPanel.transform, "Store").gameObject;
            itemsetGui.m_chooseButton = Utils.FindChild(newRootPanel.transform, "BuyButton").GetComponent<Button>();
            if (RapidLoadoutsPlugin.HasAuga)
            {
                itemsetGui.m_chooseButton.transform.Find("Label").GetComponent<Text>().text = Localization.instance.Localize("$azu_rl_buyItemSet");
            }
            else
            {
                itemsetGui.m_chooseButton.transform.Find("Text").GetComponent<Text>().text = Localization.instance.Localize("$azu_rl_buyItemSet");
            }

            itemsetGui.m_sellButton = Utils.FindChild(newRootPanel.transform, "SellButton").GetComponent<Button>();
            itemsetGui.m_sellButton.GetComponent<UITooltip>().m_text = Localization.instance.Localize("$azu_rl_equipSelected");
            if (RapidLoadoutsPlugin.HasAuga)
                itemsetGui.m_sellButton.transform.Find("Image").gameObject.SetActive(false);
            else
            {
                itemsetGui.m_sellButton.transform.Find("Image").gameObject.SetActive(false);
                itemsetGui.m_sellButton.transform.Find("Image (1)").gameObject.SetActive(true);
                itemsetGui.m_sellButton.transform.parent.gameObject.SetActive(false);
            }

            ItemSetGui.m_listRoot = Utils.FindChild(newRootPanel.transform, "ListRoot").GetComponent<RectTransform>();
            ItemSetGui.m_listElement = Utils.FindChild(newRootPanel.transform, "ItemElement").gameObject;

            if (RapidLoadoutsPlugin.HasAuga)
            {
                itemsetGui.m_listScroll = Utils.FindChild(newRootPanel.transform, "ScrollBar").GetComponent<Scrollbar>();
                ItemSetGui.m_itemEnsureVisible = Utils.FindChild(newRootPanel.transform, "ItemListBkg").GetComponent<ScrollRectEnsureVisible>();
            }
            else
            {
                itemsetGui.m_listScroll = Utils.FindChild(newRootPanel.transform, "ItemScroll").GetComponent<Scrollbar>();
                ItemSetGui.m_itemEnsureVisible = Utils.FindChild(newRootPanel.transform, "Items").GetComponent<ScrollRectEnsureVisible>();
            }

            if (RapidLoadoutsPlugin.HasAuga)
                itemsetGui.m_coinText = newRootPanel.transform.Find("Store/DividerMedium/coins/coins").GetComponent<Text>();
            else
                itemsetGui.m_coinText = newRootPanel.transform.Find("Store/coins/coins").GetComponent<Text>();
            if (RapidLoadoutsPlugin.HasAuga)
            {
                itemsetGui.m_coinIcon = newRootPanel.transform.Find("Store/DividerMedium/coins/coin icon/coin icon").GetComponent<Image>();
            }
            else
            {
                itemsetGui.m_coinIcon = newRootPanel.transform.Find("Store/coins/coin icon").GetComponent<Image>();
            }

            itemsetGui.m_topicText = Utils.FindChild(newRootPanel.transform, RapidLoadoutsPlugin.HasAuga ? "Topic" : "topic").GetComponent<Text>();

            itemsetGui.m_buyEffects = newRootPanel.GetComponent<StoreGui>().m_buyEffects;
            itemsetGui.m_sellEffects = newRootPanel.GetComponent<StoreGui>().m_sellEffects;
            itemsetGui.m_hideDistance = newRootPanel.GetComponent<StoreGui>().m_hideDistance;
            ItemSetGui.m_itemSpacing = newRootPanel.GetComponent<StoreGui>().m_itemSpacing;
            ItemSetGui.m_coinPrefab = newRootPanel.GetComponent<StoreGui>().m_coinPrefab;
            ItemSetGui.m_itemlistBaseSize = newRootPanel.GetComponent<StoreGui>().m_itemlistBaseSize;

            // Destroy the StoreGui script component
            GameObject.DestroyImmediate(newRootPanel.GetComponent<StoreGui>());

            // Add UIDragger and event handler
            var dragger = newRootPanel.AddComponent<UIDragger>();
            dragger.OnUIDropped += (source, position) => { RapidLoadoutsPlugin.ItemSetWindow.Value = position; };
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("ItemSetGui created successfully");
        }
        catch (Exception ex)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("Exception: " + ex);
        }
    }*/
}