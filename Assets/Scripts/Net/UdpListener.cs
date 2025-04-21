using System;
using System.Net;
using System.Net.Sockets;
using GameFrameSync;
using Google.Protobuf;
using UnityEngine;

public class UdpListener : IDisposable
{
    private UdpClient _mUdpClient;
    private int _mCurrentDataSequence = 0;

    private readonly ObjectPool<ResFrameSyncData> _mResFrameSyncDataPool;
    private readonly ObjectPool<MessageHead> _mMessageHeadPool;
    public int UdpListenPort { get; private set; }
    public bool Disposed { get; private set; }
    public IPEndPoint RemoteEp { get; private set; }
    public event Action<ResFrameSyncData> OnReceiveFrameSync;

    public UdpListener()
    {
        _mResFrameSyncDataPool = new ObjectPool<ResFrameSyncData>(() => new ResFrameSyncData());
        _mMessageHeadPool = new ObjectPool<MessageHead>(() => new MessageHead());

        _mUdpClient = new UdpClient(0, AddressFamily.InterNetwork);
        IPEndPoint clientLocalEndPoint = _mUdpClient.Client.LocalEndPoint as IPEndPoint;
        UdpListenPort = clientLocalEndPoint.Port;
        Debug.Log("Current UDP available port:" + UdpListenPort);
    }

    public void StartListen()
    {
        StartReceive();
    }

    private void StartReceive()
    {
        if (Disposed) return;
        try
        {
            _mUdpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (SocketException e)
        {
            Debug.LogError("UDP SocketError:" + e);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception:" + e);
        }
    }

    private void ReceiveCallback(IAsyncResult iar)
    {
        if (Disposed) return;
        try
        {
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = _mUdpClient.EndReceive(iar, ref remoteEp);
            if (RemoteEp == null)
            {
                RemoteEp = remoteEp;
            }

            // Debug.Log($"远端监听：{RemoteEp.Address}:{RemoteEp.Port}");
            ResFrameSyncData resFrameSyncData = Deserialize(data);
            if (resFrameSyncData.MessageType is MessageType.FrameSync)
            {
                // Debug.Log($"收到服务端udp消息:{resFrameSyncData.FrameId}");
                OnReceiveFrameSync?.Invoke(resFrameSyncData);

                SendAck(resFrameSyncData.MessageHead.Index);
            }

            StartReceive();
        }
        catch (SocketException e)
        {
            Debug.LogError("UDP SocketError:" + e);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception:" + e);
        }
    }

    public void Send(in ResFrameSyncData resFrameSyncData)
    {
        if (Disposed) return;
        MessageHead messageHead = _mMessageHeadPool.Allocate();
        messageHead.Index = _mCurrentDataSequence;

        resFrameSyncData.MessageHead = messageHead;
        resFrameSyncData.MessageType = MessageType.FrameSync;

        try
        {
            byte[] data = Serialize(resFrameSyncData);

            _mCurrentDataSequence++;

            _mUdpClient.Send(data, data.Length, RemoteEp);
        }
        catch (SocketException e)
        {
            Debug.LogError("UDP SocketError:" + e);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception:" + e);
        }
        finally
        {
            _mMessageHeadPool.Release(messageHead);
        }
    }

    private void SendAck(int index)
    {
        if (Disposed) return;
        MessageHead messageHead = _mMessageHeadPool.Allocate();
        messageHead.Index = index;
        messageHead.Ack = true;
        // messageHead.ClientIp = GameInterface.Interface.TcpClient.ClientIp;

        ResFrameSyncData resFrameSyncData = _mResFrameSyncDataPool.Allocate();
        resFrameSyncData.MessageHead = messageHead;
        resFrameSyncData.MessageType = MessageType.Ack;

        try
        {
            byte[] data = Serialize(resFrameSyncData);
            _mUdpClient.Send(data, data.Length, RemoteEp);
        }
        catch (SocketException e)
        {
            Debug.LogError("UDP SocketError:" + e);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception:" + e);
        }
        finally
        {
            _mMessageHeadPool.Release(messageHead);
            _mResFrameSyncDataPool.Release(resFrameSyncData);
        }
    }

    private void Close()
    {
        _mUdpClient.Close();
    }

    private byte[] Serialize(ResFrameSyncData resFrameSyncData)
    {
        byte[] data = resFrameSyncData.ToByteArray();
        return data;
    }

    private ResFrameSyncData Deserialize(byte[] data)
    {
        ResFrameSyncData resFrameSyncData = ResFrameSyncData.Parser.ParseFrom(data);
        return resFrameSyncData;
    }

    public void Dispose()
    {
        Close();
        _mUdpClient?.Dispose();
        Disposed = true;
    }
}