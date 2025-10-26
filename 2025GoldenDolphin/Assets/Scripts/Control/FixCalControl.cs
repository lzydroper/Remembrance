using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Control
{
    public class FixCalControl : MonoBehaviour
    {
        // 主要用于播放动画
        [SerializeField] private Transform directorParent;
        private readonly Dictionary<string, PlayableDirector> _anis = new();

        private void Start()
        {
            var directors = directorParent.GetComponentsInChildren<PlayableDirector>(true);
            foreach (var dir in directors)
            {
                string key = dir.name;
                _anis.TryAdd(key, dir);
            }
        }

        public IEnumerator Flow()
        {
            var toPlays = GameManager.instance.CookResult;
            foreach (var toPlay in toPlays)
            {
                if (toPlay && _anis.TryGetValue(toPlay.id, out var director))
                {
                    director.Play();
                    yield return new WaitForSeconds((float)director.duration);
                    yield return null;
                }
            }
            yield return null;
        }
        
        [ContextMenu("StartFlow")]
        private void StartFlow()
        {
            StartCoroutine(Flow());
        }
    }
}