using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//Типы астероидов
public enum typeAsteroid
{
    //Маленький
    small,
    //средний маленький
    middlesmall,
    //средний большой
    middlebig,
    //большой
    big
}
//Класс представляющий астероид, являющийся
//космическим объектом и врагом
public class Asteroid : SpaceObj, Enemy
{
    //Ссылка на спрайты маленьких астероидов
    public Sprite[] SmallAsteroidsSprites;
    //Ссылка на спрайты средних маленьких астероидов
    public Sprite[] MiddleSmallAsteroidsSprites;
    //Ссылка на спрайты средних больших астероидов
    public Sprite[] MiddleBigAsteroidsSprites;
    //Ссылка на спрайты больших астероидов
    public Sprite[] BigAsteroidsSprites;
    //Ссылка на размеры коллайдеров для разных типов астероидов
    public float[] SizeAsteroid;
    //Сила с которой начинает лететь астероид
    public float Power;
    //Зубчатый массив для хранения спрайтов астероидов
    Sprite[][] sprites_asteroids;
    //Тип астероида
    private typeAsteroid typeAsteroid = typeAsteroid.big;
    //То на что распадается астероид после взрыва
    Asteroid[] asteroids = new Asteroid[2];
    //Публичное свойство для доступа к новым астероидам
    public Asteroid[] PublicAsteroids
    {
        get { return asteroids; }
    }
    //Свойство для изменения типа астероида
    public typeAsteroid TypeAsteroid
    {
        //get возвращает значение переменной typeAsteroid
        get { return typeAsteroid; }
        //set меняет тип астероида, а также спрайт и размер коллайдера
        set
        {
            //Меняем тип астероида
            typeAsteroid = value;
            //Рандомно выбираем спрайт в соответствующий 
            //зубец массива sprites_asteroids
            GetComponent<SpriteRenderer>().sprite =
                sprites_asteroids[(int)typeAsteroid][Random.Range(0,
                sprites_asteroids[(int)typeAsteroid].Length)];
            //Выбираем размер коллайдера
            GetComponent<CircleCollider2D>().radius = SizeAsteroid[(int)typeAsteroid];
        }
    }
    private void Awake()
    {
        //Создаем массив из четырех массивов спрайтов
        sprites_asteroids = new Sprite[4][];
        //Заполняем зубчатый массив
        initialized_sprites();
        //"Толкаем" астероид
        force();
        //Добавляем обработчик события смерти
        DieEvent.AddListener(die);
    }
    //Функция, чтобы "толкать" астероид
    void force()
    {
        //Вводим вектор для того, чтобы определять куда толкать астероид,
        //чтобы, например, при появлении астероида в нижней правой части 
        //экрана, он летел влево и вверх
        Vector2 direction = transform.position - Camera.main.transform.position;
        //Задаем силу с которой "толкнем астероид"
        Vector2 force = new Vector2(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f));
        //Определяем направление куда полетит астероид
        force.x = direction.x > 0 ? -force.x : force.x;
        force.y = direction.y > 0 ? -force.y : force.y;
        //"Толкаем" астероид
        GetComponent<Rigidbody2D>().AddForce(force * Power);
    }
    //Метод для заполнения зубчатого массива
    void initialized_sprites()
    {
        //Нулевой зубец заполняем массивом маленьких астероидов
        sprites_asteroids[0] = SmallAsteroidsSprites;
        //Первый зубец заполняем массивом средних маленьких астероидов
        sprites_asteroids[1] = MiddleSmallAsteroidsSprites;
        //Второй зубец заполняем массивом средних больших астероидов
        sprites_asteroids[2] = MiddleBigAsteroidsSprites;
        //Третий зубец заполняем массивом больших астероидов
        sprites_asteroids[3] = BigAsteroidsSprites;
    }
    //Обработчик события смерти
    void die()
    {
        //Если астероид больше, чем маленький
        if (TypeAsteroid > typeAsteroid.small)
        {
            //То разбиваем его на 2
            for (int i = 0; i < asteroids.Length; i++)
            {
                //Создаем экземпляр астероида на основе текущего астероида
                asteroids[i] = Instantiate(gameObject).GetComponent<Asteroid>();
                //Его размер должен быть на 1 пункт меньше, чем у предыдущего
                asteroids[i].TypeAsteroid = TypeAsteroid - 1;
            }
        }
        //уничтожаем старый астероид
        Destroy(gameObject);
    }

    //Функция для разрушения астероида
    public void DieEnemy()
    {
        //Вызываем обработчик события DieEvent
        DieEvent?.Invoke();
    }
    //Функция для уничтожения астероида, если он вылетел за экран
    void OnExitScreen()
    {
        //если вылетили за экран
        if (ExitScreen())
        {
            //уничтожаем астероид
            Destroy(gameObject);
        }
    }
    void Update()
    {
        //Уничтожаем астероид, если он вылетел за экран
        OnExitScreen();
    }
}
