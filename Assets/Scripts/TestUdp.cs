using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestUdp : MonoBehaviour
{
    UdpDataClient udp;

    [SerializeField]
    Button send;
    [SerializeField]
    Button receive;
    [SerializeField]
    Text text;
    [SerializeField]
    Button textSend;

    public void SendButtonSet()
    {
        udp = new UdpDataClient(false);
        send.interactable = false;
        receive.interactable = false;
        textSend.interactable = true;
    }

    public void ReceiveButtonSet()
    {
        udp = new UdpDataClient(true);
        send.interactable = false;
        receive.interactable = false;
        udp.AddReceiveEvent(OnReceive);
    }

    public void TextSendButtonClick()
    {
        udp.Send(text.text);
    }

    public void OnApplicationQuit()
    {
        if(udp != null) udp.EndSocket();
    }

    public void OnReceive(string data)
    {
        Debug.Log(data);
    }
}
