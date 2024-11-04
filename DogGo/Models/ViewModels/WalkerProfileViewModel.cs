using System.Reflection;

namespace DogGo.Models.ViewModels
{
    public class WalkerProfileViewModel
    {
        //public (class) (instance of class's name) { get; set; }
        //Note: class is custom type
        //so normally: 
        //[access modifier] [type] [property name] { get; set; }

        public Walker Walker { get; set; }
        public List<Walks> Walks { get; set; } = new List<Walks>(); //= new List<Walks>(); to avoid null ref
        public TimeSpan TotalWalkTime { get; set; } //Hint: Use the DateTime class to help format the date strings.
        //now go to WalkersController to update to get access to repositories for Walks because you want to show walks info on walkers profile page

        //to access when in Details.cshtml:***************
        //@Model.Walker.Name
        //@Model.Walks
        //@Model.TotalWalkTime

    }
}
