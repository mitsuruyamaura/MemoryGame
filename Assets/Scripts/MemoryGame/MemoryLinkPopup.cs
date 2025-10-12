using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MemoryLinkPopup : PopupBase {
    [SerializeField] protected ClassInfoViewGenerator classInfoViewGenerator;
    [SerializeField] protected Transform classInfoViewTran;

    private List<ClassInfoView> classInfoViewList = new();
    private int generateClassViewCount = 3;

    protected override void Awake() {
        classInfoViewGenerator.InitObjectPool();
        base.Awake();
    }

    public override async UniTask SetInitializeAsync(object param = null, UnityAction popupAction = null) {
        //if (param == null) {
        //    return;
        //}

        // 表示するマスターデータをキャスト
        List<IMasterData> masterDataList = (List<IMasterData>)param ?? new();

        // ClassInfoView 生成
        for (int i = 0; i < generateClassViewCount; i++) {
            ClassInfoView classInfoView = (ClassInfoView)classInfoViewGenerator.GetObjectFromPool(classInfoViewTran);
            IMasterData masterData = null;
            classInfoView.ShowClassInfoView(masterData, OnClickClassInfoView);
            classInfoViewList.Add(classInfoView);
        }

        await UniTask.Yield();

        base.SetInitialize();
    }


    protected void OnClickClassInfoView(IMasterData masterData) {        
        ChooseClassAsync(masterData).Forget();
    }


    protected async UniTask ChooseClassAsync(IMasterData masterData) {
        // 選択したクラスを登録


        // 演出


        await UniTask.Yield();



        // ポップアップを閉じる
        ClosePopupProc();
    }


    public override void ClosePopupProc(bool isSe = true) {
        classInfoViewList.ForEach(view => view?.Release());
        classInfoViewList.Clear();

        base.ClosePopupProc(isSe);
    }
}