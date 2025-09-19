using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// オブジェクトプール化するための汎用クラス
/// </summary>
public abstract class PoolBase : MonoBehaviour, IPoolable {

    protected IObjectPool<PoolBase> objectPool;
    protected IDisposable disposable;
    protected bool isReleased = false;                    // オブジェクトプール用フラグ

    // ObjectPool への参照を与えるプロパティ
    public IObjectPool<PoolBase> ObjectPool { get => objectPool; set => objectPool = value; }

    public virtual void Release() {
        // オブジェクトが存在していないときには処理しない
        if (this == null || gameObject == null || !gameObject.activeInHierarchy) {
            return;
        }

        // すでに戻っているときには処理しない
        if (isReleased) {
            return;
        }

        ((RectTransform)transform).anchorMin = new Vector2(0.5f, 0.5f);
        ((RectTransform)transform).anchorMax = new Vector2(0.5f, 0.5f);

        isReleased = true;

        disposable?.Dispose();
        disposable = null;

        objectPool.Release(this);
    }
}