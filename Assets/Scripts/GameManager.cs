using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<int, TileObject> board;
    private PlayerObject[] players;
    private CardObject[] cards;
    private int currentTurn; //will be used to get player at index currentTurn in the list

    void Start() {
        setupManager(); //if this runs as a simulator then it doesnt need to take player as param in its things - or can to save rewrites
        //can have a bool to check whether a next turn has started? for what

        setPlayOrder();

        //instead of above, have a while loop that runs the turns
        //it starts with a rollDice
        //then it will  runTileAction? - no as this is in rollDice for now
        //after rollDice, the turn 'action' will be done too, so popup endTurn button & wait for an end turn, then do nextPlayerTurn()
        runTurns();
    }

    public void runTurns() {
        rollDice(players[currentTurn]);
        while (!isGameFinished()) {
            rollDice(players[currentTurn]);
            //pop and wait for response via UI
            //have a waitUntil, and a bool turnEnded, and when thats true, end waitUntil, and at start of loop set turnEnded == false - only show button after roll
            nextPlayerTurn();
        }
    }

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


    public void setupManager() {
        currentTurn = 0;
        //somehow get inputs
        //inputs: file for board data, file for card data, No. of players, player name, player piece
        //for the board file, auto read and create tile object, tile data and add to board
        //for the card file, auto read and create card object, card data and add to cards
        //for player input, create playerObject and add to players
    }


    public void purchaseProperty(PlayerObject player) {
        if (player.getMoney() >= ((PropertyData)board[player.getPosition()].getData()).getPurchasePrice()) {
            player.decreaseMoney(((PropertyData)board[player.getPosition()].getData()).getPurchasePrice());
            player.addProperty(board[player.getPosition()]);
            board[player.getPosition()].getData().setOwner(player);
        } else {
            //play UI to say you cant afford?
        }
    }

    public void rollDice(PlayerObject player) {
        //call the dice roll code here and get the number rolled
        int totalRolled = 0;
        bool jailed = false;
        //totalRolled += roll
        //if (rolled double) {
        //    rollagain
        //    totalRolled += roll
        //    if(rolldouble){
        //        roll again && totalRolled += roll
        //        if(rolleddouble){
        //            totalRolled = 0 && jailed
        //        }
        //    }
        //}


        if (jailed) {
            foreach (TileObject tile in board.Values) { //have a var set as the jail tile?
                if (tile.getData().getTileAction() == TileActionEnum.Jail) {
                    //    sent to jail -- for now setPosition();
                    player.setPosition(tile.getData().getPosition());
                    //call UI movements etc.
                    break;
                }
            }
        } else {
            player.increasePosition(totalRolled);
        }
        //runTileAction(), or maybe run tileAction for jailed aswell?
        //play the relevant ui stuff for the above
    }

    public void setPlayOrder() {
        SortedDictionary<int, PlayerObject> tempDict = new SortedDictionary<int, PlayerObject>();

        //have every player roll a dice, compare the rolls to anyone already rolled
        //adds them in the correct position in the new temp list

        for (int i = 0; i < players.Length; i++) {
            //roll the dice
            //tempDict.Add(roll,players[i]);
        }

        //add the players in the new order
        int j = players.Length - 1;
        foreach (int roll in tempDict.Keys) {
            players[j] = tempDict[roll];
            j--;
        }
        //reset all the players positions to 0
        foreach (PlayerObject player in players) {
            player.setPosition(0); //may glitch and show the pieces moving in the future
        }
    }

    public void nextPlayerTurn() {
        //Is this necessary? yes because then players that are removed wont have a turn?
        //will have to check that the next slot is not empty - as a player has lost
        currentTurn = (currentTurn + 1) % players.Length;
        while (players[currentTurn].getIsOut()) {
            currentTurn = (currentTurn + 1) % players.Length;
        }
    }

    public void runTileAction(PlayerObject player) {
        //does this need to take player if it will call it itself? can just get the player whos turn it is
        switch (board[player.getPosition()].getData().getTileAction()) {
            case TileActionEnum.CollectGO:
            case TileActionEnum.CollectFines:
                player.increaseMoney(Int32.Parse(((NonPropData)board[player.getPosition()].getData()).getTileActionValue()));
                break;
            case TileActionEnum.PayTax:
                parkingPayment(player, Int32.Parse(((NonPropData)board[player.getPosition()].getData()).getTileActionValue()));
                break;
            case TileActionEnum.PickCard:
                playCardAction(player);
                break;
            case TileActionEnum.Rent:
                //check if is owned, and if is then call playerpayment, and check if can afford in there?
                if (board[player.getPosition()].getData().getOwner() != null) {
                    playerPayment(player, board[player.getPosition()].getData().getOwner(), ((PropertyData)board[player.getPosition()].getData()).getCurrentRentPrice());
                } else {
                    //give option to buy - through a popup made by GM
                    //if player says yes - then do:
                    purchaseProperty(player);
                }
                break;
            case TileActionEnum.Jail: //have someway of tracking turns spent in jail etc.
                break;
        }
    }

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

    public void playerPayment(PlayerObject from, PlayerObject to, int amount) {
        if (from.getMoney() >= amount) {
            from.decreaseMoney(amount);
            to.increaseMoney(amount);
        } else {
            from.setIsOut(true);
        }
    }

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
                //get the current fines on the space and increase it
                ((NonPropData)tile.getData()).setTileActionValue((Int32.Parse(((NonPropData)tile.getData()).getTileActionValue())+amount).ToString());
            }
        }
    }

    //endTurn()

}
