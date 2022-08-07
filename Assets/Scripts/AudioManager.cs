using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music};

    float masterVolumePercent = .3f; // 主音量
    float sfxVolumePercent = 1; // 音效
    float musicVolumePercent = 1; // 背景音

    AudioSource sfx2DSource; // 将level completed音效变为2D音效
    AudioSource[] musicSources; // 创建两个音源，以实现切换音乐时的淡入淡出
    int activeMusicSourceIndex;

    public static AudioManager instance; // 设置该静态变量可以所有别的脚本组件轻松访问该脚本成员和方法

    Transform audioListener; // 取代 main camera 的listener，并让其始终跟随玩家位置
    Transform playerT;

    SoundLibrary library;

    private void Awake()
    {
        if (instance != null) // 理由见DontDestroyOnLoad()
        {
            Destroy(gameObject);
        }
        else // 疑问：假如走了上面的分支，那岂不是既销毁了又没生成？
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 加载新场景时不自动销毁该对象，但是切回该场景时会有两个对象，所以有了前面的判断并Destroy

            library = GetComponent<SoundLibrary>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            // 2D音源
            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>(); // 老师这里写成了newSfx2Dsource = ...应该是失误
            newSfx2Dsource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform; // 这里有疑惑：AudioListener不是组件么，为什么可以find
            playerT = FindObjectOfType<Player>().transform;

            // 记忆化
            masterVolumePercent = PlayerPrefs.GetFloat("master vol", masterVolumePercent);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", sfxVolumePercent);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", musicVolumePercent);
        }
    }

    private void Update()
    {
        if (playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume (float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
        }

        // 改变背景音乐大小
        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        // 保存改变值，使得下次加载时仍使用这个值（记忆）
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip; // 将另一个音源换成新的clip
        musicSources[activeMusicSourceIndex].Play(); // AudioSource.Play()可以在播放过程中改变音量大小，PlayAtPoint()不行

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));

    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    // 重载
    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0; 
        
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
