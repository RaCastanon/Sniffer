
using SLIPProtocol;
using System;

namespace MessageContainerData
{
    public enum MessageType
    {
        TX_TO_DEVICE,
        TX_TO_DEVICE_FROM_SNIFFER,
        RX_FROM_DEVICE
    }

    public class MessageContainer
    {
        private SLIPMessage Message;
        private MessageType MessageDirection;

        public MessageContainer(SLIPMessage Message, MessageType MessageDirection)
        {
            this.Message = Message;
            this.MessageDirection = MessageDirection;
        }

        public SLIPMessage MessageObject
        {
            get { return Message; }
        }

        public MessageType IsMessageDirection
        {
            get { return MessageDirection; }
        }
    }
}
