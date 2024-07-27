using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidLoadouts.YAMLStuff;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RapidLoadouts.UI;

public class ItemSetGui : MonoBehaviour
{
    public static ItemSetGui? m_instance = null!;
    public static GameObject m_rootPanel = null!;
    public Button m_chooseButton = null!;

    public Button m_sellButton = null!;

    //private Button _itemsetsButton = null!;
    //private Text _textComponent = null!;
    //private Image _imageComponent = null!;
    public static RectTransform m_listRoot = null!;
    public static GameObject m_listElement = null!;
    public Scrollbar m_listScroll = null!;
    public static ScrollRectEnsureVisible m_itemEnsureVisible = null!;
    public TMP_Text m_coinText = null!;
    public Image m_coinIcon = null!;
    public TMP_Text m_topicText = null!;
    public EffectList m_buyEffects = new();
    public EffectList m_sellEffects = new();
    public float m_hideDistance = 5f;
    public static float m_itemSpacing = 64f;
    public static ItemDrop m_coinPrefab = null!;
    public static List<GameObject> m_itemList = new();
    public static ItemSet? m_selectedItem;
    public static Player? m_LocalPlayerRef;
    public static float m_itemlistBaseSize;
    public List<ItemDrop.ItemData> m_tempItems = new();
    static List<ItemSet?> availableSets = new();

    public static bool PanelActive;

    public static ItemSetGui? instance => m_instance;

    public void Awake()
    {
        m_instance = this;
        if (m_rootPanel != null)
            m_rootPanel.SetActive(false);
        if (m_listRoot != null)
            m_itemlistBaseSize = m_listRoot.rect.height;
        m_chooseButton.onClick = new Button.ButtonClickedEvent();
        m_chooseButton.onClick.AddListener(OnChooseItemSet);
        m_sellButton.onClick = new Button.ButtonClickedEvent();
        m_topicText.text = Localization.instance.Localize("$azu_rl_itemSets");
    }

    public void OnDestroy()
    {
        if (!(m_instance == this))
            return;
        m_instance = null;
    }

    private void Update()
    {
        if (!m_rootPanel.activeSelf || !PanelActive) return;
        if (ShouldHide() || ShouldClose())
        {
            Hide();
        }
        else
        {
            UpdateUI();
        }
    }

    private bool ShouldHide()
    {
        Player localPlayer = Player.m_localPlayer;

        if (localPlayer == null || localPlayer.IsDead() || localPlayer.InCutscene())
            return true;

        if (!InventoryGui.IsVisible() || Minimap.IsOpen())
            return true;

        return false;
    }

    private static bool ShouldClose()
    {
        Player localPlayer = Player.m_localPlayer;
        bool isUIBlocking = (Chat.instance != null && Chat.instance.HasFocus()) || Console.IsVisible() || Menu.IsVisible() || (TextViewer.instance != null && TextViewer.instance.IsVisible()) || localPlayer.InCutscene();

        if (isUIBlocking && (ZInput.GetButtonDown("JoyButtonB") || Input.GetKeyDown(KeyCode.Escape) || ZInput.GetButtonDown("Use")))
        {
            ZInput.ResetButtonStatus("JoyButtonB");
            return true;
        }

        return false;
    }

    private void UpdateUI()
    {
        UpdateBuyButton();
        UpdateRecipeGamepadInput();
        m_coinText.text = GetPlayerCoins().ToString();
        try
        {
            m_coinPrefab = ObjectDB.instance.GetItemPrefab(m_selectedItem?.m_prefabCost).GetComponent<ItemDrop>();
        }
        catch
        {
            m_coinPrefab = ObjectDB.instance.GetItemPrefab(RapidLoadoutsPlugin.itemSetCostPrefab.Value).GetComponent<ItemDrop>();
        }

        m_coinIcon.sprite = m_coinPrefab.m_itemData.GetIcon();
    }

    public static void Show()
    {
        if (Player.m_localPlayer == null)
            return;
        if (IsVisible())
            return;
        m_LocalPlayerRef = Player.m_localPlayer;
        InventoryGui.instance.m_dropButton.gameObject.SetActive(false);
        if (!RapidLoadoutsPlugin.HasAuga)
        {
            if (Utils.FindChild(m_rootPanel.transform, "border (1)").GetComponent<Image>() != Utils.FindChild(StoreGui.instance.transform, "border (1)").GetComponent<Image>().sprite)
            {
                Utils.FindChild(m_rootPanel.transform, "border (1)").GetComponent<Image>().sprite = Utils.FindChild(StoreGui.instance.transform, "border (1)").GetComponent<Image>().sprite; //Utils.FindChild(InventoryGui.instance.m_player.transform, "bkg").GetComponent<Image>().sprite;
            }

            Utils.FindChild(m_rootPanel.transform, "border (1)").GetComponent<Image>().color = Utils.FindChild(InventoryGui.instance.m_player.transform, "bkg").GetComponent<Image>().color;
        }

        m_rootPanel.SetActive(true);
        PanelActive = true;
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("Showing Item Set GUI");
        FillList();
    }

