using Umb.Fyi.Hub.Models;
using Umbraco.UIBuilder.Persistence;

namespace Umb.Fyi.Hub.Services
{
    public class MediaTipService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        public MediaTipService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void SubmitTip(MediaTip tip)
        {
            using (var repo = _repositoryFactory.GetRepository<MediaTip, Guid>())
            {
                var existingTips = repo.GetAll(x => x.Link == tip.Link);
                if (existingTips.Success && existingTips.Model.Any())
                {
                    var existingTip = existingTips.Model.First();
                    existingTip.Votes++;
                    repo.Update(existingTip);
                }
                else
                {
                    repo.Insert(tip);
                }
            }
        }
    }
}
