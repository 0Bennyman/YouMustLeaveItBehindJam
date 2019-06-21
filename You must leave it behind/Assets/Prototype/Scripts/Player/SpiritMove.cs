using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
public class SpiritMove : MonoBehaviour
{
    public float maxSpeed,speedIncrease,speedDecrease;

    public float Health, maxHealth,damageSpeed,increaseSpeed;

    [HideInInspector]
    public float speed;

    private bool moving;

    [HideInInspector]    
    public bool vulnrable=true;

    public GameObject cam;

    public SpiritSwitch switchSpirit;

    private RaycastHit hit;

    public Image dot;

    public GameObject distSlider;

    public Slider healthSlider;

    private PostProcessVolume postVol;

    public PostProcessProfile normal, inLight;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Wall")
        {
            if (speed > 0)
            {
                speed = 3.5f;
                speed = -speed;
            }
            else
            {
                speed = 3.5f;
                speed = +speed;
            }
        }

        if (collision.tag == "LightArea")
        {
            GameObject light = collision.GetComponent<LightChild>().light;
            collision.gameObject.SetActive(false);
            if (Physics.Raycast(transform.position, (light.transform.position - transform.position), out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Light")
                {
                    Health -= damageSpeed * Time.deltaTime;
                    postVol.profile = inLight;
                }

            }
            collision.gameObject.SetActive(true);


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Light")
        {
            postVol.profile = normal;
        }
    }


    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = Health;

        postVol = cam.GetComponent<PostProcessVolume>();
    }


    GameObject GetClosestObject(GameObject[] obj)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in obj)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }


    public void GetDamaged()
    {
        Health -= damageSpeed * Time.deltaTime;
        postVol.profile = inLight;
    }

    public void ReturnToNormal()
    {
        postVol.profile = normal;
    }

    void CheckHitLight()
    {
        GameObject[] trans = GameObject.FindGameObjectsWithTag("Light");
        GameObject light = GetClosestObject(trans);
        if (Physics.Raycast(transform.position, (light.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Light")
            {
                Health -= damageSpeed * Time.deltaTime;
                postVol.profile = inLight;
            }
            else
            {
                postVol.profile = normal;
            }

        }

    }

    void Update()
    {

        //CheckHitLight();

        moving = false;
        UpdateHealth();
        if (Input.GetKey(KeyCode.W))
        {
            speed += speedIncrease * Time.deltaTime;

            moving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            speed -= speedIncrease * Time.deltaTime;
            moving = true;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                distSlider.SetActive(false);
                vulnrable = false;
                if (hit.transform.tag == "Enemy")
                {
                    switchSpirit.SwitchToPlayerBody(hit.transform.gameObject);
                }
                else
                {
                    switchSpirit.SwitchToPlayerBody(null);
                }
            }
            else
            {
                if (hit.transform.tag == "Enemy" || hit.transform.tag == "Player")
                {
                    dot.color = Color.green;
                    dot.color = new Color(dot.color.r, dot.color.g, dot.color.b, 0.3f);
                }
                else
                {
                    dot.color = Color.red;
                    dot.color = new Color(dot.color.r, dot.color.g, dot.color.b, 0.3f);
                }
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                vulnrable = false;
                distSlider.SetActive(false);
                switchSpirit.SwitchToPlayerBody(null);
            }
        }


        if (moving)
        {
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            else if (speed < -maxSpeed)
            {
                speed = -maxSpeed;
            }
        }

        if (!moving)
        {

            if (speed > 0.2f)
            {
                speed -= speedDecrease * Time.deltaTime;
            }else if (speed < -0.2f)
            {
                speed += speedDecrease * Time.deltaTime;
            }
            else
            {
                speed = 0;
            }
        }


        transform.position += cam.transform.forward * Time.deltaTime * speed;

    }



    void UpdateHealth()
    {
        if (Health < maxHealth)
        {
            Health += damageSpeed / 2 * Time.deltaTime;
            healthSlider.value = Health;
        }
    }


}
