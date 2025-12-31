using Cysharp.Threading.Tasks;
using System.Threading;

public interface ITrapEffect {
    UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token);
}