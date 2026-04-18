using UnityEngine;

[ExecuteAlways]
public class BuildingPlacementHighlightRenderer : MonoBehaviour
{
    public Material lineMaterial; // assign Unlit/Color or Sprites/Default
    private Color _color = Color.white;
    public Color color
    {
        get { return _color; }
        set
        {
            _color = value;
            UpdateColors();
        }
    }
    public float lineWidth = 0.05f;
    public string sortingLayerName = "Default";
    public int sortingOrder = 1; // higher than grid
    [Tooltip("Small local Z offset for highlight lines to avoid z-fighting")]
    public float localZ = -0.1f;
    [Tooltip("If true the LineRenderers will be created in local space and parented to this transform.")]
    public bool useLocalSpace = true;

    private Transform linesParent;
    private LineRenderer[] lineRenderers; // 4 lines for the rectangle

    void OnDisable() { DestroyLinesParent(); }

    public void UpdateHighlight(Vector3 bottomLeft, float width, float height, Vector3 originPos)
    {
        if (!lineMaterial) return;
        if (linesParent == null)
        {
            CreateLinesParent(originPos);
        }

        // Update positions if needed
        linesParent.position = originPos;

        // Convert world bottomLeft to local position relative to originPos
        Vector3 localBottomLeft = bottomLeft - originPos;

        // Define the 4 corners in local space
        Vector3 bl = localBottomLeft + new Vector3(0, 0, localZ);
        Vector3 br = localBottomLeft + new Vector3(width, 0, localZ);
        Vector3 tr = localBottomLeft + new Vector3(width, height, localZ);
        Vector3 tl = localBottomLeft + new Vector3(0, height, localZ);

        // Bottom line
        lineRenderers[0].SetPosition(0, bl);
        lineRenderers[0].SetPosition(1, br);

        // Right line
        lineRenderers[1].SetPosition(0, br);
        lineRenderers[1].SetPosition(1, tr);

        // Top line
        lineRenderers[2].SetPosition(0, tr);
        lineRenderers[2].SetPosition(1, tl);

        // Left line
        lineRenderers[3].SetPosition(0, tl);
        lineRenderers[3].SetPosition(1, bl);
    }

    private void CreateLinesParent(Vector3 originPos)
    {
        if (linesParent) DestroyLinesParent();
        linesParent = new GameObject("HighlightLines").transform;
        linesParent.SetParent(transform, false);
        linesParent.position = originPos;

        lineRenderers = new LineRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            var lr = CreateLineRenderer("highlight" + i);
            lineRenderers[i] = lr;
        }
        UpdateColors();
    }

    public void DestroyLinesParent()
    {
        if (linesParent == null) return;
        if (Application.isPlaying)
            Destroy(linesParent.gameObject);
        else
            DestroyImmediate(linesParent.gameObject);
        linesParent = null;
        lineRenderers = null;
    }

    private LineRenderer CreateLineRenderer(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(linesParent, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startColor = lr.endColor = _color;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = !useLocalSpace;
        lr.numCapVertices = 0;
        lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.alignment = LineAlignment.View;
        return lr;
    }

    public void HideHighlight()
    {
        if (linesParent != null)
        {
            linesParent.gameObject.SetActive(false);
        }
    }

    private void UpdateColors()
    {
        if (lineRenderers != null)
        {
            foreach (var lr in lineRenderers)
            {
                if (lr != null)
                {
                    lr.startColor = lr.endColor = _color;
                }
            }
        }
    }
}