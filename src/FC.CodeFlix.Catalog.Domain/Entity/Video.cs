using FC.CodeFlix.Catalog.Domain.SeedWork;

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
        public Video(
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
            CreatedAt = DateTime.Now;
        }
    }
}
