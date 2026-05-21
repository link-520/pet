using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics.Contracts;

public class ImageController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("��ק����")]
    public bool enableDrag = true;
    public KeyCode toggleDragKey = KeyCode.F1;

    [Header("�߽�����")]
    public bool constrainToScreen = true;
    public float screenMargin = 50f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 dragOffset;
    private bool isDragging = false;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true; // ȷ��ͼƬ���Խ����¼�
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleDragKey))
        {
            ToggleDragMode();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!enableDrag) return;

        isDragging = true;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
        dragOffset = localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enableDrag || !isDragging) return;

        Vector3 worldPosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out worldPosition))
        {
            rectTransform.position = worldPosition;
            if (constrainToScreen)
            {
                ConstrainToScreen();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enableDrag) return;
        isDragging = false;
    }

    private void ConstrainToScreen()
    {
        Vector3 pos = rectTransform.position;
        Vector2 size = rectTransform.sizeDelta;

        float minX = screenMargin + size.x / 2;
        float maxX = Screen.width - screenMargin - size.x / 2;
        float minY = screenMargin + size.y / 2;
        float maxY = Screen.height - screenMargin - size.y / 2;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rectTransform.position = pos;
    }

    public void ToggleDragMode()
    {
        enableDrag = !enableDrag;

        Debug.Log($"��קģʽ: {(enableDrag ? "����" : "�ر�")}");
    }
}