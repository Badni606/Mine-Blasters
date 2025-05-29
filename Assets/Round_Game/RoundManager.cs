using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
    
    List<Element> Elements = new List<Element>();
    List<Weapon> Weapons = new List<Weapon>();
    List<Effect> Effects = new List<Effect>();
    Array pickUps = Enum.GetValues(typeof(PICKUP_TYPE));


    MapGenerator mapGenerator;
    

    //tracking vars
    int sumOfPlayerCount;


    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sumOfPlayerCount = GameManager.instance.playerCount;
        sumOfPlayerCount = (sumOfPlayerCount * (sumOfPlayerCount + 1)) / 2;
        maxPickUpsCount = GameManager.instance.playerCount * pickUpsPerPlayer;


        mapGenerator = GetComponent<MapGenerator>();
        mapGenerator.BeginGeneration();

        encroacher.Init(new Vector2(mapGenerator.width, mapGenerator.height));

        LoadAbilities();
        //BeginRound(); // Now done in loadAbilities()

    }
    void LoadAbilities()
    {
        Addressables.LoadAssetsAsync<Weapon>("Weapons", (weapon)=>Weapons.Add(weapon)).Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                packLoaded();
            }
            else
            {
                Debug.LogError("Failed to load weapons.");
            }
        }; ;
        Addressables.LoadAssetsAsync<Element>("Elements", (element)=>Elements.Add(element)).Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                packLoaded();
            }
            else
            {
                Debug.LogError("Failed to load Elements.");
            }
        };
        Addressables.LoadAssetsAsync<Effect>("Effects", (effect) => Effects.Add(effect)).Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                packLoaded();
            }
            else
            {
                Debug.LogError("Failed to load Effects.");
            }
        };
    }

    int packsLoaded = 0;
    void packLoaded()
    {
        packsLoaded++;
        if(packsLoaded == 3)
        {
            BeginRound();
        }
    }


    public PauseMenu pauseMenu;

    //1. keep track of time and start shrinkning map when at threshold
    public void BeginRound()
    {
        spawnPlayers();
        
        for (int i = 0; i < maxPickUpsCount; i++)
        {
            spawnPickUp();
        }
        //start countdown
        Invoke(nameof(encroach), EncroachStartTime);

        //callbacks
        pauseMenu.Init();
    }

    //------------------------------------- ROUND ---------------------------------------------
    [Header("Encroach parameters")]
    public float EncroachStartTime;
    public Encroacher encroacher;

    void encroach()
    {
        encroacher.BeginEncroach(camera.GetCenterPoint());
    }



    //------------------------------------- PLAYERS ---------------------------------------------
    [Header("Player Paramters")]
    public GameObject playerPrefab;
    public GameObject PlayerCardPrefab;

    [Header("Game UI")]
    public GameObject GameOverUI;
    public RoundCamera camera;
    public Transform ScreenTop;
    public Transform ScreenBottom;


    void spawnPlayers()
    {
        float contatinerWidth = ScreenTop.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        float entryWidth = PlayerCardPrefab.GetComponent<RectTransform>().sizeDelta.x;
        int limit = (int)(contatinerWidth / entryWidth);
        Debug.Log($"Container width: {contatinerWidth}, Entry Width: {entryWidth}, Limit: {limit}");

        for(int i = 0; i < GameManager.instance.playerCount; i++)
        {
            Vector3 position = new Vector3(mapGenerator.spawns[i].x+0.5f, mapGenerator.spawns[i].y+0.5f, 0);
            GameObject player = Instantiate(playerPrefab, position,Quaternion.identity);
            PlayerCard playerCardInstance;
            if(ScreenTop.transform.childCount < limit) 
            {
                playerCardInstance = Instantiate(PlayerCardPrefab, ScreenTop.transform).GetComponent<PlayerCard>();
            }
            else
            {
                playerCardInstance = Instantiate(PlayerCardPrefab, ScreenBottom.transform).GetComponent<PlayerCard>();
            }
            playerCardInstance.Init(Player.HitCooldown, GameManager.instance.playerInfoList[i].colour);
            
            //setup color
            //player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = GameManager.instance.playerInfoList[i].colour;//careful here, might change
            player.GetComponent<SpriteRenderer>().material.SetColor("_TargetColor", GameManager.instance.playerInfoList[i].colour);
            player.GetComponent<Player>().PlayerCard = playerCardInstance;
            //setup controls
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            //playerInput = GameManager.instance.playerInfoList[i].playerInput;
            playerInput.SwitchCurrentControlScheme(GameManager.instance.playerInfoList[i].controlScheme, GameManager.instance.playerInfoList[i].device);
            camera.AddObjectToFollow(player.transform);
        }
    }

    float lastDeathTime = 0f;
    int winnerIndex = -1;

    public TextMeshProUGUI RoundConclusion;

    public void RegisterDeath(int playerIndex)
    {
        float now = Time.time;//in seconds
        sumOfPlayerCount -= playerIndex+1;


        lastDeathTime = now;
        Debug.Log($"Player Index: {playerIndex} registered death");
        IEnumerator check = checkWinCondition(lastDeathTime);
        StartCoroutine(check);
        //makes camera still if all dead
        camera.RemoveObject(playerIndex);
    }

    private IEnumerator checkWinCondition(float timeOfDeath)
    {
        yield return new WaitForSeconds(0.5f);

        if (isGameOver)
        {
            yield return null;
        }

        //Debug.Log($"Contdown check initiated. Sum of player: {sumOfPlayerCount}, timeOfDeath: {timeOfDeath}, lastDeathTime: {lastDeathTime}");
        //no one has died since
        if(lastDeathTime == timeOfDeath)
        {
            if (sumOfPlayerCount == 0)
            {
                RoundConclusion.text = $"Draw!";
                gameOver();
            }
            else if (sumOfPlayerCount <= GameManager.instance.playerCount) //winner
            {
                winnerIndex = sumOfPlayerCount - 1;
                GameManager.instance.SetScore(winnerIndex, GameManager.instance.GetScore(winnerIndex) + 1);
                //Debug.Log($"Winner index: {winnerIndex}");
                RoundConclusion.text = $"Player {winnerIndex + 1} wins!";
                gameOver();
            }
            else
            {
                yield return null;
            }
            if(encroacher != null)
            {
                encroacher.Stop();
            }
        }
        //someone else dies 0.5s since 
        else
        {
            //no players left 
            if (sumOfPlayerCount == 0)
            {
                RoundConclusion.text = $"Draw!";
                gameOver();
            }
        }
    }

    public bool isGameOver = false;
    void gameOver()
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        //display round over screen
        GameOverUI.SetActive(true);
        //switch to intermission screen
        Invoke(nameof(toIntermission), 3f);
    }

    void toIntermission()
    {
        SceneManager.LoadScene("Intermission");
    }



    // -------------------------------------- ABILITIES ------------------------------------------
    [Header("Abilities")]
    public GameObject PickUpGameObject;
    public int pickUpsPerPlayer = 2;
    int maxPickUpsCount = 0;
    int currentPickUpsCount = 0;
    TileBase[] blockedTiles;

    public void PickUpPicked()
    {
        currentPickUpsCount--;
        if(currentPickUpsCount < maxPickUpsCount) 
        {
            //for non intrusive spawning
            Invoke(nameof(spawnPickUp), Random.Range(2, 6));
        }
    }

    void spawnPickUp()
    {
        while (true)
        {
            Vector3Int randomSpot = new Vector3Int(Random.Range(1, mapGenerator.width - 1), Random.Range(1, mapGenerator.height - 1), 0);
            //check for blockign tiles
            TileBase isGround = mapGenerator.groundTileMap.GetTile(randomSpot);
            TileBase isObstacle = mapGenerator.obstacleTileMap.GetTile(randomSpot);

            if (isGround == null || isObstacle != null)
            {
                continue;
            }

            //check for player in vicinity
            Collider2D playerPresent = Physics2D.OverlapCircle(((Vector2Int)randomSpot), 0.6f, LayerMask.GetMask("Players"));
            if(playerPresent != null)
            {
                continue;
            }
            //check no pick up already there
            Collider2D pickUpPresent = Physics2D.OverlapCircle(((Vector2Int)randomSpot), 0.4f, LayerMask.GetMask("PickUps"));
            if (pickUpPresent != null)
            {
                continue;
            }

            Instantiate(PickUpGameObject, randomSpot + new Vector3(0.5f,0.5f,0f), Quaternion.identity);
            currentPickUpsCount++;
            break;
        }
    }



    public float WeaponChance;
    public PickUpObject GeneratePickUp()
    {
        PICKUP_TYPE type = PICKUP_TYPE.EFFECT;
        if (Random.value < WeaponChance)
        {
            type = PICKUP_TYPE.WEAPON;
        }
        //type = PICKUP_TYPE.WEAPON;///remove after testing
        PickUpObject generatedWeapon;
        switch (type)
        {
            case PICKUP_TYPE.WEAPON:
                Weapon w = Weapons[Random.Range(0, Weapons.Count)];
                Element e = Elements[Random.Range(0, Elements.Count)];
                generatedWeapon = new PickUpObject(w, e);
                break;

            case PICKUP_TYPE.EFFECT:
                generatedWeapon = new PickUpObject(Effects[Random.Range(0, Effects.Count)]);
                break;
            default:
                generatedWeapon = null;
                break;
        }
            
        return generatedWeapon;
    }
    
}
