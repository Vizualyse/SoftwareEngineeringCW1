using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GMUI : MonoBehaviour{

    //GameManager object
    private GameManager GM;

    //InGameUI
    public GameObject[] boardTiles;

    public Text curPlayerText;

    public GameObject purchasePropUI;

    //PlayerInfo UI
    public GameObject[] tabs;
    public GameObject infoPrefab;
    public Text contentPosText;
    public Text contentMoneyText;
    public GameObject contentPropContainer;
    public GameObject playerInfoBtn;

    public GameObject rollBtn;
    public GameObject endTurnBtn;

    //PlayerSelectUI
    public GameObject[] addedPlayerUI = new GameObject[6];
    public GameObject addPlayerUI;

    //Board&CardsUI
    //public GameObject[] dataInpUI = new GameObject[2]; //For now, only Board and rent is read?

    //Camera Positions
    public Transform aboveView;

    /// <summary>
    /// Initialise variables
    /// </summary>
    void Start() {
        GM = gameObject.GetComponent<GameManager>();
        setDropdownOptions();
    }

    /// <summary>
    /// Loads the appropriate players info
    /// </summary>
    /// <param name="index">Index of this tab</param>
    public void showPlayerInfo(int index) {

        PlayerObject[] players = GM.getPlayers();
        PlayerObject player = GM.getPlayers()[index];

        contentMoneyText.text = "Money: " + player.getMoney();
        contentPosText.text = "Position: " + player.getPosition();

        foreach (Transform obj in contentPropContainer.GetComponentsInChildren<Transform>()) {
            if (obj.gameObject != contentPropContainer) {
                Destroy(obj.gameObject);
            }
        }

        foreach (TileObject tile in player.getPropertiesOwned()) {
            PropertyData data = (PropertyData)tile.getData();
            GameObject propText = Instantiate(infoPrefab,contentPropContainer.transform);
            foreach(Text text in propText.GetComponentsInChildren<Text>()) {
                if (text.name == "Name") {
                    text.text = "Property: " + data.getName();
                } else{
                    text.text = "Current Rent: " + data.getCurrentRentPrice();
                }
            }
        }
    }

    /// <summary>
    /// Sets up the details of the property to show the user
    /// </summary>
    /// <param name="data">The data of the property</param>
    public void setupPurchaseUI(PropertyData data) {
        foreach (Text text in purchasePropUI.GetComponentsInChildren<Text>()) {
            if (text.name == "Name") {
                text.text = data.getName();
            } else if (text.name == "Details") {
                text.text = "Price: " + data.getPurchasePrice() + "\nBase Rent: " + data.getCurrentRentPrice();
            }
        }
    }

    /// <summary>
    /// Will set up the correct number of info tabs
    /// </summary>
    /// <param name="size">the number of players</param>
    public void setUpInfoTabs(int size) {
        PlayerObject[] players = GM.getPlayers();
        for (int i = 0; i < tabs.Length; i++) {
            if (i >= size) {
                tabs[i].SetActive(false);
            } else {
                tabs[i].GetComponentInChildren<Text>().text = players[i].getPlayerName();
            }
        }
    }

    /// <summary>
    /// Send the player data to the GM and call it to setUp Players
    /// </summary>
    public void submitPlayers() {
        //save the data of added players, and change panel -- send the data to GM (call setupPlayers)
        int active = 0;
        for (int i = 0; i < addedPlayerUI.Length;i++) {
            if (addedPlayerUI[i].activeSelf != false) {
                active++;
            }
        }

        GameObject[] temp = new GameObject[active];
        active = 0;
        for (int j = 0; j < addedPlayerUI.Length; j++) { 
            if (addedPlayerUI[j].activeSelf != false) {
                temp[active] = addedPlayerUI[j];
                active++;
            }
        }

        GM.setUpPlayers(temp);
    }

    /// <summary>
    /// Move the camera to the birds eye view
    /// </summary>
    public void setCameraToAbove() { Camera.main.transform.position = aboveView.position; }

    /// <summary>
    /// Make a call to roll the dice
    /// </summary>
    public void rollDiceButton() {GM.rollDice(); setCameraToAbove(); }

    /// <summary>
    /// Moves the camera to a new position
    /// </summary>
    /// <param name="newPos">The Transform component position to move the camera to</param>
    public void moveCamera(Transform newPos) { Camera.main.transform.position = newPos.position; }

    /// <summary>
    /// Make a call to start the next turn
    /// </summary>
    public void endTurn() {GM.nextPlayerTurn();}

    /// <summary>
    /// Update who the current player is
    /// </summary>
    /// <param name="name">The name of the current player</param>
    public void setCurPlayerText(string name) { curPlayerText.text = "Current Player: " + name; }

    /// <summary>
    /// Removes a playerData from being added
    /// </summary>
    /// <param name="ui">The playerData to ignore</param>
    public void removePlayerUI(GameObject ui) {
        ui.SetActive(false);
        //split the text display to retrieve what the selected piece was
        addPlayerUI.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData() { text = ui.GetComponentInChildren<Text>().text.Split(new string[] { " as: " }, StringSplitOptions.None)[1] });
    }

    /// <summary>
    /// Uses the data input to add a ui showing that this has been read
    /// </summary>
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

    /// <summary>
    /// Set the options of the pieces a player can choose to those in the PiecesTypeEnum class
    /// </summary>
    private void setDropdownOptions() { //make it get from an array, from which things get removed and added
        
        addPlayerUI.GetComponentInChildren<Dropdown>().options.Clear();
        foreach (String piece in System.Enum.GetNames(typeof(PiecesEnum))) {
            addPlayerUI.GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData() { text = piece });
        }
        
    }

    public void setOff(GameObject setOff) { setOff.SetActive(false);}

    public void setOn(GameObject setOn) { setOn.SetActive(true);}

    /// <summary>
    /// Toggles the active state of a GameObject
    /// </summary>
    /// <param name="obj">Toggles the active state of this GameObject</param>
    public void toggleActive(GameObject obj) { obj.SetActive(!obj.activeSelf); }

    /// <summary>
    /// Returns the specific GameObject of a tile from the board
    /// </summary>
    /// <param name="index">The index of the tile in the board (tile number - 1)</param>
    /// <returns>Returns the GameObject of the tile</returns>
    public GameObject getBoardTile(int index) { return boardTiles[index]; }

    public GameObject getRollBtn() { return rollBtn; }

    public GameObject getEndTurnBtn() { return endTurnBtn; }

    public GameObject getPurchaseUIObj() { return purchasePropUI; }

    public GameObject getPlayerInfoBtn() { return playerInfoBtn; }
}
