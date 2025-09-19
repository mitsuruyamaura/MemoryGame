using UnityEngine;

public static class GridHelper {
    private static GridLayout _gridLayout;


    public static void Initialize(GridLayout layout) {
        _gridLayout = layout;
    }

    public static Vector3 GridToWorld(Vector3Int gridPos) {
        if (_gridLayout == null) {
            Debug.Log("GridHelper not initialized. Call GridHelper.Initialize with a GridLayout.");
            return Vector3.zero;
        }

        Vector3 centerOffset = new Vector3(0.5f, 0.5f, 0);
        Vector3 worldPos = _gridLayout.LocalToWorld(gridPos) + centerOffset;

        return worldPos;
    }
}