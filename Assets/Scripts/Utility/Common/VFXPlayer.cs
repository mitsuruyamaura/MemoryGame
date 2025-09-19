using UnityEngine;

public class VFXPlayer : MonoBehaviour {
    [SerializeField] private GameObject hitEffectPrefab;

    public void PlayHitEffect(Vector3 position) {
        Instantiate(hitEffectPrefab, position, Quaternion.identity);
    }
}