using FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Domain.Validation
{
    public class VideoValidator : Validator
    {
        private readonly Video _video;

        private const int TITLE_MAX_LENGTH = 255;
        private const int DESCRIPTION_MAX_LENGTH = 4_000;


        public VideoValidator(Video video, ValidationHandler handler) : base(handler)
        {
            _video = video;
        }

        public override void Validate()
        {
            ValidateTitle();
            ValidateDescription();
        }

        public void ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(_video.Title))
                _handler.HandleError($"{nameof(_video.Title)} is required.");

            if (_video.Title.Length > 255)
                _handler.HandleError($"{nameof(_video.Title)} should be less or equal {TITLE_MAX_LENGTH} characters long.");
        }

        public void ValidateDescription()
        {
            if (string.IsNullOrWhiteSpace(_video.Description))
                _handler.HandleError($"{nameof(_video.Description)} is required.");

            if (_video.Description.Length > 4000)
                _handler.HandleError($"{nameof(_video.Description)} should be less or equal {DESCRIPTION_MAX_LENGTH} characters long.");
        }
    }
}
