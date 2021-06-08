using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;

[System.Serializable]
public class SaveData
{
    public Dictionary<string, ActiveSkill> AskillDic = new Dictionary<string, ActiveSkill>();
    public Dictionary<string, PassiveSkill> PskillDic = new Dictionary<string, PassiveSkill>();
}

[System.Serializable]
public class SaveAndLoad : MonoBehaviour
{
    private string SAVE_DATA_DIRECTORY;  // 저장할 폴더 경로
    private string SAVE_FILENAME = "SkillData.txt"; // 파일 이름

    private SaveData saveData = new SaveData();
    private TitleUIManager title;

    //Save Skill Data
    public void Init()
    {
        title = FindObjectOfType<TitleUIManager>();
        if(title != null)
        {
            title.transform.Find("Debug Text").GetComponent<Text>().text = "텍스트 참조 성공";
        }

        //SaveData에 데이터를 세팅
        SetActiveSkill();
        SetPassiveSkill();

        try
        {
            //Json직렬화
            title.transform.Find("Debug Text").GetComponent<Text>().text = "try문 진입";

            string savejson = JsonConvert.SerializeObject(saveData); // ????
            if (savejson.Equals("{}"))
            {
                Debug.Log("json null");
                title.transform.Find("Debug Text").GetComponent<Text>().text = "json null";
                return;
            }

            SAVE_DATA_DIRECTORY = Path.Combine(Application.persistentDataPath , "GameData");

            if (!Directory.Exists(SAVE_DATA_DIRECTORY)) // 해당 경로가 존재하지 않는다면
                Directory.CreateDirectory(SAVE_DATA_DIRECTORY); // 폴더 생성(경로 생성)

            File.WriteAllText(Path.Combine(SAVE_DATA_DIRECTORY, SAVE_FILENAME), savejson);
            Debug.Log("스킬 데이터 저장 완료");
            title.transform.Find("Debug Text").GetComponent<Text>().text = "스킬 데이터 저장 완료";
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("The file was not found:" + e.Message);
            title.transform.Find("Debug Text").GetComponent<Text>().text = "파일을 찾을 수 없음";
        }
        catch (DirectoryNotFoundException e)
        {      
            Debug.Log("The directory was not found: " + e.Message);
            title.transform.Find("Debug Text").GetComponent<Text>().text = "디렉토리를 찾을 수 없음";
        }
        catch (IOException e)
        {
            Debug.Log("The file could not be opened:" + e.Message);
            title.transform.Find("Debug Text").GetComponent<Text>().text = "파일을 열 수 없음";
        }
    }


    public void TestDataLoad()
    {
        Dictionary<string, ActiveSkill> testDic = new Dictionary<string, ActiveSkill>();

        try
        {
            LoadData(testDic);
            title.transform.Find("Debug Text").GetComponent<Text>().text = testDic.ToString();
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("The file was not found:" + e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log("The directory was not found: " + e.Message);
        }
        catch (IOException e)
        {
            Debug.Log("The file could not be opened:" + e.Message);
        }
    }


    //Json파일을 역직렬화하여 데이터를 불러오는 함수입니다.
    public void LoadData<T>(Dictionary<string, T> aDic) where T : class
    {
        if (File.Exists(Path.Combine(SAVE_DATA_DIRECTORY, SAVE_FILENAME)))
        {
            // 전체 읽어오기
            string loadJson = File.ReadAllText(Path.Combine(SAVE_DATA_DIRECTORY, SAVE_FILENAME));
            //Json역직렬화
            saveData = JsonConvert.DeserializeObject<SaveData>(loadJson);

            Type valueType = typeof(T);

            if (valueType == typeof(ActiveSkill))
            {
                foreach (var item in saveData.AskillDic)
                {
                    string name = item.Key;
                    var value = item.Value;

                    aDic.Add(name, value as T);
                }
            }
            else if (valueType == typeof(PassiveSkill))
            {
                foreach (var item in saveData.PskillDic)
                {
                    string name = item.Key;
                    var value = item.Value;

                    aDic.Add(name, value as T);
                }
            }

            Debug.Log("스킬 데이터 로드 완료");
            title.transform.Find("Debug Text").GetComponent<Text>().text = "스킬 데이터 로드 완료";
        }
        else
        {
            Debug.Log("스킬 데이터 파일이 없습니다.");
            title.transform.Find("Debug Text").GetComponent<Text>().text = "스킬 데이터 파일이 없습니다.";
        }

    }

    //액티브 스킬의 초기설정 함수
    private void SetActiveSkill()
    {
        //image타입도 저장이 될까? 안되네 역시
        string name;

        ///param
        ///name, description, level, mana, coolTime, eigenValue
        name = "체인 플로레";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "고출력된 매직 미사일을 생성하여, 현재 플레이어가 옮길 수 있는 타일들을 전부 찾고, " +
                                                    "옮길 수 있는 타일들 중 하나를 선택해 번개 이펙트로 알려줍니다. \n" +
                                                    "이 번개 이펙트는, 플레이어가 타일을 누를 시에 사라지며, " +
                                                    "이펙트가 생성되고 있는 도중에는, 다시 이 스킬을 사용할 수 없습니다.",
                                                    1,
                                                    30,
                                                    10f,
                                                    0));

