// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  CuiBuilder — Шаблонный helper для работы с Oxide CUI API (2026)       ║
// ║  Источник: OxideMod/Oxide.Rust/src/RustCui.cs (develop)               ║
// ║                                                                          ║
// ║  Использование:                                                          ║
// ║    Скопировать класс CuiBuilder внутрь namespace Oxide.Plugins          ║
// ║    или вложить как вложенный private static class в тело плагина.       ║
// ╚══════════════════════════════════════════════════════════════════════════╝

using Oxide.Game.Rust.Cui;
using UnityEngine;
using UnityEngine.UI;

namespace Oxide.Plugins
{
    /// <summary>
    /// Статический helper для построения Oxide CUI-интерфейсов.
    /// Все методы принимают позиционирование через Anchor + Offset параметры.
    /// </summary>
    /// <remarks>
    /// Инварианты:
    ///   - Ни один параметр цвета/позиции НЕ хардкодится здесь.
    ///   - Все значения передаются извне (из конфига плагина).
    ///   - Все методы принимают oMin/oMax как nullable (null = не задано).
    /// </remarks>
    internal static class CuiBuilder
    {
        // ─────────────────────────────────────────────────────────────────
        #region Panel

        /// <summary>
        /// Добавляет панель (фон) в контейнер.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента или слоя (Overlay/Hud/Under).</param>
        /// <param name="name">Уникальное имя панели. null = авто-GUID.</param>
        /// <param name="color">Цвет панели в формате "R G B A".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="cursor">Показать курсор при открытии.</param>
        /// <param name="keyboard">Захватить клавиатуру при открытии.</param>
        /// <param name="material">Путь к материалу. null = стандартный.</param>
        /// <param name="fadeIn">Время появления (секунды).</param>
        /// <param name="fadeOut">Время исчезновения (секунды).</param>
        /// <param name="destroyUi">Имя элемента, который уничтожить при создании этого.</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string Panel(
            ref CuiElementContainer c,
            string parent,
            string name,
            string color,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            bool cursor = false,
            bool keyboard = false,
            string material = null,
            float fadeIn = 0f,
            float fadeOut = 0f,
            string destroyUi = null)
        {
            return c.Add(new CuiPanel
            {
                Image = new CuiImageComponent
                {
                    Color    = color,
                    Material = material,
                    FadeIn   = fadeIn
                },
                RectTransform = new CuiRectTransformComponent
                {
                    AnchorMin = aMin,
                    AnchorMax = aMax,
                    OffsetMin = oMin,
                    OffsetMax = oMax
                },
                CursorEnabled   = cursor,
                KeyboardEnabled = keyboard,
                FadeOut         = fadeOut
            }, parent, name, destroyUi);
        }

        /// <summary>
        /// Добавляет панель с RawImage (PNG/URL/Steam) вместо заливки цветом.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента или слоя.</param>
        /// <param name="name">Уникальное имя панели. null = авто-GUID.</param>
        /// <param name="png">PNG из FileStorage (ImageLibrary). null = не используется.</param>
        /// <param name="url">URL изображения. null = не используется.</param>
        /// <param name="steamId">SteamID для аватарки. null = не используется.</param>
        /// <param name="color">Тинт-цвет "R G B A". "1 1 1 1" = без тинта.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="cursor">Показать курсор при открытии.</param>
        /// <param name="keyboard">Захватить клавиатуру при открытии.</param>
        /// <param name="fadeIn">Время появления (секунды).</param>
        /// <param name="fadeOut">Время исчезновения (секунды).</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string PanelRaw(
            ref CuiElementContainer c,
            string parent,
            string name,
            string color,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string png = null,
            string url = null,
            string steamId = null,
            bool cursor = false,
            bool keyboard = false,
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            return c.Add(new CuiPanel
            {
                Image    = null,
                RawImage = new CuiRawImageComponent
                {
                    Color   = color,
                    Png     = png,
                    Url     = url,
                    SteamId = steamId,
                    FadeIn  = fadeIn
                },
                RectTransform = new CuiRectTransformComponent
                {
                    AnchorMin = aMin,
                    AnchorMax = aMax,
                    OffsetMin = oMin,
                    OffsetMax = oMax
                },
                CursorEnabled   = cursor,
                KeyboardEnabled = keyboard,
                FadeOut         = fadeOut
            }, parent, name);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Label

