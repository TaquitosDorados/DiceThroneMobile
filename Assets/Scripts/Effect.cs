using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Effect : MonoBehaviour
{
    public int ID;
    public int turnsLeft;
    public bool attackModifier;
    public bool defenseModifier;
    public bool deBuffer;
    public int maxNum = 1;
    static public int currentIndex;

    public GameObject reloadUI;
    public GameObject ninjutsuUI;
    public GameObject ninjutsuResultUI;

    private GameObject canvas;
    private GameManager game;
    [SerializeField]static private int currentDmg;

    private void Start()
    {
        canvas = GameObject.Find("/Canvas");
        game = FindObjectOfType<GameManager>();
    }

    public void TurnPassed()
    {
        turnsLeft--;

        if (turnsLeft == 0)
        {
            Destroy(gameObject);
        }
    }

    public void getEffect(int _currentDmg, int newIndex)
    {
        currentDmg = _currentDmg;
        currentIndex = newIndex;
        switch (ID)
        {
            case 1:
                Reload();
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                Ninjutsu();
                break;
            case 6:
                break;
            case 7:
                break;
        }
    }

    void Reload()
    {
        Instantiate(reloadUI, canvas.transform);
    }

    public void ReloadOpc(int opc)
    {
        game = FindObjectOfType<GameManager>();
        if (opc == 1)
        {
            if (game.currentTurn == 1)
            {
                game.p1Effects.RemoveAt(currentIndex);
            }
            else
            {
                game.p2Effects.RemoveAt(currentIndex);
            }
            game.startReload(this);
            KillMySelf(1);
        } else
        {
            game.HandleAttack(currentDmg);
            game.currentIndex = currentIndex;
        }
    }

    public void ReloadResults(int result)
    {
        currentDmg += (int)Mathf.Ceil((float)result / 2);
        game = FindObjectOfType<GameManager>();
        game.HandleAttack(currentDmg);
    }

    void Ninjutsu()
    {
        Instantiate(ninjutsuUI, canvas.transform);
        
    }

    public void NinjutsuOpc(int opc)
    {
        game = FindObjectOfType<GameManager>();
        if (opc == 1)
        {
            if (game.currentTurn == 1)
            {
                game.p1Effects.RemoveAt(currentIndex);
            }
            else
            {
                game.p2Effects.RemoveAt(currentIndex);
            }
            game.startNinjutsu(this);
            KillMySelf(5);
        }
        else
        {
            game.HandleAttack(currentDmg);
            game.currentIndex = currentIndex;
        }
    }

    public void NinjutsuResults(int result)
    {
        game = FindObjectOfType<GameManager>();

        if (result < 4)
        {
            currentDmg += 1;
            game.HandleAttack(currentDmg);

        } else if (result < 6)
        {
            currentDmg += 2;
            game.HandleAttack(currentDmg);

        }
        else
        {
            canvas = GameObject.Find("/Canvas");
            Instantiate(ninjutsuResultUI, canvas.transform);
        }
    }

    public void NinjutsuResultOpc(int opc)
    {
        game = FindObjectOfType<GameManager>();
        if (opc == 1)
        {
            currentDmg += 2;
            game.HandleAttack(currentDmg);
        } else if (opc == 2)
        {
            //INFLINGIR POISON!!!!
            game.HandleAttack(currentDmg);
        } else
        {
            game.HandleAttack(currentDmg,null,null,0,0,true);
        }

    }

    void KillMySelf(int _id)
    {
        Effect[] effects = FindObjectsOfType<Effect>();

        for(int i = 0; i < effects.Length; i++)
        {
            if (effects[i].ID == _id) {
                Destroy(effects[i].gameObject);
                return;
            }
        }
    }
}
