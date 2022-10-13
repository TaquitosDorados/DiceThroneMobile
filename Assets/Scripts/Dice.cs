using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public int currentValue = 0;
    public bool selected = true;

    private bool rolling;
    private Animator animator;
    private BoxCollider2D box;
    private GameManager game;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        game = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (selected)
        {
            transform.localScale = new Vector3(1, 1, 1);
        } else
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        if (game.attackSelected)
        {
            box.enabled = false;
        }
        else
        {
            box.enabled = true;
        }
    }

    public void ChangeValue(int _newValue)
    {
        currentValue = _newValue;
        animator.Play(_newValue.ToString());
        rolling = false;
        selected = false;
        box.enabled = true;
    }

    public void Roll()
    {
        animator.Play("Rolling");
        rolling = true;
        box.enabled = false;
    }

    private void OnMouseDown()
    {
        if(!rolling && !game.attackSelected && game.tryiesLeft<3)
        selected = !selected;
    }
}
