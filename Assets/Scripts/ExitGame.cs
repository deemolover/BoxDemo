using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExitGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    float scaling = 1.1f;
    Vector3 originalScale;
    float shade = 200f / 255f;
    Color selected;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        image = GetComponent<Image>();
        selected = new Color(shade, shade, shade);
        originalScale = transform.localScale;
    }

    void OnClick()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaling;
        image.sprite = ResourceLoader.LoadImage("Sprites/UI/exitHover");
        image.color = selected;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        image.sprite = ResourceLoader.LoadImage("Sprites/UI/exit");
        image.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
