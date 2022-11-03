using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Effect : MonoBehaviour
{
    public int ID;
    public int turnsLeft;
    public bool attackModifier;
    public bool defenseModifier;
    public bool debuffer;
    public int maxNum = 1;
    static public int currentIndexAttack;
    static public int currentIndexDefence;

    public GameObject reloadUI;
    public GameObject ninjutsuUI;
    public GameObject ninjutsuResultUI;
    public GameObject EvasiveUI;
    public GameObject SmokeBombUI;

    public Effect[] inflictEffect;

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
            if (game.currentTurn == 1)
            {
                for(int i = 0; i < game.p1Effects.Count; i++)
                {
                    if (game.p1Effects[i].ID == ID)
                    {
                        game.p1Effects.RemoveAt(i);
                        i=game.p1Effects.Count;
                    }
                }
            } else
            {
                for (int i = 0; i < game.p2Effects.Count; i++)
                {
                    if (game.p2Effects[i].ID == ID)
                    {
                        game.p2Effects.RemoveAt(i);
                        i = game.p2Effects.Count;
                    }
                }
            }


            if (ID == 7)
            {
                game.ApplyDamage(0, 3);
            }

            Destroy(gameObject);
        }
    }

    public void getEffect(int _currentDmg, int newIndex)
    {
        currentDmg = _currentDmg;
        switch (ID)
        {
            case 1:
                currentIndexAttack = newIndex;
                Reload();
                break;
            case 2:
                currentIndexDefence = newIndex;
                Evasive();
                break;
            case 3:
                currentIndexDefence = newIndex;
                Bounty();
                break;
            case 4:
                KnockDown();
                break;
            case 5:
                currentIndexAttack = newIndex;
                Ninjutsu();
                break;
            case 6:
                currentIndexDefence = newIndex;
                SmokeBomb();
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
                game.p1Effects.RemoveAt(currentIndexAttack);
            }
            else
            {
                game.p2Effects.RemoveAt(currentIndexAttack);
            }
            game.startReload(this);
            KillMySelf(1);
        } else
        {
            game.HandleAttack(currentDmg);
            game.currentIndexAttack = currentIndexAttack;
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
                game.p1Effects.RemoveAt(currentIndexAttack);
            }
            else
            {
                game.p2Effects.RemoveAt(currentIndexAttack);
            }
            game.startNinjutsu(this);
            KillMySelf(5);
        }
        else
        {
            game.HandleAttack(currentDmg);
            game.currentIndexAttack = currentIndexAttack;
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
            game.HandleAttack(currentDmg,null,inflictEffect);
        } else
        {
            game.HandleAttack(currentDmg,null,null,0,0,true);
        }

    }

    void Evasive()
    {
        Instantiate(EvasiveUI, canvas.transform);
    }

    public void EvasiveOpc(int opc)
    {
        game = FindObjectOfType<GameManager>();
        if (opc == 1)
        {
            if (game.currentTurn == 2)
            {
                game.p1Effects.RemoveAt(currentIndexDefence);
            }
            else
            {
                game.p2Effects.RemoveAt(currentIndexDefence);
            }
            game.startEvasive(this);
            KillMySelf(2);
        }
        else
        {
            game.checkForDefenseEffects(currentDmg);
            game.currentIndexDefence = currentIndexDefence;
        }
    }

    public void EvasiveResults(int result)
    {
        if(result < 3)
            currentDmg = 0;
        game = FindObjectOfType<GameManager>();
        game.HandleAttack(currentDmg);
    }

    void SmokeBomb()
    {
        Instantiate(SmokeBombUI, canvas.transform);
    }

    public void SmokeBombOpc(int opc)
    {
        game = FindObjectOfType<GameManager>();
        if (opc == 1)
        {
            if (game.currentTurn == 2)
            {
                game.p1Effects.RemoveAt(currentIndexDefence);
            }
            else
            {
                game.p2Effects.RemoveAt(currentIndexDefence);
            }
            game.startEvasive(this);
            KillMySelf(6);
        }
        else
        {
            game.checkForDefenseEffects(currentDmg);
            game.currentIndexDefence = currentIndexDefence;
        }
    }

    public void SmokeBombResults(int result)
    {
        if (result < 4)
            currentDmg = 0;
        game = FindObjectOfType<GameManager>();
        game.HandleAttack(currentDmg);
    }

    void KnockDown()
    {
        game.tryiesLeft = 0;
    }

    void Bounty()
    {
        currentDmg += 2;
        game.currentIndexDefence = currentIndexDefence;
        game.HandleAttack(currentDmg);
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
