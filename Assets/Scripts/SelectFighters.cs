using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFighters : MonoBehaviour
{
    public GameObject btnFight;

    private bool p1selected;
    public void ChooseFighter(int ID)
    {
        if (!p1selected)
        {
            //PlayerPrefs.SetInt("P1char", ID);
            p1selected = true;
        }
        else
        {
            //PlayerPrefs.SetInt("P2char", ID);
            btnFight.SetActive(true);
        }
    }


}
