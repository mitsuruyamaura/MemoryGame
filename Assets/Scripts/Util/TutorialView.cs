using UnityEngine;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private Image imgUnmask;


    public void SetUnmaskPosition(Vector3 position, Sprite sprite) {        
        imgUnmask.sprite = sprite;
        imgUnmask.rectTransform.sizeDelta = sprite.rect.size * 5; // Image を使って汎用的に使う場合には乗算する
        imgUnmask.rectTransform.position = position;
    }
}