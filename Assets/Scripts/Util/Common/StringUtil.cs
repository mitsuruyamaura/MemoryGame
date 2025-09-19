using System;
using System.Linq;

public static class StringUtil {
    public static string GetPercentText(float value){
        return $"{value * 100}%";
    }

    /// <summary>
    /// 文字列をカンマ部分で区切って int 配列に変換
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int[] ParseStringToIntArray(string str) {
        // null チェック
        if (string.IsNullOrWhiteSpace(str)) {
            return Array.Empty<int>();
        }

        // 文字列をカンマ部分で区切って int 配列に変換
        return str.Split(',')
                  .Where(s => !string.IsNullOrWhiteSpace(s))
                  .Select(s => int.Parse(s))
                  .ToArray();
    }

    public static string ConvertInternationalSystemOfUnit(long value) {
        if (value < 1_0000) return value.ToString();

        string[] suffixes = { "", "K", "M", "B", "T", "Q" }; // Q: Quadrillion(千兆)
        int suffixIndex = 0;
        double abbreviated = value;

        while (abbreviated >= 10000 && suffixIndex < suffixes.Length - 1) {
            abbreviated /= 1000;
            suffixIndex++;
        }

        // 小数点第1位まで（必要に応じて調整）
        return $"{abbreviated:N0}{suffixes[suffixIndex]}";
    }
}