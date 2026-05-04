using LifeRPG.Core;
using UnityEngine;

namespace LifeRPG.UI.DesktopPet
{
    /// <summary>
    /// 桌宠小窗控制器。
    /// 负责绑定 View 的按钮事件，并转交给 UIManager。
    /// </summary>
    public class DesktopPetController : MonoBehaviour
    {
        [SerializeField] private DesktopPetView view;
        [SerializeField] private LifeRPG.Core.UIManager uiManager;

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<DesktopPetView>();
            }
        }

        private void OnEnable()
        {
            if (view == null)
            {
                return;
            }

            view.OnShowPanelClicked += HandleShowPanelClicked;
            view.OnStartContinuousEventClicked += HandleStartContinuousEventClicked;
            view.OnRecordEventClicked += HandleRecordEventClicked;
        }

        private void OnDisable()
        {
            if (view == null)
            {
                return;
            }

            view.OnShowPanelClicked -= HandleShowPanelClicked;
            view.OnStartContinuousEventClicked -= HandleStartContinuousEventClicked;
            view.OnRecordEventClicked -= HandleRecordEventClicked;
        }

        private void HandleShowPanelClicked()
        {
            if (uiManager == null)
            {
                Debug.LogWarning("DesktopPetController 缺少 UIManager 引用，无法打开主面板。");
                return;
            }

            uiManager.ShowMainPanel();
        }

        private void HandleStartContinuousEventClicked()
        {
            if (uiManager == null)
            {
                Debug.LogWarning("DesktopPetController 缺少 UIManager 引用，无法处理持续性事件入口。");
                return;
            }

            uiManager.ShowContinuousEventEntry();
        }

        private void HandleRecordEventClicked()
        {
            if (uiManager == null)
            {
                Debug.LogWarning("DesktopPetController 缺少 UIManager 引用，无法处理记录性事件入口。");
                return;
            }

            uiManager.ShowRecordEventEntry();
        }
    }
}
