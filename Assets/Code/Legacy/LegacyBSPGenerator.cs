using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LegacyBSPGenerator : MonoBehaviour {

    //Enums
    public enum MapGenerationMode
    {
        TwoD,
        ThreeD
    };


    //Variables
    [Header("Switch the mode. Tiles appear in y axis(2d) or z axis(3d)")]
    public MapGenerationMode generationMode;
    [Header("GameObject to use as a Floor Tile, leave null to use camera background")]
    public GameObject floor;
    [Header("GameObject to use as a Wall Tile, leave null to use camera background")]
    public GameObject wall;
    [Header("Number of rooms you want to create")]
    [Range(3,1000)]
    public int roomNumber = 3;

    [Header("Scale for wall and floor objects")]
    public float tileSize = 1f;
    [Header("Map scale in floor  /  wall units")]
    public Vector2 mapSize;
    [Header("Percentage of each room used")]
    [Range(0,1)]
    public float roomPercentage;
    [Header("Width of the corridors")]
    public float corridorWidth = 2f;
    private List<LegacyRoom> mainRooms;
    private List<LegacyRoom> allRooms;
    private List<LegacyRoom> roomsConected;
    [Header("See map representation on editor")]
    public bool gizmosActive = false;

	// Use this for initialization
	void Start () {
        mainRooms = new List<LegacyRoom>();
        floor.transform.localScale *= tileSize;
        // Check if we're going to cut the map horizontal or vertically. 
        bool horizontalCut = Random.Range(0, 100) < 50;
        float sizeFirstRoom;
        int firstRoomDivision = (int) Random.Range(roomNumber * 0.4f, roomNumber * 0.6f);
        if (horizontalCut)
        {
            sizeFirstRoom = Random.Range(mapSize.y * 0.4f, mapSize.y * 0.6f);
            LegacyRoom room = new LegacyRoom(new Vector3(0, 0, 0), new Vector3(mapSize.x, generationMode == MapGenerationMode.TwoD ? sizeFirstRoom : 0, generationMode == MapGenerationMode.ThreeD ? sizeFirstRoom : 0),firstRoomDivision, roomPercentage, "0");
            mainRooms.Add(room);
            room = new LegacyRoom(new Vector3(0, generationMode == MapGenerationMode.TwoD ? sizeFirstRoom : 0, generationMode == MapGenerationMode.ThreeD ? sizeFirstRoom : 0),
                new Vector3(mapSize.x, generationMode == MapGenerationMode.TwoD ? mapSize.y - sizeFirstRoom : 0, generationMode == MapGenerationMode.ThreeD ? mapSize.y - sizeFirstRoom : 0), roomNumber - firstRoomDivision, roomPercentage, "1");
            mainRooms.Add(room);

        }
        else
        {
            sizeFirstRoom = Random.Range(mapSize.x * 0.4f, mapSize.x * 0.6f);
            LegacyRoom room = new LegacyRoom(new Vector3(0, 0, 0), new Vector3(sizeFirstRoom, generationMode == MapGenerationMode.TwoD ? mapSize.y : 0, generationMode == MapGenerationMode.ThreeD ? mapSize.y : 0), firstRoomDivision, roomPercentage, "0");
            mainRooms.Add(room);
            room = new LegacyRoom(new Vector3(sizeFirstRoom, 0, 0), new Vector3(mapSize.x - sizeFirstRoom, generationMode == MapGenerationMode.TwoD ? mapSize.y : 0, generationMode == MapGenerationMode.ThreeD ? mapSize.y : 0), roomNumber - firstRoomDivision, roomPercentage, "1");
            mainRooms.Add(room);
        }

	}
	
	// Update is called once per frame
	void Update () {
	

        if (Input.GetKeyDown(KeyCode.A))
        {
            InstantiateFloors();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i< allRooms.Count; i++)
            {
                print("Room number: " + i + "  Room id: " + allRooms[i].GenerationString);
            }
        }
    }



    private void InstantiateFloors()
    {
        allRooms = mainRooms[0].GetSubRooms();
        roomsConected = new List<LegacyRoom>();

        List<LegacyRoom> auxRooms = mainRooms[1].GetSubRooms();
        for (int i = 0; i < auxRooms.Count; i++)
        {
            allRooms.Add(auxRooms[i]);
        }

        InstantiateListFloorRoom(allRooms);

        //Instantiate a path between all
        Vector3[] path;
        roomsConected.Add(allRooms[0]);
        for (int i = 0; i < allRooms.Count; i++)
        {
            do
            {
                path = GenerateCorridor(i);

                if (path != null)
                {
                    InstantiateCorridor(path);
                }
            } while (path != null);
        }

       /* //Instantiate simple paths.
        Vector3[] path;
        for (int i = 0; i<allRooms.Count-1; i++)
        {
            path = GenerateCorridor(i, i+1);
            if (path != null)
             InstantiateCorridor(path);
        }

        //Instantiate other paths, with all corridors without one.
        for (int i = 0; i< allRooms.Count; i++)
        {
            if (allRooms[i].NumberCorridors == 0)
            {
                path = GenerateCorridor(i);
                if (path != null)
                    InstantiateCorridor(path);
            }
        }
        */
    }

    private void InstantiateListFloorRoom(List<LegacyRoom> rooms)
    {

        GameObject aux;
        for (int i = 0; i< rooms.Count; i++)
        {

            floor.transform.localScale = rooms[i].RoomSize * tileSize;
            if (floor.transform.localScale.y == 0)
                floor.transform.localScale += Vector3.up;
            else
                floor.transform.localScale += Vector3.forward;


           aux = (GameObject) Instantiate(floor, new Vector3(rooms[i].StartPoint.x + (rooms[i].RoomSize.x * 0.5f) * tileSize, rooms[i].StartPoint.y + (rooms[i].RoomSize.y * 0.5f) * tileSize
                , rooms[i].StartPoint.z + (rooms[i].RoomSize.z * 0.5f) * tileSize), floor.transform.rotation);
            aux.name = i.ToString();
           
        }
    }


    private void InstantiateCorridor(Vector3[] corridorData)
    {
        Vector3 size = new Vector3 (Mathf.Abs(corridorData[0].x - corridorData[1].x), generationMode == MapGenerationMode.TwoD ? Mathf.Abs(corridorData[0].y - corridorData[1].y):1, generationMode == MapGenerationMode.ThreeD ? Mathf.Abs(corridorData[0].z - corridorData[1].z):1);
        Vector3 origin;

        floor.transform.localScale = size;
        if (floor.transform.localScale.z < 1)
        {
            floor.transform.localScale = new Vector3(floor.transform.localScale.x, floor.transform.localScale.y, corridorWidth);
        }
        else if (floor.transform.localScale.x < 1)
        {
            floor.transform.localScale = new Vector3(corridorWidth, floor.transform.localScale.y, floor.transform.localScale.z);
        }
        else if (floor.transform.localScale.y < 1)
        {
            floor.transform.localScale = new Vector3(floor.transform.localScale.x, corridorWidth, floor.transform.localScale.z);
        }


        if (corridorData[0].x < corridorData[1].x || (generationMode == MapGenerationMode.TwoD ? corridorData[0].y < corridorData[1].y : corridorData[0].z < corridorData[1].z))
        {
            origin = corridorData[0] + size / 2;
            origin = new Vector3(origin.x, generationMode == MapGenerationMode.TwoD ? origin.y : 0, generationMode == MapGenerationMode.ThreeD ? origin.z : 0);
            Instantiate(floor, origin, floor.transform.rotation);
        }
        else if (corridorData[0].x > corridorData[1].x || (generationMode == MapGenerationMode.TwoD ? corridorData[0].y > corridorData[1].y : corridorData[0].z > corridorData[1].z))
        {
            origin = corridorData[1] + size / 2;
            origin = new Vector3(origin.x, generationMode == MapGenerationMode.TwoD ? origin.y : 0, generationMode == MapGenerationMode.ThreeD ? origin.z : 0);
            Instantiate(floor, origin, floor.transform.rotation);
        }
        
       


    }


    private Vector3[] GenerateCorridor(int n1, int n2 = -1)
    {
        Vector3[] road = null;
        LegacyRoom room1 = allRooms[n1];
        LegacyRoom room2;
        //First, we need to see if the rooms can have a road to create.
        //Check if is left.
        Vector3 startPoint = room1.StartPoint;

        if (generationMode == MapGenerationMode.ThreeD)
        {
            RaycastHit hit;
            startPoint = new Vector3(startPoint.x  , 0.5f, startPoint.z);
            for (float z = 0; z <= room1.RoomSize.z; z+= room1.RoomSize.z * 0.1f)
            {
                startPoint = new Vector3(startPoint.x, 0.5f, room1.StartPoint.z + z);
                Physics.Raycast(startPoint - Vector3.left, Vector3.left, out hit); 

                if (hit.collider!= null && hit.collider.gameObject!= null)
                {
                    int num;
                    if (!int.TryParse(hit.collider.gameObject.name, out num))
                        break;

                    room2 = allRooms[(num)];
                    if ((hit.collider.gameObject.name == n2.ToString() || n2 == -1) && roomsConected.Find(x=> x == room2) == null){
                        print("room: " + n1 + " room2: " + n2 + "   :    " + hit.collider.gameObject.name  + "  left");
                        roomsConected.Add(room2);
                        road = new Vector3[2];
                        road[0] = startPoint;
                        road[1] = hit.point;
                        room1.NumberCorridors += 1;
                        room2.NumberCorridors += 1;
                        return road;
                    }
                }
            }

            //Check if is right
            startPoint = new Vector3(room1.StartPoint.x + room1.RoomSize.x, 0.5f, room1.StartPoint.z);
            for (float z = 0; z <= room1.RoomSize.z; z += room1.RoomSize.z * 0.1f)
            {
                startPoint = new Vector3(startPoint.x, 0.5f, room1.StartPoint.z + z);
                Physics.Raycast(startPoint + Vector3.right, Vector3.right, out hit);

                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    int num;
                    if (!int.TryParse(hit.collider.gameObject.name, out num))
                        break;

                    room2 = allRooms[(num)];
                    if ((hit.collider.gameObject.name == n2.ToString() || n2 == -1) && roomsConected.Find(x => x == room2) == null)
                    {
                        roomsConected.Add(room2);

                        road = new Vector3[2];
                        road[0] = startPoint;
                        road[1] = hit.point;
                        room1.NumberCorridors += 1;
                        room2.NumberCorridors += 1;
                        return road;
                    }
                }
            }


            //Check if is down
            startPoint = new Vector3(room1.StartPoint.x, 0.5f, room1.StartPoint.z);

            for (float x = 0; x<= room1.RoomSize.x; x+= room1.RoomSize.x * 0.1f)
            {
                startPoint = new Vector3(room1.StartPoint.x + x, 0.5f, startPoint.z);
                Physics.Raycast(startPoint - Vector3.forward, -Vector3.forward, out hit);
                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    int num;
                    if (!int.TryParse(hit.collider.gameObject.name, out num))
                        break;

                    room2 = allRooms[(num)];
                    if ((hit.collider.gameObject.name == n2.ToString() || n2 == -1 )&& roomsConected.Find(z => z == room2) == null)
                    {
                        roomsConected.Add(room2);

                        road = new Vector3[2];
                        road[0] = startPoint;
                        road[1] = hit.point;
                        room1.NumberCorridors += 1;
                        room2.NumberCorridors += 1;
                        return road;
                    }
                }

            }

            //Check if is up
            startPoint = new Vector3(room1.StartPoint.x, 0.5f, room1.StartPoint.z + room1.RoomSize.z);

            for (float x = 0; x <= room1.RoomSize.x; x += room1.RoomSize.x * 0.1f)
            {
                startPoint = new Vector3(room1.StartPoint.x + x, 0.5f, startPoint.z);
                Physics.Raycast(startPoint + Vector3.forward, Vector3.forward, out hit);
                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    int num;
                    if (!int.TryParse(hit.collider.gameObject.name, out num))
                        break;

                    room2 = allRooms[(num)];
                    if ((hit.collider.gameObject.name == n2.ToString() || n2 == -1) && roomsConected.Find(z=> z == room2) == null)
                    {
                        roomsConected.Add(room2);

                        road = new Vector3[2];
                        road[0] = startPoint;
                        road[1] = hit.point;
                        room1.NumberCorridors += 1;
                        room2.NumberCorridors += 1;
                        return road;
                    }
                }

            }

        }
        




        return road;
    }
    #region editorOnly
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (gizmosActive)
        {
            Gizmos.DrawCube(new Vector3( tileSize* mapSize.x * 0.5f,
                           generationMode == MapGenerationMode.ThreeD ? 0 : tileSize * mapSize.y * 0.5f,
                           generationMode == MapGenerationMode.ThreeD ?  tileSize * mapSize.y * 0.5f : 0),

                           new Vector3(tileSize * mapSize.x,
                           generationMode == MapGenerationMode.ThreeD ? 0 : tileSize * mapSize.y ,
                           generationMode == MapGenerationMode.ThreeD ? tileSize * mapSize.y  : 0));
        }
    
    }
#endif
    #endregion


}
