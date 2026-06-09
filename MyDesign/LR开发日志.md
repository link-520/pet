---
created: 2026-04-24T22:04
updated: 2026-06-07T15:58
---
LR 开发文档 ： [LifeRpg](onenote:https://d.docs.live.net/a8d159ca619f8edb/文档/游戏开发日志/LifeRpg.one#section-id={7B0069AD-548B-2844-A913-F442D23A85FF}&end)  ([Web 视图](https://onedrive.live.com/view.aspx?resid=A8D159CA619F8EDB%211303&id=documents&wd=target%28LifeRpg.one%7C7B0069AD-548B-2844-A913-F442D23A85FF%2F%29&wdsectionfileid=A8D159CA619F8EDB!s7ec37c7afdd84844b15d92e0401155d2&end))
# 每日推进
## 5.3 做 MVP
- 桌宠在桌面上，下方3个UI
	![[Pasted image 20260503155449.png]]
- 面板展示打开后有完整面板UI，并可以显示宠物
	![[Pasted image 20260503155602.png]]
## 5.4 完善mvP
- [x] 界面和功能 ，git仓库重建
- [x] 分配任务

## 5.5
- [ ] 祝林做完UI显示美化
- [ ] 我负责主界面UI功能
- [x] qq空间里找个美术 - 有原型再找 
- [x] 生成原型图片
## 5.21
- [x] 恢复拖动功能
- [x] 调整桌面桌宠，改善按钮UI
## 5.30
- [x] 制作MainPanel
## 5.31
- [x] 继续制作MainPanel
## 6.4
- [x] 加了个x框
# 6.7

# 架构 ：
## 代码架构 ：
Scripts/  
├── Core/  
│ ├── GameBootstrap.cs  
│ ├── GameManager.cs  
│ ├── UIManager.cs  
│ └── EventBus.cs // 可选，来不及可以先不要  
│  
├── Data/  
│ ├── DimensionType.cs  
│ ├── EventType.cs  
│ ├── DimensionSet.cs  
│ ├── EventDefinition.cs  
│ ├── PlayerEventData.cs  
│ ├── PlayerData.cs  
│ └── PanelViewData.cs  
│  
├── Systems/  
│ ├── PlayerDataService.cs  
│ ├── EventLibraryService.cs  
│ ├── ScoreCalculator.cs  
│ └── SaveService.cs  
│  
├── UI/  
│ ├── DesktopPet/  
│ │ ├── DesktopPetView.cs  
│ │ └── DesktopPetButtonBar.cs  
│ │  
│ ├── MainPanel/  
│ │ ├── MainPanelView.cs  
│ │ ├── EquipmentSlotView.cs  
│ │ ├── DimensionBarView.cs  
│ │ ├── EventListItemView.cs  
│ │ └── SelectedEventStatusView.cs  
│ │  
│ └── Common/  
│ ├── UIWindow.cs  
│ └── UIButtonBinder.cs  
│  
└── Controllers/  
├── DesktopPetController.cs  
└── MainPanelController.cs
GameBootstrap.cs  
项目启动入口。负责创建或引用核心对象，初始化假数据、服务、控制器，并打开桌宠小窗。

GameManager.cs  
保存当前游戏运行状态。MVP 阶段主要持有 PlayerData，后续可以扩展成全局游戏状态管理。

UIManager.cs  
管理两个界面的显示隐藏：桌宠小窗、完整面板 UI。今天只需要支持打开/关闭主面板。

DimensionType.cs  
六维枚举：身体、知识、事业、关系、财富、快乐。

LifeEventType.cs  
事件类型枚举：持续性事件、记录性事件。

DimensionValue.cs  
单个维度的数据，例如维度类型、当前分数。

LifeEventData.cs  
事件数据。包含事件名、事件类型、所属六维、次数、时间、分数。

PlayerData.cs  
玩家当前数据。包含六维数据、个人事件库、当前选中的事件或当前进行中的事件。

MainPanelViewData.cs  
专门给完整面板 UI 使用的展示数据。避免 UI 直接读复杂的 PlayerData。

MockDataService.cs  
生成今天演示用的假数据。比如六维初始值、跑步、阅读、约会、加班等事件。

PlayerDataService.cs  
提供读取和修改玩家数据的方法。比如获取六维、获取事件库、选择事件、确认事件。

DesktopPetController.cs  
连接桌宠 UI 和系统逻辑。处理底部三个按钮点击：打开面板、开始持续性事件、记录记录性事件。

MainPanelController.cs  
连接完整面板 UI 和玩家数据。负责刷新面板、响应事件选择、处理确认按钮。

DesktopPetView.cs  
桌宠小窗的整体视图。负责显示宠物图片/占位图、控制按钮栏显示。

DesktopPetButtonBarView.cs  
桌宠底部三个按钮的视图。只负责按钮点击事件暴露，不直接改数据。

MainPanelView.cs  
完整面板总视图。管理左侧装备区、六维区、中间宠物、右侧事件区、确认按钮。

EquipmentAreaView.cs  
装备区视图。今天可以只显示几个空装备槽位。

DimensionListView.cs  
六维列表容器。负责批量生成或刷新六维条目。

DimensionItemView.cs  
单个六维条目。显示维度名和分数。

EventListView.cs  
事件列表容器。负责显示玩家个人事件库。

EventItemView.cs  
单个事件条目。显示事件名、类型、次数、时间、分数。

CurrentEventStatusView.cs  
当前选中事件/当前完成情况区域。显示玩家选了什么事件，以及确认前的状态。

UIWindow.cs  
简单 UI 窗口基类。提供 Show()、Hide() 这类通用方法。

## 场景架构
LifeRPGScene
├── GameRoot
│ ├── GameBootstrap
│ ├── GameManager
│ └── UIManager
│
├── Canvas
│ ├── DesktopPetWindow
│ │ ├── PetImage
│ │ └── BottomButtonBar
│ │ ├── OpenPanelButton
│ │ ├── StartContinuousEventButton
│ │ └── RecordInstantEventButton
│ │
│ └── MainPanelWindow
│ ├── LeftArea
│ │ ├── EquipmentArea
│ │ └── DimensionArea
│ │
│ ├── CenterArea
│ │ └── PetDisplay
│ │
│ └── RightArea
│ ├── EventListArea
│ ├── CurrentEventStatusArea
│ └── ConfirmButton
│
└── EventSystem
