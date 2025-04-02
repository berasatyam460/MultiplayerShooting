
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomButton : MonoBehaviour
{
   private RoomInfo info;
   [SerializeField]TMP_Text buttonText;

   public void SetButtonDetails(RoomInfo info){
       this.info=info;
       Debug.Log(this.info.Name);
       buttonText.text=info.Name;
   }
   public void JoinToRoom(){
    Launcher.instance.JoinRoom(info);
   }
}
