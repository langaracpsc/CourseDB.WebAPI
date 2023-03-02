using OpenDatabase;

namespace CourseDB.WebAPI;

public class CourseDBService : IService
{
    protected CourseScraper Scraper;
 
    public void Start()
    {
        this.Scraper.SyncDB(false);
    }

    public Course[] GetCourses()
    {
        return this.Scraper.Manager.Courses.ToArray();
    }

    public void Stop()
    {
    }

    public CourseDBService()
    {
        this.Scraper = new CourseScraper(Term.GetCurrent(), DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));
    }
}




