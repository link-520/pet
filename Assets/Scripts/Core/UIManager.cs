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
            SetWindowVisible(warehouseWindow, true);
        }

        public void HideWarehouse()
        {
            SetWindowVisible(warehouseWindow, false);
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
