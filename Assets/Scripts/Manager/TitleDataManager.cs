using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

public class TitleDataManager : AbstractSingleton<TitleDataManager> {
    //public ActiveSkillMasterTable activeSkillMasterTable;
    //public AutoRewardMasterTable autoRewardMasterTable;
    //public BannerMasterTable bannerMasterTable;
    //public BoxRewardMasterTable boxRewardMasterTable;
    //public ChapterMasterTable chapterMasterTable;
    //public CharaLimitMasterTable charaLimitMasterTable;
    //public CharaMasterTable charaMasterTable;
    //public ConditionMasterTable conditionMasterTable;
    //public ConstantMasterTable constantMasterTable;
    //public DailyMissionMasterTable dailyMissionMasterTable;
    //public EnemyMasterTable enemyMasterTable;
    //public EnhanceSetBonusMasterTable enhanceSetBonusMasterTable;
    //public EquipEnhanceMasterTable equipEnhanceMasterTable;
    //public EquipEvolutionMasterTable equipEvolutionMasterTable;
    //public EquipMasterTable equipMasterTable;
    //public EvolutionSetBonusMasterTable evolutionSetBonusMasterTable;
    //public GemBoxRewardMasterTable gemBoxRewardMasterTable;
    //public GemMasterTable gemMasterTable;
    //public GemSetBonusMasterTable gemSetBonusMasterTable;
    //public HighSpeedAutoRewardMasterTable highSpeedAutoRewardMasterTable;
    //public ItemMasterTable itemMasterTable;
    //public LevelMasterTable levelMasterTable;
    //public LocalizeImageMasterTable localizeImageMasterTable;
    //public PassiveSkillMasterTable passiveSkillMasterTable;
    //public PersonalEquipEnhanceMasterTable personalEquipEnhanceMasterTable;
    //public PersonalEquipMasterTable personalEquipMasterTable;
    //public RankMasterTable rankMasterTable;
    //public StageMasterTable stageMasterTable;
    //public StorageExpansionMasterTable storageExpansionMasterTable;
    //public TowerReachMasterTable towerReachMasterTable;
    //public WeeklyMissionMasterTable weeklyMissionMasterTable;


    // TODO　マスターデータ用の変数を追加

    // 型 → テーブル インスタンスのマッピング
    private static Dictionary<Type, IMasterTable> tableMap;


