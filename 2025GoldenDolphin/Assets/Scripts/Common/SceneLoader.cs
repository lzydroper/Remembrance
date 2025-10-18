using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SKCell;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSinglenton<SceneLoader>
{
    [SerializeField] private GameObject transitionCanvas;
    [SerializeField] private Image transitionPanel;
    // [SerializeField] private Image transitionImage;

    [SerializeField] private float fadeTime = 1f;

    private Coroutine loadCoroutine;
    
    private Color color;
    
    // Start is called before the first frame update
    void Start()
    {
        // color = transitionImage.color;
        DontDestroyOnLoad(transitionCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(int sceneNum)
    {
        StartCoroutine(LoadCoroutine(sceneNum));
    }

    public void Load(string sceneName)
    {
        if (loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine);
        }
        Cursor.SetCursor(null,Vector2.zero, CursorMode.Auto);
        loadCoroutine = StartCoroutine(LoadCoroutine(sceneName));
    }

    IEnumerator LoadCoroutine(string sceneName)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;
        
        transitionCanvas.SetActive(true);

        transitionPanel.DOFade(1.0f, fadeTime);
        // transitionImage.gameObject.SetActive(true);
        // transitionImage.color =
        //     new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1.0f);
        
        while (loadingOperation.progress < 0.9f)
            yield return null;
        yield return new WaitForSeconds(1.0f);
        //
        loadingOperation.allowSceneActivation = true;
        // transitionImage.DOFade(0f, 1.0f);
        transitionPanel.DOFade(0.0f, fadeTime);

        yield return new WaitForSeconds(fadeTime);

        //Debug.Log("Load");

        // transitionImage.gameObject.SetActive(false);
        
        transitionCanvas.SetActive(false);
    }
    IEnumerator LoadCoroutine(int sceneNum)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(sceneNum);
        loadingOperation.allowSceneActivation = false;
        
        transitionCanvas.SetActive(true);

        transitionPanel.DOFade(1.0f, 1.0f);
        // transitionImage.gameObject.SetActive(true);
        // transitionImage.color =
            // new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1.0f);
        while (loadingOperation.progress < 0.9f)
            yield return null;
        yield return new WaitForSeconds(1.0f);
        //
        loadingOperation.allowSceneActivation = true;
        // transitionImage.DOFade(0f, 1.0f);
        transitionPanel.DOFade(0.0f, 1.0f);

        yield return new WaitForSeconds(1f);

        //Debug.Log("Load");

        // transitionImage.gameObject.SetActive(false);
        
        transitionCanvas.SetActive(false);
    }
    
}
