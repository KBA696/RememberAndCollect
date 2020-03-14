using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCode : MonoBehaviour
{
    #region Переменные
    /// <summary>
    /// Хронит историю действий сделанных в игре
    /// </summary>
    Stack<ActionsGame> actionHistory = new Stack<ActionsGame>();

    /// <summary>
    /// Настройки игры
    /// </summary>
    SettingsGame listLevels;

    /// <summary>
    /// Параметры игровой сесии
    /// </summary>
    ParametersGame parametersGame = new ParametersGame();

    /// <summary>
    /// Кнопка назад
    /// </summary>
    public GameObject BackButton;

    /// <summary>
    /// Анимация перемещения окошек
    /// </summary>
    Animator animator;

    /// <summary>
    /// Количество шариков для заполнения
    /// </summary>
    const byte numberBalls = 11;

    /// <summary>
    /// Количество шариков для заполнения
    /// </summary>
    const byte numberColors = 11;

    /// <summary>
    /// Оброзец шариков которые надо выложить
    /// </summary>
    public GameObject[] Samples = new GameObject[numberBalls];

    /// <summary>
    /// Выкладываемые шарики
    /// </summary>
    public GameObject[] WorkingBalls = new GameObject[numberBalls];

    /// <summary>
    /// Образцы шариков
    /// </summary>
    public GameObject[] SamplesBalls = new GameObject[numberColors];

    /// <summary>
    /// Контрол куда надо добавить уровни
    /// </summary>
    public Transform ControlForLevels;

    /// <summary>
    /// Префаб кнопки с уровнем
    /// </summary>
    public GameObject PlayerPrefs;

    [Header("Сообщения")]
    /// <summary>
    /// Сообщение обучения 1 задача
    /// </summary>
    public GameObject Training1;

    /// <summary>
    /// Сообщение обучения 2 задача
    /// </summary>
    public GameObject Training2;

    /// <summary>
    /// Сообщение обучения 3 задача
    /// </summary>
    public GameObject Training3;

    /// <summary>
    /// Сообщение обучения 4 задача
    /// </summary>
    public GameObject Training4;

    /// <summary>
    /// Сообщение о победе
    /// </summary>
    public GameObject MessageVictory;

    /// <summary>
    /// Сообщение о проигрыше
    /// </summary>
    public GameObject MessageLosing;

    [Header("Звук")]
    /// <summary>
    /// Кнопка вкл/выкл фоновой музыки
    /// </summary>
    public Button BackgroundMusic;

    /// <summary>
    /// Фоновая музыка
    /// </summary>
    public AudioSource SourceBackgroundMusic;

    /// <summary>
    /// Громкость фоновой музыки
    /// </summary>
    public Slider VolumeBackgroundMusic;

    /// <summary>
    /// Кнопка вкл/выкл кликов
    /// </summary>
    public Button Effect;

    /// <summary>
    /// Звук клика
    /// </summary>
    public AudioClip SourceEffect;

    /// <summary>
    /// Громкость кликов
    /// </summary>
    public Slider VolumeEffect;

    /// <summary>
    /// Текуший источник звука
    /// </summary>
    AudioSource audioSource;

    [Header("Индикатор таймера")]
    /// <summary>
    /// Слайдер таймера игры
    /// </summary>
    public Slider GameTimer;

    /// <summary>
    /// Cколько прошло времени
    /// </summary>
    float muchTimePassed = 60;

    /// <summary>
    /// Время на игру
    /// </summary>
    float timePlay;

    /// <summary>
    /// Время на запоминание
    /// </summary>
    float timeRemember;

    [Header("Поля с настройками для своей игры")]
    /// <summary>
    /// Комбокс для выбора количества шариков в своей игре
    /// </summary>
    public Dropdown SettingsTotalBalls;

    /// <summary>
    /// Комбокс для выбора количества цветов шариков в своей игре
    /// </summary>
    public Dropdown SettingsTotalColors;

    /// <summary>
    /// Текстовое поле для задания времени на запоминание в своей игре
    /// </summary>
    public InputField SettingsTimeRemember;

    /// <summary>
    /// Текстовое поле для задания времени на игру в своей игре
    /// </summary>
    public InputField SettingsTimePlay;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //UnityEngine.PlayerPrefs.DeleteAll();//Обнуляю настройки

        //Загружаем из PlayerPrefs сохраненную игру или присваиваем стандартные настройки
        if (UnityEngine.PlayerPrefs.HasKey("Save"))
        {
            listLevels = JsonUtility.FromJson<SettingsGame>(UnityEngine.PlayerPrefs.GetString("Save"));
        }
        else
        {
            listLevels = new SettingsGame();
        }

        //Задаем громкость фоновой музыки скролу из настроек и плэеру
        VolumeBackgroundMusic.value = SourceBackgroundMusic.volume = listLevels.VolumeBackground;

        //Запускаем фоновую музыку если надо
        PlayBackgroundMusic(listLevels.PlayBackground);

        //Задаем громкость кликам
        VolumeEffect.value = listLevels.VolumeEffect;

        //Задаем правильную картинку для колонки под клик в настройках
        IconCleekdMusic(listLevels.PlayEffect);

        //Ищим компонент AudioSource на текущем обьекте
        audioSource = GetComponent<AudioSource>();

        //Ищим компонент Animator на текущем обьекте
        animator = GetComponent<Animator>();

        //добовляем в историю что мы открыли главное окно
        actionHistory.Push(0);

        //Добовляем кнопки уровней
        for (int i = 0; i < SettingsGame.numberLevels; i++)
        {
            var createdPlayerPrefs = Instantiate(PlayerPrefs, ControlForLevels);

            createdPlayerPrefs.name = i.ToString();
            foreach (Transform child in createdPlayerPrefs.transform)
            {
                if (child.name == "Text")
                {
                    //Присваиваем текст кнопке который равен её уровню
                    child.GetComponent<Text>().text = (i + 1).ToString();
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN
        //Выходим из игры в Windows по нажатию Esc 
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
#endif

        //Отлавливаем изменения в настройках для громкости фоновой музыки и кликов
        var vv = listLevels.VolumeBackground != VolumeBackgroundMusic.value;
        if (vv)
        {
            SourceBackgroundMusic.volume = listLevels.VolumeBackground = VolumeBackgroundMusic.value;
            //Останавливаем воспроимзведение и меняем картинку если громкость меньше 0.02 либо запускаем фоновую музыку
            if (listLevels.VolumeBackground <= 0.02f) { PlayBackgroundMusic(false); } else { PlayBackgroundMusic(); }
        }
        var bb = listLevels.VolumeEffect != VolumeEffect.value;
        if (bb)
        {
            listLevels.VolumeEffect = VolumeEffect.value;
            if (listLevels.VolumeEffect <= 0.02f) { IconCleekdMusic(false); } else { IconCleekdMusic(); }

        }
        //Сохраняем настройки если произошли изменения
        if (vv || bb) { SaveSeting(); }

        //Задаём значения таймеру сколько осталось времени на запоминание или на игру
        if (parametersGame.ShowTimer)
        {
            if (parametersGame.StateGame == 0 && timeRemember > 0)
            {
                muchTimePassed += 1 * Time.deltaTime;
                if (timeRemember <= muchTimePassed)
                {
                    CloseBoard();
                }
            }
            else if (parametersGame.StateGame == StateGame.Started && timePlay > 0)
            {
                muchTimePassed -= 1 * Time.deltaTime;
                if (muchTimePassed <= 0)
                {
                    GameOwer();
                }
            }

            GameTimer.value = muchTimePassed;
        }
    }

    #region Игровой процес
    /// <summary>
    /// Обучающий раунд(игра)
    /// </summary>
    public void Training() { transitionsBetweenWindows(ActionsGame.TrainingRound); }

    /// <summary>
    /// Окно с перечнем простых уровней
    /// </summary>
    public void OpennNormalLevels() { transitionsBetweenWindows(ActionsGame.ListSimpleLevels); }

    /// <summary>
    /// Окно с перечнем уровней на время
    /// </summary>
    public void OpennTimLevels() { transitionsBetweenWindows(ActionsGame.ListLevelsTime); }

    /// <summary>
    /// Окно создания своего уровеня
    /// </summary>
    public void CreateYourLevel() { transitionsBetweenWindows(ActionsGame.CreateYourLevel); }

    /// <summary>
    /// Запустить игру со своими настройками
    /// </summary>
    public void MyGame() { transitionsBetweenWindows(ActionsGame.StartGameYourSettings); }

    /// <summary>
    /// Переходы между окнами
    /// </summary>
    /// <param name="actionsGame">Какое надо выполнить действие</param>
    void transitionsBetweenWindows(ActionsGame actionsGame)
    {
        PlayClick();

        int i = (int)actionsGame;//Присваиваем номер анимации если не совпал ниже в switch исправляем на нужный номер

        switch (actionsGame)
        {
            case ActionsGame.StartGame:
                MessageVictory.SetActive(false);
                MessageLosing.SetActive(false);

                #region Задаем параметры игре
                if (parametersGame.MyGame)
                {
                    int.TryParse(SettingsTimeRemember.text, out int settingsTimeRemember);
                    int.TryParse(SettingsTimePlay.text, out int settingsTimePlay);

                    if (settingsTimeRemember > 0 || settingsTimePlay > 0) { parametersGame.ShowTimer = true; }

                    DecomposeBalls(SettingsTotalBalls.value + 2, SettingsTotalColors.value + 2, false, settingsTimeRemember, settingsTimePlay);
                }
                else if (parametersGame.ShowTimer)
                {
                    switch (parametersGame.GameLevel + 1)
                    {
                        case 1:
                            DecomposeBalls(5, 2, false, 25, 45);
                            break;
                        case 2:
                            DecomposeBalls(6, 2, false, 25, 45);
                            break;
                        case 3:
                            DecomposeBalls(7, 2, false, 25, 45);
                            break;
                        case 4:
                            DecomposeBalls(8, 2, false, 25, 45);
                            break;
                        case 5:
                            DecomposeBalls(5, 3, false, 25, 45);
                            break;
                        case 6:
                            DecomposeBalls(6, 3, false, 25, 45);
                            break;
                        case 7:
                            DecomposeBalls(7, 3, false, 25, 45);
                            break;
                        case 8:
                            DecomposeBalls(8, 3, false, 25, 45);
                            break;
                        case 9:
                            DecomposeBalls(9, 3, false, 25, 45);
                            break;
                        case 10:
                            DecomposeBalls(5, 4, false, 25, 45);
                            break;
                        case 11:
                            DecomposeBalls(6, 4, false, 25, 45);
                            break;
                        case 12:
                            DecomposeBalls(7, 4, false, 25, 45);
                            break;
                        case 13:
                            DecomposeBalls(8, 4, false, 25, 45);
                            break;
                        case 14:
                            DecomposeBalls(9, 4, false, 25, 45);
                            break;
                        case 15:
                            DecomposeBalls(10, 4, false, 25, 45);
                            break;
                        case 16:
                            DecomposeBalls(5, 5, false, 25, 45);
                            break;
                        case 17:
                            DecomposeBalls(6, 5, false, 25, 45);
                            break;
                        case 18:
                            DecomposeBalls(7, 5, false, 25, 45);
                            break;
                        case 19:
                            DecomposeBalls(8, 5, false, 25, 45);
                            break;
                        case 20:
                            DecomposeBalls(9, 5, false, 25, 45);
                            break;
                        case 21:
                            DecomposeBalls(10, 5, false, 25, 45);
                            break;
                        case 22:
                            DecomposeBalls(11, 5, false, 25, 45);
                            break;
                        case 23:
                            DecomposeBalls(5, 6, false, 25, 45);
                            break;
                        case 24:
                            DecomposeBalls(6, 6, false, 25, 45);
                            break;
                        case 25:
                            DecomposeBalls(7, 6, false, 25, 45);
                            break;
                        case 26:
                            DecomposeBalls(8, 6, false, 25, 45);
                            break;
                        case 27:
                            DecomposeBalls(9, 6, false, 25, 45);
                            break;
                        case 28:
                            DecomposeBalls(10, 6, false, 25, 45);
                            break;
                        case 29:
                            DecomposeBalls(11, 6, false, 25, 45);
                            break;
                        case 30:
                            DecomposeBalls(6, 7, false, 25, 45);
                            break;
                        case 31:
                            DecomposeBalls(7, 7, false, 25, 45);
                            break;
                        case 32:
                            DecomposeBalls(8, 7, false, 25, 45);
                            break;
                        case 33:
                            DecomposeBalls(9, 7, false, 25, 45);
                            break;
                        case 34:
                            DecomposeBalls(10, 7, false, 25, 45);
                            break;
                        case 35:
                            DecomposeBalls(11, 7, false, 25, 45);
                            break;
                        case 36:
                            DecomposeBalls(6, 8, false, 25, 45);
                            break;
                        case 37:
                            DecomposeBalls(7, 8, false, 25, 45);
                            break;
                        case 38:
                            DecomposeBalls(8, 8, false, 25, 45);
                            break;
                        case 39:
                            DecomposeBalls(9, 8, false, 25, 45);
                            break;
                        case 40:
                            DecomposeBalls(10, 8, false, 25, 45);
                            break;
                        case 41:
                            DecomposeBalls(11, 8, false, 25, 45);
                            break;
                        case 42:
                            DecomposeBalls(6, 9, false, 25, 45);
                            break;
                        case 43:
                            DecomposeBalls(7, 9, false, 25, 45);
                            break;
                        case 44:
                            DecomposeBalls(8, 9, false, 25, 45);
                            break;
                        case 45:
                            DecomposeBalls(9, 9, false, 25, 45);
                            break;
                        case 46:
                            DecomposeBalls(10, 9, false, 25, 45);
                            break;
                        case 47:
                            DecomposeBalls(11, 9, false, 25, 45);
                            break;
                        case 48:
                            DecomposeBalls(6, 10, false, 25, 45);
                            break;
                        case 49:
                            DecomposeBalls(7, 10, false, 25, 45);
                            break;
                        case 50:
                            DecomposeBalls(8, 10, false, 25, 45);
                            break;
                        case 51:
                            DecomposeBalls(9, 10, false, 25, 45);
                            break;
                        case 52:
                            DecomposeBalls(10, 10, false, 25, 45);
                            break;
                        case 53:
                            DecomposeBalls(11, 10, false, 25, 45);
                            break;
                        case 54:
                            DecomposeBalls(6, 10, false, 25, 45);
                            break;
                        case 55:
                            DecomposeBalls(7, 10, false, 25, 45);
                            break;
                        case 56:
                            DecomposeBalls(8, 10, false, 25, 45);
                            break;
                        case 57:
                            DecomposeBalls(9, 10, false, 25, 45);
                            break;
                        case 58:
                            DecomposeBalls(10, 10, false, 25, 45);
                            break;
                        case 59:
                            DecomposeBalls(11, 10, false, 25, 45);
                            break;
                        case 60:
                            DecomposeBalls(6, 11, false, 25, 45);
                            break;
                        case 61:
                            DecomposeBalls(7, 11, false, 25, 45);
                            break;
                        case 62:
                            DecomposeBalls(8, 11, false, 25, 45);
                            break;
                        case 63:
                            DecomposeBalls(9, 11, false, 25, 45);
                            break;
                        case 64:
                            DecomposeBalls(10, 11, false, 25, 45);
                            break;
                        case 65:
                            DecomposeBalls(11, 11, false, 25, 45);
                            break;
                        default:
                            DecomposeBalls(11, 11, false, 25, 45);
                            break;
                    }
                }
                else
                {
                    switch (parametersGame.GameLevel + 1)
                    {
                        case 1:
                            DecomposeBalls(5, 2);
                            break;
                        case 2:
                            DecomposeBalls(6, 2);
                            break;
                        case 3:
                            DecomposeBalls(7, 2);
                            break;
                        case 4:
                            DecomposeBalls(8, 2);
                            break;
                        case 5:
                            DecomposeBalls(5, 3);
                            break;
                        case 6:
                            DecomposeBalls(6, 3);
                            break;
                        case 7:
                            DecomposeBalls(7, 3);
                            break;
                        case 8:
                            DecomposeBalls(8, 3);
                            break;
                        case 9:
                            DecomposeBalls(9, 3);
                            break;
                        case 10:
                            DecomposeBalls(5, 4);
                            break;
                        case 11:
                            DecomposeBalls(6, 4);
                            break;
                        case 12:
                            DecomposeBalls(7, 4);
                            break;
                        case 13:
                            DecomposeBalls(8, 4);
                            break;
                        case 14:
                            DecomposeBalls(9, 4);
                            break;
                        case 15:
                            DecomposeBalls(10, 4);
                            break;
                        case 16:
                            DecomposeBalls(5, 5);
                            break;
                        case 17:
                            DecomposeBalls(6, 5);
                            break;
                        case 18:
                            DecomposeBalls(7, 5);
                            break;
                        case 19:
                            DecomposeBalls(8, 5);
                            break;
                        case 20:
                            DecomposeBalls(9, 5);
                            break;
                        case 21:
                            DecomposeBalls(10, 5);
                            break;
                        case 22:
                            DecomposeBalls(11, 5);
                            break;
                        case 23:
                            DecomposeBalls(5, 6);
                            break;
                        case 24:
                            DecomposeBalls(6, 6);
                            break;
                        case 25:
                            DecomposeBalls(7, 6);
                            break;
                        case 26:
                            DecomposeBalls(8, 6);
                            break;
                        case 27:
                            DecomposeBalls(9, 6);
                            break;
                        case 28:
                            DecomposeBalls(10, 6);
                            break;
                        case 29:
                            DecomposeBalls(11, 6);
                            break;
                        case 30:
                            DecomposeBalls(6, 7);
                            break;
                        case 31:
                            DecomposeBalls(7, 7);
                            break;
                        case 32:
                            DecomposeBalls(8, 7);
                            break;
                        case 33:
                            DecomposeBalls(9, 7);
                            break;
                        case 34:
                            DecomposeBalls(10, 7);
                            break;
                        case 35:
                            DecomposeBalls(11, 7);
                            break;
                        case 36:
                            DecomposeBalls(6, 8);
                            break;
                        case 37:
                            DecomposeBalls(7, 8);
                            break;
                        case 38:
                            DecomposeBalls(8, 8);
                            break;
                        case 39:
                            DecomposeBalls(9, 8);
                            break;
                        case 40:
                            DecomposeBalls(10, 8);
                            break;
                        case 41:
                            DecomposeBalls(11, 8);
                            break;
                        case 42:
                            DecomposeBalls(6, 9);
                            break;
                        case 43:
                            DecomposeBalls(7, 9);
                            break;
                        case 44:
                            DecomposeBalls(8, 9);
                            break;
                        case 45:
                            DecomposeBalls(9, 9);
                            break;
                        case 46:
                            DecomposeBalls(10, 9);
                            break;
                        case 47:
                            DecomposeBalls(11, 9);
                            break;
                        case 48:
                            DecomposeBalls(6, 10);
                            break;
                        case 49:
                            DecomposeBalls(7, 10);
                            break;
                        case 50:
                            DecomposeBalls(8, 10);
                            break;
                        case 51:
                            DecomposeBalls(9, 10);
                            break;
                        case 52:
                            DecomposeBalls(10, 10);
                            break;
                        case 53:
                            DecomposeBalls(11, 10);
                            break;
                        case 54:
                            DecomposeBalls(6, 10);
                            break;
                        case 55:
                            DecomposeBalls(7, 10);
                            break;
                        case 56:
                            DecomposeBalls(8, 10);
                            break;
                        case 57:
                            DecomposeBalls(9, 10);
                            break;
                        case 58:
                            DecomposeBalls(10, 10);
                            break;
                        case 59:
                            DecomposeBalls(11, 10);
                            break;
                        case 60:
                            DecomposeBalls(6, 11);
                            break;
                        case 61:
                            DecomposeBalls(7, 11);
                            break;
                        case 62:
                            DecomposeBalls(8, 11);
                            break;
                        case 63:
                            DecomposeBalls(9, 11);
                            break;
                        case 64:
                            DecomposeBalls(10, 11);
                            break;
                        case 65:
                            DecomposeBalls(11, 11);
                            break;
                        default:
                            DecomposeBalls(11, 11);
                            break;
                    }
                }
                #endregion

                i = 3;
                break;

            case ActionsGame.StartGameYourSettings:
                i = -1;//ни какую анимацию не запускать
                break;

            case ActionsGame.ListLevelsTime:
            case ActionsGame.ListSimpleLevels:
                HideAll();

                parametersGame.MyGame = false;

                foreach (Transform child in ControlForLevels.transform)
                {
                    bool lockedLevel = false;
                    foreach (Transform childLevel in child.transform)
                    {
                        foreach (var item in childLevel.GetComponentsInChildren<Image>())
                        {
                            if (actionsGame == ActionsGame.ListSimpleLevels)
                            {
                                lockedLevel = listLevels.StateLevelNormal[int.Parse(child.name)];
                            }
                            else if (actionsGame == ActionsGame.ListLevelsTime)
                            {
                                lockedLevel = listLevels.StateLevelTime[int.Parse(child.name)];
                            }
                            
                            childLevel.gameObject.SetActive(lockedLevel);
                        }
                    }

                    child.GetComponent<Button>().onClick.RemoveAllListeners();

                    if (actionsGame==ActionsGame.ListSimpleLevels)
                    {
                        child.GetComponent<Button>().onClick.AddListener(() => { if (!lockedLevel) { parametersGame.GameLevel = int.Parse(child.name); parametersGame.ShowTimer = false; transitionsBetweenWindows(ActionsGame.StartGame); } });
                    }
                    else if (actionsGame == ActionsGame.ListLevelsTime)
                    {
                        child.GetComponent<Button>().onClick.AddListener(() => { if (!lockedLevel) { parametersGame.GameLevel = int.Parse(child.name); parametersGame.ShowTimer = true; transitionsBetweenWindows(ActionsGame.StartGame); } });
                    }
                }

                i = 2;
                break;

            case ActionsGame.TrainingRound:
                DecomposeBalls(3, 3, true);
                break;
            case ActionsGame.CreateYourLevel:
            case ActionsGame.OpenSettings:
            case 0:
                HideAll();
                break;
        }

        if (i > -1) { animator.SetInteger(nameof(ActionsGame), i); }

        //сохраняем только один экземпляр окна чтобы после 3 раундов(уровней) игр не жать 3 раза назад 
        if (actionHistory.Peek() != actionsGame && !(actionHistory.Peek() == ActionsGame.StartGameYourSettings && ActionsGame.StartGame == actionsGame)) { actionHistory.Push(actionsGame); }

        //Включаем кнопку назад
        BackButton.SetActive(true);

        switch (actionsGame)
        {
            case ActionsGame.StartGame:
                if (parametersGame.ShowTimer)
                {
                    GameTimer.gameObject.SetActive(true);
                    muchTimePassed = 0;
                }

                parametersGame.StateGame = StateGame.Preparation;
                parametersGame.StageTraining = 0;

                break;
            case ActionsGame.TrainingRound:
                parametersGame.StateGame = StateGame.Preparation;
                parametersGame.StageTraining = 1;
                break;

            case ActionsGame.StartGameYourSettings:
                parametersGame.MyGame = true;
                parametersGame.ShowTimer = false;
                parametersGame.GameLevel = 0;
                transitionsBetweenWindows(ActionsGame.StartGame);
                i = -1;
                break;
        }
    }

    /// <summary>
    /// Подготовка шариков которые надо запомнить
    /// </summary>
    /// <param name="totalBalls">Количество шариков (max 11)</param>
    /// <param name="totalColors">количество цветов (max 11)</param>
    /// <param name="training">true - обучение, false - остальные уровни</param>
    /// <param name="timeRemember">Время на запоминание</param>
    /// <param name="timePlay">Время на игру</param>
    void DecomposeBalls(int totalBalls, int totalColors, bool training = false, float timeRemember = 0, float timePlay = 0)
    {
        this.timeRemember = GameTimer.maxValue = timeRemember;
        this.timePlay = timePlay;

        //Количество цветов если меньше 1 или больше 11 меняю на 11
        if (totalColors > numberColors || totalColors < 1) { totalColors = numberColors; }

        //Количество шариков если меньше 1 или больше 11 меняю на 11
        if (totalBalls > numberBalls || totalBalls < 1) { totalColors = numberBalls; }
        parametersGame.TotalBalls = totalBalls;

        //Раскладываем шарики которые надо запомнить, для обучение по шаблону, а для остальных уровней рандомно
        for (int i = 0; i < numberBalls; i++)
        {
            if (training)
            {
                switch (i)
                {
                    case 0:
                        Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/1");
                        Training1.SetActive(true);
                        break;
                    case 1:
                        Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/2");
                        break;
                    case 2:
                        Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/1");
                        break;
                    default:
                        Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
                        break;
                }
            }
            else
            {
                if (i >= totalBalls)//Оставшиеся заполняем ямками под шарик
                {
                    Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
                }
                else
                {
                    Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/" + Random.Range(1, totalColors + 1));
                }
            }
            Samples[i].SetActive(true);
        }

        //Очищаю рабочую облость
        foreach (var item in WorkingBalls)
        {
            item.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
            item.SetActive(true);
        }

        //Скрываем те цвета шариков которых не будет
        for (int i = 0; i < numberColors; i++)
        {
            SamplesBalls[i].SetActive((i < totalColors ? true : false));
        }
    }

    /// <summary>
    /// Закрыть шарики доской
    /// </summary>
    void CloseBoard()
    {
        //Миняем статус игры
        parametersGame.StateGame = StateGame.Started;
        
        //Если игра на время то задаем значение через сколько раунд закончится
        if (parametersGame.ShowTimer)
        {
            muchTimePassed = GameTimer.maxValue = timePlay;
        }
        
        //Запускаем анимацию закрытия крышки
        animator.SetInteger(nameof(ActionsGame), 4);
    }

    /// <summary>
    /// Перемещаем шарик
    /// </summary>
    /// <param name="gameObject">шарик который надо переместить</param>
    public void MoveBall(GameObject gameObject)
    {
        //если игра еще не запустилась после нажатия запускаем её
        if (parametersGame.StateGame == StateGame.Preparation) { CloseBoard(); }

        //Дальше обрабатываем только если игра начата
        if (parametersGame.StateGame == StateGame.Started)
        {
            PlayClick();

            if (parametersGame.StageTraining <= 0)
            {
                //Пробегаем по всем ячейкам и в первую пустую ячейку ложим шарик, если все заполнены то завершаем игру
                for (int i = 0; i < parametersGame.TotalBalls; i++)
                {
                    if (WorkingBalls[i].GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/0"))
                    {
                        WorkingBalls[i].GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;

                        if (i == parametersGame.TotalBalls - 1) { GameOwer(); }

                        break;
                    }
                }
            }//Дальше проверки на обучающие уровни
            else if (parametersGame.StageTraining == 1 && WorkingBalls[0].GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/0") && gameObject.GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/1"))
            {
                Training1.SetActive(false);
                WorkingBalls[0].GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
                parametersGame.StageTraining = 2;
                Training2.SetActive(true);
            }
            else if (parametersGame.StageTraining == 2 && WorkingBalls[1].GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/0") && gameObject.GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/3"))
            {
                Training2.SetActive(false);
                WorkingBalls[1].GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
                parametersGame.StageTraining = 3;
                Training3.SetActive(true);
            }
            else if (parametersGame.StageTraining == 4 && WorkingBalls[2].GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/0") && gameObject.GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/2"))
            {
                Training4.SetActive(false);
                WorkingBalls[1].GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
                parametersGame.StageTraining = 5;
                Training1.SetActive(true);
            }
            else if (parametersGame.StageTraining == 5 && WorkingBalls[3].GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/0") && gameObject.GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/1"))
            {
                Training1.SetActive(false);
                WorkingBalls[2].GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
                GameOwer();
            }
        }
    }

    /// <summary>
    /// Убераем шарик который выложили по ошибке
    /// </summary>
    /// <param name="gameObject">Шарик который надо удалить</param>
    public void RemoveBall(GameObject gameObject)
    {
        if (parametersGame.StateGame == StateGame.Started)
        {
            PlayClick();

            if (parametersGame.StageTraining == 0)
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
            }
            else if (parametersGame.StageTraining == 3)//Во время обучения удоляем второй шарик
            {
                Training3.SetActive(false);
                WorkingBalls[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
                parametersGame.StageTraining = 4;
                Training4.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Заканчиваем игру и вычисляем победа или проигрыш
    /// </summary>
    void GameOwer()
    {
        bool victory = true;

        //Вычесляем победил или проиграл
        for (int i = 0; i < parametersGame.TotalBalls; i++)
        {
            if (Samples[i].GetComponent<Image>().sprite != WorkingBalls[i].GetComponent<Image>().sprite)
            {
                victory = false;
                break;
            }
        }

        //меняем статус игры
        parametersGame.StateGame = StateGame.GameOver;

        //запускаем анимацию открытия доски
        animator.SetInteger(nameof(ActionsGame), 3);

        //Оповещаем о победе или о проигрыше
        if (victory)
        {
            //Если уровень не игроком созданый то открываем следующий
            if (!parametersGame.MyGame)
            {
                if (parametersGame.ShowTimer)//Уровень на время
                {
                    if (listLevels.StateLevelTime.Length > parametersGame.GameLevel+1)
                    {
                        listLevels.StateLevelTime[parametersGame.GameLevel + 1] = false;
                    }
                }
                else if (!parametersGame.ShowTimer)//обычный уровень
                {
                    if (listLevels.StateLevelNormal.Length > parametersGame.GameLevel+1)
                    {
                        listLevels.StateLevelNormal[parametersGame.GameLevel + 1] = false;
                    }
                }

                SaveSeting();
            }

            foreach (Transform child in MessageVictory.transform)
            {
                var button = child.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    if (parametersGame.StageTraining > 0)//После обучения переходим в окно с обычными уровнями
                    {
                        button.onClick.AddListener(() => { actionHistory.Pop(); transitionsBetweenWindows(ActionsGame.ListSimpleLevels); });
                    }
                    else
                    {
                        button.onClick.AddListener(() => { parametersGame.GameLevel = parametersGame.GameLevel + 1; transitionsBetweenWindows(ActionsGame.StartGame); });
                    }
                }
            }

            MessageVictory.SetActive(true);
        }
        else
        {
            foreach (Transform child in MessageLosing.transform)
            {
                var button = child.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => { transitionsBetweenWindows(ActionsGame.StartGame); });
                }

            }

            MessageLosing.SetActive(true);
        }
    }

    /// <summary>
    /// Скрыть все что связано с игрой
    /// </summary>
    void HideAll()
    {
        parametersGame.StateGame = StateGame.GameOver;

        foreach (var item in Samples) { item.SetActive(false); }

        foreach (var item in WorkingBalls) { item.SetActive(false); }

        foreach (var item in SamplesBalls) { item.SetActive(false); }

        MessageVictory.SetActive(false);

        MessageLosing.SetActive(false);

        GameTimer.gameObject.SetActive(false);

        Training1.SetActive(false);
        Training2.SetActive(false);
        Training3.SetActive(false);
        Training4.SetActive(false);
    }
    #endregion


    #region Настройки игры
    //Открываем окно настроек
    public void OpenSettings()
    {
        //Можно было бы скрыть кнопку настроек. Но лень
        if (actionHistory.Peek() != ActionsGame.OpenSettings) { transitionsBetweenWindows(ActionsGame.OpenSettings); }
    }

    /// <summary>
    /// Изменить состояние фоновой музыкм на воспроизведение или пауза в зависимости от текущего состояния
    /// </summary>
    public void ChangeStateBackgroundMusic()
    {
        PlayBackgroundMusic(!listLevels.PlayBackground);

        PlayClick();

        SaveSeting();
    }

    /// <summary>
    /// Воспроизводить фоновую музыку
    /// </summary>
    /// <param name="turn"> true - воспроизвести фоновую музыку, false - остановить фоновую музыку</param>
    void PlayBackgroundMusic(bool turn = true)
    {
        if (turn && !SourceBackgroundMusic.isPlaying)
        {
            SourceBackgroundMusic.Play();
            BackgroundMusic.image.sprite = Resources.Load<Sprite>("Image/PlayMusic");
        }
        else if (!turn && SourceBackgroundMusic.isPlaying)
        {
            SourceBackgroundMusic.Pause();
            BackgroundMusic.image.sprite = Resources.Load<Sprite>("Image/StopMusic");
        }

        listLevels.PlayBackground = turn;
    }


    /// <summary>
    /// Воспроизводим звук нажатия(клика)
    /// </summary>
    void PlayClick()
    {
        if (listLevels.PlayEffect)
        {
            audioSource.volume = listLevels.VolumeEffect;
            audioSource.PlayOneShot(SourceEffect);
        }
    }

    /// <summary>
    /// Изменить состояние фоновой музыкм на воспроизведение или пауза в зависимости от текущего состояния
    /// </summary>
    public void ChangeStateCleekd()
    {
        IconCleekdMusic(!listLevels.PlayEffect);

        PlayClick();

        SaveSeting();
    }

    /// <summary>
    /// Меняем отоброжение колонки для клика
    /// </summary>
    /// <param name="turn"> true - воспроизвести фоновую музыку, false - остановить фоновую музыку</param>
    void IconCleekdMusic(bool turn = true)
    {
        if (turn)
        {
            Effect.image.sprite = Resources.Load<Sprite>("Image/PlayMusic");
        }
        else if (!turn)
        {
            Effect.image.sprite = Resources.Load<Sprite>("Image/StopMusic");
        }

        listLevels.PlayEffect = turn;
    }


    /// <summary>
    /// Сохраняем в PlayerPrefs настройки игры
    /// </summary>
    void SaveSeting() { UnityEngine.PlayerPrefs.SetString("Save", JsonUtility.ToJson(listLevels)); }
    #endregion

    //Открыть предыдущее окно
    public void ReturnBack()
    {
        //Удоляем текущее окно из списка и открываем предыдущее окно
        actionHistory.Pop();
        transitionsBetweenWindows(actionHistory.Peek());

        //Если в масиве меньше 2 значений значит мы на главной странице, скрываем кнопку назад
        if (actionHistory.Count < 2) { BackButton.SetActive(false); }
    }
}

/*сохранение
#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(listLevels));
    }    
#endif

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(listLevels));
    }*/
