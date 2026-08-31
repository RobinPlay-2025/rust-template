# CUI — Справочник интерфейсов Oxide/Rust (2026)

> Источник истины: `Oxide.Rust/src/RustCui.cs` (ветка `develop`, OxideMod GitHub)
> Обновлено: 2026-08

---

## Цвета

| Имя          | RGBA                   | HEX      |
|:------------:|:----------------------:|:--------:|
| Red          | `0.8 0.28 0.2 1`       | `cd4632` |
| Green        | `0.55 0.78 0.24 1`     | `8cc83c` |
| Blue         | `0.204 0.596 0.859 1`  | `3498db` |
| Прозрачный   | `0 0 0 0`              | —        |
| Чёрный 70%   | `0 0 0 0.7`            | —        |
| Белый        | `1 1 1 1`              | `ffffff` |

**Формат:** `"R G B A"` — каждое значение от 0.0 до 1.0 (float).

---

## Шрифты

| Путь к шрифту |
|:---|
| `assets/content/ui/fonts/droidsansmono.ttf` |
| `assets/content/ui/fonts/permanentmarker.ttf` |
| `assets/content/ui/fonts/robotocondensed-bold.ttf` |
| `assets/content/ui/fonts/robotocondensed-regular.ttf` |

---

## Слои (parent)

| Строка     | Описание |
|:-----------|:---------|
| `Overlay`  | Поверх всего — рекомендуется для кастомных меню |
| `Hud`      | Слой HUD (под инвентарём) |
| `Hud.Menu` | Слой игрового меню |
| `Under`    | Под HUD-ом |

---

## CuiElement — базовая единица UI

```csharp
new CuiElement
{
    Name       = "MyPanel",          // уникальное имя (CuiHelper.GetGuid())
    Parent     = "Overlay",          // родительский слой или имя другого элемента
    DestroyUi  = "OtherPanelName",   // [NEW] уничтожить другой элемент при создании этого
    FadeOut    = 0.3f,               // плавное исчезновение при уничтожении
    Update     = false,              // [NEW] true = обновить существующий (без мерцания)
    ActiveSelf = null,               // [NEW] true/false = показать/скрыть без удаления
    Components = { /* ... */ }
}
```

> **`Update = true`** — ключевой паттерн 2024+. Позволяет менять текст/цвет без `DestroyUi + AddUi`, устраняя мерцание.

---

## Компоненты

### CuiRectTransformComponent

Позиционирование. Обязателен в каждом элементе.

```csharp
new CuiRectTransformComponent
{
    AnchorMin         = "0 0",     // левый нижний угол (0..1)
    AnchorMax         = "1 1",     // правый верхний угол (0..1)
    OffsetMin         = "10 10",   // пиксельный отступ от якоря (мин)
    OffsetMax         = "-10 -10", // пиксельный отступ от якоря (макс)
    Rotation          = 0f,        // [NEW] поворот элемента в градусах
    Pivot             = "0.5 0.5", // [NEW] точка вращения (0..1)
    SetParent         = null,      // [NEW] переместить в другой родитель
    SetTransformIndex = 0,         // [NEW] порядок отрисовки среди сиблингов
}
```

**Паттерн "центр экрана, фиксированный размер" (пример: 400x300):**
```csharp
AnchorMin = "0.5 0.5", AnchorMax = "0.5 0.5",
OffsetMin = "-200 -150", OffsetMax = "200 150"
```

**Паттерн "растянуть на весь родитель":**
```csharp
AnchorMin = "0 0", AnchorMax = "1 1",
OffsetMin = "0 0", OffsetMax = "0 0"
```

---

### CuiImageComponent

