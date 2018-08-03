using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum EnemyState {
    idle = 0,
    run,
    attack,
    death
}

public enum InputType {
    None, MoveUp, MoveDown, MoveLeft, MoveRight, MoveUpLeft, MoveUpRight, MoveDownLeft, MoveDownRight, Attack, Hurt
}

[System.Serializable]
public class SpawnEnemyTickData
{
    public float xPos;
    public float yPos;
    public float zPos;
}

[System.Serializable]
public class EnemyInputTickData
{
    public int enemyInput;
    public string attackName;
    public int damage;
}

public partial class EnemyMovement : MonoBehaviour, ITickRecv {
    public int peerId;
    public GameObject enemy;
    public GameObject player;
    public float startX;
    public float startY;
    public EnemyState currentState;
    protected Animator animator;
    public AudioClip attack1;
    public AudioClip attack2;

    public int atk1Duration = 0;
    public int atk1DurationMax = 10;
    public GameObject atk1Hitbox;
    public int atkDistance;
    public bool isDead = false;
    public bool isRight = false;

    public Vector3 nextPosition;
    public Vector3 prevPosition;
    public int dropNumber = 4;

    public List<byte[]> tickList = new List<byte[]>();

    void Start () {
        startX = transform.localPosition.x;
        startY = transform.localPosition.y;
        currentState = EnemyState.idle;
        enemy = gameObject;
        animator = this.GetComponent<Animator>();
        this.GetComponent<CharacterCombat>().getHurtAction += onHurt;
        this.GetComponent<CharacterCombat>().onDeathAction += death;
        RegisterTickRev();
        prevPosition = transform.position;
        nextPosition = transform.position;
        BattleSceneControl.Instance.addEnemy(gameObject);
    }

    void Update () {
        if(isDead) {
            return;
        }

        timer += Time.deltaTime;
        Vector3 beginPosition = transform.position;
        Vector3 nextPos = Vector3.Lerp(prevPosition, nextPosition, Mathf.Clamp01(timer / TickManager.Instance.tickPeriod));
        nextPos.z = (nextPos.y / 7.5f) * 0.2f;
        transform.position = nextPos;
        Vector3 offset = beginPosition - transform.position;

        if (offset.magnitude > 0.01f)
        {
            if (offset.x < 0)
            {
                isRight = true;
            }
            else
            {
                isRight = false;
            }

            Vector3 scale = transform.localScale;
            scale.x = isRight ? 1f : -1f;
            transform.localScale = scale;
        }

        if (offset.magnitude > 0.01f && !bAttack)
        {
            currentState = EnemyState.run;
            animator.SetInteger("State", 1);
        }
        else if (offset.magnitude <= 0.01f && !bAttack)
        {
            currentState = EnemyState.idle;
            animator.SetInteger("State", 0);
        }
    }

    public void findTarget() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void PullInput()
    {
        if (isDead) { return; }

        findTarget();
        if (player == null) { return; }
        Vector3 playerPosition = player.transform.position;
        if (player.GetComponent<PlayerMovement>().isDead) {
            enemyInput = InputType.None;
            return;
        }
        if (animator.GetBool("Attack"))
        {
            enemyInput = InputType.None;
            return;
        }
        else if (animator.GetBool("Hurt")) {
            enemyInput = InputType.None;
            return;
        }
        if (Vector3.Distance(transform.position, playerPosition) < atkDistance && Mathf.Abs(transform.position.y - playerPosition.y) <= 0.5f) {
            enemyInput = InputType.Attack;
        }
        else if (Vector3.Distance(transform.position, playerPosition) < 5) {
            if (transform.position.x < playerPosition.x && transform.position.y == playerPosition.y)
            {
                enemyInput = InputType.MoveRight;
            }
            else if (transform.position.x > playerPosition.x && transform.position.y == playerPosition.y)
            {
                enemyInput = InputType.MoveLeft;
            }
            else if (transform.position.y < playerPosition.y && transform.position.x == playerPosition.x)
            {
                enemyInput = InputType.MoveUp;
            }
            else if (transform.position.y > playerPosition.y && transform.position.x == playerPosition.x)
            {
                enemyInput = InputType.MoveDown;
            }
            else if (transform.position.x > playerPosition.x && transform.position.y > playerPosition.y)
            {
                enemyInput = InputType.MoveDownLeft;
            }
            else if (transform.position.x < playerPosition.x && transform.position.y > playerPosition.y)
            {
                enemyInput = InputType.MoveDownRight;
            }
            else if (transform.position.x > playerPosition.x && transform.position.y < playerPosition.y)
            {
                enemyInput = InputType.MoveUpLeft;
            }
            else if (transform.position.x < playerPosition.x && transform.position.y < playerPosition.y)
            {
                enemyInput = InputType.MoveUpRight;
            }
            else
            {
                enemyInput = InputType.None;
            }
        } else {
            enemyInput = InputType.None;
        }
    }

