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

    public bool HasPacketOfType(Packet.Type givenType)
    {
        foreach (var packet in packets)
        {
            if (packet.type == givenType)
            {
                return true;
            }
        }
        return false;
    }

    public bool Compatible(Packet packet)
    {
        if (HasPacketOfType(Packet.Type.Wall))
            return false;
        if (packet.type == Packet.Type.Ant)
        {
            if (HasPacketOfType(Packet.Type.Ant))
                return false;
        }
        return true;
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

    public Instruction.Result TryHold(Packet holder)
    {
        Packet box = null;
        foreach (var packet in packets)
        {
            if (packet.type == Packet.Type.Box)
            {
                box = packet;
            }
        }
        if (box == null)
        {
            return Instruction.Result.ERROR;
        }
        Release(box);
        holder.Contain(box);
        return Instruction.Result.SUCCEED;
    }

    public Instruction.Result TryPack(Packet packer)
    {
        List<Packet> toPack = new List<Packet>();
        foreach (var packet in packets)
        {
            if (packet.type != Packet.Type.Ant && packet.type != Packet.Type.Box)
            {
                return Instruction.Result.ERROR;
            }
            toPack.Add(packet);
        }
        Packet box = new Packet(Packet.Type.Box, this);
        PacketRenderer renderer = Manager.CreatePacketRenderer(box, this);
        foreach (var packet in toPack)
        {
            Release(packet);
            box.Contain(packet);
        }
        return Instruction.Result.SUCCEED;
    }

    public Instruction.Result TryUnpack(Packet unpacker)
    {
        Packet box = null;
        foreach (var packet in packets)
        {
            if (packet.type == Packet.Type.Box)
            {
                box = packet;
            }
        }
        if (box == null || box.packets.Count == 0)
        {
            return Instruction.Result.ERROR;
        }
        List<Packet> toUnpack = new List<Packet>();
        foreach (Packet packet in box.packets)
            toUnpack.Add(packet);
        foreach (Packet packet in toUnpack)
        {
            box.Release(packet);
            Contain(packet);
        }
        Release(box);
        return Instruction.Result.SUCCEED;
    }

    public void HoleCheck()
    {
        Packet hole = null;
        List<Packet> toKill = new List<Packet>();
        bool filled = false;
        foreach (var packet in packets)
        {
            if (packet.type == Packet.Type.Hole)
            {
                hole = packet;
            }
            else if (packet.type == Packet.Type.Ant)
            {
                toKill.Add(packet);
                if (packet.Holding) filled = true;
            }
        }
        
        if (hole != null)
        {
            foreach (var packet in toKill)
            {
                Release(packet);
            }
            if (filled) Release(hole);
        }
    }

    public void TargetCheck()
    {
        bool hasTarget = false;
        bool delivered = false;
        foreach (var packet in packets)
        {
            if (packet.type == Packet.Type.Target)
            {
                hasTarget = true;
            }
            else if (packet.type == Packet.Type.Box)
            {
                if (packet.packets.Count == 0) // need to improve logic
                {
                    delivered = true;
                }
            }
        }
        
        if (hasTarget && delivered)
        {
            Manager.GameSet();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
