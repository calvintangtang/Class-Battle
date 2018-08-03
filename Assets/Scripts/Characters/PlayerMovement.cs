using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum TickDataType
{
    CharacterInput,
    EnemyInput,
    SpawnPlayer,
    SpawnEnemy,
    HealthPack,
    AttackData
}

[System.Serializable]
public class PlayerTickData
{
    public int peerId;
    public int dataType;
    public object data;

}

[System.Serializable]
public class CharacterInputTickData
{
    public int playerInput;
    public string attackName;
    public int damage;
}

[System.Serializable]
public class SpawnPlayerTickData
{
    public int peerId;
    public int playerClass;
    public int weapon;
}

[System.Serializable] 
public class AttackTickData
{
    public bool isAtk1;
    public int damage;
}

public partial class PlayerMovement : MonoBehaviour, ITickRecv
{
    public int peerId;
    public CharacterInfo charInfo;
    public CharacterCombat charCombat;
    public Animator animator;

    protected Joystick joystick;
    public Button attack;
    public Button special;
    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip attack3;
    public AudioClip special1;

    public float atk1Duration = 1;
    public float specialDuration = 2.5f;
    public GameObject atk1Hitbox;

    Rigidbody2D rigidbody;
    public float velocity = 7f;
    public float maxHeight = 3;
    public float minHeight = -4.5f;

    public bool isRight = true;
    public bool isDead = false;
    public bool bPlayer = false;

    public List<InputRecord> inputRecordList = new List<InputRecord>();
    public List<byte[]> tickList = new List<byte[]>();

    public enum InputType {
        None, Spawn, MoveUp, MoveDown, MoveLeft, MoveRight, MoveUpLeft, MoveUpRight, MoveDownLeft, MoveDownRight, Attack, Special
    }

    [System.Serializable]
    public class InputRecord {
        public InputType type;
        public float time;
    }

    void Start() {
        joystick = FindObjectOfType<Joystick>();
        animator = GetComponent<Animator>();
        charCombat = this.GetComponent<CharacterCombat>();
        charCombat.getHurtAction += onHurt;
        charCombat.onDeathAction += death;
        rigidbody = GetComponent<Rigidbody2D>();
        RegisterTickRev();
        prevPosition = transform.position;
        nextPosition = transform.position;
    }

