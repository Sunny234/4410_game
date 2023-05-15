using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // For each teleporter there is a corresponding teleport point
    // Ex: teleporter #0 will teleport the player to Teleport Point #0
    public GameObject[] teleporterList;
    public GameObject[] teleportPoints;
    public GameObject[] triggerObjects;
    private bool lobbyRound1Complete, lobbyRound2Complete;

    // Function for blocking player off from leaving tutorial fight
    public void EnableTutorialFightWall()
    {
        triggerObjects[1].SetActive(true);
    }

    // Function for disabling arrow of first enemy trigger box in lobby
    public void DisableLobbyArrow1()
    {
        triggerObjects[2].SetActive(false);
    }

    // Function for disabling arrow of second enemy trigger box in lobby
    public void DisableLobbyArrow2()
    {
        triggerObjects[3].SetActive(false);
    }

    // Function for event handler to take care of events following a round win
    public void HandleRoundWin(int groupNumber)
    {
        // Player defeated all enemies in tutorial level
        if (groupNumber == 0)
        {
            // Deactivate the orange walls in the tutorial level so player can leave
            triggerObjects[0].SetActive(false);
            triggerObjects[1].SetActive(false);
        }
        // Player defeated all enemies of part 1 in Lobby level
        else if (groupNumber == 1)
        {
            lobbyRound1Complete = true;
            CheckLobbyRoundsComplete();
        }
        // Player defeated all enemies of part 2 in Lobby level
        else if (groupNumber == 2)
        {
            lobbyRound2Complete = true;
            CheckLobbyRoundsComplete();
        }
        
    }

    // Function to check if both lobby rounds have been complete
    private void CheckLobbyRoundsComplete()
    {
        if (lobbyRound1Complete && lobbyRound2Complete)
        {
            teleporterList[1].SetActive(true);
            triggerObjects[4].SetActive(true);
        }
    }

    // Function for checking if current object is a teleporter
    public bool IsTeleporter(GameObject other)
    {
        for(int i = 0; i < teleporterList.Length; i++)
        {
            if (teleporterList[i].tag == other.tag)
                return true;
            
        }
        return false;
    }

    // Function so other scripts can receive location of teleport
    public Vector3 GetTelportLocation(GameObject other)
    {
        for(int i = 0; i < teleporterList.Length; i++)
        {
            if (teleporterList[i].tag == other.tag)
                return teleportPoints[i].transform.position;
        }
        
        return other.transform.position;
    }

    // Function so other scripts can receive rotation of teleport
    public Quaternion GetTelportRotation(GameObject other)
    {
        for(int i = 0; i < teleporterList.Length; i++)
        {
            if (teleporterList[i].tag == other.tag)
                return teleportPoints[i].transform.rotation;
        }
        
        return other.transform.rotation;
    }

}
