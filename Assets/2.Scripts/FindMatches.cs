using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public List<GameObject> currentMatches = new List<GameObject>();

    private int lolipopCount = 0;

    //코루틴의 동작을 제어하는 상태변수
    public bool isUpdate = false;

    public int LolipopCount { get => lolipopCount; set => lolipopCount = value; }

    public void FindAllMatches()
    {
        if (!isUpdate)
            StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        isUpdate = true;
        LolipopCount = 0;
        yield return new WaitForSeconds(.2f);
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                GameObject currentTile = BoardManager.instance.characterTilesBox[x, y];
                if (currentTile != null)
                {
                    if (x > 0 && x < BoardManager.instance.width - 1)
                    {
                        GameObject leftTile = BoardManager.instance.characterTilesBox[x - 1, y];
                        GameObject rightTile = BoardManager.instance.characterTilesBox[x + 1, y];

                        CompareTile(currentTile, leftTile, rightTile);
                    }

                    if (y > 0 && y < BoardManager.instance.height - 1)
                    {
                        GameObject downTile = BoardManager.instance.characterTilesBox[x, y - 1];
                        GameObject upTile = BoardManager.instance.characterTilesBox[x, y + 1];

                        CompareTile(currentTile, downTile, upTile);
                    }
                }
            }
        }
        isUpdate = false;
    }//coroutine

    private void CompareTile(GameObject currentTile, GameObject firstTile, GameObject secondTile)
    {
        if (firstTile != null && secondTile != null)
        {
            // 1. 현재 타일이 롤리팝 타일이면
            if (currentTile.CompareTag("Lolipop"))
            {
                //비교하는 타일중에 롤리팝이 또 있다면
                if (firstTile.CompareTag("Lolipop") || secondTile.CompareTag("Lolipop"))
                {
                    AddMatching(currentTile, firstTile, secondTile);
                    LolipopCount += 2;
                }
                else if (secondTile.CompareTag(firstTile.tag))
                {
                    AddMatching(currentTile, firstTile, secondTile);
                    LolipopCount++;
                }
            }
            // 2. 현재 타일이 롤리팝이 아니지만
            else if (!currentTile.CompareTag("Lolipop"))
            {
                // 내 옆의 타일 중 하나가 롤리팝이라면
                if (firstTile.CompareTag("Lolipop"))
                {
                    if (secondTile.CompareTag(currentTile.tag))
                    {
                        AddMatching(currentTile, firstTile, secondTile);
                        LolipopCount++;
                    }
                }
                else if (secondTile.CompareTag("Lolipop"))
                {
                    if (firstTile.CompareTag(currentTile.tag))
                    {
                        AddMatching(currentTile, firstTile, secondTile);
                        LolipopCount++;
                    }
                }
                // 3. 타일 3개가 전부 캔디가 아닐 때
                else
                {
                    //3개의 타일이 서로 같아야한다.
                    if (currentTile.CompareTag(firstTile.tag) && currentTile.CompareTag(secondTile.tag))
                    {
                        AddMatching(currentTile, firstTile, secondTile);
                    }
                }
            }
        }
    }

    private void AddMatching(GameObject currentTile, GameObject firstTile, GameObject secondTile)
    {
        if (!currentMatches.Contains(firstTile))
        {
            currentMatches.Add(firstTile);
        }
        firstTile.GetComponent<Tile>().isMatched = true;

        if (!currentMatches.Contains(secondTile))
        {
            currentMatches.Add(secondTile);
        }
        secondTile.GetComponent<Tile>().isMatched = true;
        if (!currentMatches.Contains(currentTile))
        {
            currentMatches.Add(currentTile);
        }
        currentTile.GetComponent<Tile>().isMatched = true;
    }
}