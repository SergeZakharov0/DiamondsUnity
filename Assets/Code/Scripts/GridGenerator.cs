using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

public class GridGenerator : NetworkBehaviour
{
    private GridOptions gridOptions;
    
    public override void OnNetworkSpawn() {
        if (IsServer) {
            gridOptions = gameObject.GetComponents<GridOptions>()[0];
            GenerateInitialField();
        }
    }

    public void GenerateInitialField() {
        // Generate diamonds and top line
        for (int i = 0; i < gridOptions.gridSize.x; ++i) {
            for (int j = 0; j < gridOptions.gridSize.y + 1; ++j) {
                GameObject cur = Instantiate(gridOptions.diamondPrefabs[Random.Range(0, gridOptions.diamondPrefabs.Count)], new Vector3(i, j, 0), Quaternion.identity, gameObject.transform);
                cur.transform.parent = gameObject.transform;
                cur.GetComponent<NetworkObject>().Spawn();
            }
        }

        // Generate bottom line
        for (int i = 0; i < gridOptions.gridSize.x; ++i) {
            GameObject cur = Instantiate(gridOptions.bottomLineBoxPrefab, new Vector3(i, -1, 0), Quaternion.identity, gameObject.transform);
            cur.transform.parent = gameObject.transform;
            cur.GetComponent<NetworkObject>().Spawn();
        }
    }

    void Update() {
        if (!IsSpawned || !IsServer)
        {
            return;
        }

        // Check additional hidden diamonds, generate if necessary
        for (int i = 0; i < gridOptions.gridSize.x; ++i) {
            Collider2D[] res = Physics2D.OverlapPointAll(new Vector2(i, gridOptions.gridSize.y - 0.02f));
            if (res.Length == 0) {
                GameObject cur = Instantiate(gridOptions.diamondPrefabs[Random.Range(0, gridOptions.diamondPrefabs.Count)], new Vector3(i, gridOptions.gridSize.y - 0.02f, 0), Quaternion.identity, gameObject.transform);
                cur.transform.parent = gameObject.transform;
                cur.GetComponent<NetworkObject>().Spawn();
            }
        }

    }

}
