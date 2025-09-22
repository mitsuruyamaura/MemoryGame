using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GrowthSelectPopUp : PopUpBase {

    [SerializeField] private GrowthSlotGenerator growthSlotGenerator;
    [SerializeField] private Transform generateTran;

    private List<GrowthSlot> growthSlotList = new();



    public override void InitializePopUp() {
        base.InitializePopUp();

        // 追加でスロットを獲得する場合のポイント設定

    }

    public void OpenPopUpProc(List<StatusValue> statusValueList) {
        // 開くときにスロット生成
        GenerateGrowthSlots(statusValueList);

        OpenPopUp(token).Forget();
    }


    private void GenerateGrowthSlots(List<StatusValue> statusValueList) {
        for (int i = 0; i < statusValueList.Count; i++) {
            // スロットに要素と Action を渡す
            GrowthSlot growthSlot = (GrowthSlot)growthSlotGenerator.GetObjectFromPool(generateTran);
        }
    }


    public override UniTask OpenPopUp(CancellationToken token) {



        return base.OpenPopUp(token);
    }
}