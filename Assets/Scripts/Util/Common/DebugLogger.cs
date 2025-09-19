using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

/// <summary>
/// Debug.Log メソッドのラップ用ヘルパークラス
/// Unity エディターの Console ビューでダブルクリックした際にログ元のコードにジャンプ(リダイレクト)できる
/// 
/// ラップした Debug.Log メソッドをダブルクリックした際にログ元にジャンプするようにするには
/// クラス名を ～ Logger で終わるようにし、かつ、メソッド名を Log とすることで実現可能
/// 例えば ConsoleLogger、LogHelperLogger など
/// </summary>
public static class DebugLogger
{
    /// <summary>
    /// Console ビューでダブルクリックした際に、このメソッドではなく、ログ元のコードへジャンプする Log メソッド
    /// DefineSymbol に DEBUG を含めることで、デバッグビルド時にも動作する
    /// </summary>
    /// <param name="value">ログに出力する値</param>
    /// <param name="filePath">ログ出力元のファイルパス</param>
    /// <param name="memberName">ログ出力元のメソッド名</param>
    /// <param name="lineNumber">ログ出力元の行番号</param>
    [Conditional("DEBUG")]
    public static void Log(object value, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) {
        // valueが null なら "null" を代入
        value ??= "null";

        // 呼び出し元のファイルと行番号を付ける
        string message = $"{value} \n [{Path.GetFileName(filePath)}:{lineNumber} ({memberName})]";

        // ログ表示
        UnityEngine.Debug.Log(message?.ToString());
    }
}