```csharp
new CuiImageComponent
{
    Color                  = "1 1 1 1",
    Sprite                 = "assets/content/ui/ui.background.tile.psd",
    Material               = "assets/icons/iconmaterial.mat",
    ImageType              = Image.Type.Sliced,
    Slice                  = "...",  // [NEW] 9-slice настройки
    FillCenter             = true,   // [NEW] заполнение центра слайсом
    Png                    = "...",  // Steam-изображение (uint -> string)
    ItemId                 = 0,      // [NEW] иконка предмета по ItemDefinition.itemid
    SkinId                 = 0UL,    // [NEW] скин предмета
    PixelsPerUnitMultiplier = 1f,    // [NEW] масштаб пикселей спрайта
    FadeIn                 = 0f,
}
```

> `ItemId` + `SkinId` — отображает иконку любого предмета без FileStorage/ImageLibrary.

---

### CuiRawImageComponent

```csharp
new CuiRawImageComponent
{
    Color   = "1 1 1 1",
    Sprite  = "...",
    Png     = "...",              // FileStorage PNG (ImageLibrary)
    Url     = "https://...",     // [NEW] загрузить по URL
    SteamId = "76561198XXXXXX",  // [NEW] аватарка по SteamID
    FadeIn  = 0f,
}
```

---

### CuiTextComponent

```csharp
new CuiTextComponent
{
    Text             = "Привет",
    FontSize         = 14,
    Font             = "assets/content/ui/fonts/robotocondensed-bold.ttf",
    Align            = TextAnchor.MiddleCenter,
    Color            = "1 1 1 1",
    VerticalOverflow = VerticalWrapMode.Overflow, // [NEW]
    FadeIn           = 0f,
}
```

---

### CuiButtonComponent

```csharp
new CuiButtonComponent
{
    Color            = "0.4 0.4 0.4 1",
    Command          = "chat.say /help",
    Close            = "MyPanelName",    // закрыть элемент по имени
    Sprite           = "assets/content/ui/ui.background.rounded.png",
    // [NEW] цветовые состояния кнопки:
    NormalColor      = "0.4 0.4 0.4 1",
    HighlightedColor = "0.55 0.55 0.55 1",
    PressedColor     = "0.25 0.25 0.25 1",
    SelectedColor    = null,
    DisabledColor    = "0.2 0.2 0.2 0.5",
    ColorMultiplier  = 1f,
    FadeDuration     = 0.1f,  // [NEW] время анимации смены цвета
    Interactable     = true,  // [NEW] false = кнопка задизейблена
    FadeIn           = 0f,
}
```

---

### CuiInputFieldComponent

```csharp
new CuiInputFieldComponent
{
    Text          = "",
    FontSize      = 14,
    Font          = "assets/content/ui/fonts/robotocondensed-regular.ttf",
    Align         = TextAnchor.MiddleLeft,
    Color         = "1 1 1 1",
    CharsLimit    = 128,
    Command       = "plugincmd.input",
    LineType      = InputField.LineType.SingleLine,
    ReadOnly      = false,        // [NEW]
    IsPassword    = false,
    NeedsKeyboard = true,         // [NEW] обязательно для ввода текста!
    HudMenuInput  = false,        // [NEW] работа в HUD-меню
    Autofocus     = false,        // [NEW]
    PlaceholderId = null,         // [NEW] ссылка на элемент-плейсхолдер
    Interactable  = true,         // [NEW]
    FadeIn        = 0f,
}
```

---

### CuiOutlineComponent

```csharp
new CuiOutlineComponent
{
    Color           = "0 0 0 1",
    Distance        = "1 -1",  // "X Y" в пикселях
    UseGraphicAlpha = false,
}
```

---

### CuiCountdownComponent [NEW]

Встроенный таймер — не нужен `timer.Every`.

```csharp
new CuiCountdownComponent
{
    StartTime     = 60f,
    EndTime       = 0f,
    Step          = -1f,
    Interval      = 1f,
    TimerFormat   = TimerFormat.MinutesSeconds,
    NumberFormat  = "",               // кастомный формат (при TimerFormat.Custom)
    DestroyIfDone = true,             // уничтожить элемент по завершении
    Command       = "plugincmd.done", // вызвать команду по завершении
    FadeIn        = 0f,
}
```

