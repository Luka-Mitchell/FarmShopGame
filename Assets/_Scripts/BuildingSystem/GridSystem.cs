using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
	///// [SerializeField] private GameObject mouseIndicator;
	[SerializeField] private GameObject cellIndicator;
	//[SerializeField] private InputManager inputManager;
    //[SerializeField] private Grid grid;
	//[SerializeField] private GameObject gridOutline;
	[SerializeField] private float gridHeight = 5;
	[SerializeField] private float gridWidth = 6;
	[SerializeField] private float cellSize = 3;
	[SerializeField] private Camera sceneCamera;
	[SerializeField] private BuildSystemGridRenderer gridRenderer;
	[SerializeField] private BuildingPlacementHighlightRenderer highlightRenderer;

	private Vector3 lastPosition = new(0,0,0);

	void Start()
	{
		if (cellIndicator != null) cellIndicator.SetActive(false); // Disable old indicator
	}

    [ContextMenu("Rebuild render of grid")]
	public void ShowGrid()
	{
		gridRenderer.Rebuild(gridWidth, gridHeight, cellSize, transform.position);
	}

	public void HideGrid()
	{
		gridRenderer.DestroyLinesParent();
		if (highlightRenderer != null) highlightRenderer.DestroyLinesParent();
	}

	public void SnapToGrid(GameObject placingObject, int buildingWidth = 1, int buildingHeight = 1)
	{
		Vector3 mousePos = GetSelectedMapPosition();
		Vector3 originPos = transform.position;

		// local position relative to origin
		float localX = mousePos.x - originPos.x;
		float localY = mousePos.y - originPos.y;

		// calculate cell indices
		int ix = Mathf.FloorToInt(localX / cellSize);
		int iy = Mathf.FloorToInt(localY / cellSize);

		// clamp to valid placement positions (building doesn't go out of bounds)
		ix = Mathf.Clamp(ix, 0, (int)gridWidth - buildingWidth);
		iy = Mathf.Clamp(iy, 0, (int)gridHeight - buildingHeight);

		// Bottom-left corner of building footprint in world space (grid origin is bottom-left)
		Vector3 buildingBottomLeft = originPos + new Vector3(ix * cellSize, iy * cellSize, 0f);
		
		// Center of building footprint
		Vector3 cellCenter = buildingBottomLeft + new Vector3(buildingWidth * cellSize / 2f, buildingHeight * cellSize / 2f, 0f);

		placingObject.transform.position = cellCenter;
		if (highlightRenderer != null) highlightRenderer.UpdateHighlight(buildingBottomLeft, buildingWidth * cellSize, buildingHeight * cellSize, originPos);
	}

	public bool ValidatePlacement(GameObject placingObject, int buildingWidth = 1, int buildingHeight = 1)
	{
		//Collider2D ghostCollider = placingObject.GetComponent<Collider2D>();
		//if (ghostCollider == null) return false;
		//ghostCollider.enabled = false;

		// Check the entire building footprint
		Vector2 footprintSize = new Vector2(buildingWidth * cellSize, buildingHeight * cellSize);
		Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
			placingObject.transform.position,
			footprintSize,
			0f
		);
		//ghostCollider.enabled = true;

		if (hitColliders.Length == 0 || hitColliders.Length == 1 && hitColliders[0].CompareTag("Player"))
		{
			placingObject.GetComponent<SpriteRenderer>().color = Color.green;
			if (highlightRenderer != null) highlightRenderer.color = Color.white;
			return true;
		}
		else
		{
			placingObject.GetComponent<SpriteRenderer>().color = Color.red;
			if (highlightRenderer != null) highlightRenderer.color = Color.red;
			return false;
		}
	}

	private Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = sceneCamera.ScreenToWorldPoint(Input.mousePosition);

		// keep the input position within the grid
		mousePos.x = Mathf.Clamp(mousePos.x, transform.position.x + cellSize/2, transform.position.x + gridWidth * cellSize - cellSize/2);
		mousePos.y = Mathf.Clamp(mousePos.y, transform.position.y + cellSize/2, transform.position.y + gridHeight * cellSize - cellSize/2);

		lastPosition = mousePos;
		return lastPosition;

		/*
		Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
		mousePos.z = sceneCamera.nearClipPlane;
		Ray ray = sceneCamera.ScreenPointToRay(mousePos);
		RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, placementLayerMask);
        if (Physics.Raycast(ray, out hit, placementLayerMask))
        {
            lastPosition = hit.point;
		}
		mousePos.z = 0f;
        */
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (gridRenderer == null) return;
		gridRenderer.DrawGizmos(gridWidth, gridHeight, cellSize, transform.position);
	}
#endif
}
