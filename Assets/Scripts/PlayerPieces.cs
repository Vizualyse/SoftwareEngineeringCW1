using UnityEngine;
using System.Collections;

public class PlayerPieces : MonoBehaviour
{
    private GameManager gm;
    private PlayerObject[] players;
    private GameObject[] playerPieces;
    private int[] piecesRotations;
    private int[] playerPos;

    private bool init = false;

    private GameObject goTile;

    public void Init()
    {
        init = true;

        goTile = GameObject.Find("Tile1");

        gm = GameObject.Find("GM").GetComponent<GameManager>();
        players = gm.getPlayers();
        playerPos = new int[players.Length];
        playerPieces = new GameObject[players.Length];
        piecesRotations = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerPos[i] = players[i].getPosition();
            playerPieces[i] = Prefab(players[i].getPiece(), i);
        }
    }

    public GameObject Prefab(PiecesEnum name, int playerNo)
    {
        Object pf = null;
        switch (name)
        {
            case PiecesEnum.Hatstand:
                break;
            case PiecesEnum.Goblet:
                break;
            default:
                pf = Resources.Load("Pieces/" + name);
                break;
        }
         
        if (pf != null)
        {
            GameObject inst = (GameObject)Instantiate(pf);
            
            if (inst != null)
            {
                if (playerNo < 3)
                {
                    inst.transform.position = goTile.transform.position + new Vector3(0.2f, 0.2f, -0.2f + (playerNo * 0.6f));
                }
                else
                {
                    inst.transform.position = goTile.transform.position + new Vector3(0.5f, 0.2f, -0.2f + ((playerNo-3) * 0.6f));
                }
                switch (name)
                {
                    case PiecesEnum.Boot:
                        inst.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        inst.transform.Rotate(new Vector3(0, -90, 0));
                        piecesRotations[playerNo] = -90;
                        break;
                    case PiecesEnum.Cat:
                        inst.transform.localScale = new Vector3(3, 3, 3);
                        inst.transform.Rotate(new Vector3(0, -90, 0));
                        piecesRotations[playerNo] = -90;
                        break;
                    default:
                        inst.transform.localScale = new Vector3(5, 5, 5);
                        inst.transform.Rotate(new Vector3(0, 90, 0));
                        piecesRotations[playerNo] = 90;
                        break;
                }
                return inst;
            }
        }
        return null;
    }


    void Update()
    {
        if (init)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].getPosition() != playerPos[i])
                {
                    UpdatePosition(i, players[i].getPosition());
                    playerPos[i] = players[i].getPosition();
                    
                }
            }
        }
    }

    void UpdatePosition(int playerNo, int newPos)
    {
        GameObject newTile = GameObject.Find("Tile" + (newPos));
        GameObject newTileParent = newTile.transform.parent.gameObject;
        int playersOnTile = 0;
        foreach(int p in playerPos)
        {
            if(p == newPos)
            {
                playersOnTile++;
            }
        }
        Vector3 tileOffset = new Vector3(1.3f, 0, -0.2f + playersOnTile * 0.6f);
        switch (newTileParent.name)
        {
            case "Bot":
                tileOffset = Quaternion.Euler(0, 0, 0) * tileOffset;
                playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo], 0);
                break;
            case "Left":
                tileOffset = Quaternion.Euler(0, 90, 0) * tileOffset;
                playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo]+90, 0);
                break;
            case "Top":
                tileOffset = Quaternion.Euler(0, 180, 0) * tileOffset;
                playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo]+180, 0);
                break;
            case "Right":
                tileOffset = Quaternion.Euler(0, 270, 0) * tileOffset;
                playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo]+270, 0);
                break;
        }
        Vector3 start = playerPieces[playerNo].transform.position;
        Vector3 end = new Vector3(newTile.transform.position.x , 0.2f, newTile.transform.position.z) + tileOffset;
        StartCoroutine(movePiece(start, end, playerPieces[playerNo]));
    }

    IEnumerator movePiece(Vector3 start, Vector3 end, GameObject piece)
    {
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / 1f;
            piece.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }
}
