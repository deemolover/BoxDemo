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
    bool packable = false, unpackable = false;
    Orientation antOri = Orientation.NONE;


    public Packet(Type mType, object mContainer)
    {
        type = mType;
        container = mContainer;
        if (container is GridContainer)
        {
            ((GridContainer)container).Contain(this);
        }
    }

    ~Packet()
    {
        if (container != null && container is GridContainer)
        {
            ((GridContainer)container).Release(this);
        }
    }

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
                result = Move(instr.oriVal);
                break;
            default:
                break;
        }
        Debug.Log(result);
        return result;
    }

    
}
