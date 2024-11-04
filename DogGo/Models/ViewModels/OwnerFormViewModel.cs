// to make dropdown selection for neighborhood
//think what u need to have in state if this were a React application:
//Properties for all the Owner form fields
//A list of available options for the dropdown

using System.Collections.Generic;

namespace DogGo.Models.ViewModels
{
    public class OwnerFormViewModel
    {
        public Owner Owner { get; set; }
        public List<Neighborhood> Neighborhoods { get; set; }
    }
}