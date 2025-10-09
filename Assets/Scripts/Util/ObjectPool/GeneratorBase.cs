using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 生成用基底クラス
/// 敵、フロート表示、アイテムなどに汎用的に利用する設計
/// </summary>
public abstract class GeneratorBase : MonoBehaviour {
    protected IObjectPool<PoolBase> objectPool;                 // オブジェクトプール

#pragma warning disable 0649
    [SerializeField] protected int initialPoolSize = 5;         // オブジェクトプールの初期サイズ
    [SerializeField] protected PoolBase objectPoolPrefab;       // プールさせるプレハブ
#pragma warning restore 0649


    /// <summary>
    /// 初期設定 
    /// </summary>
    /// <param name="entityObject"></param>
    public virtual void InitObjectPool() {
        // ObjectPool の初期化
        objectPool = new ObjectPool<PoolBase>(
            createFunc: () => Create(),
            actionOnGet: OnGetFromPool,
            actionOnRelease: target => target.gameObject.SetActive(false),
            actionOnDestroy: target => Destroy(target.gameObject),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 1000);
    }

    /// <summary>
    /// Get() メソッドにより、createFunc として実行される
    /// </summary>
    /// <returns></returns>
    protected virtual PoolBase Create() {
        PoolBase objectPoolInstance = Instantiate(objectPoolPrefab);

        // 参照を与えておく(依存性注入する)ことで、フロート表示側で Release できる
        objectPoolInstance.ObjectPool = objectPool;

        //Debug.Log("生成");
        return objectPoolInstance;
    }

    /// <summary>
    /// bulletPool.Get() メソッドにより、actionOnGet として実行される
    /// </summary>
    /// <param name="target"></param>
    protected virtual void OnGetFromPool(PoolBase target) {
        if (target == null || target.Equals(null)) {
            DebugLogger.Log("プールから破棄済みオブジェクトを取得しました");
            return;
        }
        
        target.gameObject.SetActive(true);
    }

    /// <summary>
    /// 外部クラスより実行する
    /// Get() メソッドにより、オブジェクトプールから弾を取り出して戻す → OnGetFromPool メソッドが実行される
    /// プール内に弾がない場合には新しく生成して戻す → Create メソッドが実行される
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public virtual PoolBase GetObjectFromPool(Vector3 position, Quaternion rotation) {
        PoolBase pooledObject = objectPool.Get();

        if (pooledObject == null || pooledObject.Equals(null)) {
            DebugLogger.Log("Poolから無効なオブジェクトを取得しました");
            pooledObject = Create();
        }

        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;

        //DebugLogger.Log("取得");
        return pooledObject;
    }

    /// <summary>
    /// 上記メソッドの UI オブジェクト生成用
    /// </summary>
    /// <param name="parentTran"></param>
    /// <returns></returns>
    public virtual PoolBase GetObjectFromPool(Transform parentTran) {
        PoolBase pooledObject = objectPool?.Get();

        if (pooledObject == null || pooledObject.Equals(null)) {
            DebugLogger.Log("Poolから無効なオブジェクトを取得しました");
            pooledObject = Create();
        }

        pooledObject.transform.SetParent(parentTran, false);
        pooledObject.transform.localScale = Vector3.one;

        //DebugLogger.Log("取得");
        return pooledObject;
    }
}