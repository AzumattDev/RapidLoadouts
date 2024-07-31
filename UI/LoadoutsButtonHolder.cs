using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RapidLoadouts.UI
{
    public class LoadoutsButtonHolder : MonoBehaviour
    {
        private Button _itemSetsButton = null!;
        private TMP_Text _textComponent = null!;

        private void Start()
        {
            InvokeRepeating(nameof(EnsureRecyclingButtonExistsIfPossible), 0f, 5f);
        }

        public void EnsureRecyclingButtonExistsIfPossible()
        {
            if (InventoryGui.instance == null) return;
            if (_itemSetsButton == null)
            {
                SetupButton();
            }

            _itemSetsButton?.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            try
            {
                Destroy(_itemSetsButton.gameObject);
            }
            catch
            {
                // ignored
            }
        }

        private void SetupButton()
        {
            if (_itemSetsButton != null)
                return;
            if (RapidLoadoutsPlugin.HasAuga)
            {
                _itemSetsButton = Instantiate(InventoryGui.instance.m_container.Find("TakeAll").GetComponent<Button>(), InventoryGui.instance.m_player.transform);
            }
            else
            {
                _itemSetsButton = Instantiate(InventoryGui.instance.m_takeAllButton, InventoryGui.instance.m_player.transform);
            }

            _itemSetsButton.transform.SetParent(InventoryGui.instance.m_player.transform);
            _itemSetsButton.name = "ItemSetsButton";
            Vector3 newLocalPosition = GetSavedButtonPosition();
            _itemSetsButton.transform.localPosition = newLocalPosition;
            _itemSetsButton.onClick = new Button.ButtonClickedEvent();
            _itemSetsButton.onClick.AddListener(PurchasableLoadoutGui.ToggleUI);
            _itemSetsButton.onClick.AddListener(PersonalLoadoutGui.ToggleUI);
            _textComponent = _itemSetsButton.GetComponentInChildren<TMP_Text>();
            _textComponent.text = Localization.instance.Localize("$azu_rl_loadouts");
            UIDragger? dragger = _itemSetsButton.gameObject.AddComponent<UIDragger>();
            dragger.OnUIDropped += (source, position) => { RapidLoadoutsPlugin.InventoryItemSetButtonPosition.Value = position; };
        }

        private Vector3 GetSavedButtonPosition()
        {
            Vector3 newLocalPosition = RapidLoadoutsPlugin.InventoryItemSetButtonPosition.Value;
            return newLocalPosition;
        }
    }
}