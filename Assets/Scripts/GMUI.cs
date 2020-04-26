using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GMUI : MonoBehaviour{

    //GameManager object
    private GameManager GM;

    //InGameUI
    public GameObject[] bottomUI;
    public GameObject[] boardTiles;

    public Text curPlayerText;

    public GameObject[] playerInfoObjs;

    public GameObject rollBtn;
    public GameObject endTurnBtn;

    //PlayerSelectUI
    public GameObject[] addedPlayerUI = new GameObject[6];
    public GameObject addPlayerUI;

    //Board&CardsUI
    //public GameObject[] dataInpUI = new GameObject[2]; //For now, only Board and rent is read?

    //Camera Positions
    public Transform aboveView;

    void Start() {
        GM = gameObject.GetComponent<GameManager>();
        setDropdownOptions();
    }
    
    public void submitPlayers() {
        //save the data of added players, and change panel -- send the data to GM (call setupPlayers)
        //start the game -- diceRoll? call at end of setupPlayers 
        int i = 0;
        while (addedPlayerUI[i].activeSelf != false && i < addedPlayerUI.Length) {
            i++;
        }
        GameObject[] temp = new GameObject[i];
        for(int j = 0; j < i; j++) {
            temp[j] = addedPlayerUI[j];
        }

        //Set playerInfos to inactive
        for (int j = i; j < playerInfoObjs.Length; j++) {
            playerInfoObjs[j].SetActive(false);
        }

        GM.setUpPlayers(temp);
    }

    public void setCameraToAbove() { Camera.main.transform.position = aboveView.position; }

    public void rollDiceButton() {GM.rollDice(); setCameraToAbove(); }

    public void moveCamera(Transform newPos) { Camera.main.transform.position = newPos.position; }

    public void endTurn() {GM.nextPlayerTurn();}

    public void setCurPlayerText(string name) { curPlayerText.text = "Current Player: " + name; }

    public void updatePlayerInfo(PlayerObject player, int index) {
        foreach (Text obj in playerInfoObjs[index].GetComponentsInChildren<Text>()) {
            if (obj.name == "MoneyText") { obj.text = "Money: £" + player.getMoney(); }
            else if(obj.name == "PositionText") { obj.text = "Position: " + player.getPosition(); }
            else if (obj.name == "NameText") { obj.text = "Name: " + player.getPlayerName(); }
        }
    }

    public void removePlayerUI(GameObject ui) {
        ui.SetActive(false);
        //split the text display to retrieve what the selected piece was
        addPlayerUI.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData() { text = ui.GetComponentInChildren<Text>().text.Split(new string[] { " as: " }, StringSplitOptions.None)[1] });
    }

    public void MakeAddPlayerUI() {
        bool added = false;
        int i = 0;
        Dropdown piece = addPlayerUI.GetComponentInChildren<Dropdown>();
        InputField inp = addPlayerUI.GetComponentInChildren<InputField>();
        while (i < addedPlayerUI.Length && !added) {
            if (addedPlayerUI[i].activeSelf == false) {
                addedPlayerUI[i].SetActive(true);
                addedPlayerUI[i].GetComponentInChildren<Text>().text = inp.text + " as: " + piece.options[piece.value].text;
                added = true;
                piece.options.RemoveAt(piece.value);
            }
            i++;
        }
    }

    private void setDropdownOptions() { //make it get from an array, from which things get removed and added
        
        addPlayerUI.GetComponentInChildren<Dropdown>().options.Clear();
        foreach (String piece in System.Enum.GetNames(typeof(PiecesEnum))) {
            addPlayerUI.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData() { text = piece });
        }
        
    }

    public void setOff(GameObject setOff) { setOff.SetActive(false);}
    public void setOn(GameObject setOn) { setOn.SetActive(true);}
    public void toggleActive(GameObject obj) { obj.SetActive(!obj.activeSelf); }
    public GameObject getBoardTile(int index) { return boardTiles[index]; }

    public GameObject getRollBtn() { return rollBtn; }
    public GameObject getEndTurnBtn() { return endTurnBtn; }
}
