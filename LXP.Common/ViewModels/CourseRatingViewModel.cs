using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace LXP.Common.ViewModels
{
    public class CourseRatingViewModel
    {
         public Guid Course_Id { get; set; }

         public string  Title { get; set; }

         public  decimal Rating {get; set; }
    }
}
