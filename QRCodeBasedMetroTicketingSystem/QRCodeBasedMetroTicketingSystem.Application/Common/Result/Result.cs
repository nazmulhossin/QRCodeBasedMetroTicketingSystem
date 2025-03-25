namespace QRCodeBasedMetroTicketingSystem.Application.Common.Result
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success(string message = "") => new Result(true, message);
        public static Result Failure(string message) => new Result(false, message);
    }
}
