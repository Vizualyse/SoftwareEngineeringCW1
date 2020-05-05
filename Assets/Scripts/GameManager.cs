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
    private KeyValuePair<string, LinkedList<CardObject>>[] cards;
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
    /// Will check if the criteria to end the game have been met, if so it will end the game
    /// </summary>
    public IEnumerator isGameFinished() { 
        doneWait = false;
        yield return new WaitUntil(() => doneWait == true);
        int playersStillIn = 0;
        for (int i = 0; i < players.Length; i++) {
            if (!players[i].getIsOut()) {
                playersStillIn++;
            }
        }
        if (playersStillIn == 1) {
            nextPlayerTurn();
            uiManager.setUpGameOverUI(players[currentTurn]);
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

        //Read the cards files
        TextAsset oppertunityData = Resources.Load<TextAsset>("Excels/Opportunity Knocks");
        TextAsset potData = Resources.Load<TextAsset>("Excels/Pot Luck");

        //for the card file, auto read and create card object, card data and add to card
        cards = new KeyValuePair<string, LinkedList<CardObject>>[2]; // there will only be 2 piles

        cards[0] = new KeyValuePair<string, LinkedList<CardObject>>("Oppertunity Knocks", new LinkedList<CardObject>());
        cards[1] = new KeyValuePair<string, LinkedList<CardObject>>("Pot Luck", new LinkedList<CardObject>());

        foreach (TextAsset file in new TextAsset[2]{ oppertunityData,potData}) {
            string[] read = file.text.Split(new char[] { '\n' });
            for (int i = 1; i < read.Length - 1; i++) {
                string[] row = read[i].Split(new char[] { ',' });
                CardObject card = new CardObject();

                CardActionEnum action;

                switch (row[1]) {
                    case "Bank pays player":
                        action = CardActionEnum.BankPays;
                        break;
                    case "Player pays bank":
                        action = CardActionEnum.PayBank;
                        break;
                    case "Player moves forwards":
                        action = CardActionEnum.MoveForwards;
                        break;
                    case "Player moves backwards":
                        action = CardActionEnum.MoveBackwards;
                        break;
                    case "Player moves forwards Collect GO":
                        action = CardActionEnum.MoveForwardsGO;
                        break;
                    case "Player puts on free parking":
                        action = CardActionEnum.PayParking;
                        break;
                    case "Go to jail":
                        action = CardActionEnum.GoToJail;
                        break;
                    case "Get out of jail":
                        action = CardActionEnum.GetOutOfJail;
                        break;
                    case "Player moves to GO":
                        action = CardActionEnum.MoveForwardsGO;
                        break;
                    case "Player receives from each player":
                        action = CardActionEnum.PlayersPay;
                        break;
                    default:
                        action = CardActionEnum.PayBank;
                        row[2] = "10";
                        break;
                }

                // the fine paid or pick card will be problematic
                if (file == oppertunityData) {
                    card.setupCard(new CardData(row[0], "Oppertunity Knocks", action, row[2]));
                    cards[0].Value.AddFirst(card);
                } else {
                    card.setupCard(new CardData(row[0], "Pot Luck", action, row[2]));
                    cards[1].Value.AddFirst(card);
                }
            }
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
            if (!row[2].Equals("")) {//Houses
                propRentData.Add(RentTypeEnum.Unimproved,Int32.Parse(row[2]));
                propRentData.Add(RentTypeEnum.OneHouse, Int32.Parse(row[3]));
                propRentData.Add(RentTypeEnum.TwoHouses, Int32.Parse(row[4]));
                propRentData.Add(RentTypeEnum.ThreeHouses, Int32.Parse(row[5]));
                propRentData.Add(RentTypeEnum.FourHouses, Int32.Parse(row[6]));
                propRentData.Add(RentTypeEnum.OneHotel, Int32.Parse(row[7]));
            }else if (!row[8].Equals("")) {//Utilities
                propRentData.Add(RentTypeEnum.OneUtility, Int32.Parse(row[8]));
                propRentData.Add(RentTypeEnum.TwoUtilities, Int32.Parse(row[9]));
            }else if (!row[10].Equals("")) {//Stations
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
                texts[1].text = ((PropertyData)newData).getPurchasePrice().ToString();///move to GMUI?
                foreach (MeshRenderer render in obj.GetComponentsInChildren<MeshRenderer>()) { 
                    if (render.name=="Cube (1)") {
                        render.material = new Material(Shader.Find("Custom/NewSurfaceShader"));
                        switch (((PropertyData)newData).getColour()) {
                            case "Yellow":
                                render.material.SetColor("_Color", Color.yellow);
                                break;
                            case "Purple":
                                render.material.SetColor("_Color", new Color(0.56f, 0f, 0.99f, 1)); //PURPLE
                                break;
                            case "Brown":
                                render.material.SetColor("_Color", new Color(0.52f, 0.26f, 0.06f, 1)); //BROWN
                                break;
                            case "Blue":
                                render.material.SetColor("_Color", Color.cyan);
                                break;
                            case "Orange":
                                render.material.SetColor("_Color", new Color(0.99f, 0.63f, 0f, 1)); //ORANGE
                                break;
                            case "Red":
                                render.material.SetColor("_Color", Color.red);
                                break;
                            case "Green":
                                render.material.SetColor("_Color", Color.green);
                                break;
                            case "Deep blue":
                                render.material.SetColor("_Color", Color.blue);
                                break;
                        }
                    }
                }
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
        if (rollBuffer[2] != 0 && diceManager.getIsDouble()) { //sent to jail
            foreach (TileObject tile in board.Values) { //have a var set as the jail tile?
                if (tile.getData().getTileAction() == TileActionEnum.Jail) {
                    //    sent to jail -- for now setPosition();
                    players[currentTurn].setPosition(tile.getData().getPosition());
                    uiManager.setUpPopupUI(players[currentTurn] + " is jailed");
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
                player.increaseMoney(Int32.Parse(((NonPropData)board[player.getPosition()].getData()).getTileActionValue()));
                uiManager.setUpPopupUI(player.getPlayerName() + " gained £" + ((NonPropData)board[player.getPosition()].getData()).getTileActionValue());
                break;
            case TileActionEnum.PayTax:
                parkingPayment(player, Int32.Parse(((NonPropData)board[player.getPosition()].getData()).getTileActionValue()));
                uiManager.setUpPopupUI(player.getPlayerName() + " payed £" + ((NonPropData)board[player.getPosition()].getData()).getTileActionValue() + " in taxes");
                break;
            case TileActionEnum.PickCard:
                playCardAction(player);
                break;
            case TileActionEnum.Rent:
                if (board[player.getPosition()].getData().getOwner() != null && board[player.getPosition()].getData().getOwner() != player) {
                    playerPayment(player, board[player.getPosition()].getData().getOwner(), ((PropertyData)board[player.getPosition()].getData()).getCurrentRentPrice());
                    uiManager.setUpPopupUI(player.getPlayerName() + " payed " + board[player.getPosition()].getData().getOwner().getPlayerName() + " £" + ((PropertyData)board[player.getPosition()].getData()).getCurrentRentPrice() 
                        + " for landing on " + board[player.getPosition()].getData().getName());
                } else if(board[player.getPosition()].getData().getOwner() != player) {
                    uiManager.setupPurchaseUI((PropertyData)board[player.getPosition()].getData());
                    uiManager.setOn(uiManager.getPurchaseUIObj());
                    uiManager.setOff(uiManager.getEndTurnBtn());
                }
                break;
            case TileActionEnum.Jail: 
                break;
        }
    }

    /// <summary>
    /// Picks a card for the player and runs it's action
    /// </summary>
    /// <param name="player">Current player</param>
    public void playCardAction(PlayerObject player) {
        //make sure to get a card from correct pile
        CardObject card;
        if (board[player.getPosition()].getData().getName() == "Pot Luck") { //get a card and put at back of pile
            card = cards[1].Value.First.Value;
            cards[1].Value.RemoveFirst();
            cards[1].Value.AddLast(card);
        } else {
            card = cards[0].Value.First.Value;
            cards[0].Value.RemoveFirst();
            cards[0].Value.AddLast(card);
        }

        int val;

        //play UI of pick up ?
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
            case CardActionEnum.MoveForwards:
                if (!Int32.TryParse(card.getData().getCardActionValue(), out val)) {
                    foreach (TileObject tile in board.Values) {
                        if (tile.getData().getName() == card.getData().getCardActionValue()) {
                            player.setPosition(tile.getData().getPosition());
                            //call UI movements etc.
                            break;
                        }
                    }
                } else {
                    player.increasePosition(val);
                }
                break;
            case CardActionEnum.MoveBackwards:
                if (!Int32.TryParse(card.getData().getCardActionValue(), out val)) {
                    foreach (TileObject tile in board.Values) {
                        if (tile.getData().getName() == card.getData().getCardActionValue()) {
                            player.setPosition(tile.getData().getPosition());
                            //call UI movements etc.
                            break;
                        }
                    }
                } else {
                    player.increasePosition(-val);
                }
                break;
            case CardActionEnum.PayBank:
                bankPayment(player, -Int32.Parse(card.getData().getCardActionValue())); //& play relevant UI
                break;
            case CardActionEnum.PayParking:
                parkingPayment(player, Int32.Parse(card.getData().getCardActionValue())); //& play relevant UI
                break;
            case CardActionEnum.PlayersPay: //Only other players paying this player
                int i = 0;
                while (i < players.Length) {//might cause issues if go bankrupt halfway through
                    if (player != players[i]) {
                        playerPayment(player, players[i], Int32.Parse(card.getData().getCardActionValue()));
                    }
                }
                break;
            case CardActionEnum.MoveToGo:
                foreach (TileObject tile in board.Values) {
                    if (tile.getData().getTileAction() == TileActionEnum.CollectGO) {
                        player.setPosition(tile.getData().getPosition());
                        //call UI movements etc.
                        break;
                    }
                }
                break;
            case CardActionEnum.MoveForwardsGO:
                int prevPos = player.getPosition();
                if (!Int32.TryParse(card.getData().getCardActionValue(), out val)) {
                    foreach (TileObject tile in board.Values) {
                        if (tile.getData().getName() == card.getData().getCardActionValue()) {
                            player.setPosition(tile.getData().getPosition());
                            //call UI movements etc.
                            break;
                        }
                    }
                } else {
                    player.increasePosition(val);
                }

                if (prevPos > player.getPosition()) {
                    foreach (TileObject tile in board.Values) {
                        if (tile.getData().getTileAction() == TileActionEnum.CollectGO) {
                            player.increaseMoney(Int32.Parse(((NonPropData)tile.getData()).getTileActionValue()));
                            uiManager.setUpPopupUI(player.getPlayerName() + " gained £" + ((NonPropData)tile.getData()).getTileActionValue());
                            break;
                        }
                    }
                }
                break;
            case CardActionEnum.BankPays:
                bankPayment(player, Int32.Parse(card.getData().getCardActionValue()));
                break;
        }
        
        uiManager.setUpPopupUI(card.getData().getPile() + "\n" + card.getData().getName());
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
            uiManager.setUpPopupUI(from.getPlayerName() + " payed " + to.getPlayerName() + " £" + amount);
        } else {
            uiManager.setUpPopupUI(from.getPlayerName() + " is out of the game");
            from.setIsOut(true);
            StartCoroutine(isGameFinished());
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
            uiManager.setUpPopupUI(player.getPlayerName() + " is payed £" + amount);
        } else {
            if (player.getMoney() > amount) {
                player.decreaseMoney(amount);
                uiManager.setUpPopupUI(player.getPlayerName() + " lost £" + amount);
            } else {
                uiManager.setUpPopupUI(player.getPlayerName() + " is out of the game");
                player.setIsOut(true);
                StartCoroutine(isGameFinished());
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
            uiManager.setUpPopupUI(player.getPlayerName() + " is out of the game");
            player.setIsOut(true);
            StartCoroutine(isGameFinished());
        }


        //find the parking space - or the first one
        foreach (TileObject tile in board.Values) {
            if (tile.getData().getTileAction() == TileActionEnum.CollectFines) {
                //get the current fines on the space and increase it
                ((NonPropData)tile.getData()).setTileActionValue((Int32.Parse(((NonPropData)tile.getData()).getTileActionValue())+amount).ToString()); //dont increase if cant afford
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
