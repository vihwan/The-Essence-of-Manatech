using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SaveData
{
    public List<ActiveSkill> skillList = new List<ActiveSkill>();
}

public class SaveAndLoad : MonoBehaviour
{
    private string SAVE_DATA_DIRECTORY;  // 저장할 폴더 경로
    private string SAVE_FILENAME = "/SkillData.txt"; // 파일 이름

    private SaveData saveData = new SaveData();
    public ActiveSkill[] actSkills = new ActiveSkill[4];

    //Save Skill Data
    private void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/8.GameData/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY)) // 해당 경로가 존재하지 않는다면
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY); // 폴더 생성(경로 생성)

        //skill 안에 있는 변수
        //name, description, icon, level, necessaryMana, cooldownTime
        //image타입도 저장이 될까? 안되네 역시

        actSkills[0].name = "체인 플로레";
        actSkills[0].description = "사용할 경우, 옮길 수 있는 타일의 위치를 알려줍니다.";
        actSkills[0].level = 1;
        actSkills[0].necessaryMana = 30;
        actSkills[0].cooldownTime = 10;

        actSkills[1].name = "변이 파리채";
        actSkills[1].description = "미구현 스킬입니다.";
        actSkills[1].level = 1;
        actSkills[1].necessaryMana = 30;
        actSkills[1].cooldownTime = 10;

        actSkills[2].name = "잭프로스트 빙수";
        actSkills[2].description = "미구현 스킬입니다.";
        actSkills[2].level = 1;
        actSkills[2].necessaryMana = 30;
        actSkills[2].cooldownTime = 10;

        actSkills[3].name = "잭 오 할로윈";
        actSkills[3].description = "사용할 경우, 잭오랜턴 타일들 중 랜덤하게 잭오할로윈 타일로 바뀝니다. \n" +
                          "잭오할로윈 타일을 클릭하면, 그 타일을 중심으로 3X3 범위 내의 타일이 파괴됩니다.";
        actSkills[3].level = 1;
        actSkills[3].necessaryMana = 75;
        actSkills[3].cooldownTime = 30;

        for (int i = 0; i < 4; i++)
        {
            saveData.skillList.Add(actSkills[i]);
        }

        //Json직렬화
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    public void LoadData(List<ActiveSkill> skillList)
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            // 전체 읽어오기
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            //Json역직렬화
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            for (int i = 0; i < saveData.skillList.Count; i++)
            {
                skillList[i] = saveData.skillList[i];
            }

            Debug.Log("스킬 데이터 로드 완료");
        }
        else
            Debug.Log("스킬 데이터 파일이 없습니다.");
    }
}