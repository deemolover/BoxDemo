using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridContainer : MonoBehaviour
{
    bool initialized = false; 

    public Vector2 InitLocation;

    GridManager manager;
    public GridManager Manager
    {
        get
        {
            return manager;
        }
    }
    Vector2 location;
    public Vector2 Location
    {
        get
        {
            return location;
        }
    }

    List<Packet> packets = new List<Packet>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(GridManager mManager, Vector2 mLocation)
    {
        manager = mManager;
        location = mLocation;
        transform.position = GridManager.LocationToPos(Location);
        initialized = true;
    }

    public void Release(Packet packet)
    {
        if (packets.Contains(packet))
        {
            packets.Remove(packet);
        }
        if (packet.container != null && packet.container.Equals(this))
            packet.container = null;
    }

    public void Contain(Packet packet)
    {
        if (packet.container == null || !packet.container.Equals(this))
            packet.container = this;
        if (!packets.Contains(packet))
            packets.Add(packet);
    }

    public Instruction.Result QueryAdjacentContainer(Orientation oriVal, ref GridContainer container)
    {
        if (!initialized) return Instruction.Result.ERROR;
        Instruction.Result result = Instruction.Result.ERROR;
        Vector2 target;
        switch (oriVal)
        {
            case Orientation.UP:
                target = Location + new Vector2(0, 1);
                break;
            case Orientation.DOWN:
                target = Location + new Vector2(0,-1);
                break;
            case Orientation.LEFT:
                target = Location + new Vector2(-1,0);
                break;
            case Orientation.RIGHT:
                target = Location + new Vector2(1, 0);
                break;
            default:
                return Instruction.Result.ERROR;
        }
        result = Manager.QueryContainer(target, ref container);
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
