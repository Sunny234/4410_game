using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    // Variables for storing messages and their titles
    public GameObject messageParent;

    // Function so other scripts can check if a trigger is a tutorial message
    public bool IsTutorialMessageTrigger(GameObject other)
    {
        for (int i = 0; i < messageParent.transform.childCount; i++)
        {
            if (messageParent.transform.GetChild(i).gameObject.tag == other.tag)
                return true;
        }
        
        return false;
    }

    // Function so other scripts can display a message on screen
    public void DisplayMessage(string messageTag, bool display)
    {
        // If 'display' true, we'll be setting the message to active
        if(display)
        {
            // Go through children of messageParent until we find the one with the matching tag 
            for(int i = 0; i < messageParent.transform.childCount; i++)
            {
                if (messageParent.transform.GetChild(i).gameObject.tag == messageTag)
                    messageParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        // Otherwise, we'll be setting the message to inactive
        else
        {
            // Go through children of messageParent until we find the one with the matching tag
            for(int i = 0; i < messageParent.transform.childCount; i++)
            {
                if (messageParent.transform.GetChild(i).gameObject.tag == messageTag)
                    messageParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
