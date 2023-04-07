using System.Collections;
using System.Diagnostics;
using System.Globalization;
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
        "CRN", "CourseNumber", "CourseType", "Credits", "EndTime", "Fees", "Instructor", "Room", "RptLimit", "Schedule", "Seats", "Section", "StartTime", "Subj", "Term", "Title", "Waitlist"
    };

    protected  Hashtable QueryHash;
    
    public void Start()
    {
        this.Scraper.SyncDB();
        this.Scraper.Manager.CacheCourses(this.Scraper.CourseTerm);
    }

    protected bool CheckKeyValidity(string key)
    {
        return (Array.BinarySearch(CourseDBService.ValidKeys, key) != -1);
    }

    
    /// <summary>
    /// Checks the if the keys of the provided query map are valid
    /// </summary>
    /// <param name="queryMap"> Query map to check. </param>
    /// <returns> Key validity. </returns>
    protected bool CheckKeys(Dictionary<string, string> queryMap)
    {
        foreach (string key in queryMap.Keys)
            if (!this.CheckKeyValidity(key))
                return false;
        return true;
    }

    public Course[] GetCourses()
    {
        return this.Scraper.Manager.Courses.ToArray();
    }

    public Course[] GetCourses(Dictionary<string, object> queryMap)
    {
        foreach (string key in queryMap.Keys)
            if (!this.CheckKeyValidity(key))
                throw new InvalidKeyException($"Provided key \"{key}\" is invalid.");
        
        return this.Scraper.Manager.GetCoursesByQuery(queryMap);
    }

    public Course[] FetchAlikeQuery(Dictionary<string, object> queryMap)
    {
        string conditionString = null;

        Course[] courses = null;
      
        foreach (string key in queryMap.Keys)
            if (!this.CheckKeyValidity(key))
                throw new InvalidKeyException($"Provided key \"{key}\" is invalid");

        courses = this.Scraper.Manager.GetCoursesByQueryMatch(queryMap); 
        
        return courses;
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


