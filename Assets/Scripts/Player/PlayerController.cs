using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerModel playerModel;
    public PlayerView playerView;
    public PhotonView myPhotonView;

    public static int myActorNumber = 0;

    private void Awake()
    {
        playerModel = new PlayerModel();
        playerView = GetComponent<PlayerView>();
        myPhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        Debug.Log("PHOTON Actor Number: " + myPhotonView.OwnerActorNr);
        if(myPhotonView.IsMine)
        {
            //This is my View then lets setup Player Names...
            transform.SetParent(_GameManager.Instance.myPlayerHolder);
            myActorNumber = myPhotonView.OwnerActorNr;
        }

        else
        {
            int index = myPhotonView.OwnerActorNr - myActorNumber;
            if(index < 0)
            {
                index = NetworkManager.Instance.GetPlayerCount() - Mathf.Abs(index);
            }
            transform.SetParent(_GameManager.Instance.otherPlayerHolders[index - 1]);
        }

        transform.localPosition = new Vector3();
        playerView.SetPlayerData(myPhotonView.Owner.NickName);
    }

    public void Init(string playerID, string playerName)
    {
        playerModel.playerID = playerID;
        playerModel.playerName = playerName;
        playerView.UpdatePlayerName(playerModel.playerName);
    }

    //public void SetInitialCards(List<CardController> cards)
    //{
    //    playerModel.CardList = cards;
    //    playerView.DisplayCards(cards);
    //}

    //public void SetInitialCards(List<CardModel> cards)
    //{
    //    List<CardController> cardControllerList = new List<CardController>();

    //    for (int i = 0; i < cards.Count; i++)
    //    {
    //        CardController cardController = Multi_GameManager.Instance.SpawnCardFromCardModel(cards[i]);
    //        cardControllerList.Add(cardController);
    //    }

    //    playerModel.CardList = cardControllerList;
    //    playerView.DisplayCards(cardControllerList);
    //}

    //public void AssignNewCard(CardController pickedNewCard)
    //{
    //    playerModel.CardList.Add(pickedNewCard);
    //    LeanTween.move(pickedNewCard.gameObject, this.transform.position, .5f).setEaseOutBounce().setOnComplete(() => 
    //    {
    //        playerView.DisplayCards(playerModel.CardList);
    //    });
    //}

    //public void AssignNewCard(CardModel pickedNewCard)
    //{
    //    CardController cardController = Multi_GameManager.Instance.SpawnCardFromCardModel(pickedNewCard);

    //    playerModel.CardList.Add(cardController);
    //    LeanTween.move(cardController.gameObject, this.transform.position, .5f).setEaseOutBounce().setOnComplete(() =>
    //    {
    //        playerView.DisplayCards(playerModel.CardList);
    //    });
    //}
}
