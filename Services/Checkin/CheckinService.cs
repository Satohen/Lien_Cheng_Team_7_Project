using 第7小組專題.Models.Checkin;
using 第7小組專題.Repository.Checkin;

namespace 第7小組專題.Services.Checkin
{
    public class CheckinService
    {
        private readonly CheckinRepository _repo;

        public CheckinService(IConfiguration config)
        {
            _repo = new CheckinRepository(config);
        }

        public List<CheckinModels> GetAllCheckins()
        {
            return _repo.GetAll();
        }
    }
}
