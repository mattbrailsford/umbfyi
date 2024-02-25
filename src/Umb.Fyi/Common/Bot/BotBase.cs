using Umb.Fyi.Hub.Models;
using Umb.Fyi.Hub.Services;

namespace Umb.Fyi.Common.Bot
{
    public abstract class BotBase
    {
        protected readonly MediaTipService _tipService;

        public BotBase(MediaTipService tipService)
        {
            _tipService = tipService;
        }

        protected void PostTip(MediaTip tip)
            => PostTipCoreAsync(tip, true).ConfigureAwait(false).GetAwaiter().GetResult();

        protected Task PostTipAsync(MediaTip tip)
            => PostTipCoreAsync(tip, false);

        protected async Task PostTipCoreAsync(MediaTip tip, bool sync)
        {
            _tipService.SubmitTip(tip);
        }
    }
}
