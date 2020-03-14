/// <summary>
/// Перечисление состояний игры
/// </summary>
public enum StateGame : byte
{
    /// <summary>
    /// Показ примера шариков на запоменание
    /// </summary>
    Preparation = 0,
    /// <summary>
    /// Игра начата
    /// </summary>
    Started = 1,
    /// <summary>
    /// Игра окончена
    /// </summary>
    GameOver = 2,
}
