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
        UpdateType();
        initialized = true;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public Instruction.Result ReceiveInstruction(Instruction instr)
    {
        return packet.ReceiveInstruction(instr);
    }


    Packet.Type currentType = Packet.Type.Unk;
    public void UpdateType()
    {
        currentType = packet.type;
        GetComponent<SpriteRenderer>().sortingOrder = GridManager.OrderOfPacket(currentType);
        string filepath = "Sprites/{0}";
        switch (currentType)
        {
            case Packet.Type.Ant:
                filepath = "character";
                if (packet.Packer) filepath = "packer";
                if (packet.Unpacker) filepath = "unpacker";
                break;
            case Packet.Type.Target:
                filepath = "target";
                break;
            case Packet.Type.Box:  filepath = "box";        break;
            case Packet.Type.Wall: filepath = "wall";       break;
            case Packet.Type.Hole: filepath = "hole";       break;
            default: filepath = "box";  break;
        }

        filepath = string.Format("Sprites/{0}", filepath);
        GetComponent<SpriteRenderer>().sprite = ResourceLoader.LoadImage(filepath);
    }

    private void OnMouseDown()
    {
        Debug.Log(manager);
        manager.UpdateSelectedPacket(this);
    }


    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        if (packet == null || packet.container == null)
            Destroy(gameObject);
        if (packet.container is Packet)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        } else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
        transform.position = GridManager.LocationToPos(packet.Location);

        if (currentType != packet.type)
        {
            UpdateType();
        }        
    }
}
