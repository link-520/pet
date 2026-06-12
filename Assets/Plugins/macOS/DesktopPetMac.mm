#import <Cocoa/Cocoa.h>
#import <QuartzCore/QuartzCore.h>

static NSArray<NSWindow *> *DesktopPetGetUnityWindows()
{
    NSApplication *application = [NSApplication sharedApplication];
    NSMutableArray<NSWindow *> *windows = [NSMutableArray array];

    for (NSWindow *window in application.windows) {
        if (window && window.isVisible) {
            [windows addObject:window];
        }
    }

    NSWindow *mainWindow = application.mainWindow ?: application.keyWindow;
    if (mainWindow && ![windows containsObject:mainWindow]) {
        [windows addObject:mainWindow];
    }

    if (windows.count == 0 && application.windows.count > 0) {
        [windows addObject:application.windows.firstObject];
    }

    return windows;
}

static void DesktopPetMakeLayerTransparent(CALayer *layer)
{
    if (!layer) return;

    layer.opaque = NO;
    layer.backgroundColor = [[NSColor clearColor] CGColor];

    if ([layer isKindOfClass:[CAMetalLayer class]]) {
        CAMetalLayer *metalLayer = (CAMetalLayer *)layer;
        metalLayer.opaque = NO;
        metalLayer.backgroundColor = [[NSColor clearColor] CGColor];
    }

    for (CALayer *sublayer in layer.sublayers) {
        DesktopPetMakeLayerTransparent(sublayer);
    }
}

static void DesktopPetMakeViewTransparent(NSView *view)
{
    if (!view) return;

    view.wantsLayer = YES;
    DesktopPetMakeLayerTransparent(view.layer);

    for (NSView *subview in view.subviews) {
        DesktopPetMakeViewTransparent(subview);
    }
}

static void DesktopPetHideStandardButtons(NSWindow *window)
{
    [[window standardWindowButton:NSWindowCloseButton] setHidden:YES];
    [[window standardWindowButton:NSWindowMiniaturizeButton] setHidden:YES];
    [[window standardWindowButton:NSWindowZoomButton] setHidden:YES];
}

static void DesktopPetFillVisibleScreen(NSWindow *window)
{
    NSScreen *screen = window.screen ?: [NSScreen mainScreen];
    if (!screen) return;

    [window setFrame:screen.visibleFrame display:YES animate:NO];
}

static bool DesktopPetGetMousePositionInWindow(float *x, float *y)
{
    NSWindow *window = [NSApplication sharedApplication].mainWindow ?:
                       [NSApplication sharedApplication].keyWindow ?:
                       DesktopPetGetUnityWindows().firstObject;
    if (!window) return false;

    NSPoint mouseLocation = [NSEvent mouseLocation];
    NSPoint pointInWindow = [window convertPointFromScreen:mouseLocation];
    NSView *contentView = window.contentView;

    if (contentView) {
        NSPoint pointInContent = [contentView convertPoint:pointInWindow fromView:nil];
        NSPoint pointInPixels = [contentView convertPointToBacking:pointInContent];

        if (x) *x = (float)pointInPixels.x;
        if (y) *y = (float)pointInPixels.y;
        return NSPointInRect(pointInContent, contentView.bounds);
    }

    if (x) *x = (float)pointInWindow.x;
    if (y) *y = (float)pointInWindow.y;
    return NSPointInRect(pointInWindow, NSMakeRect(0, 0, window.frame.size.width, window.frame.size.height));
}

extern "C" {

void SetMacWindowTransparent()
{
    dispatch_async(dispatch_get_main_queue(), ^{
        NSApplication *application = [NSApplication sharedApplication];
        [application setActivationPolicy:NSApplicationActivationPolicyRegular];

        for (NSWindow *window in DesktopPetGetUnityWindows()) {
            // 去掉标题栏和边框
            [window setStyleMask:NSWindowStyleMaskBorderless];
            [window setTitleVisibility:NSWindowTitleHidden];
            [window setTitlebarAppearsTransparent:YES];
            DesktopPetHideStandardButtons(window);

            // 窗口透明
            [window setOpaque:NO];
            [window setBackgroundColor:[NSColor clearColor]];
            [window setHasShadow:NO];
            [window setAlphaValue:1.0];
            DesktopPetFillVisibleScreen(window);

            // 置顶
            [window setLevel:NSFloatingWindowLevel];

            // 可以出现在所有桌面空间
            [window setCollectionBehavior:NSWindowCollectionBehaviorCanJoinAllSpaces |
                                          NSWindowCollectionBehaviorFullScreenAuxiliary |
                                          NSWindowCollectionBehaviorStationary |
                                          NSWindowCollectionBehaviorIgnoresCycle];

            DesktopPetMakeViewTransparent(window.contentView.superview);
            DesktopPetMakeViewTransparent(window.contentView);
        }
    });
}

void SetMacClickThrough(bool enabled)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        for (NSWindow *window in DesktopPetGetUnityWindows()) {
            [window setIgnoresMouseEvents:enabled];
        }
    });
}

bool GetMacMousePositionInWindow(float *x, float *y)
{
    __block bool hasPosition = false;
    __block float positionX = 0.0f;
    __block float positionY = 0.0f;

    void (^readPosition)(void) = ^{
        hasPosition = DesktopPetGetMousePositionInWindow(&positionX, &positionY);
    };

    if ([NSThread isMainThread]) {
        readPosition();
    } else {
        dispatch_sync(dispatch_get_main_queue(), readPosition);
    }

    if (x) *x = positionX;
    if (y) *y = positionY;
    return hasPosition;
}

bool IsMacMouseButtonPressed()
{
    __block bool pressed = false;

    void (^readPressedState)(void) = ^{
        pressed = [NSEvent pressedMouseButtons] != 0;
    };

    if ([NSThread isMainThread]) {
        readPressedState();
    } else {
        dispatch_sync(dispatch_get_main_queue(), readPressedState);
    }

    return pressed;
}

}
