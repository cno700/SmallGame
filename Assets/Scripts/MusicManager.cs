using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme; // ”Œœ∑±≥æ∞“Ù¿÷
    public AudioClip menuTheme; // ≤Àµ•±≥æ∞“Ù¿÷


    private void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme, 2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.instance.PlayMusic(mainTheme, 3);
        }
    }
}
