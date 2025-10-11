using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class ItemInfoDisplayManager : AbstractSingleton<ItemInfoDisplayManager> {
    [SerializeField] private CanvasGroup itemInfoCanvas;

    [SerializeField] private InfoViewGenerator itemInfoViewGenerator;
    [SerializeField] private Transform backpackItemTran;
    [SerializeField] private Transform TeasureChestItemTran;
    [SerializeField] private Button btnFilter;
    [SerializeField] private CanvasGroup cgFilter;
    private IDisposable disposable;
    private ItemInfoView baclpackItemInfoView;   // 表示しているバックパックアイテム View

    [SerializeField] private Image imgBagIcon;
    [SerializeField] private Ease bagEase;

    [SerializeField] private Sprite[] treasureBoxSprite;
    [SerializeField] private Image imgTreasureBoxIcon;
    [SerializeField] private Ease treasureBoxEase;

    public bool isTreasureShow;

    [SerializeField] private InfoViewGenerator blessingInfoViewGenerator;
    [SerializeField] private Transform blessingInfoViewTran;


    protected override void Awake() {
        base.Awake();
        HideItemInfo();
        cgFilter.blocksRaycasts = false;
        cgFilter.alpha = 0f;

        itemInfoViewGenerator.InitObjectPool();
        blessingInfoViewGenerator.InitObjectPool();
    }

    /// <summary>
    /// ホバーしたバックパック内のアイテムの情報表示
    /// </summary>
    /// <param name="backPackInItem"></param>
    public void ShowItemInfo(BackPackInItem backPackInItem) {
        // 最初だけ生成して保持しておく
        if(baclpackItemInfoView == null) {
            baclpackItemInfoView = (ItemInfoView)itemInfoViewGenerator.GetObjectFromPool(backpackItemTran);
        }

        // バックパック内のアイテムの情報表示        
        baclpackItemInfoView.ShowBackPackItemInfo(backPackInItem);

        ShowItemInfo();
    }

    /// <summary>
    /// アイテムキャンバス表示
    /// </summary>
    public void ShowItemInfo() {
        if (this == null || itemInfoCanvas == null) {
            return;
        }
        itemInfoCanvas.alpha = 1.0f;
    }

    /// <summary>
    /// アイテムキャンバス非表示
    /// </summary>
    public void HideItemInfo() {
        if (this == null || itemInfoCanvas == null) {
            return;
        }
        itemInfoCanvas.alpha = 0;
        baclpackItemInfoView?.HideInfoView();
    }

    /// <summary>
    /// 袋の表示演出
    /// </summary>
    public void ShowBagIcon() {
        imgBagIcon.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(imgBagIcon.DOFade(1.0f, 0.25f).SetEase(Ease.InQuart));
        sequence.Join(imgBagIcon.transform.DOScale(Vector3.one, 0.25f).SetEase(bagEase));
        sequence.Join(imgBagIcon.transform.DOLocalJump(new(0, 0, 0), 25f, 2, 0.5f).SetEase(bagEase).SetRelative());
        sequence.AppendInterval(0.5f).OnComplete(() => {
            imgBagIcon.gameObject.SetActive(false);
            imgBagIcon.color = new(1, 1, 1, 0);
            imgBagIcon.transform.localPosition = Vector3.zero;
        });
    }

    /// <summary>
    /// 宝箱の表示演出
    /// </summary>
    /// <param name="rarity"></param>
    public void ShowTreasureBoxIcon(Rarity rarity) {
        int index = (int)rarity;

        Sprite sprite = treasureBoxSprite[index];
        imgTreasureBoxIcon.sprite = sprite;

        imgTreasureBoxIcon.gameObject.SetActive(true);
        imgTreasureBoxIcon.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(imgTreasureBoxIcon.DOFade(1.0f, 0.25f).SetEase(Ease.InQuart));
        sequence.Join(imgTreasureBoxIcon.transform.DOScale(Vector3.one * 1.3f, 0.25f).SetEase(treasureBoxEase));
        sequence.Join(imgTreasureBoxIcon.transform.DOLocalJump(new(0, 0, 0), 200f, 1, 0.5f).SetEase(treasureBoxEase).SetRelative());
        sequence.Append(imgTreasureBoxIcon.transform.DOScale(Vector3.one, 0.25f).SetEase(treasureBoxEase));
        sequence.AppendInterval(0.5f).OnComplete(() => {
            imgTreasureBoxIcon.gameObject.SetActive(false);
            imgTreasureBoxIcon.color = new(1, 1, 1, 0);
            imgTreasureBoxIcon.transform.localPosition = Vector3.zero;
        });
    }

    /// <summary>
    /// 入手したアイテムの表示
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask ShowTreasureItemInfoAsync(ItemData itemData, CancellationToken token) {
        isTreasureShow = true;

        await UniTask.Delay(1000);

        // 宝箱カードの情報表示
        ItemInfoView treasureChestItemInfoView = (ItemInfoView)itemInfoViewGenerator.GetObjectFromPool(TeasureChestItemTran);
        treasureChestItemInfoView.ShowTeasureChestInfo(itemData);

        ShowItemInfo();

        // 画面タップするまで待機(ほかの UI には触らないようにする)
        bool isTouch = false;
        cgFilter.blocksRaycasts = true;
        cgFilter.alpha = 1.0f;
        disposable = btnFilter.OnClickExt(() => isTouch = true, this);

        await UniTask.WaitUntil(() => isTouch == true, cancellationToken: token);

        treasureChestItemInfoView?.Release();
        disposable.Dispose();
        cgFilter.blocksRaycasts = false;
        cgFilter.alpha = 0f;

        isTreasureShow = false;
        HideItemInfo();
    }

    /// <summary>
    /// イベントカード獲得時の画面表示
    /// </summary>
    /// <param name="blessingData"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask ShowBlessingInfoAsync(BlessingData blessingData, CancellationToken token) {
        // イベントカードの情報表示
        BlessingInfoView blessingInfoView = (BlessingInfoView)blessingInfoViewGenerator.GetObjectFromPool(blessingInfoViewTran);
        blessingInfoView.ShowBleesingInfo(blessingData);

        // 画面タップするまで待機(ほかの UI には触らないようにする)
        bool isTouch = false;
        cgFilter.blocksRaycasts = true;
        cgFilter.alpha = 1.0f;

        disposable = btnFilter.OnClickExt(() => isTouch = true, this);

        await UniTask.WaitUntil(() => isTouch == true, cancellationToken: token);

        blessingInfoView?.Release();
        disposable.Dispose();
        cgFilter.blocksRaycasts = false;
        cgFilter.alpha = 0f;

        HideItemInfo();
    }
}