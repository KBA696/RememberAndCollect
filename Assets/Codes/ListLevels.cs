using System;

/// <summary>
/// Настройки игры
/// </summary>
[Serializable]
public class SettingsGame
{
    /// <summary>
    /// Количество шариков для заполнения
    /// </summary>
    public const byte numberLevels = 65;

    /// <summary>
    /// Состояние уровня игры без времени
    /// </summary>
    public bool[] StateLevelNormal = new bool[numberLevels]
    {
        false ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true
    };

    /// <summary>
    /// Состояние уровня игры со временем
    /// </summary>
    public bool[] StateLevelTime = new bool[numberLevels]
    {
        false ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true ,
        true
    };

    /// <summary>
    /// Проигрвапть фоновую музыку
    /// </summary>
    public bool PlayBackground = true;

    /// <summary>
    /// Громкость фоновой музыки (max 1f)
    /// </summary>
    public float VolumeBackground = 0.75f;

    /// <summary>
    /// Воспроизведение эффектов
    /// </summary>
    public bool PlayEffect = true;

    /// <summary>
    /// Громкость эффектов (max 1f)
    /// </summary>
    public float VolumeEffect = 1f;
}

