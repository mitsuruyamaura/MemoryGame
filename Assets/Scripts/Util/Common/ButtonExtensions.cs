using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using System;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtensions {

    /// <summary>
    /// OnClick の購読処理の拡張メソッド
    /// </summary>
    /// <param name="button"></param>
    /// <param name="onClick">ボタン押下時の処理</param>
    /// <param name="owner">AddTo に紐づけるコンポーネント</param>
    /// <param name="throttleTime">ボタンの再有効化までの待機時間。指定なければ1秒待機</param>
    /// <returns></returns>
    public static IDisposable OnClickExt(this Button button, Action onClick, Component owner, TimeSpan? throttleTime = null) {
        // ボタンの再有効化までの待機時間。引数指定がない場合には初期値(1秒)を設定
        TimeSpan time = throttleTime ?? TimeSpan.FromMilliseconds(1000);

        // 処理をまとめておく
        IDisposable subscription = button.OnClickAsObservable()
            .ThrottleFirst(time)
            .Subscribe(_ => onClick?.Invoke());

        // AddTo 用の紐づけ対象を受け取る(主に this が来る)
        if (owner != null) {
            subscription.AddTo(owner);
        }

        return subscription;
    }

    /// <summary>
    /// OnClick の非同期型の購読処理の拡張メソッド
    /// </summary>
    /// <param name="button"></param>
    /// <param name="onClickAsync"></param>
    /// <param name="owner"></param>
    /// <param name="throttleTime"></param>
    /// <returns></returns>
    public static IDisposable OnClickExtAsync(this Button button, Func<UniTask> onClickAsync, Component owner, TimeSpan? throttleTime = null) {
        // ボタンの再有効化までの待機時間。引数指定がない場合には初期値(1秒)を設定
        TimeSpan time = throttleTime ?? TimeSpan.FromMilliseconds(1000);

        // 処理をまとめておく
        IDisposable subscription = button.OnClickAsObservable()
            .ThrottleFirst(time)
            .SubscribeAwait(async (_, ct) => await onClickAsync());

        // AddTo 用の紐づけ対象を受け取る(主に this が来る。この情報が、上記の ct にも利用される)
        if (owner != null) {
            subscription.AddTo(owner);
        }

        return subscription;
    }
}