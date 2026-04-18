using UnityEngine;

public class RandomAnimationOffset : MonoBehaviour
{
    
    void Start()
    {
        Animator myAnimator = GetComponent<Animator>();
        if (myAnimator != null)
        {
            // sets random starting point, for offset in same animation of many of the same objects
            // => e.g. many SwampMud-AnimationObjects ligned up in a row, with different movement patterns
            float randomStart = Random.Range(0f, 1f);
            myAnimator.SetFloat("Offset", randomStart);
        }
    }

    
    
}
