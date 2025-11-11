using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private Dictionary<string, Room> roomDictionary = new Dictionary<string, Room>();

    void Start()
    {
        InitializeRooms();
        
        InitializeFog();
    }

    void InitializeRooms()
    {
        for (int i = 403; i < 460; i++)
        {
            string roomNumber = i.ToString();

            GameObject checkForFloor;
            checkForFloor = GameObject.Find(roomNumber + "_floor");

            if (checkForFloor == null)
            {
                continue;
            }

            if (i == 430 || i == 444)
            {
                InitializeRoom(roomNumber, roomNumber + "_floor", roomNumber + "_ceiling", new string[] { roomNumber + "_wall1", roomNumber + "_wall2", roomNumber + "_wall3", roomNumber + "_wall4", roomNumber + "_wall5" }, roomNumber + "_collider");
            }
            else
            {
                InitializeRoom(roomNumber, roomNumber + "_floor", roomNumber + "_ceiling", new string[] { roomNumber + "_wall1", roomNumber + "_wall2", roomNumber + "_wall3", roomNumber + "_wall4" }, roomNumber + "_collider");
            }
        }

        InitializeRoom("410A", "410A_floor", "410A_ceiling", new string[] { "410A_wall1", "410A_wall2", "410A_wall3", "410A_wall4" }, "410A_collider");
        InitializeRoom("410B", "410B_floor", "410B_ceiling", new string[] { "410B_wall1", "410B_wall2", "410B_wall3", "410B_wall4" }, "410B_collider");
        InitializeRoom("410C", "410C_floor", "410C_ceiling", new string[] { "410C_wall1", "410C_wall2", "410C_wall3", "410C_wall4", "410C_wall5" }, "410C_collider");
        InitializeRoom("PO", "PO_floor", "PO_ceiling", new string[] { "PO_wall1", "PO_wall2", "PO_wall3", "PO_wall4" }, "PO_collider");
        InitializeRoom("Thrift", "Thrift_floor", "Thrift_ceiling", new string[] { "Thrift_wall1", "Thrift_wall2", "Thrift_wall3", "Thrift_wall4" }, "Thrift_collider");
    }

    private void InitializeRoom(string roomName, string floorName, string ceilingName, string[] wallNames, string colliderName)
    {
        GameObject floor = GameObject.Find(floorName);
        GameObject ceiling = GameObject.Find(ceilingName);
        List<GameObject> walls = new List<GameObject>();
        GameObject collider = GameObject.Find(colliderName);

        foreach (string wallName in wallNames)
        {
            GameObject wall = GameObject.Find(wallName);
            if (wall != null)
            {
                walls.Add(wall);
            }
            else
            {
                return;
            }
        }

        if (floor != null && ceiling != null && walls.Count > 0 && collider != null)
        {
            Room room = ScriptableObject.CreateInstance<Room>();
            room.Setup(floor, ceiling, walls, collider, roomDictionary.Count);
            roomDictionary[roomName] = room;
        }
        else
        {
            return;
        }
    }

    public Room GetRoomByName(string roomName)
    {
        if (roomDictionary.TryGetValue(roomName, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    public List<Room> GetRoomList()
    {
        //extract rooms from roomDictionary.values
        List<Room> roomList = new List<Room>();
        foreach (Room room in roomDictionary.Values)
        {
            roomList.Add(room);
        }
        return roomList;
    }

    void InitializeFog()
    {
        LightingHelper.Instance.UpdateFog();
    }

    //use this to visualize some data about each room
    private void OnDrawGizmos()
    {
        if (roomDictionary == null)
        {
            return;
        }
        foreach (Room room in roomDictionary.Values)
        {
            if (room.Ceiling != null)
            {
                Mesh ceilingMesh = room.Ceiling.GetComponent<MeshFilter>().sharedMesh;
                for(int i = 0; i < ceilingMesh.vertices.Length; i++)
                {
                    float i01 = (float)i / (ceilingMesh.vertices.Length - 1);
                    i01 *= 0.5f;
                    Color col = Color.HSVToRGB(i01, 1, 1);
                    Gizmos.color = col;
                    Vector3 globalPoint = room.Ceiling.transform.TransformPoint(ceilingMesh.vertices[i]);
                    Gizmos.DrawSphere(globalPoint, 0.2f);
                }

                Vector3 origin = room.Ceiling.transform.TransformPoint(ceilingMesh.vertices[0]);
                Vector3 next = room.Ceiling.transform.TransformPoint(ceilingMesh.vertices[1]);
                Vector3 localRight = (next - origin).normalized;
                Vector3 localUp = Vector3.Cross(localRight, Vector3.up).normalized;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin + Vector3.up * 2, origin + Vector3.up * 2 + localRight);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin + Vector3.up * 2, origin + Vector3.up * 2 + localUp);
                
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(room.centerOfMass, 0.5f);
                
                if (room.size != Vector2.zero)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(room.origin + Vector3.up, room.origin + room.xAxis * room.size.x + Vector3.up);
                    Gizmos.DrawLine(room.origin + Vector3.up, room.origin + room.zAxis * room.size.y + Vector3.up);
                    Gizmos.DrawLine(room.origin + room.xAxis * room.size.x + Vector3.up, room.origin + room.xAxis * room.size.x + room.zAxis * room.size.y + Vector3.up);
                    Gizmos.DrawLine(room.origin + room.zAxis * room.size.y + Vector3.up, room.origin + room.xAxis * room.size.x + room.zAxis * room.size.y + Vector3.up);
                }

                Gizmos.color = Color.yellow;
                foreach (Vector3 lp in room.lightPosTemp)
                {
                    Gizmos.DrawSphere(lp, 0.5f);
                }
            }
        }
    }
}