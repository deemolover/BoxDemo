using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        manager = mManager;
        packet = new Packet(type, container);
        initialized = true;
    }

    public Instruction.Result ReceiveInstruction(Instruction instr)
    {
        return packet.ReceiveInstruction(instr);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        transform.position = GridManager.LocationToPos(packet.Location);
    }
}
