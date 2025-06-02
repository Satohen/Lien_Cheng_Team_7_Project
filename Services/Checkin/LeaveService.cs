using Azure.Core;
using 第7小組專題.Models.Checkin;
using 第7小組專題.Repository.Checkin;

namespace 第7小組專題.Services.Checkin
{
    public class LeaveService
    {
        private readonly LeaveRepository _repo;

        public LeaveService(IConfiguration config)
        {
            _repo = new LeaveRepository(config);
        }

        public object ApplyLeave(LeaveApplyRequest request, IFormFile? attachment)
        {    // 檢查：如果重疊則直接拒絕
            if (_repo.HasOverlappingLeave(request.employeeId, request.fromDate, request.toDate))
            {
                return new { message = "您已申請過相同日期範圍內的請假，請勿重複申請。" };
            }

            string? filePath = null;

            try
            {
                if (attachment != null && attachment.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Leave");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(attachment.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        attachment.CopyTo(stream);
                    }

                    filePath = $"/Uploads/Leave/{uniqueFileName}";
                }

                var success = _repo.InsertLeaveRequest(request, filePath);
                return success
                    ? new { message = "請假申請成功！" }
                    : new { message = "申請失敗，請稍後再試。" };
            }
            catch (Exception ex)
            {
                Console.WriteLine("儲存檔案錯誤: " + ex.Message);
                return new { message = "後端處理錯誤：" + ex.Message };
            }
        }

        public object GetLeaveRecordsByEmployee(LeaveQueryRequest request)
        {
            var (records, total) = _repo.GetPagedRecords(request);
            return new { data = records, total = total };
        }

    }
}