    void Update() {
        if (isDead || !TickManager.Instance.bRunning) {
            return;
        } 

        timer += Time.deltaTime;
        Vector3 beginPosition = transform.position;
        Vector3 finalPos = Vector3.Lerp(prevPosition, nextPosition, Mathf.Clamp01(timer / TickManager.Instance.tickPeriod));
        finalPos.z = (finalPos.y / 7.5f) * 0.2f;
        transform.position = finalPos;
        Vector3 offset = beginPosition - transform.position;

        if (offset != Vector3.zero) {
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

            animator.SetInteger("State", 1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    public void PullInput()
    {
        if (isDead)
        {
            recordAction(InputType.None);
            return;
        }
        else if (bAttack)
        {
            recordAction(InputType.Attack);
        }
        else if (bSpecial)
        {
            recordAction(InputType.Special);
        }
        else if (joystick.Horizontal > 0 && Mathf.Abs(joystick.Vertical) < 0.5f)
        {
            recordAction(InputType.MoveRight);
        }
        else if (joystick.Horizontal < 0 && Mathf.Abs(joystick.Vertical) < 0.5f)
        {
            recordAction(InputType.MoveLeft);
        }
        else if (joystick.Vertical > 0 && Mathf.Abs(joystick.Horizontal) < 0.5f)
        {
            recordAction(InputType.MoveUp);
        }
        else if (joystick.Vertical < 0 && Mathf.Abs(joystick.Horizontal) < 0.5f)
        {
            recordAction(InputType.MoveDown);
        }
        else if (joystick.Vertical > 0.5f && joystick.Horizontal > 0.5f)
        {
            recordAction(InputType.MoveUpRight);
        }
        else if (joystick.Vertical > 0.5f && joystick.Horizontal < -0.5f)
        {
            recordAction(InputType.MoveUpLeft);
        }
        else if (joystick.Vertical < -0.5f && joystick.Horizontal > 0.5f)
        {
            recordAction(InputType.MoveDownRight);
        }
        else if (joystick.Vertical < -0.5f && joystick.Horizontal < -0.5f)
        {
            recordAction(InputType.MoveDownLeft);
        }
        else
        {
            recordAction(InputType.None);
        }
    }

    public void attackButton() {
        if (animator.GetBool("Attacking"))
            return;
        if (isDead)
            return;
        bAttack = true;
    }

    public IEnumerator startAtk1()
    {
        animator.SetTrigger("Attack");
        animator.SetBool("Attacking", true);
        SoundManager.Instance.RandomizeSFX(attack1, attack2, attack3);
        yield return new WaitForSeconds(atk1Duration);
        animator.SetBool("Attacking", false);
    }

    public void specialButton()
    {
        if (charCombat.stats.curMP <= 10) {
            return;
        }
        if (animator.GetBool("Special Attacking"))
            return;
        if (isDead)
            return;
        bSpecial = true;
    }

    public IEnumerator startSpecial()
    {
        charCombat.stats.curMP -= 10;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("Special");
        SoundManager.Instance.PlaySingle(special1);
        animator.SetBool("Special Attacking", true);
        yield return new WaitForSeconds(specialDuration);

        if (charInfo.charClass == ClassType.Mage) {
            foreach (GameObject enemy in BattleSceneControl.Instance.enemyList) {
                if (Vector3.Distance(transform.position, enemy.transform.position) <= 10)
                {
                    enemy.GetComponent<CharacterCombat>().receiveHit(50);
                }
            }
        }
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        animator.SetBool("Special Attacking", false);
    }

    public void cheerAnimation() {
        animator.SetInteger("State", 2);
    }

    public void onHurt() {
        if (isDead || animator.GetBool("Attacking")) {
            return;
        }
        animator.SetTrigger("Hurt");
    }

    public void death() {
        if (isDead) { return; }
        isDead = true;
        animator.SetTrigger("Death");
        Destroy(GetComponent<Rigidbody2D>());
        GetComponent<CapsuleCollider2D>().enabled = false;
        isDead = true;
        charCombat.isDead = true;
        TickManager.Instance.removePlayer(this);
    }

    public void recordAction(InputType type)
    {
        playerInput = type;
    }
}

/***
 * For Tick system
 */
public partial class PlayerMovement
{
    float timer = 0;
    public Vector3 nextPosition;
    public Vector3 prevPosition;
    public InputType tickInput;
    public InputType playerInput;
    public bool bAttack;
    public bool bSpecial;

    public void RegisterTickRev()
    {
        TickManager.Instance.registerTickRecv(this);
    }

    public void RunTick(PlayerTickData tickData = null)
    {
        if (isDead) { return; }
        if (tickData != null)
        {
            if (tickData.dataType == (int)TickDataType.CharacterInput)
            {
                timer = 0;
                prevPosition = transform.position;

                CharacterInputTickData charInput = (CharacterInputTickData)tickData.data;
                tickInput = (InputType)charInput.playerInput;
                switch (tickInput)
                {
                    case InputType.None:
                        nextPosition = transform.position;
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
                    case InputType.Attack:
                        StartCoroutine(startAtk1());
                        break;
                    case InputType.Special:
                        StartCoroutine(startSpecial());
                        CameraControl.Instance.onSpecialAtkFor(specialDuration);
                        break;
                    default:
                        break;
                }
                if (nextPosition.y > maxHeight || nextPosition.y < minHeight)
                {
                    nextPosition = prevPosition;
                }
            }
        }
    }

    public void SendTick()
    {
        if (bPlayer) {
            PullInput();

            PlayerTickData data = new PlayerTickData();
            data.peerId = peerId;
            CharacterInputTickData inputData = new CharacterInputTickData();
            inputData.playerInput = (int)playerInput;
            data.dataType = (int)TickDataType.CharacterInput;
            data.data = inputData;

            if (bAttack)
            {
                inputData.attackName = "normalAttack";
                inputData.damage = 10;
            }


            byte[] byteData = ObjectToByteArray<PlayerTickData>(data);
            tickList.Add(byteData);
            TickManager.Instance.ReceiveTick(byteData);

            resetData();
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

    private void resetData()
    {
        playerInput = InputType.None;
        bAttack = false;
        bSpecial = false;
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
