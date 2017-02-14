using UnityEngine;
using XInputDotNetPure;
public class LaneScript : MonoBehaviour
{
    public int Index;
    public float Direction;
    public PortalScript EntracePortal;
    public GameObject Explosion;
    public Texture MainColor1;
    public Texture MainColor2;
    public Texture MainColor3;
    public Texture MainColor4;
    public Texture EndColor1;
    public Texture EndColor2;
    public Texture EndColor3;
    public Texture EndColor4;
    public Texture Health1;
    public Texture Health2;
    public Texture Health3;
    public Texture Health4;
    public Texture HealthBackground1;
    public Texture HealthBackground2;
    public Texture HealthBackground3;
    public Texture HealthBackground4;

    private float elapsedTime = 0f;
    private float vibDuration = 0.5f;
    private bool vibing = false;  
    private GameObject plane;
    private DamageAbleObject damageScript;

    void Start()
    {
        damageScript = GetComponent<DamageAbleObject>();
        damageScript.OnDeath += DamageScript_OnDeath;
        damageScript.OnReceiveDamage += DamageScript_OnReceiveDamage;

        SetupLaneColor();
    }

    private void SetupLaneColor()
    {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        foreach (Renderer item in renderer)
        {
            if (item.name == "healthbar")
                plane = item.gameObject;

            if (Index == 0)
            {
                if (item.name == "Line")
                    item.material.SetTexture("_MainTex", MainColor1);
                else if (item.name == "LineEnd")
                    item.material.SetTexture("_MainTex", EndColor1);
                else if (item.name == "healthbar")
                    item.material.SetTexture("_MainTex", Health1);
                else if (item.name == "healthbarBackground")
                    item.material.SetTexture("_MainTex", HealthBackground1);
            }
            else if (Index == 1)
            {
                if (item.name == "Line")
                    item.material.SetTexture("_MainTex", MainColor2);
                else if (item.name == "LineEnd")
                    item.material.SetTexture("_MainTex", EndColor2);
                else if (item.name == "healthbar")
                    item.material.SetTexture("_MainTex", Health2);
                else if (item.name == "healthbarBackground")
                    item.material.SetTexture("_MainTex", HealthBackground2);
            }
            else if (Index == 2)
            {
                if (item.name == "Line")
                    item.material.SetTexture("_MainTex", MainColor3);
                else if (item.name == "LineEnd")
                    item.material.SetTexture("_MainTex", EndColor3);
                else if (item.name == "healthbar")
                    item.material.SetTexture("_MainTex", Health3);
                else if (item.name == "healthbarBackground")
                    item.material.SetTexture("_MainTex", HealthBackground3);
            }
            else if (Index == 3)
            {
                if (item.name == "Line")
                    item.material.SetTexture("_MainTex", MainColor4);
                else if (item.name == "LineEnd")
                    item.material.SetTexture("_MainTex", EndColor4);
                else if (item.name == "healthbar")
                    item.material.SetTexture("_MainTex", Health4);
                else if (item.name == "healthbarBackground")
                    item.material.SetTexture("_MainTex", HealthBackground4);
            }
        }
    }

    private void DamageScript_OnReceiveDamage(object sender, Assets.Scripts.OnHealthChangedArgs e)
    {
        GamePad.SetVibration((PlayerIndex)Index, 1, 1);
        plane.transform.position += new Vector3(2.8f * (20 / damageScript.MaxHealth), 0, 0) * Direction;
        vibing = true;
        elapsedTime = 0f;
    }

    private void DamageScript_OnDeath(object sender, System.EventArgs e)
    {
        int startValue = 24;
        int steps = 4;
        for (int i = 0; i < 12; i++)
        {
            Instantiate(Explosion, new Vector3(transform.position.x - (startValue - (steps * i)), transform.position.y + 1, transform.position.z), Quaternion.identity);
        }
        
        GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
        foreach (GameObject item in drones)
        {
            Enemy enemy = item.GetComponent<Enemy>();
            if (enemy != null && enemy.CurrentLane == this)
                Destroy(item);
        }

        GamePad.SetVibration((PlayerIndex)Index, 0, 0);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (vibing)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > vibDuration)
            {
                elapsedTime = 0f;
                vibing = false;
                GamePad.SetVibration((PlayerIndex)Index, 0, 0);
            }
        }
    }
}
