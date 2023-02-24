using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class GridController : NetworkBehaviour
{   
    private GridOptions gridOptions;

    private GameObject[,] gridObjects;

    void Start() {
        gridOptions = gameObject.GetComponents<GridOptions>()[0];
        gridObjects = new GameObject[gridOptions.gridSize.x, gridOptions.gridSize.y];
    }

    bool BuildObjectGrid() {
         for (int i = 0; i < gridOptions.gridSize.x; ++i) {
            for (int j = 0; j < gridOptions.gridSize.y; ++j) {
                Collider2D curCollider = Physics2D.OverlapPoint(new Vector2(i, j));
                if (curCollider == null) {
                    return false;
                }
                gridObjects[i, j] = curCollider.gameObject;
            }
        }
        return true;
    }

    public bool IsSomethingMoving() {
        Rigidbody2D[] bodies = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach (Rigidbody2D body in bodies) {
            if (body.velocity.y < -0.5) {
                return true;
            }
        }
        return false;
    }

    public void SwapTwoDiamonds(Vector2Int firstObjPos, Vector2Int secondObjPos) 
    {
        GameObject firstObj = gridObjects[firstObjPos.x, firstObjPos.y];
        GameObject secondObj = gridObjects[secondObjPos.x, secondObjPos.y];

        // Swap positions
        Vector3 pos = firstObj.transform.position;
        firstObj.transform.position = secondObj.transform.position;
        secondObj.transform.position = pos;

        // Swap in the array
        GameObject temp = gridObjects[firstObjPos.x, firstObjPos.y];
        gridObjects[firstObjPos.x, firstObjPos.y] = gridObjects[secondObjPos.x, secondObjPos.y];
        gridObjects[secondObjPos.x, secondObjPos.y] = temp;
    }

    bool CheckPoint(Vector2Int point) {
        return point.x >= 0 && point.x < gridOptions.gridSize.x && point.y >= 0 && point.y < gridOptions.gridSize.y;
    }

    bool CheckForCommonType(GameObject firstObj, GameObject secondObj) {
        return firstObj.tag == secondObj.tag;
    }

    Vector2Int ConvertToGridPoint(Vector3 position) {
        return new Vector2Int((int)(position.x + 0.5), (int)(position.y + 0.5));
    }
    
    void PerformSwaps() {
        PlayerController[] controllers = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController controller in controllers) {
            if (controller.previousPosition.x != 0) {
                Vector2Int firstPoint = ConvertToGridPoint(controller.gameObject.transform.position);
                Vector2Int secondPoint = ConvertToGridPoint(controller.previousPosition);
                if (!CheckPoint(firstPoint) || !CheckPoint(secondPoint)) {
                    continue;
                }

                if ((Mathf.Abs(firstPoint.x - secondPoint.x) == 1 && Mathf.Abs(firstPoint.y - secondPoint.y) == 0) 
                    || (Mathf.Abs(firstPoint.y - secondPoint.y) == 1 && Mathf.Abs(firstPoint.x - secondPoint.x) == 0)) {
                    SwapTwoDiamonds(firstPoint, secondPoint);
                }
                controller.gameObject.transform.position = new Vector3(0, 0, 0);
                controller.previousPosition = new Vector3(0, 0, 0);
            }
        }
    }

    void BuildConnectedComponent(GameObject currentObject, List<GameObject> collectedObjects, int[] xCounters, int[] yCounters) {
        Vector2Int originalPoint = ConvertToGridPoint(currentObject.transform.position);
        Vector2Int[] pointsToCheck = {new Vector2Int(originalPoint.x + 1, originalPoint.y), new Vector2Int(originalPoint.x - 1, originalPoint.y), new Vector2Int(originalPoint.x, originalPoint.y + 1), new Vector2Int(originalPoint.x, originalPoint.y - 1)};
        collectedObjects.Add(currentObject);
        gridObjects[originalPoint.x, originalPoint.y] = null;
        xCounters[originalPoint.x] += 1;
        yCounters[originalPoint.y] += 1;

        foreach (Vector2Int checkPoint in pointsToCheck) {
            if (CheckPoint(checkPoint)) {
                GameObject checkObj = gridObjects[checkPoint.x, checkPoint.y];
                if (checkObj == null) {
                    continue;
                }
                if (!CheckForCommonType(currentObject, checkObj)) {
                    continue;
                }
                BuildConnectedComponent(checkObj, collectedObjects, xCounters, yCounters);
            }
        }
    }

    void CheckForSolutions() {
        for (int i = 0; i < gridOptions.gridSize.x; ++i) {
            for (int j = 0; j < gridOptions.gridSize.y; ++j) {
                // Get the current object
                GameObject curObj = gridObjects[i, j];
                if (curObj == null) {
                    continue;
                }
                
                // Build the object component
                int[] xCounters = new int[gridOptions.gridSize.x];
                int[] yCounters = new int[gridOptions.gridSize.y];
                List<GameObject> componentObjects = new List<GameObject>();
                BuildConnectedComponent(curObj, componentObjects, xCounters, yCounters);

                // Check if need to destroy & destroy
                if (Mathf.Max(xCounters.Max(), yCounters.Max()) >= 3) {
                    foreach (GameObject objToDestroy in componentObjects) {
                        Destroy(objToDestroy);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (!IsServer) {
            return;
        }
        if (!IsSomethingMoving() && BuildObjectGrid()) {
            PerformSwaps();
            CheckForSolutions();
        }
    }
}
