using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPDungeon : Singleton<BSPDungeon>
{
    public enum GenerationType
    {
        I2D,
        I3D
    }

    [SerializeField]
    private GenerationType generationMode; 

    [SerializeField]
    private PositiveVector2 size;

    [SerializeField]
    private int minRoomNumber, maxRoomNumber;

    [SerializeField]
    private PositiveVector2 minRoomSize; 


    [SerializeField][Range(0,0.25f)]
    private float chaosMod;

    [SerializeField][Range(0,1f)]
    private float ocuppationPercentage = 0.9f; 

    private List<Room> rooms;

    private int[,] board;
    [SerializeField]
    private GameObject prefab; 


	void Start () {
        InitializeRooms();
        board = new int[size.X, size.Y]; 

        for (int i = 0; i< rooms.Count; i++)
        {
            int next = i + 1 >= rooms.Count ? 0 : i + 1;
            CreateRoad(rooms[i], rooms[next]); 
        }
        foreach (Room item in rooms)
        {
            //Debug.Log(item.ToString()); 
            for (int i = item.StartPoint.X; i<item.StartPoint.X + item.RoomSize.X; i++)
            {
                for (int j = item.StartPoint.Y; j<item.StartPoint.Y+ item.RoomSize.Y; j++)
                {
                    if (board[i, j]== 1) Debug.Log("jajaxd"); 
                    board[i, j] = 1; 
                }
            }
        }



        for (int i = 0; i< board.GetLength(0); i++)
        {
            for (int j = 0; j< board.GetLength(1); j++)
            {
                if (board[i,j] == 0)
                    Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity); 
            }
        }

        for (int i = -10; i< 0; i++) { 
            for (int j = -10; j< board.GetLength(1)+10 ; j++) {
                Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity);
            }
        }


        for (int i = -10; i < 0; i++)
        {
            for (int j = -10; j < board.GetLength(0)+10; j++)
            {
                Instantiate(prefab, new Vector3(j, i, 0), Quaternion.identity);
            }
        }

        for (int i = board.GetLength(0); i < board.GetLength(0) +10; i++)
        {
            for (int j = -10; j < board.GetLength(1)+10; j++)
            {
                Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity);
            }
        }

        for (int i = board.GetLength(1); i < board.GetLength(1) + 10; i++)
        {
            for (int j = -10; j < board.GetLength(0)+10; j++)
            {
                Instantiate(prefab, new Vector3(j, i, 0), Quaternion.identity);
            }
        }

    }

   


    private void InitializeRooms()
    {
        rooms = new List<Room>();
        MakeRoomsCut(); 
    }

    private void MakeRoomsCut()
    {
        bool horizontalCut = Random.Range(0, 100) < 50;
        int roomNumber = Random.Range(minRoomNumber, maxRoomNumber);
        PositiveVector2 positionActual = new PositiveVector2((int)this.transform.position.x, (int)this.transform.position.y);
        float sizeFirstChunk = Random.Range(0.5f - chaosMod, 0.5f + chaosMod);
        Room[] initialRooms =  MakeRooms(horizontalCut, size, roomNumber, positionActual, sizeFirstChunk);

       // Debug.Log("INITIAL ROOM: " + initialRooms[0].ToString());
        //Debug.Log("2 INITIAL ROOM: " + initialRooms[1].ToString()); 
       
    }

    public Room[] MakeRooms(bool horizontal, PositiveVector2 size, int roomNumber, PositiveVector2 initialPoint, float sizeFirstChunk = 0.5f) 
    {
        PositiveVector2 pointToStart = initialPoint;

        Room[] rooms = new Room[2];
        int roomsToApply = (int)(roomNumber * sizeFirstChunk);
        PositiveVector2 sizeToApply = new PositiveVector2(horizontal ? size.X : (int) (size.X*sizeFirstChunk), horizontal ? (int)(size.Y * sizeFirstChunk) : size.Y);

        rooms[0]= new Room(pointToStart, sizeToApply, minRoomSize, roomsToApply);
        if (horizontal)
        {
            pointToStart.Y += sizeToApply.Y;
        }
        else
        {
            pointToStart.X += sizeToApply.X;
        }
        sizeToApply = new PositiveVector2(horizontal ? size.X : (int) (size.X * (1-sizeFirstChunk)), horizontal ? (int)(size.Y * (1 - sizeFirstChunk)): size.Y);
        roomsToApply = roomNumber - roomsToApply;
        rooms[1] = new Room(pointToStart, sizeToApply, minRoomSize, roomsToApply);

        return rooms; 
       

    }


    public void DeleteRoom(Room room)
    {
        rooms.Remove(room); 
    }

    public void AddRoom(Room room)
    {
        Room nextRoom = room;
        nextRoom.SetDefinitiveSize(ocuppationPercentage, minRoomSize); 
        rooms.Add(room); 

    }

    private void CreateRoad(Room room1, Room room2)
    {
        int roomXStart = Random.Range(room1.StartPoint.X, room1.StartPoint.X + room1.RoomSize.X);
        int roomYStart = Random.Range(room1.StartPoint.Y, room1.StartPoint.Y + room1.RoomSize.Y);


        int distanceX = 0;
        int distanceY = 0;
        if (roomXStart < room2.StartPoint.X || roomXStart >= room2.StartPoint.X + room2.RoomSize.X)
        {
            if (roomYStart < room2.StartPoint.Y || roomYStart >= room2.StartPoint.Y + room2.RoomSize.Y)
            {
                distanceX = Random.Range(room2.StartPoint.X, room2.StartPoint.X + room2.RoomSize.X) - roomXStart;
                distanceY = Random.Range(room2.StartPoint.Y, room2.StartPoint.Y + room2.RoomSize.Y) - roomYStart;
            }
            else
            {
                distanceX = Random.Range(room2.StartPoint.X, room2.StartPoint.X + room2.RoomSize.X) - roomXStart;
            }
        }
        else if (roomYStart < room2.StartPoint.Y || roomYStart >= room2.StartPoint.Y + room2.RoomSize.Y)
        {
            distanceY = Random.Range(room2.StartPoint.Y, room2.StartPoint.Y + room2.RoomSize.Y) - roomYStart;
        }

        int modX = distanceX == 0 ? 0: Mathf.Abs(distanceX) / distanceX;
        int modY = distanceY == 0 ? 0:  Mathf.Abs(distanceY) / distanceY;

        int i = 0;
        int j = roomYStart; 
        for ( i = roomXStart;modX > 0 ?  i < roomXStart + distanceX : i > roomXStart + distanceX; i+=modX)
        {
            board[i, j] = 1;
        }

        for (j = roomYStart; modY > 0 ? j < roomYStart + distanceY : j > roomYStart + distanceY; j += modY)
        {
            board[i, j] = 1;
        }
    }
}
