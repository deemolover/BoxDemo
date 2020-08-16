using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacketRenderer : MonoBehaviour
{
    bool initialized = false;
    public Vector2 InitLocation;
    public GridManager manager;
    public Packet.Type type;

    public Packet packet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(GridManager mManager, GridContainer container)
    {
        Init(mManager, new Packet(type, container));
    }

    public void Init(GridManager mManager, Packet mPacket)
    {
        manager = mManager;
        packet = mPacket;
        GetComponent<SpriteRenderer>().sortingOrder = 1; // hard-coded
        initialized = true;
    }

    public Instruction.Result ReceiveInstruction(Instruction instr)
    {
        return packet.ReceiveInstruction(instr);
    }


    Packet.Type currentType = Packet.Type.Unk;
    public void UpdateType()
    {
        currentType = packet.type;
        string filepath = "Sprites/{0}";
        switch (currentType)
        {
            case Packet.Type.Box:
                filepath = string.Format(filepath, "box");
                break;
            case Packet.Type.Ant:
                filepath = string.Format(filepath, "character");
                break;
            default:
                filepath = string.Format(filepath, "box");
                break;
        }
        GetComponent<SpriteRenderer>().sprite = ResourceLoader.LoadImage(filepath);
    }


    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        transform.position = GridManager.LocationToPos(packet.Location);

        if (currentType != packet.type)
        {
            UpdateType();
        }        
    }
}