    public static void Hide()
    {
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("Hiding Item Set GUI");
        m_LocalPlayerRef = null;
        m_rootPanel.SetActive(false);
        PanelActive = false;
        InventoryGui.instance.m_dropButton.gameObject.SetActive(true);
    }

    public static void ToggleUI()
    {
        if (IsVisible())
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public static bool IsVisible() => (m_instance && m_rootPanel.activeSelf) || PanelActive;

    public void OnChooseItemSet() => BuySelectedItemSet();

    public void BuySelectedItemSet()
    {
        if (m_selectedItem == null || !CanAfford(m_selectedItem))
            return;
        ItemSetHelper.TryGetSet(RegexUtilities.TrimInvalidCharacters(m_selectedItem.m_name), m_selectedItem.m_dropCurrent);
        if (!string.IsNullOrEmpty(m_selectedItem.m_setEffect))
        {
            if (m_selectedItem.m_setEffectAsGP)
            {
                ItemSetHelper.SetAnyGuardianPower(m_selectedItem.m_setEffect);
            }
            else
            {
                Player.m_localPlayer.m_seman.AddStatusEffect(m_selectedItem.m_setEffect.GetStableHashCode());
            }
        }

        if (!Player.m_localPlayer.NoCostCheat())
        {
            Player.m_localPlayer.GetInventory().RemoveItem(m_coinPrefab.m_itemData.m_shared.m_name, m_selectedItem.m_price);
        }

        m_buyEffects.Create(transform.position, Quaternion.identity);
        FillList();
    }

    public static int GetPlayerCoins() => Player.m_localPlayer.GetInventory().CountItems(m_coinPrefab.m_itemData.m_shared.m_name);

    public bool CanAfford(ItemSet? item)
    {
        int playerCoins = GetPlayerCoins();
        return item != null && (item.m_price <= playerCoins || Player.m_localPlayer.NoCostCheat());
    }

    public static List<ItemSet?> GetAvailableSets()
    {
        availableSets.Clear();
        if (availableSets.Any())
        {
            return availableSets;
        }

        if (RapidLoadoutsPlugin.RL_yamlData != null)
        {
            availableSets.AddRange(RapidLoadoutsPlugin.RL_yamlData);
        }

        return availableSets;
    }

    public static void FillList()
    {
        int playerCoins = GetPlayerCoins();
        int index1 = GetSelectedItemIndex();
        List<ItemSet?> availableItems = GetAvailableSets().Where(x=> x!= null && (string.IsNullOrEmpty(x.m_requiredGlobalKey) || ZoneSystem.instance.GetGlobalKey(x.m_requiredGlobalKey))).ToList();

        foreach (Object @object in m_itemList)
            Destroy(@object);
        m_itemList.Clear();

        m_listRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(m_itemlistBaseSize, availableItems.Count * m_itemSpacing));

        for (int index2 = 0; index2 < availableItems.Count; ++index2)
        {
            ItemSet? itemSet = availableItems[index2];
            GameObject element = Instantiate(m_listElement, m_listRoot);
            element.SetActive(true);
            ((element.transform as RectTransform)!).anchoredPosition = new Vector2(0.0f, index2 * -m_itemSpacing);

            int price = 0;
            // If the yaml file has a price, use that. Otherwise, use the default price.
            if (itemSet != null && itemSet.m_price != 0)
            {
                price = itemSet.m_price;
            }

            bool flag = price <= playerCoins || Player.m_localPlayer.NoCostCheat();
            Image elementIcon = element.transform.Find("icon").GetComponent<Image>();

            if (itemSet?.m_items == null || itemSet.m_items.Count == 0 || ItemSetHelper.ConvertToItemDrop(itemSet.m_items[0].m_item) == null)

            {
                elementIcon.sprite = ObjectDB.instance.GetItemPrefab("YagluthDrop").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
            }
            else
            {
                elementIcon.sprite = ItemSetHelper.ConvertToItemDrop(itemSet.m_items[0].m_item)?.m_itemData?.m_shared?.m_icons[0];
            }


            elementIcon.color = flag ? Color.white : new Color(1f, 0.0f, 1f, 0.0f);
            string str = Localization.instance.Localize(itemSet?.m_name ?? "Unknown Item Set");
            TMP_Text elementName = element.transform.Find("name").GetComponent<TMP_Text>();
            elementName.text = str;
            elementName.color = flag ? Color.white : Color.grey;
            UITooltip elementTooltip = element.GetComponent<UITooltip>();
            elementTooltip.m_topic = itemSet?.m_name ?? "Unknown Item Set";

            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Drop Current Items: {itemSet is { m_dropCurrent: true }}");

            // append all skills from the itemSet.m_skills
            stringBuilder.Append(Environment.NewLine + "Skills: ");
            if (itemSet is { m_skills.Count: > 0 })
            {
                foreach (SetSkill? skill in itemSet.m_skills)
                {
                    stringBuilder.Append($"{skill.m_skill} {skill.m_level}{Environment.NewLine}");
                }
            }

            if (itemSet?.m_items != null)
            {
                if (itemSet.m_items.Count > 0)
                {
                    foreach (SetItem? item in itemSet.m_items)
                    {
                        string? itemName = ItemSetHelper.ConvertToItemDrop(item.m_item)?.m_itemData?.m_shared?.m_name;
                        if (!string.IsNullOrEmpty(itemName))
                        {
                            stringBuilder.Append($"{Environment.NewLine}{Localization.instance.Localize(itemName)} x{ItemSetHelper.Repeat("\u2605", item.m_quality)}");
                        }
                    }
                }
            }


            if (!string.IsNullOrEmpty(itemSet?.m_setEffect))
            {
                stringBuilder.Append($"{Environment.NewLine}Set Effect: {itemSet?.m_setEffect}");
            }

            elementTooltip.m_text = stringBuilder.ToString();
            TMP_Text component4 = Utils.FindChild(element.transform, "price").GetComponent<TMP_Text>();
            component4.text = Player.m_localPlayer.NoCostCheat() ? "Free" : itemSet?.m_price.ToString();
            if (!flag)
                component4.color = Color.grey;
            element.GetComponent<Button>().onClick.AddListener(() => OnSelectedItem(element));
            m_itemList.Add(element);
        }

        if (index1 < 0)
            index1 = 0;
        SelectItem(index1, false);
    }


