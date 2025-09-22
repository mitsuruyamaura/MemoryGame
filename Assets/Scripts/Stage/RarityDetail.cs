using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RarityDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgRarity;


    public void PlayAnim() {

        imgRarity.transform.DOScale(1.5f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => { imgRarity.transform.localScale = Vector3.one; } );
    }
}