using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Caracteristicas")]
    public bool smallStraight;
    public bool largeStraight;
    public int basicsNeeded;
    public int specialsNeeded;
    public int sixesNeeded;
    public bool canAttack;
    public bool passive;
    [Header("Efectos de Ataque")]
    public int dmg;
    public int dmg2;
    public int dmg3;
    public int myRolledDice;
    public int enemyRolledDice;
    public bool unblockable;
    public Effect[] efectoEnMi;
    public Effect[] efectoEnOtro;

    public enum Especial{ NA, Showdown, dBlossom, wTheLine};
    public Especial especial;
    

    private int initialSpritePos;
    private GameManager game;
    private SpriteRenderer sprite;
    private BoxCollider2D box;
    private Vector2 initialPos;
    

    void Start()
    {
        game = FindObjectOfType<GameManager>();
        sprite = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        initialPos = transform.position;
        initialSpritePos = sprite.sortingOrder;
    }


    void Update()
    {
        if (canAttack)
        {
            sprite.color = new Color(1, 1, 1);
            box.enabled = true;
        }
        else
        {
            sprite.color = new Color(0.5f, 0.5f, 0.5f);
            box.enabled = false;
        }

        if (game.tryiesLeft < 3 && game.tryiesLeft>-1 && game.rollPhase) //Para checar si ya empezo el turno
            checkActivation();
    }

    private void OnMouseDown()
    {
        if (game.rollPhase)
        {
            if (!game.attackSelected)
            {
                transform.position = new Vector2(0, 0);
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                sprite.sortingOrder = 7;
                game.attackSelected = true;
                game.myAttack = this;
            }
            else
            {
                transform.position = initialPos;
                transform.localScale = new Vector3(1f, 1f, 1f);
                sprite.sortingOrder = initialSpritePos;
                game.attackSelected = false;
                game.myAttack = null;
            }
        }
    }

    void checkActivation()
    {
        if (smallStraight)
        {
            if(game.currentScores[0]>=1 && game.currentScores[1] >=1 && game.currentScores[2] >=1 && game.currentScores[3] >=1)
            {
                canAttack = true;
            } else if (game.currentScores[1] >=1 && game.currentScores[2] >=1 && game.currentScores[3] >=1 && game.currentScores[4] >=1)
            {
                canAttack = true;
            } else if (game.currentScores[2] >=1 && game.currentScores[3] >=1 && game.currentScores[4] >=1 && game.currentScores[5] >=1)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }
        }
        else if (largeStraight)
        {
            if ((game.currentScores[0] >=1 && game.currentScores[1] >=1 && game.currentScores[2] >=1 && game.currentScores[3] >=1 && game.currentScores[4] >=1)
                || (game.currentScores[1] >=1 && game.currentScores[2] >=1 && game.currentScores[3] >=1 && game.currentScores[4] >=1 && game.currentScores[5] >=1))
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }
        }
        else
        {
            if((game.currentScores[0]+ game.currentScores[1]+ game.currentScores[2]) >=basicsNeeded
                && (game.currentScores[3] + game.currentScores[4]) >=specialsNeeded
                && game.currentScores[5] >= sixesNeeded
                )
            {
                canAttack = true;
            } else
            {
                canAttack = false;
            }
        }
    }

    public void SendAttack()
    {
        box.enabled = false;
        if ((game.currentScores[0] + game.currentScores[1] + game.currentScores[2]) > 4 && dmg3 != 0) //Pa ver si es un basico fuerte
        {
            game.HandleAttack(dmg3);
        } else if ((game.currentScores[0] + game.currentScores[1] + game.currentScores[2]) > 3 && dmg2 != 0) //Pa ver si es un basico medio
        {
            game.HandleAttack(dmg2);
        } else
        {
            game.HandleAttack(dmg, efectoEnMi, efectoEnOtro, myRolledDice, enemyRolledDice, unblockable);
        }
    }

    public void diceResults(int[] values)
    {
        int _dmg = 0;
        bool _unblockable = false;
        switch (especial){
            case Especial.Showdown:
                if (values[0] == -1)
                {
                    //GANÓ
                    game.HandleAttack(7);
                } else if (values[0] == -2)
                {
                    //PERDIO
                    game.HandleAttack(5);
                }
                break;
            case Especial.dBlossom:
                _dmg = 1 * (values[0] + values[1] + values[2]);
                _dmg += 2 * (values[3] + values[4]);
                if(values[5] > 0)
                {
                    _unblockable = true;
                }
                game.HandleAttack(_dmg, null, null, 0, 0, _unblockable);
                break;
            case Especial.wTheLine:
                _dmg = values[0] + values[1] * 2 + values[2] * 3 + values[3] * 4 + values[4] * 5 + values[5] * 6;
                if (_dmg <= 6)
                {
                    _unblockable = true;
                }
                game.HandleAttack(_dmg, null, null, 0, 0, _unblockable);
                break;
            default:
                break;
        }
    }
}
