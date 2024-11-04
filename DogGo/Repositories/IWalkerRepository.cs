using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{

    public interface IWalkerRepository
    {
        List<Walker> GetAllWalkers();
        Walker GetWalkerById(int id);

        List<Walker> GetWalkersInNeighborhood(int neighborhoodId);
        List<Walks> GetRecentWalksByWalkerId(int selectedWalkerId); //walks for a selected walker 
        TimeSpan CalculateTotalWalkTime(int selectedWalkerId);//total walk duration for a selected walker 
    }
}