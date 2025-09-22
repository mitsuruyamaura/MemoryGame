using UnityEngine;
using System;

public class ResetManager : MonoBehaviour {

    public bool useResetEveryDay;            // 毎日リセットするかどうか。true で毎日リセット
    public DayOfWeek resetDay;               // 毎日リセットしない場合、リセットする曜日を指定
    public int resetHour = 4;                // リセット時間（指定した時間）

    public string resetDateString;           // yyyy/MM/dd HH の形式で保存する文字列

    private DateTime resetDate;              // 実際のリセット日時
    private DateTime? debugDate;             // 仮の日時を設定するための変数
    private const int DAY_IN_WEEK = 7;        // 1週間の日数を変数として定義


    void Start() {
        // 保存されたリセット日付の取得
        string savedDate = PlayerPrefs.GetString("ResetDate", "");

        // 保存されたデータがある場合
        if (!string.IsNullOrEmpty(savedDate)) {
            // DateTime として変換
            resetDate = DateTime.Parse(savedDate);
            Debug.Log($"リセットした日付 : {resetDate:yyyy/MM/dd HH}");
        } else {
            // 初期化時にリセット日時を設定
            SetNextResetDate();
        }

        //InvokeRepeating("CheckResetDate", 0f, 1f); // 1秒ごとにCheckResetDateを呼ぶ
    }

    void Update() {
        // スペースキーが押された場合、仮の日時を使ってテスト
        if (Input.GetKeyDown(KeyCode.Space)) {
            TestWithDebugDate();
        }

        /// <summary>
        /// テスト
        /// </summary>
        void TestWithDebugDate() {
            if (debugDate.HasValue) {
                Debug.Log($"テスト用仮日時: {debugDate.Value}");

                // 仮の日時を使ってリセットチェックを実行
                CheckResetDate();
            } else {
                Debug.Log("仮の日時が設定されていません。");
            }
        }

        /// <summary>
        /// リセットチェック
        /// </summary>
        void CheckResetDate() {
            DateTime today = debugDate ?? DateTime.Today; // 仮の日時が設定されていればそれを使用

            // 現在の日付がリセット日付以降か確認
            if (DateTime.Compare(today, resetDate) >= 0) {
                // 次のリセット日付を更新
                OnApplicationQuit();

                // ここにリセット処理を書く
            }
        }
    }

    /// <summary>
    /// 次のリセット日時を設定
    /// </summary>
    void SetNextResetDate() {
        // DateTime.Today は現在の日付の午前0時0分 (0:00) を取得する
        // DateTime.Now とは異なり、時刻は無視されている
        DateTime today = DateTime.Today;

        // 毎日リセットの場合
        if (useResetEveryDay) {
            // resetHour 分を 0 時に加える
            resetDate = today.AddHours(resetHour);

            // 現在時刻が設定されたリセット日時を過ぎている場合
            //   -> 翌日同じ時間にリセット日時を設定し直す
            if (DateTime.Now >= resetDate) {
                // 翌日同じ時間にリセット
                resetDate = resetDate.AddDays(1);
            }
        } else {
            // 指定した曜日にリセットする場合
            // 1週間は7日なので、resetDayに指定された曜日までの追加日数を計算
            // 翌週以降も対応可能(1/1 → 1/9 で更新チェックの場合、1/15 が次のリセット日になる)
            int dayAddition = ((int)resetDay - (int)today.DayOfWeek + DAY_IN_WEEK) % DAY_IN_WEEK;

            // 同じ曜日の場合、指定時間（resetHour）を過ぎている場合は翌週に設定
            if (dayAddition == 0 && DateTime.Now.Hour >= resetHour) {
                // 7日後(翌週の同じ曜日)に設定
                dayAddition += DAY_IN_WEEK;
            }

            // 計算した追加日数を基に、リセット日時を設定（曜日を考慮して設定）
            resetDate = today.AddDays(dayAddition).AddHours(resetHour);
        }

        // リセット日時の分と秒を0に設定（時刻がぴったり指定時間になるように調整）
        resetDate = new DateTime(resetDate.Year, resetDate.Month, resetDate.Day, resetDate.Hour, 0, 0);

        // インスペクター表示用に、resetDate を「yyyy/MM/dd HH」の形式で文字列化
        resetDateString = resetDate.ToString("yyyy/MM/dd HH");

        // 設定を保存
        PlayerPrefs.SetString("ResetDate", resetDateString);
    }

    /// <summary>
    /// ResetManagerEditor より resetDateString のインスペクターの値変更時に自動的に実行される
    /// デバッグ用の仮日時を DataTime として設定する
    /// </summary>
    /// <param name="newDate">resetDateString の値。yyyy/MM/dd HH で入力する(2022/01/10 01 のように 0 も補記する)</param>
    public void SetDebugDate(DateTime newDate) {
        debugDate = newDate;
        Debug.Log(newDate.ToString("yyyy/MM/dd HH"));
    }

    void OnApplicationQuit() {
        // 次のリセット日時を計算
        SetNextResetDate();

        // 日付のみの文字列を取得して PlayerPrefsに保存
        string shortDate = resetDate.ToShortDateString();
        PlayerPrefs.SetString("ResetDate", shortDate);
    }
}