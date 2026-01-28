using System.Collections;
using DG.Tweening;
using NewBagSystem;
using SKCell;
using UnityEngine;
using UnityEngine.Playables;

namespace Control
{
    public class FixCalControl : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        // 主要用于播放动画
        // [SerializeField] private Transform directorParent;
        // private readonly Dictionary<string, PlayableDirector> _anis = new();

        [SerializeField] private GameObject postcard;
        [SerializeField] private SKText textUI;
        private PlayableDirector _director;
        private Renderer _renderer;

        private void Start()
        {
            // var directors = directorParent.GetComponentsInChildren<PlayableDirector>(true);
            // foreach (var dir in directors)
            // {
            //     string key = dir.name;
            //     _anis.TryAdd(key, dir);
            // }
            if (!TryGetComponent(out _director))
                Debug.LogError("Can't get playable director");
            if (!postcard.TryGetComponent(out _renderer))
                Debug.LogError("Can't get renderer from postcard");
        }

        public IEnumerator Flow()
        {
            var toPlays = GameManager.instance.CookResult;
            foreach (var toPlay in toPlays)
            {
                // if (toPlay && _anis.TryGetValue(toPlay.id, out var director))
                // {
                //     director.Play();
                //     yield return new WaitForSeconds((float)director.duration);
                //     yield return null;
                // }
                if (toPlay)
                    yield return PlayPostcard(toPlay);
            }
            yield return null;
        }

        private Vector3 _postcardTargetScale;
        private IEnumerator PlayPostcard(Recipe recipe)
        {
            _renderer.material.SetTexture(MainTex, recipe.texture);
            textUI.text = recipe.textContent;
            _postcardTargetScale = recipe.scale;
            yield return null;
            
            _director.Play();
            yield return new WaitForSeconds((float)_director.duration);
            yield return null;
            postcard.transform.localScale = Vector3.zero;
        }

        public void StartScaleTween()
        {
            postcard.transform.DOScale(_postcardTargetScale, 0.5f).From(Vector3.one);
        }
        
        [ContextMenu("StartFlow")]
        private void StartFlow()
        {
            StartCoroutine(Flow());
        }

        [ContextMenu("StartPlayAllPostcards")]
        private void StartPlayAllPostcards()
        {
            StartCoroutine(PlayAllPostcards());
        }
        
        private IEnumerator PlayAllPostcards()
        {
            var toPlays = ItemManager.instance.GetAllRecipes();
            foreach (var toPlay in toPlays)
                yield return PlayPostcard(toPlay);
        }
    }
}