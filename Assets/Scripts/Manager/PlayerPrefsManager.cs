using UnityEngine;

/// <summary>
/// PlayerPrefs のヘルパークラス
/// </summary>
public static class PlayerPrefsManager {
    /// <summary>
    /// ユーザーIDのプロパティ
    /// </summary>
    public static string UserId {
        set {
            PlayerPrefs.SetString("UserId", value);
            PlayerPrefs.Save();
        }
        get => PlayerPrefs.GetString("UserId");
    }

    /// <summary>
    /// email アドレスのプロパティ
    /// </summary>
    public static string EmailAdress {
        set {
            //Debug.Log($"{value}");
            PlayerPrefs.SetString("EmailAdress", value);
        }

        get => PlayerPrefs.GetString("EmailAdress");
    }

    /// <summary>
    /// PlayFab ログイン用のメールアドレスが端末に登録されているか確認
    /// null ならば（登録されていないなら）true
    /// </summary>
    /// <returns></returns>
    public static bool CheckEmailNotRegistered() {
        //Debug.Log(EmailAdress);
        return string.IsNullOrEmpty(EmailAdress);
    }

    public static string TestEmailAdress {
        set {
            //Debug.Log($"{value}");
            PlayerPrefs.SetString("TestEmailAdress", value);
        }

        get => PlayerPrefs.GetString("TestEmailAdress");
    }

    public static string TestPassword {
        set {
            //Debug.Log($"{value}");
            PlayerPrefs.SetString("TestPassword", value);
        }

        get => PlayerPrefs.GetString("TestPassword");
    }

    /// <summary>
    /// オートログイン用のメールアドレスが端末に登録されているか確認
    /// null ならば（登録されていないなら）true
    /// </summary>
    /// <returns></returns>
    public static bool CheckAutoLoginEmailNotRegistered() {
        //Debug.Log(TestEmailAdress);
        return string.IsNullOrEmpty(TestEmailAdress);
    }
}