using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public Vector3 previousPosition;
    void Update()
    {
        if (IsClient) {
            if (Application.isMobilePlatform) {
                // Touch screen controls
            } else {
                // Mouse controls
                if (Input.GetMouseButtonDown(0)) {
                    Vector3 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    SubmitPositionRequestServerRpc(new Vector3(clickedPos.x + 1, clickedPos.y, 0));
                }
            }  
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 newPos)
    {
        previousPosition = gameObject.transform.position;
        gameObject.transform.position = newPos;
    }
}
