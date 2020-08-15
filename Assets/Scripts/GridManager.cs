using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GridManager : MonoBehaviour
{
    PacketRenderer selectedPacketRenderer;

    Dictionary<int, GridContainer> containers = new Dictionary<int, GridContainer>();

    public static Vector2 LocationToPos(Vector2 location)
    {
        return new Vector2((location.x - 0.5f) * 1.2f,
                            (location.y - 0.5f) * 1.2f);
    }

    public static int HashLocation(Vector2 vec)
    {
        return (int)(vec.x) * 1000 + (int)(vec.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GridContainer container in gameObject.GetComponentsInChildren<GridContainer>())
        {
            container.Init(this, container.InitLocation);
            containers.Add(HashLocation(container.Location), container);
            container.transform.position = LocationToPos(container.Location);
        }

        foreach (PacketRenderer renderer in gameObject.GetComponentsInChildren<PacketRenderer>())
        {
            GridContainer container = null;
            var result = QueryContainer(renderer.InitLocation, ref container);
            if (result == Instruction.Result.SUCCEED)
            {
                renderer.Init(this, container);
            }
            if (renderer.name == "Character")
                selectedPacketRenderer = renderer;
        }
    }

    public Instruction.Result QueryContainer(Vector2 location, ref GridContainer container)
    {
        int key = HashLocation(location);
        if (containers.ContainsKey(key))
        {
            container = containers[key];
            return Instruction.Result.SUCCEED;
        }
        return Instruction.Result.ERROR;
    }

    // Update is called once per frame
    void Update()
    {
        Orientation ori = Orientation.NONE;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ori = Orientation.UP;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ori = Orientation.LEFT;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ori = Orientation.DOWN;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ori = Orientation.RIGHT;

        if (ori != Orientation.NONE)
        {
            Instruction instr = new Instruction(Instruction.Type.MOVE);
            instr.oriVal = ori;
            selectedPacketRenderer.ReceiveInstruction(instr);
        }
    }
}
