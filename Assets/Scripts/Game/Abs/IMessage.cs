namespace Game
{
    public interface IMessage
    {
        void Dispatcher(int id, IBody body = null);
    }
}