**Форматы TimerFormat:**
`None`, `SecondsHundreth`, `MinutesSeconds`, `MinutesSecondsHundreth`,
`HoursMinutes`, `HoursMinutesSeconds`, `HoursMinutesSecondsMilliseconds`,
`HoursMinutesSecondsTenths`, `DaysHoursMinutes`, `DaysHoursMinutesSeconds`, `Custom`

---

### Layout Groups [NEW]

Автоматически расставляют дочерние элементы — не нужно считать позиции вручную.

```csharp
// Горизонтальная раскладка
new CuiHorizontalLayoutGroupComponent
{
    Spacing                = 5f,
    ChildAlignment         = TextAnchor.MiddleLeft,
    ChildForceExpandWidth  = false,
    ChildForceExpandHeight = true,
    ChildControlWidth      = true,
    ChildControlHeight     = true,
    Padding                = "5 5 5 5", // left right top bottom
}

// Вертикальная раскладка
new CuiVerticalLayoutGroupComponent { Spacing = 4f /* аналогично */ }

// Сетка
new CuiGridLayoutGroupComponent
{
    CellSize        = "100 50",
    Spacing         = "5 5",
    StartCorner     = GridLayoutGroup.Corner.UpperLeft,
    StartAxis       = GridLayoutGroup.Axis.Horizontal,
    ChildAlignment  = TextAnchor.UpperLeft,
    Constraint      = GridLayoutGroup.Constraint.FixedColumnCount,
    ConstraintCount = 4,
    Padding         = "5 5 5 5",
}

// Авто-размер под содержимое
new CuiContentSizeFitterComponent
{
    HorizontalFit = ContentSizeFitter.FitMode.PreferredSize,
    VerticalFit   = ContentSizeFitter.FitMode.PreferredSize,
}

// Переопределить размер дочернего элемента
new CuiLayoutElementComponent
{
    PreferredWidth  = 200f,
    PreferredHeight = 40f,
    MinWidth        = 100f,
    MinHeight       = 30f,
    FlexibleWidth   = 1f,
    FlexibleHeight  = 0f,
    IgnoreLayout    = false,
}
```

---

### CuiScrollViewComponent [NEW]

```csharp
new CuiScrollViewComponent
{
    ContentTransform = new CuiRectTransform
    {
        AnchorMin = "0 0",
        AnchorMax = "1 0",
        OffsetMin = "0 0",
        OffsetMax = "0 800", // высота контента
    },
    Horizontal        = false,
    Vertical          = true,
    MovementType      = ScrollRect.MovementType.Elastic,
    Elasticity        = 0.1f,
    Inertia           = true,
    DecelerationRate  = 0.135f,
    ScrollSensitivity = 30f,
    VerticalScrollbar = new CuiScrollbar
    {
        Size           = 4f,
        HandleColor    = "0.7 0.7 0.7 0.5",
        HighlightColor = "1 1 1 0.5",
        PressedColor   = "0.5 0.5 0.5 0.8",
        TrackColor     = "0 0 0 0.2",
        AutoHide       = true,
    },
}
```

> Дочерние элементы добавляются с Parent = имя ScrollView элемента.

---

### CuiCanvasGroupComponent [NEW]

```csharp
new CuiCanvasGroupComponent
{
    Alpha          = 0.8f,  // прозрачность всей группы
    BlocksRaycasts = true,  // блокировка кликов
    Interactable   = true,
    Fade           = "0.5", // скорость фейда
}
```

---

### CuiMaskComponent [NEW]

```csharp
new CuiMaskComponent { ShowMaskGraphic = false }
```

---

### CuiTooltipComponent [NEW]

