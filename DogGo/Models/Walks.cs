using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Data;
using System.Security.Principal;
using System;

namespace DogGo.Models
{
    public class Walks
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; } //int in database already! so must convert to timespan later!
        public int WalkerId { get; set; }
        public int DogId { get; set; }

        //added ClientName for walker profile view 
        public string ClientName { get; set; }

        public string Status { get; set; } //added for select walker in neighboorhood and make a walk
        

    }
}
