using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public void MoveTo(Vector2Int newPos) 
    {
        gameObject.transform.position = new Vector3(newPos.x, newPos.y, 0);
    }
}
