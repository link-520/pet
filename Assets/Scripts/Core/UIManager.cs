using UnityEngine;

namespace LifeRPG.Core
{
    /// <summary>
    /// UI 显隐管理器。今天只保留窗口打开/关闭骨架，不写复杂 UI 逻辑。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject desktopPetWindow;
        [SerializeField] private GameObject mainPanelWindow;

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

        /// <summary>
        /// 持续性事件入口。MVP 阶段先打开主面板，之后再接具体事件流程。
        /// </summary>
        public void ShowContinuousEventEntry()
        {
            ShowMainPanel();
        }

        /// <summary>
        /// 记录性事件入口。MVP 阶段先打开主面板，之后再接具体事件流程。
        /// </summary>
        public void ShowRecordEventEntry()
        {
            ShowMainPanel();
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
