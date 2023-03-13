using System.Collections;
using Microsoft.SqlServer.Server;
using OpenDatabase;

namespace CourseDB.WebAPI;

public class InvalidKeyException : Exception
{
    public InvalidKeyException(string key) : base($"Invalid key \"{key}\"")
    {
    }
}

public class CourseDBService : IService
{
    protected CourseScraper Scraper;

    protected static string[] ValidKeys = new string[] {
                "Term",
                "Seats",
                "Waitlist",
                "CRN",
                "Room",
                "Subj",
                "CourseNumber",
                "Section",
                "Credits",
                "Title",
                "Fees",
                "RptLimit",
                "CourseType",
                "Instructor",
                "Schedule",
                "StartTime",
                "EndTime"
    };

    protected  Hashtable QueryHash;
    
    public void Start()
    {
        this.Scraper.SyncDB();
    }

    public Course[] GetCourses()
    {
        return this.Scraper.Manager.Courses.ToArray();
    }

    public Course[] GetCourses(Dictionary<string, object> queryMap)
    {
        foreach (string key in queryMap.Keys)
            if (Tools.LinearSearch(key, CourseDBService.ValidKeys) == -1)
                throw new InvalidKeyException($"Provided key \"{key}\" is invalid.");

        return this.Scraper.Manager.GetCoursesByQuery(queryMap);
    }
    
    
    public void Stop()
    {
    }

    public CourseDBService()
    {
        this.Scraper = new CourseScraper(Term.GetCurrent(), DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));

        this.QueryHash = new Hashtable();
        
        for (int x = 0; x < CourseDBService.ValidKeys.Length; x++)
            this.QueryHash.Add(CourseDBService.ValidKeys[x], null);
    }
}




