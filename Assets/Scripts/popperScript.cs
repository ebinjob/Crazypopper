using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopperHandler : MonoBehaviour
{
    public GameObject explosionSprite;
    public GameObject projectileSprite;

    public GameObject popperPurple;
    public GameObject popperBlue;
    public GameObject PopperYellow;

    public float projectileSpeed = 5f; 
    public float explosionDuration = 0.5f;
    
    void OnMouseDown()
    {
        GameManager.Instance.remainingTaps--;
        this.handleTap();

    }
    Vector2Int GetGridPosition()
    {
        int gridX = Mathf.FloorToInt(transform.position.x);
        int gridY = Mathf.FloorToInt(transform.position.y); 
        gridX = Mathf.Clamp(gridX, 0, GameManager.Instance.gridRows - 1);
        gridY = Mathf.Clamp(gridY, 0, GameManager.Instance.gridColumns - 1);

        return new Vector2Int(gridX, gridY);
    }
    public void handleTap()
    {       
        AudioManager.Instance.PlayPop();
        GameObject newObject;
        
        if (gameObject.tag != null && gameObject.tag == "Purple")
        {
            Destroy(gameObject);          
            if (explosionSprite != null)
            {
                GameObject explosion = Instantiate(explosionSprite, transform.position, Quaternion.identity);
                Destroy(explosion, explosionDuration);
            }           
            SpawnProjectiles();
        }
        else if (gameObject.tag != null && gameObject.tag == "Blue")
        {
            Destroy(gameObject);           
            newObject = Instantiate(popperPurple, transform.position, Quaternion.identity);
            GameManager.Instance.AddEyes(newObject);
            GameManager.Instance.spawnedPrefabs.Add(newObject);

        }
        else if (gameObject.tag != null && gameObject.tag == "Yellow")
        {
            Destroy(gameObject);           
            newObject = Instantiate(popperBlue, transform.position, Quaternion.identity);
            GameManager.Instance.AddEyes(newObject);
            GameManager.Instance.spawnedPrefabs.Add(newObject);

        }
        GameManager.Instance.OnPrefabDestroyed(this.GetGridPosition(), gameObject);
    }
    void SpawnProjectiles()
    {
        if (projectileSprite == null) return;

        Vector3[] directions = {
            Vector3.up,    
            Vector3.down,  
            Vector3.right, 
            Vector3.left   
        };

        foreach (Vector3 direction in directions)
        {          
            GameObject projectile = Instantiate(projectileSprite, transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(direction);           
        }
    }
}

