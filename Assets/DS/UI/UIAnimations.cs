using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

public class UIAnimations : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeTime = 1f;
    [SerializeField] RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Start()
    {

    }

    public void FadeOut(EventArgument e)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
#if DOTWEEN
        canvasGroup.DOFade(0, fadeTime ).OnComplete(() =>
        {
            if (e != null)
                e.events?.Invoke();
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(false);
        });

#elif LEAN_TWEEN
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeTime).setOnComplete(() =>
          {
              if(e != null)
                 e.events?.Invoke();
              canvasGroup.blocksRaycasts = true;
              gameObject.SetActive(false);
          });
#else
    Debug.Log("No tween engine found, TODO: Add Coroutine fallback  for FadeOut(EventArgument)");
#endif
    }

    public void FadeOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
#if DOTWEEN
        canvasGroup.DOFade(0, fadeTime).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(false);
        });

#elif  LEAN_TWEEN
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeTime).setOnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(false);

        });
#else
    Debug.Log("No tween engine found, TODO: Add Coroutine fallback for FadeOut()");
#endif
    }

    public void MoveInWidth()
    {
#if LEAN_TWEEN
        rectTransform.LeanMoveX(-rectTransform.sizeDelta.x, fadeTime);
#endif
    }

    public void MoveOutWidth()
    {
#if LEAN_TWEEN
        rectTransform.LeanMoveX(0f, fadeTime);
#endif
    }

    public void FadeIn()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;

#if DOTWEEN
        canvasGroup.DOFade(1, fadeTime).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });

#elif LEAN_TWEEN
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeTime).setOnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
#else
    Debug.Log("No tween engine found, TODO: Add Coroutine fallback for FadeIn()");
#endif
    }

}

