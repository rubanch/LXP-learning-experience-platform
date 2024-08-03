using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace LXP.Common.ViewModels
{
    public class TopicRatingViewModel
    {
         public Guid Topic_Id { get; set; }

         public string  Name { get; set; }

         public  decimal Rating {get; set; }
    }
}
