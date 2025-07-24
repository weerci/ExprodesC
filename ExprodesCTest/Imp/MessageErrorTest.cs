using ExprodesC.Models;
using ExprodesC.ViewModels;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace ExprodesCTest.Imp
{
    public class ErrorMessageTest
    {
        [Fact]
        public void Send_message_error_test()
        {
            SenderError senderError = new();
            TargetError targetError = new();

            senderError.AddError();
            Assert.Equal("Caption error", targetError.Error.Caption);
            Assert.Equal("System.ArgumentNullException", targetError.Error?.Error?.GetType().ToString());
        }
    }

    class SenderError : BaseVM
    {
        public void AddMessage()
        {
            Log.SendMessage("Message caption", "Message description");
        }
        public void AddMessageOther()
        {
            Log.SendMessage("Ohter message caption", "Other message description");
        }

        public void AddError()
        {
            Log.SendError("Caption error", new ArgumentNullException());
        }
        public void AddErrorOther()
        {
            Log.SendError("Other caption error", new AggregateException());
        }
    }

    class TargetError : BaseVM
    {
        public TargetError()
        {
            Log.Errors.Subscribe(x => Error = x.Last().Item.Current);
            Log.Messages.Subscribe(x => Message = x.Last().Item.Current);
        }

        [ObservableAsProperty] public ExpMessage Error { get; set; } = null!;
        [ObservableAsProperty] public ExpMessage Message { get; set; } = null!;
    }
}
