using System.Collections;
using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
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
        this.Scraper.Manager.CacheCourses(this.Scraper.CourseTerm);
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

    public string[] GetTerms()
    {
        Record[] records = this.Scraper.Manager.Database.FetchQueryData("SELECT DISTINCT Term FROM Courses", "Courses");

        string[] terms = new string[records.Length];

        for (int x = 0; x < records.Length; x++)
            terms[x] = (string)records[x].Values[0];
        
        return terms;
    }

    public void  SetTerm(Term term)
    {
        this.Scraper.SetTerm(term);
    }

    public Term GetTerm()
    {
        return Scraper.CourseTerm;
    }

    public void Stop()
    {
        this.Scraper.Manager.Database.Disconnect();
    }

    public CourseDBService()
    {
        this.Scraper = new CourseScraper(Term.GetCurrent(), DatabaseConfiguration.LoadFromFile("DatabaseConfig.json"));

        this.QueryHash = new Hashtable();
        
        for (int x = 0; x < CourseDBService.ValidKeys.Length; x++)
            this.QueryHash.Add(CourseDBService.ValidKeys[x], null);
    }
}


