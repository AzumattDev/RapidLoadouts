using HarmonyLib;
using RapidLoadouts.UI;
using RapidLoadouts.YAMLStuff;
using UnityEngine;
using UnityEngine.UI;

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
        // Get the root panel
        if (StoreGui.instance == null)
        {
            Debug.LogError("StoreGui.instance is null");
            return;
        }

        var rootPanel = StoreGui.instance.gameObject;

        // Clone the root panel
        var newRootPanel = GameObject.Instantiate(rootPanel, InventoryGui.instance.m_player.transform, false);
        newRootPanel.name = "AzuRapidLoadoutsRootPanel";
        // Set the new root panel position to be middle of the screen
        newRootPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 150);
        newRootPanel.GetComponent<RectTransform>().localPosition = RapidLoadoutsPlugin.ItemSetWindow.Value;
        var dragger = newRootPanel.gameObject.AddComponent<UIDragger>();
        dragger.OnUIDropped += (source, position) => { RapidLoadoutsPlugin.ItemSetWindow.Value = position; };

        // Add the ItemSetGui component to the newRootPanel
        var itemsetGui = newRootPanel.AddComponent<ItemSetGui>();


        newRootPanel.AddComponent<UIDragger>();
        var traderComponent = newRootPanel.GetComponent<StoreGui>();

        if (traderComponent == null)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("traderComponent is null");
            return;
        }

        if (itemsetGui == null)
        {
            RapidLoadoutsPlugin.RapidLoadoutsLogger.LogError("itemsetGui is null");
            return;
        }

        ItemSetGui.m_rootPanel = Utils.FindChild(newRootPanel.transform, "Store").gameObject;
        itemsetGui.m_chooseButton = Utils.FindChild(newRootPanel.transform, "BuyButton").GetComponent<Button>();
        itemsetGui.m_chooseButton.transform.Find("Text").GetComponent<Text>().text = Localization.instance.Localize("$azu_rl_buyItemSet");
        itemsetGui.m_sellButton = Utils.FindChild(newRootPanel.transform, "SellButton").GetComponent<Button>();
        itemsetGui.m_sellButton.GetComponent<UITooltip>().m_text = Localization.instance.Localize("$azu_rl_equipSelected");
        itemsetGui.m_sellButton.transform.Find("Image").gameObject.SetActive(false);
        itemsetGui.m_sellButton.transform.Find("Image (1)").gameObject.SetActive(true);
        itemsetGui.m_sellButton.transform.parent.gameObject.SetActive(false);
        ItemSetGui.m_listRoot = Utils.FindChild(newRootPanel.transform, "ListRoot").GetComponent<RectTransform>();
        ItemSetGui.m_listElement = Utils.FindChild(newRootPanel.transform, "ItemElement").gameObject;
        itemsetGui.m_listScroll = Utils.FindChild(newRootPanel.transform, "ItemScroll").GetComponent<Scrollbar>();
        ItemSetGui.m_itemEnsureVisible = Utils.FindChild(newRootPanel.transform, "Items").GetComponent<ScrollRectEnsureVisible>();
        itemsetGui.m_coinText = newRootPanel.transform.Find("Store/coins/coins").GetComponent<Text>();
        itemsetGui.m_coinIcon = newRootPanel.transform.Find("Store/coins/coin icon").GetComponent<Image>();
        newRootPanel.transform.Find("Store/coins/coin icon").GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 5);
        newRootPanel.transform.Find("Store/coins").GetComponent<RectTransform>().anchoredPosition += new Vector2(35, 0);
        itemsetGui.m_topicText = Utils.FindChild(newRootPanel.transform, "topic").GetComponent<Text>();
        itemsetGui.m_buyEffects = traderComponent.m_buyEffects;
        itemsetGui.m_sellEffects = traderComponent.m_sellEffects;
        itemsetGui.m_hideDistance = traderComponent.m_hideDistance;
        ItemSetGui.m_itemSpacing = traderComponent.m_itemSpacing;
        ItemSetGui.m_coinPrefab = traderComponent.m_coinPrefab;
        ItemSetGui.m_itemlistBaseSize = traderComponent.m_itemlistBaseSize;

        // Destroy the StoreGui script component
        GameObject.DestroyImmediate(newRootPanel.GetComponent<StoreGui>());
    }
}