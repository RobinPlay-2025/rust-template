// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  CuiBuilder — Шаблонный helper для работы с Oxide/Carbon CUI API (2026) ║
// ║  Источник: OxideMod/Oxide.Rust/src/RustCui.cs & Facepunch CommunityEntity║
// ║                                                                          ║
// ║  Использование:                                                          ║
// ║    Скопировать класс CuiBuilder внутрь namespace Oxide.Plugins          ║
// ║    или вложить как вложенный internal static class в тело плагина.       ║
// ╚══════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using Oxide.Game.Rust.Cui;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

namespace Oxide.Plugins
{
    /// <summary>
    /// Статический типобезопасный helper для построения Oxide/Carbon CUI интерфейсов.
    /// Поддерживает полный стек компонентов CUI (21+ компонентов, Layout-группы, 9-slice, Tooltip, Draggable, Slot, Pie, Vitals).
    /// </summary>
    /// <remarks>
    /// Инварианты:
    ///   - Ни один параметр цвета/размера/позиции НЕ хардкодится. Все значения берутся из конфигурации плагина.
    ///   - Строгая типизация: запрещено использование dynamic и отключение предупреждений компилятора.
    ///   - Поддержка Zero-flicker обновлений через Update = true и массового удаления DestroyUi.
    /// </remarks>
    internal static class CuiBuilder
    {
        // ─────────────────────────────────────────────────────────────────
        #region RectTransform & Base Creation Helper

