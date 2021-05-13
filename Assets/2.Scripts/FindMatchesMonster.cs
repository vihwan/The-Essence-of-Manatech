using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//몬스터 전용 FindMatches Class
public class FindMatchesMonster : MonoBehaviour
{
    public List<GameObject> currentMatches = new List<GameObject>();

    //코루틴의 동작을 제어하는 상태변수
    public bool isUpdate = false;

    public void FindAllMatches()
    {
        if (!isUpdate)
            StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        isUpdate = true;
        yield return new WaitForSeconds(.2f);
        for (int x = 0; x < BoardManagerMonster.instance.width; x++)
        {
            for (int y = 0; y < BoardManagerMonster.instance.height; y++)
            {
                GameObject currentTile = BoardManagerMonster.instance.monsterTilesBox[x, y];
                if (currentTile != null)
                {
                    if (x > 0 && x < BoardManagerMonster.instance.width - 1)
                    {
                        GameObject leftTile = BoardManagerMonster.instance.monsterTilesBox[x - 1, y];
                        GameObject rightTile = BoardManagerMonster.instance.monsterTilesBox[x + 1, y];

                        CompareTile(currentTile, leftTile, rightTile);
                    }

                    if (y > 0 && y < BoardManagerMonster.instance.height - 1)
                    {
                        GameObject downTile = BoardManagerMonster.instance.monsterTilesBox[x, y - 1];
                        GameObject upTile = BoardManagerMonster.instance.monsterTilesBox[x, y + 1];

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
            //3개의 타일이 서로 같아야한다.
            if (currentTile.CompareTag(firstTile.tag) && currentTile.CompareTag(secondTile.tag))
            {
                AddMatching(currentTile, firstTile, secondTile);
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
