using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHide : MonoBehaviour
{
    public NodesGrid grid;
    public Node positionNode;
    public GameObject player;
    public float speed = 1.0f;

    public int rangeToHide = 3;
    public bool isMoving = false;
    Vector3 hidePosition;

    // Start is called before the first frame update
    void Start()
    {
        positionNode = grid.NodeFromWorldPoint(gameObject.transform.position);
        hidePosition = transform.position;

        InvokeRepeating("HideFromPlayer", 0f, 0.5f);
    }

    void HideFromPlayer()
    {
        if(!isMoving && !IsHidden(transform.position, player.transform.position))
        {
            isMoving = true;
            grid.FindPath(transform.position, GetNodeToHide().position);
        }
    }

    bool IsHidden(Vector3 npcPosition, Vector3 playerPosition)
    {
        RaycastHit hit;
        Vector3 direction = playerPosition - npcPosition;
        bool isHidden = Physics.Raycast(npcPosition, direction, out hit, direction.magnitude, grid.obstacleLayers);
        if(isHidden)
            Debug.DrawRay(npcPosition, direction.normalized * hit.distance, Color.green);
        else
            Debug.DrawRay(npcPosition, direction, Color.red);
        return isHidden;
    }

    Node GetNodeToHide()
    {
        //TODO: if there is no obstacles between npc and player move npc to furthest node
        Node nodeToHide = positionNode;

        foreach(Node node in grid.GetNearestNodesInRange(positionNode, rangeToHide))
        {
            if (!node.walkable)
                continue;

            if(IsHidden(node.position, player.transform.position))
            {
                nodeToHide = node;
                break;
            }
        }

        return nodeToHide;
    }

    void Update()
    {
        if (isMoving)
        {
            
            if(grid.path.Count > 0)
            {
                float dist = Vector3.Distance(transform.position, hidePosition);

                if(dist < 0.001f)
                {
                    hidePosition = grid.path[0].position;
                    grid.path.RemoveAt(0);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, hidePosition) < 0.001f)
                    isMoving = false;
            }
            
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, hidePosition, step);
        }
    }
}
