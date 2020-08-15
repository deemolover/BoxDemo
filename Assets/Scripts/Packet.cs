using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

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
        ERROR = 1
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
        Box = 2
    }
    public Type type;
    public object container;
    public List<Packet> packets = new List<Packet>(); // child packets

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
        if (container is GridContainer)
        {
            ((GridContainer)container).Release(this);
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
        GridContainer target = null;
        Instruction.Result result = Instruction.Result.ERROR;
        if (container is GridContainer)
        {
            GridContainer from = (GridContainer)container;
            result = from.QueryAdjacentContainer(oriVal, ref target);
            if (result == Instruction.Result.SUCCEED)
            {
                /* TODO: check barriers */
                from.Release(this);
                target.Contain(this);
            } else
            {
            }
        }
        return result;
    }

    public Instruction.Result ReceiveInstruction(Instruction instr)
    {
        switch (instr.type)
        {
            case Instruction.Type.MOVE:
                Move(instr.oriVal);
                break;
            default:
                break;
        }
        return Instruction.Result.SUCCEED;
    }

    
}
