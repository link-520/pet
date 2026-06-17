using System;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.Warehouse
{
    public enum WarehouseTab
    {
        Clothes,
        Events,
        Dimensions
    }

    /// <summary>
    /// 仓库窗口只负责通用显隐和三个仓库页签切换。
    /// </summary>
    public class WarehouseWindowController : MonoBehaviour
    {
        private Button clothesTabButton;
        private Button eventTabButton;
        private Button dimensionTabButton;
        private Button closeButton;
        private WarehouseTab currentTab = WarehouseTab.Clothes;

        private readonly string[] clothesContentNames = { "ClothessTab", "ClothesTab", "ClothesContent", "EquipmentContent" };
        private readonly string[] eventContentNames = { "EventTab", "Eventtab", "EventsContent", "EventContent" };
        private readonly string[] dimensionContentNames = { "Dimension", "DimensionTab", "DimensionContent", "DimensionsContent" };

        private void Awake()
        {
            BindReferences();
            BindButtons();
            ShowTab(currentTab);
        }

        private void OnEnable()
        {
            BindReferences();
            ShowTab(currentTab);
        }

        public void Open(WarehouseTab tab)
        {
            currentTab = tab;
            BindReferences();
            ShowTab(tab);
        }

        public void OpenClothes()
        {
            Open(WarehouseTab.Clothes);
        }

        public void OpenEvents()
        {
            Open(WarehouseTab.Events);
        }

        public void OpenDimensions()
        {
            Open(WarehouseTab.Dimensions);
        }

        private void BindReferences()
        {
            clothesTabButton = FindButton("ClothesTabButton", clothesTabButton);
            eventTabButton = FindButton("EventTabButton", eventTabButton);
            dimensionTabButton = FindButton("DimensionTabButton", dimensionTabButton);
            closeButton = FindButton("close", closeButton);
        }

        private void BindButtons()
        {
            BindButton(clothesTabButton, OpenClothes);
            BindButton(eventTabButton, OpenEvents);
            BindButton(dimensionTabButton, OpenDimensions);

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HideWindow);
                closeButton.onClick.AddListener(HideWindow);
            }
        }

        private void BindButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }

        private Button FindButton(string objectName, Button cachedButton)
        {
            if (cachedButton != null)
            {
                return cachedButton;
            }

            Transform buttonRoot = FindChildByName(transform, objectName);
            return buttonRoot != null ? buttonRoot.GetComponent<Button>() : null;
        }

        private void ShowTab(WarehouseTab tab)
        {
            SetButtonSelected(clothesTabButton, tab == WarehouseTab.Clothes);
            SetButtonSelected(eventTabButton, tab == WarehouseTab.Events);
            SetButtonSelected(dimensionTabButton, tab == WarehouseTab.Dimensions);

            SetContentVisible(clothesContentNames, tab == WarehouseTab.Clothes);
            SetContentVisible(eventContentNames, tab == WarehouseTab.Events);
            SetContentVisible(dimensionContentNames, tab == WarehouseTab.Dimensions);
        }

        private void SetButtonSelected(Button button, bool selected)
        {
            if (button == null)
            {
                return;
            }

            Image image = button.targetGraphic as Image;
            if (image == null)
            {
                image = button.GetComponent<Image>();
            }

            if (image != null)
            {
                image.color = selected
                    ? new Color(1f, 0.84f, 0.54f, 1f)
                    : Color.white;
            }
        }

        private void SetContentVisible(string[] names, bool visible)
        {
            foreach (string name in names)
            {
                Transform content = FindChildByName(transform, name);
                if (content != null && content.gameObject != gameObject)
                {
                    content.gameObject.SetActive(visible);
                }
            }
        }

        private void HideWindow()
        {
            gameObject.SetActive(false);
        }

        private Transform FindChildByName(Transform root, string targetName)
        {
            if (root == null || string.IsNullOrEmpty(targetName))
            {
                return null;
            }

            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (string.Equals(child.name, targetName, StringComparison.Ordinal))
                {
                    return child;
                }
            }

            return null;
        }
    }
}
