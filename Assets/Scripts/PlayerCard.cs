using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    public int ID;
    public GameObject anim;
    private BoxCollider2D box;
    private SelectFighters game;
    private SpriteRenderer sprite;
    private void Start()
    {
        game = FindObjectOfType<SelectFighters>();
        box = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        Instantiate(anim);
        game.ChooseFighter(ID);
        box.enabled = false;
        sprite.color = new Color(0.5f, 0.5f, 0.5f);
    }

}
