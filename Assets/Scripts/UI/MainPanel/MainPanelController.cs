using LifeRPG.Services;
using UnityEngine;

namespace LifeRPG.UI.MainPanel
{
    /// <summary>
    /// 完整面板控制器。负责从服务层取数据，并驱动 MainPanelView 刷新。
    /// </summary>
    public class MainPanelController : MonoBehaviour
    {
        [SerializeField] private MainPanelView view;

        private EventLibraryService eventLibraryService;
        private PlayerDataService playerDataService;

        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<MainPanelView>();
            }

            // MVP 阶段先在控制器内部创建服务，后续再交给 GameBootstrap 统一注入。
            eventLibraryService = new EventLibraryService();
            playerDataService = new PlayerDataService(eventLibraryService);
        }

        private void OnEnable()
        {
            if (view != null)
            {
                view.OnEventSelected += HandleEventSelected;
                view.OnConfirmClicked += HandleConfirmClicked;
            }

            RefreshPanel();
        }

        private void OnDisable()
        {
            if (view == null)
            {
                return;
            }

            view.OnEventSelected -= HandleEventSelected;
            view.OnConfirmClicked -= HandleConfirmClicked;
        }

        public void RefreshPanel()
        {
            if (view == null || playerDataService == null || eventLibraryService == null)
            {
                return;
            }

            view.Refresh(playerDataService.GetPlayerData(), eventLibraryService.GetAllEvents());
        }

        private void HandleEventSelected(string eventId)
        {
            playerDataService.SelectEvent(eventId);
            RefreshPanel();
        }

        private void HandleConfirmClicked()
        {
            playerDataService.ConfirmSelectedEvent();
            RefreshPanel();
        }
    }
}
