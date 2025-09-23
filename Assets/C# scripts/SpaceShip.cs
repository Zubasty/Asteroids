using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Абстрактный класс представляющий космический корабль
public abstract class SpaceShip : ExplosionObj
{
    //"Сила" движения
    public float PowerMove;
    //Ссылка на спрайт стрелки, используемой при вылете
    //корабля за экран
    public SpriteRenderer Arrow;
    //Объект RigidBody
    new Rigidbody2D rigidbody;
    //Функция инициализации компонентов космического корабля
    private protected void Inic()
    {
        //Получаем компонент RigidBody
        rigidbody = GetComponent<Rigidbody2D>();
        //Создаем стрелку
        Arrow = Instantiate(Arrow);
        //Добавляем обработчик события смерти
        DieEvent.AddListener(die);
    }
    void LateUpdate()
    {
        //Если вылетели за границу экрана
        if(ExitScreen())
        {
            //включаем, перемещаем и поворачиваем стрелку
            ArrowEnable();
        }
        //Если находимся в границах камеры
        else
        {
            //выключаем отображение стрелки
            Arrow.enabled = false;
        }
    }
    //Функция для корректного отображения стрелки
    void ArrowEnable()
    {
        //Включаем стрелку
        Arrow.enabled = true;
        //Создаем переменную для определения позиции стрелки
        Vector2 new_pos = new Vector2();

        new_pos.x = pos_arrow(transform.position.x, 
            cam_width/2, cam_pos.x);
        new_pos.y = pos_arrow(transform.position.y, 
            cam_height/2, cam_pos.y);
        Arrow.transform.position = new_pos;
        Arrow.transform.up = transform.position - Arrow.transform.position;
    }
    //Функция для определения одной координаты позиции стрелки
    //принимает на вход координату корабля, разность 
    //половины ширины/высоты камеры и половины стрелки 
    //(в зависимости от того x это или y), координату камеры
    float pos_arrow(float old_сoor, float size_cam, float coord_cam)
    {
        //Если координата корабля больше чем необходимая координата
        //чтобы не вылететь за камеру
        if(Mathf.Abs(old_сoor) > size_cam)
        {
            //Если мы слева/снизу от камеры
            if(old_сoor<coord_cam)
            {
                //то двигаем стрелку к краю и немного
                //"задвигаем" ее в камеру
                return 0.5f - size_cam;
            }
            //Если мы сверху/справа
            else
            {
                //то двигаем стрелку к краю и немного
                //"задвигаем" ее в камеру
                return size_cam - 0.5f;
            }
        }
        //Если же мы по этой координате не находимся "за" камерой
        else
        {
            //То координата стрелки совпадает с координатой корабля
            return old_сoor;
        }
    }
    //Функция движения
    private protected void move(Vector2 vec)
    {
        //Задаем кораблю направление движения
        rigidbody.AddForce(vec * PowerMove);
    }
    void die()
    {
        //Уничтожаем корабль
        Destroy(gameObject);
        //Уничтожаем еще и стрелку
        Destroy(Arrow.gameObject);
    }
}
