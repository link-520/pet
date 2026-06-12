using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;
// 这个类负责在桌面环境下设置窗口透明和点击穿透，支持 Windows 和 macOS 平台。
public class DesktopWindowService : MonoBehaviour
{
    [SerializeField] private bool transparentCameraBackground = true; // 是否将摄像机背景设置为透明，这样才能看到桌面背景
    [SerializeField] private bool transparentAreaClickThrough = true;
    // 因为是静态语言，所以需要提前声明平台相关的函数和变量，使用条件编译来区分不同平台的实现
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
    [DllImport("DesktopPetMac")]
    private static extern void SetMacWindowTransparent();

    [DllImport("DesktopPetMac")]
    private static extern void SetMacClickThrough([MarshalAs(UnmanagedType.I1)] bool enabled);

    [DllImport("DesktopPetMac")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool GetMacMousePositionInWindow(out float x, out float y);

    [DllImport("DesktopPetMac")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool IsMacMouseButtonPressed();

    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
    private PointerEventData pointerEventData;
    private bool clickThroughEnabled;
    private float nextMacWindowRefreshTime;
#endif

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    // 定义一个结构来存储窗口边框的边距大小
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    // 导入 user32.dll 以获取活动窗口句柄 (HWND)
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // 导入 Dwmapi.dll 以将窗口边框扩展到客户区域
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // 导入 user32.dll 以修改窗口属性
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // 导入 user32.dll 以设置窗口位置
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    // 导入 user32.dll 以设置分层窗口属性 (透明度)
    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    // 代码中使用的常量和变量
    const int GWL_EXSTYLE = -20;  // 修改窗口样式的索引
    const uint WS_EX_LAYERED = 0x00080000;  // 分层窗口的扩展样式
    const uint WS_EX_TRANSPARENT = 0x00000020;  // 透明窗口的扩展样式
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);  // 窗口插入位置（始终置顶）
    const uint LWA_COLORKEY = 0x00000001;  // 设置颜色键的标志（用于透明度）
    private IntPtr hWnd;  // 活动窗口的句柄
#endif

    private void Start()
    {
        Application.runInBackground = true; // 允许应用在后台运行，这样宠物在切换到其他窗口时也能继续活动

        if (transparentCameraBackground)
        {
            MakeCameraBackgroundsTransparent();
        }

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        StartCoroutine(ApplyMacDesktopWindow());

        // 先不要开启点击穿透，不然你点不到宠物和按钮
        SetMacClickThrough(false);
#endif


#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        // 获取活动窗口的句柄（类似编号，用它来识别是哪个窗口。用对象太臃肿）
        hWnd = GetActiveWindow();

        // 创建一个边距结构来定义边框大小
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };

        // 将窗口边框扩展到客户区域（玻璃效果）相当于去掉windows边框
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // 将窗口样式设置为分层和透明，
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        // 设置窗口颜色键（用于透明度）
        SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        // 将窗口位置设置为始终置顶
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif

    }

    private void Update()
    {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        RefreshMacDesktopWindow();
        UpdateMacClickThrough();
#endif
    }

    private static void MakeCameraBackgroundsTransparent()
    {
        foreach (Camera camera in Camera.allCameras)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;

            Color backgroundColor = camera.backgroundColor;
            backgroundColor.a = 0f;
            camera.backgroundColor = backgroundColor;
        }
    }

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
    private IEnumerator ApplyMacDesktopWindow()
    {
        // Unity 的 NSWindow / Metal view 有时会在 Start 后才最终挂好，重复应用能避免时序问题。
        for (int i = 0; i < 180; i++)
        {
            SetMacWindowTransparent();
            yield return null;
        }
    }

    private void RefreshMacDesktopWindow()
    {
        if (Time.unscaledTime < nextMacWindowRefreshTime)
        {
            return;
        }

        SetMacWindowTransparent();
        nextMacWindowRefreshTime = Time.unscaledTime < 5f
            ? Time.unscaledTime
            : Time.unscaledTime + 0.5f;
    }

    private void UpdateMacClickThrough()
    {
        if (!transparentAreaClickThrough || EventSystem.current == null)
        {
            SetMacClickThroughIfChanged(false);
            return;
        }

        if (IsMacMouseButtonPressed())
        {
            SetMacClickThroughIfChanged(false);
            return;
        }

        if (!GetMacMousePositionInWindow(out float mouseX, out float mouseY))
        {
            SetMacClickThroughIfChanged(true);
            return;
        }

        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        pointerEventData.Reset();
        pointerEventData.position = new Vector2(mouseX, mouseY);

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        SetMacClickThroughIfChanged(raycastResults.Count == 0);
    }

    private void SetMacClickThroughIfChanged(bool enabled)
    {
        if (clickThroughEnabled == enabled)
        {
            return;
        }

        clickThroughEnabled = enabled;
        SetMacClickThrough(enabled);
    }
#endif
}
