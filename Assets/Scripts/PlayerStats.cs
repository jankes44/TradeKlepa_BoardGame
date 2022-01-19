using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject PlayerNameText;
    public PhotonView PV;
    private GameControl2 gameControl;
    public GameObject PlayerAvatar;

    public string playerName;
    public int actorNo;
    public int waypointIndex = 0;
    public int targetWaypointIndex = 0;
    public float moveSpeed = 2.1f;
    public bool moveAllowed = false;
    public bool isMoving = false;

    Animator animator;
    Transform gosciu;

    int isWalkingHash;
    public bool isLocal;
    public Transform follow;
    public Transform lookat;
    public bool YourTurnStarted = false;
    int stop;

    //STATS
    public int level = 1;

    public int maxHP = 100;
	public float agility = 10f;
	public float strength = 10f;
	public float vitality = 10f;
    public int damage = 1;

    //DEBUFFS ----------
    public int rollDebuffCount = 0;
    public bool skipTurnDebuff = false;

    bool collided = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>();
        gameControl = GameControl2.instance;
        actorNo = PV.Owner.ActorNumber;
        gameControl.playersList = GameObject.FindGameObjectsWithTag("Player").OrderBy(go => go.GetComponent<PlayerStats>().actorNo).ToArray();
        Vector3 startPos = new Vector3(gameControl.startPosX, gameControl.startPosY, gameControl.startPosZ);
        playerName = GetComponent<PhotonView>().Owner.NickName;
        isLocal = PV.IsMine || !PhotonNetwork.IsConnected;
        gosciu = transform.Find("giga_chad");
        animator = gosciu.GetComponent<Animator>();
        Debug.Log(animator);
        isWalkingHash = Animator.StringToHash("isWalking");

        if (PhotonNetwork.IsMasterClient && isLocal)
        {
            Debug.Log("Only master");

            string uid = System.Guid.NewGuid().ToString();

            PV.RPC("RPC_CreateEvent", RpcTarget.AllBuffered, uid, 1, 1);
            PV.RPC("RPC_CreateEvent", RpcTarget.AllBuffered, uid, 230, -1); //creates the shop
            StartCoroutine("FirstTurn");
        }
    }

    void Update()
    {
        if (GetComponent<PhotonView>().Owner != null && PlayerNameText && gameControl.isActiveAndEnabled)
        {
            string nickName = GetComponent<PhotonView>().Owner.NickName;
            PlayerNameText.transform.LookAt(Camera.main.transform.position);
            PlayerNameText.transform.Rotate(0, 180, 0);
            PlayerNameText.GetComponent<TextMesh>().text = nickName;
            playerName = nickName;
            if (moveAllowed && PV.IsMine && !YourTurnStarted)
            {
                YourTurnStarted = true;
                StartCoroutine("YourTurn");
            }
        }
        

    }

    GameObject FindPlayer(int actorNo) {
        return gameControl.playersList.Where(pl => pl.GetComponent<PlayerStats>().actorNo == actorNo).SingleOrDefault();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    public void animationToggle(bool toggle)
    {
        animator.SetBool(isWalkingHash, toggle);
    }

    private IEnumerator YourTurn()
    {
        if (skipTurnDebuff)
        {
            Debug.Log("Sorry, skip...");
            skipTurnDebuff = false;
            gameControl.ChangeTurn();
        } else
        {
            Debug.Log("Your turn started!");
            gameControl.ToggleRollDiceBtn(true);
        }
        yield return null;
    }

    private IEnumerator FirstTurn()
    {
        yield return new WaitForSeconds(2);
        SyncTurnMaster(-1);
        yield return null;
    }

    public void Heal(int healthGain)
    {
        Debug.Log($"healed {healthGain} hp");
    }

    //TURN SYNC ----------------------
    public void SyncTurnMaster(int current)
    {
        if (PV.IsMine)
        {
            if (current == gameControl.playerCount - 1)
            {
                current = 0;
            }
            else
            {
                current++;
            }
            if (current == 0) CreateEvent();

            string nextPlayerName = gameControl.playersList[current].GetComponent<PlayerStats>().playerName;
            Debug.Log(nextPlayerName + " " + current + " " + gameControl.playerCount);

            PV.RPC("SyncTurnRPC", RpcTarget.AllBuffered, current, nextPlayerName);
        }
    }

    [PunRPC]
    public void SyncTurnRPC(int whosTurn, string nextPlayerName)
    {
        gameControl.turn = nextPlayerName;
        gameControl.turnIndex = whosTurn;
        gameControl.playersList[whosTurn].GetComponent<PlayerStats>().moveAllowed = true;
        gameControl.CurrentPlayerTxt.text = nextPlayerName + "'s turn";
        gameControl.freelook.GetComponent<CinemachineFreeLook>().Follow = gameControl.playersList[whosTurn].GetComponent<PlayerStats>().follow;
        gameControl.freelook.GetComponent<CinemachineFreeLook>().LookAt = gameControl.playersList[whosTurn].GetComponent<PlayerStats>().lookat;
        gameControl.currentPlayer = gameControl.playersList[gameControl.turnIndex].GetComponent<PlayerStats>();
        // gameControl.ToggleKostka(true);
        gameControl.RollTheDice();
        //int rand = Random.Range(0, 2);
        //if (rand == 0)
        //{
        //    gameControl.DayAndNight("day");
        //} else if (rand == 1)
        //{
        //    gameControl.DayAndNight("night");
        //}
        Debug.Log("Synchronised whosTurn " + nextPlayerName);
    }

    //DICE ROLL ------------------
    public void RollTheDice()
    {
        if (PV.IsMine) {
            int rand = Random.Range(1, 7);
            if (rollDebuffCount > 0)
            {
                rand = 1;
                rollDebuffCount--;
            }
            PV.RPC("RPC_RollTheDice", RpcTarget.AllBuffered, rand);
        }
    }

    [PunRPC]
    public void RPC_RollTheDice(int rolled)
    {
        StartCoroutine(Roll(rolled));
        Debug.Log("RPC received, rolled: " + rolled);
    }

    public void StopDice() {
        gameControl.ToggleSkipTurnBtn(true);
        gameControl.ToggleRollDiceBtn(false);
        PV.RPC("RPC_StopDice", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_StopDice()
    {
        stop = 1;
        Debug.Log("RPC received, stopped the dice");
    }

    IEnumerator Roll(int rolled)
    {
        int rand;
        stop = 0;
        gameControl.imageRenderer.gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        
        while (stop == 0) {
            rand = Random.Range(1, 7);
            gameControl.imageRenderer.sprite = gameControl.kostkas[rand - 1];
            gameControl.imageRenderer.gameObject.transform.Rotate(0.0f, 0.0f, 38f+rand, Space.Self);
            yield return new WaitForSeconds(0.05f);
        }

        gameControl.imageRenderer.sprite = gameControl.kostkas[rolled - 1];

        gameObject.GetComponent<GraphwayTest>().steps = rolled;
        Debug.Log(rolled);

        yield return new WaitForSeconds(1f);
        // gameControl.ToggleKostka(false);

        yield return null;
    }

    //COMBAT SYSTEM
    public void OnAttack(string attackType) {
        int chance = Random.Range(1, 100);
        int chanceEnemy = Random.Range(1, 100);
		int typeOfAttackEnemy = Random.Range(1,4);

        PV.RPC("RPC_OnAttack", RpcTarget.AllBuffered, attackType, chance, chanceEnemy, typeOfAttackEnemy);
    }

	[PunRPC]
    public void RPC_OnAttack(string attackType, int chance, int chanceEnemy, int typeOfAttackEnemy) {
        BattleSystem BS = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        BS.StartCoroutine(BS.PlayerAttack(attackType, chance, chanceEnemy, typeOfAttackEnemy));
		print("Attack " + $"{attackType}, chance {chance}, chanceEnemy {chanceEnemy}, attackEnemy {typeOfAttackEnemy}");

    }

    public void OnHeal() {
        int chanceEnemy = Random.Range(1, 100);
		int typeOfAttackEnemy = Random.Range(1,4);
        PV.RPC("RPC_Heal", RpcTarget.AllBuffered, chanceEnemy, typeOfAttackEnemy);
    }

	[PunRPC]
    public void RPC_Heal(int chanceEnemy, int typeOfAttackEnemy) {
		print("Heal");
        BattleSystem BS = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        BS.StartCoroutine(BS.PlayerHeal(chanceEnemy, typeOfAttackEnemy));
    }

    public void OpenChest(int item) {
        PV.RPC("RPC_OpenChest", RpcTarget.AllBuffered, item);
    }

	[PunRPC]
    public void RPC_OpenChest(int item) {
        GameControl2.instance.battleSystem.chest.OpenChest(item);
    }

    //EVENTS ------------------
    public void CreateEvent()
    {
        int randPlace = Random.Range(0, gameControl.eventControl.unitList.Length);
        int randEvent = Random.Range(0, gameControl.eventControl.eventList.Length);
        string uid = System.Guid.NewGuid().ToString();

        PV.RPC("RPC_CreateEvent", RpcTarget.AllBuffered, uid, randPlace, randEvent);
    }

    [PunRPC]
    public void RPC_CreateEvent(string uid, int randPlace, int randEvent)
    {
        Debug.Log("RPC received, spawning event");
        gameControl.eventControl.AddEvent(uid, randPlace, randEvent);
    }

    public void EnterEvent(string eventID, int eventIndex)
    {
        PV.RPC("RPC_EnterEvent", RpcTarget.AllBuffered, eventID, eventIndex);
    }

    [PunRPC]
    public void RPC_EnterEvent(string eventID, int eventIndex)
    {
        Debug.Log("RPC received, enter event");
        Debug.Log(eventIndex);
        if (eventIndex == -1) gameControl.eventControl.EventEnter(eventID, gameControl.eventControl.shop);
        else gameControl.eventControl.EventEnter(eventID, gameControl.eventControl.eventList[eventIndex]);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isLocal && gameControl.currentPlayer && actorNo == gameControl.currentPlayer.actorNo)
        {
            if (collided) return;
            if (other.gameObject.GetComponent<EventUnit>() && other.gameObject.GetComponent<EventUnit>().hasEvent == true && !isMoving && gameObject.GetComponent<GraphwayTest>().speed == 0)
            {


                collided = true;
                EnterEvent(other.gameObject.GetComponent<EventUnit>().eventID, other.gameObject.GetComponent<EventUnit>().eventIndex);

                Debug.Log("Event start " + other.gameObject.GetComponent<EventUnit>().eventName);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collided = false;
    }

    public void EventConfirm() {
        if (isLocal) {
            PV.RPC("RPC_EventConfirm", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void RPC_EventConfirm() {
        print("Event confirm RPC");
		EventControl.instance.EventConfirm();
    }

    // EQUIPMENT - EQUIPPING ITEMS SYNC
    public void EquipItem(string newItemName) {
        if (isLocal) {
            PV.RPC("RPC_EquipItem", RpcTarget.Others, newItemName, actorNo);
        }
    }

    [PunRPC]
    public void RPC_EquipItem(string newItemName, int actor) {
        EquipmentManager player = gameControl.playersList.Where(pl => pl.GetComponent<PlayerStats>().actorNo == actorNo).SingleOrDefault().GetComponent<EquipmentManager>();
        
        Equipment newItem = gameControl.ItemList.Where(item => item.name == newItemName).SingleOrDefault();

        player.EquipSync(newItem, player);
    }

    public void UnequipItem(int slotIndex) {
        if (isLocal) {
            PV.RPC("RPC_UnequipItem", RpcTarget.Others, slotIndex, actorNo);
        }
    }

    [PunRPC]
    public void RPC_UnequipItem(int slotIndex, int actor) {
        EquipmentManager player = gameControl.playersList.Where(pl => pl.GetComponent<PlayerStats>().actorNo == actorNo).SingleOrDefault().GetComponent<EquipmentManager>();
        
        player.UnequipSync(slotIndex, player);
    }
}
