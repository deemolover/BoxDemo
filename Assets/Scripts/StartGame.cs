using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnMouseUpAsButton()
    {
        OnClick();
    }

    void OnClick()
    {
        SceneManager.LoadScene("SelectLevel");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
