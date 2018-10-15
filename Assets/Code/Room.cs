using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    private PositiveVector2 startPoint, roomSize;

    private List<Room> roomChilds;


    public Room(PositiveVector2 startPoint, PositiveVector2 size, PositiveVector2 minSize, int roomsAsigned)
    {
        //En cada habitación, parto en dos si me deja el minimo de tamaño. 
        this.startPoint = startPoint;
        this.roomSize = size;

        if (!CheckIfMinSize(minSize))
        {
            if (roomsAsigned > 1)
            {
                roomChilds = new List<Room>(BSPDungeon.Instance.MakeRooms(Random.Range(0, 100) < 50, size, roomsAsigned, startPoint));
                //Debug.Log("Room : " + this.ToString() + " childs: " + roomChilds[0].ToString() + "  ::::::   " + roomChilds[1].ToString());
                foreach (Room room in roomChilds)
                {
                    if (room.CheckIfMinSize(minSize))
                    {
                        BSPDungeon.Instance.AddRoom(this);
                        foreach(Room child in roomChilds)
                        {
                            BSPDungeon.Instance.DeleteRoom(child); 
                        }
                        break;
                    }
                }
            }
            else
            {
                BSPDungeon.Instance.AddRoom(this); 
            }
        }
        


    }

    public void SetDefinitiveSize(float mod, PositiveVector2 minSize)
    {

        PositiveVector2 oldSize = roomSize; 
        roomSize = new PositiveVector2((int)(roomSize.X * mod), (int)(roomSize.Y*mod));
        if (CheckIfMinSize(minSize))
        {
            roomSize = oldSize; 
        }
       
    }


    public bool CheckIfMinSize(PositiveVector2 minSize)
    {
        return (this.roomSize.X < minSize.X || this.roomSize.Y < minSize.Y);
    }

    public override string ToString()
    {
        return "Point Start: (" + startPoint.X + "," + startPoint.Y + ")  Size: (" + roomSize.X + "," + roomSize.Y + ")"; 
    }







    public PositiveVector2 StartPoint
    {
        get
        {
            return startPoint;
        }
    }

    public PositiveVector2 RoomSize
    {
        get
        {
            return roomSize;
        }

    }
}
