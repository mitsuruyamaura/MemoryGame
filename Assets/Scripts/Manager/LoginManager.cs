using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using System.Collections.Generic;

public static class LoginManager {

    // 仮持ち
    static public string playfabUserId;
    static public string sessionTicket;

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
    public static async UniTask InitializeAsync() {
        DebugLogger.Log("初期化開始");

        // TitleId 設定
        PlayFabSettings.staticSettings.TitleId = "";   // 自分のタイトルを入れる

        DebugLogger.Log("TitleID 設定: " + PlayFabSettings.staticSettings.TitleId);

        // PlayFab へのログイン準備とログイン
        await LoginPlayFab();

        DebugLogger.Log("初期化完了");
    }

    /// <summary>
    /// PlayFabへのログイン準備とログイン
    /// 他のクラスでログインする場合は子の関数を使用する
    /// </summary>
    /// <returns></returns>
    public static async UniTask LoginPlayFab() {
        DebugLogger.Log("ログイン準備開始");

        // ユーザーデータとタイトルデータを初期化
        await LoginAndUpdateLocalCacheAsync();
    }

    /// <summary>
    /// ユーザーデータとタイトルデータを初期化
    /// </summary>
    /// <returns></returns>
    private static async UniTask LoginAndUpdateLocalCacheAsync() {
        DebugLogger.Log("初期化開始");

        // ユーザーIDの取得をする
        var userId = PlayerPrefsManager.UserId; // var型はstring型

        // false ユーザーIDが取得できない場合は新規作成して匿名ログインをする
        // true 取得できた場合は、ユーザーIDを使用してログインする
        // varの型はLoginResult型(PlayFab SDKで用意されている)
        LoginResult loginResult = string.IsNullOrEmpty(userId)
            ? await CreateNewUserAsync()
            : await LoadUserAsync(userId);

        // とりあえずいったんね♡
        playfabUserId = loginResult.PlayFabId;

        // PlayFab のデータを自動で取得する設定にしているので、取得したデータをローカルにキャッシュする
        await UpdateLocalCacheAsync(loginResult);
    }

    /// <summary>
    /// 新規ユーザーを作成して PlayFab に匿名ログイン
    /// UserId を PlayerPrefs に保存
    /// </summary>
    /// <returns></returns>
    private static async UniTask<LoginResult> CreateNewUserAsync() {
        DebugLogger.Log("ユーザーIDが見つからない");
        while (true) {
            // UserId の採番
            var newUserId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            // ログインリクエストの作成
            var request = new LoginWithCustomIDRequest {
                CustomId = newUserId,
                CreateAccount = true,
                InfoRequestParameters = CombinedInfoRequestParams   // プロパティの情報を設定
            };

            // PlayFab にログイン
            var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

            // エラーハンドリング
            if (response.Error != null) {
                DebugLogger.Log($"Error : {response.Error.GenerateErrorReport()}");
            }

            // もしも LastLoginTime に値が入っている場合には、採番した ID が既存ユーザーと重複しているのでリトライする
            if (response.Result.LastLoginTime.HasValue) {
                continue;
            }

            // PlayerPrefs に UserId を記録する
            PlayerPrefsManager.UserId = newUserId;
            sessionTicket = response.Result.SessionTicket;
            DebugLogger.Log($"sessionTicket : {sessionTicket}");

            return response.Result;
        }
    }
    /// <summary>
    /// ログインしてユーザーデータをロード
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private static async UniTask<LoginResult> LoadUserAsync(string userId) {
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
        }

        // エラーの内容を見てハンドリングを行い、ログインに成功しているかを判定
        var message = response.Error is null ? $"Login success! My PlayFabID is {response.Result.PlayFabId}" : response.Error.GenerateErrorReport();
        DebugLogger.Log(message);

        sessionTicket = response.Result.SessionTicket;
        DebugLogger.Log($"sessionTicket : {sessionTicket}");

