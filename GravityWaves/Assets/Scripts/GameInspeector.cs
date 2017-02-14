using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using XInputDotNetPure;
using System;
using UnityEngine.SceneManagement;

public class GameInspeector : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject Lane;
    public GameObject Drone;
    private List<Player> spawnedPlayers;
    private List<LaneScript> activeLanes = new List<LaneScript>();
    private List<int> spawned = new List<int>();

    private float spawnDuration = 0.6f;
    private float elapsedTime = 0f;
    private int spawnCounter = 10;

    private float spawnInterval = 10f;
    private float elapsedInterval = 10f;

    private bool ShowGameOver = false;
    private float gameOverDuration = 5;
    private float elapsedGameOverTime = 0f;
    private Player winner = null;
    private GUIStyle style; 

    // Use this for initialization
    void Start ()
    {
        GlobalReferences.PlayerStates.Clear();
        GamePadManager.DisconnectAll();
        spawnedPlayers = new List<Player>();
        style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = new Color(255, 114, 0);
        CheckForNewPlayers();
    }

    // Update is called once per frame
	void Update ()
    {
        if (!ShowGameOver)
        {
            Player lastPlayer = null;
            bool gameOver = spawnedPlayers.Count > 1;
            if (spawnedPlayers != null)
            {
                for (int i = 0; i < spawnedPlayers.Count; i++)
                {
                    Player player = spawnedPlayers[i];
                    if (player != null)
                    {
                        if (lastPlayer == null)
                        {
                            lastPlayer = player;
                        }
                        else
                        {
                            gameOver = false;
                            break;
                        }
                    }
                }
            }

            ShowGameOver = gameOver;
            if (gameOver)
            {
                if (lastPlayer != null)
                {
                    winner = lastPlayer;
                    lastPlayer.HealthContainer.MaxHealth = 100000f;
                    lastPlayer.HealthContainer.Heal(100090f);
                    GameObject[] objects = GameObject.FindGameObjectsWithTag("Drone");
                    foreach (GameObject item in objects)
                    {
                        DamageAbleObject damage = item.GetComponent<DamageAbleObject>();
                        if (damage != null)
                        {
                            damage.DoDamage(1000);
                        }
                    }
                }
            }
            else
            {
                elapsedInterval += Time.deltaTime;
                if (elapsedInterval > spawnInterval)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime > spawnDuration)
                    {
                        bool spawnedDrones = false;
                        for (int i = 0; i < activeLanes.Count; i++)
                        {
                            if (spawned[i] <= spawnCounter)
                            {
                                spawned[i]++;
                                spawnedDrones = true;
                                SpawnDrone(activeLanes[i]);
                            }
                        }

                        if(!spawnedDrones)
                        {
                            elapsedInterval = 0f;
                            for (int i = 0; i < spawned.Count; i++)
                            {
                                spawned[i] = 0;
                            }
                            spawnCounter = 5;
                        }

                        elapsedTime = 0f;
                    }
                }
            }
        }
        else
        {
            elapsedGameOverTime += Time.deltaTime;
            if (elapsedGameOverTime > gameOverDuration)
                SceneManager.LoadScene(0);
        }
    }

    private void SpawnPlayers()
    {
        spawnedPlayers = new List<Player>();
        
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            SpawnPlayer(GlobalReferences.PlayerStates[i], new Vector3(i * 1.7f, 3f, 0), 0);
        }
    }

    private void PlayerScript_OnPlayerExit(object sender, PlayerEventArgs e)
    {
        spawnedPlayers.Remove(e.PlayerScript);
        RemovePlayerState(e.PlayerScript.Index);
        GamePadManager.Disconnect(e.PlayerScript.Index);
        
        Destroy(e.PlayerObject);
    }

    private void CheckForNewPlayers()
    {
        PlayerIndex[] freePads = GamePadManager.GetFreeControllers();
        for (int i = 0; i < freePads.Length; i++)
        {
            if (GlobalReferences.PlayerStates.Count < GlobalReferences.PlayerCount)
            {
                if (!IndexInUse(freePads[i]))
                {
                    GamePadState state = GamePad.GetState(freePads[i]);
                    if (state.IsConnected)
                    {
                        PlayerState newPlayerState = new PlayerState(freePads[i], state, true, 0);
                        GlobalReferences.PlayerStates.Add(newPlayerState);
                        GamePadManager.Connect((int)freePads[i]);
                        SpawnPlayer(newPlayerState, new Vector3(0, 2f, (((int)freePads[i] - 1) * 6.25f) - 3), (((int)freePads[i] - 1) * 6.25f) -3);
                    }
                }
            }
        }
    }

    private bool IndexInUse(PlayerIndex index)
    {
        for (int i = 0; i < spawnedPlayers.Count; i++)
        {
            if (spawnedPlayers[i].Index == index)
                return true;
        }

        return false;
    }

    private void SpawnPlayer(PlayerState playerState, Vector3 position, float laneZ)
    {
        GameObject lane = SpawnLane(new Vector3(0, 0, laneZ), (int)playerState.Index);
        DamageAbleObject script = lane.GetComponent<DamageAbleObject>();
        GameObject newPlayer = Instantiate(PlayerPrefab, position, Quaternion.Euler(0, 90, 0), lane.transform);
        Player playerScript = newPlayer.GetComponent<Player>();
        playerScript.HealthContainer = script;
        playerScript.Index = playerState.Index;
        spawnedPlayers.Add(playerScript);
    }

    private GameObject SpawnLane(Vector3 position, int playerIndex)
    {
        GameObject lane = Instantiate(Lane, position, Quaternion.Euler(0, 90, 0), transform);
        LaneScript laneScript = lane.GetComponent<LaneScript>();
        laneScript.Index = playerIndex;
        laneScript.Direction = playerIndex % 2 == 0 ? 1 : -1;
        PortalScript[] portals = lane.GetComponentsInChildren<PortalScript>();
        if(portals.Length == 2)
        {
            if (laneScript.Direction < 0)
            {
                if(portals[0].transform.position.x < portals[1].transform.position.x)
                {
                    portals[0].OutPortal = portals[1].gameObject;
                    portals[0].Index = laneScript.Index;
                    laneScript.EntracePortal = portals[0];
                }
                else
                {
                    portals[1].OutPortal = portals[0].gameObject;
                    portals[1].Index = laneScript.Index;
                    laneScript.EntracePortal = portals[1];
                }
            }
            else if(laneScript.Direction > 0)
            {
                if (portals[0].transform.position.x > portals[1].transform.position.x)
                {
                    portals[0].OutPortal = portals[1].gameObject;
                    portals[0].Index = laneScript.Index;
                    laneScript.EntracePortal = portals[0];
                }
                else
                {
                    portals[1].OutPortal = portals[0].gameObject;
                    portals[1].Index = laneScript.Index;
                    laneScript.EntracePortal = portals[1];
                }
            }
            activeLanes.Add(laneScript);
            spawned.Add(0);
        }
        else
        {
            throw new Exception("Portals missing!");
        }

        return lane;
    }

    private void SpawnDrone(LaneScript laneScript)
    {
        GameObject drone = Instantiate(Drone, laneScript.EntracePortal.OutPortal.transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, -90 * laneScript.Direction, 180));
        Enemy enemy = drone.GetComponent<Enemy>();
        enemy.CurrentLane = laneScript;
    }

    private void RemovePlayerState(PlayerIndex index)
    {
        for (int i = 0; i < GlobalReferences.PlayerStates.Count; i++)
        {
            if (GlobalReferences.PlayerStates[i].Index == index)
                GlobalReferences.PlayerStates.RemoveAt(i);
        }
    }

    private void OnGUI()
    {
        if (ShowGameOver)
        {
            string text = "Draw";
            if (winner != null)
                text = "Player " + ((int)winner.Index + 1).ToString() + " won!";
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width, Screen.height), text, style);
        }
    }
}
