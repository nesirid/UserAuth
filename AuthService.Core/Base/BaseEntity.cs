namespace AuthService.Core.Base
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
