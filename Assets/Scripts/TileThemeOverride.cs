using System.Collections.Generic;
using UnityEngine;

public class TileThemeOverride : MonoBehaviour
{
    [Header("Font Size")]
    public int stationFontSize = 20;
    public int utilitiesFontSize = 20;
    public int cardTileFontSize = 17;
    public int propertyTileFontSize = 16;
    public int taxTileFontSize = 17;
    public int priceFont = 15;


    public void ModifyTile()
    {
        GMUI gmui = GameObject.Find("GM").GetComponent<GMUI>();
        GameObject[] boardTiles = gmui.boardTiles;

        List<int> cornerTiles = new List<int>(new int[] { 1, 11, 21, 31 });
        List<int> stations = new List<int>(new int[]{ 6, 16, 26, 36 });
        List<int> utilities = new List<int>(new int[] { 13, 29 });
        List<int> cardTiles = new List<int>(new int[] { 3, 8, 18, 23, 34, 37 });
        List<int> taxTiles = new List<int>(new int[] { 39, 5 });

        foreach (GameObject g in boardTiles)
        {
            string tileNo = g.name.Substring(4);
            TextMesh[] textMeshes = g.GetComponentsInChildren<TextMesh>();
            TextMesh name = g.GetComponentInChildren<TextMesh>();
            TextMesh price = null;


            switch (tileNo)
            {
                case string a when stations.Contains(int.Parse(tileNo)):    //stations
                    name.text = name.text.Replace(" ", "\n");
                    name.alignment = UnityEngine.TextAlignment.Center;
                    name.fontSize = stationFontSize;
                    g.transform.GetChild(2).localPosition = new Vector3(1.132f, 1, 1);
                    price = (TextMesh)textMeshes.GetValue(1);
                    price.fontSize = priceFont;
                    break;

                case string a when utilities.Contains(int.Parse(tileNo)):   //utilites
                    name.text = name.text.Replace(" ", "\n");
                    name.alignment = UnityEngine.TextAlignment.Center;
                    name.fontSize = utilitiesFontSize;
                    g.transform.GetChild(2).localPosition = new Vector3(1.132f, 1, 1);
                    price = (TextMesh)textMeshes.GetValue(1);
                    price.fontSize = priceFont;
                    break;

                case string a when cardTiles.Contains(int.Parse(tileNo)):   //cards
                    name.text = name.text.Replace(" ", "\n");
                    name.alignment = UnityEngine.TextAlignment.Center;
                    name.fontSize = cardTileFontSize;
                    g.transform.GetChild(2).localPosition = new Vector3(1.132f, 1, 1);
                    break;

                case string a when taxTiles.Contains(int.Parse(tileNo)):    //tax
                    name.text = name.text.Replace(" ", "\n");
                    name.alignment = UnityEngine.TextAlignment.Center;
                    name.fontSize = taxTileFontSize;
                    g.transform.GetChild(2).localPosition = new Vector3(1.132f, 1, 1);
                    break;

                case string a when cornerTiles.Contains(int.Parse(tileNo))  :   break;

                default:
                    name.text = name.text.Replace(" ", "\n");
                    name.alignment = UnityEngine.TextAlignment.Center;
                    name.fontSize = propertyTileFontSize;
                    g.transform.GetChild(2).localPosition = new Vector3(1.132f, 1, 0.72f);
                    price = (TextMesh)textMeshes.GetValue(1);
                    price.fontSize = priceFont;
                    break;
            }
            
            
        }
    }
}
