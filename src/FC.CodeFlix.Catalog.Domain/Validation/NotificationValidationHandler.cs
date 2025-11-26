namespace FC.CodeFlix.Catalog.Domain.Validation
{
    public class NotificationValidationHandler : ValidationHandler
    {
        private readonly List<ValidationError> _errors;

        public IReadOnlyCollection<ValidationError> Errors => _errors.AsReadOnly();

        public bool HasErrors()
            => _errors.Count > 0;

        public NotificationValidationHandler()
            => _errors = new();

        public override void HandleError(ValidationError error)
            => _errors.Add(error);
    }
}
