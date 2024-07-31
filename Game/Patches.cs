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
        ItemSetHelper.AddCreateLoadout(ItemSets.instance);
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
static class PlayerSpawnedPatch
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

            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug("Root panel: " + rootPanel.name);
            CreateMainPanel(rootPanel, __instance, out GameObject? newRootPanel, out PurchasableLoadoutGui? itemsetGui);

            if (newRootPanel == null || itemsetGui == null)
            {
                RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("Failed to create main panel, second panel not created");
                return;
            }

            {
                CreateSecondPanel(newRootPanel, itemsetGui);
            }

            // Log success
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug("Second panel created successfully");
        }
        catch (Exception ex)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("Exception: " + ex);
        }
    }

    private static void CreateMainPanel(GameObject rootPanel, Player player, out GameObject clonedRootPanel, out PurchasableLoadoutGui gui)
    {
        // Clone the root panel
        GameObject newRootPanel = Object.Instantiate(rootPanel, InventoryGui.instance.m_player.transform, false);
        newRootPanel.name = "AzuRapidLoadoutsRootPanel";
        Utils.FindChild(newRootPanel.transform, "border (1)").gameObject.GetComponent<Image>().sprite = player.m_inventory.GetBkg();
        clonedRootPanel = newRootPanel;
        // Set the new root panel position to be middle of the screen
        newRootPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 150);
        newRootPanel.GetComponent<RectTransform>().localPosition = RapidLoadoutsPlugin.LoadoutWindow.Value;

        // Add the PurchasableLoadoutGui component to the newRootPanel
        PurchasableLoadoutGui itemsetGui = newRootPanel.AddComponent<PurchasableLoadoutGui>();
        if (itemsetGui == null)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug("PurchasableLoadoutGui component not found on newRootPanel");
            gui = null;
            return;
        }

        gui = itemsetGui;

        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug("PurchasableLoadoutGui component found on newRootPanel");
        // Update references and settings
        PurchasableLoadoutGui.m_rootPanel = Utils.FindChild(newRootPanel.transform, "Store").gameObject;
        itemsetGui.m_chooseButton = Utils.FindChild(newRootPanel.transform, "BuyButton").GetComponent<Button>();
        itemsetGui.m_chooseButton.transform.Find(RapidLoadoutsPlugin.HasAuga ? "Label" : "Text").GetComponent<TMP_Text>().text = Localization.instance.Localize("$azu_rl_buyLoadout");

        itemsetGui.m_sellButton = Utils.FindChild(newRootPanel.transform, "SellButton").GetComponent<Button>();
        itemsetGui.m_sellButton.GetComponent<UITooltip>().m_text = RapidLoadoutsPlugin.HasAuga
            ? "Not implemented, couldn't hide this button without making the UI look unbalanced and ugly. Plus side? It makes a cool coin sound at least. Figured I'd leave the sound in."
            : Localization.instance.Localize("$azu_rl_equipSelected");

        itemsetGui.m_sellButton.transform.Find("Image").gameObject.SetActive(false);
        if (!RapidLoadoutsPlugin.HasAuga)
        {
            itemsetGui.m_sellButton.transform.Find("Image (1)").gameObject.SetActive(true);
            itemsetGui.m_sellButton.transform.parent.gameObject.SetActive(false);
        }

        PurchasableLoadoutGui.m_listRoot = Utils.FindChild(newRootPanel.transform, "ListRoot").GetComponent<RectTransform>();
        PurchasableLoadoutGui.m_listElement = Utils.FindChild(newRootPanel.transform, "ItemElement").gameObject;

        itemsetGui.m_listScroll = Utils.FindChild(newRootPanel.transform, RapidLoadoutsPlugin.HasAuga ? "ScrollBar" : "ItemScroll").GetComponent<Scrollbar>();
        PurchasableLoadoutGui.m_itemEnsureVisible = Utils.FindChild(newRootPanel.transform, RapidLoadoutsPlugin.HasAuga ? "ItemListBkg" : "Items").GetComponent<ScrollRectEnsureVisible>();

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
        PurchasableLoadoutGui.m_itemSpacing = newRootPanel.GetComponent<StoreGui>().m_itemSpacing;
        PurchasableLoadoutGui.m_coinPrefab = newRootPanel.GetComponent<StoreGui>().m_coinPrefab;
        PurchasableLoadoutGui.m_itemlistBaseSize = newRootPanel.GetComponent<StoreGui>().m_itemlistBaseSize;

        // Destroy the StoreGui script component
        GameObject.DestroyImmediate(newRootPanel.GetComponent<StoreGui>());

        // Add UIDragger and event handler
        UIDragger dragger = newRootPanel.AddComponent<UIDragger>();
        dragger.OnUIDropped += (source, position) => { RapidLoadoutsPlugin.LoadoutWindow.Value = position; };
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug("PurchasableLoadoutGui created successfully");
    }

    private static void CreateSecondPanel(GameObject newRootPanel, PurchasableLoadoutGui itemsetGui)
    {
        // Instantiate the new panel
        GameObject secondPanel = Object.Instantiate(newRootPanel, newRootPanel.transform);
        secondPanel.name = "AzuRapidLoadoutsSecondPanel";
        // Destroy the UIDragger component
        GameObject.DestroyImmediate(secondPanel.GetComponent<UIDragger>());
        GameObject.DestroyImmediate(secondPanel.GetComponent<PurchasableLoadoutGui>());
        PersonalLoadoutGui personalLoadoutGui = newRootPanel.AddComponent<PersonalLoadoutGui>();
        if (personalLoadoutGui == null)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebugIfDebug("PurchasableLoadoutGui component not found on newRootPanel");
            return;
        }

        PersonalLoadoutGui.m_rootPanel = Utils.FindChild(secondPanel.transform, "Store").gameObject;
        personalLoadoutGui.m_chooseButton = Utils.FindChild(secondPanel.transform, "BuyButton").GetComponent<Button>();
        personalLoadoutGui.m_chooseButton.onClick.RemoveAllListeners();
        personalLoadoutGui.m_chooseButton.transform.Find(RapidLoadoutsPlugin.HasAuga ? "Label" : "Text").GetComponent<TMP_Text>().text = Localization.instance.Localize("$azu_rl_emptyItemSet");

        personalLoadoutGui.m_sellButton = Utils.FindChild(newRootPanel.transform, "SellButton").GetComponent<Button>();
        personalLoadoutGui.m_sellButton.GetComponent<UITooltip>().m_text = RapidLoadoutsPlugin.HasAuga
            ? "Not implemented, couldn't hide this button without making the UI look unbalanced and ugly. Plus side? It makes a cool coin sound at least. Figured I'd leave the sound in."
            : Localization.instance.Localize("$azu_rl_equipSelected");

        personalLoadoutGui.m_sellButton.transform.Find("Image").gameObject.SetActive(false);
        if (!RapidLoadoutsPlugin.HasAuga)
        {
            personalLoadoutGui.m_sellButton.transform.Find("Image (1)").gameObject.SetActive(true);
            personalLoadoutGui.m_sellButton.transform.parent.gameObject.SetActive(false);
        }

        RectTransform secondPanelRT = secondPanel.GetComponent<RectTransform>();
        secondPanelRT.localPosition = new Vector3(250, 0, 0); // Position relative to the first panel

        // Remove unnecessary elements from the second panel
        UnityEngine.Object.Destroy(Utils.FindChild(secondPanel.transform, "SellPanel").gameObject);
        UnityEngine.Object.Destroy(Utils.FindChild(secondPanel.transform, "border (1)").gameObject);
        UnityEngine.Object.Destroy(Utils.FindChild(secondPanel.transform, "bkg").gameObject);

        // Update references for the second panel
        PersonalLoadoutGui.m_listRoot = Utils.FindChild(secondPanel.transform, "ListRoot").GetComponent<RectTransform>();
        PersonalLoadoutGui.m_listElement = Utils.FindChild(secondPanel.transform, "ItemElement").gameObject;

        personalLoadoutGui.m_listScroll = Utils.FindChild(secondPanel.transform, RapidLoadoutsPlugin.HasAuga ? "ScrollBar" : "ItemScroll").GetComponent<Scrollbar>();
        PersonalLoadoutGui.m_itemEnsureVisible = Utils.FindChild(secondPanel.transform, RapidLoadoutsPlugin.HasAuga ? "ItemListBkg" : "Items").GetComponent<ScrollRectEnsureVisible>();

        personalLoadoutGui.m_coinText = secondPanel.transform.Find($"Store/{(RapidLoadoutsPlugin.HasAuga ? "DividerMedium/" : "")}coins/coins").GetComponent<TMP_Text>();
        personalLoadoutGui.m_coinIcon = secondPanel.transform.Find($"Store/{(RapidLoadoutsPlugin.HasAuga ? "DividerMedium/" : "")}coins/coin icon{(RapidLoadoutsPlugin.HasAuga ? "/coin icon" : "")}").GetComponent<Image>();

        if (!RapidLoadoutsPlugin.HasAuga)
        {
            secondPanel.transform.Find("Store/coins/coin icon").GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 5);
            secondPanel.transform.Find("Store/coins").GetComponent<RectTransform>().anchoredPosition += new Vector2(35, 0);
        }

        personalLoadoutGui.m_topicText = Utils.FindChild(secondPanel.transform, RapidLoadoutsPlugin.HasAuga ? "Topic" : "topic").GetComponent<TMP_Text>();


        personalLoadoutGui.m_buyEffects = itemsetGui.m_buyEffects;
        personalLoadoutGui.m_sellEffects = itemsetGui.m_sellEffects;
        personalLoadoutGui.m_hideDistance = itemsetGui.m_hideDistance;
        PersonalLoadoutGui.m_itemSpacing = PurchasableLoadoutGui.m_itemSpacing;
        PersonalLoadoutGui.m_coinPrefab = PurchasableLoadoutGui.m_coinPrefab;
        PersonalLoadoutGui.m_itemlistBaseSize = PurchasableLoadoutGui.m_itemlistBaseSize;

        // Stretch the original panel to fit both panels
        Utils.FindChild(newRootPanel.transform, "border (1)").GetComponent<RectTransform>().anchorMax = new Vector2(2, 1);
    }
}