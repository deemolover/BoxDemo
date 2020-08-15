using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    Dictionary<int, GridContainer> containers = new Dictionary<int, GridContainer>();

    public int HashLocation(Vector2 vec)
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
        
    }
}
