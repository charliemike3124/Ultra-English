using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showLoginObjects : MonoBehaviour
{
    Image loginFill;
    bool isFilled = false; 

    void Start()
    {
        loginFill = GetComponent<Image>();

        
    }

    private void Update()
    {
        if (loginFill.fillAmount < 1)
        {
            loginFill.fillAmount += Time.deltaTime;
        }
        if(loginFill.fillAmount == 1)
        {
            isFilled = true;
        }
    }

}