```csharp
new CuiTooltipComponent
{
    Text        = "Нажмите для открытия",
    TooltipType = CommunityEntity.TooltipType.Default,
    Offset      = "0 10",
    UseCentre   = true,
    Delay       = Tooltip.DelayType.Short,
    Position    = TooltipContainer.PositionMode.Cursor,
}
```

---

### CuiDraggableComponent [NEW]

```csharp
new CuiDraggableComponent
{
    LimitToParent = true,
    MaxDistance   = 0f,
    AllowSwapping = false,
    DropAnywhere  = false,
    DragAlpha     = 0.6f,
    KeepOnTop     = true,
    PositionRPC   = CommunityEntity.DraggablePositionSendType.None,
    AnchorOffset  = "0 0",
    Filter        = "",
    ParentPadding = "0 0 0 0",
    MoveToAnchor  = false,
    RebuildAnchor = false,
}
```

---

### CuiNeedsCursorComponent / CuiNeedsKeyboardComponent

```csharp
new CuiNeedsCursorComponent()    // показать курсор мыши
new CuiNeedsKeyboardComponent()  // [NEW] захват клавиатуры (отдельный компонент)
```

> Также: `CuiPanel.CursorEnabled = true` и `CuiPanel.KeyboardEnabled = true`

---

### CuiSlotComponent [NEW]

```csharp
new CuiSlotComponent { Filter = "" }  // drag-and-drop инвентарный слот
```

---

## CuiHelper — методы

```csharp
// Добавить UI
CuiHelper.AddUi(BasePlayer player, CuiElementContainer container);
CuiHelper.AddUi(BasePlayer player, string json);

// Уничтожить один элемент
CuiHelper.DestroyUi(BasePlayer player, string name);

// [NEW] Уничтожить несколько элементов одним сетевым пакетом
CuiHelper.DestroyUi(BasePlayer player, List<string> names);
CuiHelper.DestroyUi(BasePlayer player, string[] names);

// Сгенерировать уникальное имя
string name = CuiHelper.GetGuid();

// Конвертация цвета
CuiHelper.SetColor(cuiComponent, color); // Color -> "R G B A"
Color c = CuiHelper.GetColor(cuiComponent);

// Сериализация
string json = CuiHelper.ToJson(elements, format: false);
List<CuiElement> els = CuiHelper.FromJson(json);
```

---

## Хуки

```csharp
// Заблокировать AddUi для конкретного игрока
object CanUseUI(BasePlayer player, string json) { return null; /* или объект = запрет */ }

// Уведомление при DestroyUi
void OnDestroyUI(BasePlayer player, string elem) { }
```

---

## Паттерны (helper-методы)

### CreatePanel

```csharp
static string CreatePanel(
    ref CuiElementContainer c, string parent, string name, string color,
    string aMin, string aMax, string oMin = null, string oMax = null,
    bool cursor = false, bool keyboard = false,
    float fadeIn = 0f, float fadeOut = 0f,
    string material = null, string destroyUi = null)
{
    return c.Add(new CuiPanel
    {
        Image           = new CuiImageComponent { Color = color, Material = material, FadeIn = fadeIn },
        RectTransform   = new CuiRectTransformComponent
                          { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax },
        CursorEnabled   = cursor,
        KeyboardEnabled = keyboard, // [NEW]
        FadeOut         = fadeOut,
    }, parent, name, destroyUi);
}
```

### CreateLabel

```csharp
static void CreateLabel(
    ref CuiElementContainer c, string parent, string text, int size, string color,
    string aMin, string aMax, string oMin = null, string oMax = null,
    TextAnchor align = TextAnchor.MiddleCenter,
    string font = "assets/content/ui/fonts/robotocondensed-regular.ttf",
    float fadeIn = 0f, float fadeOut = 0f,
    bool update = false) // [NEW] update=true -> без мерцания
{
    c.Add(new CuiElement
    {
        Parent = parent, Name = CuiHelper.GetGuid(), FadeOut = fadeOut, Update = update,
        Components =
        {
            new CuiTextComponent
                { Text = text, FontSize = size, Color = color, Font = font, Align = align, FadeIn = fadeIn },
            new CuiRectTransformComponent
                { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax }
        }
    });
}
```

