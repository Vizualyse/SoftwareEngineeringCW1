using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, TileObject> board;
    private PlayerObject[] players;
    private CardObject[] cards;
    private int currentTurn; //will be used to get player at index currentTurn in the list

    private LinkedList<KeyValuePair<PlayerObject,int>> rollOrderList;
    private int[] rollBuffer;

    private bool doneWait; //used for waiting on Ienumerators
    private int rollType; //either a setup roll, or a normal roll which has a 1st 2nd and 3rd roll (if doubles)

    private rollScript diceManager; // will be used to call the dice roll
    private GMUI uiManager; // will be used to get Inputs, and give outputs

    /// <summary>
    /// Initialises variables
    /// </summary>
    void Start() {
        diceManager = GameObject.Find("diceRoller").GetComponent<rollScript>();
        uiManager = gameObject.GetComponent<GMUI>();
        rollType = 0;
        setupManager();

        rollOrderList = new LinkedList<KeyValuePair<PlayerObject, int>>();
        rollBuffer = new int[3];
    }

    /// <summary>
    /// Will check if the criteria to end the game have been met
    /// </summary>
    /// <returns>Returns true if only one player is left</returns>
    public bool isGameFinished() { //call this whenever a player sets is Out to true?
        int playersStillIn = 0;
        for (int i = 0; i < players.Length; i++) {
            if (!players[i].getIsOut()) {
                playersStillIn++;
            }
        }
        if (playersStillIn == 1) {
            return true; // have another method to show on UI that game is over
        } else {
            return false;
        }
    }

    /// <summary>
    /// Searches for the data files, loads and then sets up the assets
    /// </summary>
    public void setupManager() {
        currentTurn = 0;
        board = new Dictionary<int, TileObject>(); //Set the tileObjects to be of the GameObjects recieved from GMUI?
        for (int i = 1; i < 41; i++) {
            board.Add(i, uiManager.getBoardTile(i - 1).GetComponent<TileObject>());
        }

        //for the card file, auto read and create card object, card data and add to cards -- will make same card for now
        //For now, only 10 cards exist
        cards = new CardObject[10];
        for (int i = 0; i < 10; i++) {
            CardObject card = new CardObject();
            card.setupCard(new CardData("get cash", "Oppertunity Knocks", CardActionEnum.PayBank, "10"));
            cards[i] = card;
        }

        //somehow get inputs - from GMUI
        //inputs: file for board data, file for card data, No. of players, player name, player piece

        //for the board file, auto read and create tile object, tile data and add to board
        TextAsset boardData = Resources.Load<TextAsset>("Excels/Board");
        TextAsset rentData = Resources.Load<TextAsset>("Excels/Rent");

        Dictionary<int, Dictionary<RentTypeEnum, int>> rentTypeData = new Dictionary<int, Dictionary<RentTypeEnum, int>>();
        //reading rent data
        string[] data = rentData.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length-1; i++) {
            string[] row = data[i].Split(new char[] { ',' });

            Dictionary<RentTypeEnum, int> propRentData = new Dictionary<RentTypeEnum, int>();
            if (!row[2].Equals("")) {
                propRentData.Add(RentTypeEnum.Unimproved,Int32.Parse(row[2]));
                propRentData.Add(RentTypeEnum.OneHouse, Int32.Parse(row[3]));
                propRentData.Add(RentTypeEnum.TwoHouses, Int32.Parse(row[4]));
                propRentData.Add(RentTypeEnum.ThreeHouses, Int32.Parse(row[5]));
                propRentData.Add(RentTypeEnum.FourHouses, Int32.Parse(row[6]));
                propRentData.Add(RentTypeEnum.OneHotel, Int32.Parse(row[7]));
            }else if (!row[8].Equals("")) {
                propRentData.Add(RentTypeEnum.OneUtility, Int32.Parse(row[8]));
                propRentData.Add(RentTypeEnum.TwoUtilities, Int32.Parse(row[9]));
            }else if (!row[10].Equals("")) {
                propRentData.Add(RentTypeEnum.OneStation, Int32.Parse(row[10]));
                propRentData.Add(RentTypeEnum.TwoStations, Int32.Parse(row[11]));
                propRentData.Add(RentTypeEnum.ThreeStations, Int32.Parse(row[12]));
                propRentData.Add(RentTypeEnum.FourStations, Int32.Parse(row[13]));
            }

            rentTypeData.Add(Int32.Parse(row[0]),propRentData);
        }

        //reading the board data
        data = boardData.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length - 1; i++) {
            string[] row = data[i].Split(new char[] { ',' });

            TileData newData;

            if (row[4].Equals("No")) {//Set up a Non Property Tile data -- What about its UI?
                TileActionEnum action = TileActionEnum.None;

                if (row[3].Equals("CollectGO")) {
                    action = TileActionEnum.CollectGO;
                } else if (row[3].Equals("Go to jail")) {
                    action = TileActionEnum.Jail;
                } else if (row[3].Equals("Take card")) {
                    action = TileActionEnum.PickCard;
                } else if (row[3].Equals("Collect fines")) {
                    row[6] = "0";
                    action = TileActionEnum.CollectFines;
                } else if (row[3].Equals("Pay tax")) {
                    action = TileActionEnum.PayTax;
                }

                newData = new NonPropData(row[6], row[1], Int32.Parse(row[0]), false, action);
            } else { //Set up  a property tile data
                RentTypeEnum current = RentTypeEnum.Unimproved;
                if (rentTypeData[(Int32.Parse(row[0]))].ContainsKey(RentTypeEnum.OneUtility)) {
                    current = RentTypeEnum.OneUtility;
                } else if (rentTypeData[(Int32.Parse(row[0]))].ContainsKey(RentTypeEnum.OneStation)) {
                    current = RentTypeEnum.OneStation;
                }

                newData = new PropertyData(row[2], Int32.Parse(row[5]), rentTypeData[(Int32.Parse(row[0]))], current, row[1], Int32.Parse(row[0]), true, TileActionEnum.Rent);
            }


            board[i].setData(newData);

            //Set the UI for that board tile
            GameObject obj = uiManager.getBoardTile(newData.getPosition() - 1);
            TextMesh[] texts = obj.GetComponentsInChildren<TextMesh>();
            if (newData.getCanBeOwned()) {
                texts[0].text = newData.getName();
                texts[1].text = ((PropertyData)newData).getPurchasePrice().ToString();
            } else if (texts.Length > 0) { //Not a corner tile
                texts[0].text = newData.getName();
                texts[1].text = "";

            }
        }
    }

    /// <summary>
    /// Create the PlayerObjects that will be used in the game
    /// </summary>
    /// <param name="playerData">Array of GameObjects that contain the data input by the user</param>
    public void setUpPlayers(GameObject[] playerData) {
        players = new PlayerObject[playerData.Length]; 
        for (int i = 0; i < players.Length; i++) { 
            PlayerObject player = new PlayerObject(); //this will hold reference to its own UI, use prefabs? one for each piece
            string[] data = playerData[i].GetComponentInChildren<Text>().text.Split(new string[] { " as: " }, StringSplitOptions.None);
            player.setupPlayer(data[0], (PiecesEnum) Enum.Parse(typeof(PiecesEnum), data[1]), 1500,0); 
            players[i] = player;
        }
        uiManager.setCurPlayerText(players[0].getPlayerName());
    }

    /// <summary>
    /// Adds the property to the players list, if they can afford it
    /// </summary>
    /// <param name="player">The player wanting to purchase the property</param>
    public void purchaseProperty(PlayerObject player) {
        if (player.getMoney() >= ((PropertyData)board[player.getPosition()].getData()).getPurchasePrice()) {
            player.decreaseMoney(((PropertyData)board[player.getPosition()].getData()).getPurchasePrice());
            player.addProperty(board[player.getPosition()]);
            board[player.getPosition()].getData().setOwner(player);
        } else {
            //play UI to say you cant afford?
        }
    }

    /// <summary>
    /// Adds the property to the players list, if they can afford it - purchases for current Turn player
    /// </summary>
    public void purchaseProperty() {
        PlayerObject player = players[currentTurn];
        if (player.getMoney() >= ((PropertyData)board[player.getPosition()].getData()).getPurchasePrice()) {
            player.decreaseMoney(((PropertyData)board[player.getPosition()].getData()).getPurchasePrice());
            player.addProperty(board[player.getPosition()]);
            board[player.getPosition()].getData().setOwner(player);
        } else {
            //play UI to say you cant afford?
        }
    }

    /// <summary>
    /// Call to roll the dice, and wait to check the numbers rolled
    /// </summary>
    public void rollDice() { 
        diceManager.setRoll(true);
        uiManager.setOff(uiManager.getEndTurnBtn()); //make it so the player can not potentially glitch the system
        uiManager.setOff(uiManager.getRollBtn());

        StartCoroutine(waitDiceRolls());
    }

    /// <summary>
    /// Checks the numbers rolled by the player, moves or jails them
    /// </summary>
    private void checkRoll() {

        if (rollBuffer[2] != 0 && diceManager.getIsDouble()) { //jailed
            foreach (TileObject tile in board.Values) { //have a var set as the jail tile?
                if (tile.getData().getTileAction() == TileActionEnum.Jail) {
                    //    sent to jail -- for now setPosition();
                    players[currentTurn].setPosition(tile.getData().getPosition());
                    //call UI movements etc.
                    break;
                }
            }
        } else { //legal roll
            players[currentTurn].increasePosition(rollBuffer[0] + rollBuffer[1] + rollBuffer[2]);
        }
        for (int i = 0; i < rollBuffer.Length; i++) { rollBuffer[i] = 0; }
        if (rollType > 0) { runTileAction(); }
    }

    /// <summary>
    /// Waits for the dice to stop rolling, and then either re-rolls or checks the roll
    /// </summary>
    private IEnumerator waitDiceRolls() {
        
        //have method in GMUI to disable and enable roll & endTurn specifically and add them here
        doneWait = false;
        yield return new WaitUntil(() => doneWait == true);
        switch (rollType) {
            case 0:
                bool added = false;
                LinkedListNode<KeyValuePair<PlayerObject, int>> node = rollOrderList.First;
                while (!added) {
                    if (rollOrderList.Count == 0) {
                        rollOrderList.AddFirst(new KeyValuePair<PlayerObject, int>(players[currentTurn], diceManager.getRollNo()));
                        added = true;
                    } else if (node.Value.Value <= diceManager.getRollNo()) {
                        rollOrderList.AddBefore(node, new KeyValuePair<PlayerObject, int>(players[currentTurn], diceManager.getRollNo()));
                        added = true;
                    } else if (node == rollOrderList.Last) {
                        rollOrderList.AddAfter(node, new KeyValuePair<PlayerObject, int>(players[currentTurn], diceManager.getRollNo()));
                        added = true;
                    } else {
                        node = node.Next;
                    }
                }
                uiManager.setOn(uiManager.getEndTurnBtn());
                break;
            case 1:
                rollBuffer[0] = diceManager.getRollNo();
                if (diceManager.getIsDouble()) {
                    rollType = 2;
                    uiManager.setOff(uiManager.getEndTurnBtn());
                    uiManager.setOn(uiManager.getRollBtn());
                } else {
                    rollType = 1;
                    checkRoll();
                }
                break;
            case 2:
                rollBuffer[1] = diceManager.getRollNo();
                if (diceManager.getIsDouble()) {
                    uiManager.setOff(uiManager.getEndTurnBtn());
                    uiManager.setOn(uiManager.getRollBtn());
                    rollType = 3;
                } else {
                    rollType = 1;
                    checkRoll();
                }
                break;
            case 3:
                rollBuffer[2] = diceManager.getRollNo();
                rollType = 1;
                checkRoll();
                break;
        }
    }

    
    /// <summary>
    /// After having all the players do initial dice rolls, sorts the array in terms of number rolled (lowest -> highest)
    /// </summary>
    public void setPlayOrder() {

        //have every player roll a dice, compare the rolls to anyone already rolled
        //adds them in the correct position in the new temp list

        //add the players in the new order
        int count = rollOrderList.Count;
        for (int i = 0; i < count; i++) {
            players[i] = rollOrderList.First.Value.Key;
            rollOrderList.RemoveFirst();
        }
        uiManager.setUpInfoTabs(players.Length);
        uiManager.setOn(uiManager.getPlayerInfoBtn());
    }

    /// <summary>
    /// At the end of a turn, moves to the next player in the array
    /// </summary>
    public void nextPlayerTurn() {
        //check if currentTurn was last in players, if so set
        if (currentTurn == players.Length - 1) {
            rollType = 1;
            setPlayOrder();
        }

        currentTurn = (currentTurn + 1) % players.Length;
        while (players[currentTurn].getIsOut()) {
            currentTurn = (currentTurn + 1) % players.Length;
        }

        uiManager.setCurPlayerText(players[currentTurn].getPlayerName());
    }

    /// <summary>
    /// Will check the tile landed on, and runs the relevant action
    /// </summary>
    public void runTileAction() {
        PlayerObject player = players[currentTurn];

        uiManager.setOn(uiManager.getEndTurnBtn());
        switch (board[player.getPosition()].getData().getTileAction()) {
            case TileActionEnum.CollectGO:
            case TileActionEnum.CollectFines:
                //player.increaseMoney(Int32.Parse(((NonPropData)board[player.getPosition()].getData()).getTileActionValue()));
                break;
            case TileActionEnum.PayTax:
                //parkingPayment(player, Int32.Parse(((NonPropData)board[player.getPosition()].getData()).getTileActionValue()));
                break;
            case TileActionEnum.PickCard:
                //playCardAction(player);
                break;
            case TileActionEnum.Rent:
                //check if is owned, and if is then call playerpayment, and check if can afford in there?
                if (board[player.getPosition()].getData().getOwner() != null) {
                    playerPayment(player, board[player.getPosition()].getData().getOwner(), ((PropertyData)board[player.getPosition()].getData()).getCurrentRentPrice());
                } else {
                    uiManager.setupPurchaseUI((PropertyData)board[player.getPosition()].getData());
                    uiManager.setOn(uiManager.getPurchaseUIObj());
                    uiManager.setOff(uiManager.getEndTurnBtn());
                }
                break;
            case TileActionEnum.Jail: //have someway of tracking turns spent in jail etc.
                break;
        }
    }

    /// <summary>
    /// Picks a card for the player and runs it's action
    /// </summary>
    /// <param name="player">Current player</param>
    public void playCardAction(PlayerObject player) {
        int cardNo = UnityEngine.Random.Range(0, cards.Length);
        CardObject card = cards[cardNo];
        //play UI
        switch (card.getData().getActionType()) {
            case CardActionEnum.GoToJail:
                foreach (TileObject tile in board.Values) {
                    if (tile.getData().getTileAction() == TileActionEnum.Jail) {
                        player.setPosition(tile.getData().getPosition());
                        //call UI movements etc.
                        break; 
                    }
                }
                break;
            case CardActionEnum.GetOutOfJail:
                break; //for now nothing as one cannot be jailed
            case CardActionEnum.MoveForwards: //there may need to be more Enum types to differentiate the type of movement e.g. nearest vs set dest.
                break;
            case CardActionEnum.MoveBackwards:
                break;//same as forward, need more specifications?
            case CardActionEnum.PayBank:
                bankPayment(player, Int32.Parse(card.getData().getCardActionValue())); //& play relevant UI
                break;
            case CardActionEnum.PayParking:
                parkingPayment(player, Int32.Parse(card.getData().getCardActionValue())); //& play relevant UI
                break;
            case CardActionEnum.PlayersPay: //Only other players paying this player
                int i = 0;
                while (i < players.Length) {
                    if (player != players[i]) {
                        playerPayment(player, players[i], Int32.Parse(card.getData().getCardActionValue()));
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Make a payment between 2 players
    /// </summary>
    /// <param name="from">The player making the payment</param>
    /// <param name="to"> The player recieving the payment</param>
    /// <param name="amount">The amount being paid (can be pos or neg)</param>
    public void playerPayment(PlayerObject from, PlayerObject to, int amount) {
        if (from.getMoney() >= amount) {
            from.decreaseMoney(amount);
            to.increaseMoney(amount);
        } else {
            from.setIsOut(true);
        }
    }

    /// <summary>
    /// A payment made between a player and the bank
    /// </summary>
    /// <param name="player">The player involved in the transaction</param>
    /// <param name="amount">The amount being paid (can be pos or neg)</param>
    public void bankPayment(PlayerObject player, int amount) {
        if (amount > 0) {
            player.increaseMoney(amount);
        } else {
            if (player.getMoney() > amount) {
                player.decreaseMoney(amount);
            } else {
                player.setIsOut(true);
            }
        }
    }

    /// <summary>
    /// A payment that adds money to the parking space
    /// </summary>
    /// <param name="player">The player involved in the transaction</param>
    /// <param name="amount">The amount being paid (can be pos or neg)</param>
    public void parkingPayment(PlayerObject player, int amount) {
        //check if player can afford
        if (player.getMoney() > amount) {
            player.decreaseMoney(amount);
        } else {
            player.setIsOut(true);
        }


        //find the parking space - or the first one
        foreach (TileObject tile in board.Values) {
            if (tile.getData().getTileAction() == TileActionEnum.CollectFines) {
                Debug.Log(((NonPropData)tile.getData()).getTileActionValue());
                //get the current fines on the space and increase it
                ((NonPropData)tile.getData()).setTileActionValue((Int32.Parse(((NonPropData)tile.getData()).getTileActionValue())+amount).ToString());
            }
        }
    }

    /// <summary>
    /// Used to let GM know that auxillary code has finished running e.g. dice roll
    /// </summary>
    /// <param name="val">New bool value for doneWait</param>
    public void setDoneWait(bool val) { doneWait = val; }

    /// <summary>
    /// Return the current value of doneWait
    /// </summary>
    /// <returns>Returns the current value of doneWait</returns>
    public bool getDoneWait() { return doneWait; }

    /// <summary>
    /// Returns the players
    /// </summary>
    /// <returns>Returns the players</returns>
    public PlayerObject[] getPlayers() { return players; }
}
