using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    float scaling = 1.1f;
    Vector3 originalScale;
    float shade = 200f / 255f;
    Color selected;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        selected = new Color(shade, shade, shade);
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaling;
        image.color = selected;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        image.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
