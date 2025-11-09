using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyChest : MonoBehaviour
{
    public Button button;
    public Sprite originalImage;
    public Sprite hoverImage;
    Image currentImage;
    // Use this for initialization
    void Start()
    {
        
        currentImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeWhenHover()
    {
        currentImage.sprite = hoverImage;
    }

    public void changeWhenLeave()
    {
        currentImage.sprite = originalImage;
    }
}