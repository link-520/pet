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

            eventLibraryService = EventLibraryService.GetShared();
            playerDataService = PlayerDataService.GetShared(eventLibraryService);
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
            if (view == null || playerDataService == null)
            {
                return;
            }

            view.Refresh(playerDataService.GetPlayerData(), playerDataService.GetPersonalEvents());
        }

        private void HandleEventSelected(string eventId)
        {
            playerDataService.SelectEvent(eventId);
            RefreshPanel();
        }

        private void HandleConfirmClicked()
        {
            if (view == null || !view.CanSubmitSelectedRecordEvent)
            {
                return;
            }

            playerDataService.RecordEventOnce(playerDataService.GetPlayerData().SelectedEventId);
            RefreshPanel();
        }
    }
}
