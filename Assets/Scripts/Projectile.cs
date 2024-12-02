using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
   
    public Rigidbody2D projectile;
    void Start()
    {
        
    }

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, 2f);
    }
   
    void Update()
    {
        projectile.transform.Translate(direction * 5 * Time.deltaTime);       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PopperHandler popper = collision.GetComponent<PopperHandler>();
        if (popper != null)
        {
            Destroy(gameObject);
            popper.handleTap(); 
            
        }
    }
    void DestroyAllProjectiles()
    {
        Projectile[] projectiles = FindObjectsOfType<Projectile>();

        foreach (Projectile projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }
    }
}

