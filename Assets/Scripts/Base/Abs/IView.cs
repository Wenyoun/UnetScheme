using Zyq.Game.Base;
namespace Base
{
    public interface IView : ILifecycle, IUpdate, ICanUpdate
    {
        void OnStart();

        void OnRegEvent();

        void OnRepeat(object body);

        void OnShowBefore();

        void OnShow(object body);

        void OnHideBefore();

        void OnHide(object body);
    }
}