using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Класс представляющий собой снаряд космического
//корабля, являющегося взрывающимся объектом
public class BulletController : ExplosionObj
{
    //переменная, определяющая находится снаряд
    //за экраном из-за того что он был выпущен там
    //или из-за того, что он уже за него вылетел
    bool shootIsExitScreen;
    private void Start()
    {
        //Если при создании объекта он
        //находился за экраном
        if(ExitScreen())
            //то переменная shootIsExitScreen равна true
            shootIsExitScreen = true;
        //Добавляем обработчик события смерти
        DieEvent.AddListener(die);
    }
    void die()
    {
        Destroy(gameObject);
    }
    //Функция для уничтожения пули, которая оказалась за экраном
    void OnExitScreen()
    {
        //Если патрон находится за экраном
        if (ExitScreen())
        {
            //Если он находится за экраном не 
            //из-за того что был выпущен там
            if (!shootIsExitScreen)
            {
                //То уничтожаем его
                Destroy(gameObject);
            }
        }
        //Если патрон находится в зоне видимости
        else
        {
            //То теперь при вылете за экран 
            //его можно уничтожить
            shootIsExitScreen = false;
        }
    }
    private void Update()
    {
        OnExitScreen();
    }
}
