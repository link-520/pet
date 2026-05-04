using System;
using UnityEngine;
using UnityEngine.UI;

namespace LifeRPG.UI.DesktopPet
{
    /// <summary>
    /// 桌宠小窗视图。
    /// 只负责持有 UI 引用、显示宠物占位图、暴露按钮点击事件。
    /// </summary>
    public class DesktopPetView : MonoBehaviour
    {
        [Header("宠物显示")]
        [SerializeField] private Image petImage;
        [SerializeField] private Sprite placeholderPetSprite;

        [Header("底部按钮")]
        [SerializeField] private Button showPanelButton;
        [SerializeField] private Button startContinuousEventButton;
        [SerializeField] private Button recordEventButton;

        public event Action OnShowPanelClicked;
        public event Action OnStartContinuousEventClicked;
        public event Action OnRecordEventClicked;

        private void Awake()
        {
            DisableRootRaycastBlocker();
            BindButtons();
            RefreshPetImage();
        }

        /// <summary>
        /// 外部也可以临时替换宠物图。MVP 阶段没有宠物数据系统，先保留简单入口。
        /// </summary>
        public void SetPetSprite(Sprite sprite)
        {
            placeholderPetSprite = sprite;
            RefreshPetImage();
        }

        private void BindButtons()
        {
            if (showPanelButton != null)
            {
                showPanelButton.onClick.AddListener(() => OnShowPanelClicked?.Invoke());
            }

            if (startContinuousEventButton != null)
            {
                startContinuousEventButton.onClick.AddListener(() => OnStartContinuousEventClicked?.Invoke());
            }

            if (recordEventButton != null)
            {
                recordEventButton.onClick.AddListener(() => OnRecordEventClicked?.Invoke());
            }
        }

        private void RefreshPetImage()
        {
            if (petImage == null || placeholderPetSprite == null)
            {
                return;
            }

            petImage.sprite = placeholderPetSprite;
        }

        private void DisableRootRaycastBlocker()
        {
            Graphic rootGraphic = GetComponent<Graphic>();
            if (rootGraphic != null && rootGraphic != petImage)
            {
                // DesktopPetWindow 可以是全屏 RectTransform，但根节点背景不能吃掉主面板点击。
                rootGraphic.raycastTarget = false;
            }
        }
    }
}
