using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region Field Variable
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);

    //상태변수
    private bool isSelected = false;
    private bool isDragging = false;
    private bool matchFound = false;
    //  public static bool isContinuousMatch = false;

    const float DRAG_SPEED = 5f;

    [SerializeField] private int row;
    [SerializeField] private int col;

    int dirX = 0;
    int dirY = 0;


    private float verticalSpeed;
    private float horizontalSpeed;
    private Vector3 originPos;
    private Vector3 dragDirection;
    private RaycastHit hitInfo;


    private Vector3[] adjacentDirections = new Vector3[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down };

    //필요한 컴포넌트
    private Image image;
    private BoardManager boardManager;
    private static Tile selectedTile = null;

    #endregion

    private void Start()
    {
        image = GetComponent<Image>();
        boardManager = FindObjectOfType<BoardManager>();
        originPos = transform.position;
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            image = null;
            return;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectedTile == null)
            Select();
        Debug.Log(eventData.pointerCurrentRaycast); //클릭한 대상의 레이캐스트 정보를 얻는다.
        Debug.Log("row : " + row + ", col : " + col);

    }

    public void OnPointerUp(PointerEventData eventData)
    {

        isDragging = false;

        // 이동거리가 타일의 0.8 이상이라면
        // 드래그 대상과 서로 교체 SwapTile
        // 아니면 원래대로
        if (dragDirection.magnitude >= gameObject.GetComponent<Image>().rectTransform.rect.width * 0.9)
        {

            Debug.Log("조건만족, 스왑 실행");
            SwapTile();

        }
        else
            transform.position = originPos; //원래 자리로 돌아옴

        Deselect();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //PointerEventData.delta : Pointer delta since last update.
        //마우스의 이동방향을 받아오기 위함.
        float mouseDirectionX = eventData.delta.normalized.x;
        float mouseDirectionY = eventData.delta.normalized.y;

        #region 드래그 방향 제한
        if (Mathf.Abs(mouseDirectionX) > Mathf.Abs(mouseDirectionY) && !isDragging)
        {
            //x가 y보다 크면, x축으로만 이동하도록 제한
            isDragging = true;
            horizontalSpeed = DRAG_SPEED;
            verticalSpeed = 0f;

        }
        else if (Mathf.Abs(mouseDirectionY) > Mathf.Abs(mouseDirectionX) && !isDragging)
        {
            //y가 x보다 크면, y축으로만 이동하도록 제한
            isDragging = true;
            verticalSpeed = DRAG_SPEED;
            horizontalSpeed = 0f;
        }
        #endregion

        DecideDirection(mouseDirectionY, mouseDirectionX);

        transform.position += new Vector3(mouseDirectionX * horizontalSpeed, mouseDirectionY * verticalSpeed, 0f);

        dragDirection = transform.position - originPos;
        #region 드래그 이동거리 제한
        //이동거리 제한
        if (dragDirection.magnitude > gameObject.GetComponent<Image>().rectTransform.rect.width)
        {
            Vector3 dir = dragDirection.normalized;
            transform.position = new Vector3(originPos.x, originPos.y, originPos.z) + dir * gameObject.GetComponent<Image>().rectTransform.rect.height;
        }

        #endregion

    }


    //방향을 결정하여 교체할 대상을 쉽게 파악할 수 있다.
    private void DecideDirection(float mouseDirX, float mouseDirY)
    {
        if (horizontalSpeed > 0 && verticalSpeed == 0) //가로 이동
        {
            if (mouseDirX > 0)
                dirX = 1;
            else if (mouseDirX < 0)
                dirX = -1;
            dirY = 0;
        }
        else if (verticalSpeed > 0 && horizontalSpeed == 0)
        {
            if (mouseDirY > 0)
                dirY = 1;
            else if (mouseDirY < 0)
                dirY = -1;
            dirX = 0;
        }
    }



    //타일을 선택할 경우
    //1. 타일의 색상이 바뀐다.
    //2. 타일 선택 소리가 나온다.
    private void Select()
    {
        isSelected = true;
        image.color = selectedColor;
        selectedTile = gameObject.GetComponent<Tile>();
        SoundManager.instance.PlaySE("Select");
    }

    private void Deselect()
    {
        isSelected = false;
        image.color = Color.white;
        selectedTile = null;
    }


    private void Matching()
    {
        


        ClearAllMatches();
        if (BoardManager.instance.MatchFoundCount == 0)
            GUIManager.instance.ComboCounter = 0;
    }


    //타일 위치 서로 바꾸기
    public void SwapTile()
    {
        if (horizontalSpeed != 0)
        {
            if (row + dirX > 7 || row + dirX < 0) 
                return;
            Switching();
        }
        else if (verticalSpeed != 0)
        {
            if (col + dirY > 11 || col + dirY < 0) 
                return;
            Switching();
        }
        SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행
    }


    private void Switching()
    {
        GameObject secondTile = boardManager.GetTile(row + dirX, col + dirY);

        int secondTileRow = secondTile.GetComponent<Tile>().GetArrNumX();
        int secondTileCol = secondTile.GetComponent<Tile>().GetArrNumY();

        // 넘어간 타일과 위치를 교환한다.
        transform.position = secondTile.GetComponent<Tile>().GetOriginPos();
        secondTile.transform.position = originPos;

        // 각 타일의 originPos 값을 초기화한다.
        originPos = transform.position;
        secondTile.GetComponent<Tile>().SetOriginPos(secondTile.transform.position);


        //두 타일의 row와 col를 바꾼다.
        int tempRow = gameObject.GetComponent<Tile>().row;
        int tempCol = gameObject.GetComponent<Tile>().col;

        gameObject.GetComponent<Tile>().SetArrNumber(secondTileRow, secondTileCol);
        secondTile.GetComponent<Tile>().SetArrNumber(tempRow, tempCol);

        /*
                //두 타일의 이름을 바꾼다.
                gameObject.name = FindObjectOfType<BoardManager>().SetTileName(secondTileRow, secondTileCol);
                secondTile.name = FindObjectOfType<BoardManager>().SetTileName(tempRow, tempCol);*/
    }

    private void SetOriginPos(Vector3 originPos)
    {
        this.originPos = originPos;
    }

    private Vector3 GetOriginPos()
    {
        return originPos;
    }


    //특정 타일과 근접한 모든타일을 얻어서 저장한다.
    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        //전역변수로 설정한 방향으로 전부 빛을 쏴서 타일을 찾은 뒤에
        //선택했던 타일과 인접하는지를 찾는 과정이다.
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    //특정 타일에서 Ray를 쏴서 부딪히는 컬라이더 오브젝트를 찾는다.
    private GameObject GetAdjacent(Vector3 castDir)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, castDir, out hitInfo))
        {
            if (hitInfo.collider != null)
            {
                return hitInfo.collider.gameObject;
            }
        }
        return null;
    }


    //매칭이 되었는지를 검사하는 함수
    private List<GameObject> FindMatch(Vector3 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit hitInfo;
        bool isMatch = Physics.Raycast(transform.position, castDir, out hitInfo);
        if (isMatch)
        {
            while (hitInfo.collider != null && hitInfo.collider.GetComponent<SpriteRenderer>().sprite == image.sprite)
            {
                matchingTiles.Add(hitInfo.collider.gameObject);
                Physics.Raycast(hitInfo.collider.transform.position, castDir, out hitInfo);
            }
        }
        return matchingTiles;
    }

    //매칭타일을 클리어
    private void ClearMatch(Vector3[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    //모든 매칭들을 클리어
    public void ClearAllMatches()
    {
        if (image.sprite == null)
            return;

        ClearMatch(new Vector3[2] { Vector3.left, Vector3.right });
        ClearMatch(new Vector3[2] { Vector3.up, Vector3.down });

        if (matchFound)
        {
            image.sprite = null;
            matchFound = false;
            //???
            //ShiftDelay가 너무 짧아서 코루틴 실행이 다 안된건가?
            //아니면 또다른 매칭이 이루어져서 코루틴이 멈춰버린 것인가? ★
            //StopCoroutine(BoardManager.instance.FindNullTiles());

            //비어있는 타일을 검색
            BoardManager.instance.FindNullTiles();
            SoundManager.instance.PlaySE("Clear");

            GUIManager.instance.ComboCounter++;
            BoardManager.instance.MatchFoundCount++;

        }
    }


    public void SetArrNumber(int x, int y)
    {
        row = x;
        col = y;
    }


    public int GetArrNumX()
    {
        return row;
    }
    
    public int GetArrNumY()
    {
        return col;
    }
}
