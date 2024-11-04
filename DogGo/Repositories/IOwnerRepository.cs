using DogGo.Models;

namespace DogGo.Repositories
{
    public interface IOwnerRepository
    {
        List<Owner> GetAllOwners();
        Owner GetOwnerById(int id);

        //these new methods added in OwnerRepo
        //Owner GetOwnerByName(string name);
        //Owner GetOwnerByUsername(string username);
        Owner GetOwnerByEmail(string email);
        //Owner GetOwnerByPhone(string phone);
       void AddOwner(Owner owner);

        void UpdateOwner(Owner owner);
        void DeleteOwner(int ownerId);
        List<Neighborhood> GetAllNeighborhoods();
    }

}
