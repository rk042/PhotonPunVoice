using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.IO.Compression;

//using Photon.Voice.Unity.UtilityScripts;
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
      
    private byte[] bytes;

    private void Awake()
    {
        //connect to photon
        PhotonNetwork.ConnectUsingSettings();

        //get voice connection object
        voiceConnection = GetComponent<VoiceConnection>();

        saveAudioIntoFile(callback);
        
    }

    private void callback(int i)
    {
        if (i == 1)
        {
            Debug.Log("song is saving..");

            //first crate directory other wash you get error
            Directory.CreateDirectory(Application.persistentDataPath + "/saveAudio/");

            string path = Application.persistentDataPath + "/saveAudio/" + "SaveAudioByte" + ".mp3";

            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, bytes);
            }
        }
    }

    private void saveAudioIntoFile(System.Action<int> action)
    {
        //first crate directory other wash you get error
        Directory.CreateDirectory(Application.persistentDataPath + "/saveAudio/");
        string path;

        path =Path.Combine(Application.persistentDataPath,Application.persistentDataPath + "/out_2021-09-19_09-14-04-1111_370.wav");

        //#if UNITY_EDITOR
        //        //path of file with file extantion name.
        //        path = Application.persistentDataPath + "/saveAudio/" + "LoadAudio" + ".mp3";
        //#else
        //        path="file:///"+Application.persistentDataPath + "/saveAudio/" + "LoadAudio" + ".mp3";
        //#endif
        if (File.Exists(path))
        {
            bytes = File.ReadAllBytes(path);

            Debug.Log("read complete");
            action(1);
        }
        else
        {
            Debug.Log("file not exists "+path);
            action(0);
        }
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

    public override void OnJoinedRoom()
    {
        Debug.Log("joined room "+PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.CurrentRoom.PlayerCount==2)
        {
            Debug.LogError("both players in same room");
        }
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
        Debug.Log("rpc button called");

        //photonView.RPC("RPC_sendbyteData", RpcTarget.All, bytes);               

        Debug.Log("Length - "+bytes.Length);

        photonView.RPC("RPC_Test", RpcTarget.All, bytes);
    }
   
    [PunRPC]
    private void RPC_sendbyteData(byte[] by)
    {
        Debug.Log("RPC_sendbyteData called");

        //first crate directory other wash you get error
        Directory.CreateDirectory(Application.persistentDataPath + "/PhotonSendData/");
        string path;
        path = Path.Combine(Application.persistentDataPath, path = Application.persistentDataPath + "/PhotonSendData/" + "SaveAudioByte" + ".mp3");
        //#if UNITY_EDITOR
        //        path= Application.persistentDataPath + "/PhotonSendData/" + "SaveAudioByte" + ".mp3";
        //#else
        //        path="file:///"+Application.persistentDataPath + "/PhotonSendData/" + "SaveAudioByte" + ".mp3";
        //#endif
       

        if (!File.Exists(path))
        {
            File.WriteAllBytes(path, by);
            Debug.Log("file save complete rpc "+path);
        }
        else
        {
            Debug.Log("file path not existe rpc "+path);
        }
    }

    [PunRPC]
    private void RPC_Test(byte[] by)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("player name is " + PhotonNetwork.PlayerList[i].NickName);
        }

        //first crate directory other wash you get error
        Directory.CreateDirectory(Application.persistentDataPath + "/PhotonSendData/");
        string path;
        path = Path.Combine(Application.persistentDataPath, path = Application.persistentDataPath + "/PhotonSendData/" + "SaveAudioByte" + ".mp3");
        //#if UNITY_EDITOR
        //        path= Application.persistentDataPath + "/PhotonSendData/" + "SaveAudioByte" + ".mp3";
        //#else
        //        path="file:///"+Application.persistentDataPath + "/PhotonSendData/" + "SaveAudioByte" + ".mp3";
        //#endif
        if (!File.Exists(path))
        {
            File.WriteAllBytes(path, by);
            Debug.Log("file save complete rpc " + path);
        }
        else
        {
            Debug.Log("file path not existe rpc " + path);
        }
    }

    IEnumerator GetAudioClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            while (!www.isDone)
            {
                Debug.Log("progress "+www.downloadProgress);
                yield return null;
            }
          
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("player enter room"+newPlayer.NickName);
    }   
}
