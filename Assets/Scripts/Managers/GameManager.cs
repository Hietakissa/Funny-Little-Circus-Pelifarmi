using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;

    [SerializeField] Manager[] managers;
    public ItemManager ItemManager => (ItemManager)managers[0];
    public CinematicManager CinematicManager => (CinematicManager)managers[1];
    //public CinematicManager CinematicManager;

    CheckpointSO currentCheckpoint;

    public event Action OnLoadCheckpoint;
    public event Action OnRefreshDoors;

    public Dictionary<Transform, bool> tempStates = new Dictionary<Transform, bool>();
    public Dictionary<Transform, bool> savedStates = new Dictionary<Transform, bool>();

    void Awake()
    {
        Singleton = this;

        foreach (Manager manager in managers) manager.Init();
        foreach (Manager manager in managers) manager.LateInit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) CinematicManager.PlayCinematic(0);
        else if (Input.GetKeyDown(KeyCode.L)) LoadCheckpoint();
    }

    public bool SetCheckpoint(CheckpointSO checkpoint)
    {
        if (currentCheckpoint != null && currentCheckpoint == checkpoint) return false;

        currentCheckpoint = checkpoint;

        foreach (KeyValuePair<Transform, bool> kvp in tempStates)
        {
            savedStates[kvp.Key] = kvp.Value;
        }

        ItemManager.SaveTempInventory();

        return true;
    }

    public void LoadCheckpoint()
    {
        if (currentCheckpoint != null)
        {
            GameData.Player.Movement.Teleport(currentCheckpoint.GetPlayerSpawnPos());

            Vector3 rot = currentCheckpoint.GetPlayerRotation();
            GameData.Player.Camera.SetRotations(rot.x, rot.y);

            foreach (KeyValuePair<Transform, bool> kvp in savedStates)
            {
                tempStates[kvp.Key] = kvp.Value;
            }

            ItemManager.LoadSavedInventory();

            OnLoadCheckpoint?.Invoke();
        }
    }

    public void RefreshDoors()
    {
        OnRefreshDoors?.Invoke();
    }
}

public abstract class Manager : MonoBehaviour
{
    public virtual void Init()
    {

    }
    public virtual void LateInit()
    {

    }
}