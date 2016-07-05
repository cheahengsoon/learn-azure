using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Yammerly.Models
{
    public class Employee : EntityData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string PhotoUrl { get; set; }


        [JsonIgnore]
        public string Name
        {
            get { return $"{FirstName} {LastName}"; } 
        }
    }
}
