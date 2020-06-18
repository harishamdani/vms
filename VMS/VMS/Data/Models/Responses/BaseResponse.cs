namespace VMS.Data.Models.Responses
{
    public abstract class BaseResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
