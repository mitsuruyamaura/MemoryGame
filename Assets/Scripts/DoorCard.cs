using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DoorCard : CardBase {
    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Door");

        await base.ExecuteCardAsync(token);
    }
}