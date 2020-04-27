using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
using System.Linq;

public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance = null;

    [SerializeField] private PhotonView photonView;

    [SerializeField] private string gamePlayerPrefabName;
    [SerializeField] private CardController cardPrefab;

    //Card Dealer
    [SerializeField] private CardDealer cardDealer;

    //All Players
    public List<PlayerController> playerList = new List<PlayerController>();
    public List<PhotonView> playerPhotonViewList = new List<PhotonView>();

    //Last Card
    public CardController lastCardController = null;

    //Center Table
    [SerializeField] private Transform centerTableTransform;

    public Transform myPlayerHolder;
    public Transform[] otherPlayerHolders;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }

        photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        LobbyCanvasManager.onGameStartClick += StartGame;
    }

    private void OnDisable()
    {
        LobbyCanvasManager.onGameStartClick -= StartGame;
    }

    private void Start()
    {
        //InitCard Dealer
        cardDealer.Init();
    }

    public void StartGame()
    {
        //Spawn Game Players using RPC
        photonView.RPC("SpawnGamePlayer_RPC", RpcTarget.AllBufferedViaServer, new object[] { });

        StartCoroutine(StuffThattMasterClientDoes());
    }

    private IEnumerator StuffThattMasterClientDoes()
    {
        yield return new WaitForSecondsRealtime(2f);
        //Stuff to do from Master Client only..
        if (NetworkManager.Instance.IsMasterClient())
        {
            //Shuffle Cards
            Shuffle(cardDealer.cardsDeck);

            //Update Shuffled Cards on all Clients via RPC
            string shuffledCardsJson = JsonConvert.SerializeObject(cardDealer.cardsDeck);
            photonView.RPC("UpdatedShuffledCards_RPC", RpcTarget.AllBufferedViaServer, new object[] { shuffledCardsJson });

            Dictionary<int, List<int>> viewIDtoCardIDList = new Dictionary<int, List<int>>();

            int playerCount = NetworkManager.Instance.GetPlayerCount();
            for (int i = 0; i < playerList.Count; i++)
            {
                PhotonView pv = playerList[i].GetComponent<PhotonView>();

                List<int> pickedCardIDList = new List<int>();
                PickCards(7, out pickedCardIDList);
                viewIDtoCardIDList.Add(pv.ViewID, pickedCardIDList);
            }

            CardModel firstCard = GetFirstCard();

            string selectedCardDictionary = JsonConvert.SerializeObject(viewIDtoCardIDList);
            int firstCardID = firstCard.cardID;

            Debug.Log("selectedCardDictionary " + viewIDtoCardIDList.Count);
            Debug.Log("selectedCardDictionary " + selectedCardDictionary);
            photonView.RPC("SetCards_RPC", RpcTarget.AllBufferedViaServer, new object[] { selectedCardDictionary, firstCardID });
        }
    }




    #region Some Game Related Functions..

    public static void Shuffle<T>(IList<T> list)
    {
        System.Random random = new System.Random();
        int count = list.Count;
        while (count > 1)
        {
            --count;
            int index = random.Next(count + 1);
            T obj = list[index];
            list[index] = list[count];
            list[count] = obj;
        }
    }

    private void PickCards(int count, out List<CardModel> pickedCards)
    {
        List<CardModel> cardsDeck = cardDealer.cardsDeck;
        pickedCards = new List<CardModel>();
        for (int index = 0; index < count; ++index)
        {
            pickedCards.Add(cardsDeck[0]);
            cardsDeck.RemoveAt(0);
        }
    }

    private void PickCards(int count, out List<int> pickedCardsIDlIst)
    {
        List<CardModel> cardsDeck = cardDealer.cardsDeck;
        pickedCardsIDlIst = new List<int>();
        for (int index = 0; index < count; ++index)
        {
            pickedCardsIDlIst.Add(cardsDeck[0].cardID);
            cardsDeck.RemoveAt(0);
        }
    }

    public CardController SpawnCardFromCardModel(CardModel cardModel)
    {
        CardController cardController = UnityEngine.Object.Instantiate<CardController>(cardPrefab, this.transform);
        cardController.cardModel = cardModel;
        cardController.gameObject.AddComponent<SpriteRenderer>().sprite = this.cardDealer.GetCardImageFromCardName(cardModel.cardName);
        return cardController;
    }

    private CardModel GetFirstCard()
    {
        CardModel cardModel = null;
        List<CardModel> cardsDeck = cardDealer.cardsDeck;

        for (int i = 0; i < cardsDeck.Count; ++i)
        {
            if (!cardsDeck[i].IsSpecial && !cardsDeck[i].IsWild)
            {
                cardModel = cardsDeck[i];
                break;
            }
        }

        return cardModel;
    }

    private void PlayFirstCard(CardModel firstCardModel)
    {
        CardController cardController = SpawnCardFromCardModel(firstCardModel);
        cardController.transform.SetParent(centerTableTransform);
        cardController.transform.localPosition = new Vector3();

        lastCardController = cardController;
    }

    public void CardPlayed(CardController cardPlayed)
    {
        //SOME TURN LOGIC

        int viewID = cardPlayed.playerController.GetComponent<PhotonView>().ViewID;
        photonView.RPC("CardPlayed_RPC", RpcTarget.AllBufferedViaServer, new object[] { cardPlayed.cardModel.cardID, viewID });
    }

    #endregion



    #region RPC's

    [PunRPC]
    public void SpawnGamePlayer_RPC()
    {
        PhotonNetwork.Instantiate(gamePlayerPrefabName, new Vector3(), Quaternion.identity);

        LobbyCanvasManager.hideLobbyUI?.Invoke();
    }

    [PunRPC]
    public void UpdatedShuffledCards_RPC(string shuffledCardJson)
    {
        List<CardModel> shuffledCardList = JsonConvert.DeserializeObject<List<CardModel>>(shuffledCardJson);

        cardDealer.cardsDeck = shuffledCardList;
    }

    [PunRPC]
    public void SetCards_RPC(string selectedCardsDictionary, int firstCardID)
    {
        Dictionary<int, List<int>> viewIDtoCardIDList = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(selectedCardsDictionary);

        for (int i = 0; i < playerList.Count; i++)
        {
            PhotonView pv = playerList[i].GetComponent<PhotonView>();

            List<int> cardIDList = viewIDtoCardIDList[pv.ViewID];
            List<CardModel> cardModelList = new List<CardModel>();

            for (int j = 0; j < cardIDList.Count; j++)
            {
                CardModel cardModel = cardDealer.cardsDeck.Find(x => x.cardID == cardIDList[j]);
                cardModelList.Add(cardModel);
            }

            playerList[i].SetInitialCards(cardModelList);
        }

        PlayFirstCard(cardDealer.cardsDeck.Find(x => x.cardID == firstCardID));
    }

    [PunRPC]
    public void CardPlayed_RPC(int playedCardID, int viewID)
    {
        CardModel playedCardModel = cardDealer.cardsDeck.Find(x => x.cardID == playedCardID);
        CardController cardController = SpawnCardFromCardModel(playedCardModel);

        PhotonView pv = playerPhotonViewList.Find(x => x.ViewID == viewID);

        cardController.transform.SetParent(pv.transform);

        LeanTween.move(cardController.gameObject, centerTableTransform.position, .25f).setEaseInCubic();

        lastCardController = cardController;
    }

    #endregion
}
