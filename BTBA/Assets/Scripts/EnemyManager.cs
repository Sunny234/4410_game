using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemyParents; // We will use this to keep track of "alive" and "dead" enemies 
    public GameObject enemiesDefeatedText; // Message for "All Enemies defeated!"
    public GameObject enemyCountTextParent; // Parent to allow us disabling / enabling enemy count text
    public TextMeshProUGUI enemyCountText; // Actual text so we can update this count message
    private int totalEnemies;
    private int remainingEnemies;
    private GameObject activeEnemyGroup;

    // Booleans to help us control when to trigger events
    public bool allEnemiesDefeated = false;
    public bool enemiesSpawned = false;
    public bool enemyKilled = false;
    public bool playerVictory = false;
    public int groupNumber;

    private void Update()
    {
        if(enemiesSpawned)
            SetEnemyCountText();
            allEnemiesDefeated = !(EnemiesRemain()); // If enemies remain, all enemies not defeated
        
        if(enemiesSpawned && allEnemiesDefeated)
            PlayerVictory();
    }

    // Function so other scripts can spawn in the groups of enemies by their tag
    public void SpawnEnemyGroup(string groupTag)
    {
        // Go through children of messageParent until we find the one with the matching tag 
        for(int i = 0; i < enemyParents.Length; i++)
        {
            if (enemyParents[i].tag == groupTag)
            {
                groupNumber = i;
                activeEnemyGroup = enemyParents[i];
                enemyParents[i].SetActive(true);
                allEnemiesDefeated = false;
                enemiesSpawned = true;
                playerVictory = false;
                totalEnemies = enemyParents[i].transform.childCount;
                remainingEnemies = totalEnemies;
                SetEnemyCountText();
                EnableCountParent(true);
            }
        }
    }

    // Function so other scripts can check if this trigger tag belongs to an enemy group
    public bool IsEnemyGroupTrigger(GameObject other)
    {
        for (int i = 0; i < enemyParents.Length; i++)
        {
            if (enemyParents[i].tag == other.tag)
                return true;
        }
        
        return false;
    }

    // Function to enable / disable text parent
    private void EnableCountParent(bool active)
    {
        if (active)
            enemyCountTextParent.SetActive(true);
        else
            enemyCountTextParent.SetActive(false);
    }

    // Function to check if all enemies are dead
    private bool EnemiesRemain()
    {
        if (enemiesSpawned)
        {
            for (int i = 0; i < activeEnemyGroup.transform.childCount; i++)
            {
                // If we find one child (enemy) of this enemy group who is active, return true
                if (activeEnemyGroup.transform.GetChild(i).gameObject.activeSelf)
                    return true;
            }

            return false;
        }
        else
            return false;
    }

    // Function to update text on how many enemies are remaining
    private void SetEnemyCountText()
    {
        UpdateEnemyCount();
        enemyCountText.text = "Enemies remaining: " + remainingEnemies.ToString() + " / " + totalEnemies.ToString();
    }

    // Function to handle end of enemy round
    private void PlayerVictory()
    {
        allEnemiesDefeated = false;
        enemiesSpawned = false;
        SetEnemiesDefeatedText(true);
        Invoke(nameof(DisableText), 2.0f);
    }

    // Function to enable to "All enemies defeated" text
    private void SetEnemiesDefeatedText(bool active)
    {
        if (active)
            enemiesDefeatedText.SetActive(true);
        else   
            enemiesDefeatedText.SetActive(false);
    }

    // Function to update how many enemies are remaining
    private void UpdateEnemyCount()
    {
        int tempNum = totalEnemies;
        for (int i = 0; i < activeEnemyGroup.transform.childCount; i++)
        {
            // If we find one child (enemy) of this enemy group who is active, return true
            if (!(activeEnemyGroup.transform.GetChild(i).gameObject.activeSelf))
                tempNum -= 1;
        }

        remainingEnemies = tempNum;
    }

    // Function to disable text, useful because we can use "Invoke" with it
    private void DisableText()
    {
        SetEnemiesDefeatedText(false);
        EnableCountParent(false);
        playerVictory = true;
    }
}
