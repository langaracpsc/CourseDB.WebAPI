using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CourseDB.WebAPI;

[Route("/courses")]
public class CourseController : ControllerBase
{
    public CourseDBService Service;

    [HttpGet]
    public async Task<string> Get()
    {
        return await Task.Run(() =>
        {
            return new HttpObject(HttpReturnType.Success, this.Service.GetCourses()).ToJson();
        });
    }

    [HttpGet("/courses/query")]
    public async Task<string> GetCoursesByQuery([FromHeader]string query)
    {
        return await Task.Run(() =>
        {
            Course[] courses = null;

            try
            {
                Stopwatch watch = Stopwatch.StartNew();
            
                watch.Start();
                courses = this.Service.GetCourses(JsonConvert.DeserializeObject<Dictionary<string, object>>(query));
                watch.Stop();
                
                Console.WriteLine($"Elasped deserialize: {watch.Elapsed.Seconds}:{watch.Elapsed.Milliseconds}:{watch.Elapsed.Microseconds}");
            }
            catch (KeyNotFoundException e)
            {
                return new HttpError(HttpErrorType.InvalidKeyError, "Provided key is invalid.").ToJson();
            }

            Stopwatch watch1 = Stopwatch.StartNew();
            
            watch1.Start();
            
            string retObject = new HttpObject(HttpReturnType.Success, courses).ToJson();

            watch1.Stop();
            
            Console.WriteLine($"Elasped serialize: {watch1.Elapsed.Seconds}:{watch1.Elapsed.Milliseconds}:{watch1.Elapsed.Microseconds}");
            
            return retObject;
        });
    }

    [HttpPost("/setterm")]
    public async Task<string> SetTerm([FromHeader]string term)
    {
        return await Task.Run(() =>
        {
            Console.WriteLine(term);
            this.Service.SetTerm(Term.FromTermString(term));
            return new HttpObject(HttpReturnType.Success, "Success").ToJson();
        });
    }

    [HttpGet("/term")]
    public async Task<string> GetTerm()
    {
        return await Task.Run(() =>
        {
            return new HttpObject(HttpReturnType.Success, this.Service.GetTerm()).ToJson();
        });
    }

    public CourseController()
    {
        this.Service = new CourseDBService();
        this.Service.Start();
    } 
} 
