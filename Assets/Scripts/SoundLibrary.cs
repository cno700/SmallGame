using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    // 将各种音效存放起来，调用的时候直接输入名字就行，不用再去分配声音对象


    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();

    private void Awake()
    {
        foreach (SoundGroup soundGroup in soundGroups) {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);
        }
    }

    public AudioClip GetClipFromName (string name)
    {
        if (groupDictionary.ContainsKey(name))
        {
            AudioClip[] sounds = groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    // 不同音效组
    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
        

    }

}
