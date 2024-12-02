using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject purplePrefab;
    public GameObject bluePrefab;
    public GameObject yellowPrefab;

    [Header("Grid Settings")]
    public int gridRows = 5;
    public int gridColumns = 5;
    public float cellSize = 1f;

    [Header("UI Elements")]
    public TMP_Text levelText;
    public TMP_Text tapCountText;
    public Button retryButton;

    [Header("Decorations")]
    public GameObject eyesPrefab;

    private bool[,] gridOccupancy;

    private List<Vector2Int> availablePositions;
    private bool levelupinitiated;
    public List<GameObject> spawnedPrefabs;

    public int currentLevel = 1;
    public int remainingTaps;

    public static GameManager Instance { get; private set; }

  

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        retryButton.onClick.AddListener(RetryLevel);
        retryButton.gameObject.SetActive(false);
        InitializeGame();
    }

    void InitializeGame()
    {     
        retryButton.gameObject.SetActive(false);
        gridOccupancy = new bool[gridRows, gridColumns];
        availablePositions = new List<Vector2Int>();
        spawnedPrefabs = new List<GameObject>();
        
        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns; col++)
            {
                availablePositions.Add(new Vector2Int(row, col));
            }
        }

        SetupLevelParameters();
        SpawnPrefabsForLevel();
        UpdateUI();
    }

    void SetupLevelParameters()
    {      
        int prefabCount = CalculatePrefabCount();
        remainingTaps = prefabCount + 2;
    }

    int CalculatePrefabCount()
    {
        if (currentLevel <= 2) return 3;
        if (currentLevel <= 4) return 4;
        return 5;
    }

    void SpawnPrefabsForLevel()
    {        
        List<GameObject> availablePrefabs = GetAvailablePrefabsForLevel();
        int totalPositions = gridRows * gridColumns;
        int missingPositionsCount = Mathf.FloorToInt(totalPositions * 0.3f);

        availablePositions = availablePositions.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < missingPositionsCount; i++)
        {
            availablePositions.RemoveAt(0); 
        }
        int prefabCount = availablePositions.Count;
        for (int i = 0; i < prefabCount; i++)
        {
            GameObject prefabToSpawn = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            Vector2Int gridPos = availablePositions[i];
            gridOccupancy[gridPos.x, gridPos.y] = true;
            Vector3 spawnPosition = CalculateWorldPosition(gridPos);
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            spawnedPrefab.transform.localScale = new Vector3(2f, 2f, 2f);
            spawnedPrefab.transform.rotation = Quaternion.identity;
            spawnedPrefabs.Add(spawnedPrefab);        
            this.AddEyes(spawnedPrefab);
        }
    }
    public void AddEyes(GameObject popper)
    {
        if (eyesPrefab != null)
        {
            GameObject eyesInstance = Instantiate(eyesPrefab, popper.transform);

            // Get the renderer bounds to calculate height
            Renderer renderer = popper.GetComponent<Renderer>();
            float eyesY = renderer != null ? renderer.bounds.size.y : 1f;
            float eyesX = renderer != null ? renderer.bounds.size.x : 1f;

            // Position eyes just above the center of the popper prefab
            eyesInstance.transform.localPosition = new Vector3(eyesX + 1.15f, eyesY / 2 - 0.61f, 0);
            eyesInstance.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        }
    }
    Vector3 CalculateWorldPosition(Vector2Int gridPos)
    {      
        Camera mainCamera = Camera.main;
        Vector2 gridCenter = mainCamera.transform.position;

        float totalWidth = gridColumns * cellSize;
        float totalHeight = gridRows * cellSize;

        return new Vector3(
            gridCenter.x - (totalWidth / 2) + (gridPos.y * cellSize) + (cellSize / 2),
            gridCenter.y + (totalHeight / 2) - (gridPos.x * cellSize) - (cellSize / 2),
            0
        );
    }

    List<GameObject> GetAvailablePrefabsForLevel()
    {
        List<GameObject> availablePrefabs = new List<GameObject>();

        if (currentLevel <= 2)
        {
            availablePrefabs.Add(purplePrefab);
        }
        else if (currentLevel <= 4)
        {
            availablePrefabs.Add(purplePrefab);
            availablePrefabs.Add(bluePrefab);
        }
        else
        {
            availablePrefabs.Add(purplePrefab);
            availablePrefabs.Add(bluePrefab);
            availablePrefabs.Add(yellowPrefab);
        }

        return availablePrefabs;
    }

    void UpdateUI()
    {
        levelText.text = "Level: " + currentLevel;
        tapCountText.text = "Taps: " + remainingTaps;
        
    }

    public void OnPrefabDestroyed(Vector2Int gridPos, GameObject prefab)
    {
        if (gridPos.x >= 0 && gridPos.x < gridRows && gridPos.y >= 0 && gridPos.y < gridColumns)
        {
            
            gridOccupancy[gridPos.x, gridPos.y] = false;
            availablePositions.Add(gridPos);                     
            UpdateUI();           
        }
        else
        {
            Debug.LogError("Grid position out of bounds: " + gridPos);
        }

        Destroy(prefab);

        spawnedPrefabs.Remove(prefab);

        if (spawnedPrefabs.Count > 0 && remainingTaps <= 0)
        {
            AudioManager.Instance.PlayAwh();
            foreach (var prefabs in spawnedPrefabs)
            {
                Destroy(prefabs);
            }
            spawnedPrefabs.Clear();
            retryButton.gameObject.SetActive(true);
            
        }
        else if (spawnedPrefabs.Count == 0 && !levelupinitiated )
        {
            DestroyAllProjectiles();
            levelupinitiated = true;
            AudioManager.Instance.PlayApplause();
            
            StartCoroutine(DelayLevelUp());
        }
    }

    public void RetryLevel()
    {
        retryButton.gameObject.SetActive(false);
        InitializeGame();
    }

    void DestroyAllProjectiles()
    {      
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");       
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile);
        }
    }
    private IEnumerator DelayLevelUp()
    {
        
        yield return new WaitForSeconds(3f);
        levelupinitiated = false;
        currentLevel++;
        InitializeGame();
    }
}
