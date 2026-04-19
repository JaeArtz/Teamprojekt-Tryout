using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class LeafAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    public List<GameObject> leaves;
    private GameObject leafPanel;

    private YieldInstruction fadeInstruction = new YieldInstruction();
    private void Awake()
    {
        leafPanel = GameObject.Find("LeafPanel");
        leafPanel.SetActive(false);
    }
    public IEnumerator displayLeaves()
    {
        AlterShownLeaves();
        leafPanel.SetActive(true);

        float elapsedTime = 0.0f;
        float fadeTime = 1.0f;
        Color[] colors = new Color[4];
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            for(int i = 0; i < 4; i++)
            {
                colors[i] = leaves[i].GetComponent<Image>().color;
                colors[i].a =  Mathf.Clamp01(elapsedTime / fadeTime);
                leaves[i].GetComponent<Image>().color = colors[i];
            }
        }

        elapsedTime = 0.0f;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            for (int i = 0; i < 4; i++)
            {
                colors[i] = leaves[i].GetComponent<Image>().color;
                colors[i].a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
                leaves[i].GetComponent<Image>().color = colors[i];
            }
        }
        leafPanel.SetActive(false);
    }
    
    void AlterShownLeaves()
    {
        Color[] colors = new Color[4];
        int filledOutLeaves = SaveSystem.LoadLeafData().Count % 4;
        for(int i = 0; i < 4; i++)
        {
            colors[i] = leaves[i].GetComponent<Image>().color;
            if((filledOutLeaves == 0) && (SaveSystem.LoadLeafData().Count != 0))
            {
                colors[i].r = 1;
                colors[i].g = 1;
                colors[i].b = 1;
            }
            else
            {
                if (i < filledOutLeaves)
                {
                    colors[i].r = 1;
                    colors[i].g = 1;
                    colors[i].b = 1;
                }

                else
                {
                    colors[i].r = 0;
                    colors[i].g = 0;
                    colors[i].b = 0;
                }
            }
           
            leaves[i].GetComponent<Image>().color = colors[i];
        }
    }
}
