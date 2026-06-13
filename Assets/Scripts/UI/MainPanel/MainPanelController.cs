using LifeRPG.Data;
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
        [SerializeField] private LifeRPG.Core.UIManager uiManager;

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

            if (uiManager == null)
            {
                uiManager = FindAnyObjectByType<LifeRPG.Core.UIManager>();
            }
        }

        private void OnEnable()
        {
            if (view != null)
            {
                view.OnEventSelected += HandleEventSelected;
                view.OnConfirmClicked += HandleConfirmClicked;
                view.OnCloseClicked += HandleCloseClicked;
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
            view.OnCloseClicked -= HandleCloseClicked;
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
            if (view == null || !view.CanSubmitSelectedEvent)
            {
                return;
            }

            EventDefinition selectedEvent = playerDataService.GetSelectedEvent();
            if (selectedEvent == null)
            {
                return;
            }

            if (selectedEvent.Type == LifeRPG.Data.EventType.Record)
            {
                playerDataService.RecordEventOnce(selectedEvent.Id);
            }
            else
            {
                playerDataService.RecordContinuousEvent(selectedEvent.Id, selectedEvent.RequiredMinutes);
            }

            RefreshPanel();
        }

        public void StartSelectedContinuousEvent()
        {
            if (playerDataService == null)
            {
                return;
            }

            playerDataService.StartContinuousEvent(playerDataService.GetPlayerData().SelectedEventId);
            RefreshPanel();
        }

        public void FinishActiveContinuousEvent()
        {
            if (playerDataService == null)
            {
                return;
            }

            playerDataService.FinishActiveContinuousEvent();
            RefreshPanel();
        }

        public void ForceSettleToday()
        {
            if (playerDataService == null)
            {
                return;
            }

            playerDataService.ForceSettleToday();
            RefreshPanel();
        }

        public void CloseMainPanel()
        {
            if (uiManager == null)
            {
                Debug.LogWarning("MainPanelController 缺少 UIManager 引用，无法关闭主面板。");
                return;
            }

            uiManager.HideMainPanel();
        }

        private void HandleCloseClicked()
        {
            CloseMainPanel();
        }
    }
}
