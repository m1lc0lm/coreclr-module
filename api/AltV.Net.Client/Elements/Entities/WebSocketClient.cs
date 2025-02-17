﻿using System.Runtime.InteropServices;
using AltV.Net.CApi.ClientEvents;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared.Utils;

namespace AltV.Net.Client.Elements.Entities
{
    public class WebSocketClient : BaseObject, IWebSocketClient
    {
        public IntPtr WebSocketClientNativePointer { get; }
        public override IntPtr NativePointer => WebSocketClientNativePointer;

        private static IntPtr GetBaseObjectNativePointer(ICore core, IntPtr webSocketClientNativePointer)
        {
            unsafe
            {
                return core.Library.Client.WebSocketClient_GetBaseObject(webSocketClientNativePointer);
            }
        }
        
        public WebSocketClient(ICore core, IntPtr webSocketClientNativePointer) : base(core, GetBaseObjectNativePointer(core, webSocketClientNativePointer), BaseObjectType.WebsocketClient)
        {
            WebSocketClientNativePointer = webSocketClientNativePointer;
        }
        
        public WebSocketClient(ICore core, string url) : this(core, core.CreateWebSocketClientPtr(url))
        {
            core.WebSocketClientPool.Add(this);
        }


        public bool AutoReconnect
        {
            get
            {
                unsafe
                {
                    return Core.Library.Client.WebSocketClient_IsAutoReconnect(WebSocketClientNativePointer) == 1;
                }
            }
            set
            {
                unsafe
                {
                    Core.Library.Client.WebSocketClient_SetAutoReconnect(WebSocketClientNativePointer, (byte) (value ? 1 : 0));
                }
            }
        }
        
        public bool PerMessageDeflate
        {
            get
            {
                unsafe
                {
                    return Core.Library.Client.WebSocketClient_IsPerMessageDeflate(WebSocketClientNativePointer) == 1;
                }
            }
            set
            {
                unsafe
                {
                    Core.Library.Client.WebSocketClient_SetPerMessageDeflate(WebSocketClientNativePointer, (byte) (value ? 1 : 0));
                }
            }
        }
        
        public ushort PingInterval
        {
            get
            {
                unsafe
                {
                    return Core.Library.Client.WebSocketClient_GetPingInterval(WebSocketClientNativePointer);
                }
            }
            set
            {
                unsafe
                {
                    Core.Library.Client.WebSocketClient_SetPingInterval(WebSocketClientNativePointer, value);
                }
            }
        }
        
        public WebSocketReadyState ReadyState
        {
            get
            {
                unsafe
                {
                    return (WebSocketReadyState) Core.Library.Client.WebSocketClient_GetReadyState(WebSocketClientNativePointer);
                }
            }
        }
        
        public string Url
        {
            get
            {
                unsafe
                {
                    var size = 0;
                    return Core.PtrToStringUtf8AndFree(Core.Library.Client.WebSocketClient_GetUrl(WebSocketClientNativePointer, &size), size);
                }
            }
            
            set
            {
                unsafe
                {
                    var valuePtr = MemoryUtils.StringToHGlobalUtf8(value);
                    Core.Library.Client.WebSocketClient_SetUrl(WebSocketClientNativePointer, valuePtr);
                    Marshal.FreeHGlobal(valuePtr);
                }
            }
        }

        public void AddSubProtocol(string subProtocol)
        {
            unsafe
            {
                var valuePtr = MemoryUtils.StringToHGlobalUtf8(subProtocol);
                Core.Library.Client.WebSocketClient_AddSubProtocol(WebSocketClientNativePointer, valuePtr);
                Marshal.FreeHGlobal(valuePtr);
            }
        }
        
        public string[] GetSubProtocols()
        {
            unsafe
            {
                uint size = 0;
                var subProtocolsPtr = Core.Library.Client.WebSocketClient_GetSubProtocols(WebSocketClientNativePointer, &size);
                return Core.MarshalStringArrayPtrAndFree(subProtocolsPtr, size);
            }
        }

        public bool Send(string message)
        {
            unsafe
            {
                var messagePtr = MemoryUtils.StringToHGlobalUtf8(message);
                var result = Core.Library.Client.WebSocketClient_Send_String(WebSocketClientNativePointer, messagePtr) == 1;
                Marshal.FreeHGlobal(messagePtr);
                return result;
            }
        }

        public bool Send(byte[] message)
        {
            unsafe
            {
                var messagePtr = MemoryUtils.ByteArrayToHGlobal(message);
                var result = Core.Library.Client.WebSocketClient_Send_Binary(WebSocketClientNativePointer, messagePtr, (uint) message.Length) == 1;
                Marshal.FreeHGlobal(messagePtr);
                return result;
            }
        }
        
        public void SetExtraHeader(string name, string value)
        {
            unsafe
            {
                var namePtr = MemoryUtils.StringToHGlobalUtf8(name);
                var valuePtr = MemoryUtils.StringToHGlobalUtf8(value);
                Core.Library.Client.WebSocketClient_SetExtraHeader(WebSocketClientNativePointer, namePtr, valuePtr);
                Marshal.FreeHGlobal(namePtr);
                Marshal.FreeHGlobal(valuePtr);
            }
        }
        
        public void Start()
        {
            unsafe
            {
                Core.Library.Client.WebSocketClient_Start(WebSocketClientNativePointer);
            }
        }
        
        public void Stop()
        {
            unsafe
            {
                Core.Library.Client.WebSocketClient_Stop(WebSocketClientNativePointer);
            }
        }
    }
}