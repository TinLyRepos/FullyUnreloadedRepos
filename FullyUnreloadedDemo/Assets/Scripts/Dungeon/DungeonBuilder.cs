using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    [SerializeField] private Dictionary<string, Room> roomDictionary = new();

    private Dictionary<string, SO_RoomTemplate> roomTemplateDictionary = new();
    private List<SO_RoomTemplate> roomTemplateList = null;
    private SO_NodeTypeList roomNodeTypeList = null;

    private bool buildSuccessful;

    //===========================================================================
    protected override void Awake()
    {
        base.Awake();

        // Load the room node type newList
        LoadRoomNodeTypeList();
    }

    private void OnEnable()
    {
        // Set dimmed material to off
        GameResources.Instance.material_dimmed.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        // Set dimmed material to fully visible
        GameResources.Instance.material_dimmed.SetFloat("Alpha_Slider", 1f);
    }

    //===========================================================================
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.nodeTypeList;
    }

    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();

        // Load room template newList into dictionary
        foreach (SO_RoomTemplate roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
            }
        }
    }

    private SO_MapNodeGraph GetRandomRoomNodeGraph(List<SO_MapNodeGraph> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count == 0)
        {
            Debug.Log("No room node graphs in list");
            return null;
        }

        return roomNodeGraphList[Random.Range(0, roomNodeGraphList.Count)];
    }

    private void ClearDungeon()
    {
        if (roomDictionary.Count == 0)
            return;

        foreach (KeyValuePair<string, Room> keyValuePair in roomDictionary)
        {
            Room room = keyValuePair.Value;

            // If room already created => delete it
            if (room.instantiatedRoom != null)
                Destroy(room.instantiatedRoom.gameObject);
        }

        roomDictionary.Clear();
    }

    private Room GetRoom(string roomID)
    {
        if (roomDictionary.TryGetValue(roomID, out Room room))
            return room;

        return null;
    }

    //===========================================================================
    private List<string> CopyStringList(List<string> sourcelist)
    {
        List<string> newList = new List<string>();

        foreach (string source in sourcelist)
            newList.Add(source);

        return newList;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> sourcelist)
    {
        List<Doorway> newList = new List<Doorway>();

        foreach (Doorway doorway in sourcelist)
        {
            Doorway newDoorway = new Doorway();
            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newList.Add(newDoorway);
        }
        return newList;
    }

    private SO_RoomTemplate GetRandomRoomTemplate(SO_NodeType roomNodeType)
    {
        List<SO_RoomTemplate> matchingRoomTemplateList = new List<SO_RoomTemplate>();

        // Loop through room template newList
        foreach (SO_RoomTemplate roomTemplate in roomTemplateList)
        {
            // Add matching room templates
            if (roomTemplate.roomNodeType == roomNodeType)
                matchingRoomTemplateList.Add(roomTemplate);
        }

        // Return null of newList is empty
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Select random room template from newList and return
        return matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private IEnumerable<Doorway> GetValidDoorways(List<Doorway> doorwayList)
    {
        // Loop through doorway list
        foreach (Doorway doorway in doorwayList)
        {
            if (doorway.isConnected == false && doorway.isUnavailable == false)
                yield return doorway;
        }
    }

    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {
        //switch (parentDoorway.orientation)
        //{
        //    case Orientation.North:
        //        return doorwayList.Find(x => x.orientation == Orientation.South);
        //    case Orientation.East:
        //        return doorwayList.Find(x => x.orientation == Orientation.West);
        //    case Orientation.South:
        //        return doorwayList.Find(x => x.orientation == Orientation.North);
        //    case Orientation.West:
        //        return doorwayList.Find(x => x.orientation == Orientation.East);
        //    case Orientation.None:
        //    default:
        //        return null;
        //}

        foreach (Doorway dw in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.East && dw.orientation == Orientation.West)
                return dw;
            else if (parentDoorway.orientation == Orientation.West && dw.orientation == Orientation.East)
                return dw;
            else if (parentDoorway.orientation == Orientation.North && dw.orientation == Orientation.South)
                return dw;
            else if (parentDoorway.orientation == Orientation.South && dw.orientation == Orientation.North)
                return dw;
        }

        return null;
    }

    private bool IsRoomOverlapping(Room r1, Room r2)
    {
        bool overlappingX = HelperUtilities.IsIntervalOverlapping(r1.lowerBounds.x, r1.upperBounds.x, r2.lowerBounds.x, r2.upperBounds.x);
        bool overlappingY = HelperUtilities.IsIntervalOverlapping(r1.lowerBounds.y, r1.upperBounds.y, r2.lowerBounds.y, r2.upperBounds.y);

        if (overlappingX && overlappingY)
            return true;

        return false;
    }

    private Room TryGetOverlappingRoom(Room roomToCheck)
    {
        foreach (KeyValuePair<string, Room> keyValuePair in roomDictionary)
        {
            Room room = keyValuePair.Value;

            // Skip if same room as roomToCheck or room hasn't been positioned
            if (room.id == roomToCheck.id || room.isPositioned == false)
                continue;

            if (IsRoomOverlapping(roomToCheck, room))
                return room;
        }

        return null;
    }

    private SO_RoomTemplate GetRandomTemplateForRoomConsistentWithParent(SO_Node roomNode, Doorway doorway)
    {
        SO_RoomTemplate roomTemplate = null;

        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorway.orientation)
            {
                case Orientation.North:
                case Orientation.South:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridor_Vertical));
                    break;
                case Orientation.East:
                case Orientation.West:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridor_Horizontal));
                    break;
                case Orientation.None:
                    break;
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }
        return roomTemplate;
    }

    private Room TryBuildRoomFromRoomTemplate(SO_Node roomNode, SO_RoomTemplate roomTemplate)
    {
        // Initialize room from template
        Room room = new Room();

        room.id = roomNode.guid;
        room.templateID = roomTemplate.guid;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;

        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.spawnPositionArray = roomTemplate.spawnPositionArray;

        room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;

        room.roomIDList_Child = CopyStringList(roomNode.roomNodeIDList_Child);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        // Set parent ID for room
        if (roomNode.roomNodeIDList_Parent.Count == 0) // Entrance
        {
            room.roomID_parent = string.Empty;
            room.isVisited = true;

            // Set Entrance in game manager
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.roomID_parent = roomNode.roomNodeIDList_Parent[0];
        }

        // If there are no enemies to spawn then default the room to be clear of enemies
        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
            room.isCleared = true;

        return room;
    }

    private bool TryPlaceRoom(Room parentRoom, Doorway parentDoorway, Room room)
    {
        // Get current room doorway position
        Doorway doorway = GetOppositeDoorway(parentDoorway, room.doorwayList);

        // Return if no room doorway position
        if (doorway == null)
        {
            // Just mark the parent doorway as unvailable so we don't try to connect it again
            parentDoorway.isUnavailable = true;

            return false;
        }

        // Calculate "world" grid positon of parent doorway
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + parentDoorway.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect
        // (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)
        switch (doorway.orientation)
        {
            case Orientation.North:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.East:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.South:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.West:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.None:
                break;
            default:
                break;
        }

        // Calculate room lower bounds and upper bounds based on positioning the aligh with parent doorway
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = TryGetOverlappingRoom(room);
        if (overlappingRoom == null)
        {
            // Mark doorway as connected and unavaiable
            parentDoorway.isConnected = true;
            parentDoorway.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            return true;
        }
        else
        {
            // Mark doorway as unavaiable to prevent future check
            parentDoorway.isUnavailable = true;
            return false;
        }
    }

    private bool TryPlaceRoomWithNoOverlaps(SO_Node roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        // Try to place against all available doorways of the parent
        // until the room can be placed with no overlap
        while (roomOverlaps)
        {
            // Select random unconnected available doorway for parent
            List<Doorway> validDoorwaysList = GetValidDoorways(parentRoom.doorwayList).ToList();

            if (validDoorwaysList.Count == 0)
                return false; // room overlaps

            Doorway parentDoorway = validDoorwaysList[Random.Range(0, validDoorwaysList.Count)];

            // Get a random room template for room node that is consistent with the parent door orientation
            SO_RoomTemplate roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, parentDoorway);

            // Create a room
            Room room = TryBuildRoomFromRoomTemplate(roomNode, roomTemplate);

            // Place the room - return true if no room overlaps
            if (TryPlaceRoom(parentRoom, parentDoorway, room))
            {
                // If room doesn't overlap then set to false to exit while loop
                roomOverlaps = false;

                room.isPositioned = true;

                // Add room to dictionary
                roomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true; // no room overlaps
    }

    private bool ProcessRoomsInOpenRoomNodeQueue(SO_MapNodeGraph roomNodeGraph, Queue<SO_Node> openRoomNodeQueue, bool noRoomOverlaps)
    {
        // While room nodes in openQueue & no room overlaps detected
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // Get next room node from open room node queue
            SO_Node roomNode = openRoomNodeQueue.Dequeue();

            // Add child Nodes to queue from room node graph (with links to this parent room)
            foreach (SO_Node childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // If the room is the entrance mark as positioned and add to room dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                SO_RoomTemplate roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
                Room room = TryBuildRoomFromRoomTemplate(roomNode, roomTemplate);
                room.isPositioned = true;

                // Add to dictionary
                roomDictionary.Add(room.id, room);
            }

            // If the room is not entrance
            else
            {
                // Get parent room for node
                Room parentRoom = roomDictionary[roomNode.roomNodeIDList_Parent[0]];

                // Check if room can be placed with out overlaps
                noRoomOverlaps = TryPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    private bool TryBuildRandomDungeon(SO_MapNodeGraph roomNodeGraph)
    {
        // Create Open Room Node Queue
        Queue<SO_Node> openRoomNodeQueue = new Queue<SO_Node>();

        // Add Entrance Node to room node queue from room node gragh
        SO_Node entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));
        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;  // Dungeon Not Built
        }

        // Start with no room overlap
        bool noRoomOverlaps = true;

        // Process open room nodes queue
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // If all the room nodes have been processed and no room overlap => return true
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            // else false to build dungeon
            return false;
        }
    }

    private void InstantiateRoomGameobjects()
    {
        foreach (KeyValuePair<string, Room> keyValuePair in roomDictionary)
        {
            Room room = keyValuePair.Value;

            // Calculate room position (remember the room position need to be adjusted by the room template lower bounds)
            Vector3 roomPosition = new Vector3(
                room.lowerBounds.x - room.templateLowerBounds.x,
                room.lowerBounds.y - room.templateLowerBounds.y,
                0.0f);

            // Instantiate room
            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab
            InstantiatedRoom instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();
            instantiatedRoom.room = room;

            // Init the instantiated Room
            instantiatedRoom.Initialize(roomGameObject);

            // Save gameobject reference
            room.instantiatedRoom = instantiatedRoom;
        }
    }

    //===========================================================================
    public bool GenerateDungeon(SO_DungeonLevel currentLevel)
    {
        roomTemplateList = currentLevel.roomTemplateList;

        LoadRoomTemplatesIntoDictionary();

        // Gerenerating Logic
        int dungeonAttempts = 0;
        buildSuccessful = false;
        while (buildSuccessful == false && dungeonAttempts < Settings.MAX_DUNGEON_REBUILD_ATTEMPTS)
        {
            dungeonAttempts++;

            // Select a random room node graph from the newList
            SO_MapNodeGraph roomNodeGraph = GetRandomRoomNodeGraph(currentLevel.roomNodeGraphList);

            int graphAttempts = 0;
            buildSuccessful = false;
            while(buildSuccessful == false && graphAttempts <= Settings.MAX_DUNGEON_REBUILD_ATTEMPTS_FOR_GRAPH)
            {
                // Clear dungeon room gameobject and dungeon room dictionary
                ClearDungeon();

                graphAttempts++;

                // Try to build a random dungeon for the selected room node graph
                buildSuccessful = TryBuildRandomDungeon(roomNodeGraph);
            }

            if (buildSuccessful)
            {
                // Create Room Gameobjects in game world
                InstantiateRoomGameobjects();
            }
        }

        return buildSuccessful;
    }

    public SO_RoomTemplate GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out SO_RoomTemplate roomTemplate))
            return roomTemplate;

        return null;
    }
}