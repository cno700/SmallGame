using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music};

    float masterVolumePercent = .3f; // ������
    float sfxVolumePercent = 1; // ��Ч
    float musicVolumePercent = 1; // ������

    AudioSource sfx2DSource; // ��level completed��Ч��Ϊ2D��Ч
    AudioSource[] musicSources; // ����������Դ����ʵ���л�����ʱ�ĵ��뵭��
    int activeMusicSourceIndex;

    public static AudioManager instance; // ���øþ�̬�����������б�Ľű�������ɷ��ʸýű���Ա�ͷ���

    Transform audioListener; // ȡ�� main camera ��listener��������ʼ�ո������λ��
    Transform playerT;

    SoundLibrary library;

    private void Awake()
    {
        if (instance != null) // ���ɼ�DontDestroyOnLoad()
        {
            Destroy(gameObject);
        }
        else // ���ʣ�������������ķ�֧�������Ǽ���������û���ɣ�
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �����³���ʱ���Զ����ٸö��󣬵����лظó���ʱ��������������������ǰ����жϲ�Destroy

            library = GetComponent<SoundLibrary>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            // 2D��Դ
            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>(); // ��ʦ����д����newSfx2Dsource = ...Ӧ����ʧ��
            newSfx2Dsource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform; // �������ɻ�AudioListener�������ô��Ϊʲô����find
            playerT = FindObjectOfType<Player>().transform;

            // ���仯
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

        // �ı䱳�����ִ�С
        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        // ����ı�ֵ��ʹ���´μ���ʱ��ʹ�����ֵ�����䣩
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip; // ����һ����Դ�����µ�clip
        musicSources[activeMusicSourceIndex].Play(); // AudioSource.Play()�����ڲ��Ź����иı�������С��PlayAtPoint()����

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));

    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    // ����
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
