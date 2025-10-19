using System;
using System.Collections;
using System.Collections.Generic;
using BagSystem;
using SKCell;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorManager : SKMonoSingleton<DirectorManager>
{
    public Dictionary<KeyValuePair<bool, ItemData>, PlayableDirector> directors =
        new Dictionary<KeyValuePair<bool, ItemData>, PlayableDirector>();
    [SerializeField] private List<PlayableDirector> Directors; 
    [SerializeField] private List<ItemData> items; 
    private void Start()
    {
        for(int i=0;i<items.Count;i++)
        {
            directors[new KeyValuePair<bool, ItemData>(i % 2 == 0, items[i])] = Directors[i];
        }
    }

    public void PlayDirector(bool isLong,ItemData itemData)
    {
        KeyValuePair<bool, ItemData> key = new KeyValuePair<bool, ItemData>(isLong, itemData);
        if(directors.ContainsKey(key))
            directors[key].Play();
        else
        {
            SKUtils.EditorLogNormal("There is no shit.");
        }
    }
}
