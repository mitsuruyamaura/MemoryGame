using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class ResourcesLoader {
    private static Dictionary<string, AsyncOperationHandle<Sprite>> handleCache = new();  // ロードした画像のキャッシュ用

    private static AsyncOperationHandle handle;
    // TODO 他にもキャッシュ用のデータが増えたら Dictionary を追加(ゲームオブジェクトなど)


    public static async UniTask SetUpResourcesLoader() {
        // Addressables の初期化を確認
        try {
            handle = Addressables.InitializeAsync();
            await handle.ToUniTask();
            if (handle.Status != AsyncOperationStatus.Succeeded) {
                DebugLogger.Log("Addressables Initialization Failed: " + handle.Status);
            }
        } catch (Exception ex) {
            DebugLogger.Log("Addressables Initialization Exception: " + ex);
        }
    }

    /// <summary>
    /// 指定したアドレスの画像を Addressables より非同期ロード
    /// Addressables に存在しない場合は Resources.Load を試す
    /// </summary>
    public static async UniTask<Sprite> LoadSpriteAsync(string address, bool forceUseResources = false) {
        // Resources.Load を強制的に使用する場合
        if (forceUseResources) {
            return Resources.Load<Sprite>(address);
        }

        // Addressables からキャッシュ済の画像であるかチェックして、キャッシュ済ならそれを返す(重複読み込みをなくす)
        if (handleCache.TryGetValue(address, out var cachedHandle)) {
            return cachedHandle.Result;
        }

        // Addressables にあるかチェック
        bool exists = await CheckAddressExistsAsync(address);

        // Addressables にある場合には Addressables からロード
        if (exists) {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                // キャッシュに保存
                handleCache[address] = handle;
                return handle.Result;
            } else {
                DebugLogger.Log($"Addressables failed to load: {address}");
                if (handle.OperationException != null) {
                    DebugLogger.Log($"Error Message: {handle.OperationException.Message}");
                    DebugLogger.Log($"Stack Trace: {handle.OperationException.StackTrace}");
                }
            }
        }

        DebugLogger.Log($"Addressables にない場合は Resources.Load を試す");

        // Addressables にない場合は Resources.Load を試す
        return Resources.Load<Sprite>(address);
    }

    /// <summary>
    /// 指定したアドレスが Addressables に存在するか非同期チェック
    /// </summary>
    private static async UniTask<bool> CheckAddressExistsAsync(string address) {
        // ロケーション情報を取得
        AsyncOperationHandle<IList<IResourceLocation>> locationHandle = Addressables.LoadResourceLocationsAsync(address);
        await locationHandle.ToUniTask();

        if (locationHandle.Result == null) {
            DebugLogger.Log($"locationHandle.Result: {locationHandle.Result}");
            return false;
        }

        // handle.Status が AsyncOperationStatus.Succeeded かつ、handle.Result.Count が 0 以上なら Addressables に存在しているので true
        bool exists = locationHandle.Status == AsyncOperationStatus.Succeeded && locationHandle.Result.Count > 0;

        // ロケーション情報は使い終わったらメモリ解放していい
        Addressables.Release(locationHandle);

        return exists;
    }

    /// <summary>
    /// すべての画像のキャッシュデータを解放
    /// OnApplicationQuit などから実行
    /// </summary>
    public static void ReleaseAllSprites() {
        foreach (var handle in handleCache.Values) {
            Addressables.Release(handle);
        }
        handleCache.Clear();
    }

    /// <summary>
    /// 指定した画像のキャッシュデータを解放
    /// </summary>
    /// <param name="address"></param>
    public static void ReleaseSprite(string address) {
        if (handleCache.TryGetValue(address, out var handle)) {
            Addressables.Release(handle);
            handleCache.Remove(address);
        }
    }
}