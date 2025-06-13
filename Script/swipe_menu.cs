using UnityEngine;
using UnityEngine.UI;

public class swipe_menu : MonoBehaviour
{
    public GameObject scrollbar;
    float scroll_pos = 0;
    float[] pos;
    float distance;

    void Start()
    {
        scroll_pos = scrollbar.GetComponent<Scrollbar>().value;

        // Ba�lang��ta t�m boyutunu normalle�tir
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localScale = Vector3.one;
        }
    }

    void Update()
    {
        int childCount = transform.childCount;
        pos = new float[childCount];
        distance = 1f / (childCount - 1f);

        for (int i = 0; i < childCount; i++)
        {
            pos[i] = distance * i;
        }

        // Kayd�rma yap�l�yorsa scroll pozisyonunu al
        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            // En yak�n pozisyona yumu�ak ge�i�
            for (int i = 0; i < childCount; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2f) && scroll_pos > pos[i] - (distance / 2f))
                {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        // Butonlar� b�y�tme/k���ltme kontrol�
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (Mathf.Abs(pos[i] - scroll_pos) < distance / 2f)
            {
                // SADECE merkeze en yak�n olan� b�y�t
                child.localScale = Vector2.Lerp(child.localScale, new Vector2(1.2f, 1.2f), 0.1f);
            }
            else
            {
                // Di�erleri orijinal boyutta kals�n
                child.localScale = Vector2.Lerp(child.localScale, new Vector2(1f, 1f), 0.1f);
            }
        }
    }
}
