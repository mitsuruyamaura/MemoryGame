using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Card : PoolBase {
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;
    [SerializeField] private Button button;

    public int CardID { get; private set; }
    public bool IsFaceUp { get; private set; }

    private Action<Card> onSelected;

    public void Setup(int id, Action<Card> onSelectedCallback) {
        CardID = id;
        onSelected = onSelectedCallback;
        IsFaceUp = false;
        isReleased = false;

        disposable = button.OnClickExt(() => onSelected?.Invoke(this), this);

        frontImage.enabled = false;
    }

    public void Flip(bool faceUp) {
        IsFaceUp = faceUp;

        transform.DORotate(new Vector3(0, 90, 0), 0.15f)
            .OnComplete(() => {
                frontImage.enabled = faceUp;
                backImage.enabled = !faceUp;
                transform.DORotate(Vector3.zero, 0.15f);
            });
    }

    public void Hide() {
        transform.DOScale(Vector3.zero, 0.2f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}