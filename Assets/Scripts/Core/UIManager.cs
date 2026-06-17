using LifeRPG.UI.Warehouse;
using UnityEngine;

namespace LifeRPG.Core
{
    /// <summary>
    /// UI 显隐管理器。负责核心窗口的打开和关闭。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject desktopPetWindow;
        [SerializeField] private GameObject mainPanelWindow;
        [SerializeField] private GameObject warehouseWindow;

        private void Start()
        {
            ShowDesktopPet();
            HideMainPanel();
            HideWarehouse();
        }

        public void ShowDesktopPet()
        {
            SetWindowVisible(desktopPetWindow, true);
        }

        public void HideDesktopPet()
        {
            SetWindowVisible(desktopPetWindow, false);
        }

        public void ShowMainPanel()
        {
            SetWindowVisible(mainPanelWindow, true);
        }

        public void HideMainPanel()
        {
            SetWindowVisible(mainPanelWindow, false);
        }

        public void ShowContinuousEventEntry()
        {
            ShowMainPanel();
        }

        public void ShowRecordEventEntry()
        {
            ShowMainPanel();
        }

        public void ShowWarehouse()
        {
            ShowWarehouseTab(WarehouseTab.Clothes);
        }

        public void ShowEquipmentWarehouse()
        {
            ShowWarehouseTab(WarehouseTab.Clothes);
        }

        public void ShowEventWarehouse()
        {
            ShowWarehouseTab(WarehouseTab.Events);
        }

        public void ShowDimensionWarehouse()
        {
            ShowWarehouseTab(WarehouseTab.Dimensions);
        }

        private void ShowWarehouseTab(WarehouseTab tab)
        {
            SetWindowVisible(warehouseWindow, true);

            if (warehouseWindow == null)
            {
                Debug.LogWarning("UIManager 找不到仓库窗口，无法打开仓库。");
                return;
            }

            WarehouseWindowController controller = warehouseWindow.GetComponent<WarehouseWindowController>();

            controller.Open(tab);
        }

        public void HideWarehouse()
        {
            SetWindowVisible(warehouseWindow, false);
        }



        private GameObject FindLoadedObjectByName(string objectName)
        {
            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform candidate in transforms)
            {
                if (candidate == null
                    || candidate.name != objectName
                    || !candidate.gameObject.scene.IsValid())
                {
                    continue;
                }

                return candidate.gameObject;
            }

            return null;
        }

        private void SetWindowVisible(GameObject window, bool visible)
        {
            if (window == null)
            {
                return;
            }

            window.SetActive(visible);
        }
    }
}
