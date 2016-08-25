using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {


    //Starting point from left-down corner
    private Vector3 startPoint;
    private Vector3 roomSize;
    private int numberOfRooms;
    private float roomPercentage = 0.975f;
    private List<Room> subRooms;
    private int numberCorridors = 0;
    //First generation, first number, second, second... 
    private string generationString;
    public Vector3 StartPoint
    {
        get
        {
            return startPoint;
        }

    }

    public Vector3 RoomSize
    {
        get
        {
            return roomSize;
        }

    }

    public List<Room> SubRooms
    {
        get
        {
            return subRooms;
        }

       
    }

    public int NumberCorridors
    {
        get
        {
            return numberCorridors;
        }

        set
        {
            numberCorridors = value;
        }
    }

    public string GenerationString
    {
        get
        {
            return generationString;
        }

    }

    public Room (Vector3 startPoint, Vector3 roomSize, int roomDivisions, float roomPercentage, string previousGeneration = "")
    {
        this.startPoint = startPoint;
        this.roomSize = roomSize;
        this.numberOfRooms = roomDivisions;
        this.roomPercentage = roomPercentage;
        this.generationString = previousGeneration;
        if (this.numberOfRooms == 1)
        {
            this.startPoint = this.startPoint * (1 + (1 - roomPercentage));
            this.roomSize = this.roomSize * roomPercentage;

        }
        else if (this.numberOfRooms > 1)
        {
            this.subRooms = new List<Room>();
            bool twoD = this.roomSize.z == 0;

            //Re-escalate the room for lesser size. 
            Vector3 trueStartPoint = new Vector3(this.startPoint.x + 15, twoD ? this.startPoint.y + 15 : 0, twoD ? 0 : this.startPoint.z +15);
            Vector3 trueRoomSize = new Vector3(this.roomSize.x -15, twoD ? this.roomSize.y -15 : 0, twoD?0:this.roomSize.z -15); //* this.roomPercentage;

            bool horizontal = Random.Range(0, 100) < 50;
            int firstRooms;
            if (numberOfRooms > 2)
            {
                firstRooms = (int)Random.Range(this.numberOfRooms * 0.4f, this.numberOfRooms * 0.6f);
            }
            else
            {
                firstRooms = 1;
            }
            float sizeFirstRoom;
            if (horizontal)
            {
                
                sizeFirstRoom = twoD ? trueRoomSize.y : trueRoomSize.z;
                sizeFirstRoom = Random.Range(sizeFirstRoom * 0.4f, sizeFirstRoom * 0.6f) ;
                Room room = new Room(trueStartPoint, new Vector3(trueRoomSize.x, twoD ? sizeFirstRoom : 0, twoD ? 0 : sizeFirstRoom), firstRooms, roomPercentage, this.generationString+"0");
                this.subRooms.Add(room);
                if ((this.numberOfRooms - firstRooms) != 0)
                {
                    room = new Room(trueStartPoint + (twoD ? Vector3.up * (sizeFirstRoom): Vector3.forward * (sizeFirstRoom)),
                    new Vector3(trueRoomSize.x, twoD ? trueRoomSize.y - sizeFirstRoom : 0, twoD ? 0 : trueRoomSize.z - sizeFirstRoom), this.numberOfRooms - firstRooms, roomPercentage, this.generationString+"1");
                    this.subRooms.Add(room);
                }
            }
            else
            {
                
                sizeFirstRoom = Random.Range(trueRoomSize.x * 0.4f, trueRoomSize.x * 0.6f);
                Room room = new Room(trueStartPoint, new Vector3(sizeFirstRoom, twoD ? trueRoomSize.y : 0, twoD ? 0 : trueRoomSize.z), firstRooms, roomPercentage, this.generationString + "0");
                this.subRooms.Add(room);
                if ((this.numberOfRooms - firstRooms) != 0)
                {
                    room = new Room(trueStartPoint + Vector3.right * (sizeFirstRoom),
                        new Vector3(trueRoomSize.x - sizeFirstRoom, twoD ? trueRoomSize.y : 0, twoD ? 0 : trueRoomSize.z), this.numberOfRooms - firstRooms, roomPercentage, this.generationString +"1");
                    this.subRooms.Add(room);
                }
            }

        }
    }

    public List<Room> GetSubRooms()
    {
        List<Room> rooms = new List<Room>();
        if (this.subRooms == null)
        {
            rooms.Add(this);
            return rooms;
        }
        else
        {
            for (int i = 0; i < this.subRooms.Count; i++)
            {
                for(int j = 0; j< this.subRooms[i].GetSubRooms().Count; j++)
                {
                    rooms.Add(this.subRooms[i].GetSubRooms()[j]);
                }
            }
            return rooms;
        }
    }

    public override bool Equals(object obj)
    {
        Room aux = obj as Room;

        return aux!=null && this.generationString == aux.generationString;
    }

}