        /// <summary>
        /// Добавляет текстовую метку.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="text">Текст (поддерживает rich text Unity).</param>
        /// <param name="size">Размер шрифта.</param>
        /// <param name="color">Цвет текста "R G B A".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="align">Выравнивание текста.</param>
        /// <param name="font">Путь к шрифту. null = системный.</param>
        /// <param name="overflow">Режим вертикального переполнения.</param>
        /// <param name="fadeIn">Время появления (секунды).</param>
        /// <param name="fadeOut">Время исчезновения (секунды).</param>
        /// <param name="update">true = обновить существующий элемент без пересоздания (без мерцания).</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string Label(
            ref CuiElementContainer c,
            string parent,
            string text,
            int size,
            string color,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            TextAnchor align = TextAnchor.MiddleCenter,
            string font = null,
            VerticalWrapMode overflow = VerticalWrapMode.Truncate,
            float fadeIn = 0f,
            float fadeOut = 0f,
            bool update = false)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name    = name,
                Parent  = parent,
                FadeOut = fadeOut,
                Update  = update,
                Components =
                {
                    new CuiTextComponent
                    {
                        Text             = text,
                        FontSize         = size,
                        Color            = color,
                        Font             = font,
                        Align            = align,
                        VerticalOverflow = overflow,
                        FadeIn           = fadeIn
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        /// <summary>
        /// Добавляет текстовую метку с обводкой (outline).
        /// </summary>
        /// <param name="outlineColor">Цвет обводки "R G B A".</param>
        /// <param name="outlineDist">Дистанция обводки "X Y", например "1 -1".</param>
        /// <inheritdoc cref="Label"/>
        public static string LabelOutline(
            ref CuiElementContainer c,
            string parent,
            string text,
            int size,
            string color,
            string outlineColor,
            string outlineDist,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            TextAnchor align = TextAnchor.MiddleCenter,
            string font = null,
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name    = name,
                Parent  = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiTextComponent
                    {
                        Text     = text,
                        FontSize = size,
                        Color    = color,
                        Font     = font,
                        Align    = align,
                        FadeIn   = fadeIn
                    },
                    new CuiOutlineComponent
                    {
                        Color    = outlineColor,
                        Distance = outlineDist
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Button

        /// <summary>
        /// Добавляет кнопку с текстом.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="bgColor">Цвет фона кнопки "R G B A".</param>
        /// <param name="text">Текст на кнопке.</param>
        /// <param name="textSize">Размер шрифта текста.</param>
        /// <param name="textColor">Цвет текста "R G B A".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="command">Консольная команда при нажатии.</param>
        /// <param name="close">Имя элемента, который закрыть при нажатии.</param>
        /// <param name="sprite">Спрайт кнопки. null = стандартный.</param>
        /// <param name="highlightColor">Цвет при наведении. null = не задан.</param>
        /// <param name="pressedColor">Цвет при нажатии. null = не задан.</param>
        /// <param name="fadeDuration">Длительность анимации смены цвета (секунды).</param>
        /// <param name="interactable">false = кнопка задизейблена визуально.</param>
        /// <param name="align">Выравнивание текста.</param>
        /// <param name="font">Путь к шрифту. null = системный.</param>
        /// <param name="fadeIn">Время появления (секунды).</param>
        /// <param name="fadeOut">Время исчезновения (секунды).</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string Button(
            ref CuiElementContainer c,
            string parent,
            string bgColor,
            string text,
            int textSize,
            string textColor,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string command = "",
            string close = null,
            string sprite = null,
            string highlightColor = null,
            string pressedColor = null,
            float? fadeDuration = null,
            bool? interactable = null,
            TextAnchor align = TextAnchor.MiddleCenter,
            string font = null,
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            return c.Add(new CuiButton
            {
                Button = new CuiButtonComponent
                {
                    Color            = bgColor,
                    Command          = command,
                    Close            = close,
                    Sprite           = sprite,
                    HighlightedColor = highlightColor,
                    PressedColor     = pressedColor,
                    FadeDuration     = fadeDuration,
                    Interactable     = interactable,
                    FadeIn           = fadeIn
                },
                RectTransform = new CuiRectTransformComponent
                {
                    AnchorMin = aMin,
                    AnchorMax = aMax,
                    OffsetMin = oMin,
                    OffsetMax = oMax
                },
                Text = new CuiTextComponent
                {
                    Text     = text,
                    FontSize = textSize,
                    Color    = textColor,
                    Font     = font,
                    Align    = align
                },
                FadeOut = fadeOut
            }, parent);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Image

        /// <summary>
        /// Добавляет изображение через RawImage (PNG из FileStorage/ImageLibrary).
        /// </summary>
        /// <param name="png">Строка-идентификатор PNG из FileStorage.</param>
        /// <inheritdoc cref="Panel"/>
        public static string Image(
            ref CuiElementContainer c,
            string parent,
            string png,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string color = "1 1 1 1",
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name    = name,
                Parent  = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png    = png,
                        Color  = color,
                        FadeIn = fadeIn
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        /// <summary>
        /// Добавляет аватарку игрока через SteamID.
        /// </summary>
        /// <param name="steamId">SteamID64 игрока (строка).</param>
        /// <inheritdoc cref="Image"/>
        public static string ImageSteam(
            ref CuiElementContainer c,
            string parent,
            string steamId,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string color = "1 1 1 1",
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name    = name,
                Parent  = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiRawImageComponent
                    {
                        SteamId = steamId,
                        Color   = color,
                        FadeIn  = fadeIn
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        /// <summary>
        /// Добавляет иконку игрового предмета по ItemID и SkinID.
        /// Не требует ImageLibrary.
        /// </summary>
        /// <param name="itemId">ItemDefinition.itemid предмета.</param>
        /// <param name="skinId">Skin ID (0 = стандартный скин).</param>
        /// <inheritdoc cref="Image"/>
        public static string ItemIcon(
            ref CuiElementContainer c,
            string parent,
            int itemId,
            ulong skinId,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string color = "1 1 1 1",
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name    = name,
                Parent  = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiImageComponent
                    {
                        ItemId = itemId,
                        SkinId = skinId,
                        Color  = color
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Input

        /// <summary>
        /// Добавляет поле ввода текста.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="command">Консольная команда при отправке текста.</param>
        /// <param name="charLimit">Максимальное количество символов.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="placeholder">Начальный текст поля.</param>
        /// <param name="fontSize">Размер шрифта.</param>
        /// <param name="color">Цвет текста "R G B A".</param>
        /// <param name="font">Путь к шрифту. null = системный.</param>
        /// <param name="align">Выравнивание текста.</param>
        /// <param name="lineType">Тип переноса строк.</param>
        /// <param name="readOnly">true = только чтение.</param>
        /// <param name="isPassword">true = скрыть символы.</param>
        /// <param name="autofocus">true = автофокус при открытии.</param>
        /// <param name="hudMenuInput">true = работа в HUD-меню.</param>
        /// <param name="fadeOut">Время исчезновения (секунды).</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string Input(
            ref CuiElementContainer c,
            string parent,
            string command,
            int charLimit,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string placeholder = "",
            int fontSize = 14,
            string color = "1 1 1 1",
            string font = null,
            TextAnchor align = TextAnchor.MiddleLeft,
            InputField.LineType lineType = InputField.LineType.SingleLine,
            bool readOnly = false,
            bool isPassword = false,
            bool autofocus = false,
            bool hudMenuInput = false,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name    = name,
                Parent  = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Text          = placeholder,
                        FontSize      = fontSize,
                        Color         = color,
                        Font          = font,
                        Align         = align,
                        CharsLimit    = charLimit,
                        Command       = command,
                        LineType      = lineType,
                        ReadOnly      = readOnly,
                        IsPassword    = isPassword,
                        NeedsKeyboard = true,     // обязательно для приёма ввода
                        Autofocus     = autofocus,
                        HudMenuInput  = hudMenuInput
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Countdown

        /// <summary>
        /// Добавляет встроенный таймер обратного отсчёта.
        /// Не требует timer.Every — работает на стороне клиента.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="startTime">Начальное значение таймера (секунды).</param>
        /// <param name="endTime">Конечное значение (обычно 0).</param>
        /// <param name="step">Шаг изменения (отрицательный для убывания, например -1).</param>
        /// <param name="interval">Интервал обновления (секунды).</param>
        /// <param name="format">Формат отображения времени.</param>
        /// <param name="command">Консольная команда по завершении. null = не вызывать.</param>
        /// <param name="destroyIfDone">true = уничтожить элемент по завершении.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string Countdown(
            ref CuiElementContainer c,
            string parent,
            float startTime,
            float endTime,
            float step,
            float interval,
            TimerFormat format,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string command = null,
            bool destroyIfDone = true)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name   = name,
                Parent = parent,
                Components =
                {
                    new CuiCountdownComponent
                    {
                        StartTime     = startTime,
                        EndTime       = endTime,
                        Step          = step,
                        Interval      = interval,
                        TimerFormat   = format,
                        Command       = command,
                        DestroyIfDone = destroyIfDone
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region ScrollView

        /// <summary>
        /// Добавляет вертикальную прокручиваемую область.
        /// Дочерние элементы добавлять с Parent = возвращённое имя.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="name">Уникальное имя. null = авто-GUID.</param>
        /// <param name="contentHeight">Высота контентной области в пикселях.</param>
        /// <param name="scrollSensitivity">Чувствительность скролла.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="scrollbarSize">Ширина полосы прокрутки в пикселях.</param>
        /// <param name="scrollbarColor">Цвет ползунка прокрутки "R G B A".</param>
        /// <param name="scrollbarTrackColor">Цвет дорожки прокрутки "R G B A".</param>
        /// <param name="autoHideScrollbar">true = скрыть полосу если не нужна.</param>
        /// <returns>Имя ScrollView-элемента (используй как Parent для дочерних).</returns>
        public static string ScrollView(
            ref CuiElementContainer c,
            string parent,
            string name,
            float contentHeight,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            float scrollSensitivity = 30f,
            float scrollbarSize = 4f,
            string scrollbarColor = "0.7 0.7 0.7 0.5",
            string scrollbarTrackColor = "0 0 0 0.2",
            bool autoHideScrollbar = true)
        {
            if (string.IsNullOrEmpty(name))
                name = CuiHelper.GetGuid();

            c.Add(new CuiElement
            {
                Name   = name,
                Parent = parent,
                Components =
                {
                    new CuiScrollViewComponent
                    {
                        ContentTransform = new CuiRectTransform
                        {
                            AnchorMin = "0 1",
                            AnchorMax = "1 1",
                            OffsetMin = $"0 {-contentHeight}",
                            OffsetMax = "0 0"
                        },
                        Horizontal        = false,
                        Vertical          = true,
                        MovementType      = ScrollRect.MovementType.Elastic,
                        Elasticity        = 0.1f,
                        Inertia           = true,
                        DecelerationRate  = 0.135f,
                        ScrollSensitivity = scrollSensitivity,
                        VerticalScrollbar = new CuiScrollbar
                        {
                            Size        = scrollbarSize,
                            HandleColor = scrollbarColor,
                            TrackColor  = scrollbarTrackColor,
                            AutoHide    = autoHideScrollbar
                        }
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = aMin,
                        AnchorMax = aMax,
                        OffsetMin = oMin,
                        OffsetMax = oMax
                    }
                }
            });
            return name;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Update (без мерцания)

        /// <summary>
        /// Обновляет текст существующего элемента без его пересоздания (без мерцания).
        /// Требует, чтобы элемент был создан с заданным именем.
        /// </summary>
        /// <param name="player">Игрок, которому обновить UI.</param>
        /// <param name="elemName">Имя существующего элемента.</param>
        /// <param name="parent">Родитель элемента (тот же, что при создании).</param>
        /// <param name="newText">Новый текст.</param>
        /// <param name="size">Размер шрифта.</param>
        /// <param name="color">Цвет текста "R G B A".</param>
        /// <param name="aMin">AnchorMin (тот же, что при создании).</param>
        /// <param name="aMax">AnchorMax (тот же, что при создании).</param>
        public static void UpdateText(
            BasePlayer player,
            string elemName,
            string parent,
            string newText,
            int size,
            string color,
            string aMin, string aMax)
        {
            var c = new CuiElementContainer();
            c.Add(new CuiElement
            {
                Name   = elemName,
                Parent = parent,
                Update = true,    // обновить существующий — ключевой флаг
                Components =
                {
                    new CuiTextComponent { Text = newText, FontSize = size, Color = color },
                    new CuiRectTransformComponent { AnchorMin = aMin, AnchorMax = aMax }
                }
            });
            CuiHelper.AddUi(player, c);
        }

        /// <summary>
        /// Показывает или скрывает существующий элемент без его уничтожения.
        /// </summary>
        /// <param name="player">Игрок.</param>
        /// <param name="elemName">Имя элемента.</param>
        /// <param name="parent">Родитель элемента.</param>
        /// <param name="visible">true = показать, false = скрыть.</param>
        public static void SetVisible(
            BasePlayer player,
            string elemName,
            string parent,
            bool visible)
        {
            var c = new CuiElementContainer();
            c.Add(new CuiElement
            {
                Name       = elemName,
                Parent     = parent,
                Update     = true,
                ActiveSelf = visible
            });
            CuiHelper.AddUi(player, c);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Destroy

        /// <summary>
        /// Уничтожает один элемент UI у игрока.
        /// </summary>
        public static void Destroy(BasePlayer player, string name)
            => CuiHelper.DestroyUi(player, name);

        /// <summary>
        /// Уничтожает несколько элементов UI одним сетевым пакетом.
        /// </summary>
        public static void Destroy(BasePlayer player, params string[] names)
            => CuiHelper.DestroyUi(player, names);

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Color helpers

        /// <summary>
        /// Конвертирует UnityEngine.Color в строку "R G B A" для CUI.
        /// </summary>
        public static string ToColor(Color color)
            => $"{color.r:F3} {color.g:F3} {color.b:F3} {color.a:F3}";

        /// <summary>
        /// Конвертирует HEX-строку (#RRGGBB или RRGGBB) в строку "R G B A" для CUI.
        /// </summary>
        /// <param name="hex">HEX цвет, например "cd4632" или "#cd4632".</param>
        /// <param name="alpha">Прозрачность 0..1.</param>
        public static string HexToColor(string hex, float alpha = 1f)
        {
            hex = hex.TrimStart('#');
            if (hex.Length != 6) return $"1 1 1 {alpha:F3}";
            float r = System.Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
            float g = System.Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
            float b = System.Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
            return $"{r:F3} {g:F3} {b:F3} {alpha:F3}";
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Position helpers

        /// <summary>
        /// Вычисляет OffsetMin/OffsetMax для элемента фиксированного размера,
        /// центрированного по заданной якорной точке.
        /// </summary>
        /// <param name="width">Ширина элемента в пикселях.</param>
        /// <param name="height">Высота элемента в пикселях.</param>
        /// <param name="oMin">Выходное значение OffsetMin.</param>
        /// <param name="oMax">Выходное значение OffsetMax.</param>
        public static void CenteredOffset(float width, float height, out string oMin, out string oMax)
        {
            oMin = $"{-width / 2f} {-height / 2f}";
            oMax = $"{width / 2f} {height / 2f}";
        }

        /// <summary>
        /// Вычисляет OffsetMin/OffsetMax для элемента,
        /// смещённого от левого верхнего угла родителя.
        /// </summary>
        /// <param name="x">Отступ от левого края (пиксели).</param>
        /// <param name="y">Отступ от нижнего края (пиксели, положительный = вверх).</param>
        /// <param name="width">Ширина элемента.</param>
        /// <param name="height">Высота элемента.</param>
        /// <param name="oMin">Выходное значение OffsetMin.</param>
        /// <param name="oMax">Выходное значение OffsetMax.</param>
        public static void AbsoluteOffset(float x, float y, float width, float height,
            out string oMin, out string oMax)
        {
            oMin = $"{x} {y}";
            oMax = $"{x + width} {y + height}";
        }

        #endregion
    }
}