        return response.Result;
    }
    /// <summary>
    /// PlayFab から取得したデータ群をローカル(端末)にキャッシュ
    /// </summary>
    /// <param name="loginResult"></param>
    /// <returns></returns>
    public static async UniTask UpdateLocalCacheAsync(LoginResult loginResult) {
        // カタログ類の初期化。他のインスタンスの初期化にも必要なので最初に行う
        await UniTask.WhenAll(

            );

        // タイトルデータの取得
        TitleDataManager.instance.CacheTilteData(loginResult.InfoResultPayload.TitleData);

        // タイトルデータから Localize 情報取得
        //StringManager.instance.SetLocalizeDatas(loginResult.InfoResultPayload.TitleData);

        // ユーザーデータの取得とエラーハンドリング
        //bool isResponseUserData = UserManager.instance.PlayFabToClientUserData(loginResult.InfoResultPayload.UserData);
        //if (!isResponseUserData) {
        //    DebugLogger.Log($"UserData のデータ取得ができませんでした");
        //}

        // ステージ進捗データの取得とエラーハンドリング
        //bool isResponseBattleStageProgressData = await BattleStageProgressManager.instance.PlayFabToClientBattleStageProgressDataAsync(loginResult.InfoResultPayload.UserData);
        //if (!isResponseBattleStageProgressData) {
        //    DebugLogger.Log($"BattleStageProgressData のデータ取得ができませんでした");
        //}

        //// 放置データの取得とエラーハンドリング
        //bool isResponseHouchiData = HouchiManager.instance.PlayFabToClientHouchiData(loginResult.InfoResultPayload.UserData);
        //if (!isResponseHouchiData) {
        //    DebugLogger.Log($"HouchiData のデータ取得ができませんでした");
        //}

        //// タワー進捗データの取得とエラーハンドリング
        //bool isResponseTowerProgressData = await TowerManager.instance.PlayFabToClientTowerProgressDataAsync(loginResult.InfoResultPayload.UserData);
        //if (!isResponseTowerProgressData) {
        //    DebugLogger.Log($"TowerProgressData のデータ取得ができませんでした");
        //}

        //// ミッションクリアデータの取得とエラーハンドリング
        //bool isResponseMissionClearData = await MissionManager.instance.PlayFabToClientMissionClearDataAsync(loginResult.InfoResultPayload.UserData);
        //if (!isResponseMissionClearData) {
        //    DebugLogger.Log($"MissionClearData のデータ取得ができませんでした");
        //}

        // カタログデータの取得
        //await CatalogDataManager.instance.CacheCatalogData();

        DebugLogger.Log("各種データのキャッシュ完了");

        // TODO テスト用
        //CallCloudScriptAPITest().Forget();
    }

    /// <summary>
    /// PlayFab のアカウント作成(Email などを使う、匿名ログインではないユーザーアカウントの作成)
    /// </summary>
    /// <param name="email"></param>
    /// <param name="passward"></param>
    /// <returns></returns>
    public static async UniTask<bool> CreatePlayFabAccount(string email, string passward) {
        // アカウント作成用のリクエスト
        var registerData = new RegisterPlayFabUserRequest() {
            TitleId = "",  //  GameData.instance.GetGameServer(),
            Email = email,
            Password = passward,
            Username = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20),  // TODO 時間を後ろに足す
        }; 

        var response = await PlayFabClientAPI.RegisterPlayFabUserAsync(registerData);

        //エラーハンドリング
        if (response.Error != null) {
            // メールアドレスが登録済みの場合 true (異なる端末でも同じ Email とパスワードを使えばログイン可能)
            if (response.Error.Error == PlayFabErrorCode.EmailAddressNotAvailable) {
                DebugLogger.Log("メールアドレス登録済みなので新規作成せずにメールアドレスでログイン");
                return true;
            }

            // 上記エラー以外のエラー処理
            GenerateErrorLog(response.Error);
            return false;
        }

        // データを全て新規作成　サーバーに保存
        //if (!await UserManager.instance.FirstLoginCreateUserDataAsync()) {
        //    // できなかった場合
        //    return false;
        //}

        // PlayerPrefs に UserId と Email を記録
        PlayerPrefsManager.UserId = response.Result.PlayFabId;
        PlayerPrefsManager.EmailAdress = email;

        return true;
    }

    /// <summary>
    /// Email とパスワードを利用した PlayFab へのログイン
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async UniTask<bool> LoginWithEmailAddressAsync(string email, string password) {
        DebugLogger.Log("Email によるログインリクエスト");

        // Email によるログインリクエストの作成
        var request = new LoginWithEmailAddressRequest {
            Email = email,
            Password = password,
            InfoRequestParameters = CombinedInfoRequestParams
        };

        // メールとパスワードで PlayFab にログイン
        var response = await PlayFabClientAPI.LoginWithEmailAddressAsync(request);

        //エラーハンドリング
        if (response.Error != null) {
            GenerateErrorLog(response.Error);
            return false;
        }

        // Email でログインしたことを記録する
        PlayerPrefsManager.EmailAdress = email;

        // 新しく PlayFab から UserId を取得
        // InfoResultPayload はクライアントプロフィールオプション(InfoRequestParameters)で許可されてないと null になる
        PlayerPrefsManager.UserId = response.Result.PlayFabId;
        DebugLogger.Log($"PlayerID : {response.Result.InfoResultPayload.PlayerProfile.PlayerId}");

        //// ユーザーデータの更新
        //bool IsResponseUserData = UserData.instance.PlayFabToClientUserData(response.Result.InfoResultPayload.UserData);

        //// エラーハンドリング
        //if (!IsResponseUserData) {
        //    DebugLogger.Log($"一部で新規作成又は、データ更新ができませんでした");
        //    return false;
        //}

        //Debug.Log("各種データのキャッシュ完了");
        // PlayFab のデータを自動で取得する設定にしているので、取得したデータをローカルにキャッシュする
        await UpdateLocalCacheAsync(response.Result);

        //Debug.Log("ログイン完了");
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
                DebugLogger.Log("有効なメールアドレスと、6～100文字以内のパスワードを入力し直してください。");
                break;
            case PlayFabErrorCode.EmailAddressNotAvailable:
                DebugLogger.Log("このメールアドレスはすでに使用されています。");
                break;
            case PlayFabErrorCode.InvalidEmailAddress:
                DebugLogger.Log("このメールアドレスは使用出来ません。");
                break;
            case PlayFabErrorCode.InvalidPassword:
                DebugLogger.Log("このパスワードは無効です。");
                break;
            case PlayFabErrorCode.AccountAlreadyLinked:
                break;
            default:
                DebugLogger.Log(playFabError.GenerateErrorReport());
                break;
        }
    }


