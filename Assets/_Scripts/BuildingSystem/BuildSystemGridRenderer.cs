using UnityEngine;

[ExecuteAlways]
public class BuildSystemGridRenderer : MonoBehaviour
{
    public Material lineMaterial; // assign Unlit/Color or Sprites/Default
    public Color color = Color.white;
    public float lineWidth = 0.02f;
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;
    [Tooltip("Small local Z offset for grid lines to avoid z-fighting with other renderers")]
    public float localZ = 0f;
    [Tooltip("If true the LineRenderers will be created in local space and parented to this transform. This reduces flicker in 2D.")]
    public bool useLocalSpace = true;
    [Tooltip("Draw the grid in the Scene view using Gizmos (visible outside Play mode).")]
    public bool drawGizmosInEditor = true;

    private Transform linesParent;

    void OnDisable() { DestroyLinesParent(); }

    public void Rebuild(float width, float height, float cellSize, Vector3 originPos)
    {
        if (!lineMaterial) return;
        if (linesParent) DestroyLinesParent();
        linesParent = new GameObject("GridLines").transform;
        linesParent.SetParent(transform, false);
        linesParent.position = originPos;

        for (int x = 0; x <= width; x++)
        {
            var lr = CreateLineRenderer("v" + x);
            Vector3 a = new Vector3(x * cellSize, 0, localZ);
            Vector3 b = new Vector3(x * cellSize, height * cellSize, localZ);
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
        }

        for (int y = 0; y <= height; y++)
        {
            var lr = CreateLineRenderer("h" + y);
            Vector3 a = new Vector3(0, y * cellSize, localZ);
            Vector3 b = new Vector3(width * cellSize, y * cellSize, localZ);
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
        }
    }

    

    public void DestroyLinesParent()
    {
        if (linesParent == null) return;
        if (Application.isPlaying)
            Destroy(linesParent.gameObject);
        else
            DestroyImmediate(linesParent.gameObject);
        linesParent = null;
    }

    private LineRenderer CreateLineRenderer(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(linesParent, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startColor = lr.endColor = color;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = !useLocalSpace;
        lr.numCapVertices = 0;
        lr.sortingLayerName = sortingLayerName;
        // push sorting order a bit higher to reduce chance of z-fighting with sprites
        lr.sortingOrder = sortingOrder;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.alignment = LineAlignment.View;
        return lr;
    }

#if UNITY_EDITOR
    // Draw grid using explicit parameters (callable from other components)
    public void DrawGizmos(float width, float height, float cellSize, Vector3 originPos)
    {
        if (!drawGizmosInEditor) return;
        Gizmos.color = color;

        Vector3 baseOrigin = originPos;

        // vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 a = baseOrigin + new Vector3(x * cellSize, 0f, localZ);
            Vector3 b = baseOrigin + new Vector3(x * cellSize, height * cellSize, localZ);
            Gizmos.DrawLine(a, b);
        }

        // horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 a = baseOrigin + new Vector3(0f, y * cellSize, localZ);
            Vector3 b = baseOrigin + new Vector3(width * cellSize, y * cellSize, localZ);
            Gizmos.DrawLine(a, b);
        }
    }
#endif
}