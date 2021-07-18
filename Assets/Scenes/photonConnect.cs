using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine.UI;

/* this project is under devloping but you use push to talk feature for this project,
 * i am student i do not use for com. perpose.
 * without editer setup this script not usefull for you.
 * enter your photon appid and voice appid 
 */

public class photonConnect : MonoBehaviourPunCallbacks
{


    public Recorder recorder;
    public Text _regionText;

    private VoiceConnection voiceConnection;
    [SerializeField] private AudioClip audioClipLocal;

    private void Awake()
    {
        //connect to photon
        PhotonNetwork.ConnectUsingSettings();

        //get voice connection object
        voiceConnection = GetComponent<VoiceConnection>();
        
    }

    public void btn_JoinRoom()
    {
        // join random room
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// create test room
    /// </summary>
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;


        PhotonNetwork.CreateRoom("test", roomOptions);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        // if random join failed so create test room 
        CreateRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogError("connected to the master ");

        // player region name
        _regionText.text = PhotonNetwork.CloudRegion;
        
        // init the recorder and use voice connection as param
        recorder.Init(voiceConnection);

        // join lobby for show the avelible rooms
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var item in roomList)
        {
            Debug.LogError("room name is - > "+item.Name);
        }
    }

    public void btn_StartRecoding()
    {
        //start recording
        recorder.StartRecording();
        
        //start transmit
        recorder.TransmitEnabled = true;

    }

    public void btn_StopRecoding()
    {
        //stop recording
        recorder.StopRecording();
        
        //stop transmit 
        recorder.TransmitEnabled = false;
    }


    /// <summary>
    /// underdevloping 
    /// this method is use to save recorder audio in to audio clip and play when you need.
    /// </summary>
    public void btn_playerSound()
    {
        GetComponent<AudioSource>().PlayOneShot(audioClipLocal);
    }
}