        /// <summary>
        /// Создает компонент RectTransform с поддержкой расширенных параметров (Rotation, Pivot, SetParent, SetTransformIndex).
        /// </summary>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="rotation">Угол поворота в градусах.</param>
        /// <param name="pivot">Точка вращения "X Y" (0..1). null = по умолчанию.</param>
        /// <param name="setParent">Имя нового родителя для ре-перентинга. null = без изменений.</param>
        /// <param name="transformIndex">Индекс трансформации среди соседей (z-order).</param>
        /// <returns>Экземпляр <see cref="CuiRectTransformComponent"/>.</returns>
        public static CuiRectTransformComponent CreateRectTransform(
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            float rotation = 0f,
            string pivot = null,
            string setParent = null,
            int transformIndex = 0)
        {
            return new CuiRectTransformComponent
            {
                AnchorMin = aMin,
                AnchorMax = aMax,
                OffsetMin = oMin,
                OffsetMax = oMax,
                Rotation = rotation,
                Pivot = pivot,
                SetParent = setParent,
                SetTransformIndex = transformIndex
            };
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Panel

        /// <summary>
        /// Добавляет базовую панель (фон) в контейнер CUI элементов.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента или слоя (Overlay/Hud/Under/Hud.Menu).</param>
        /// <param name="name">Уникальное имя панели. null = авто-GUID.</param>
        /// <param name="color">Цвет панели в формате "R G B A".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="cursor">Показать ли курсор мыши при открытии.</param>
        /// <param name="keyboard">Захватить ли ввод с клавиатуры при открытии.</param>
        /// <param name="material">Путь к материалу Unity. null = стандартный.</param>
        /// <param name="fadeIn">Время плавного появления (секунды).</param>
        /// <param name="fadeOut">Время плавного исчезновения (секунды).</param>
        /// <param name="destroyUi">Имя элемента, который будет автоматически уничтожен при создании этого.</param>
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
                    Color = color,
                    Material = material,
                    FadeIn = fadeIn
                },
                RectTransform = new CuiRectTransformComponent
                {
                    AnchorMin = aMin,
                    AnchorMax = aMax,
                    OffsetMin = oMin,
                    OffsetMax = oMax
                },
                CursorEnabled = cursor,
                KeyboardEnabled = keyboard,
                FadeOut = fadeOut
            }, parent, name, destroyUi);
        }

        /// <summary>
        /// Добавляет панель с поддержкой 9-slice спрайтов и масштабирования пикселей.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="name">Уникальное имя элемента.</param>
        /// <param name="sprite">Путь к спрайту (например, "assets/content/ui/ui.background.tile.psd").</param>
        /// <param name="color">Цвет тинта "R G B A".</param>
        /// <param name="slice">Настройки слайса 9-slice "left top right bottom".</param>
        /// <param name="fillCenter">Заполнять ли центр 9-slice спрайта.</param>
        /// <param name="ppuMultiplier">Множитель плотности пикселей (PixelsPerUnitMultiplier).</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="cursor">Показать курсор мыши.</param>
        /// <param name="keyboard">Захват клавиатуры.</param>
        /// <param name="fadeIn">Время появления.</param>
        /// <param name="fadeOut">Время исчезновения.</param>
        /// <returns>Имя созданного элемента.</returns>
        public static string PanelSliced(
            ref CuiElementContainer c,
            string parent,
            string name,
            string sprite,
            string color,
            string slice,
            bool fillCenter,
            float ppuMultiplier,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            bool cursor = false,
            bool keyboard = false,
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            string elemName = string.IsNullOrEmpty(name) ? CuiHelper.GetGuid() : name;
            c.Add(new CuiElement
            {
                Name = elemName,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiImageComponent
                    {
                        Sprite = sprite,
                        Color = color,
                        ImageType = Image.Type.Sliced,
                        Slice = slice,
                        FillCenter = fillCenter,
                        PixelsPerUnitMultiplier = ppuMultiplier,
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

            if (cursor)
            {
                c.Add(new CuiElement
                {
                    Parent = elemName,
                    Components = { new CuiNeedsCursorComponent() }
                });
            }

            if (keyboard)
            {
                c.Add(new CuiElement
                {
                    Parent = elemName,
                    Components = { new CuiNeedsKeyboardComponent() }
                });
            }

            return elemName;
        }

        /// <summary>
        /// Добавляет панель с RawImage (PNG / URL / Steam Avatar).
        /// </summary>
        /// <inheritdoc cref="Panel"/>
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
                Image = null,
                RawImage = new CuiRawImageComponent
                {
                    Color = color,
                    Png = png,
                    Url = url,
                    SteamId = steamId,
                    FadeIn = fadeIn
                },
                RectTransform = new CuiRectTransformComponent
                {
                    AnchorMin = aMin,
                    AnchorMax = aMax,
                    OffsetMin = oMin,
                    OffsetMax = oMax
                },
                CursorEnabled = cursor,
                KeyboardEnabled = keyboard,
                FadeOut = fadeOut
            }, parent, name);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Label

        /// <summary>
        /// Добавляет текстовую метку (Text).
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="text">Отображаемый текст (поддерживает rich text Unity).</param>
        /// <param name="size">Размер шрифта в пунктах.</param>
        /// <param name="color">Цвет текста "R G B A".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="align">Выравнивание текста.</param>
        /// <param name="font">Путь к шрифту. null = стандартный.</param>
        /// <param name="overflow">Режим вертикального переполнения.</param>
        /// <param name="fadeIn">Время плавного появления.</param>
        /// <param name="fadeOut">Время плавного исчезновения.</param>
        /// <param name="update">true = обновить существующий элемент без мерцания (Update pattern).</param>
        /// <param name="name">Кастомное имя элемента (нужно при update=true).</param>
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
            bool update = false,
            string name = null)
        {
            string elemName = string.IsNullOrEmpty(name) ? CuiHelper.GetGuid() : name;
            c.Add(new CuiElement
            {
                Name = elemName,
                Parent = parent,
                FadeOut = fadeOut,
                Update = update,
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = text,
                        FontSize = size,
                        Color = color,
                        Font = font,
                        Align = align,
                        VerticalOverflow = overflow,
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
            return elemName;
        }

        /// <summary>
        /// Добавляет текстовую метку с эффектом обводки (Outline).
        /// </summary>
        /// <param name="outlineColor">Цвет обводки "R G B A".</param>
        /// <param name="outlineDist">Смещение обводки "X Y" в пикселях, например "1 -1".</param>
        /// <param name="useGraphicAlpha">Учитывать ли альфа-канал текста для обводки.</param>
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
            bool useGraphicAlpha = false,
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = text,
                        FontSize = size,
                        Color = color,
                        Font = font,
                        Align = align,
                        FadeIn = fadeIn
                    },
                    new CuiOutlineComponent
                    {
                        Color = outlineColor,
                        Distance = outlineDist,
                        UseGraphicAlpha = useGraphicAlpha
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
        /// Добавляет кнопку с текстом и расширенным управлением состояниями цвета (2025-2026).
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="bgColor">Цвет фона кнопки "R G B A".</param>
        /// <param name="text">Текст кнопки.</param>
        /// <param name="textSize">Размер шрифта текста.</param>
        /// <param name="textColor">Цвет текста "R G B A".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="command">Консольная команда, вызываемая при клике.</param>
        /// <param name="close">Имя CUI элемента, который автоматически закроется при клике.</param>
        /// <param name="sprite">Спрайт кнопки. null = стандартный.</param>
        /// <param name="normalColor">Цвет нормального состояния. null = берется из bgColor.</param>
        /// <param name="highlightColor">Цвет при наведении курсора. null = не задан.</param>
        /// <param name="pressedColor">Цвет при нажатии. null = не задан.</param>
        /// <param name="selectedColor">Цвет выбранного состояния. null = не задан.</param>
        /// <param name="disabledColor">Цвет заблокированного состояния. null = не задан.</param>
        /// <param name="colorMultiplier">Множитель интенсивности цвета.</param>
        /// <param name="fadeDuration">Длительность анимации смены цвета (секунды).</param>
        /// <param name="interactable">false = кнопка некликабельна и имеет disabledColor.</param>
        /// <param name="align">Выравнивание текста.</param>
        /// <param name="font">Шрифт текста.</param>
        /// <param name="fadeIn">Время появления.</param>
        /// <param name="fadeOut">Время исчезновения.</param>
        /// <returns>Имя созданного элемента кнопки.</returns>
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
            string normalColor = null,
            string highlightColor = null,
            string pressedColor = null,
            string selectedColor = null,
            string disabledColor = null,
            float colorMultiplier = 1f,
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
                    Color = bgColor,
                    Command = command,
                    Close = close,
                    Sprite = sprite,
                    NormalColor = normalColor,
                    HighlightedColor = highlightColor,
                    PressedColor = pressedColor,
                    SelectedColor = selectedColor,
                    DisabledColor = disabledColor,
                    ColorMultiplier = colorMultiplier,
                    FadeDuration = fadeDuration,
                    Interactable = interactable,
                    FadeIn = fadeIn
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
                    Text = text,
                    FontSize = textSize,
                    Color = textColor,
                    Font = font,
                    Align = align
                },
                FadeOut = fadeOut
            }, parent);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Image & Media

        /// <summary>
        /// Добавляет изображение по PNG id из FileStorage / ImageLibrary.
        /// </summary>
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
                Name = name,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = png,
                        Color = color,
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
        /// Добавляет изображение, загружаемое напрямую по HTTP/HTTPS URL на клиенте.
        /// </summary>
        /// <param name="url">Прямая ссылка на картинку.</param>
        /// <inheritdoc cref="Image"/>
        public static string ImageUrl(
            ref CuiElementContainer c,
            string parent,
            string url,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            string color = "1 1 1 1",
            float fadeIn = 0f,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Url = url,
                        Color = color,
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
        /// Добавляет аватарку игрока по SteamID64 (клиент скачивает автоматически).
        /// </summary>
        /// <param name="steamId">SteamID64 строка.</param>
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
                Name = name,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiRawImageComponent
                    {
                        SteamId = steamId,
                        Color = color,
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
        /// Добавляет нативную иконку игрового предмета по ItemID и SkinID (без сторонних плагинов и FileStorage).
        /// </summary>
        /// <param name="itemId">ItemDefinition.itemid (например, -151838493).</param>
        /// <param name="skinId">Skin ID предмета (0 = дефолтный скин).</param>
        /// <param name="ppuMultiplier">Множитель пикселей спрайта.</param>
        /// <inheritdoc cref="Image"/>
        public static string ItemIcon(
            ref CuiElementContainer c,
            string parent,
            int itemId,
            ulong skinId = 0UL,
            string aMin = "0 0", string aMax = "1 1",
            string oMin = null, string oMax = null,
            string color = "1 1 1 1",
            float ppuMultiplier = 1f,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiImageComponent
                    {
                        ItemId = itemId,
                        SkinId = skinId,
                        Color = color,
                        PixelsPerUnitMultiplier = ppuMultiplier
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
        #region InputField

        /// <summary>
        /// Добавляет поле ввода текста с поддержкой клавиатурного фокуса.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="command">Консольная команда, вызываемая при отправке текста.</param>
        /// <param name="charLimit">Лимит символов ввода.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="placeholder">Начальный/подсказочный текст.</param>
        /// <param name="fontSize">Размер шрифта.</param>
        /// <param name="color">Цвет текста "R G B A".</param>
        /// <param name="font">Шрифт.</param>
        /// <param name="align">Выравнивание текста в поле ввода.</param>
        /// <param name="lineType">Тип ввода строк (SingleLine / MultiLineNewline / MultiLineSubmit).</param>
        /// <param name="readOnly">Только для чтения.</param>
        /// <param name="isPassword">Скрывать ли вводимые символы звёздочками.</param>
        /// <param name="autofocus">Автофокус при открытии.</param>
        /// <param name="hudMenuInput">Разрешить ввод в меню HUD.</param>
        /// <param name="placeholderId">ID элемента-плейсхолдера.</param>
        /// <param name="interactable">Кликабельность поля ввода.</param>
        /// <param name="fadeOut">Время исчезновения.</param>
        /// <returns>Имя созданного элемента поля ввода.</returns>
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
            string placeholderId = null,
            bool? interactable = null,
            float fadeOut = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = fadeOut,
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Text = placeholder,
                        FontSize = fontSize,
                        Color = color,
                        Font = font,
                        Align = align,
                        CharsLimit = charLimit,
                        Command = command,
                        LineType = lineType,
                        ReadOnly = readOnly,
                        IsPassword = isPassword,
                        NeedsKeyboard = true, // Обязательно для ввода текста
                        Autofocus = autofocus,
                        HudMenuInput = hudMenuInput,
                        PlaceholderId = placeholderId,
                        Interactable = interactable
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
        #region Countdown (Клиентский таймер)

        /// <summary>
        /// Добавляет встроенный клиентский таймер обратного отсчёта.
        /// Не нагружает сервер тиками timer.Every.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="startTime">Начальное время (в секундах).</param>
        /// <param name="endTime">Конечное время (обычно 0).</param>
        /// <param name="step">Шаг изменения (отрицательный для обратного отсчета, например -1f).</param>
        /// <param name="interval">Интервал обновления отображения (секунды).</param>
        /// <param name="format">Формат таймера (<see cref="TimerFormat"/>).</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="numberFormat">Кастомный формат чисел (при TimerFormat.Custom).</param>
        /// <param name="command">Консольная команда по истечении таймера.</param>
        /// <param name="destroyIfDone">Уничтожить ли элемент автоматически по завершении.</param>
        /// <param name="fadeIn">Время появления.</param>
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
            string numberFormat = "",
            string command = null,
            bool destroyIfDone = true,
            float fadeIn = 0f)
        {
            string name = CuiHelper.GetGuid();
            c.Add(new CuiElement
            {
                Name = name,
                Parent = parent,
                Components =
                {
                    new CuiCountdownComponent
                    {
                        StartTime = startTime,
                        EndTime = endTime,
                        Step = step,
                        Interval = interval,
                        TimerFormat = format,
                        NumberFormat = numberFormat,
                        Command = command,
                        DestroyIfDone = destroyIfDone,
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

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Layout Groups (Автоматическая раскладка / Flexbox)

        /// <summary>
        /// Добавляет панель с горизонтальной авто-раскладкой дочерних элементов (HorizontalLayoutGroup).
        /// Идеально для строк кнопок, иконок инвентаря и панелей навигации.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="name">Имя создаваемого контейнера.</param>
        /// <param name="spacing">Расстояние между дочерними элементами в пикселях.</param>
        /// <param name="alignment">Выравнивание дочерних элементов внутри группы.</param>
        /// <param name="padding">Внутренние отступы "left right top bottom" (например "10 10 5 5").</param>
        /// <param name="controlWidth">Управлять ли шириной дочерних элементов.</param>
        /// <param name="controlHeight">Управлять ли высотой дочерних элементов.</param>
        /// <param name="forceExpandWidth">Принудительно растягивать ширину дочерних элементов.</param>
        /// <param name="forceExpandHeight">Принудительно растягивать высоту дочерних элементов.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <returns>Имя контейнера (использовать как parent для дочерних элементов).</returns>
        public static string HorizontalLayout(
            ref CuiElementContainer c,
            string parent,
            string name,
            float spacing,
            TextAnchor alignment = TextAnchor.MiddleLeft,
            string padding = "0 0 0 0",
            bool controlWidth = true,
            bool controlHeight = true,
            bool forceExpandWidth = false,
            bool forceExpandHeight = true,
            string aMin = "0 0", string aMax = "1 1",
            string oMin = null, string oMax = null)
        {
            string elemName = string.IsNullOrEmpty(name) ? CuiHelper.GetGuid() : name;
            c.Add(new CuiElement
            {
                Name = elemName,
                Parent = parent,
                Components =
                {
                    new CuiHorizontalLayoutGroupComponent
                    {
                        Spacing = spacing,
                        ChildAlignment = alignment,
                        Padding = padding,
                        ChildControlWidth = controlWidth,
                        ChildControlHeight = controlHeight,
                        ChildForceExpandWidth = forceExpandWidth,
                        ChildForceExpandHeight = forceExpandHeight
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
            return elemName;
        }

        /// <summary>
        /// Добавляет панель с вертикальной авто-раскладкой дочерних элементов (VerticalLayoutGroup).
        /// Идеально для списков, меню, таблиц лидеров и диалоговых окон.
        /// </summary>
        /// <inheritdoc cref="HorizontalLayout"/>
        public static string VerticalLayout(
            ref CuiElementContainer c,
            string parent,
            string name,
            float spacing,
            TextAnchor alignment = TextAnchor.UpperCenter,
            string padding = "0 0 0 0",
            bool controlWidth = true,
            bool controlHeight = true,
            bool forceExpandWidth = true,
            bool forceExpandHeight = false,
            string aMin = "0 0", string aMax = "1 1",
            string oMin = null, string oMax = null)
        {
            string elemName = string.IsNullOrEmpty(name) ? CuiHelper.GetGuid() : name;
            c.Add(new CuiElement
            {
                Name = elemName,
                Parent = parent,
                Components =
                {
                    new CuiVerticalLayoutGroupComponent
                    {
                        Spacing = spacing,
                        ChildAlignment = alignment,
                        Padding = padding,
                        ChildControlWidth = controlWidth,
                        ChildControlHeight = controlHeight,
                        ChildForceExpandWidth = forceExpandWidth,
                        ChildForceExpandHeight = forceExpandHeight
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
            return elemName;
        }

        /// <summary>
        /// Добавляет панель с сеточной авто-раскладкой элементов (GridLayoutGroup).
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="name">Имя создаваемого контейнера.</param>
        /// <param name="cellSize">Размер ячейки "Width Height" в пикселях (например "100 100").</param>
        /// <param name="spacing">Промежуток между ячейками "X Y" в пикселях (например "5 5").</param>
        /// <param name="startCorner">Начальный угол заполнения сетки.</param>
        /// <param name="startAxis">Основная ось заполнения (горизонтальная / вертикальная).</param>
        /// <param name="alignment">Выравнивание содержимого ячеек.</param>
        /// <param name="constraint">Ограничение сетки (Flexible / FixedColumnCount / FixedRowCount).</param>
        /// <param name="constraintCount">Количество колонок/строк при соответствующем ограничении.</param>
        /// <param name="padding">Внутренние отступы "left right top bottom".</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <returns>Имя контейнера сетки.</returns>
        public static string GridLayout(
            ref CuiElementContainer c,
            string parent,
            string name,
            string cellSize,
            string spacing,
            GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft,
            GridLayoutGroup.Axis startAxis = GridLayoutGroup.Axis.Horizontal,
            TextAnchor alignment = TextAnchor.UpperLeft,
            GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.FixedColumnCount,
            int constraintCount = 4,
            string padding = "0 0 0 0",
            string aMin = "0 0", string aMax = "1 1",
            string oMin = null, string oMax = null)
        {
            string elemName = string.IsNullOrEmpty(name) ? CuiHelper.GetGuid() : name;
            c.Add(new CuiElement
            {
                Name = elemName,
                Parent = parent,
                Components =
                {
                    new CuiGridLayoutGroupComponent
                    {
                        CellSize = cellSize,
                        Spacing = spacing,
                        StartCorner = startCorner,
                        StartAxis = startAxis,
                        ChildAlignment = alignment,
                        Constraint = constraint,
                        ConstraintCount = constraintCount,
                        Padding = padding
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
            return elemName;
        }

        /// <summary>
        /// Добавляет модификатор размера под содержимое (ContentSizeFitter) к элементу.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя элемента, размер которого нужно подгонять под дочерние элементы.</param>
        /// <param name="horizontalFit">Режим подгонки по горизонтали (Unconstrained / MinSize / PreferredSize).</param>
        /// <param name="verticalFit">Режим подгонки по вертикали (Unconstrained / MinSize / PreferredSize).</param>
        public static void AddContentSizeFitter(
            ref CuiElementContainer c,
            string targetParent,
            ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.PreferredSize,
            ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.PreferredSize)
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiContentSizeFitterComponent
                    {
                        HorizontalFit = horizontalFit,
                        VerticalFit = verticalFit
                    }
                }
            });
        }

        /// <summary>
        /// Добавляет параметры переопределения компоновки (LayoutElement) к дочернему элементу внутри LayoutGroup.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя дочернего элемента внутри LayoutGroup.</param>
        /// <param name="preferredWidth">Предпочитаемая ширина (пиксели).</param>
        /// <param name="preferredHeight">Предпочитаемая высота (пиксели).</param>
        /// <param name="minWidth">Минимальная ширина (пиксели).</param>
        /// <param name="minHeight">Минимальная высота (пиксели).</param>
        /// <param name="flexibleWidth">Гибкий множитель ширины (0..1+).</param>
        /// <param name="flexibleHeight">Гибкий множитель высоты (0..1+).</param>
        /// <param name="ignoreLayout">Игнорировать ли этот элемент при автоматической раскладке родительской группы.</param>
        public static void AddLayoutElement(
            ref CuiElementContainer c,
            string targetParent,
            float preferredWidth = -1f,
            float preferredHeight = -1f,
            float minWidth = -1f,
            float minHeight = -1f,
            float flexibleWidth = -1f,
            float flexibleHeight = -1f,
            bool? ignoreLayout = null)
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiLayoutElementComponent
                    {
                        PreferredWidth = preferredWidth,
                        PreferredHeight = preferredHeight,
                        MinWidth = minWidth,
                        MinHeight = minHeight,
                        FlexibleWidth = flexibleWidth,
                        FlexibleHeight = flexibleHeight,
                        IgnoreLayout = ignoreLayout
                    }
                }
            });
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region CanvasGroup & Mask (Группировка и Маскирование)

        /// <summary>
        /// Добавляет CanvasGroup для управления прозрачностью и кликабельностью всей иерархии дочерних элементов.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя родительского элемента.</param>
        /// <param name="alpha">Общая прозрачность группы (0.0 - 1.0).</param>
        /// <param name="blocksRaycasts">Блокировать ли клики мыши для всей группы.</param>
        /// <param name="interactable">Кликабельны ли элементы группы.</param>
        /// <param name="fade">Скорость/время затухания группы.</param>
        public static void AddCanvasGroup(
            ref CuiElementContainer c,
            string targetParent,
            float? alpha = null,
            bool? blocksRaycasts = null,
            bool? interactable = null,
            string fade = null)
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiCanvasGroupComponent
                    {
                        Alpha = alpha,
                        BlocksRaycasts = blocksRaycasts,
                        Interactable = interactable,
                        Fade = fade
                    }
                }
            });
        }

        /// <summary>
        /// Добавляет маску (Mask), обрезающую всё содержимое дочерних элементов по границам родителя.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя родительского элемента-маски.</param>
        /// <param name="showMaskGraphic">Отрисовывать ли фон самой маски.</param>
        public static void AddMask(
            ref CuiElementContainer c,
            string targetParent,
            bool showMaskGraphic = false)
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiMaskComponent
                    {
                        ShowMaskGraphic = showMaskGraphic
                    }
                }
            });
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Tooltip, Draggable & Slot (Интерактивные механики 2025-2026)

        /// <summary>
        /// Добавляет всплывающую подсказку (Tooltip) к элементу CUI.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя элемента, на который вешается тултип.</param>
        /// <param name="text">Текст подсказки (поддерживает emoji).</param>
        /// <param name="type">Тип тултипа (Default / AlwaysOnTop / AlwaysOnTopEmoji).</param>
        /// <param name="offset">Смещение от курсора/элемента "X Y".</param>
        /// <param name="useCentre">Центрировать ли тултип.</param>
        /// <param name="delay">Задержка перед показом (<see cref="Tooltip.DelayType"/>).</param>
        /// <param name="position">Режим позиционирования (<see cref="TooltipContainer.PositionMode"/>).</param>
        public static void AddTooltip(
            ref CuiElementContainer c,
            string targetParent,
            string text,
            CommunityEntity.TooltipType type = CommunityEntity.TooltipType.Default,
            string offset = "0 10",
            bool useCentre = true,
            Tooltip.DelayType delay = Tooltip.DelayType.Short,
            TooltipContainer.PositionMode position = TooltipContainer.PositionMode.Cursor)
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiTooltipComponent
                    {
                        Text = text,
                        TooltipType = type,
                        Offset = offset,
                        UseCentre = useCentre,
                        Delay = delay,
                        Position = position
                    }
                }
            });
        }

        /// <summary>
        /// Делает элемент перетаскиваемым мышью (Draggable).
        /// Поддерживает хуки OnCuiDraggableDrag и OnCuiDraggableDrop на сервере.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя перетаскиваемого элемента.</param>
        /// <param name="limitToParent">Ограничивать ли перемещение границами родителя.</param>
        /// <param name="maxDistance">Максимальная дистанция перетаскивания (0 = без ограничений).</param>
        /// <param name="allowSwapping">Разрешить ли замену местами со слотом назначения.</param>
        /// <param name="dropAnywhere">Разрешить сброс в любое место экрана.</param>
        /// <param name="dragAlpha">Прозрачность элемента во время перетаскивания (0..1).</param>
        /// <param name="keepOnTop">Отображать ли перетаскиваемый элемент поверх всех остальных.</param>
        /// <param name="positionRpc">Тип отправки координат позиции на сервер через DragRPC.</param>
        /// <param name="filter">Строковый фильтр совместимости для сброса в слот.</param>
        /// <param name="parentPadding">Отступы границ родителя "left right top bottom".</param>
        /// <param name="anchorOffset">Смещение точки привязки "X Y".</param>
        /// <param name="moveToAnchor">Возвращать ли элемент в исходный якорь при отпускании.</param>
        public static void AddDraggable(
            ref CuiElementContainer c,
            string targetParent,
            bool limitToParent = true,
            float maxDistance = 0f,
            bool allowSwapping = false,
            bool dropAnywhere = false,
            float dragAlpha = 0.6f,
            bool keepOnTop = true,
            CommunityEntity.DraggablePositionSendType positionRpc = CommunityEntity.DraggablePositionSendType.None,
            string filter = "",
            string parentPadding = "0 0 0 0",
            string anchorOffset = "0 0",
            bool moveToAnchor = false)
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiDraggableComponent
                    {
                        LimitToParent = limitToParent,
                        MaxDistance = maxDistance,
                        AllowSwapping = allowSwapping,
                        DropAnywhere = dropAnywhere,
                        DragAlpha = dragAlpha,
                        KeepOnTop = keepOnTop,
                        PositionRPC = positionRpc,
                        Filter = filter,
                        ParentPadding = parentPadding,
                        AnchorOffset = anchorOffset,
                        MoveToAnchor = moveToAnchor
                    }
                }
            });
        }

        /// <summary>
        /// Добавляет компонент слота (Slot) для приема перетаскиваемых элементов Draggable.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="targetParent">Имя элемента-слота.</param>
        /// <param name="filter">Строковый фильтр совместимости.</param>
        public static void AddSlot(
            ref CuiElementContainer c,
            string targetParent,
            string filter = "")
        {
            c.Add(new CuiElement
            {
                Parent = targetParent,
                Components =
                {
                    new CuiSlotComponent
                    {
                        Filter = filter
                    }
                }
            });
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region ScrollView (Область прокрутки)

        /// <summary>
        /// Добавляет область скроллинга (ScrollView) с настраиваемыми ползунками и эластичностью.
        /// </summary>
        /// <param name="c">Контейнер элементов.</param>
        /// <param name="parent">Имя родительского элемента.</param>
        /// <param name="name">Уникальное имя ScrollView.</param>
        /// <param name="contentHeight">Высота контентной области в пикселях.</param>
        /// <param name="aMin">AnchorMin "X Y" (0..1).</param>
        /// <param name="aMax">AnchorMax "X Y" (0..1).</param>
        /// <param name="oMin">OffsetMin "X Y" в пикселях. null = не задано.</param>
        /// <param name="oMax">OffsetMax "X Y" в пикселях. null = не задано.</param>
        /// <param name="vertical">Включить ли вертикальный скролл.</param>
        /// <param name="horizontal">Включить ли горизонтальный скролл.</param>
        /// <param name="scrollSensitivity">Чувствительность колеса мыши.</param>
        /// <param name="scrollbarSize">Толщина полосы прокрутки в пикселях.</param>
        /// <param name="scrollbarColor">Цвет ползунка прокрутки "R G B A".</param>
        /// <param name="scrollbarTrackColor">Цвет трека прокрутки "R G B A".</param>
        /// <param name="autoHideScrollbar">Автоматически скрывать полосу прокрутки.</param>
        /// <param name="movementType">Тип движения (Elastic / Unrestricted / Clamped).</param>
        /// <param name="elasticity">Коэффициент эластичности при выходе за границы.</param>
        /// <param name="inertia">Включить ли инерцию прокрутки.</param>
        /// <param name="decelerationRate">Скорость замедления инерции.</param>
        /// <returns>Имя контейнера ScrollView (использовать как parent для элементов списка).</returns>
        public static string ScrollView(
            ref CuiElementContainer c,
            string parent,
            string name,
            float contentHeight,
            string aMin, string aMax,
            string oMin = null, string oMax = null,
            bool vertical = true,
            bool horizontal = false,
            float scrollSensitivity = 30f,
            float scrollbarSize = 4f,
            string scrollbarColor = "0.7 0.7 0.7 0.5",
            string scrollbarTrackColor = "0 0 0 0.2",
            bool autoHideScrollbar = true,
            MovementType movementType = MovementType.Elastic,
            float elasticity = 0.1f,
            bool inertia = true,
            float decelerationRate = 0.135f)
        {
            string elemName = string.IsNullOrEmpty(name) ? CuiHelper.GetGuid() : name;

            c.Add(new CuiElement
            {
                Name = elemName,
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
                        Horizontal = horizontal,
                        Vertical = vertical,
                        MovementType = movementType,
                        Elasticity = elasticity,
                        Inertia = inertia,
                        DecelerationRate = decelerationRate,
                        ScrollSensitivity = scrollSensitivity,
                        VerticalScrollbar = vertical ? new CuiScrollbar
                        {
                            Size = scrollbarSize,
                            HandleColor = scrollbarColor,
                            TrackColor = scrollbarTrackColor,
                            AutoHide = autoHideScrollbar
                        } : null,
                        HorizontalScrollbar = horizontal ? new CuiScrollbar
                        {
                            Size = scrollbarSize,
                            HandleColor = scrollbarColor,
                            TrackColor = scrollbarTrackColor,
                            AutoHide = autoHideScrollbar
                        } : null
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
            return elemName;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Zero-Flicker Updates & State Management

        /// <summary>
        /// Обновляет текст существующего элемента без мерцания (Update = true).
        /// </summary>
        /// <param name="player">Игрок.</param>
        /// <param name="elemName">Имя существующего элемента.</param>
        /// <param name="parent">Имя родительского слоя/элемента (то же, что при создании).</param>
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
                Name = elemName,
                Parent = parent,
                Update = true,
                Components =
                {
                    new CuiTextComponent { Text = newText, FontSize = size, Color = color },
                    new CuiRectTransformComponent { AnchorMin = aMin, AnchorMax = aMax }
                }
            });
            CuiHelper.AddUi(player, c);
        }

        /// <summary>
        /// Показывает или скрывает существующий элемент UI без уничтожения и пересоздания.
        /// </summary>
        /// <param name="player">Игрок.</param>
        /// <param name="elemName">Имя элемента.</param>
        /// <param name="parent">Родитель элемента.</param>
        /// <param name="visible">true = показать (ActiveSelf=true), false = скрыть (ActiveSelf=false).</param>
        public static void SetVisible(
            BasePlayer player,
            string elemName,
            string parent,
            bool visible)
        {
            var c = new CuiElementContainer();
            c.Add(new CuiElement
            {
                Name = elemName,
                Parent = parent,
                Update = true,
                ActiveSelf = visible
            });
            CuiHelper.AddUi(player, c);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Destroy Helpers

        /// <summary>
        /// Уничтожает один UI элемент у игрока.
        /// </summary>
        public static void Destroy(BasePlayer player, string name)
        {
            if (player != null && !string.IsNullOrEmpty(name))
            {
                CuiHelper.DestroyUi(player, name);
            }
        }

        /// <summary>
        /// Уничтожает массив UI элементов одним сетевым RPC пакетом (SendDestroyUIs).
        /// </summary>
        public static void Destroy(BasePlayer player, params string[] names)
        {
            if (player != null && names != null && names.Length > 0)
            {
                CuiHelper.DestroyUi(player, names);
            }
        }

        /// <summary>
        /// Уничтожает список UI элементов одним сетевым RPC пакетом.
        /// </summary>
        public static void Destroy(BasePlayer player, List<string> names)
        {
            if (player != null && names != null && names.Count > 0)
            {
                CuiHelper.DestroyUi(player, names);
            }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Rust 2025-2026 Native Mechanics: Pie (Radial Menu) & Custom Vitals

        /// <summary>
        /// Отправляет игроку кастомное радиальное контекстное меню (Pie Menu) через нативный RPC OpenPie.
        /// Внедрено в Facepunch Rust в июле 2026.
        /// </summary>
        /// <param name="player">Целевой игрок.</param>
        /// <param name="pie">Объект CustomPie с пунктами меню.</param>
        public static void SendPieMenu(BasePlayer player, CustomPie pie)
        {
            if (player == null || pie == null) return;
            CommunityEntity.ServerInstance?.SendPie(player, pie);
        }

        /// <summary>
        /// Создает и конфигурирует элемент пункта радиального меню CustomPieMenu.
        /// </summary>
        /// <param name="pie">Родительский объект CustomPie.</param>
        /// <param name="name">Заголовок пункта меню.</param>
        /// <param name="description">Описание/подсказка пункта.</param>
        /// <param name="command">Консольная команда при выборе.</param>
        /// <param name="sprite">Спрайт иконки (например "assets/icons/facepunch.png").</param>
        /// <param name="disabled">Заблокирован ли пункт меню.</param>
        /// <param name="selected">Выбран ли пункт по умолчанию.</param>
        /// <param name="nextCommand">Команда перехода на следующее подменю. null = нет.</param>
        /// <param name="prevCommand">Команда возврата в предыдущее подменю. null = нет.</param>
        public static void AddPieItem(
            CustomPie pie,
            string name,
            string description,
            string command,
            string sprite,
            bool disabled = false,
            bool selected = false,
            string nextCommand = "",
            string prevCommand = "")
        {
            if (pie == null) return;
            if (pie.menus == null) pie.menus = Facepunch.Pool.Get<List<CustomPieMenu>>();

            CustomPieMenu item = Facepunch.Pool.Get<CustomPieMenu>();
            item.name = name;
            item.description = description;
            item.command = command;
            item.sprite = sprite;
            item.disabled = disabled;
            item.selected = selected;
            item.nextCommand = nextCommand ?? string.Empty;
            item.prevCommand = prevCommand ?? string.Empty;

            pie.menus.Add(item);
        }

        /// <summary>
        /// Отправляет игроку кастомные индикаторы состояния (Custom Vitals / HUD Status Bars).
        /// Внедрено в Facepunch Rust в октябре 2025 (RPC RPC_UpdateVitals).
        /// </summary>
        /// <param name="player">Целевой игрок.</param>
        /// <param name="vitals">Объект с набором кастомных полос состояния.</param>
        public static void SendVitals(BasePlayer player, CustomVitals vitals)
        {
            if (player == null || vitals == null) return;
            CommunityEntity.ServerInstance?.SendCustomVitals(player, vitals);
        }

        /// <summary>
        /// Создает и добавляет кастомный индикатор состояния (Vital Note) в CustomVitals.
        /// </summary>
        /// <param name="vitals">Родительский контейнер CustomVitals.</param>
        /// <param name="leftText">Текст слева (например "RAD SHIELD").</param>
        /// <param name="rightText">Текст справа (например "{timeleft:mm\\:ss}").</param>
        /// <param name="timeLeft">Оставшееся время таймера (в секундах).</param>
        /// <param name="bgColor">Цвет фона полосы.</param>
        /// <param name="iconColor">Цвет иконки.</param>
        /// <param name="leftTextColor">Цвет левого текста.</param>
        /// <param name="rightTextColor">Цвет правого текста.</param>
        /// <param name="icon">Путь к спрайту иконки.</param>
        /// <param name="active">Активен ли индикатор.</param>
        public static void AddVitalInfo(
            CustomVitals vitals,
            string leftText,
            string rightText,
            int timeLeft,
            Color bgColor,
            Color iconColor,
            Color leftTextColor,
            Color rightTextColor,
            string icon = "",
            bool active = true)
        {
            if (vitals == null) return;
            if (vitals.vitals == null) vitals.vitals = Facepunch.Pool.Get<List<CustomVitalInfo>>();

            CustomVitalInfo info = Facepunch.Pool.Get<CustomVitalInfo>();
            info.leftText = leftText;
            info.rightText = rightText;
            info.timeLeft = timeLeft;
            info.backgroundColor = bgColor;
            info.iconColor = iconColor;
            info.leftTextColor = leftTextColor;
            info.rightTextColor = rightTextColor;
            info.icon = icon ?? string.Empty;
            info.active = active;

            vitals.vitals.Add(info);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────
        #region Color & Position Utilities

        /// <summary>
        /// Конвертирует структуру <see cref="UnityEngine.Color"/> в формат CUI строки "R G B A".
        /// </summary>
        public static string ToColor(Color color)
            => $"{color.r:F3} {color.g:F3} {color.b:F3} {color.a:F3}";

        /// <summary>
        /// Конвертирует HEX цвет (например "#cd4632" или "8cc83c") в формат CUI строки "R G B A".
        /// </summary>
        /// <param name="hex">HEX строка цвета.</param>
        /// <param name="alpha">Значение прозрачности от 0.0 до 1.0.</param>
        /// <returns>Форматированная строка "R G B A".</returns>
        public static string HexToColor(string hex, float alpha = 1f)
        {
            if (string.IsNullOrEmpty(hex)) return $"1 1 1 {alpha:F3}";
            hex = hex.TrimStart('#');
            if (hex.Length != 6) return $"1 1 1 {alpha:F3}";

            float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
            float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
            float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
            return $"{r:F3} {g:F3} {b:F3} {alpha:F3}";
        }

        /// <summary>
        /// Вычисляет симметричные смещения OffsetMin / OffsetMax для центрированного элемента фиксированного размера.
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
        /// Вычисляет смещения OffsetMin / OffsetMax для элемента с абсолютным позиционированием в пикселях.
        /// </summary>
        /// <param name="x">Отступ слева.</param>
        /// <param name="y">Отступ снизу.</param>
        /// <param name="width">Ширина элемента.</param>
        /// <param name="height">Высота элемента.</param>
        /// <param name="oMin">Выходное значение OffsetMin.</param>
        /// <param name="oMax">Выходное значение OffsetMax.</param>
        public static void AbsoluteOffset(float x, float y, float width, float height, out string oMin, out string oMax)
        {
            oMin = $"{x} {y}";
            oMax = $"{x + width} {y + height}";
        }

        #endregion
    }
}
