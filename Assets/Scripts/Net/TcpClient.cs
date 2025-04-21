using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using SocketProtocol;
using UnityEngine;
using Timer = System.Timers.Timer;

public class TcpClient : IDisposable
{
    private Socket _mSocket;
    private Message _mMessage;
    private string _mIpAddress;
    private int _mPort;
    private DateTime _mLastPongTime;

    private Timer _mSendHeartbeatTimer;
    private Timer _mCheckHeartbeatTimer;
    private Timer _mReconnectTimer;
    private int _mReconnectCount = 0;

    public int ClientId { get; private set; }
    public string ClientIp { get; private set; }
    public bool IsOnline { get; set; }

    public event Action<MainPack> OnServerResponse;
    public event Action OnClientCloseConnection;

    public TcpClient(string ip, int port)
    {
        _mIpAddress = ip;
        _mPort = port;
        _mMessage = new Message();

        InitSocket(ip, port);
    }

    private void InitSocket(string ip, int port)
    {
        try
        {
            _mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _mSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

            IPEndPoint localIpEndPoint = _mSocket.LocalEndPoint as IPEndPoint;
            ClientIp = localIpEndPoint.Address.ToString();

            Debug.Log("Socket connected to " + _mIpAddress + ":" + _mPort);
            _mLastPongTime = DateTime.Now;
            _mSendHeartbeatTimer = new Timer(5000)
            {
                AutoReset = true,
                Enabled = true,
            };
            _mSendHeartbeatTimer.Elapsed += SendHeartbeat;
            _mSendHeartbeatTimer.Start();

            _mCheckHeartbeatTimer = new Timer(10000)
            {
                AutoReset = true,
                Enabled = true,
            };
            _mCheckHeartbeatTimer.Elapsed += CheckHeartbeatTimeout;
            _mCheckHeartbeatTimer.Start();

            _mReconnectTimer = new Timer(GameAssets.RECONNECT_INTERVAL_MS)
            {
                AutoReset = true,
                Enabled = false
            };
            _mReconnectTimer.Elapsed += HandleReconnect;

            StartReceive();
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket Error:" + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e.StackTrace}");
        }
    }

    private void CheckHeartbeatTimeout(object state, ElapsedEventArgs e)
    {
        Debug.Log("Checking heartbeat timeout:" + (DateTime.Now - _mLastPongTime).TotalSeconds);
        if (_mSocket.Connected && (DateTime.Now - _mLastPongTime).TotalSeconds > 20)
        {
            Debug.Log("服务器心跳未返回，尝试重新连接");
            CloseSocket();

            ReConnectSocket();
        }
    }

    private void ReConnectSocket()
    {
        _mReconnectTimer.Start();
    }

    private void HandleReconnect(object sender, ElapsedEventArgs e)
    {
        try
        {
            _mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _mSocket.Connect(new IPEndPoint(IPAddress.Parse(_mIpAddress), _mPort));

            _mReconnectTimer.Stop();
            _mSendHeartbeatTimer.Start();
            _mCheckHeartbeatTimer.Start();
            _mLastPongTime = DateTime.Now;

            // 重连后，完成登录操作
            Invoker.Instance.DelegateList.Add(ReSignInByAuthorization);

            Debug.Log("重连成功!");
            StartReceive();
        }
        catch (SocketException ex)
        {
            _mReconnectCount++;
            Debug.LogError($"重连失败，次数：{_mReconnectCount}");
            if (_mReconnectCount > GameAssets.RECONNECT_COUNT_MAX)
            {
                _mReconnectTimer.Stop();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"重连失败, {ex}");
        }
    }

    private void ReSignInByAuthorization()
    {
        string authJson = PlayerPrefs.GetString(GameAssets.AUTHORIZATION_KEY, "");
        if (string.IsNullOrEmpty(authJson))
        {
            Debug.LogError("本地没有存储登录凭证!");
            CloseSocket();
        }

        Authorization auth = JsonUtility.FromJson<Authorization>(authJson);
        SignInPack signInPack = new SignInPack
        {
            Username = auth.username,
            Password = auth.password,
        };

        MainPack mainPack = new MainPack()
        {
            RequestCode = RequestCode.User,
            ActionCode = ActionCode.SignIn,
            SignInPack = signInPack
        };

        Send(mainPack);
    }

    private void SendHeartbeat(object state, ElapsedEventArgs e)
    {
        Debug.Log("发送心跳包...");
        HeartbeatPack heartbeatPack = new HeartbeatPack
        {
            Triggered = true,
            Type = "PING",
            Timestamp = DateTime.Now.Ticks
        };
        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.HeartBeat,
            Heartbeat = heartbeatPack
        };
        Send(pack);
    }

    private void StartReceive()
    {
        Debug.Log("开始接收服务端消息...");
        try
        {
            _mSocket.BeginReceive(_mMessage.Buffer, _mMessage.MessageLen
                , _mMessage.RemainSize, SocketFlags.None, ReceiveCallback, null);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Error:" + e.Message);
            CloseSocket();
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e}");
        }
    }

    private void ReceiveCallback(IAsyncResult iar)
    {
        if (!_mSocket.Connected)
        {
            Debug.LogError("Socket已断开");
            return;
        }

        try
        {
            int len = _mSocket.EndReceive(iar);
            if (len == 0)
            {
                CloseSocket();
                Debug.Log("连接已被对端关闭！");
                return;
            }

            _mMessage.ReadBuffer(len, HandleServerResponse);
            Debug.Log("接收服务端消息完毕...");
            StartReceive();
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket Error:" + e.Message);
            CloseSocket();
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e}");
        }
    }

    private void HandleServerResponse(MainPack pack)
    {
        if (pack.ResponseCode is ResponseCode.HeartBeatResponse && pack.Heartbeat is { Triggered: true, Type: "PONG" })
        {
            Debug.Log("接收到服务端的心跳...");
            _mLastPongTime = DateTime.Now;
            return;
        }

        if (pack.ActionCode == ActionCode.AssignClient)
        {
            Debug.Log($"当前客户端Id: {ClientId}");
            ClientId = pack.ClientPack.ClientId;
            // GameInterface.Interface.UdpListener.UdpListenPort = pack.ClientPack.UdpListenPort;
            return;
        }

        OnServerResponse?.Invoke(pack);
    }

    public void CloseSocket()
    {
        Debug.Log("Close Socket!!!");
        if (_mSocket != null)
        {
            OnClientCloseConnection?.Invoke();
            _mSocket.Shutdown(SocketShutdown.Both);
            _mSocket.Close();

            _mSendHeartbeatTimer?.Stop();
            _mCheckHeartbeatTimer?.Stop();
            _mReconnectTimer?.Stop();
        }
    }

    public void Send(MainPack pack)
    {
        try
        {
            if (_mSocket.Connected)
            {
                pack.ClientPack = new ClientPack
                {
                    ClientId = ClientId,
                    UdpListenPort = GameInterface.Interface.UdpListener.UdpListenPort,
                };
                _mSocket.Send(Message.GetPackData(pack));
            }
            else
            {
                Debug.LogError("连接已关闭，尝试重新连接...");
                ReConnectSocket();
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketError:" + e);
        }
    }

    public async Task<int> SendAsync(MainPack pack)
    {

        if (_mSocket.Connected)
        {
            _mSocket.Send(Message.GetPackData(pack));
            ReadOnlyMemory<byte> rm = new ReadOnlyMemory<byte>(_mMessage.Buffer);
            return await _mSocket.SendAsync(rm, SocketFlags.None);
        }

        Debug.LogError("连接已关闭，无法发送消息...");
        return await Task.FromResult(0);
    }

    public void Dispose()
    {
        CloseSocket();
        _mSocket?.Dispose();
        _mSendHeartbeatTimer?.Dispose();
        _mCheckHeartbeatTimer?.Dispose();
        _mReconnectTimer.Stop();
        _mReconnectTimer?.Dispose();
    }
}