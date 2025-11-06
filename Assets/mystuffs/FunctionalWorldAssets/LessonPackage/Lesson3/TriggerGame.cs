using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGame : MonoBehaviour
{
    int i = 0;
    bool playerTouchedFirst = false;

    void Start() {
        SetI(3); // initialize i
    }

    void SetI(int value) {
        i = value;
        Debug.Log("SetI called → i is now " + i);
    }

    void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            // Player increases i by 2 if first touch, else by 1
            if (!playerTouchedFirst) {
                playerTouchedFirst = true;
                i += 2;
                Debug.Log("Player touched first → i increased by 2 → i = " + i);
            } else {
                i += 1;
                Debug.Log("Player touched again → i increased by 1 → i = " + i);
            }
        } 
        else if (other.tag == "Yeti") {
            // Yeti runs a loop, increments i 5 times
            for (int j = 0; j < 5; j++) {
                i++;
            }
            Debug.Log("Yeti did the loop → i increased by 5 → i = " + i);
        }
        else if (other.tag == "Wizard") {
            // Wizard calls SetI() to set i
            SetI(10);
            Debug.Log("Wizard touched causing a spell to reset i to 10");
        }
        else if (other.tag == "Pet") {
            // Pet announces current i
            Debug.Log("Pet says: i is currently " + i);
        }
        else {
            // Unknown objects increment i by 1
            i++;
            Debug.Log("Something weird touched me → i increased by 1 → i = " + i);
        }
    }
}