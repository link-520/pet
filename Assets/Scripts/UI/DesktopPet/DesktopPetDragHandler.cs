using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LifeRPG.UI.DesktopPet
{
    /// <summary>
    /// 桌宠拖拽脚本。
    /// 建议挂在 PetImage 上，把 Target Rect 拖成 DesktopPetWindow。
    /// </summary>
    public class DesktopPetDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("拖拽目标")]
        [SerializeField] private RectTransform targetRect;
        [SerializeField] private Canvas canvas;

        [Header("拖拽设置")]
        [SerializeField] private bool enableDrag = true;
        [SerializeField] private bool constrainToCanvas = true;
        [SerializeField] private float edgePadding = 10f;

        private RectTransform canvasRect;
        private Vector2 dragOffset;
        private bool isDragging;

        public bool IsDragging => isDragging;

        private void Awake()
        {
            if (targetRect == null)
            {
                targetRect = GetComponent<RectTransform>();
            }

            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }

            if (canvas != null)
            {
                canvasRect = canvas.transform as RectTransform;
            }

            Graphic graphic = GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = true;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enableDrag || targetRect == null)
            {
                return;
            }

            isDragging = true;

            RectTransform parentRect = targetRect.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 pointerLocalPosition);

            dragOffset = targetRect.anchoredPosition - pointerLocalPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!enableDrag || !isDragging || targetRect == null)
            {
                return;
            }

            RectTransform parentRect = targetRect.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 pointerLocalPosition);

            targetRect.anchoredPosition = pointerLocalPosition + dragOffset;

            if (constrainToCanvas)
            {
                ClampToCanvas();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }

        private void ClampToCanvas()
        {
            if (canvasRect == null || targetRect == null)
            {
                return;
            }

            Vector2 canvasSize = canvasRect.rect.size;
            Vector2 targetSize = targetRect.rect.size;
            Vector2 position = targetRect.anchoredPosition;

            float minX = -canvasSize.x * 0.5f + targetSize.x * 0.5f + edgePadding;
            float maxX = canvasSize.x * 0.5f - targetSize.x * 0.5f - edgePadding;
            float minY = -canvasSize.y * 0.5f + targetSize.y * 0.5f + edgePadding;
            float maxY = canvasSize.y * 0.5f - targetSize.y * 0.5f - edgePadding;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            targetRect.anchoredPosition = position;
        }
    }
}
