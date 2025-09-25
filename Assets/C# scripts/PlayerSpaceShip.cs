using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//Класс представляющий космический корабль игрока
public class PlayerSpaceShip : SpaceShip
{
    [SerializeField] private ParticleSystem _moveEffectPrefab;
    [SerializeField] private Transform[] _effectPositions;

    //Событие выстрела
    public UnityEvent ShotEvent = new UnityEvent();
    //Префаб пули
    public GameObject PrefabBullet;
    //Сила выстрела
    public float PowerShot;
    //Пауза для отсутствия взаимодействия с кораблем
    public bool pauseSpace;

    private void Awake()
    {
        //Инициализируем компоненты
        Inic();
        //Задаем цвет стрелке
        Arrow.color = Color.green;
        //Добавляем обработчик события ShotEvent
        ShotEvent.AddListener(shot);
    }
       
    private void OnEnable()
    {
        Debug.Log("Added Listener");
        MoveEvent.AddListener(AfterMove);
    }

    public void Update()
    {
        if(!pauseSpace)
        {
            //Переменная для позиции курсора
            Vector2 pos_mouse = new Vector2();
            //Получаем позицию курсора
            pos_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Поворачиваем корабль к позиции курсора
            transform.up = pos_mouse - new Vector2(transform.position.x, transform.position.y);
            //Проверяем нажатие на левую кнопку мыши
            if (Input.GetMouseButtonDown(0))
            {
                //Выстрел
                ShotEvent?.Invoke();
            }
            //Проверяем нажатие на правую кнопку мыши
            if (Input.GetMouseButtonDown(1))
            {
                //движение
                move(transform.up);
            }
        }
    }
    //Функция выстрела
    void shot()
    {
        //Создаем пулю
        GameObject Bullet = Instantiate(PrefabBullet);
        //Указываем ее начальные координаты соответствующие координатам корабля
        Bullet.transform.position = transform.position;
        //Задаем ей направление с заданной силой
        Bullet.GetComponent<Rigidbody2D>().AddForce(transform.up * PowerShot);
    }

    private void AfterMove()
    {
        for(int i = 0; i < _effectPositions.Length; i++)
        {
            ParticleSystem effect = Instantiate(_moveEffectPrefab);
            effect.transform.position = _effectPositions[i].position;
            effect.transform.rotation = transform.rotation;
            Destroy(effect.gameObject, effect.main.startLifetime.constant + effect.main.duration);
        }
    }

    private void OnDisable()
    {
        MoveEvent.RemoveListener(AfterMove);
    }
}
