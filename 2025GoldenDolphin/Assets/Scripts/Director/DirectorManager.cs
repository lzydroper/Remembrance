using System;
using System.Collections;
using System.Collections.Generic;
using BagSystem;
using SKCell;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorManager : SKMonoSingleton<DirectorManager>
{
    public Dictionary<KeyValuePair<bool,ItemData>, PlayableDirector> directors;
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
        directors[new KeyValuePair<bool, ItemData>(isLong,itemData)].Play();
    }
}
