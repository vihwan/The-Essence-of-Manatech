using System.Collections;
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
    private string SAVE_FILENAME = "/SkillData.txt"; // 파일 이름

    private SaveData saveData = new SaveData();

    //Save Skill Data
    private void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/8.GameData/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY)) // 해당 경로가 존재하지 않는다면
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY); // 폴더 생성(경로 생성)

        SetActiveSkill();
        SetPassiveSkill();

        //Json직렬화
        string savejson = JsonConvert.SerializeObject(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, savejson);

        Debug.Log("스킬 데이터 저장 완료");
        Debug.Log(savejson);
    }



    public void LoadData<T> (Dictionary<string, T> aDic) where T : class
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            // 전체 읽어오기
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            //Json역직렬화
            saveData = JsonConvert.DeserializeObject<SaveData>(loadJson);

            Type valueType = typeof(T);

            if(valueType == typeof(ActiveSkill))
            {
                foreach (var item in saveData.AskillDic)
                {
                    string name = item.Key;
                    var value = item.Value;

                    aDic.Add(name, value as T);
                }
            }
            else if(valueType == typeof(PassiveSkill))
            {
                foreach (var item in saveData.PskillDic)
                {
                    string name = item.Key;
                    var value = item.Value;

                    aDic.Add(name, value as T);
                }
            }

            Debug.Log("스킬 데이터 로드 완료");
        }
        else
            Debug.Log("스킬 데이터 파일이 없습니다.");
    }

    private void SetActiveSkill()
    {
        //image타입도 저장이 될까? 안되네 역시
        string name;

        ///param
        ///name, description, level, mana, coolTime, eigenValue
        name = "체인 플로레";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "사용할 경우, 옮길 수 있는 타일의 위치를 알려줍니다.",
                                                    1,
                                                    30,
                                                    10f,
                                                    0));

        name = "변이 파리채";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "사용할 경우, 플루토 타일들을 다른 랜덤 타일들로 바꿉니다.",
                                                    1,
                                                    50,
                                                    20f,
                                                    5));

        name = "잭프로스트 빙수";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "미구현입니다.",
                                                    1,
                                                    70,
                                                    40f,
                                                    5));

        name = "잭 오 할로윈";
        saveData.AskillDic.Add(name, new ActiveSkill(name,
                                                    "사용할 경우, 잭오랜턴 타일을 랜덤하게 잭오 할로윈 타일로 바꿉니다. \n " +
                                                    "클릭할 경우, 3x3범위의 타일을 파괴합니다.",
                                                    1,
                                                    75,
                                                    30f,
                                                    3));
    }

    private void SetPassiveSkill()
    {

        string name;


        ///param
        ///name, description, level, eigenValue

        name = "고대의 도서관";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "처음 시작 시, 주어지는 제한 시간을 소폭 상승 시킨다.",
                                                    1,
                                                    10f));

        name = "쇼타임";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "제한 시간이 30초 이하로 남았을 경우 발동. 모든 점수 획득량이 증가한다",
                                                    1,
                                                    0.2f));

        name = "현자의 돌";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "퍼즐을 완성할 때 마다, 획득하는 점수가 상승한다.",
                                                    1,
                                                    1.0f));

        name = "붉은 사탕";
        saveData.PskillDic.Add(name, new PassiveSkill(name,
                                                    "스위트 캔디바 타일의 등장 확률이 상승하고, " +
                                                    "스위트 캔디바 타일과 매칭 시 점수 획득량이 크게 상승한다.",
                                                    1,
                                                    10f));
    }
}