    public IEnumerator startAtk1()
    {
        bAttack = true;
        animator.SetTrigger("Attack");
        animator.SetBool("Attacking", true);
        if (attack1 != null && attack2 != null)
        {
            SoundManager.Instance.RandomizeSFX(attack1, attack2);
        }
        atk1Duration = atk1DurationMax;
        while (atk1Duration > 0) {
            yield return 0;
        }
        animator.SetBool("Attacking", false);
        bAttack = false;
    }

    public void onHurt()
    {
        if (isDead)
        {
            return;
        }
        animator.SetBool("Hurting", true);
        animator.SetTrigger("Hurt");
    }

    public void death()
    {
        animator.SetTrigger("Death");
        isDead = true;
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().enabled = false;
        Destroy(atk1Hitbox);
        StartCoroutine(DropItem());
    }

    public IEnumerator DropItem() {
        yield return new WaitForSeconds(3);
        Debug.Log(TickManager.Instance.Tick);
        Random.InitState(TickManager.Instance.Tick);
        int dropNum = Random.Range(0, dropNumber);
        if (dropNum == 1)
        {
            PlayerTickData itemData = new PlayerTickData();
            itemData.peerId = 10000;
            itemData.dataType = (int)TickDataType.HealthPack;
            HealthPackData hpData = new HealthPackData();
            hpData.xPos = transform.position.x;
            hpData.yPos = transform.position.y;
            itemData.data = hpData;

            byte[] byteData = ObjectToByteArray<PlayerTickData>(itemData);
            tickList.Add(byteData);
            TickManager.Instance.ReceiveTick(byteData);
            Debug.Log("you get an item");
        }
        else {
            Debug.Log("no item for you");
        }
        Destroy(gameObject);
    }
}

/* Tick System */
public partial class EnemyMovement {
    float timer = 0;
    public InputType tickInput;
    public InputType enemyInput;
    public bool bAttack;
    public float velocity = 3f;

    public void RegisterTickRev()
    {
        TickManager.Instance.registerTickRecv(this);
    }

    public void SendTick()
    {
        PullInput();
        //Debug.Log("Next: " + nextPosition.x);
        //Debug.Log("Prev: " + prevPosition.x);
        PlayerTickData data = new PlayerTickData();
        data.peerId = peerId;
        EnemyInputTickData inputData = new EnemyInputTickData();
        inputData.enemyInput = (int)enemyInput;
        data.dataType = (int)TickDataType.EnemyInput;
        data.data = inputData;

        if (bAttack)
        {
            inputData.attackName = "normalAttack";
            inputData.damage = 10;
        }

        byte[] byteData = ObjectToByteArray<PlayerTickData>(data);
        tickList.Add(byteData);
        //Debug.Log(tickList.Count);
        TickManager.Instance.ReceiveTick(byteData);

        resetData();
    }

    public void RunTick(PlayerTickData tickData = null)
    {
        if (isDead) { return; }
        timer = 0;
        prevPosition = transform.position;

        if (atk1Duration > 0) {
            atk1Duration--;
            return;
        }
        if (tickData != null)
        {
            if (tickData.dataType == (int)TickDataType.EnemyInput)
            {
                EnemyInputTickData enemyInput = (EnemyInputTickData)tickData.data;
                tickInput = (InputType)enemyInput.enemyInput;
                switch (tickInput)
                {
                    case InputType.None:
                        break;
                    case InputType.Attack:
                        StartCoroutine(startAtk1());
                        break;
                    case InputType.Hurt:
                        onHurt();
                        break;
                    case InputType.MoveUp:
                        nextPosition += Vector3.up * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveDown:
                        nextPosition += Vector3.down * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveLeft:
                        nextPosition += Vector3.left * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveRight:
                        nextPosition += Vector3.right * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveUpLeft:
                        nextPosition += (Vector3.up + Vector3.left) * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveUpRight:
                        nextPosition += (Vector3.up + Vector3.right) * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveDownLeft:
                        nextPosition += (Vector3.down + Vector3.left) * velocity * TickManager.Instance.tickPeriod;
                        break;
                    case InputType.MoveDownRight:
                        nextPosition += (Vector3.down + Vector3.right) * velocity * TickManager.Instance.tickPeriod;
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public void UpdateTick(List<byte[]> tickDataList)
    {
        PlayerTickData receivedData = null;

        foreach (var tickByte in tickDataList)
        {
            PlayerTickData data = ByteArrayToObject<PlayerTickData>(tickByte);
            if (data.peerId == peerId)
            {
                receivedData = data;
            }
        }
        RunTick(receivedData);
        SendTick();
    }

    public void resetData()
    {
        enemyInput = InputType.None;
        bAttack = false;
    }

    // Convert an object to a byte array
    private byte[] ObjectToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    private T ByteArrayToObject<T>(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        T obj = (T)binForm.Deserialize(memStream);

        return obj;
    }
}
