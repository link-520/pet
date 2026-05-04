using UnityEngine;

namespace LifeRPG.UI.DesktopPet
{
    /// <summary>
    /// 桌宠简单自动走路。
    /// MVP 阶段只做 UI 位置缓慢移动，不做动画状态机。
    /// </summary>
    public class DesktopPetWalker : MonoBehaviour
    {
        [Header("移动目标")]
        [SerializeField] private RectTransform targetRect;
        [SerializeField] private DesktopPetDragHandler dragHandler;

        [Header("走路设置")]
        [SerializeField] private bool enableWalk = false;
        [SerializeField] private float moveSpeed = 35f;
        [SerializeField] private float waitMinSeconds = 1.5f;
        [SerializeField] private float waitMaxSeconds = 4f;
        [SerializeField] private Vector2 walkRange = new Vector2(240f, 120f);

        private Vector2 homePosition;
        private Vector2 targetPosition;
        private float waitTimer;

        private void Awake()
        {
            if (targetRect == null)
            {
                targetRect = GetComponent<RectTransform>();
            }

            if (dragHandler == null)
            {
                dragHandler = GetComponentInChildren<DesktopPetDragHandler>();
            }
        }

        private void OnEnable()
        {
            if (targetRect == null)
            {
                return;
            }

            homePosition = targetRect.anchoredPosition;
            PickNextTarget();
        }

        private void Update()
        {
            if (!enableWalk || targetRect == null)
            {
                return;
            }

            if (dragHandler != null && dragHandler.IsDragging)
            {
                homePosition = targetRect.anchoredPosition;
                return;
            }

            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            targetRect.anchoredPosition = Vector2.MoveTowards(
                targetRect.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime);

            if (Vector2.Distance(targetRect.anchoredPosition, targetPosition) < 0.5f)
            {
                PickNextTarget();
            }
        }

        private void PickNextTarget()
        {
            waitTimer = Random.Range(waitMinSeconds, waitMaxSeconds);
            targetPosition = homePosition + new Vector2(
                Random.Range(-walkRange.x, walkRange.x),
                Random.Range(-walkRange.y, walkRange.y));
        }
    }
}
