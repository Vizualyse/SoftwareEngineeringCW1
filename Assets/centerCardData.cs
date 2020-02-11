using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Center Card")]
public class CardData : ScriptableObject{

	public string cardName;
	public enum cardType {OppertunityKnocks,PotLuck};
	public cardType cardtype;

	public string actionText;
	public enum actionType {payment,movement,outOfJail,goToJail};
	public enum participantType {player,allOtherPlayers,bank};

	public participantType leftParticipant; //in "player pays 50 to bank", this is player
	public actionType actiontype; //in "player pays 50 to bank", this is payment
	public string actionValue; //in "player pays 50 to bank", this is 50
	public participantType rightParticipant; //in "player pays 50 to bank", this is bank

	//maybe put this in the monobehaviour type script
	public void action(){
		//switch statement for actionType, LevelManager calls this to do the card action
		//for any payment ones, the actionValue will have to be changed to a int
		switch (actiontype){
			case actionType.goToJail:
				//player gets sent to the jail, no collecting money
				break;
			case actionType.outOfJail:
				//get a card that can be kept and used if in jail
				break;
			case actionType.movement:
				//to move forward + integer, to move backwards - integer; moving forwards mean able to collect money
				break;
			case actionType.payment:
				// leftParticipant pays rightParticipant actionValue amount of money
				break;
		}	
	}
}
