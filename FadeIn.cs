using System;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class FadeIn : MonoBehaviour
{
    //フェード用画像
    [SerializeField] public Image fedeImage;

    void Start()
    {
        sceneFadeIn(() => { });
    }

    //フェードイン
    public void sceneFadeIn(Action onFadeEnd)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fedeImage.DOFade(1.0f, 1.0f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            onFadeEnd.Invoke();
        }));
    }
}
