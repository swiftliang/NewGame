using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    public GameObject arrow;
    public Transform pointShoot;
    [Range(0,2)]
    public float speed;

    private Vector3 direction;
    private bool canShoot;
    private Animator ani;
    
	// Use this for initialization
	void Start () {
        ani = GetComponent<Animator>();
        ani.speed = speed;
	}

    // Update is called once per frame
    void Update()
    {
        bool checkUI;
#if UNITY_EDITOR
        checkUI = EventSystem.current.IsPointerOverGameObject();
#endif

#if (UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR)
        checkUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        if (Input.GetMouseButton(0) && !checkUI)
        {

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            direction = pointShoot.transform.position - touchPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pointShoot.eulerAngles = new Vector3(0, 0, angle + 90);
            canShoot = true;
            if (ani != null)
            {
                ani.SetBool("attack", true);
            }
        }

    }

    public void attack()
    {
        SoundManager.instance.playSoundShoot();
        GameObject arrow = GenManager.currentPool.GetArrow();
        arrow.transform.eulerAngles = pointShoot.eulerAngles;
        arrow.transform.position = pointShoot.position;
        arrow.SetActive(true);
    }

    public bool CanShoot
    {
        set
        {
            canShoot = value;
        }
        get
        {
            return canShoot;
        }
    }
}
