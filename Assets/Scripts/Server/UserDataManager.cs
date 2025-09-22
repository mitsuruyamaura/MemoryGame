using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using R3;
using R3.Triggers;
using UnityEngine;

public class UserDataManager : AbstractSingleton<UserDataManager> {

    private CharaStatus charaStatus;

    /// <summary>
    /// 外部からの参照 キャラステータス
    /// </summary>
    public CharaStatus CharaStatus { get => charaStatus; set => charaStatus = value; }

    /// <summary>
    /// プレイヤーデータ内の作成と更新(プレイヤーデータ(タイトル)の Key に１つだけ値を登録する方法)
    /// </summary>
    /// <param name="updateUserData"></param>
    /// <param name="userDataPermission"></param>
    public async UniTask UpdateUserDataForKeyAsync(Dictionary<string, string> updateUserData, UserDataPermission userDataPermission = UserDataPermission.Private) {
        // リクエストの作成
        var request = new UpdateUserDataRequest {
            // データ
            Data = updateUserData,

            // アクセス許可の変更
            Permission = userDataPermission
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        // エラーハンドリング
        if (response.Error != null) {
            Debug.Log("エラー");
            return;
        }

        Debug.Log($"プレイヤーデータ{updateUserData.Keys}更新");
    }

    /// <summary>
    /// プレイヤーデータから指定した Key の情報の削除
    /// </summary>
    /// <param name="deleteKey">削除する Key の名前</param>
    public async void DeleteUserDataForKeyAsync(string deleteKey) {
        // リクエストの作成
        var request = new UpdateUserDataRequest {
            KeysToRemove = new List<string> { deleteKey }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        // エラーハンドリング
        if (response.Error != null) {

            Debug.Log("エラー");
            return;
        }

        Debug.Log($"プレイヤーデータ{deleteKey}削除");
    }

    /// <summary>
    /// データをサーバーに更新
    /// </summary>
    /// <param name="keyName"> サーバーに保存するキーの名前 </param>
    /// <param name="serializeObject"> Jsonに変換するクラス </param>
    /// <param name="userDataPermission"></param>
    /// <returns></returns>
    public async UniTask<bool> UpdateUserDataForAllAsync(string keyName, object serializeObject, UserDataPermission userDataPermission = UserDataPermission.Private) {
        // Jsonライブラリ処理 Json型にコンバート
        string userJson = JsonConvert.SerializeObject(serializeObject);

        // リクエストの作成
        var request = new UpdateUserDataRequest {
            Data = new Dictionary<string, string>
            {
                { keyName, userJson }
            },

            Permission = userDataPermission,
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        // エラーハンドリング
        if (response.Error != null) {
            LoginManager.GenerateErrorLog(response.Error);
            return false;
        }

        // アプリのバージョンチェック。true ならバージョンアップ
        //bool versionCheckResult = await VersionCheckManager.CheckLatestAppVersion();

        // 現在のバージョンが最新バージョンではない場合
        //if (versionCheckResult) {
        //    Debug.Log($"アプリバージョンが最新ではないため、バージョンアップします。");
        //    return false;
        //}

        return true;
    }

    /// <summary>
    /// 初回ログインの新規作成
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> FirstLoginCreateUserDataAsync() {
        // リクエスト作成
        var request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                // 新規作成の際に必要なデータは↓に追加
                //{"CharaStatus", JsonConvert.SerializeObject(CharaStatus.Create())},

            }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        // エラーハンドリング
        if (response.Error != null) {
            LoginManager.GenerateErrorLog(response.Error);
            return false;
        }

        return true;
    }

    /// <summary>
    /// PlayFab の最新データを取得してローカルにキャッシュ
    /// </summary>
    /// <param name="userData"></param>
    public bool PlayFabToClientUserData(Dictionary<string, UserDataRecord> userData) {
        // true  ? Jsonライブラリ処理でコンバート
        // false : 新規作成 
        CharaStatus = userData.TryGetValue(ConstData.CHARA_STATUS_KEY, out var status)
            ? CharaStatus = JsonConvert.DeserializeObject<CharaStatus>(status.Value)
            : CharaStatus.Create();


        // 所持アイテムのリスト取得
        //if (userData.TryGetValue(ConstData.INVENTORY_USEABLE_ITEM_LIST_KEY, out var _)) {
        //    inventoryUseableItemList = JsonHelper.ListFromJson<InvenItem>(userData[ConstData.INVENTORY_USEABLE_ITEM_LIST_KEY].Value).ToList();
        //}



        // TODO 他にも処理があれば追加

        // 購読処理用のデータにセット
        //UpdateReactiveUserData();
        return true;
    }



    ///// <summary>
    ///// インベントリに消費アイテムを追加
    ///// </summary>
    ///// <param name="inventryItem"></param>
    //public void AddInventoryItemList(InvenItem inventryItem) {

    //    // 所持しているアイテムであるか確認する(一度でも獲得していれば対象となる)
    //    var existingItem = inventoryUseableItemList.FirstOrDefault(item => item.itemMasterData.item_id == inventryItem.itemMasterData.item_id);

    //    // 所持しているアイテムの場合
    //    if (existingItem != null) {

    //        // 最大値以内で加算
    //        existingItem.count = Mathf.Min(existingItem.count + inventryItem.count, inventryItem.itemMasterData.max_count);
    //    } else {
    //        // 獲得したアイテムが最大所持数以内か確認
    //        int count = Mathf.Min(inventryItem.count, inventryItem.itemMasterData.max_count);

    //        // 所持していないアイテムの場合には、List に追加する
    //        inventoryUseableItemList.Add(new(inventryItem.itemMasterData, count));
    //    }
    //    OrderByDescendingUseableItemList();

    //    // サーバーに消費アイテムを保存
    //    SaveInventoryUseableItemList().Forget();
    //}

    ///// <summary>
    ///// インベントリから対象のアイテムを削除
    ///// </summary>
    ///// <param name="inventryItem"></param>
    ///// <returns></returns>
    //public InvenItem RemoveInventoryItemList(InvenItem inventryItem) {

    //    // 所持しているアイテムであるか確認する(一度でも獲得していれば対象となる)
    //    InvenItem existingItem = inventoryUseableItemList.FirstOrDefault(item => item.itemMasterData.item_id == inventryItem.itemMasterData.item_id);

    //    // 所持しているアイテムの場合
    //    if (existingItem != null) {

    //        // 最大値以内で減算
    //        existingItem.count = Mathf.Max(existingItem.count - inventryItem.count, 0);

    //        OrderByDescendingUseableItemList();

    //        // サーバーに消費アイテムを保存
    //        SaveInventoryUseableItemList().Forget();

    //        return existingItem;
    //    } else {
    //        return null;
    //    }
    //}

    ///// <summary>
    ///// サーバーに消費アイテムを保存
    ///// </summary>
    ///// <returns></returns>
    //public async UniTask SaveInventoryUseableItemList() {
    //    string itemList = JsonHelper.ListToJson(inventoryUseableItemList);
    //    Dictionary<string, string> updateItemList = new() { { ConstData.INVENTORY_USEABLE_ITEM_LIST_KEY, itemList } };
    //    await UpdateUserDataForKeyAsync(updateItemList);
    //}

    ///// <summary>
    ///// 消費アイテムのスロット index 昇順並び替え(1 -> 100)
    ///// </summary>
    //public void OrderByDescendingUseableItemList() {
    //    if (inventoryUseableItemList.Count > 0) {
    //        inventoryUseableItemList = inventoryUseableItemList.OrderByDescending(item => item.itemMasterData.item_id).ToList();
    //    }
    //}

    ///// <summary>
    ///// Flagの加算とCountの返却
    ///// </summary>
    ///// <param name="flagIndex"></param>
    //public int AddFlag(int flagIndex) {
    //    int retFlagCount = 0;
    //    var flagData = flagList.FirstOrDefault(flag => flag.flag_index == flagIndex);
    //    if (flagData != null) {
    //        // 所持しているFlagの場合Countを加算
    //        flagData.count++;
    //        retFlagCount = flagData.count;
    //    } else {
    //        // 所持していないFlagの場合には、List に追加する
    //        flagList.Add(new FlagData(flagIndex, 1));
    //        retFlagCount = 1;
    //    }
    //    return retFlagCount;
    //}

    ///// <summary>
    ///// アイテム獲得用の会話イベントが登録されている(獲得済)か判定
    ///// true ならアイテム獲得済
    ///// </summary>
    ///// <param name="searchEventDetailId"></param>
    ///// <param name="searchListIndex"></param>
    ///// <returns></returns>
    //public bool IsEventRewardReceived(int searchEventDetailId, int searchListIndex) {
    //    return eventRewardList.Any(data => data.event_detail_id == searchEventDetailId && data.list_index == searchListIndex);
    //}


    //    /// <summary>
    //    /// アイテム獲得
    //    /// </summary>
    //    /// <param name="rewardType"></param>
    //    /// <param name="index"></param>
    //    /// <param name="getCount"></param>
    //    public void GetItem(RewardType rewardType, int index, int getCount = 1) {

    //        // 獲得したアイテムが、消費アイテムか素材であるか確認
    //        if (rewardType == RewardType.item) {

    //            // 獲得したアイテムの情報取得
    //            ItemMasterData itemMasterData = TitleDataManager.instance.GetItemMasterData(index);

    //            // 獲得したアイテムをいずれかのインベントリに追加
    //            if (itemMasterData.inven_type == ConstData.ITEM) {
    //                AddInventoryItemList(new InvenItem(itemMasterData, getCount));
    //            } else if (itemMasterData.inven_type == ConstData.MATERIAL) {
    //                AddInventoryMaterialList(new InvenItem(itemMasterData, getCount));
    //            } else if (itemMasterData.inven_type == ConstData.ALBUM) {
    //                // アルバムリストに追加
    //                GoddesAlbumPieceList.Add(index);
    //                SaveGoddesAlbumPiece(index);
    //            }
    //        } else if (rewardType == RewardType.pet) {
    //            // ペットの獲得処理
    //            PetMasterData petMasterData = TitleDataManager.instance.GetPetMasterData(index);
    //            AddInventoryPetList(InvenPet.Init(petMasterData.pet_id));
    //        } else if (rewardType == RewardType.equipment) {
    //            // 装備品
    //            EquipmentMasterData equipmeMasterData = TitleDataManager.instance.GetEquipmentMasterData(index);
    //            EquipmentType equipmentType = ConvertEnumType.FromString<EquipmentType>(equipmeMasterData.type);
    //            AddInventoryEquipment(equipmentType, index);
    //            //装備獲得をセーブ
    //            SaveInventoryEquipment(equipmentType);
    //        } else if (rewardType == RewardType.piece) {
    //            // アルバムリストに追加
    //            GoddesAlbumPieceList.Add(index);
    //            SaveGoddesAlbumPiece(index);
    //        } else if (rewardType == RewardType.skill) {
    //            AddSkillList(index);
    //            SaveSkillList().Forget();
    //        }
    //    }

    //    /// <summary>
    //    /// アイテム消費
    //    /// </summary>
    //    /// <param name="rewardType"></param>
    //    /// <param name="index"></param>
    //    /// <param name="reduceCount"></param>
    //    public void ReduceItem(RewardType rewardType, int index, int reduceCount = 1, bool isSave = true) {
    //        // 消費アイテムか素材であるか確認
    //        if (rewardType == RewardType.item) {

    //            // 消費するアイテムの情報取得
    //            ItemMasterData itemMasterData = TitleDataManager.instance.GetItemMasterData(index);

    //            // いずれかのインベントリより消費
    //            if (itemMasterData.inven_type == ConstData.ITEM) {
    //                RemoveInventoryItemList(new InvenItem(itemMasterData, reduceCount));
    //            } else if (itemMasterData.inven_type == ConstData.MATERIAL) {
    //                RemoveInventoryMaterialList(new InvenItem(itemMasterData, reduceCount), isSave);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 指定したアイテムが必要数を満たしているか確認
    //    /// </summary>
    //    /// <param name="invenType"></param>
    //    /// <param name="itemIndex"></param>
    //    /// <param name="needCount"></param>
    //    /// <returns></returns>
    //    public bool CheckTargetItemCount(string invenType, int itemIndex, int needCount) {
    //#if DEBUG
    //        Debug.Log($"アイテムの種類 : {invenType} / Index : {itemIndex} / 必要数 : {needCount}");
    //#endif
    //        if (invenType == ConstData.ITEM) {
    //            return inventoryUseableItemList.Any(item => item.itemMasterData.item_id == itemIndex && item.count >= needCount);
    //        } else if (invenType == ConstData.MATERIAL) {
    //            return inventoryMaterialList.Any(item => item.itemMasterData.item_id == itemIndex && item.count >= needCount);
    //        }
    //        return false;
    //    }



    ///// <summary>
    ///// //討伐モンスターを追加
    ///// </summary>
    ///// <param name="monsterIndex">討伐したモンスターのindex_id</param>
    ///// <param name="addNum">追加するモンスターの討伐数（デフォルトは１）</param>
    //public void AddDefeatedMonster(int monsterIndex, int addNum = 1) {
    //    //モンスターの討伐歴があるか検索
    //    foreach (MonsterDefeatedCount monsterDefeated in MonsterDefeatedCountList) {
    //        //討伐済みの場合は、討伐数を足す
    //        if (monsterDefeated.mon_index == monsterIndex) {
    //            monsterDefeated.defeat_count += addNum;
    //            return;
    //        }
    //    }
    //    //初めての討伐の場合はモンスターIDと一緒にリストに追加する
    //    MonsterDefeatedCount monsterData = new();
    //    monsterData.mon_index = monsterIndex;
    //    monsterData.defeat_count = addNum;
    //    MonsterDefeatedCountList.Add(monsterData);

    //    // サーバーに討伐済みモンスターの情報を保存
    //    string defeatList = JsonHelper.ListToJson(MonsterDefeatedCountList);
    //    Dictionary<string, string> updatedefeatList = new() { { ConstData.SUBDUE_MONSTER_KEY, defeatList } };
    //    UpdateUserDataForKeyAsync(updatedefeatList).Forget();

    //    return;
    //}

    ///// <summary>
    ///// //モンスターの討伐数を取得
    ///// </summary>
    ///// <param name="monsterIndex">検索するモンスターのindex_id</param>
    ///// <returns>モンスター討伐数（リストになければ０を返す）</returns>
    //public int GetDefeatedMonsterCount(int monsterIndex) {
    //    foreach (MonsterDefeatedCount monsterDefeated in MonsterDefeatedCountList) {
    //        if (monsterDefeated.mon_index == monsterIndex) {
    //            return monsterDefeated.defeat_count;
    //        }
    //    }
    //    return 0;
    //}

    ///// <summary>
    ///// アイテム(消費アイテム、素材)から現在の所持数を取得
    ///// 所持していない場合には 0 を戻す
    ///// </summary>
    ///// <param name="itemId"></param>
    ///// <returns></returns>
    //public int GetItemCount(int itemId) {
    //    return inventoryUseableItemList
    //        .Concat(inventoryMaterialList)                          // 同じクラスの List 同士を統合
    //        .Where(item => item.itemMasterData.item_id == itemId)   // 統合した List 内に該当するアイテムがあるかチェック(条件に一致する要素がない場合、空のコレクションを返す)
    //        .Select(item => item.count)                             // 取得したデータからアイテム数を取得(要素が1つも見つからなかった場合、Selectメソッドは何も処理せずに空のコレクションを返す)
    //        .FirstOrDefault();                                      // アイテム数を返す。データが空のコレクション(所持していない)場合は 0 を返す
    //}



    private void Start() {

    }
}