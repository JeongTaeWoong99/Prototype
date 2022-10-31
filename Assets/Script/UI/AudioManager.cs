using UnityEngine;

public class AudioManager : MonoBehaviour
{  
    public static AudioManager instance;

    public AudioSource levelMusic, gameOverMusic, winMusic; // Play on Awake = levelMusic
    public AudioSource[] sfx;                               // sfx 

    void Start()
    {
        instance = this;
    }

    public void PlayGameOver()
	{
        levelMusic.Stop();

        gameOverMusic.Play();
	}

    public void PlayLevelWin()
	{
		levelMusic.Stop();

        winMusic.Play();
	}

    public void PlaySFX(int sfxToPlay)
	{
		sfx[sfxToPlay].Stop();  
        sfx[sfxToPlay].Play(); 
	}

}