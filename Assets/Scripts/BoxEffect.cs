using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoxEffect : MonoBehaviour
{
    bool mouseOver = false;
    bool state = false;
    SpriteRenderer image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<SpriteRenderer>();
        image.sprite = ResourceLoader.LoadImage("Sprites/UI/SelectLevel");
        selected = new Color(shade, shade, shade);
        originalScale = transform.localScale;
        // selected = Color.grey;
    }
    
    float scaling = 1.1f;
    Vector3 originalScale;
    float shade = 200f / 255f;
    Color selected;

    private void OnMouseEnter()
    {
        state = true;
    }

    private void OnMouseExit()
    {
        state = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseOver != state)
        {
            mouseOver = state;
            if (mouseOver)
            {
                transform.localScale = originalScale * scaling;
                image.sprite = ResourceLoader.LoadImage("Sprites/UI/SelectLevelOpen");
                image.color = selected;
            } else
            {
                transform.localScale = originalScale;
                image.sprite = ResourceLoader.LoadImage("Sprites/UI/SelectLevel");
                image.color = Color.white;
            }
        }
    }
}