    public static void OnSelectedItem(GameObject button) => SelectItem(FindSelectedRecipe(button), false);

    public static int FindSelectedRecipe(GameObject button)
    {
        for (int index = 0; index < m_itemList.Count; ++index)
        {
            if (m_itemList[index] == button)
                return index;
        }

        return -1;
    }

    public static void SelectItem(int index, bool center)
    {
        RapidLoadoutsPlugin.RapidLoadoutsLogger.LogDebug("Setting selected recipe " + index.ToString());
        for (int index1 = 0; index1 < m_itemList.Count; ++index1)
        {
            bool flag = index1 == index;
            m_itemList[index1].transform.Find("selected").gameObject.SetActive(flag);
        }

        if (center && index >= 0)
            m_itemEnsureVisible.CenterOnItem(m_itemList[index].transform as RectTransform);
        m_selectedItem = index < 0 ? null : GetAvailableSets()[index];
    }


    public static int GetSelectedItemIndex()
    {
        int selectedItemIndex = 0;
        List<ItemSet?> availableItems = GetAvailableSets();
        for (int index = 0; index < availableItems.Count; ++index)
        {
            if (availableItems[index] == m_selectedItem)
                selectedItemIndex = index;
        }

        return selectedItemIndex;
    }

    public void UpdateBuyButton()
    {
        UITooltip component = m_chooseButton.GetComponent<UITooltip>();
        if (m_selectedItem != null)
        {
            bool flag1 = CanAfford(m_selectedItem);
            bool flag2 = Player.m_localPlayer.GetInventory().HaveEmptySlot();
            m_chooseButton.interactable = flag1 & flag2;
            if (!flag1)
                component.m_text = Localization.instance.Localize("$msg_missingrequirement");
            else if (!flag2)
                component.m_text = Localization.instance.Localize("$inventory_full");
            else
                component.m_text = "";
        }
        else
        {
            m_chooseButton.interactable = false;
            component.m_text = "";
        }
    }

    public void UpdateRecipeGamepadInput()
    {
        if (m_itemList.Count <= 0)
            return;
        if (ZInput.GetButtonDown("JoyLStickDown") || ZInput.GetButtonDown("JoyDPadDown"))
            SelectItem(Mathf.Min(m_itemList.Count - 1, GetSelectedItemIndex() + 1), true);
        if (!ZInput.GetButtonDown("JoyLStickUp") && !ZInput.GetButtonDown("JoyDPadUp"))
            return;
        SelectItem(Mathf.Max(0, GetSelectedItemIndex() - 1), true);
    }
}