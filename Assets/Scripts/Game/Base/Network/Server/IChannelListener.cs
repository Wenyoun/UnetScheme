namespace Nice.Game.Base
{
    internal interface IChannelListener
    {
        void OnAddChannel(IChannel channel);

        void OnRemoveChannel(IChannel channel);
    }
}