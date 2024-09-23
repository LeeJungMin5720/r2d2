using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 7f;
    public float jumpspeed = 15f;
    public Collider2D bottomCollider;
    public CompositeCollider2D terrainCollider;
    
    float vx = 0;
    float preVx = 0;
    bool isGrounded;
    Vector2 originPosition;
    float lastShoot;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.position;
    }

    public void Restart()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<BoxCollider2D>().enabled = true;

        transform.eulerAngles = Vector3.zero;
        transform.position = originPosition;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        vx = Input.GetAxisRaw("Horizontal");

        if(vx < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        if (vx > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if (bottomCollider.IsTouching(terrainCollider)) //지금땅에 붙어 있는가
        {  //예
            if (!isGrounded) 
            { //그 전에는 안 붙어 있었는가
                //착지
                if (vx == 0)
                {
                    GetComponent<Animator>().SetTrigger("Idle");
                }
                else
                {
                    GetComponent<Animator>().SetTrigger("Run");
                }
            }
            else
            {  //그 전에는 붙어 있었는가
                //계속 걷는 중
                if(preVx != 0)
                {
                    if(vx == 0)
                    {
                        GetComponent<Animator>().SetTrigger("Idle");
                    }
                    else
                    {
                        GetComponent<Animator>().SetTrigger("Run");
                    }
                }
            }
            isGrounded = true;
        }
        else
        { //땅에 안 붙어 있다
            if (isGrounded)  //전에는 붙어있는가
            { //점프시작
                GetComponent<Animator>().SetTrigger("Jump");
            }
            isGrounded = false;
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpspeed;
        }

        preVx = vx;

        //총알 발사
        if (Input.GetButtonDown("Fire1") && lastShoot + 0.5f < Time.time)
        {
            Vector2 bulletV = Vector2.zero;
            if (GetComponent<SpriteRenderer>().flipX)
            {
                bulletV = new Vector2(-10, 0);
            }   
            else
            {
                bulletV = new Vector2(10, 0);
            }

            GameObject bullet = ObjectPool.Instance.Getbullet();
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().velocity = bulletV;
            lastShoot = Time.time;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * vx * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Die();
        }


    }
    void Die()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GetComponent<Rigidbody2D>().angularVelocity = 720;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0,10),ForceMode2D.Impulse);
        GetComponent<BoxCollider2D>().enabled = false;

        GameManager.Instance.Die();
    }


}
