using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource correctSound;
    public AudioSource wrongSound;

    public void PlayCorrectSound()
    {
        correctSound.Play();
    }

    public void PlayWrongSound()
    {
        wrongSound.Play();
    }
}
