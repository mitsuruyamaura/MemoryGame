using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 各カードの実行処理を定義するインターフェース
/// Executor クラスに実装される
/// </summary>
public interface ICardExecutor {
    UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token);
}