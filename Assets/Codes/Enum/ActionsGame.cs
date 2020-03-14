/// <summary>
/// Перечисление действий с окнами в игре
/// </summary>
enum ActionsGame : byte
{
    /// <summary>
    /// Открываем окно с настройками
    /// </summary>
    OpenSettings = 1,

    /// <summary>
    /// Запускаем обучающий раунд(игру)
    /// </summary>
    TrainingRound = 3,

    /// <summary>
    /// Открываем окно создания своего уровеня
    /// </summary>
    CreateYourLevel = 5,

    /// <summary>
    /// Открываем список простых уровней
    /// </summary>
    ListSimpleLevels = 10,

    /// <summary>
    /// Открываем список уровней на время
    /// </summary>
    ListLevelsTime = 11,

    /// <summary>
    /// Запускаем игру со своими настройками
    /// </summary>
    StartGameYourSettings = 12,

    /// <summary>
    /// Запускаем игру
    /// </summary>
    StartGame = 13
}
