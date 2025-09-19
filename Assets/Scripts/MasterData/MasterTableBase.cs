using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 各マスターに実装する
/// </summary>
public interface IMasterData {
    int Id { get; }
}

/// <summary>
/// アイコンID を利用したい各マスターに実装する
/// </summary>
public interface IHasIcon {
    string IconId { get; }
}

/// <summary>
/// マスターテーブルを非ジェネリックで扱えるようにする
/// </summary>
public interface IMasterTable {
    // テーブルのデータ型(例: typeof(ActiveSkillMaster))
    Type DataType { get; }

    // ボックス化した形で取得(object を返す)
    object GetDataBoxed(int id);

    int Count { get; }
}

/// <summary>
/// IMasterData インターフェースを実装しているマスターデータをテーブル化する抽象クラス
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MasterTableBase<T, U> : AbstractSingleton<U> , IMasterTable where T : IMasterData where U : Component {
    [SerializeField] protected List<T> list;
    public List<T> List => list; // Get専用プロパティ

    protected override void Awake() {
        if (instance == null) {
            instance = this as U;
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// PlayFab の TitleData をキャッシュする
    /// </summary>
    /// <param name="data"></param>
    public void Set(IEnumerable<T> data) {
        if (data == null) {
            Debug.LogWarning($"{typeof(T).Name} のデータが null です");
            return;
        }
        list = new List<T>(data); // List にデータをセット
    }

    /// <summary>
    /// 指定した ID のマスターデータ取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T GetData(int id) {
        return list.FirstOrDefault(masterData => masterData.Id == id);
    }

    /// <summary>
    /// 指定した ID のマスターデータがあるかチェック
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool TryGetData(int id, out T data) {
        data = list.FirstOrDefault(masterData => masterData.Id == id);
        return data != null;
    }

    /// <summary>
    /// リフレクションの GetValue でアクセスできるように IMasterTable のメソッドは明示的実装しておく
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    object IMasterTable.GetDataBoxed(int id) {
        return GetData(id);
    }

    Type IMasterTable.DataType => typeof(T);

    int IMasterTable.Count => list?.Count ?? 0;
}