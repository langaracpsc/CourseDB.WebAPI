using System.Collections;
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
                courses = this.Service.GetCourses(JsonConvert.DeserializeObject<Dictionary<string, object>>(query));
            }
            catch (KeyNotFoundException e)
            {
                return new HttpError(HttpErrorType.InvalidKeyError, "Provided key is invalid.").ToJson();
            }
    
            return new HttpObject(HttpReturnType.Success, courses).ToJson();
        });
    }

    public CourseController()
    {
        this.Service = new CourseDBService();
        this.Service.Start();
    } 
} 
