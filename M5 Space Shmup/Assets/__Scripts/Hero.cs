using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    // Singleton instance
    static public Hero S;

    [Header("Set in Inspector")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    private GameObject lastTriggerGo = null;

    void Awake()
    {
        if (S == null)
        {
            S = this; // Set the Singleton
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

    void Update()
    {

        // Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
      
        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if ( Input.GetKeyDown( KeyCode.Space))
        {
            TempFire();
        }
    }

    void TempFire ()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        rigidB.linearVelocity = Vector3.up * projectileSpeed;
    }


    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        // print("Triggered: " + go.name); // Debug print if needed

        // Make sure it's not the same triggering GameObject as last time
        if (go == lastTriggerGo)
        {
            return;
        }

        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            // If the shield was triggered by an enemy
            shieldLevel--;           // Decrease the level of the shield by 1
            Destroy(go);             // Destroy the enemy
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }
    public float shieldLevel
    {
        get { return _shieldLevel; }
        set { _shieldLevel = Mathf.Min(value, 4);
        if ( value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart( gameRestartDelay );
            }
        }
    }

}
