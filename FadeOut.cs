using System;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class FadeOut : MonoBehaviour
{
    //フェード用画像
    [SerializeField] public Image fedeImage;

    void Start()
    {
        Color color = gameObject.GetComponent<Image>().color;
        color.a = 1f;
        gameObject.GetComponent<Image>().color = color;
        sceneFadeOutImage(() => { });
    }

    //フェードアウト
    public void sceneFadeOutImage(Action onFadeEnd)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fedeImage.DOFade(0.0f, 1.0f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            onFadeEnd.Invoke();
        }));
    }
}
