using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Класс представляющий вражеский космический корабль
public class EnemySpaceShip : SpaceShip, Enemy
{
    //Ссылка на игрока
    public PlayerSpaceShip Player;
    private void Awake()
    {
        //Инициализируем компоненты
        Inic();
        //Задаем цвет стрелке
        Arrow.color = Color.red;
    }
    void Update()
    {
        //Если игрок существует (еще не умер)
        if (Player)
        {
            //Переменная для позиции игрока
            Vector2 pos_player = new Vector2();
            //Получаем позицию игрока
            pos_player = Player.transform.position;
            //Нло движется к нашему кораблю
            move((pos_player - new Vector2(transform.position.x,
                transform.position.y)).normalized*Time.deltaTime);
        }
    }
    //Реализация метода die интерфейса Enemy
    public void DieEnemy()
    {
        DieEvent?.Invoke();
        //Чистим событие DieEvent, чтобы не было повторных вызовов
        //при столкновении двух врагов
        DieEvent.RemoveAllListeners();
    }
}