        name = "변이 파리채";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "전방에 거대한 파리채로 게임판을 강하게 내리쳐, 모든 플루토 타일들을 다른 랜덤 타일들로 바꿉니다. \n" +
                                                    "만약, 게임판에 플루토 타일이 하나도 없다면, 이 스킬을 사용할 수 없습니다.",
                                                    1,
                                                    50,
                                                    20f,
                                                    5));

        name = "잭프로스트 빙수";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "잭프로스트 형상의 빙수기를 조립하여, 남은 시간과 적의 움직임을 일정 시간동안 시원한 빙수로 만들어 멈추게 만듭니다. \n" +
                                                    "그러나, 상대가 스킬을 사용중이라면, 움직임을 멈출 수 없습니다.",
                                                    1,
                                                    70,
                                                    40f,
                                                    5));

        name = "잭 오 할로윈";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "가장 크고 달콤한 호박에 가장 질 좋은 붉은 사탕을 먹여 탄생한 호문쿨루스인 잭 오 할로윈을 생성합니다. \n" +
                                                    "사용할 경우, 일정 갯수의 잭오랜턴 타일을 '잭 오 할로윈' 타일로 바꿉니다. \n" +
                                                    "잭 오 할로윈 타일을 클릭할 경우, 자신을 기준으로 3x3범위의 타일을 파괴합니다. \n" +
                                                    "잭 오 할로윈 타일은 너무 뜨거운 마그마를 뿜어내어, 옮길 수 없습니다.",
                                                    1,
                                                    75,
                                                    30f,
                                                    3));
    }
    //패시브 스킬의 초기설정 함수
    private void SetPassiveSkill()
    {

        string name;


        ///param
        ///name, description, level, eigenValue

        name = "고대의 도서관";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "고대의 지식을 빌려, 게임 시작 시에 주어지는 제한 시간을 소폭 상승 시킨다.",
                                                    1,
                                                    10f));

        name = "쇼타임";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "제한 시간이 60초 이하로 남았을 경우 발동. 모든 점수 획득량이 증가한다",
                                                    1,
                                                    0.2f));

        name = "현자의 돌";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "마도학의 성배 현자의 돌을 촉매로 사용함으로서 이전까지와는 다른 수준의 성취를 이루어 낸다. \n " +
                                                    "퍼즐을 완성할 때 마다, 획득하는 점수가 상승한다.",
                                                    1,
                                                    1.0f));

        name = "붉은 사탕";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "마도학의 정수라고도 할 수 있는 붉은 사탕은, 현자의 돌을 가공하여 만든 세상에서 가장 귀중하면서도 위험한 사탕이다. \n" +
                                                    "스위트 캔디바 타일의 등장 확률이 상승하고, " +
                                                    "스위트 캔디바 타일과 매칭 시 점수 획득량이 크게 상승한다.",
                                                    1,
                                                    10f));
    }
}