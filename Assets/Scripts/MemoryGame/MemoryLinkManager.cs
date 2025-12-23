using Cysharp.Threading.Tasks;
using UnityEngine;

public class MemoryLinkManager : MonoBehaviour {
    [SerializeField] private MemoryLinkPopup memoryLinkPopup;

    public void Setup() {




    }


    public async UniTask ShowMemoryLinkPopup() {


        await memoryLinkPopup.SetInitializeAsync();

        // 選択されるか、閉じられるまで待機
        // channel 使う


    }


    public void HideMemoryLinkPopup() {


        memoryLinkPopup.ClosePopupProc();
    }
}