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

    float timeLeft = 60;
    float timeMax;
    float timeЗапоминать;

    [Header("Поля с настройками для своей игры")]
    public Dropdown dropdown1;
    public Dropdown dropdown2;
    public InputField inputField1;
    public InputField inputField2;

    




    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();//Обнуляю настройки

        if (!UnityEngine.PlayerPrefs.HasKey("Save"))
        {
            listLevels = new SettingsGame();
        }
        else
        {
            listLevels = JsonUtility.FromJson<SettingsGame>(UnityEngine.PlayerPrefs.GetString("Save"));
        }


        //Задаем громкость фоновой музыки скролу из настроек и плэеру
        VolumeBackgroundMusic.value = SourceBackgroundMusic.volume = listLevels.VolumeBackground;

        //Запускаем фоновую музыку если надо
        PlayBackgroundMusic(listLevels.PlayBackground);

        //Задаем громкость кликам
        VolumeEffect.value = listLevels.VolumeEffect;

        PlayCleekdMusic(listLevels.PlayEffect);




        audioSource = GetComponent<AudioSource>();

        animator = gameObject.GetComponent<Animator>();
        actionHistory.Push(0);


        for (int i = 0; i < SettingsGame.numberLevels; i++)
        {
            var instance = Instantiate(PlayerPrefs, ControlForLevels);

            instance.name = i.ToString();
            foreach (Transform child in instance.transform)
            {
                if (child.name == "Text")
                {
                    child.GetComponent<Text>().text = (i + 1).ToString();
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }     
#endif

        //Если произошли изменения в громкости музыки правим и сохраняем в реестр
        var vv = listLevels.VolumeBackground != VolumeBackgroundMusic.value;
        if (vv)
        {
            SourceBackgroundMusic.volume = listLevels.VolumeBackground = VolumeBackgroundMusic.value;
            if (listLevels.VolumeBackground <= 0.02f) { PlayBackgroundMusic(false); } else { PlayBackgroundMusic(); }
        }
        var bb = listLevels.VolumeEffect != VolumeEffect.value;
        if (bb)
        {
            listLevels.VolumeEffect = VolumeEffect.value;
            if (listLevels.VolumeEffect <= 0.02f) { PlayCleekdMusic(false); } else { PlayCleekdMusic(); }

        }
        if (vv|| bb) { SaveSeting(); }


        if (parametersGame.ShowTimer)
        {
            if (parametersGame.StateGame == 0 && timeЗапоминать > 0)
            {
                timeLeft += 1 * Time.deltaTime;
                if (timeЗапоминать <= timeLeft)
                {
                    Gecr();
                }
            }
            else if (parametersGame.StateGame == StateGame.Started && timeMax > 0)
            {
                timeLeft -= 1 * Time.deltaTime;
                if (timeLeft <= 0)
                {
                    ПодвестиИтог();
                }
            }

            GameTimer.value = timeLeft;
        }
    }

    #region Игровой процес
    /// <summary>
    /// Обучающий раунд(игра)
    /// </summary>
    public void Обучение() { transitionsBetweenWindows(ActionsGame.TrainingRound); }

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
    public void ОкеоСвоиУровни()  { transitionsBetweenWindows(ActionsGame.CreateYourLevel); }

    /// <summary>
    /// Запустить игру со своими настройками
    /// </summary>
    public void СвояИгра() { transitionsBetweenWindows(ActionsGame.StartGameYourSettings); }

    /// <summary>
    /// Переходы между окнами
    /// </summary>
    /// <param name="actionsGame">Какое надо выполнить действие</param>
    void transitionsBetweenWindows(ActionsGame actionsGame)
    {
        PlayClick();

        int i = (int)actionsGame;

        switch (actionsGame)
        {
            case ActionsGame.StartGame:
                MessageVictory.SetActive(false);
                MessageLosing.SetActive(false);

                #region Задаем параметры игре
                if (parametersGame.свойУровень)
                {
                    if (int.Parse(inputField1.text) > 0 || int.Parse(inputField2.text) > 0)
                    {
                        parametersGame.ShowTimer = true;
                    }
                    collectSamples(dropdown1.value + 2, dropdown2.value + 2, false, int.Parse(inputField1.text), int.Parse(inputField2.text));
                }
                else if (parametersGame.ShowTimer)
                {
                    switch (parametersGame.уровень + 1)
                    {
                        case 1:
                            collectSamples(5, 2, false, 25, 45);
                            break;
                        case 2:
                            collectSamples(6, 2, false, 25, 45);
                            break;
                        case 3:
                            collectSamples(7, 2, false, 25, 45);
                            break;
                        case 4:
                            collectSamples(8, 2, false, 25, 45);
                            break;
                        case 5:
                            collectSamples(5, 3, false, 25, 45);
                            break;
                        case 6:
                            collectSamples(6, 3, false, 25, 45);
                            break;
                        case 7:
                            collectSamples(7, 3, false, 25, 45);
                            break;
                        case 8:
                            collectSamples(8, 3, false, 25, 45);
                            break;
                        case 9:
                            collectSamples(9, 3, false, 25, 45);
                            break;
                        case 10:
                            collectSamples(5, 4, false, 25, 45);
                            break;
                        case 11:
                            collectSamples(6, 4, false, 25, 45);
                            break;
                        case 12:
                            collectSamples(7, 4, false, 25, 45);
                            break;
                        case 13:
                            collectSamples(8, 4, false, 25, 45);
                            break;
                        case 14:
                            collectSamples(9, 4, false, 25, 45);
                            break;
                        case 15:
                            collectSamples(10, 4, false, 25, 45);
                            break;
                        case 16:
                            collectSamples(5, 5, false, 25, 45);
                            break;
                        case 17:
                            collectSamples(6, 5, false, 25, 45);
                            break;
                        case 18:
                            collectSamples(7, 5, false, 25, 45);
                            break;
                        case 19:
                            collectSamples(8, 5, false, 25, 45);
                            break;
                        case 20:
                            collectSamples(9, 5, false, 25, 45);
                            break;
                        case 21:
                            collectSamples(10, 5, false, 25, 45);
                            break;
                        case 22:
                            collectSamples(11, 5, false, 25, 45);
                            break;
                        case 23:
                            collectSamples(5, 6, false, 25, 45);
                            break;
                        case 24:
                            collectSamples(6, 6, false, 25, 45);
                            break;
                        case 25:
                            collectSamples(7, 6, false, 25, 45);
                            break;
                        case 26:
                            collectSamples(8, 6, false, 25, 45);
                            break;
                        case 27:
                            collectSamples(9, 6, false, 25, 45);
                            break;
                        case 28:
                            collectSamples(10, 6, false, 25, 45);
                            break;
                        case 29:
                            collectSamples(11, 6, false, 25, 45);
                            break;
                        case 30:
                            collectSamples(6, 7, false, 25, 45);
                            break;
                        case 31:
                            collectSamples(7, 7, false, 25, 45);
                            break;
                        case 32:
                            collectSamples(8, 7, false, 25, 45);
                            break;
                        case 33:
                            collectSamples(9, 7, false, 25, 45);
                            break;
                        case 34:
                            collectSamples(10, 7, false, 25, 45);
                            break;
                        case 35:
                            collectSamples(11, 7, false, 25, 45);
                            break;
                        case 36:
                            collectSamples(6, 8, false, 25, 45);
                            break;
                        case 37:
                            collectSamples(7, 8, false, 25, 45);
                            break;
                        case 38:
                            collectSamples(8, 8, false, 25, 45);
                            break;
                        case 39:
                            collectSamples(9, 8, false, 25, 45);
                            break;
                        case 40:
                            collectSamples(10, 8, false, 25, 45);
                            break;
                        case 41:
                            collectSamples(11, 8, false, 25, 45);
                            break;
                        case 42:
                            collectSamples(6, 9, false, 25, 45);
                            break;
                        case 43:
                            collectSamples(7, 9, false, 25, 45);
                            break;
                        case 44:
                            collectSamples(8, 9, false, 25, 45);
                            break;
                        case 45:
                            collectSamples(9, 9, false, 25, 45);
                            break;
                        case 46:
                            collectSamples(10, 9, false, 25, 45);
                            break;
                        case 47:
                            collectSamples(11, 9, false, 25, 45);
                            break;
                        case 48:
                            collectSamples(6, 10, false, 25, 45);
                            break;
                        case 49:
                            collectSamples(7, 10, false, 25, 45);
                            break;
                        case 50:
                            collectSamples(8, 10, false, 25, 45);
                            break;
                        case 51:
                            collectSamples(9, 10, false, 25, 45);
                            break;
                        case 52:
                            collectSamples(10, 10, false, 25, 45);
                            break;
                        case 53:
                            collectSamples(11, 10, false, 25, 45);
                            break;
                        case 54:
                            collectSamples(6, 10, false, 25, 45);
                            break;
                        case 55:
                            collectSamples(7, 10, false, 25, 45);
                            break;
                        case 56:
                            collectSamples(8, 10, false, 25, 45);
                            break;
                        case 57:
                            collectSamples(9, 10, false, 25, 45);
                            break;
                        case 58:
                            collectSamples(10, 10, false, 25, 45);
                            break;
                        case 59:
                            collectSamples(11, 10, false, 25, 45);
                            break;
                        case 60:
                            collectSamples(6, 11, false, 25, 45);
                            break;
                        case 61:
                            collectSamples(7, 11, false, 25, 45);
                            break;
                        case 62:
                            collectSamples(8, 11, false, 25, 45);
                            break;
                        case 63:
                            collectSamples(9, 11, false, 25, 45);
                            break;
                        case 64:
                            collectSamples(10, 11, false, 25, 45);
                            break;
                        case 65:
                            collectSamples(11, 11, false, 25, 45);
                            break;
                        default:
                            collectSamples(11, 11, false, 25, 45);
                            break;
                    }

                }
                else
                {
                    switch (parametersGame.уровень + 1)
                    {
                        case 1:
                            collectSamples(5, 2);
                            break;
                        case 2:
                            collectSamples(6, 2);
                            break;
                        case 3:
                            collectSamples(7, 2);
                            break;
                        case 4:
                            collectSamples(8, 2);
                            break;
                        case 5:
                            collectSamples(5, 3);
                            break;
                        case 6:
                            collectSamples(6, 3);
                            break;
                        case 7:
                            collectSamples(7, 3);
                            break;
                        case 8:
                            collectSamples(8, 3);
                            break;
                        case 9:
                            collectSamples(9, 3);
                            break;
                        case 10:
                            collectSamples(5, 4);
                            break;
                        case 11:
                            collectSamples(6, 4);
                            break;
                        case 12:
                            collectSamples(7, 4);
                            break;
                        case 13:
                            collectSamples(8, 4);
                            break;
                        case 14:
                            collectSamples(9, 4);
                            break;
                        case 15:
                            collectSamples(10, 4);
                            break;
                        case 16:
                            collectSamples(5, 5);
                            break;
                        case 17:
                            collectSamples(6, 5);
                            break;
                        case 18:
                            collectSamples(7, 5);
                            break;
                        case 19:
                            collectSamples(8, 5);
                            break;
                        case 20:
                            collectSamples(9, 5);
                            break;
                        case 21:
                            collectSamples(10, 5);
                            break;
                        case 22:
                            collectSamples(11, 5);
                            break;
                        case 23:
                            collectSamples(5, 6);
                            break;
                        case 24:
                            collectSamples(6, 6);
                            break;
                        case 25:
                            collectSamples(7, 6);
                            break;
                        case 26:
                            collectSamples(8, 6);
                            break;
                        case 27:
                            collectSamples(9, 6);
                            break;
                        case 28:
                            collectSamples(10, 6);
                            break;
                        case 29:
                            collectSamples(11, 6);
                            break;
                        case 30:
                            collectSamples(6, 7);
                            break;
                        case 31:
                            collectSamples(7, 7);
                            break;
                        case 32:
                            collectSamples(8, 7);
                            break;
                        case 33:
                            collectSamples(9, 7);
                            break;
                        case 34:
                            collectSamples(10, 7);
                            break;
                        case 35:
                            collectSamples(11, 7);
                            break;
                        case 36:
                            collectSamples(6, 8);
                            break;
                        case 37:
                            collectSamples(7, 8);
                            break;
                        case 38:
                            collectSamples(8, 8);
                            break;
                        case 39:
                            collectSamples(9, 8);
                            break;
                        case 40:
                            collectSamples(10, 8);
                            break;
                        case 41:
                            collectSamples(11, 8);
                            break;
                        case 42:
                            collectSamples(6, 9);
                            break;
                        case 43:
                            collectSamples(7, 9);
                            break;
                        case 44:
                            collectSamples(8, 9);
                            break;
                        case 45:
                            collectSamples(9, 9);
                            break;
                        case 46:
                            collectSamples(10, 9);
                            break;
                        case 47:
                            collectSamples(11, 9);
                            break;
                        case 48:
                            collectSamples(6, 10);
                            break;
                        case 49:
                            collectSamples(7, 10);
                            break;
                        case 50:
                            collectSamples(8, 10);
                            break;
                        case 51:
                            collectSamples(9, 10);
                            break;
                        case 52:
                            collectSamples(10, 10);
                            break;
                        case 53:
                            collectSamples(11, 10);
                            break;
                        case 54:
                            collectSamples(6, 10);
                            break;
                        case 55:
                            collectSamples(7, 10);
                            break;
                        case 56:
                            collectSamples(8, 10);
                            break;
                        case 57:
                            collectSamples(9, 10);
                            break;
                        case 58:
                            collectSamples(10, 10);
                            break;
                        case 59:
                            collectSamples(11, 10);
                            break;
                        case 60:
                            collectSamples(6, 11);
                            break;
                        case 61:
                            collectSamples(7, 11);
                            break;
                        case 62:
                            collectSamples(8, 11);
                            break;
                        case 63:
                            collectSamples(9, 11);
                            break;
                        case 64:
                            collectSamples(10, 11);
                            break;
                        case 65:
                            collectSamples(11, 11);
                            break;
                        default:
                            collectSamples(11, 11);
                            break;
                    }
                }
                #endregion

                i = 3;
                break;

            case ActionsGame.StartGameYourSettings:
                i = -1;
                break;

            case ActionsGame.ListLevelsTime:
                СкрытьВсе();
                parametersGame.свойУровень = false;

                foreach (Transform child in ControlForLevels.transform)
                {
                    bool re = false;
                    foreach (Transform child1 in child.transform)
                    {
                        foreach (var item in child1.GetComponentsInChildren<Image>())
                        {
                            re = listLevels.StateLevelTime[int.Parse(child.name)];
                            child1.gameObject.SetActive(re);
                        }
                    }

                    child.GetComponent<Button>().onClick.RemoveAllListeners();
                    child.GetComponent<Button>().onClick.AddListener(() => { if (!re) { parametersGame.уровень = int.Parse(child.name); parametersGame.ShowTimer = true; transitionsBetweenWindows(ActionsGame.StartGame); } });
                }

                i = 2;
                break;

            case ActionsGame.ListSimpleLevels:
                СкрытьВсе();
                parametersGame.свойУровень = false;

                foreach (Transform child in ControlForLevels.transform)
                {
                    bool re = false;
                    foreach (Transform child1 in child.transform)
                    {
                        foreach (var item in child1.GetComponentsInChildren<Image>())
                        {
                            re = listLevels.StateLevelNormal[int.Parse(child.name)];
                            child1.gameObject.SetActive(re);
                        }
                    }
                    child.GetComponent<Button>().onClick.RemoveAllListeners();
                    child.GetComponent<Button>().onClick.AddListener(() => { if (!re) { parametersGame.уровень = int.Parse(child.name); parametersGame.ShowTimer = false; transitionsBetweenWindows(ActionsGame.StartGame); } });
                }

                i = 2;
                break;

            case ActionsGame.TrainingRound:
                collectSamples(3, 3, true);
                break;
            case ActionsGame.CreateYourLevel:
            case ActionsGame.OpenSettings:
            case 0:
                СкрытьВсе();
                break;
        }

        if (i>-1) { animator.SetInteger(nameof(ActionsGame), i); }

        //сохраняем только один экземпляр окна чтобы после 3 раундов(уровней) игр не жать 3 раза назад 
        if (actionHistory.Peek() != actionsGame && !(actionHistory.Peek() == ActionsGame.StartGameYourSettings && ActionsGame.StartGame == actionsGame)) { actionHistory.Push(actionsGame); }

        //Включаем кнопку назад
        BackButton.SetActive(true);

        switch (actionsGame)
        {
            case ActionsGame.StartGame:
                MessageVictory.SetActive(false);
                MessageLosing.SetActive(false);

                #region Задаем параметры игре
                if (parametersGame.свойУровень)
                {
                    if (int.Parse(inputField1.text) > 0 || int.Parse(inputField2.text) > 0)
                    {
                        parametersGame.ShowTimer = true;
                    }
                    collectSamples(dropdown1.value + 2, dropdown2.value + 2, false, int.Parse(inputField1.text), int.Parse(inputField2.text));
                }
                else if (parametersGame.ShowTimer)
                {
                    switch (parametersGame.уровень + 1)
                    {
                        case 1:
                            collectSamples(5, 2, false, 25, 45);
                            break;
                        case 2:
                            collectSamples(6, 2, false, 25, 45);
                            break;
                        case 3:
                            collectSamples(7, 2, false, 25, 45);
                            break;
                        case 4:
                            collectSamples(8, 2, false, 25, 45);
                            break;
                        case 5:
                            collectSamples(5, 3, false, 25, 45);
                            break;
                        case 6:
                            collectSamples(6, 3, false, 25, 45);
                            break;
                        case 7:
                            collectSamples(7, 3, false, 25, 45);
                            break;
                        case 8:
                            collectSamples(8, 3, false, 25, 45);
                            break;
                        case 9:
                            collectSamples(9, 3, false, 25, 45);
                            break;
                        case 10:
                            collectSamples(5, 4, false, 25, 45);
                            break;
                        case 11:
                            collectSamples(6, 4, false, 25, 45);
                            break;
                        case 12:
                            collectSamples(7, 4, false, 25, 45);
                            break;
                        case 13:
                            collectSamples(8, 4, false, 25, 45);
                            break;
                        case 14:
                            collectSamples(9, 4, false, 25, 45);
                            break;
                        case 15:
                            collectSamples(10, 4, false, 25, 45);
                            break;
                        case 16:
                            collectSamples(5, 5, false, 25, 45);
                            break;
                        case 17:
                            collectSamples(6, 5, false, 25, 45);
                            break;
                        case 18:
                            collectSamples(7, 5, false, 25, 45);
                            break;
                        case 19:
                            collectSamples(8, 5, false, 25, 45);
                            break;
                        case 20:
                            collectSamples(9, 5, false, 25, 45);
                            break;
                        case 21:
                            collectSamples(10, 5, false, 25, 45);
                            break;
                        case 22:
                            collectSamples(11, 5, false, 25, 45);
                            break;
                        case 23:
                            collectSamples(5, 6, false, 25, 45);
                            break;
                        case 24:
                            collectSamples(6, 6, false, 25, 45);
                            break;
                        case 25:
                            collectSamples(7, 6, false, 25, 45);
                            break;
                        case 26:
                            collectSamples(8, 6, false, 25, 45);
                            break;
                        case 27:
                            collectSamples(9, 6, false, 25, 45);
                            break;
                        case 28:
                            collectSamples(10, 6, false, 25, 45);
                            break;
                        case 29:
                            collectSamples(11, 6, false, 25, 45);
                            break;
                        case 30:
                            collectSamples(6, 7, false, 25, 45);
                            break;
                        case 31:
                            collectSamples(7, 7, false, 25, 45);
                            break;
                        case 32:
                            collectSamples(8, 7, false, 25, 45);
                            break;
                        case 33:
                            collectSamples(9, 7, false, 25, 45);
                            break;
                        case 34:
                            collectSamples(10, 7, false, 25, 45);
                            break;
                        case 35:
                            collectSamples(11, 7, false, 25, 45);
                            break;
                        case 36:
                            collectSamples(6, 8, false, 25, 45);
                            break;
                        case 37:
                            collectSamples(7, 8, false, 25, 45);
                            break;
                        case 38:
                            collectSamples(8, 8, false, 25, 45);
                            break;
                        case 39:
                            collectSamples(9, 8, false, 25, 45);
                            break;
                        case 40:
                            collectSamples(10, 8, false, 25, 45);
                            break;
                        case 41:
                            collectSamples(11, 8, false, 25, 45);
                            break;
                        case 42:
                            collectSamples(6, 9, false, 25, 45);
                            break;
                        case 43:
                            collectSamples(7, 9, false, 25, 45);
                            break;
                        case 44:
                            collectSamples(8, 9, false, 25, 45);
                            break;
                        case 45:
                            collectSamples(9, 9, false, 25, 45);
                            break;
                        case 46:
                            collectSamples(10, 9, false, 25, 45);
                            break;
                        case 47:
                            collectSamples(11, 9, false, 25, 45);
                            break;
                        case 48:
                            collectSamples(6, 10, false, 25, 45);
                            break;
                        case 49:
                            collectSamples(7, 10, false, 25, 45);
                            break;
                        case 50:
                            collectSamples(8, 10, false, 25, 45);
                            break;
                        case 51:
                            collectSamples(9, 10, false, 25, 45);
                            break;
                        case 52:
                            collectSamples(10, 10, false, 25, 45);
                            break;
                        case 53:
                            collectSamples(11, 10, false, 25, 45);
                            break;
                        case 54:
                            collectSamples(6, 10, false, 25, 45);
                            break;
                        case 55:
                            collectSamples(7, 10, false, 25, 45);
                            break;
                        case 56:
                            collectSamples(8, 10, false, 25, 45);
                            break;
                        case 57:
                            collectSamples(9, 10, false, 25, 45);
                            break;
                        case 58:
                            collectSamples(10, 10, false, 25, 45);
                            break;
                        case 59:
                            collectSamples(11, 10, false, 25, 45);
                            break;
                        case 60:
                            collectSamples(6, 11, false, 25, 45);
                            break;
                        case 61:
                            collectSamples(7, 11, false, 25, 45);
                            break;
                        case 62:
                            collectSamples(8, 11, false, 25, 45);
                            break;
                        case 63:
                            collectSamples(9, 11, false, 25, 45);
                            break;
                        case 64:
                            collectSamples(10, 11, false, 25, 45);
                            break;
                        case 65:
                            collectSamples(11, 11, false, 25, 45);
                            break;
                        default:
                            collectSamples(11, 11, false, 25, 45);
                            break;
                    }

                }
                else
                {
                    switch (parametersGame.уровень + 1)
                    {
                        case 1:
                            collectSamples(5, 2);
                            break;
                        case 2:
                            collectSamples(6, 2);
                            break;
                        case 3:
                            collectSamples(7, 2);
                            break;
                        case 4:
                            collectSamples(8, 2);
                            break;
                        case 5:
                            collectSamples(5, 3);
                            break;
                        case 6:
                            collectSamples(6, 3);
                            break;
                        case 7:
                            collectSamples(7, 3);
                            break;
                        case 8:
                            collectSamples(8, 3);
                            break;
                        case 9:
                            collectSamples(9, 3);
                            break;
                        case 10:
                            collectSamples(5, 4);
                            break;
                        case 11:
                            collectSamples(6, 4);
                            break;
                        case 12:
                            collectSamples(7, 4);
                            break;
                        case 13:
                            collectSamples(8, 4);
                            break;
                        case 14:
                            collectSamples(9, 4);
                            break;
                        case 15:
                            collectSamples(10, 4);
                            break;
                        case 16:
                            collectSamples(5, 5);
                            break;
                        case 17:
                            collectSamples(6, 5);
                            break;
                        case 18:
                            collectSamples(7, 5);
                            break;
                        case 19:
                            collectSamples(8, 5);
                            break;
                        case 20:
                            collectSamples(9, 5);
                            break;
                        case 21:
                            collectSamples(10, 5);
                            break;
                        case 22:
                            collectSamples(11, 5);
                            break;
                        case 23:
                            collectSamples(5, 6);
                            break;
                        case 24:
                            collectSamples(6, 6);
                            break;
                        case 25:
                            collectSamples(7, 6);
                            break;
                        case 26:
                            collectSamples(8, 6);
                            break;
                        case 27:
                            collectSamples(9, 6);
                            break;
                        case 28:
                            collectSamples(10, 6);
                            break;
                        case 29:
                            collectSamples(11, 6);
                            break;
                        case 30:
                            collectSamples(6, 7);
                            break;
                        case 31:
                            collectSamples(7, 7);
                            break;
                        case 32:
                            collectSamples(8, 7);
                            break;
                        case 33:
                            collectSamples(9, 7);
                            break;
                        case 34:
                            collectSamples(10, 7);
                            break;
                        case 35:
                            collectSamples(11, 7);
                            break;
                        case 36:
                            collectSamples(6, 8);
                            break;
                        case 37:
                            collectSamples(7, 8);
                            break;
                        case 38:
                            collectSamples(8, 8);
                            break;
                        case 39:
                            collectSamples(9, 8);
                            break;
                        case 40:
                            collectSamples(10, 8);
                            break;
                        case 41:
                            collectSamples(11, 8);
                            break;
                        case 42:
                            collectSamples(6, 9);
                            break;
                        case 43:
                            collectSamples(7, 9);
                            break;
                        case 44:
                            collectSamples(8, 9);
                            break;
                        case 45:
                            collectSamples(9, 9);
                            break;
                        case 46:
                            collectSamples(10, 9);
                            break;
                        case 47:
                            collectSamples(11, 9);
                            break;
                        case 48:
                            collectSamples(6, 10);
                            break;
                        case 49:
                            collectSamples(7, 10);
                            break;
                        case 50:
                            collectSamples(8, 10);
                            break;
                        case 51:
                            collectSamples(9, 10);
                            break;
                        case 52:
                            collectSamples(10, 10);
                            break;
                        case 53:
                            collectSamples(11, 10);
                            break;
                        case 54:
                            collectSamples(6, 10);
                            break;
                        case 55:
                            collectSamples(7, 10);
                            break;
                        case 56:
                            collectSamples(8, 10);
                            break;
                        case 57:
                            collectSamples(9, 10);
                            break;
                        case 58:
                            collectSamples(10, 10);
                            break;
                        case 59:
                            collectSamples(11, 10);
                            break;
                        case 60:
                            collectSamples(6, 11);
                            break;
                        case 61:
                            collectSamples(7, 11);
                            break;
                        case 62:
                            collectSamples(8, 11);
                            break;
                        case 63:
                            collectSamples(9, 11);
                            break;
                        case 64:
                            collectSamples(10, 11);
                            break;
                        case 65:
                            collectSamples(11, 11);
                            break;
                        default:
                            collectSamples(11, 11);
                            break;
                    }
                }
                #endregion
                i = 3;


                if (parametersGame.ShowTimer)
                {
                    GameTimer.gameObject.SetActive(true);
                    timeLeft = 0;
                }


                parametersGame.StateGame = StateGame.Preparation;
                parametersGame.StageTraining = 0;

                break;
            case ActionsGame.TrainingRound:
                parametersGame.StateGame = StateGame.Preparation;
                parametersGame.StageTraining = 1;
                break;

            case ActionsGame.StartGameYourSettings:
                parametersGame.свойУровень = true;
                parametersGame.ShowTimer = false;
                parametersGame.уровень = 0;
                transitionsBetweenWindows(ActionsGame.StartGame);
                i = -1;
                break;
        }
    }


    //количество шариков максимум 11, количество цветов максимум 11 ,
    void collectSamples(int totalBalls, int totalColors, bool training = false, float МахЗапоминать = 0, float МахИграть = 0)
    {
        // сложность = МахЗапоминать > 0 || МахИграть > 0;
        timeЗапоминать = GameTimer.maxValue = МахЗапоминать;
        timeMax = МахИграть;


        //Количество цветов если меньше 1 или больше 11 меняю на 11
        if (totalColors > numberColors || totalColors < 1) { totalColors = numberColors; }

        //Количество шариков если меньше 1 или больше 11 меняю на 11
        if (totalBalls > numberBalls || totalBalls < 1) { totalColors = numberBalls; }
        parametersGame.TotalBalls = totalBalls;

        for (int i = 0; i < numberBalls; i++)
        {
            if (training)
            {
                switch (i)
                {
                    case 0:
                        Samples[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/1");
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
                Training1.SetActive(true);
            }
            else
            {
                if (i >= totalBalls)
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


    //Закрыть шарики доской
    void Gecr()
    {
        parametersGame.StateGame = StateGame.Started;
        if (parametersGame.ShowTimer)
        {
            timeLeft = GameTimer.maxValue = timeMax;
        }
        animator.SetInteger(nameof(ActionsGame), 4);
    }

    public void УстановитьШарик(GameObject gameObject)
    {
        if (parametersGame.StateGame == StateGame.Preparation) { Gecr(); }
        if (parametersGame.StateGame == StateGame.Started)
        {
            PlayClick();

            if (parametersGame.StageTraining <= 0)
            {
                for (int i = 0; i < parametersGame.TotalBalls; i++)
                {
                    if (WorkingBalls[i].GetComponent<Image>().sprite == Resources.Load<Sprite>("Image/Balloons/0"))
                    {
                        WorkingBalls[i].GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;

                        if (i == parametersGame.TotalBalls - 1) { ПодвестиИтог(); }

                        break;
                    }
                }
            }
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
                parametersGame.StageTraining = 0;
                ПодвестиИтог();
            }
        }
    }

    public void УбратьШарик(GameObject gameObject)
    {
        if (parametersGame.StateGame == StateGame.Started)
        {
            PlayClick();

            if (parametersGame.StageTraining == 0)
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
            }
            else if (parametersGame.StageTraining == 3)
            {
                Training3.SetActive(false);
                WorkingBalls[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Balloons/0");
                parametersGame.StageTraining = 4;
                Training4.SetActive(true);
            }
        }
    }

    void ПодвестиИтог()
    {
        bool Итого = true;
        for (int i = 0; i < parametersGame.TotalBalls; i++)
        {
            if (Samples[i].GetComponent<Image>().sprite != WorkingBalls[i].GetComponent<Image>().sprite)
            {
                Итого = false;
                break;
            }
        }

        parametersGame.StateGame = StateGame.GameOver;
        animator.SetInteger(nameof(ActionsGame), 3);

        if (Итого)
        {
            if (parametersGame.уровень > -1)
            {
                if (parametersGame.ShowTimer)
                {

                    if (listLevels.StateLevelTime.Length > parametersGame.уровень)
                    {
                        listLevels.StateLevelTime[parametersGame.уровень + 1] = false;
                    }


                }
                else if (!parametersGame.ShowTimer)
                {
                    if (listLevels.StateLevelNormal.Length > parametersGame.уровень)
                    {
                        listLevels.StateLevelNormal[parametersGame.уровень + 1] = false;
                    }
                }

                SaveSeting();
            }

            foreach (Transform child in MessageVictory.transform)
            {
                var ffd = child.GetComponent<Button>();
                if (ffd != null)
                {
                    ffd.onClick.RemoveAllListeners();
                    if (parametersGame.StageTraining > 0)
                    {
                        actionHistory.Pop();
                        ffd.onClick.AddListener(() => { transitionsBetweenWindows(ActionsGame.ListSimpleLevels); });
                    }
                    else
                    {
                        ffd.onClick.AddListener(() => { parametersGame.уровень = parametersGame.уровень + 1; transitionsBetweenWindows(ActionsGame.StartGame); });
                    }
                }

            }
            MessageVictory.SetActive(true);
        }
        else
        {
            foreach (Transform child in MessageLosing.transform)
            {
                var ffd = child.GetComponent<Button>();
                if (ffd!=null)
                {
                    ffd.onClick.RemoveAllListeners();
                    ffd.onClick.AddListener(() => { transitionsBetweenWindows(ActionsGame.StartGame); });
                }

            }
            MessageLosing.SetActive(true);
        }
    }

    //Скрыть все игровые кнопки
    void СкрытьВсе()
    {
        parametersGame.StateGame = StateGame.GameOver;

        foreach (var item in Samples)
        {
            item.SetActive(false);
        }
        foreach (var item in WorkingBalls)
        {
            item.SetActive(false);
        }
        foreach (var item in SamplesBalls)
        {
            item.SetActive(false);
        }

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
        PlayCleekdMusic(!listLevels.PlayEffect);

        PlayClick();

        SaveSeting();
    }

    /// <summary>
    /// Воспроизводить звук кликов
    /// </summary>
    /// <param name="turn"> true - воспроизвести фоновую музыку, false - остановить фоновую музыку</param>
    void PlayCleekdMusic(bool turn = true)
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
