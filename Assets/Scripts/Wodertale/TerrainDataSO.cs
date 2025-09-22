using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainDataSO", menuName = "Create TerrainDataSO")]
public class TerrainDataSO : ScriptableObject {
    public List<TerrainData> terrainDataList = new();   
}