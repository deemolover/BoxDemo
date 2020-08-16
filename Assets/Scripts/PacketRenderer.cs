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
    SpriteRenderer arrowReminder;
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

        foreach (Transform child in transform)
        {
            if (child.name == "orientation")
            {
                arrowReminder = child.gameObject.GetComponent<SpriteRenderer>();
            }
        }
        UpdateType();
        if (packet.type != Packet.Type.Ant)
            arrowReminder.enabled = false;
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
        arrowReminder.sortingOrder = GridManager.OrderOfPacket(currentType) - 1;
        string filepath = "Sprites/{0}";
        switch (currentType)
        {
            case Packet.Type.Ant:
                filepath = "character";
                if (packet.Packer) filepath = "packer";
                if (packet.Unpacker) filepath = "unpacker";
                int x = 0, y = 0;
                switch (packet.AntOrientation)
                {
                    case Orientation.UP:    y = 1;  break;
                    case Orientation.LEFT:  x = -1; break;
                    case Orientation.DOWN:  y = -1; break;
                    case Orientation.RIGHT: x = 1;  break;
                    default:break;
                }
                arrowReminder.transform.up = new Vector3(x, y, 0);
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
        Debug.Log("clicked, type:" + packet.type.ToString());
        if (packet.Selectable)
            manager.UpdateSelectedPacket(this);
    }

    public void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        arrowReminder.enabled = false;
    }

    public void Show()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        if (packet.type == Packet.Type.Ant)
            arrowReminder.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        if (packet == null || packet.container == null)
            Destroy(gameObject);
        if (packet.container is Packet)
        {
            Hide();
        } else
        {
            Show();
        }
        Vector3 vec = GridManager.LocationToPos(packet.Location);
        vec.z = - GridManager.OrderOfPacket(packet.type) / 100f;
        transform.position = vec;

        if (currentType != packet.type)
        {
            UpdateType();
        }        
    }
}
