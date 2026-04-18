using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	/// obsolete class, now integrated into GridSystem

    [SerializeField] private Camera sceneCamera;
    [SerializeField] private GameObject gridOutline;
	//[SerializeField] private Grid grid;
	[SerializeField] private int gridSize = 6;
	[SerializeField] private int cellSize = 5;
    //[SerializeField] private LayerMask placementLayerMask;

    private Vector3 lastPosition = new Vector3(0,0,0);

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = sceneCamera.ScreenToWorldPoint(Input.mousePosition);

		// keep the input position within the grid
		if (Mathf.Abs(mousePos.x - gridOutline.transform.position.x) < 3 * 5
		 && Mathf.Abs(mousePos.y - gridOutline.transform.position.y) < 3 * 5)
		{
			lastPosition = mousePos;
		}
		else
		{
			float gridRad = gridSize/2 * cellSize;
			float halfCell = cellSize/2;
			Vector3 gridPos = gridOutline.transform.position;
			mousePos.x = Mathf.Clamp(mousePos.x, -gridRad + gridPos.x + halfCell, gridRad + gridPos.x - halfCell);
			mousePos.y = Mathf.Clamp(mousePos.y, -gridRad + gridPos.y + halfCell, gridRad + gridPos.y - halfCell);
			lastPosition = mousePos;
		}

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
}
