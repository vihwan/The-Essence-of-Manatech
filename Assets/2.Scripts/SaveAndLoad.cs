using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class SaveData
{
    public List<ActiveSkill> AskillList = new List<ActiveSkill>();
    public List<PassiveSkill> PskillList = new List<PassiveSkill>();
}

public class SaveAndLoad : MonoBehaviour
{
    private string SAVE_DATA_DIRECTORY;  // 저장할 폴더 경로
    private string SAVE_FILENAME = "/SkillData.txt"; // 파일 이름

    private SaveData saveData = new SaveData();
    private ActiveSkill[] actSkills = new ActiveSkill[4];
    private PassiveSkill[] pasSkills = new PassiveSkill[4];

    //Save Skill Data
    private void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/8.GameData/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY)) // 해당 경로가 존재하지 않는다면
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY); // 폴더 생성(경로 생성)

        SetActiveSkill();
        SetPassiveSkill();

        //Json직렬화
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    //LoadData를 여러 속성의 List들이 쓸 수 있도록 Refactoring
    public void LoadData<T>(List<T> tList) where T : class
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            // 전체 읽어오기
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            //Json역직렬화
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            for (int i = 0; i < saveData.PskillList.Count; i++)
            {
                //명시적 캐스팅을 해주지 않으면 에러
                tList.Add(saveData.PskillList[i] as T);
            }

            Debug.Log("스킬 데이터 로드 완료");
        }
        else
            Debug.Log("스킬 데이터 파일이 없습니다.");
    }

    private void SetActiveSkill()
    {
        //skill 안에 있는 변수
        //name, description, icon, level, necessaryMana, cooldownTime
        //image타입도 저장이 될까? 안되네 역시

        actSkills[0] = new ActiveSkill
        {
            name = "체인 플로레",
            description = "사용할 경우, 옮길 수 있는 타일의 위치를 알려줍니다.",
            level = 1,
            necessaryMana = 30,
            cooldownTime = 10
        };
        actSkills[1] = new ActiveSkill
        {

            name = "변이 파리채",
            description = "미구현 스킬입니다.",
            level = 1,
            necessaryMana = 30,
            cooldownTime = 10
        };
        actSkills[2] = new ActiveSkill
        {

            name = "잭프로스트 빙수",
            description = "미구현 스킬입니다.",
            level = 1,
            necessaryMana = 30,
            cooldownTime = 10
        };
        actSkills[3] = new ActiveSkill
        {

            name = "잭 오 할로윈",
            description = "사용할 경우, 잭오랜턴 타일들 중 랜덤하게 잭오할로윈 타일로 바뀝니다. \n" +
                          "잭오할로윈 타일을 클릭하면, 그 타일을 중심으로 3X3 범위 내의 타일이 파괴됩니다.",
            level = 1,
            necessaryMana = 75,
            cooldownTime = 30
        };


        for (int i = 0; i < actSkills.Length; i++)
        {
            saveData.AskillList.Add(actSkills[i]);
        }
    }

    private void SetPassiveSkill()
    {
        pasSkills[0] = new PassiveSkill
        {
            name = "고대의 도서관",
            description = "처음 시작 시, 주어지는 제한 시간을 소폭 상승 시킨다.",
            level = 1
        };
        pasSkills[1] = new PassiveSkill
        {
            name = "쇼타임",
            description = "제한 시간이 30초 이하로 남았을 경우 발동. 모든 점수 획득량이 증가한다.",
            level = 1
        };
        pasSkills[2] = new PassiveSkill
        {
            name = "현자의 돌",
            description = "퍼즐을 완성할 때 마다, 획득하는 점수가 상승한다.",
            level = 1
        };
        pasSkills[3] = new PassiveSkill
        {
            name = "붉은 사탕",
            description = "스위트 캔디바 타일의 등장 확률이 상승하고, 스위트 캔디바 타일과 매칭 시 점수 획득량이 크게 상승한다.",
            level = 1
        };

        for (int i = 0; i < pasSkills.Length; i++)
        {
            saveData.PskillList.Add(pasSkills[i]);
        }
    }
}