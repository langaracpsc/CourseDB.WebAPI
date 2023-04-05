using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenDatabase;

namespace CourseDB.WebAPI;

public class Cache
{
    public static string JsonCache = null;

    public static CourseDBService Service = null;

    public static Dictionary<string, string> QueryMap = null; // caches query results
}

[Route("/courses")]
public class CourseController : ControllerBase
{
    public string JsonCache;
    
    [HttpGet]
    public async Task<string> Get()
    {
        return await Task.Run(() =>
        {
            return Cache.JsonCache; //(Cache.JsonCache != null) ? Cache.JsonCache : Cache.JsonCache = new HttpObject(HttpReturnType.Success, this.Service.GetCourses()).ToJson();
        });
    }

    [HttpGet("/courses/query")]
    public async Task<string> GetCoursesByQuery([FromHeader]string query)
    {
        return await Task.Run(() =>
        {
            Course[] courses = null;

            string key; 
            
            try
            {
                if (Cache.QueryMap.ContainsKey(key = CourseDB.WebAPI.Tools.EliminateSubString(query, " ")))
                    return Cache.QueryMap[key];
                
                courses = Cache.Service.GetCourses(JsonConvert.DeserializeObject<Dictionary<string, object>>(query));
            }
            catch (KeyNotFoundException e)
            {
                return new HttpError(HttpErrorType.InvalidKeyError, "Provided key is invalid.").ToJson();
            }

            string retObject = new HttpObject(HttpReturnType.Success, courses).ToJson();

            Cache.QueryMap.Add(key, retObject);
            
            return retObject;
        });
    }
    
    [HttpGet("/courses/querymatch")]
    public async Task<string> GetCoursesByQueryMatch([FromHeader]string query)
    {
        return await Task.Run(() =>
        {
            Course[] courses = null;

            string key = null,
                retObject;

            try
            {
                // Checks the query against the cache
                if (Cache.QueryMap.ContainsKey(key = CourseDB.WebAPI.Tools.EliminateSubString(query, " "))) 
                    return Cache.QueryMap[key];

                courses = Cache.Service.FetchAlikeQuery(JsonConvert.DeserializeObject<Dictionary<string, string>>(query));
            }
            catch (KeyNotFoundException e)
            {
                return new HttpError(HttpErrorType.InvalidKeyError, "Provided key is invalid.").ToJson();
            }
            catch (Exception e)
            {
                return new HttpError(HttpErrorType.InvalidQueryException, "Provided query is invalid.").ToJson();
            }
            
            retObject = new HttpObject(HttpReturnType.Success, courses).ToJson();

            Cache.QueryMap.Add(key, retObject);
            
            return retObject;
        });
    }

    [HttpPost("/setterm")]
    public async Task<string> SetTerm([FromHeader]string term)
    {
        return await Task.Run(() =>
        {
            Console.WriteLine(term);
            Cache.Service.SetTerm(Term.FromTermString(term));
            return new HttpObject(HttpReturnType.Success, "Success").ToJson();
        });
    }

    [HttpGet("/term")]
    public async Task<string> GetTerm()
    {
        return await Task.Run(() =>
        {
            return new HttpObject(HttpReturnType.Success, Cache.Service.GetTerm()).ToJson();
        }); 
    }
    
    [HttpGet("/terms")]
    public async Task<string> GetTerms()
    {
        return new HttpObject(HttpReturnType.Success, Cache.Service.GetTerms()).ToJson();
    }

    public CourseController()
    {
        if (Cache.Service == null)
        {
            Cache.Service = new CourseDBService();
            Cache.Service.Start();
        }

        if (Cache.QueryMap == null)
            Cache.QueryMap = new Dictionary<string, string>();
       
        if (Cache.JsonCache == null)
        {
            Cache.JsonCache = new HttpObject(HttpReturnType.Success, Cache.Service.GetCourses()).ToJson();
            
            Console.WriteLine("Fetched courses.");
        }
    } 
} 
