using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DiamondGrid : MonoBehaviour
{
    public List<GameObject> diamondPrefabs;
    public GameObject bottomLineBoxPrefab;
    public Vector2Int gridSize = new Vector2Int(1, 8);    
    private PlayerController[] controllers;
    void Start()
    {
        controllers = gameObject.GetComponentsInChildren<PlayerController>();
        // Generate diamonds and top line
        for (int i = 0; i < gridSize.x; ++i) {
            for (int j = 0; j < gridSize.y + 1; ++j) {
                GameObject cur = Instantiate(diamondPrefabs[Random.Range(0, 4)], new Vector3(i, j, 0), Quaternion.identity, gameObject.transform);
            }
        }

        // Generate bottom line
        for (int i = 0; i < gridSize.x; ++i) {
            GameObject cur = Instantiate(bottomLineBoxPrefab, new Vector3(i, -1, 0), Quaternion.identity, gameObject.transform);
        }
    }

    public bool IsSomethingMoving() {
        Rigidbody2D[] childBodies = gameObject.GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D body in childBodies) {
            if (body.velocity.y < -0.5) {
                return true;
            }
        }
        return false;
    }

    public void CheckForSolutions() {
        // Resolve vertical solutions
        for (int i = 0; i < gridSize.x; ++i) {
            List<GameObject> equalDiamonds = new List<GameObject>();
            string lastDiamondTag = "";
            for (int j = 0; j < gridSize.y; ++j) {
                Collider2D[] curPoint = Physics2D.OverlapPointAll(new Vector2(i, j));
                if (curPoint.Length == 0) {
                    lastDiamondTag = "";
                    if (equalDiamonds.Count >= 3) {
                        foreach (GameObject delObj in equalDiamonds) {
                            Destroy(delObj);
                        }
                    }
                    equalDiamonds.Clear();
                } else {
                    GameObject curDiamondObj = curPoint[0].gameObject;
                    string curDiamondTag = curPoint[0].gameObject.tag;
                    if (lastDiamondTag == curDiamondTag) {
                        equalDiamonds.Add(curDiamondObj);
                    } else {
                    if (equalDiamonds.Count >= 3) {
                        foreach (GameObject delObj in equalDiamonds) {
                            Destroy(delObj);
                        }
                    }
                    equalDiamonds.Clear();
                    equalDiamonds.Add(curDiamondObj);
                    lastDiamondTag = curDiamondTag;
                    }
                }
            }
            if (equalDiamonds.Count >= 3) {
                foreach (GameObject delObj in equalDiamonds) {
                    Destroy(delObj);
                }
            }
        }

        // Resolve horizontal solutions
        for (int j = 0; j < gridSize.y; ++j) {
            List<GameObject> equalDiamonds = new List<GameObject>();
            string lastDiamondTag = "";
            for (int i = 0; i < gridSize.x; ++i) {
                Collider2D[] curPoint = Physics2D.OverlapPointAll(new Vector2(i, j));
                if (curPoint.Length == 0) {
                    lastDiamondTag = "";
                    equalDiamonds.Clear();
                } else {
                    GameObject curDiamondObj = curPoint[0].gameObject;
                    string curDiamondTag = curPoint[0].gameObject.tag;
                    if (lastDiamondTag == curDiamondTag) {
                        equalDiamonds.Add(curDiamondObj);
                    } else {
                    if (equalDiamonds.Count >= 3) {
                        foreach (GameObject delObj in equalDiamonds) {
                            Destroy(delObj);
                        }
                    }
                    equalDiamonds.Clear();
                    equalDiamonds.Add(curDiamondObj);
                    lastDiamondTag = curDiamondTag;
                    }
                }
            }
            if (equalDiamonds.Count >= 3) {
                foreach (GameObject delObj in equalDiamonds) {
                    Destroy(delObj);
                }
            }
        }
    }

    void Update()
    {
        // Check additional hidden diamonds, generate if necessary
        for (int i = 0; i < gridSize.x; ++i) {
            Collider2D[] res = Physics2D.OverlapPointAll(new Vector2(i, gridSize.y - 0.02f));
            if (res.Length == 0) {
                GameObject cur = Instantiate(diamondPrefabs[Random.Range(0, 4)], new Vector3(i, gridSize.y - 0.02f, 0), Quaternion.identity, gameObject.transform);
            }
        }

        if (!IsSomethingMoving()) {
            CheckForSolutions();
            controllers[0].isLockedInAnimation = false;
        } else {
            controllers[0].isLockedInAnimation = true;
        }
    }

    public void SwapDiamonds(Vector2Int firstCell, Vector2Int secondCell) 
    {
        // Get first object
        Collider2D[] firstColliders = Physics2D.OverlapPointAll(firstCell);
        if (firstColliders.Length == 0) {
            return;
        }
        GameObject firstObj = firstColliders[0].gameObject;

        // Get second object
        Collider2D[] secondColliders = Physics2D.OverlapPointAll(secondCell);
        if (secondColliders.Length == 0) {
            return;
        }
        GameObject secondObj = secondColliders[0].gameObject;

        // Swap
        Vector3 pos = firstObj.transform.position;
        firstObj.transform.position = secondObj.transform.position;
        secondObj.transform.position = pos;
    }
}
