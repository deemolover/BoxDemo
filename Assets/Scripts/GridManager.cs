using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridManager : MonoBehaviour
{
    public GameObject gridPrefab;
    public GameObject packetPrefab;

    PacketRenderer selectedPacketRenderer;
    // GridContainer selectedContainer;

    Dictionary<int, GridContainer> containers = new Dictionary<int, GridContainer>();

    public static int OrderOfPacket(Packet.Type type)
    {
        if (type == Packet.Type.Ant) return 16;
        if (type == Packet.Type.Target) return 2;
        if (type == Packet.Type.Hole) return 1;
        return 4;
    }

    public static Vector2 LocationToPos(Vector2 location)
    {
        return new Vector2((location.x - 0.5f) * 1.2f,
                            (location.y - 0.5f) * 1.2f);
    }

    public static int HashLocation(Vector2 vec)
    {
        return (int)(vec.x+100) * 1000 + (int)(vec.y+100);
    }

    // Start is called before the first frame update
    void Start()
    {
        // LoadDemo();
        LoadLevel("level_demo");
        GameObject.Find("EditorUI/Reload").GetComponent<Button>().onClick.AddListener(OnClickReload);        
    }

    // UI FUNCTIONS

    public void OnClickReload()
    {
        LoadLevel("level_demo");
    }

    void LoadDemo()
    {
        foreach (GridContainer container in gameObject.GetComponentsInChildren<GridContainer>())
        {
            container.Init(this, container.InitLocation);
            containers.Add(HashLocation(container.Location), container);
            // moved to container.Init()
            // container.transform.position = LocationToPos(container.Location);
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

    void ClearScene()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        selectedPacketRenderer = null;
        containers = new Dictionary<int, GridContainer>();

    }

    void LoadLevel(string levelName)
    {
        ClearScene();

        string filepath = Application.streamingAssetsPath + "/Levels/" + levelName + ".csv";
        var data = CSVTool.Read(filepath, Encoding.UTF8);

        int width = 0;
        foreach (var line in data)
        {
            width = Mathf.Max(width, line.Count);
        }
        int height = data.Count;

        for (int i = 0; i < data.Count; ++i)
        {
            for (int j = 0; j < data[i].Count; ++j)
            {
                string element = data[i][j];
                if (element == "")
                {
                    continue;
                }
                GameObject obj = Instantiate(gridPrefab);
                obj.transform.parent = transform;
                GridContainer container = obj.AddComponent<GridContainer>();
                Vector2 location = new Vector2(j - width / 2, height / 2 - i);
                container.Init(this, location);
                containers.Add(HashLocation(container.Location), container);

                Packet.Type packetType = Packet.Type.Unk;
                Packet packet = null;
                
                if (element.StartsWith("ant"))
                {
                    // ant related info
                    bool packable = false, unpackable = false;
                    Orientation antOri = Orientation.NONE;
                    packetType = Packet.Type.Ant;
                    string antInfo = element.Substring(3);
                    if (antInfo.StartsWith("P"))
                    {
                        packable = true;
                        antInfo = antInfo.Substring(1);
                    } else if (antInfo.StartsWith("U"))
                    {
                        unpackable = true;
                        antInfo = antInfo.Substring(1);
                    }
                    if (antInfo.Length >= 2)
                        switch (antInfo[1]) 
                        {
                            case 'r': antOri = Orientation.RIGHT;break;
                            case 'l': antOri = Orientation.LEFT; break;
                            case 'u': antOri = Orientation.UP;   break;
                            case 'd': antOri = Orientation.DOWN; break;
                        }
                    // not safe here
                    packet = new Packet(packetType, container);
                    packet.AntInit(packable, unpackable, antOri);
                } else
                {
                    switch (element)
                    {
                        case "tar":
                            packetType = Packet.Type.Target;
                            break;
                        case "pack":
                            packetType = Packet.Type.Box;
                            break;
                        case "wall":
                            packetType = Packet.Type.Wall;
                            break;
                        case "hole":
                            packetType = Packet.Type.Hole;
                            break;
                        default:
                            break;
                    }
                    if (packetType != Packet.Type.Unk)
                        packet = new Packet(packetType, container);
                }

                if (packetType != Packet.Type.Unk)
                {
                    GameObject pack = Instantiate(packetPrefab);
                    pack.transform.parent = transform;
                    var renderer = pack.AddComponent<PacketRenderer>();
                    renderer.Init(this, packet);
                    if (packet.type == Packet.Type.Ant)
                    {
                        selectedPacketRenderer = renderer;
                    }
                }
            }
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
        return Instruction.Result.INVALID_GRID;
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