### CreateButton

```csharp
static void CreateButton(
    ref CuiElementContainer c, string parent, string color,
    string text, int fontSize, string textColor,
    string aMin, string aMax, string oMin = null, string oMax = null,
    string command = "", string close = null,
    TextAnchor align = TextAnchor.MiddleCenter,
    float fadeIn = 0f, float fadeOut = 0f)
{
    c.Add(new CuiButton
    {
        Button        = new CuiButtonComponent
                        { Color = color, Command = command, Close = close, FadeIn = fadeIn },
        RectTransform = new CuiRectTransformComponent
                        { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax },
        Text          = new CuiTextComponent
                        { Text = text, FontSize = fontSize, Color = textColor, Align = align },
        FadeOut       = fadeOut,
    }, parent);
}
```

### CreateImage (RawImage / PNG)

```csharp
static void CreateImage(
    ref CuiElementContainer c, string parent, string png,
    string aMin, string aMax, string oMin = null, string oMax = null,
    string color = "1 1 1 1", float fadeIn = 0f, float fadeOut = 0f)
{
    c.Add(new CuiElement
    {
        Parent = parent, Name = CuiHelper.GetGuid(), FadeOut = fadeOut,
        Components =
        {
            new CuiRawImageComponent { Png = png, Color = color, FadeIn = fadeIn },
            new CuiRectTransformComponent
                { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax }
        }
    });
}
```

### CreateItemIcon [NEW]

```csharp
// Показать иконку предмета без ImageLibrary
static void CreateItemIcon(
    ref CuiElementContainer c, string parent,
    int itemId, ulong skinId,
    string aMin, string aMax, string oMin = null, string oMax = null)
{
    c.Add(new CuiElement
    {
        Parent = parent, Name = CuiHelper.GetGuid(),
        Components =
        {
            new CuiImageComponent { ItemId = itemId, SkinId = skinId, Color = "1 1 1 1" },
            new CuiRectTransformComponent
                { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax }
        }
    });
}
```

### CreateInput

```csharp
static void CreateInput(
    ref CuiElementContainer c, string parent, int charLimit, string command,
    string aMin, string aMax, string oMin = null, string oMax = null,
    int fontSize = 14, string color = "1 1 1 1",
    bool readOnly = false, bool autofocus = false,
    InputField.LineType lineType = InputField.LineType.SingleLine,
    float fadeOut = 0f)
{
    c.Add(new CuiElement
    {
        Parent = parent, Name = CuiHelper.GetGuid(), FadeOut = fadeOut,
        Components =
        {
            new CuiInputFieldComponent
            {
                Text = "", FontSize = fontSize, Color = color,
                CharsLimit = charLimit, Command = command,
                NeedsKeyboard = true, // ОБЯЗАТЕЛЬНО [NEW]
                ReadOnly = readOnly, Autofocus = autofocus, LineType = lineType,
                Align = TextAnchor.MiddleLeft,
            },
            new CuiRectTransformComponent
                { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax }
        }
    });
}
```

### UpdateElement [NEW] — обновление без DestroyUi

```csharp
// Обновить текст/цвет существующего лейбла без мерцания
static void UpdateLabel(
    BasePlayer player, string elemName, string newText,
    int size, string color, string aMin, string aMax)
{
    var c = new CuiElementContainer();
    c.Add(new CuiElement
    {
        Name   = elemName, // то же имя, что при создании!
        Parent = "Overlay",
        Update = true,     // ключевой флаг [NEW]
        Components =
        {
            new CuiTextComponent { Text = newText, FontSize = size, Color = color },
            new CuiRectTransformComponent { AnchorMin = aMin, AnchorMax = aMax }
        }
    });
    CuiHelper.AddUi(player, c);
}
```

