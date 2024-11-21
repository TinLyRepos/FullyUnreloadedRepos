using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    // Builds a path for the room, from the startGridPos to the endGridPos
    // And adds movement steps to the returned Stack.
    // Returns null if no path is found.
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPos, Vector3Int endGridPos)
    {
        // Adjust positions by lower bounds
        startGridPos -= (Vector3Int)room.templateLowerBounds;
        endGridPos -= (Vector3Int)room.templateLowerBounds;

        // Create open list and closed hashset
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // Create gridnodes for path finding
        GridNode gridNodes = new GridNode(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetNode(startGridPos.x, startGridPos.y);
        Node targetNode = gridNodes.GetNode(endGridPos.x, endGridPos.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);

        if (endPathNode != null)
            return CreatePathStack(endPathNode, room);

        return null;
    }

    // Find the shortest path
    // returns the end Node if a path has been found, else returns null.
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNode gridNodes,
        List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // Add start node to open list
        openNodeList.Add(startNode);

        // Loop through open node list until empty
        while (openNodeList.Count > 0)
        {
            // Sort List
            openNodeList.Sort();

            // current node = the node in the open list with the lowest fCost
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // if the current node = target node then finish
            if (currentNode == targetNode)
                return currentNode;

            // add current node to the closed list
            closedNodeHashSet.Add(currentNode);

            // evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes,
                openNodeList, closedNodeHashSet, instantiatedRoom);
        }
        return null;
    }

    //  Create a Stack<Vector3> containing the movement path 
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        // Get mid point of cell
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // Convert grid position to world position
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(
                new Vector3Int(
                    nextNode.gridPosition.x + room.templateLowerBounds.x,
                    nextNode.gridPosition.y + room.templateLowerBounds.y,
                    0));

            // Set the world position to the middle of the grid cell
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    // Evaluate neighbour nodes
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNode gridNodes,
        List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);

                if (validNeighbourNode != null)
                    continue;

                // Calculate new gcost for neighbour
                int newCostToNeighbour;

                // Get the movement penalty
                // Unwalkable paths have a value of 0. Default movement penalty is set in
                // Settings and applies to other grid squares.
                int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                {
                    validNeighbourNode.gCost = newCostToNeighbour;
                    validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                    validNeighbourNode.parentNode = currentNode;

                    if (!isValidNeighbourNodeInOpenList)
                    {
                        openNodeList.Add(validNeighbourNode);
                    }
                }
            }
        }
    }

    //Returns the distance int between nodeA and nodeB
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        // To avoid using floats
        // 10 used instead of 1, and 14 is a pythagoras approximation SQRT(10*10 + 10*10)

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }

    // Evaluate a neighbour node at neighboutNodeXPosition, neighbourNodeYPosition
    // using the specified gridNodes, closedNodeHashSet, and instantiated room.
    // Returns null if the node isn't valid
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNode gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // If neighbour node position is beyond grid then return null
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x ||
            neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y ||
            neighbourNodeXPosition < 0 ||
            neighbourNodeYPosition < 0)
            return null;

        // Get neighbour node
        Node neighbourNode = gridNodes.GetNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // check for obstacle at that position
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        // check for moveable obstacle at that position
        int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];

        // if neighbour is an obstacle or neighbour is in the closed list then skip
        if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
            return null;

        return neighbourNode;
    }
}