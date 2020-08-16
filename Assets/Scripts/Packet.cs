using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class ResourceLoader
{
    public static Dictionary<string, Sprite> sprites;
    public static Sprite LoadImage(string filepath)
    {
        if (sprites == null)
            sprites = new Dictionary<string, Sprite>();
        if (!sprites.ContainsKey(filepath))
        {
            var pref = Resources.Load(filepath, typeof(Sprite));
            if (pref == null)
            {
                return null;
            }
            Sprite tmp = GameObject.Instantiate(pref) as Sprite;
            sprites.Add(filepath, tmp);
        }
        return sprites[filepath];
    }
}

public enum Orientation
{
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3,
    NONE = 4
}

public class Instruction
{
    public enum Result
    {
        SUCCEED = 0,
        ERROR = 1,
        INVALID_ANT_MOVE = 2,
        INCOMPATIBLE = 3,
        INVALID_GRID = 4,
    }

    public enum Type
    {
        MOVE = 1,
        SKILL = 2
    }

    public Type type;
    public Orientation oriVal;
    public int data;
    public Instruction(Type mType)
    {
        type = mType;
    }
}

public class Packet
{
    public enum Type
    {
        Unk = 0,
        Ant = 1,
        Box = 2,
        Target = 16,
        Wall = 32,
        Hole = 64
    }
    public Type type;
    public object container;
    public List<Packet> packets = new List<Packet>(); // child packets

    // ant related info, valid when this is an ant
    // TODO: describe with enum
    bool packable = false, unpackable = false;
    Orientation antOri = Orientation.NONE;
    public Orientation AntOrientation
    {
        get
        {
            if (type == Type.Ant) return antOri;
            else return Orientation.NONE;
        }
    }
    public bool Selectable
    {
        get { return type == Type.Ant; }
    }
    public bool Movable
    {
        get { return type == Type.Ant; }
    }
    public bool Holding
    {
        get { return type == Type.Ant && packets.Count != 0; }
    }
    public bool Holder
    {
        get { return type == Type.Ant && !packable && !unpackable; }
    }
    public bool Packer
    {
        get { return type == Type.Ant && packable; }
    }
    public bool Unpacker
    {
        get { return type == Type.Ant && unpackable; }
    }


    public Packet(Type mType, object mContainer)
    {
        type = mType;
        container = mContainer;
        if (container is GridContainer)
        {
            ((GridContainer)container).Contain(this);
        }
    }

    /*
    ~Packet()
    {
        if (container != null && container is GridContainer)
        {
            ((GridContainer)container).Release(this);
        }
    }
    */

    public void AntInit(bool mPackable, bool mUnpackable, Orientation orientation)
    {
        if (type == Type.Ant)
        {
            packable = mPackable;
            unpackable = mUnpackable;
            antOri = orientation;
        }
    }

    public Vector2 Location
    {
        get
        {
            if (container is GridContainer)
            {
                return ((GridContainer)container).Location;
            } else if (container is Packet)
            {
                return ((Packet)container).Location;
            }
            return new Vector2(-1,-1); // should not happen
        }
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

    public Instruction.Result Move(Orientation oriVal)
    {
        if (type == Type.Ant && antOri != oriVal)
            return Instruction.Result.INVALID_ANT_MOVE;
        Instruction.Result result = Instruction.Result.ERROR;
        if (container is GridContainer)
        {
            GridContainer target = null;
            GridContainer from = (GridContainer)container;
            result = from.QueryAdjacentContainer(oriVal, ref target);
            if (result == Instruction.Result.SUCCEED)
            {
                if (!target.Compatible(this))
                    return Instruction.Result.INCOMPATIBLE;
                from.Release(this);
                target.Contain(this);
                target.HoleCheck();
            }
        }
        return result;
    }

    public Instruction.Result HoldOrEase()
    {
        // if (!Holder) return Instruction.Result.ERROR;
        Instruction.Result result = Instruction.Result.ERROR;
        if (Holding)
        {
            if (container is GridContainer)
            {
                List<Packet> toEase = new List<Packet>();
                var recv = ((GridContainer)container);
                foreach (var packet in packets)
                {
                    toEase.Add(packet);
                }
                foreach (var packet in toEase)
                {
                    Release(packet);
                    recv.Contain(packet);
                }
                recv.TargetCheck();
                result = Instruction.Result.SUCCEED;
            }
        } else
        {
            if (container is GridContainer)
            {
                result = ((GridContainer)container).TryHold(this);
            }
        }
        return result;
    }

    public Instruction.Result Pack()
    {
        // if (!Packer) return Instruction.Result.ERROR;
        Instruction.Result result = Instruction.Result.ERROR;
        if (container is GridContainer)
        {
            GridContainer target = null;
            GridContainer from = (GridContainer)container;
            result = from.QueryAdjacentContainer(antOri, ref target);
            if (result == Instruction.Result.SUCCEED)
            {
                result = target.TryPack(this);
                if (result == Instruction.Result.SUCCEED)
                {
                    from.Release(this);
                    target.Contain(this);
                    target.HoleCheck();
                }
            }
        }
        return result;
    }

    public Instruction.Result Unpack()
    {
        // if (!Unpacker) return Instruction.Result.ERROR;
        Instruction.Result result = Instruction.Result.ERROR;
        if (container is GridContainer)
        {
            GridContainer target = null;
            GridContainer from = (GridContainer)container;
            result = from.QueryAdjacentContainer(antOri, ref target);
            if (result == Instruction.Result.SUCCEED)
            {
                if (!target.Compatible(this))
                    return Instruction.Result.INCOMPATIBLE;
                result = from.TryUnpack(this);
                if (result == Instruction.Result.SUCCEED)
                {
                    from.Release(this);
                    target.Contain(this);
                    target.HoleCheck();
                    from.TargetCheck();
                }
            }
        }
        return result;
    }

    public Instruction.Result ReceiveInstruction(Instruction instr)
    {
        Instruction.Result result = Instruction.Result.ERROR;
        switch (instr.type)
        {
            case Instruction.Type.MOVE:
                if (Movable) result = Move(instr.oriVal);
                break;
            case Instruction.Type.SKILL:
                if (Holder) result = HoldOrEase();
                else if (Packer) result = Pack();
                else if (Unpacker) result = Unpack();
                break;
            default:
                break;
        }
        Debug.Log(result);
        return result;
    }

    
}
