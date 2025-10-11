using Cysharp.Threading.Tasks;
using System.Threading;

public interface ITrap {
    UniTask ExecuteAsync(TrapData trapData, CancellationToken token);
}
