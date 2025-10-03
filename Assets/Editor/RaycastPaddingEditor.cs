using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image コンポーネントの RaycastPadding を可視化するエディター拡張
/// </summary>
public class RaycastPaddingEditor {
    [DrawGizmo(GizmoType.Selected)]
    private static void DrawImageRaycastPaddingGizmos(Image image, GizmoType gizmoType) {
        var color = Color.red;

        Gizmos.color = color;

        var rt = image.rectTransform;
        var positions = new Vector3[4];
        var rect = GetRect(image.rectTransform);
        var pad = image.raycastPadding;
        positions[0] = new Vector3(rect.x + pad.x, rect.y + pad.y);
        positions[1] = new Vector3(rect.x + rect.width - pad.z, rect.y + pad.y);
        positions[2] = new Vector3(rect.x + rect.width - pad.z, rect.y + rect.height - pad.w);
        positions[3] = new Vector3(rect.x + pad.x, rect.y + rect.height - pad.w);

        for (int i = 0; i < positions.Length; i++) {
            //Debug.Log($"name= {image.gameObject.name} : {positions[i]}");
        }

        if (image.raycastTarget) {
            Gizmos.DrawLine(positions[0], positions[1]);
            Gizmos.DrawLine(positions[1], positions[2]);
            Gizmos.DrawLine(positions[2], positions[3]);
            Gizmos.DrawLine(positions[3], positions[0]);
        }
    }

    public static Rect GetRect(RectTransform rectTransform) {
        Vector2 size = rectTransform.rect.size;
        size.x *= rectTransform.lossyScale.x;
        size.y *= rectTransform.lossyScale.y;

        return new Rect {
            center = (Vector2)rectTransform.position - new Vector2(
                rectTransform.pivot.x * size.x,
                rectTransform.pivot.y * size.y
            ),
            size = size,
        };
    }
}