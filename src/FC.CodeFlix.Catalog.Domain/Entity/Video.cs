using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.Validation;
using FC.CodeFlix.Catalog.Domain.ValueObject;

namespace FC.CodeFlix.Catalog.Domain.Entity
{
    public class Video : AggregateRoot
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int YearLaunched { get; set; }

        public bool Opened { get; set; }

        public bool Published { get; set; }

        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; }

        public Rating Rating { get; set; }

        public Image? Thumb { get; private set; }
        public Image? ThumbHalf { get; private set; }
        public Image? Banner { get; private set; }

        public Video(
            string title,
            string description,
            int yearLaunched,
            bool opened,
            bool published,
            int duration,
            Rating rating
        )
        {
            Title = title;
            Description = description;
            YearLaunched = yearLaunched;
            Opened = opened;
            Published = published;
            Duration = duration;
            Rating = Rating;
            CreatedAt = DateTime.Now;
        }

        public void Validate(ValidationHandler notificationHandler)
            => (new VideoValidator(this, notificationHandler)).Validate();

        public void Update(
            string title,
            string description,
            int yearLaunched,
            bool opened,
            bool published,
            int duration
        )
        {
            Title = title;
            Description = description;
            YearLaunched = yearLaunched;
            Opened = opened;
            Published = published;
            Duration = duration;
        }

        public void UpdateThumb(string path) => Thumb = new Image(path);

        public void UpdateThumbHalf(string path) => ThumbHalf = new Image(path);

        public void UpdateBanner(string path) => Banner = new Image(path);
    }
}
