using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goober : EnemyClass
{
    [Header("Pathfinding")]
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [Header("Debug")]
    [SerializeReference] private float distanceCheck;

    void Update()
    {
        //see how far its able to look/go
        Debug.DrawRay(transform.position, Vector2.up * distanceCheck, Color.yellow);
    }

    public void CreatePath()
    {
        if(path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), 3 * Time.deltaTime);

            if(Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();
            while(path == null || path.Count == 0)
            {
                path = AStarManager.instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
            }
        }
    }
}
