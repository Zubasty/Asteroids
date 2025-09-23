using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGame : MonoBehaviour
{
    //Переменная отвечающая за кол-во набранных очков
    int score = 0;
    //Время между спавнами врагов в начале игры
    public float TimeSpawnStart;
    //Ссылка на префаб игрока
    public PlayerSpaceShip PlayerPref;
    //Ссылка на префаб вражеского корабля
    public EnemySpaceShip EnemySpacePref;
    //Ссылка на префаб астероида
    public Asteroid AsteroidPref;
    //Ссылка на префаб текста набора очков
    public TextMesh PrefText;
    //Ссылка на панель паузы
    public GameObject PanelPause;
    //Ссылка на кнопку старт/рестарт/продолжить
    public Button StartButton;
    //Ссылка на кнопку выход
    public Button ExitButton;
    //Ссылка на спрайты вкл/выкл звук
    public Sprite[] SoundSprites;
    //Ссылка на кнопку включения/выключения звука
    public Button SoundButton;
    //Ссылка на спрайты вкл/выкл музыки
    public Sprite[] MusicSprites;
    //Ссылка на кнопку вкл/выкл музыки
    public Button MusicButton;
    //Ссылка на количество очков
    public Text ScoreText;
    //Ссылка на фоновую музыку                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
    public AudioSource MusicFon;
    //Время появления новых объектов
    float timeSpawn = 0;
    //Время прошедшее с появления последнего объекта
    float time = 0;
    //Игрок
    PlayerSpaceShip Player;
    //Список врагов
    List<Enemy> Enemies = new List<Enemy>();
    //Переменная отвечающая за паузу
    bool pause = true;
    //Переменная отвечающая за музыку
    bool music_on = true;
    //Переменная отвечающая за звуки
    bool sound_on = true;
    //Свойство для включения и отключения звука
    bool Sound_on
    {
        //Возвращаем значение sound_on
        get { return sound_on; }
        set
        {
            //Меняем значение sound_on
            sound_on = value;
            //Изменяем наличие звука взрыва у класса взрывающихся объектов
            ExplosionObj.isOnSoundExp = sound_on;
            //Перебираем всех уже созданных врагов
            foreach(Enemy e in Enemies)
            {
                //если это вражеский космический корабль
                if(e is EnemySpaceShip)
                    //отключаем/включаем звук у вражеского космического корабля
                    ((EnemySpaceShip)e).GetComponent<AudioSource>().mute = !sound_on;
            }
            //отключаем/включаем звук у префаба вражеского космического корабля
            EnemySpacePref.GetComponent<AudioSource>().mute = !sound_on;
            //отключаем/включаем звук у игрока
            if(Player)
                Player.GetComponent<AudioSource>().mute = !sound_on;
            //и префаба игрока
            PlayerPref.GetComponent<AudioSource>().mute = !sound_on;
        }
    }
    //Свойство для включения и отключения музыки
    bool Music_on
    {
        //Возвращаем значение music_on
        get { return music_on; }
        set
        {
            //Меняем значение music_on
            music_on = value;
            //Включаем/отключаем звук у MusicFon
            MusicFon.mute = !music_on;
        }
    }
    //Свойство, отвечающее за кол-во очков
    int Score
    {
        //Возвращаем кол-во очков
        get { return score; }
        set 
        { 
            //Меняем кол-во очков
            score = value;
            //Выводим информацию об изменении в UI
            ScoreText.text = "Очки: " + score;
        }
    }
    //Свойство отвечающее за паузу
    bool Pause
    {
        //Возвращаем значение pause
        get { return pause; }
        set
        {
            //Меняем значение pause
            pause = value;
            //Включаем/отключаем панель паузы
            PanelPause.SetActive(pause);
            //Добавляем/убираем паузу
            Time.timeScale = pause ? 0 : 1;
            //Если игрок существует, то ставим
            //взаимодействие с ним на паузу
            if (Player) Player.pauseSpace = pause;
        }
    }
    //Функция клика по кнопке пауза
    public void PauseClick()
    {
        //Если существует игрок, т.е. игра началась, то
        if(Player)
            //Меняем значение свойства Pause
            Pause = !Pause;
    }
    //Обработчик события смерти врага, удаляем информацию о нем
    void OnDieEnemy(Enemy enemy)
    {
        //Пытаемся преобразовать переменную врага в астероид
        Asteroid a = enemy as Asteroid;
        //Если получилось
        if (a)
        {
            //То перебираем то, во что астероид должен
            //превратиться после смерти
            foreach(Asteroid ast in a.PublicAsteroids)
            {
                //Если новый астероид существует
                if(ast)
                {
                    //Добавляем его в список врагов
                    Enemies.Add(ast);
                    //Добавляем обработчики события смерти для новых астероидов
                    ast.DieEvent.AddListener(delegate { OnDieEnemy(ast); });
                    ast.DieEvent.AddListener(delegate { OnDieEnemyPlusScore(ast); });
                }
            }
        }
        //Удаляем врага из ббщего списка врагов
        Enemies.Remove(enemy);
    }
    //Обработчик события смерти врага, начисляем деньги
    void OnDieEnemyPlusScore(Enemy enemy)
    {
        //Переменная для определения кол-ва очков
        int sc = 0;
        //Если враг - это космический корабль
        if(enemy is EnemySpaceShip)
        {
            //То плюс 50 очков
            sc = 50;
        }
        //иначе
        else
        {
            //Расчитываем количество очков за уничтоженный астероид
            /*
            Самый маленький астероид - 40 очков, маленький средний - 30
            средний большой - 20, самый большой - 10.
            f(0)=40, f(1)=30, f(2)=20, f(1)=10, т.о.
            f(x)=-10*x+40
             */
            sc = -10 * (int)((Asteroid)enemy).TypeAsteroid + 40;
        }
        //Создаем доп.начисление очков
        InstantiateTextScorePlus(sc, ((MonoBehaviour)enemy).transform.position);
    }
    private void Awake()
    {
        //Для класса SpaceObj инициализируем значения
        SpaceObj.InicStaticCam();
        //Добавляем обработчики клика по кнопке StartButton
        StartButton.onClick.AddListener(delegate {
            //Выключается пауза
            Pause = false; 
            //В кнопку пишем текст "Продолжить"
            StartButton.GetComponentInChildren<Text>().text = "Продолжить"; 
        });
        StartButton.onClick.AddListener(StartGame);
        //Добавляем обработчик клика по кнопке выход
        ExitButton.onClick.AddListener(delegate { Application.Quit(); });
        //Добавляем обработчик клика по кнопке звука
        SoundButton.onClick.AddListener(SoundSwitch);
        //Добавляем обработчик клика по кнопке музыки
        MusicButton.onClick.AddListener(MusicSwitch);
        //У префабов космических кораблей включаем звук
        PlayerPref.GetComponent<AudioSource>().mute = 
            EnemySpacePref.GetComponent<AudioSource>().mute = false;

    }
    //Функция для вкл/выкл звуков
    void SoundSwitch()
    {
        //Меняем значение Sound_on
        Sound_on = !Sound_on;
        //И картинку на кнопке
        SoundButton.GetComponent<Image>().sprite = SoundSprites[Sound_on ? 1 : 0];
    }
    //Функция для вкл/выкл музыки
    void MusicSwitch()
    {
        //Меняем значение Music_on
        Music_on = !Music_on;
        //И картинку на кнопке
        MusicButton.GetComponent<Image>().sprite = MusicSprites[Music_on ? 1 : 0];
    }
    //Функция старта игрового процесса
    void StartGame()
    {
        //Задаем время спавна врагов
        timeSpawn = TimeSpawnStart;
        //Обнуляем количество очков
        Score = 0;
        //Создаем экземпляр игрока
        Player = Instantiate(PlayerPref);
        //Добавляем обработчик события выстрела
        Player.ShotEvent.AddListener(delegate { Score-=5; });
        //Убираем функцию StartGame из обработчиков события StartButton
        StartButton.onClick.RemoveListener(StartGame);
        //Добавляем обработчик события смерти игрока
        Player.DieEvent.AddListener(delegate { StartCoroutine(EndGame()); });
    }
    //Корутин, срабатывающий при смерти игрока
    IEnumerator EndGame()
    {
        //Сразу делаем так, чтобы выполнение корутина
        //продолжилось через 3 секунды, чтобы успели
        //выполниться все анимации
        yield return new WaitForSeconds(3);
        //Перебираем всех врагов
        while (Enemies.Count>0)
        {
            //Получаем экземпляр типа MonoBehaviour
            SpaceObj enemy = (SpaceObj)Enemies[0];
            //Если враг еще существует
            if (enemy)
            {
                EnemySpaceShip SpSh = enemy as EnemySpaceShip;
                if(SpSh)
                {
                    Destroy(SpSh.Arrow.gameObject);
                }
                //То уничтожаем объект врага
                Destroy(enemy.gameObject);
            }    
            //Удаляем врага из списка
            Enemies.Remove(Enemies[0]);
        }
        //Добавляем обработчик клика по кнопке старт
        StartButton.onClick.AddListener(StartGame);
        //Меняем текст в кнопке на "Заново"
        StartButton.GetComponentInChildren<Text>().text = "Заново";
        //Игровой процесс ставим на паузу
        Pause = true;
        yield return null;
    }
    //Функция для создания врагов
    void SpawnEnemy()
    {
        //В 80% случаев должен создавать астероид
        if(Random.value<0.8f)
        {
            SpawnAsteroid();
        }
        //В остальных 20% вражеский космический корабль
        else
        {
            SpawnUFO();
        }
    }
    //Функция создания текста после смерти врага
    private void InstantiateTextScorePlus(int sc, Vector2 pos)
    {
        //Добавляем очки
        Score += sc;
        //Создаем экземпляр текста
        TextMesh txt = Instantiate(PrefText);
        //Пишем в него "+" и кол-во полученных очков
        txt.text = "+" + sc;
        //Перемещаем его к убитому врагу
        txt.transform.position = pos;
        //Запускаем корутин жизненного цикла экземпляра текста
        StartCoroutine(TextLifeCycle(txt));
    }
    //Корутин жизненного цикла экземпляра
    IEnumerator TextLifeCycle(TextMesh txt)
    {
        //Перемещаем его немного выше, чем находится экземпляр
        //вражеского объекта
        txt.transform.Translate(0, 1, 0);
        //До тех пор пока текст не стал полностью прозрачным
        while (txt.color.a>0)
        {
            //Увеличиваем его прозрачность
            txt.color = new Color(txt.color.r, txt.color.g, 
                txt.color.b, txt.color.a - Time.deltaTime/3);
            //И перемещаем выше
            txt.transform.Translate(0, Time.deltaTime, 0);
            //Конец кадра
            yield return null;
        }
        //Когда объект стал полностью прозрачным, уничтожаем его
        Destroy(txt.gameObject);
        yield return null;
    }
    //Функция спавна астероида
    void SpawnAsteroid()
    {
        //Создаем астероид и добавляем его в список врагов
        Enemies.Add(Instantiate(AsteroidPref, SpaceObj.RandomPositionInCamera(), Quaternion.identity));
        //Передаем только что созданный астероид в переменную
        Asteroid a = (Asteroid)Enemies[Enemies.Count - 1];
        //Задаем ему случайный тип
        a.TypeAsteroid = (typeAsteroid)Random.Range(0, 4);
        //Добавляем обработчики события смерти астероида
        a.DieEvent.AddListener(delegate { OnDieEnemy(a); });
        a.DieEvent.AddListener(delegate { OnDieEnemyPlusScore(a); });
    }
    //Функция спавна вражеского космического корабля
    void SpawnUFO()
    {
        //Создаем вражеский космический корабль и добавляем его в список врагов
        Enemies.Add(Instantiate(EnemySpacePref, SpaceObj.RandomPositionInCamera(), Quaternion.identity));
        //Передаем созданный вражеский космический корабль в переменную
        EnemySpaceShip ESS = (EnemySpaceShip)Enemies[Enemies.Count - 1];
        //Передаем ссылку на игрока
        ESS.Player = Player;
        //Добавляем обработчики события смерти вражеского космического корабля
        ESS.DieEvent.AddListener(delegate { OnDieEnemy(ESS); });
        ESS.DieEvent.AddListener(delegate { OnDieEnemyPlusScore(ESS); });
    }
    private void Update()
    {
        //Если не пауза
        if(!Pause)
        {
            //То время до спавна следующего врага уменьшается
            time -= Time.deltaTime;
            //Если время меньше нуля
            if (time < 0)
            {
                //То уменьшаем время до спавна следующего врага
                timeSpawn -= Time.deltaTime;
                //Передаем его в переменную time
                time = timeSpawn;
                //Создаем врага
                SpawnEnemy();
            }
        }
    }
}
