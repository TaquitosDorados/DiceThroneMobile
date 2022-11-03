using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject rollButton;
    public GameObject attackButton;
    public GameObject endTurnButton;
    public GameObject shieldIcon;
    public GameObject rollPhaseIcon;
    public GameObject mainPanel;

    public int[] currentScores;
    public int tryiesLeft = 3;

    public int currentIndexAttack = -1;
    public int currentIndexDefence = -1;

    public int whoIsGS = 1; //NOMAS PA DARLE SU PASIVA

    public Dice player1Dice;
    public Dice player2Dice;

    public Attack[] player1Attacks;
    public Attack[] player2Attacks;

    public Defense player1Defense;
    public Defense player2Defense;

    public Effect[] effectList;

    public Transform initialDicePos;
    public Transform initialAttacksPos;
    public bool rollPhase = true;
    public bool attackSelected;
    public Attack myAttack;

    public Text RollPhaseText;
    public Text p1LifeText;
    public Text p2LifeText;
    public Text dmgText;
    public Slider p1LifeBar;
    public Slider p2LifeBar;

    public List<Effect> p1Effects;
    public List<Effect> p2Effects;

    public Animator p1Anim;
    public Animator p2Anim;

    [SerializeField] private Dice[] currentDice;
    [SerializeField] private Attack[] currentAttacks;
    private int numSelected = 0;
    public int currentTurn = 2;
    private Defense currentDefender;
    
    private bool currentlyUnblockable;
    private bool currentlyUltimate;
    [SerializeField]private int p1Life;
    [SerializeField] private int p2Life;

    void Start()
    {
        currentTurn = PlayerPrefs.GetInt("P2char");
        startChangeTurn();
        p1Life = 50;
        p2Life = 50;
        p1LifeText.text = p1Life + " HP";
        p2LifeText.text = p2Life + " HP";
    }

    void Update()
    {
        numSelected = 0;
        if (tryiesLeft > 0 && rollPhase)
        {
            if (currentDice[0] != null)
            {
                for (int i = 0; i < currentDice.Length; i++)
                {
                    if (currentDice[i].selected)
                        numSelected++;
                }
            }
        }

        if (numSelected > 0)
        {
            rollButton.SetActive(true);
        } else
        {
            rollButton.SetActive(false);
        }

        if (attackSelected && rollPhase)
        {
            attackButton.SetActive(true);
            rollButton.SetActive(false);
        } else
        {
            attackButton.SetActive(false);
        }

        
    }

    public void Roll()
    {
        StartCoroutine(Rolling());
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentDice[i].selected)
            {
                currentDice[i].Roll();
            }
        }
        endTurnButton.SetActive(false);
    }

    void checkForDebuffers()
    {

        if (currentTurn == 1)
        {
            for (int i = 0; i < p1Effects.Count; i++)
            {
                if (p1Effects[i].debuffer)
                {
                    p1Effects[i].getEffect(0, -1);
                }
            }
        }
        else
        {
            for (int i = 0; i < p2Effects.Count; i++)
            {
                if (p2Effects[i].debuffer)
                {
                    p2Effects[i].getEffect(0, -1);
                }
            }
        }
    }

    IEnumerator Rolling()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentDice[i].selected)
            {
                currentDice[i].ChangeValue(Random.Range(1, 7));
            }
        }

        checkScores();
        tryiesLeft--;
                checkForDebuffers();
    }

    void checkScores()
    {
        currentScores = new int[6];
        for (int i = 0; i < currentDice.Length; i++)
        {
            currentScores[currentDice[i].currentValue - 1]++; //Si el dado es un cuatro, sube score en pos 3 para saber que hay un 4
        }
        if(rollPhase)
        endTurnButton.SetActive(true);
    }

    public void startChangeTurn()
    {
        StartCoroutine(changeTurns());

    }

    IEnumerator changeTurns()
    {
        dmgText.text = "";

        currentlyUltimate = false;
        currentlyUnblockable = false;
        currentIndexAttack = -1;
        currentIndexDefence = -1;
        destroyCurrentDice();
        destroyCurrentAttacks();
        if(currentDefender!=null)
            Destroy(currentDefender.gameObject);
        currentDefender = null;
        attackSelected = false;

        EffectTurnPassed();

        p1Anim.Play("Idle");
        p2Anim.Play("Idle");

        endTurnButton.SetActive(false);

        rollPhaseIcon.SetActive(true);
        if (currentTurn == 1)
        {
            RollPhaseText.text = "Player 2";
        } else
        {
            RollPhaseText.text = "Player 1";
        }
        RollPhaseText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        rollPhaseIcon.SetActive(false);
        RollPhaseText.gameObject.SetActive(false);

        if (currentTurn == 1)
        {
            currentTurn = 2;

            for (int i = 0; i < 5; i++)  //Spawnear Dados
            {
                var newDice = Instantiate(player2Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                //Destroy(currentDice[i].gameObject);
                currentDice[i] = newDice;
            }

            float attackSpawnStep = 15f / (((player2Attacks.Length-1) / 2f));

            for (int i = 0; i < player2Attacks.Length-1; i++) //Spawnear Ataques
            {
                var newAttacks = Instantiate(player2Attacks[i]);

                if (i % 2 != 0)
                {
                    newAttacks.transform.position = new Vector2(initialAttacksPos.position.x + (attackSpawnStep * (int)i / 2), initialAttacksPos.position.y - 2.2f);
                    newAttacks.GetComponent<SpriteRenderer>().sortingOrder = 4;
                }
                else
                {
                    newAttacks.transform.position = new Vector2(initialAttacksPos.position.x + (attackSpawnStep * (i / 2)), initialAttacksPos.position.y);
                    newAttacks.GetComponent<SpriteRenderer>().sortingOrder = 3;
                }

                //Destroy(currentAttacks[i].gameObject);
                currentAttacks[i] = newAttacks;
            }
            var newAttack = Instantiate(player2Attacks[player2Attacks.Length - 1]);
            //Destroy(currentAttacks[player2Attacks.Length - 1].gameObject);
            currentAttacks[player2Attacks.Length - 1] = newAttack;

        } else if (currentTurn == 2)
        {
            currentTurn = 1;

            for (int i = 0; i < 5; i++)//Spawnear Dados
            {
                var newDice = Instantiate(player1Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                //Destroy(currentDice[i].gameObject);
                currentDice[i] = newDice;
            }

            float attackSpawnStep = 15f / (((player1Attacks.Length-1) / 2f));

            for (int i = 0; i < player1Attacks.Length-1; i++) //Spawnear Ataques
            {
                var newAttacks = Instantiate(player1Attacks[i]);

                if (i % 2 != 0)
                {
                    newAttacks.transform.position = new Vector2(initialAttacksPos.position.x + (attackSpawnStep * (int)i / 2), initialAttacksPos.position.y - 2.2f);
                    newAttacks.GetComponent<SpriteRenderer>().sortingOrder = 4;
                }
                else
                {
                    newAttacks.transform.position = new Vector2(initialAttacksPos.position.x + (attackSpawnStep * (i / 2)), initialAttacksPos.position.y);
                    newAttacks.GetComponent<SpriteRenderer>().sortingOrder = 3;
                }

                //Destroy(currentAttacks[i].gameObject);
                currentAttacks[i] = newAttacks;
            }
            var newAttack = Instantiate(player1Attacks[player1Attacks.Length - 1]);
            //Destroy(currentAttacks[player1Attacks.Length - 1].gameObject);
            currentAttacks[player1Attacks.Length - 1] = newAttack;
        }

        if(currentTurn == whoIsGS)
        {
            if (currentTurn == 1)
            {
                int sameFound = 0;
                for(int i = 0; i < p1Effects.Count; i++)
                {
                    if(p1Effects[i].ID == 1)
                    {
                        sameFound++;
                    }
                }

                if (sameFound < effectList[0].maxNum)
                {
                    var newReload = Instantiate(effectList[0]);
                    p1Effects.Add(newReload);
                }
            }
            else
            {
                int sameFound=0;
                for (int i = 0; i < p2Effects.Count; i++)
                {
                    if (p2Effects[i].ID == 1)
                    {
                        sameFound++;
                    }
                }

                if (sameFound < effectList[0].maxNum)
                {
                    var newReload = Instantiate(effectList[0]);
                    p2Effects.Add(newReload);
                }
            }

        }

        rollPhase = true;
        tryiesLeft = 3;
    }

    void EffectTurnPassed()
    {
        if (currentTurn == 1)
        {
            for (int i = 0; i < p1Effects.Count; i++)
            {
                p1Effects[i].TurnPassed();
            }
        }
        else
        {
            for (int i = 0; i < p2Effects.Count; i++)
            {
                p2Effects[i].TurnPassed();
            }
        }
    }

    public void getAttack()
    {
        endTurnButton.SetActive(false);
        myAttack.SendAttack();
        rollPhase = false;
        //attackSelected = false;
        attackButton.SetActive(false);    
    }

    public void HandleAttack(int dmg = 0, Effect[] effectMySelf = null, Effect[] effectOther = null, int numMyDice = 0, int numOthersDice = 0, bool isUnblockable = false, bool ultimate = false)
    {
        if (isUnblockable)
        currentlyUnblockable = isUnblockable;

        if(ultimate)
            currentlyUltimate = ultimate;
        //DARME EL EFECTO
        if (effectMySelf != null)
        {
            for(int i = 0; i < effectMySelf.Length; i++)
            {
                giveMeEffect(effectMySelf[i]);
            }
        }


        //DARLE EFECTO AL ENEMIGO
        if (effectOther != null)
        {
            for (int i = 0; i < effectOther.Length; i++)
            {
                giveOtherEffect(effectOther[i]);
            }
        }


            dmgText.text = "Current Dmg: " + dmg;
            if(currentlyUnblockable)
                dmgText.text += "\nUndefendable";
        

        if (numMyDice > 0 && numOthersDice > 0) //Ambos tienen que tirar dados
        {
            StartCoroutine(Duel(true));
        } else if(numMyDice > 0) //Tiene que tirar dado para hacer el efecto del ataque
        {
            StartCoroutine(RollForAttack(myAttack, numMyDice));
        } else //NO TIENE QUE TIRAR DADOS
        {
            Debug.Log("El atacante ha hecho" + dmg);
            if (dmg > 0)
            {
                checkForAttackModifiers(dmg);
            } else
            {
                endTurnButton.SetActive(true);
            }

        }
    }

    public void giveMeEffect(Effect myEffect)
    {
        if (currentTurn == 1)
        {
            int sameFound = 0;
            for (int i = 0; i < p1Effects.Count; i++)
            {
                if (p1Effects[i].ID == myEffect.ID)
                {
                    sameFound++;
                }
            }

            if (sameFound < effectList[myEffect.ID-1].maxNum)
            {
                var newEffect = Instantiate(effectList[myEffect.ID-1]);
                p1Effects.Add(newEffect);
            }
        }
        else
        {
            int sameFound = 0;
            for (int i = 0; i < p2Effects.Count; i++)
            {
                if (p2Effects[i].ID == myEffect.ID)
                {
                    sameFound++;
                }
            }

            if (sameFound < effectList[myEffect.ID-1].maxNum)
            {
                var newEffect = Instantiate(effectList[myEffect.ID-1]);
                p2Effects.Add(newEffect);
            }
        }
    }

    public void giveOtherEffect(Effect myEffect)
    {
        if (currentTurn == 2)
        {
            int sameFound = 0;
            for (int i = 0; i < p1Effects.Count; i++)
            {
                if (p1Effects[i].ID == myEffect.ID)
                {
                    sameFound++;
                }
            }

            if (sameFound < effectList[myEffect.ID - 1].maxNum)
            {
                var newEffect = Instantiate(effectList[myEffect.ID - 1]);
                p1Effects.Add(newEffect);
            }
        }
        else
        {
            int sameFound = 0;
            for (int i = 0; i < p2Effects.Count; i++)
            {
                if (p2Effects[i].ID == myEffect.ID)
                {
                    sameFound++;
                }
            }

            if (sameFound < effectList[myEffect.ID - 1].maxNum)
            {
                var newEffect = Instantiate(effectList[myEffect.ID - 1]);
                p2Effects.Add(newEffect);
            }
        }
    }

    void checkForAttackModifiers(int _currentdmg)
    {
        Debug.Log("En check modifier" + _currentdmg);
        if (currentTurn == 1)
        {
            for(int i = currentIndexAttack+1; i < p1Effects.Count; i++)
            {
                if (p1Effects[i].attackModifier)
                {
                    p1Effects[i].getEffect(_currentdmg, i);
                    return;
                }
                currentIndexAttack = i;
            }
            p1Anim.Play("Attack");
        } else
        {
            for (int i = currentIndexAttack+1; i < p2Effects.Count; i++)
            {
                if (p2Effects[i].attackModifier)
                {
                    p2Effects[i].getEffect(_currentdmg, i);
                    return;
                }
                currentIndexAttack = i;
            }
            p2Anim.Play("Attack");
        }

        /*if (currentlyUnblockable)
        {
            ApplyDamage(_currentdmg);
        }
        else
        {
            startDefense(_currentdmg);
        }*/
        currentIndexAttack = -1;
        if (!currentlyUltimate)
        {
            StartCoroutine(DefenseAnim(_currentdmg));
        } else
        {
            ApplyDamage(_currentdmg);
        }
    }

    IEnumerator DefenseAnim(int _currentdmg)
    {
        shieldIcon.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        shieldIcon.SetActive(false);
        checkForDefenseEffects(_currentdmg);
    }

    public void checkForDefenseEffects(int _currentDmg)
    {

        if (currentTurn == 2)
        {
            for (int i = currentIndexDefence + 1; i < p1Effects.Count; i++)
            {
                if (p1Effects[i].defenseModifier)
                {
                    p1Effects[i].getEffect(_currentDmg, i);
                    return;
                }
                currentIndexDefence = i;
            }
        }
        else
        {
            for (int i = currentIndexDefence + 1; i < p2Effects.Count; i++)
            {
                if (p2Effects[i].defenseModifier)
                {
                    p2Effects[i].getEffect(_currentDmg, i);
                    return;
                }
                currentIndexDefence = i;
            }
        }

        currentIndexDefence = -1;

        if (currentlyUnblockable)
        {
            ApplyDamage(_currentDmg);
        }
        else
        {
            startDefense(_currentDmg);
        }
    }

    IEnumerator RollForAttack(Attack myAttack, int numMyDice)
    {
        destroyCurrentDice();
        yield return new WaitForSeconds(1.0f);
        currentDice = new Dice[numMyDice];
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentTurn == 1)
            {
                var newDice = Instantiate(player1Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
            else
            {
                var newDice = Instantiate(player2Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
        }
        yield return new WaitForSeconds(1.0f);
        Roll();
        yield return new WaitForSeconds(7.0f);

        myAttack.diceResults(currentScores);
        //REGRESARLE LOS VALORES AL ATAQUE
        
    }

    public void startDefenseRoll(int numDice)
    {
        StartCoroutine(DefenseRoll(numDice));
    }

    IEnumerator DefenseRoll(int numDice)
    {
        destroyCurrentDice();
        yield return new WaitForSeconds(1.0f);
        currentDice = new Dice[numDice];
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentTurn == 2)
            {
                var newDice = Instantiate(player1Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
            else
            {
                var newDice = Instantiate(player2Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
        }
        yield return new WaitForSeconds(1.0f);
        Roll();
        yield return new WaitForSeconds(3.0f);
        currentDefender.HandleResults(currentScores);
    }

    public void startDuel()
    {
        StartCoroutine(Duel());
    }

    public IEnumerator Duel(bool _isAttack = false)
    {
        destroyCurrentDice();

        currentDice = new Dice[2];

        var newDice = Instantiate(player1Dice);
        newDice.transform.position = new Vector2(initialDicePos.position.x, initialDicePos.position.y);
        newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
        currentDice[0] = newDice;
        newDice = Instantiate(player2Dice);
        newDice.transform.position = new Vector2(initialDicePos.position.x + 12, initialDicePos.position.y);
        newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
        currentDice[1] = newDice;

        yield return new WaitForSeconds(1.0f);
        Roll();
        yield return new WaitForSeconds(3.0f);

        if (_isAttack)
        {
            int[] a = new int[1];
            if (currentDice[0].currentValue >= currentDice[1].currentValue)
            {
                if(currentTurn == 1)
                {
                    //El Atacante gana
                    a[0] = -1;
                    myAttack.diceResults(a);
                }
                else
                {
                    //El Atacante pierde
                    a[0] = -2;
                    myAttack.diceResults(a);
                }
            }
            else
            {
                if (currentTurn == 1)
                {
                    //El Atacante pierde
                    a[0] = -2;
                    myAttack.diceResults(a);
                }
                else
                {
                    //El Atacante gana
                    a[0] = -1;
                    myAttack.diceResults(a);
                }
            }
        } else
        {
            //Es la defensa de GunSlinger
            int[] a = new int[1];
            if (currentDice[0].currentValue > currentDice[1].currentValue)
            {
                if (currentTurn == 1)
                {
                    //El Defensor pierde
                    a[0] = -2;
                    currentDefender.HandleResults(a);
                }
                else
                {
                    //El Defensor gana
                    a[0] = -1;
                    currentDefender.HandleResults(a);
                }
            }
            else
            {
                if (currentTurn == 1)
                {
                    //El Defensor Gana
                    a[0] = -1;
                    currentDefender.HandleResults(a);
                }
                else
                {
                    //El Defensor Pierde
                    a[0] = -2;
                    currentDefender.HandleResults(a);
                }
            }
        }

    }

    public void startDefense(int _dmg)
    {
        currentScores = new int[6];
        //destroyCurrentDice();
        

        if (currentTurn == 1)
        {
            myAttack.gameObject.transform.position = new Vector2(-2f, 0f);
            myAttack.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var inst = Instantiate(player2Defense);
            inst.transform.position = new Vector2(2f, 0f);
            inst.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            currentDefender = inst;
        } else
        {
            myAttack.gameObject.transform.position = new Vector2(2f, 0f);
            myAttack.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            var inst = Instantiate(player1Defense);
            inst.transform.position = new Vector2(-2f, 0f);
            inst.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            currentDefender = inst;
        }

        currentDefender.startDefense(_dmg);
    }

    public void ApplyDamage(int defenderDMGReceived, int attackerDMGReceived = 0)
    {
        p1Anim.Play("Idle");
        p2Anim.Play("Damaged");

        if (attackerDMGReceived > 0)
        {
            dmgText.text += "\nDmg Returned: " + attackerDMGReceived;
            if (currentTurn == 1)
            {
                p1Anim.Play("Damaged");
            } else
            {
                p2Anim.Play("Damaged");
            }
        }

        if(currentTurn == 1)
        {
            p2Life -= defenderDMGReceived;
            p2LifeText.text = p2Life + " HP";
            p1Life -= attackerDMGReceived;
            p1LifeText.text = p1Life + " HP";
            if(defenderDMGReceived>0)
                p2Anim.Play("Damaged");
        } else
        {
            p1Life -= defenderDMGReceived;
            p1LifeText.text = p1Life + " HP";
            p2Life -= attackerDMGReceived;
            p2LifeText.text = p2Life + " HP";
            if (defenderDMGReceived > 0)
                p1Anim.Play("Damaged");
        }

        p1LifeBar.value = (float)p1Life / 50.0f;
        p2LifeBar.value = (float)p2Life / 50.0f;

        checkForGame();

        endTurnButton.SetActive(true);
    }

    public void startReload(Effect myReload)
    {
        StartCoroutine(reload(myReload));
    }

    IEnumerator reload(Effect myReload)
    {
        destroyCurrentDice();
        yield return new WaitForSeconds(1.0f);
        currentDice = new Dice[1];
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentTurn == 1)
            {
                var newDice = Instantiate(player1Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
            else
            {
                var newDice = Instantiate(player2Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
        }
        yield return new WaitForSeconds(1.0f);
        Roll();
        yield return new WaitForSeconds(4.0f);
        myReload.ReloadResults(currentDice[0].currentValue);
    }

    public void startNinjutsu(Effect myNinjutsu)
    {
        StartCoroutine(ninjutsu(myNinjutsu));
    }

    IEnumerator ninjutsu(Effect myNinjutsu)
    {
        destroyCurrentDice();
        yield return new WaitForSeconds(1.0f);
        currentDice = new Dice[1];
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentTurn == 1)
            {
                var newDice = Instantiate(player1Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
            else
            {
                var newDice = Instantiate(player2Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
        }
        yield return new WaitForSeconds(1.0f);
        Roll();
        yield return new WaitForSeconds(4.0f);
        myNinjutsu.NinjutsuResults(currentDice[0].currentValue);
    }

    public void startEvasive(Effect myEvasive)
    {
        StartCoroutine(evasive(myEvasive));
    }

    IEnumerator evasive(Effect myEvasive)
    {
        destroyCurrentDice();
        yield return new WaitForSeconds(1.0f);
        currentDice = new Dice[1];
        for (int i = 0; i < currentDice.Length; i++)
        {
            if (currentTurn == 2)
            {
                var newDice = Instantiate(player1Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
            else
            {
                var newDice = Instantiate(player2Dice);
                newDice.transform.position = new Vector2(initialDicePos.position.x + (3.0f * i), initialDicePos.position.y);
                newDice.GetComponent<SpriteRenderer>().sortingOrder = 8;
                currentDice[i] = newDice;
            }
        }
        yield return new WaitForSeconds(1.0f);
        Roll();
        yield return new WaitForSeconds(4.0f);
        myEvasive.EvasiveResults(currentDice[0].currentValue);
    }

    void destroyCurrentDice()
    {
        for (int i = 0; i < currentDice.Length; i++) //Destruir los dados anteriores
        {
            Destroy(currentDice[i].gameObject);
        }

        currentDice = new Dice[5];
    }

    void destroyCurrentAttacks()
    {
        for (int i = 0; i < currentAttacks.Length; i++)
        {
            Destroy(currentAttacks[i].gameObject);
        }
    }

    public GameObject explosion;
    public GameObject GameOverScreen;
    public TextMeshProUGUI WinnerText;
    void checkForGame()
    {
        if(p1Life<=0&& p2Life <= 0)
        {
            mainPanel.SetActive(false);
            destroyCurrentDice();
            destroyCurrentAttacks();
            if (currentDefender != null)
                Destroy(currentDefender.gameObject);
            WinnerText.text = "Draw";
            GameOverScreen.SetActive(true);
            Instantiate(explosion, p1Anim.gameObject.transform);
            Instantiate(explosion, p2Anim.gameObject.transform);
        } else if(p1Life <= 0)
        {
            mainPanel.SetActive(false);
            destroyCurrentDice();
            destroyCurrentAttacks();
            if (currentDefender != null)
                Destroy(currentDefender.gameObject);
            WinnerText.text = "Ninja Wins";
            GameOverScreen.SetActive(true);
            Instantiate(explosion, p1Anim.gameObject.transform);
        } else if(p2Life <= 0)
        {
            mainPanel.SetActive(false);
            destroyCurrentDice();
            destroyCurrentAttacks();
            if (currentDefender != null)
                Destroy(currentDefender.gameObject);
            WinnerText.text = "Gunslinger Wins";
            GameOverScreen.SetActive(true);
            Instantiate(explosion, p2Anim.gameObject.transform);
        }
    }
}
