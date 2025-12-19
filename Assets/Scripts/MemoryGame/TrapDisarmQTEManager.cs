using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// 罠解除用のQTEマネージャー
/// </summary>
public class TrapDisarmQTEManager : AbstractSingleton<TrapDisarmQTEManager> {
    [SerializeField] private PlayerInputUI playerInputUI;
    [SerializeField] private CorrectSequenceUI correctSequenceUI;

    private List<QTESymbol> correctSequenceList = new();   // 正解シーケンス
    private List<QTESymbol> playerSequenceList = new();    // プレイヤー入力シーケンス

    private float timeLimit;
    private bool isFinished;
    private bool isSuccess;
    private StageUIManager stageUIManager;

    public void SetUp(StageUIManager stageUIManager) {
        this.stageUIManager = stageUIManager;
    }

    /// <summary>
    /// 罠解除QTEを開始する
    /// </summary>S
    /// </summary>
    /// <param name="symbolTypeCount">2～4</param>
    /// <param name="sequenceLength">2～6</param>
    /// <param name="limitSeconds">難易度依存</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask<bool> StartTrapDisarmQTEAsync(int symbolTypeCount, int sequenceLength, float limitSeconds, CancellationToken token) {
        Setup(symbolTypeCount, sequenceLength, limitSeconds);

        isFinished = false;
        isSuccess = false;

        float timer = 0f;

        stageUIManager.ShowTimeCanvasTrapDisarm();

        while (!isFinished) {
            if (token.IsCancellationRequested) {
                Failure();
                break;
            }

            timer += Time.deltaTime;
            stageUIManager.UpdateTimeCanvasTrapDisarm(timeLimit, timer);

            if (timer >= timeLimit) {
                Failure();
                break;
            }

            await UniTask.Yield(token);
        }

        stageUIManager.HideTimeCanvasTrapDisarm();
        return isSuccess;
    }

    /// <summary>
    /// 毎回のQTE用のセットアップ
    /// </summary>
    /// <param name="symbolTypeCount"></param>
    /// <param name="sequenceLength"></param>
    /// <param name="limitSeconds"></param>
    private void Setup(int symbolTypeCount, int sequenceLength, float limitSeconds) {
        timeLimit = limitSeconds;
        playerSequenceList.Clear();

        // 正解シンボル生成
        correctSequenceList = GenerateSequence(symbolTypeCount, sequenceLength);

        // プレイヤー入力用シンボルの生成。毎回ランダムな順番にする
        List<QTESymbol> shuffledSequence =
            System.Enum.GetValues(typeof(QTESymbol))      // QTESymbol の全列挙子を取得
                .Cast<QTESymbol>()                        // IEnumerable<QTESymbol> にキャスト
                .OrderBy(_ => UnityEngine.Random.value)   // ランダムに並び替え
                .ToList();

        // シンボルのボタン作成
        playerInputUI.GenerateRandomPlayerInputSymbols(shuffledSequence, OnPressSymbol);

        correctSequenceUI.Set(correctSequenceList);
    }

    /// <summary>
    /// 正解シーケンスを生成する
    /// </summary>
    /// <param name="symbolTypeCount"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private List<QTESymbol> GenerateSequence(int symbolTypeCount, int length) {
        List<QTESymbol> list = new();

        for (int i = 0; i < length; i++) {
            int rand = Random.Range(0, symbolTypeCount);
            list.Add((QTESymbol)rand);
        }

        return list;
    }

    /// <summary>
    /// シンボルボタンから呼ばれる
    /// </summary>
    public void OnPressSymbol(QTESymbol symbol) {
        if (isFinished) return;

        // 正解したシンボルの数(最初は 0)
        int index = playerSequenceList.Count;

        // 順番チェック
        if (correctSequenceList[index] != symbol) {
            ResetInput();
            return;
        }

        // 正解したシンボルをUIに反映
        correctSequenceUI.SuccessSymbol(index);

        // 正解した回答のプレイヤー入力を記録
        playerSequenceList.Add(symbol);

        if (playerSequenceList.Count == correctSequenceList.Count) {
            Success();
        }
    }

    private void ResetInput() {
        correctSequenceUI.ResetAllSymbols();
        playerSequenceList.Clear();
    }

    private void Success() {
        isFinished = true;
        isSuccess = true;

        stageUIManager.SuccessTrapDisarmInfo();
        SoundManager.instance.PlaySE(SE_TYPE.TrapDisarm);
    }

    private void Failure() {
        isFinished = true;
        isSuccess = false;

        stageUIManager.FailureTrapDisarmInfo();
        SoundManager.instance.PlaySE(SE_TYPE.Miss);
    }
}