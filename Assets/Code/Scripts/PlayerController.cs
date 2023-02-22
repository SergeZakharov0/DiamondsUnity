using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Vector2Int currentCellPos;
    public bool isLockedInAnimation = false;
    public bool isSelected = false;
    private DiamondGrid diamondGrid;
    private PlayerMovement playerMovement;

    void Start()
    {
        diamondGrid = gameObject.GetComponentInParent<DiamondGrid>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (isLockedInAnimation) {
            return;
        }
        
        if (!isSelected) {
            //currentCellPos = new Vector2Int((int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 1.5), (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).y + 0.5));
            if (Input.GetKeyDown(KeyCode.D)) 
            {
                currentCellPos += new Vector2Int(1,0);
            }
            if (Input.GetKeyDown(KeyCode.A)) 
            {
                currentCellPos += new Vector2Int(-1,0);
            }
            if (Input.GetKeyDown(KeyCode.W)) 
            {
                currentCellPos += new Vector2Int(0,1);
            }
            if (Input.GetKeyDown(KeyCode.S)) 
            {
                currentCellPos += new Vector2Int(0,-1);
            }
            if (Input.GetKeyDown(KeyCode.Space) ) 
            {
                isSelected = true;        
            }
            if (currentCellPos.x >= diamondGrid.gridSize.x) {
                currentCellPos.x = diamondGrid.gridSize.x - 1;
            }
            if (currentCellPos.y >= diamondGrid.gridSize.y) {
                currentCellPos.y = diamondGrid.gridSize.y - 1;
            }
            if (currentCellPos.x < 0) {
                currentCellPos.x = 0;
            }
            if (currentCellPos.y < 0) {
                currentCellPos.y = 0;
            }
            playerMovement.MoveTo(currentCellPos);
        } else {
            if (Input.GetKeyDown(KeyCode.D)) 
            {
                TriggerSwap(currentCellPos, currentCellPos + new Vector2Int(1,0));
            }
            if (Input.GetKeyDown(KeyCode.A)) 
            {
                TriggerSwap(currentCellPos, currentCellPos + new Vector2Int(-1,0));
            }
            if (Input.GetKeyDown(KeyCode.W)) 
            {
                TriggerSwap(currentCellPos, currentCellPos + new Vector2Int(0,1));
            }
            if (Input.GetKeyDown(KeyCode.S)) 
            {
                TriggerSwap(currentCellPos, currentCellPos + new Vector2Int(0,-1));
            }
        }        
    }

    void TriggerSwap(Vector2Int firstCell, Vector2Int secondCell) 
    {
        diamondGrid.SwapDiamonds(firstCell, secondCell);
        isSelected = false;
    }
}
