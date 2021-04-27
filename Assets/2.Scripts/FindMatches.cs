using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public List<GameObject> currentMatches = new List<GameObject>();

    private BoardManager board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
    }


    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }



    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.2f);
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                GameObject currentTile = board.characterTilesBox[x, y];
                if (currentTile != null)
                {
                    if (x > 0 && x < board.width - 1)
                    {
                        GameObject leftTile = board.characterTilesBox[x - 1, y];
                        GameObject rightTile = board.characterTilesBox[x + 1, y];
                        if (leftTile != null && rightTile != null)
                        {
                            if (leftTile.tag == currentTile.tag && rightTile.tag == currentTile.tag)
                            {
                                if (!currentMatches.Contains(leftTile))
                                {
                                    currentMatches.Add(leftTile);

                                }
                                leftTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(rightTile))
                                {
                                    currentMatches.Add(rightTile);

                                }
                                rightTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(currentTile))
                                {
                                    currentMatches.Add(currentTile);

                                }
                                currentTile.GetComponent<Tile>().isMatched = true;
                            }
                        }
                    }

                    if (y > 0 && y < board.height - 1)
                    {
                        GameObject downTile = board.characterTilesBox[x, y - 1];
                        GameObject upTile = board.characterTilesBox[x, y + 1];
                        if (downTile != null && upTile != null)
                        {
                            if (downTile.tag == currentTile.tag && upTile.tag == currentTile.tag)
                            {
                                if (!currentMatches.Contains(downTile))
                                {
                                    currentMatches.Add(downTile);
                                }
                                downTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(upTile))
                                {
                                    currentMatches.Add(upTile);
                                }
                                upTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(currentTile))
                                {
                                    currentMatches.Add(currentTile);

                                }
                                currentTile.GetComponent<Tile>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
