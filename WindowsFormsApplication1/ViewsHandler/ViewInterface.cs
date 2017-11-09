using BlueBoxProtocol;
using MessageContainerData;

namespace ViewsHandler
{
    public enum ViewStatus
    {
        ENABLE,
        DISABLE
    }

    public enum CommunicationStatus
    {
        ENABLE,
        DISABLE
    }

    public enum ListenerType
    {
        GUI_LISTENER,
        SNIFFER_LISTENER
    }

    public delegate void ReceiveAVCLANCallback(MessageContainer message);
    public delegate void SendAVCLANMessage(MessageContainer message);

    public interface ViewInterface
    {
        void processIncomingMessage(MessageContainer message);
        void setCommunicationStatus(CommunicationStatus status);
        void setViewStatus(ViewStatus status);
        void setCommunicationDelegate(SendAVCLANMessage callback);
        ListenerType getListenerType();
    }
}