//*********// Cloud Script 関連 //********//


    [Serializable]
    public class CloudScriptResponse<T> {
        public int status;
        public string errorMessage;
        public T data;
    }

    [Serializable]
    public class SampleResult {
        public string message;
    }


    /// <summary>
    /// Playfab の CloudScript API のテスト
    /// </summary>
    /// <returns></returns>
    public static async UniTask<string> CallCloudScriptAPITest() {
        ExecuteFunctionRequest request = new () {
            Entity = new PlayFab.CloudScriptModels.EntityKey() {
                Id = PlayFabSettings.staticPlayer.EntityId,        //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType,    //Get this from when you logged in
            },

            FunctionName = "DebugEquipAdd",                        // API の名称

            FunctionParameter = new Dictionary<string, object>()   // リクエストの引数データ
            {
                { "equipMasterId", 23 },
                { "enhanceLevel", 15 },
                { "evolutionLevel", 18 },
                { "sessionTicket", sessionTicket }
            },

            GeneratePlayStreamEvent = false
        };

        var response = await PlayFabCloudScriptAPI.ExecuteFunctionAsync(request);

        DebugLogger.Log($"response : {response}");
        DebugLogger.Log($"response.Result : {response?.Result}");
        DebugLogger.Log($"response.Result.FunctionResult : {response?.Result?.FunctionResult}");

        // PlayFab 側でのエラーがある場合
        if(response.Error != null) {
            GenerateErrorLog(response.Error);
            return null;  // もしくは数値
        }

        // 値が正常に取れた場合
        if (response != null && response.Result != null && response.Result.FunctionResult != null) {
            string json = response.Result.FunctionResult.ToString();

            var sampleResult = JsonConvert.DeserializeObject<CloudScriptResponse<SampleResult>>(json);
            //SampleResult sampleResult = JsonConvert.DeserializeObject<SampleResult>(json);

            //for (int i = 0; i < sampleResult.data.Count; i++) {
            //    DebugLogger.Log($"index : {i} / stageType : {sampleResult.data[i].stageType}");
            //}

            DebugLogger.Log($"data.message : {sampleResult.data}");

            //CloudScriptResultBase cloudScriptResultBase = JsonConvert.DeserializeObject<CloudScriptResultBase>(json);
            //DebugLogger.Log($"Message: {cloudScriptResultBase.errorMessage}");

            return json;
        } else {
            // PlayFab 側のエラーを含める

            // 画面ごとのものは、画面ごとの物で

            // "Status": 200


            DebugLogger.Log("CloudScript response is null or malformed.");
            return null;
        }
    }

    /// <summary>
    /// 実際に利用する想定のメソッド
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="functionName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async UniTask<string> CallCloudScriptFunction<T>(string functionName, Dictionary<string, object> parameters = null) {
        ExecuteFunctionRequest request = new ExecuteFunctionRequest() {
            Entity = new PlayFab.CloudScriptModels.EntityKey() {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },

            FunctionName = functionName,
            FunctionParameter = parameters,
            GeneratePlayStreamEvent = false
        };

        var response = await PlayFabCloudScriptAPI.ExecuteFunctionAsync(request);

        // PlayFab 側でのエラーがある場合
        if (response.Error != null) {
            GenerateErrorLog(response.Error);
            return null;  // もしくは数値
        }

        // 値が正常に取れた場合
        if(response?.Result?.FunctionResult != null) {
            string json = response.Result.FunctionResult.ToString();
            var result = JsonConvert.DeserializeObject<CloudScriptResponse<T>>(json);

            DebugLogger.Log($"CloudScript '{functionName}' executed successfully.");
            return json;
        } else {
            DebugLogger.Log("CloudScript response is null or malformed.");
            return null;
        }
    }
}

// FunctionName = "TeamEdit",
// { "stageType", 1 },
// { "charaNos", [410000001, 410000002, 410000009, 0, 410000008, 0] },  // 省略して初期化するとメモリリークする
// { "sessionTicket", sessionTicket }

// FunctionName = "DebugEquipAdd",
// { "equipMasterId", 23 },
// { "enhanceLevel", 15 },
// { "evolutionLevel", 18 },
// { "sessionTicket", sessionTicket }

// FunctionName = "CharaEquip",
// { "characterId", 410000001 },
// { "sessionTicket", sessionTicket }

// FunctionName = "CharaListUnEquipAll",
// { "characterId", 410000001 },
// { "sessionTicket", sessionTicket }

// FunctionName = "DebugCharaAdd",
// { "sessionTicket", sessionTicket }
// { "characterId", 410925003 },
// { "level", 1 },
// { "rank", 0 },
// { "limit", 0 }