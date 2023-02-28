using Microsoft.AspNetCore.Mvc;

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

    public CourseController()
    {
        this.Service = new CourseDBService();
        this.Service.Start();
    } 
}