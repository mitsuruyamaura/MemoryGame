using UnityEngine;

public class EffectManager : AbstractSingleton<EffectManager> {

    public GameObject LevelUpLogoPrefab;

    public Transform effectConteinerTran;

    // mi
    // Player 側
    public GameObject chargeUpEffectPrefab;
    public GameObject dashWindPrefab;
    public GameObject recoveryStaminaEffectPrefab;
    public GameObject recoveryLifeEffectPrefab;
    public GameObject orbGetEffectPrefab;
    public GameObject abilityPowerUpPrefab_1;
    public GameObject abilityPowerUpPrefab_2;
    public GameObject levelUpPrefab;

    public GameObject clearEffectPrefab;
    public GameObject curseEffectPrefab;

    // Enemy 側
    public GameObject enemyHitEffectPrefab;
    public GameObject destroyEffectPrefab;
    public BossEffect bossEffectPrefab;


}