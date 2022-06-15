using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 3.0f;
    public float sprintingspeed = 5.0f;
    bool sprinting = false;

    Vector2 movement;

    public Text healthtext;
    public Image healthbar;


    public float health, MaxHealth = 100;

    public GameObject gameoverScreen;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
        gameoverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        healthtext.text = "Health: " + Mathf.RoundToInt(health) + "%";
        if(health <= 0)
        {
            health = 0;
            gameoverScreen.SetActive(true);
        }
        if (health > MaxHealth) health = MaxHealth;
        HealthBarFiller();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            sprinting = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            sprinting = false;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void HealthBarFiller()
    {
        healthbar.fillAmount = health / MaxHealth;
    }

    private void Damage(float damagepoints)
    {
        health -= damagepoints;
    }
    private void Heal(float healpoints)
    {
        health += healpoints;
    }

    private void FixedUpdate()
    {
        if(sprinting)
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime * sprintingspeed);
        else
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime * moveSpeed);

    }
}
