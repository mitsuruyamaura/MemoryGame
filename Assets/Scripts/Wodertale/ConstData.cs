/// <summary>
/// ボコスカ用
/// ContentController としてサーバーから取得する方法もある
/// </summary>
public static class ConstData {
    public const int DEFAULT_PLAYER_HP = 50;

    public const int MAX_INVENTORY_SIZE = 32;
    public const int DEFAULT__INVENTORY_SIZE = 6;

    public const int MAX_LEVEL = 999;
    public const int MAX_STATUS_VALUE = 999;

    public const int MAX_USE_ITEM_COUNT = 999;
    public const int MAX_ENHANCE_LEVEL = 99;


    public const string PLAYFAB_TEST_TITLEID = "E1AC6";
    public const string PLAYFAB_DEV_TITLEID = "E1AC6";

    public const string CHARA_STATUS_KEY = "charaStatus";

    public const float XFADE_TIME = 1.4f;                  // クロスフェード時間
    public const float DEFAULT_MASTER_VOLUME = 0.7f;
    public const string MASTER_AUDIO_NAME = "Master";      // AudioMixer の AudioGroup の名前で指定できる。AudioMixer 側のスクリプトでの制御許可の設定が必要

    public const int WAVE_INFO_COUNT = 45;                 // 次の Wave までの残り歩数表示タイミング
    public const int WAVE_WALK_COUNT = 50;                 // 各 Wave ごとの最大歩数

    public const int POWER_SPOT_NEED_RELEASE_POINT = 50;   // パワースポットの解放に必要なソウルポイント

    public const string GAME_DATA_SAVE_KEY = "GameDataSaveKey_";    // クリアデータのセーブ用 Key
    public const int MAX_SAVE_SLOTS = 3;                            // セーブスロットの数

    public const string BGM_VOLUME = "bgmVolume";          // BGM ボリュームのセーブ用 Key

    public const float REACTION_BASE = 20.0f;
}