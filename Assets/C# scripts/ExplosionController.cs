using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Класс для уничтожения объекта взрыва
public class ExplosionController : MonoBehaviour
{
    //Функция, чтобы принять объект звука
    public void LetsGo(AudioSource Audio)
    {
        //Запускаем корутин уничтожения объекта
        StartCoroutine(DieSoundObj(Audio));
    }
    //Корутин уничтожения звукового объекта
    static IEnumerator DieSoundObj(AudioSource Audio)
    {
        //Узнаем время проигрывания звука
        float time = Audio.clip.length;
        //Ждем до тех пор пока время проигрывания не выйдет 
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        //Уничтожаем звуковой объект
        Destroy(Audio.gameObject);
        yield return null;
    }
}
