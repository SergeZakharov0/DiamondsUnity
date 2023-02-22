using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridTransformLimiter : MonoBehaviour
{
    public WorldGridParams attachedGridParams;
    public Transform attachedTransform;
    void Start()
    {
        attachedTransform.position = new Vector3((int)attachedTransform.position.x, (int)attachedTransform.position.y, (int)attachedTransform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        attachedTransform.position = new Vector3((int)attachedTransform.position.x, (int)attachedTransform.position.y, (int)attachedTransform.position.z);
        if (attachedTransform.position.x >= attachedGridParams.gridSize.x) {
            attachedTransform.position = new Vector3(attachedGridParams.gridSize.x-1, attachedTransform.position.y, attachedTransform.position.z);
        }
        if (attachedTransform.position.x < 0) {
            attachedTransform.position = new Vector3(0, attachedTransform.position.y, attachedTransform.position.z);
        }
        if (attachedTransform.position.y >= attachedGridParams.gridSize.y) {
            attachedTransform.position = new Vector3(attachedTransform.position.x, attachedGridParams.gridSize.y-1, attachedTransform.position.z);
        }
        if (attachedTransform.position.y < 0) {
            attachedTransform.position = new Vector3(attachedTransform.position.x, 0, attachedTransform.position.z);
        }
    }
}
