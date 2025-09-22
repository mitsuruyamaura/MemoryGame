using Cysharp.Threading.Tasks;
using System;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GrowthSlot : PoolBase, IPoolable
{
    [SerializeField] private Button btnChoose;
    [SerializeField] private Image imgGrowthIcon;
    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtDescription;

    private CharaStatus charaStatus;
    private HoverButton hoverButton;

    public void SetUp(CharaStatus charaStatus, UnityAction<CharaStatus> chooseAction) {  // Title 用文字
        this.charaStatus = charaStatus;

        disposable = btnChoose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => chooseAction?.Invoke(charaStatus));

        // UI 設定

    }

    public override void Release() {
        disposable?.Dispose();
        objectPool.Release(this);
    }
}