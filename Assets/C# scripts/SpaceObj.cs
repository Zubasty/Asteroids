using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//Класс представляющий космический объект
public abstract class SpaceObj : MonoBehaviour
{
    //Событие смерти
    public UnityEvent DieEvent = new UnityEvent();
    //Переменная для определения позиции камеры
    public static Vector2 cam_pos;
    //Переменная для определения высоты камеры
    public static float cam_height;
    //Переменная для определения ширины камеры
    public static float cam_width;
    //Функция инициализации позиции, высоты и ширины камеры
    public static void InicStaticCam()
    {
        //Получение позиции камеры
        cam_pos = Camera.main.transform.position;
        //Получение высоты камеры
        cam_height = Camera.main.orthographicSize*2;
        //Получение ширины камеры
        cam_width = Screen.width * Camera.main.orthographicSize * 2 / Screen.height;
    }
    //Функция для определения находится ли объект вне камеры
    private protected bool ExitScreen()
    {
        //Возвращаем true если объект вышел за камеру
        return Mathf.Abs(transform.position.x - cam_pos.x) > cam_width/2
            || Mathf.Abs(transform.position.y - cam_pos.y) > cam_height/2;
    }
    //Функция для определения случайной позиции внутри камеры
    public static Vector2 RandomPositionInCamera()
    {
        //Координата x
        float x = 0;
        //Координата y
        float y = 0;
        //С вероятность 50%
        if (Random.value > 0.5f)
        {
            //Задаем случайно координату x
            x = Camera.main.transform.position.x +
                Random.Range(-cam_width / 2 + 0.01f, cam_width / 2 - 0.01f);
            //И определяем координату y возле края камеры
            y = Random.value > 0.5f ? -cam_height / 2 + 0.01f : cam_height / 2 - 0.01f;
        }
        else
        {
            //Определяем координату x возле края камеры
            x = Random.value > 0.5f ? -cam_width / 2 + 0.01f : cam_width / 2 - 0.01f;
            //Задаем случайно координату y
            y = Camera.main.transform.position.y +
                Random.Range(-cam_height / 2 + 0.01f, cam_height / 2 - 0.01f);

        }
        //Возвращаем позицию x, y
        return new Vector2(x, y);
    }
}
