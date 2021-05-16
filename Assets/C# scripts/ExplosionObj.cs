using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Класс представляющий взрывающиеся объекты,
//являющиеся космическими объектами
public abstract class ExplosionObj : SpaceObj
{
    public static bool isOnSoundExp = true;
    public AudioClip SoundExplosion;
    //Ссылка на префаб взрыва
    public GameObject ExplosionPrefab;
    //Активация взрыва
    public void explosionActivate(Vector2 pos)
    {
        //Создаем экземпляр взрыва
        GameObject exp = Instantiate(ExplosionPrefab);
        //Перемещаем его к текущему объекту
        exp.transform.position = pos;
        //Уничтожаем текущий объект
        DieEvent?.Invoke();
        //Чистим событие DieEvent, чтобы не было повторных вызовов
        //при столкновении двух врагов
        DieEvent.RemoveAllListeners();
        if(isOnSoundExp)
        {
            AudioSource Audio = exp.AddComponent<AudioSource>();
            Audio.clip = SoundExplosion;
            Audio.Play();
            exp.GetComponent<ExplosionController>().LetsGo(Audio);
        }
        else
        {
            Destroy(exp.gameObject, 1);
        }
    }
    //Функция столкновения взрывающегося объекта с другим объектом
    private void CollisionFunc(Collision2D collision)
    {
        //Если столкнулись с врагом
        if (collision.gameObject.GetComponent<SpaceObj>() is Enemy)
        {
            {
                //Уничтожаем врага
                Enemy e = (Enemy)collision.gameObject.GetComponent<SpaceObj>();
                e.DieEnemy();
                //Активируем взрыв в точке столкновения
                explosionActivate(collision.contacts[0].point);
            }
        }
    }
    //Столкновение объекта с другими объектами
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Вызываем функцию столкновения
        CollisionFunc(collision);
    }
}
//Интерфейс для врагов
public interface Enemy
{
    //Функция смерти, которую реализуют все враги
    void DieEnemy();
}