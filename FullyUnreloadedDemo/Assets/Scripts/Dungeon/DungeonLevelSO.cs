using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Header("BASIC LEVEL DETAILS")]
    [SerializeField] private string levelName = string.Empty;
    public string Name => levelName;
    [Space]
    [Header("ROOM TEMPLATE")]
    public List<SO_RoomTemplate> roomTemplateList = default;
    [Space]
    [Header("ROOM NODE GRAPH")]
    public List<SO_MapNodeGraph> roomNodeGraphList = default;

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        // Room templates need to be specified for all the room node types in the graph
        bool isCorridor_Horizontal = false;
        bool isCorridor_Vertical = false;
        bool isEntrance = false;

        // Loop through all the room templates to check that this node type has been specified
        foreach (SO_RoomTemplate roomTemplate in roomTemplateList)
        {
            if (roomTemplate == null)
                return;

            if (roomTemplate.roomNodeType.isCorridor_Horizontal)
                isCorridor_Horizontal = true;

            if (roomTemplate.roomNodeType.isCorridor_Vertical)
                isCorridor_Vertical = true;

            if (roomTemplate.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isCorridor_Horizontal == false)
            Debug.Log($"In {this.name} : No Horizontal Corridor Room Type Specified!");

        if (isCorridor_Vertical == false)
            Debug.Log($"In {this.name} : No Vertical Corridor Room Type Specified!");

        if (isEntrance == false)
            Debug.Log($"In {this.name} : No Entrance Room Type Specified!");

        // Loop through all node graph
        foreach (SO_MapNodeGraph roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // Loop through all nodes in node graph
            foreach (SO_MapNode roomNode in roomNodeGraph.nodeList)
            {
                if (roomNode == null)
                    continue;

                // Corridors and entrance already checked
                if (roomNode.roomNodeType.isEntrance || roomNode.roomNodeType.isCorridor || roomNode.roomNodeType.isNone ||
                    roomNode.roomNodeType.isCorridor_Horizontal || roomNode.roomNodeType.isCorridor_Vertical)
                    continue;

                bool isRoomNodeTypeFound = false;

                // Loop through all room templates to check that this node type has been specified
                foreach (SO_RoomTemplate roomTemplate in roomTemplateList)
                {
                    if (roomTemplate == null)
                        continue;

                    if (roomTemplate.roomNodeType == roomNode.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (isRoomNodeTypeFound == false)
                    Debug.Log($"In {this.name}: No room template {roomNode.roomNodeType.name} found for node graph {roomNodeGraph.name}");
            }
        }
    }
#endif
}