### CreateOutlineLabel

```csharp
static void CreateOutlineLabel(
    ref CuiElementContainer c, string parent, string text, int size,
    string color, string outlineColor, string outlineDistance,
    string aMin, string aMax, string oMin = null, string oMax = null,
    TextAnchor align = TextAnchor.MiddleCenter,
    float fadeIn = 0f, float fadeOut = 0f)
{
    c.Add(new CuiElement
    {
        Parent = parent, Name = CuiHelper.GetGuid(), FadeOut = fadeOut,
        Components =
        {
            new CuiTextComponent
                { Text = text, FontSize = size, Color = color, Align = align, FadeIn = fadeIn },
            new CuiOutlineComponent
                { Color = outlineColor, Distance = outlineDistance },
            new CuiRectTransformComponent
                { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax }
        }
    });
}
```

---

## Лучшие практики 2026

### 1. Именование элементов — только константами

```csharp
private const string UI_MAIN   = "Plugin.Main";
private const string UI_LIST   = "Plugin.List";
private const string UI_HEADER = "Plugin.Header";
```

### 2. Всегда DestroyUi перед AddUi (при пересоздании)

```csharp
CuiHelper.DestroyUi(player, UI_MAIN);
CuiHelper.AddUi(player, container);
```

### 3. Update = true для горячих данных (HUD, таймеры, счётчики)

Исключает мерцание при частом обновлении.

### 4. DestroyUi при выходе игрока

```csharp
void OnPlayerDisconnected(BasePlayer player, string reason)
{
    CuiHelper.DestroyUi(player, UI_MAIN);
}
```

### 5. Один вызов DestroyUi для нескольких элементов [NEW]

```csharp
// Один сетевой пакет вместо N вызовов
CuiHelper.DestroyUi(player, new[] { UI_MAIN, UI_LIST, UI_HEADER });
```

### 6. Offset-only для фиксированного размера

```csharp
// Anchor = точка привязки, Offset = половина размера в каждую сторону
AnchorMin = "0.5 0.5", AnchorMax = "0.5 0.5",
OffsetMin = $"{-cfg.Width / 2f} {-cfg.Height / 2f}",
OffsetMax = $"{cfg.Width / 2f} {cfg.Height / 2f}"
```

### 7. Все числа — из конфига (правило проекта)

```csharp
// ЗАПРЕЩЕНО: OffsetMin = "10 -200"
// ОБЯЗАТЕЛЬНО: OffsetMin = $"{cfg.PaddingX} {-cfg.Height}"
```

---

## История изменений API

| Период | Что добавлено |
|--------|---------------|
| ~2019  | Базовый API: `CuiPanel`, `CuiButton`, `CuiLabel`, `CuiImageComponent`, `CuiRawImageComponent`, `CuiOutlineComponent`, `CuiInputFieldComponent` |
| ~2021  | `CuiCountdownComponent`, `NeedsKeyboard` в InputField |
| ~2022  | `CuiScrollViewComponent`, `CuiCanvasGroupComponent`, `CuiMaskComponent` |
| ~2023  | Layout Groups (H/V/Grid), `ContentSizeFitter`, `LayoutElement` |
| ~2024  | `CuiTooltipComponent`, `CuiDraggableComponent`, `CuiSlotComponent` |
| ~2024  | `CuiElement.Update`, `CuiElement.DestroyUi`, `CuiElement.ActiveSelf` |
| ~2024  | `CuiImageComponent.ItemId/SkinId`, `CuiRawImageComponent.SteamId/Url` |
| ~2025  | `CuiRectTransform.Rotation/Pivot/SetParent`, `CuiButtonComponent` — 5 цветовых состояний |
| 2026   | `ThreadLocal<JsonWriterResources>` + `JsonArrayPool` в `CuiHelper` (zero-alloc сериализация) |

