using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Card", menuName = "Tile Card")]
public class tileCardData : ScriptableObject {

	public string cardName;
	public string cardColour;
	public int cardPosition; //position on board
	public enum tileType {property,train,utility,other} //train comes under property
	public tileType tiletype;

	public enum rentTypes {baseCost,oneHouse,twoHouses,threeHouses,fourHouses,hotel,twoTrains,threeTrains,fourTrains};
	public Dictionary<rentTypes,int> rentPrices; //set as 'null' for non-rentable tile's, or leave empty?

	//maybe put this in the monobehaviour type script
	public void action(){
		switch (tiletype) {
		case tileType.property:
			//
			break;
		}
	}
}
