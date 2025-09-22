using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

public class LoginManagerMono : AbstractSingleton<LoginManagerMono>
{
    /// <summary>
    /// ログインと同時に PlayFab から取得する情報の設定用クラスである GetPlayerCombinedInfoRequestParams のプロパティ。
    /// GetPlayerCombinedInfoRequestParams クラスで設定した値が InfoRequestParameters の設定値になり、true にしてある項目で各情報が自動的に取得できるようになる
    /// 各パラメータの初期値はすべて false
    /// 取得が多くなるほどログイン時間がかかり、メモリを消費するので気を付ける
    /// 取得結果は InfoResultPayLoad に入っている。false のものはすべて null になる
    /// </summary>
    public static GetPlayerCombinedInfoRequestParams CombinedInfoRequestParams { get; }
        = new GetPlayerCombinedInfoRequestParams {
            GetUserAccountInfo = true,
            GetPlayerProfile = true,
            GetTitleData = true,
            GetUserData = true,
            GetUserInventory = true,
            GetUserVirtualCurrency = true,
            GetPlayerStatistics = true
        };


    /// <summary>
    /// 初期化処理
    /// </summary>
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]   // Unityroom は PlayFab 対応していないので止めておく
    public async UniTaskVoid InitializeAsync() {

        Debug.Log("初期化開始");

        // TitleId 設定
        PlayFabSettings.staticSettings.TitleId = ConstData.PLAYFAB_DEV_TITLEID;

        Debug.Log("TitleID 設定: " + PlayFabSettings.staticSettings.TitleId);

        // PlayFab へのログイン準備とログイン
        await LoginPlayFab();

        Debug.Log("初期化完了");
    }

    /// <summary>
    /// PlayFabへのログイン準備とログイン
    /// 他のクラスでログインする場合は子の関数を使用する
    /// </summary>
    /// <returns></returns>

    public async UniTask LoginPlayFab() {
        // デバッグ
        Debug.Log("ログイン準備開始");

        // ユーザーデータとタイトルデータを初期化
        await LoginAndUpdateLocalCacheAsync();
    }

    /// <summary>
    /// ユーザーデータとタイトルデータを初期化
    /// </summary>
    /// <returns></returns>
    private async UniTask LoginAndUpdateLocalCacheAsync() {
        // デバッグ
        Debug.Log("初期化開始");

        // ユーザーIDの取得をする
        var userId = PlayerPrefsManager.UserId; // var型はstring型

        // false ユーザーIDが取得できない場合は新規作成して匿名ログインをする
        // true 取得できた場合は、ユーザーIDを使用してログインする
        // varの型はLoginResult型(PlayFab SDKで用意されている)
        var loginResult = string.IsNullOrEmpty(userId)
                    ? await CreateNewUserAsync()
                    : await LoadUserAsync(userId);

        // PlayFab のデータを自動で取得する設定にしているので、取得したデータをローカルにキャッシュする
        await UpdateLocalCacheAsync(loginResult);

    }
    /// <summary>
    /// 新規ユーザーを作成してUserIdをPlayerPrefsに保存
    /// </summary>
    /// <returns></returns>
    private async UniTask<LoginResult> CreateNewUserAsync() {
        Debug.Log("ユーザーIDが見つからない");
        while (true) {
            // UserId の採番
            var newUserId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            // ログインリクエストの作成(以下の処理は、いままで　PrepareLoginPlayPab メソッド内に書いてあったものを修正して記述)
            var request = new LoginWithCustomIDRequest {
                CustomId = newUserId,  //  <=  ここが前の処理と異なる
                CreateAccount = true,
                InfoRequestParameters = CombinedInfoRequestParams   // プロパティの情報を設定
            };

            // PlayFab にログイン
            var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

            // エラーハンドリング
            if (response.Error != null) {
                Debug.Log($"Error : {response.Error.GenerateErrorReport()}");
            }

            // もしも LastLoginTime に値が入っている場合には、採番した ID が既存ユーザーと重複しているのでリトライする
            if (response.Result.LastLoginTime.HasValue) {
                continue;
            }

            // PlayerPrefs に UserId を記録する
            PlayerPrefsManager.UserId = newUserId;

            return response.Result;
        }
    }
    /// <summary>
    /// ログインしてユーザーデータをロード
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async UniTask<LoginResult> LoadUserAsync(string userId) {
        // ログインリクエストの作成
        var request = new LoginWithCustomIDRequest {
            CustomId = userId,
            CreateAccount = false,   //　<=　アカウントの上書き処理は行わないようにする
            InfoRequestParameters = CombinedInfoRequestParams   // プロパティの情報を設定
        };

        // PlayFab にログイン
        var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

        // エラーハンドリング
        if (response.Error != null) {
            GenerateErrorLog(response.Error);
            // response.Errorにエラーに対応した処理を switch 文などで記述して複数のエラーに対応できるようにする
        }

        // エラーの内容を見てハンドリングを行い、ログインに成功しているかを判定
        var message = response.Error is null ? $"Login success! My PlayFabID is {response.Result.PlayFabId}" : response.Error.GenerateErrorReport();

        Debug.Log(message);

        return response.Result;
    }
    /// <summary>
    /// PlayFab から取得したデータ群をローカル(端末)にキャッシュ
    /// </summary>
    /// <param name="loginResult"></param>
    /// <returns></returns>
    public async UniTask UpdateLocalCacheAsync(LoginResult loginResult) {

        // カタログ類の初期化。他のインスタンスの初期化にも必要なので最初に行う
        await UniTask.WhenAll(

            );

        // タイトルデータの取得
        TitleDataManager.instance.CacheTilteData(loginResult.InfoResultPayload.TitleData);

        // タイトルデータから Localize 情報取得
        //StringManager.instance.SetLocalizeDatas(loginResult.InfoResultPayload.TitleData);

        //Debug.Log("各種データのキャッシュ完了");
    }

    /// <summary>
    /// PlayFab のアカウント作成
    /// </summary>
    /// <param name="email"></param>
    /// <param name="passward"></param>
    /// <returns></returns>
    public async UniTask<bool> CreatePlayFabAccount(string email, string passward) {
        // アカウント作成用のリクエスト
        var registerData = new RegisterPlayFabUserRequest() {
            //TitleId = PlayFabSettings.staticSettings.TitleId,
            TitleId = GameData.instance.GetGameServer(),
            Email = email,
            Password = passward,
            Username = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20),  // TODO 時間を後ろに足す
        };

        var response = await PlayFabClientAPI.RegisterPlayFabUserAsync(registerData);

        //エラーハンドリング
        if (response.Error != null) {
            // メールアドレスが登録済みの場合 true (異なる端末でも同じ Email とパスワードを使えばログイン可能)
            if (response.Error.Error == PlayFabErrorCode.EmailAddressNotAvailable) {
                Debug.Log("メールアドレス登録済みなので新規作成せずにメールアドレスでログイン");
                return true;
            }

            // 上記エラー以外のエラー処理
            GenerateErrorLog(response.Error);
            return false;
        }

        // データを全て新規作成　サーバーに保存
        if (!await UserDataManager.instance.FirstLoginCreateUserDataAsync()) {
            // できなかった場合
            return false;
        }

        // PlayerPrefs に UserId と Email を記録
        PlayerPrefsManager.UserId = response.Result.PlayFabId;
        PlayerPrefsManager.EmailAdress = email;

        return true;
    }

    /// <summary>
    /// エラー処理のログ生成
    /// </summary>
    /// <param name="playFabError"></param>
    public static void GenerateErrorLog(PlayFabError playFabError) {
        // エラーの内容に応じた例外処理を記述する
        switch (playFabError.Error) {

            case PlayFabErrorCode.InvalidParams:
                Debug.Log("有効なメールアドレスと、6～100文字以内のパスワードを入力し直してください。");
                break;
            case PlayFabErrorCode.EmailAddressNotAvailable:
                Debug.Log("このメールアドレスはすでに使用されています。");
                break;
            case PlayFabErrorCode.InvalidEmailAddress:
                Debug.Log("このメールアドレスは使用出来ません。");
                break;
            case PlayFabErrorCode.InvalidPassword:
                Debug.Log("このパスワードは無効です。");
                break;
            case PlayFabErrorCode.AccountAlreadyLinked:
                break;
            default:
                Debug.Log(playFabError.GenerateErrorReport());
                break;
        }
    }
}