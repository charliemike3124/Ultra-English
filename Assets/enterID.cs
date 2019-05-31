using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enterID : MonoBehaviour
{
    private Animator examAnimator;
    private Animator fillBlanksAnimator;
    private Animator readingAnimator;

    private Animator animator;
    private void Awake()
    {
        examAnimator = GameObject.Find("Button Exam").GetComponent<Animator>();
        fillBlanksAnimator = GameObject.Find("Button Fill Blanks").GetComponent<Animator>();
        readingAnimator = GameObject.Find("Button Reading").GetComponent<Animator>();

        animator = GameObject.Find("Input ID").GetComponent<Animator>();

    }
    void Update()
    {
        if(examAnimator.GetInteger("examParameter") == 0 && fillBlanksAnimator.GetInteger("fillParameter") == 0)
        {
            animator.SetInteger("param", 3); //ir a reading
        }

        else if (fillBlanksAnimator.GetInteger("fillParameter") == 0 && readingAnimator.GetInteger("readingParameter") == 0)
        {
            animator.SetInteger("param", 1); //ir a exam
        }

        else if (readingAnimator.GetInteger("readingParameter") == 0 && examAnimator.GetInteger("examParameter") == 0)
        {
            animator.SetInteger("param", 2); //ir a fill
        }
        else
        {
            animator.SetInteger("param", 0);
        }
    }

    public void goBack()
    {
        examAnimator.SetInteger("examParameter",1);
        fillBlanksAnimator.SetInteger("fillParameter",1);
        readingAnimator.SetInteger("readingParameter",1);
    }
}
