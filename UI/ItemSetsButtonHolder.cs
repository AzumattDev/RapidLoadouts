﻿using UnityEngine;
using UnityEngine.UI;

namespace RapidLoadouts.UI
{
    public class ItemSetsButtonHolder : MonoBehaviour
    {
        private Button _itemSetsButton = null!;
        private Text _textComponent = null!;

        public delegate void RecycleAllHandler();

        public event RecycleAllHandler OnRecycleAllTriggered = null!;

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

            _itemSetsButton.gameObject.SetActive(true);
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
            _itemSetsButton = Instantiate(InventoryGui.instance.m_takeAllButton, InventoryGui.instance.m_player.transform);
            _itemSetsButton.transform.SetParent(InventoryGui.instance.m_player.transform);
            _itemSetsButton.name = "ItemSetsButton";
            var newLocalPosition = GetSavedButtonPosition();
            _itemSetsButton.transform.localPosition = newLocalPosition;
            _itemSetsButton.onClick = new Button.ButtonClickedEvent();
            _itemSetsButton.onClick.AddListener(ItemSetGui.ToggleUI);
            _textComponent = _itemSetsButton.GetComponentInChildren<Text>();
            _textComponent.text = Localization.instance.Localize("$azu_rl_itemSets");
            var dragger = _itemSetsButton.gameObject.AddComponent<UIDragger>();
            dragger.OnUIDropped += (source, position) => { RapidLoadoutsPlugin.InventoryItemSetButtonPosition.Value = position; };
        }

        private Vector3 GetSavedButtonPosition()
        {
            var newLocalPosition = RapidLoadoutsPlugin.InventoryItemSetButtonPosition.Value;
            return newLocalPosition;
        }
    }
}