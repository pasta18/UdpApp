﻿using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System;

public class UdpDataClient
{
    public readonly bool isServer; // サーバー側ならtrue
    public bool isRunningWork; // 動作中ならtrue
    public bool isSend = false; // 送信ボタンが押されたらtrue

    private readonly int PACKET_SIZE;
    private byte[] packet; // データを確認

    private Thread runWorkThread;

    private Socket sock;

    private event Action<String> onReceive = delegate { };


    public UdpDataClient(bool isServer, int packetSize = 1024)
    {
        this.isServer = isServer;
        PACKET_SIZE = packetSize;
        packet = new byte[PACKET_SIZE];

        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    // 非同期通信の開始
    public void RunWorkAsync(int port, IPAddress ipAddress = null)
    {
        if (isServer) // サーバー側
        {
            runWorkThread = new Thread(new ThreadStart(() =>
            {
                var local = new IPEndPoint(IPAddress.Any, 8000);
                var remote = new IPEndPoint(IPAddress.Any, 8000) as EndPoint;
                sock.Bind(local);

                while (isRunningWork)
                {
                    var length = sock.ReceiveFrom(packet, ref remote);
                    var data = Encoding.UTF8.GetString(packet);

                    this.OnReceive(data);
                }
            }));
        }
        else // クライアント側
        {
            runWorkThread = new Thread(new ThreadStart(() =>
            {
                IPAddress address;

                if (ipAddress == null) address = IPAddress.Loopback;
                else address = ipAddress;

                var remote = new IPEndPoint(address, 8000);

                sock.Bind(remote);

                while (isRunningWork)
                {
                    if(isSend)
                    {
                        sock.SendTo(packet, remote);
                        isSend = false;
                    }
                }
            }));
        }
    }
    // 受け取ったときの動作
    private void OnReceive(string data)
    {
        onReceive(data);
    }
    // 受け取ったときの動作の追加
    public void AddReceiveEvent(Action<string> action)
    {
        onReceive += action;
    }
    // 通信終了
    public void EndSocket()
    {
        if (isRunningWork)
        {
            isRunningWork = false;
            runWorkThread.Join();
        }
        if (sock != null)
        {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
    // 送信の準備
    public void Send(string data)
    {
        packet = Encoding.UTF8.GetBytes(data);
        isSend = true;
    }
}
