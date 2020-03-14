/// <summary>
/// Параметры игровой сессии
/// </summary>
public class ParametersGame
{
    /// <summary>
    /// В каком состоянии находится игра
    /// </summary>
    public StateGame StateGame = 0;

    /// <summary>
    /// Количество запоминаемых шариков
    /// </summary>
    public int TotalBalls = 0;

    /// <summary>
    /// Этапы обучения (max 5)
    /// </summary>
    public int StageTraining = 0;

    /// <summary>
    /// Уровень текущей игры
    /// </summary>
    public int GameLevel = -1;

    /// <summary>
    /// Показывать таймер. true-запущен уровень на время, false - запущен обычный уровень 
    /// </summary>
    public bool ShowTimer;

    /// <summary>
    /// trye - игра создана игроком, false - стандартные уровни от разработчика
    /// </summary>
    public bool MyGame;
}