    /// <summary>
    /// PlayFab の TilteData(各種マスターデータ)をローカルにキャッシュ
    /// </summary>
    /// <param name="titleData"></param>
    public void CacheTilteData(Dictionary<string, string> titleData) {
        //activeSkillMasterTable.Set(JsonHelper.ListFromJson<ActiveSkillMaster>(titleData["ActiveSkillMaster"]));
        //autoRewardMasterTable.Set(JsonHelper.ListFromJson<AutoRewardMaster>(titleData["AutoRewardMaster"]));
        //bannerMasterTable.Set(JsonConvert.DeserializeObject<List<BannerMaster>>(titleData["BannerMaster"], new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
        //boxRewardMasterTable.Set(JsonHelper.ListFromJson<BoxRewardMaster>(titleData["BoxRewardMaster"]));
        //chapterMasterTable.Set(JsonHelper.ListFromJson<ChapterMaster>(titleData["ChapterMaster"]));
        //charaLimitMasterTable.Set(JsonHelper.ListFromJson<CharaLimitMaster>(titleData["CharaLimitMaster"]));
        //charaLimitMasterTable.SetSkillList();
        //charaMasterTable.Set(JsonHelper.ListFromJson<CharaMaster>(titleData["CharaMaster"]));
        //conditionMasterTable.Set(JsonHelper.ListFromJson<ConditionMaster>(titleData["ConditionMaster"]));
        //constantMasterTable.Set(JsonHelper.ListFromJson<ConstantMaster>(titleData["ConstantMaster"]));
        //dailyMissionMasterTable.Set(JsonHelper.ListFromJson<DailyMissionMaster>(titleData["DailyMissionMaster"]));
        //enemyMasterTable.Set(JsonHelper.ListFromJson<EnemyMaster>(titleData["EnemyMaster"]));
        //enhanceSetBonusMasterTable.Set(JsonHelper.ListFromJson<EnhanceSetBonusMaster>(titleData["EnhanceSetBonusMaster"]));
        //equipEnhanceMasterTable.Set(JsonHelper.ListFromJson<EquipEnhanceMaster>(titleData["EquipEnhanceMaster"]));
        //equipEvolutionMasterTable.Set(JsonHelper.ListFromJson<EquipEvolutionMaster>(titleData["EquipEvolutionMaster"]));
        //equipMasterTable.Set(JsonHelper.ListFromJson<EquipMaster>(titleData["EquipMaster"]));
        //evolutionSetBonusMasterTable.Set(JsonHelper.ListFromJson<EvolutionSetBonusMaster>(titleData["EvolutionSetBonusMaster"]));
        //gemBoxRewardMasterTable.Set(JsonHelper.ListFromJson<GemBoxRewardMaster>(titleData["GemBoxRewardMaster"]));
        //gemMasterTable.Set(JsonHelper.ListFromJson<GemMaster>(titleData["GemMaster"]));
        //gemSetBonusMasterTable.Set(JsonHelper.ListFromJson<GemSetBonusMaster>(titleData["GemSetBonusMaster"]));
        //highSpeedAutoRewardMasterTable.Set(JsonHelper.ListFromJson<HighSpeedAutoRewardMaster>(titleData["HighSpeedAutoRewardMaster"]));
        //itemMasterTable.Set(JsonHelper.ListFromJson<ItemMaster>(titleData["ItemMaster"]));
        //levelMasterTable.Set(JsonHelper.ListFromJson<LevelMaster>(titleData["LevelMaster"]));
        //localizeImageMasterTable.Set(JsonHelper.ListFromJson<LocalizeImageMaster>(titleData["LocalizeImageMaster"]));
        //passiveSkillMasterTable.Set(JsonHelper.ListFromJson<PassiveSkillMaster>(titleData["PassiveSkillMaster"]));
        //passiveSkillMasterTable.SetPassiveStatusTypes();
        //personalEquipEnhanceMasterTable.Set(JsonHelper.ListFromJson<PersonalEquipEnhanceMaster>(titleData["PersonalEquipEnhanceMaster"]));
        //personalEquipMasterTable.Set(JsonHelper.ListFromJson<PersonalEquipMaster>(titleData["PersonalEquipMaster"]));
        //rankMasterTable.Set(JsonHelper.ListFromJson<RankMaster>(titleData["RankMaster"]));
        //stageMasterTable.Set(JsonHelper.ListFromJson<StageMaster>(titleData["StageMaster"]));
        //stageMasterTable.SetTowerRewardData();
        //// バトルデータとタワーデータを分けて保持
        //StageChapterManager.instance.SetupSortStageData();
        //storageExpansionMasterTable.Set(JsonHelper.ListFromJson<StorageExpansionMaster>(titleData["StorageExpansionMaster"]));
        //towerReachMasterTable.Set(JsonHelper.ListFromJson<TowerReachMaster>(titleData["TowerReachMaster"]));
        //towerReachMasterTable.SetReachRewardList();
        //weeklyMissionMasterTable.Set(JsonHelper.ListFromJson<WeeklyMissionMaster>(titleData["WeeklyMissionMaster"]));

        // TODO 他のマスターデータの取得処理追加


        // マスターテーブルのマップを作成
        BuildTableMap();
    }

    /// <summary>
    /// リフレクションでインスペクターに設定されているマスターテーブルを自動登録
    /// </summary>
    private void BuildTableMap() {
        tableMap = new Dictionary<Type, IMasterTable>();

        // このクラスのフィールドをすべて取得
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields) {
            // フィールドが IMasterTable を継承しているか確認
            if (typeof(IMasterTable).IsAssignableFrom(field.FieldType)) {
                // キャストを兼ねた null チェックしてインスタンスを取得
                if (field.GetValue(this) is IMasterTable tableInstance) {
                    // 型とテーブルを紐付け
                    tableMap[tableInstance.DataType] = tableInstance;
                }
            }
        }
        //DebugLogger.Log(tableMap.Count);
    }

    /// <summary>
    /// 型と ID でマスターデータを取得
    /// 使い方: TitleDataManager.FindById<ActiveSkillMaster>(1001);
    /// </summary>
    public static M FindById<M>(int id) where M : class, IMasterData {
        if (tableMap == null) {
            DebugLogger.Log("FindById: テーブルマップが初期化されていません");
            return default;
        }

        if (!tableMap.TryGetValue(typeof(M), out var table)) {
            DebugLogger.Log($"FindById: マスターテーブルが登録されていません ({typeof(M).Name})");
            return default;
        }

        var boxed = table.GetDataBoxed(id);
        if (boxed == null) {
            DebugLogger.Log($"FindById: マスターテーブルが登録されていません (boxed : {boxed})");
            return default;
        }

        return boxed as M;
    }
}