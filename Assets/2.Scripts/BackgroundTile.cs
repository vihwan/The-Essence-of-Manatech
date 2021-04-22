using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BackgroundTile : MonoBehaviour
{
    public List<Sprite> characters = new List<Sprite>(); //캐릭터들을 저장하는 리스트

    public GameObject[] characterTiles;
    public GameObject characterTilesPrefab;  //Tile Prefab

    private void Start()
    {
        CreateTile();
    }

    private void CreateTile()
    {
        GameObject newCharacterTile = Instantiate(characterTilesPrefab, transform.position, Quaternion.identity);
        newCharacterTile.transform.SetParent(transform);


        /*#region 처음 보드를 생성할 때, 바로 캐릭터 타일이 3개가 연결되어 나오지 않도록 방지하는 코드
        List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
        possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음

        possibleCharacters.Remove(previousLeft[y]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
        possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제
        #endregion

        Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
        newCharacterTile.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
        previousLeft[y] = newSprite;
        previousBelow = newSprite;*/
    }
}
