﻿using System.Collections;
using UnityEngine;

public class rollScript : MonoBehaviour
{
    private GameObject GM;
    private GameObject spawnPoint = null;
    public int diceOutput = 0;
    public bool doubleDice = false;
    public bool roll = false;


    public bool getIsDouble() { return doubleDice; }
    public int getRollNo() { return diceOutput; }
    public void setRoll(bool newVal) { roll = newVal; }

    private void Start()
    {
        GM = GameObject.Find("GM");
    }

    private void Update()
    {
        UpdateRoll();

    }

    /// <summary>
    /// Stores the number rolled on the dice, after checking to see if they have stopped rolling
    /// </summary>
    private IEnumerator DiceRoll()
    {
        int diceValue = 0;
        int temp = -1;
        bool first = true;

        while (temp != diceValue)                   //check if the dice value has remained the same for over 1 sec
        {
            diceValue = 0;
            temp = -1;
            foreach (RollingDie d in Dice.allDice)
            {
                diceValue += d.value;
            }
            temp = diceValue;                       //check the dice value once and save to temp
            diceValue = 0;

            if (first)
            {
                yield return new WaitForSeconds(2f);
                first = false;
            }

            foreach (RollingDie d in Dice.allDice)
            {
                diceValue += d.value;               //check the dice value again and save to diceValue
            }
        }
        temp = -1;
        foreach (RollingDie d in Dice.allDice)
        {
            if (d.value == temp)
            {
                doubleDice = true;                  //check for doubles
            }
            temp = d.value;
        }
        diceOutput = diceValue;
        GM.GetComponent<GameManager>().setDoneWait(true);
    }

    /// <summary>
    /// Creates a force to apply to the dice
    /// </summary>
    /// <returns>Returns the force created</returns>
    private Vector3 Force()
    {
        Vector3 rollTarget = new Vector3(3 * Random.Range(-1, 1), 0.1f * Random.Range(-1, 1), 3 * Random.Range(-1, 1));
        return Vector3.Lerp(spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 20);
    }

    /// <summary>
    /// Creates a random colour for the dice
    /// </summary>
    /// <returns>returns this colour</returns>
    string randomColor
    {
        get
        {
            string _color = "blue";
            int c = System.Convert.ToInt32(Random.value * 6);
            switch (c)
            {
                case 0: _color = "red"; break;
                case 1: _color = "green"; break;
                case 2: _color = "blue"; break;
                case 3: _color = "yellow"; break;
                case 4: _color = "white"; break;
                case 5: _color = "black"; break;
            }
            return _color;
        }
    }

    /// <summary>
    /// Checks if a roll has been called, if so it runs a roll
    /// </summary>
    void UpdateRoll()
    {
        spawnPoint = GameObject.Find("diceRoller");
        // check if we have to roll dice
        if (roll)
        {
            roll = false;
            diceOutput = 0;
            doubleDice = false;

            // left mouse button clicked so roll random colored dice 2 of each dieType
            Dice.Clear();

            Dice.Roll("1d6", "d6-" + randomColor, spawnPoint.transform.position, Force());
            Dice.Roll("1d6", "d6-" + randomColor, spawnPoint.transform.position, Force());

            StartCoroutine("DiceRoll");
        }
    }
}
