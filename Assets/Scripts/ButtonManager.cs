using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("Animators Login Screen")]
    public Animator loginCanvasAnimator;
    public Animator registerCanvasAnimator;
    public Animator appTitleAnimator;

    [Header("Animators Questions Menu")]
    public Animator examAnimator;
    public Animator fillBlanksAnimator;
    public Animator readingAnimator;



    


    /// <summary>
    /// For the login screen
    /// </summary>
    public void loginScreenEnterAnimation()
    {

        loginCanvasAnimator.SetInteger("loginParameter", 2);
        appTitleAnimator.SetInteger("titleParameter", 1);
    }
    public void loginScreenLoadScene()
    {
        SceneManager.LoadScene("Questions Menu");
    }

    public void showLogin()
    {
        loginCanvasAnimator.SetInteger("loginParameter", 1);        
    }
    public void hideLogin()
    {
        loginCanvasAnimator.SetInteger("loginParameter", 0);
    }

    public void showRegister()
    {
        registerCanvasAnimator.SetInteger("registerParameter", 1);
    }
    public void hideRegister()
    {
        registerCanvasAnimator.SetInteger("registerParameter", 0);
    }

    /// <summary>
    /// For the questions menu
    /// </summary>

    public void showExam()
    {
        examAnimator.SetInteger("examParameter", 1);
    }
    public void hideExam()
    {
        examAnimator.SetInteger("examParameter", 0);
    }    

    public void showFillBlanks()
    {
        fillBlanksAnimator.SetInteger("fillParameter", 1);
    }
    public void hideFillBlanks()
    {
        fillBlanksAnimator.SetInteger("fillParameter", 0);
    }

    public void showReading()
    {
        readingAnimator.SetInteger("readingParameter", 1);
    }
    public void hideReading()
    {
        readingAnimator.SetInteger("readingParameter", 0);
    }

}
