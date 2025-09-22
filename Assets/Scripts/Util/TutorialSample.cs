using UnityEngine;
using UnityEngine.UI;

public class TutorialSample : MonoBehaviour
{
    [SerializeField] private TutorialView tutorialView;
    [SerializeField] private Image imgButton;


    void Start() {
        tutorialView.SetUnmaskPosition(imgButton.rectTransform.position, imgButton.sprite);
    }
}