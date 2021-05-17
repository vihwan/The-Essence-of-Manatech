using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelperTest : MonoBehaviour {

    int[] o = new int[] { 1, 2, 3, 4 };

    public enum MathType
    {
        Linear,
        Clerp,
        Spring,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        Bounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        Punch,
    }

    private delegate float MathFunc(float t, float start, float end );

    private Dictionary<MathType, MathFunc> m_Func = new Dictionary<MathType, MathFunc>();

    private Vector3 m_start;
    private Vector3 m_end;
    public float m_elapsedTime = 0;
    public float m_targetTime = 1;
    public float m_duration = 1;

    public MathType m_funcXType;
    public MathType m_funcYType;
    public MathType m_funcZType;

    public bool m_run = false;

    public Transform m_target;
    public float speed;



    // Use this for initialization
    void Start () {
        
        m_Func.Add(MathType.Bounce, MathHelper.Bounce);
        m_Func.Add(MathType.Clerp, MathHelper.Clerp);
        m_Func.Add(MathType.EaseInBack, MathHelper.EaseInBack);
        m_Func.Add(MathType.EaseInCirc, MathHelper.EaseInCirc);
        m_Func.Add(MathType.EaseInCubic, MathHelper.EaseInCubic);
        m_Func.Add(MathType.EaseInExpo, MathHelper.EaseInExpo);
        m_Func.Add(MathType.EaseInOutBack, MathHelper.EaseInOutBack);
        m_Func.Add(MathType.EaseInOutCirc, MathHelper.EaseInOutCirc);
        m_Func.Add(MathType.EaseInOutCubic, MathHelper.EaseInOutCubic);
        m_Func.Add(MathType.EaseInOutExpo, MathHelper.EaseInOutExpo);
        m_Func.Add(MathType.EaseInOutQuad, MathHelper.EaseInOutQuad);
        m_Func.Add(MathType.EaseInOutQuart, MathHelper.EaseInOutQuart);
        m_Func.Add(MathType.EaseInOutQuint, MathHelper.EaseInOutQuint);
        m_Func.Add(MathType.EaseInOutSine, MathHelper.EaseInOutSine);
        m_Func.Add(MathType.EaseInQuad, MathHelper.EaseInQuad);
        m_Func.Add(MathType.EaseInQuart, MathHelper.EaseInQuart);
        m_Func.Add(MathType.EaseInQuint, MathHelper.EaseInQuint);
        m_Func.Add(MathType.EaseInSine, MathHelper.EaseInSine);
        m_Func.Add(MathType.EaseOutBack, MathHelper.EaseOutBack);
        m_Func.Add(MathType.EaseOutCirc, MathHelper.EaseOutCirc);
        m_Func.Add(MathType.EaseOutCubic, MathHelper.EaseOutCubic);
        m_Func.Add(MathType.EaseOutExpo, MathHelper.EaseOutExpo);
        m_Func.Add(MathType.EaseOutQuad, MathHelper.EaseOutQuad);
        m_Func.Add(MathType.EaseOutQuart, MathHelper.EaseOutQuart);
        m_Func.Add(MathType.EaseOutQuint, MathHelper.EaseOutQuint);
        m_Func.Add(MathType.EaseOutSine, MathHelper.EaseOutSine);
        m_Func.Add(MathType.Linear, MathHelper.Linear);
        m_Func.Add(MathType.Spring, MathHelper.Spring);

        m_start = transform.position;

        //m_end = new Vector3(worldPos.x, worldPos.y, 0);
        m_end = m_target.position;
        m_run = true;
        m_elapsedTime = 0;

    }
	
	// Update is called once per frame
	void Update () { 
        if (m_run)
        {
            m_elapsedTime += Time.deltaTime /speed;
            Vector3 pos = Vector3.zero;
            pos.x = m_Func[(MathType)m_funcXType](m_start.x, m_end.x, m_elapsedTime);
            pos.y = m_Func[(MathType)m_funcYType](m_start.y, m_end.y, m_elapsedTime);
            pos.z = m_Func[(MathType)m_funcYType](m_start.y, m_end.y, m_elapsedTime); ;
            transform.position = pos;
            transform.rotation = Quaternion.Euler(0f,0f,pos.z);


            Debug.Log(transform.position);

            if (m_elapsedTime >= m_targetTime)
            {
                pos.x = m_Func[(MathType)m_funcXType]( m_start.x, m_end.x, 1);
                pos.y = m_Func[(MathType)m_funcYType]( m_start.y, m_end.y, 1);
                pos.z = 0;
                //pos.z = m_Func[(MathType)m_funcZType](1, m_start.z, m_end.z);

                m_elapsedTime = 0;
                m_run = false;
            }
        }
    }

}
