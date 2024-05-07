using AutoGen.Mistral;

namespace EasyAgent.Pages.Chat
{
    public partial class Chat
    {
        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;
        protected List<ChatMessage> MessageList = [];
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected async Task OnSendAsync()
        {
        }
    }
}
