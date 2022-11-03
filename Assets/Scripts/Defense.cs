using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{
    public enum Character { Gunslinger, Ninja};
    public Character character;   
    public GameObject myOptions;
    public Effect[] applySelf;
    public Effect[] applyOther;
    
    private GameManager game;
    static private int dmgRecieved;
    private GameObject canvas;
    static private int returnDamage;
    private bool givingSmoke;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
        canvas = GameObject.Find("/Canvas");
    }
    // Start is called before the first frame update
    void Start()
    {
        game.tryiesLeft = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startDefense(int _dmg)
    {
        switch (character)
        {
            case Character.Gunslinger:
                GunSlingerDefense(_dmg);
                break;
            case Character.Ninja:
                NinjaDefense(_dmg);
                break;
            default:
                break;
        }
    }

    private void GunSlingerDefense(int _dmgRecieved)
    {
        dmgRecieved = _dmgRecieved;
        Debug.Log("He recibido " + dmgRecieved);
        game.startDuel();
    }

    private void NinjaDefense(int _dmgRecieved)
    {
        dmgRecieved = _dmgRecieved;
        Debug.Log("He recibido " + dmgRecieved);
        game.startDefenseRoll(3);
    }

    public void HandleResults(int[] results)
    {
        switch (character)
        {
            case Character.Gunslinger:
                GunSlingerResults(results);
                break;
            case Character.Ninja:
                NinjaResults(results);
                break;
            default:
                break;
        }
    }

    void GunSlingerResults(int[] results)
    {
        if (results[0] == -1)
        {
            //GANÓ
            Debug.Log("Gunslinger ganó la defensa");
            Instantiate(myOptions, canvas.transform);
        }
        else if (results[0] == -2)
        {
            Debug.Log("Gunslinger perdió la defensa");
            game.ApplyDamage(dmgRecieved, 1);
        }
    }

    public void GSOptionSelect(int opc)
    {
        game = FindObjectOfType<GameManager>();
        if (opc == 1)
        {
            Debug.Log("Se regresó 3 de daño");
            int _dmg = dmgRecieved;
            game.ApplyDamage(_dmg, 3);
        } else
        {
            int _dmg = dmgRecieved/2;
            Debug.Log(dmgRecieved);
            game.ApplyDamage(_dmg);
        }
    }

    void NinjaResults(int[] results)
    {
        game = FindObjectOfType<GameManager>();

        returnDamage = 0;
        givingSmoke = false;
        if (results[0] + results[1] + results[2] > 0)
        {
            returnDamage += 1;
        }
        if (results[3] + results[4] > 0)
        {
            returnDamage += 2;
        }
        if (results[5] > 1)
        {
            givingSmoke = true;
        }

        if (game.tryiesLeft == 1)
        {
            Instantiate(myOptions, canvas.transform);
        }
        else
        {
            game.ApplyDamage(dmgRecieved, returnDamage);
        }
    }

    public void NinjaReroll()
    {
        game=FindObjectOfType<GameManager>();
        game.startDefenseRoll(3);
        game.tryiesLeft = 0;
    }

    public void NinjaAccept()
    {
        game = FindObjectOfType<GameManager>();
        if (givingSmoke)
            game.giveOtherEffect(applySelf[0]);
        game.ApplyDamage(dmgRecieved, returnDamage);
    }
}
