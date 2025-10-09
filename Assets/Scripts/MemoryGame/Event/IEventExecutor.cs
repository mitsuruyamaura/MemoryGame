using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// イベントに実装するインターフェース
/// </summary>
public interface IEventExecutor {
    UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token);
}