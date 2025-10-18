using System.Collections;
using System.Collections.Generic;
using BagSystem;
using SKCell;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorManager : SKMonoSingleton<DirectorManager>
{
    public Dictionary<KeyValuePair<bool,ItemData>, PlayableDirector> directors;

    public void PlayDirector(bool isLong,ItemData itemData)
    {
        directors[new KeyValuePair<bool, ItemData>(isLong,itemData)].Play();
    }
}
