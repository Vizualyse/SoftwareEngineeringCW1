using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<int, TileObject> board;
    private List<PlayerObject> players;
    private List<CardObject> cards;
    private int currentTurn; //will be used to get player at index currentTurn in the list

    void Start() {
        setupManager(); //if this runs as a simulator then it doesnt need to take player as param in its things
        //can have a bool to check whether a next turn has started? for what
    }

    public void setupManager() {
        currentTurn = 0;
        //somehow get inputs
        //inputs: file for board data, file for card data, No. of players, player name, player piece
        //for the board file, auto read and create tile object, tile data and add to board
        //for the card file, auto read and create card object, card data and add to cards
        //for player input, create playerObject and add to players
    }


    public void purchaseProperty() {
        //check if property not already owned && if player can afford
        //maybe check if the player is on that property currently <-- DO this as auctions dont exist yet
        //if can but, set owner of tileData and add to players list, and reduce their money
    }

    public void rollDice(PlayerObject player) {
        //call the dice roll code here and get the number rolled
        //add that number to the player.increasePosition(number);
        //Before adding, check for doubles, so if trible double then go to jail
        //check if player lands on an owned tile
        //check if the tile landed on has a tileAction
        //if so, then pay that player
    }

    public void setPlayOrder() {
        List<PlayerObject> temp = new List<PlayerObject>();

        //have every player roll a dice, compare the rolls to anyone already rolled
        //adds them in the correct position in the new temp list
        foreach (PlayerObject player in players) {
            rollDice(player);
            foreach(PlayerObject _player in temp) {
                if(player.getPosition() >= _player.getPosition()) {
                    temp.Insert(temp.IndexOf(_player),player);
                }
            }
            if (temp.IndexOf(player) == -1) {
                temp.Add(player);
            }
        }

        //then the temp list becomes the main list
        players = temp;
        //reset all the players positions to 0
        foreach (PlayerObject player in players) {
            player.setPosition(0); //may glitch and show the pieces moving in the future
        }
    }

    public void nextPlayerTurn() {
        //Is this necessary? yes because then players that are removed wont have a turn?
    }

    public void runTileAction(PlayerObject player) {
        //does this need to take player if it will call it itself? can just get the player whos turn it is
    }

    public void playCardAction(PlayerObject player) {
        //get a random card? again does it need player?
    }

    public void playerPayment(PlayerObject from, PlayerObject to, int amount) {
        //does this need to take both args or atleast does it need the to?
        if (from.getMoney() >= amount) {
            from.decreaseMoney(amount);
            to.increaseMoney(amount);
        } else {
            //Error? or dont allow? eventually let player mortgage etc. but for now remove player?
        }
    }

    public void bankPayment(PlayerObject player, int amount) {
        //same as aboves
        if (amount > 0) {
            player.increaseMoney(amount);
        } else {
            if (player.getMoney() > amount) {
                player.decreaseMoney(amount);
            } else {
                //remove from list?
            }
        }
    }

    public void parkingPayment(PlayerObject player, int amount) {
        //same as aboves
        foreach (TileObject tile in board.Values) {
            if (tile.getData().getTileAction() == TileActionEnum.CollectFines) {
                //get the current fines on the space and increase it
                ((NonPropData)tile.getData()).setTileActionValue((Int32.Parse(((NonPropData)tile.getData()).getTileActionValue())+amount).ToString());
            }
        }
    }

}
