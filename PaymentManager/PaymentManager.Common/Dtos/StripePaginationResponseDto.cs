namespace PaymentManager.Common.Dtos
{
    public class StripePaginationResponseDto<T> where T : class
    {
        public bool HasMore { get; set; }
        public T? Data { get; set; }
    }
}
