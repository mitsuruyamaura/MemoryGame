using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// メモリアランクアップ実行クラス
/// </summary>
public class MemoriaRankUpExecutor {
    private MemoryLinkManager memoryLinkManager;

    public MemoriaRankUpExecutor(MemoryLinkManager memoryLinkManager) {
        this.memoryLinkManager = memoryLinkManager;
    }

    public async UniTask ExecuteMemoriaRankUpAsync(MemoryStoneData memoryStoneData, CancellationToken token) {
        //DebugLogger.Log("MemoryFragments");
        SoundManager.instance.PlaySE(SE_TYPE.MemoryFragments);

        // 思い出の秘石の獲得数を加算、秘石をスロットにセット
        GameData.instance.AddMemoryStoneList(memoryStoneData);

        // メモリアランクアップの確認
        bool isRankUp = GameData.instance.CheckMemoriaRankUp();

        if (isRankUp) {
            // TODO 一旦コメントアウト
            // 獲得した思い出の秘石の情報をポップアップで表示
            //await memoryLinkManager.ShowMemoryLinkPopup();
        }

        await UniTask.Yield(token);
    }
}