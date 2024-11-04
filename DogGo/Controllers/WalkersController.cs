using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

//after creating WalkerController, go to IWalkerRepository
namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        //added this------------------------ Added a private field for IWalkerRepository and a constructor
        private readonly IWalkerRepository _walkerRepo;
        //added this for profile view:
        private readonly IOwnerRepository _ownerRepo;
        private readonly IDogRepository _dogRepo;
        //private readonly IWalkRepository _walkRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository, IOwnerRepository ownerRepository, IDogRepository dogRepository) //, IWalkRepository walkRepository
        {
            _walkerRepo = walkerRepository;
            _ownerRepo = ownerRepository;
            _dogRepo = dogRepository;
            //_walkRepo = walkRepository;
        }

        //end------------------------


        // GET: Walkers
        //When a user is on localhost:5001/Walkers, we want to show them a view that contains a list of all the walkers in our system.
        //This code will get all the walkers in the Walker table, convert it to a List and pass it off to the view.
        public ActionResult Index()
        {
           
            if (User.Identity.IsAuthenticated)
            {
                //List<Walker> walkers = _walkerRepo.GetAllWalkers(); //Dont want to list all walkers anymore, only those in owner's neighborhood

                //add this 2 lines after created new helper method SCROLL DOWN AT THE BOTTOM OF WALKERSCONTROLLER:GetCurrentUserId()
                int ownerId = GetCurrentUserId();

                Owner owner = _ownerRepo.GetOwnerById(ownerId); //Hint: Use the OwnerRepository to look up the owner by Id before getting the walkers.
                List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);//to only show walkers in owner's neighborhod //removed List<Walker> at the beginning because declared at the top above if
                return View(walkers);
            }
            else
            {
                // For unauthenticated users, get all walkers
                List<Walker> walkers = _walkerRepo.GetAllWalkers(); //get error walkers not exist in this context so decalre List of walkers above if statement
                return View(walkers);
            }
            
          
        }


        // GET: Walkers/Details/5
        //rl is walkers/details/2, the framework will invoke the Details method and pass in the value 2. The code looks in the database for a walker with the id of 2. If it finds one, it will return it to the view. If it doesn't the user will be given a 404 Not Found page.
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
           // List<Walks> walks = _walkRepo.GetWalksByWalkerId(walker.Id);

            if (walker == null)
            {
                return NotFound();
            }

            //return View(walker);


            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                //LHS: check WalkerProfileViewModel for name, RHS:check Repo and repo method names
                Walker = walker,
                Walks = _walkerRepo.GetRecentWalksByWalkerId(id),

                TotalWalkTime = _walkerRepo.CalculateTotalWalkTime(id)


            };
            return View(vm);
        }

        //Right click the Details method and select Add View. Select 'Razor View' and click Add. Keep the name "Details", select "Details" for the Template dropdown, and select "Walker" for the model class. Make the same changes in the view as before and replace the image url with the image tag




        // GET: Walkers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Walkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Walkers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Walkers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Walkers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Walkers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }






        //// GET: Walkers/SelectWalker/5
        //public ActionResult SelectWalker(int id)
        //{
        //    // Retrieve the selected walker
        //    Walker walker = _walkerRepo.GetWalkerById(id);

        //    // Get all dogs for the current user
        //    List<Dog> dogs = _dogRepo.GetAllDogs();

        //    if (walker == null || dogs == null)
        //    {
        //        return NotFound();
        //    }

        //    // Create a ViewModel to pass data to the view
        //    var viewModel = new ScheduleWalkViewModel
        //    {
        //        Walker = walker,
        //        Dogs = dogs,
        //        DateTime = DateTime.Now // Default to current date and time
        //    };

        //    return View(viewModel);
        //}


        //// POST: Walkers/ScheduleWalk
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ScheduleWalk(ScheduleWalkViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Create a new walk with the requested status
        //        var newWalk = new Walks
        //        {
        //            WalkerId = model.Walker.Id,
        //            DogId = model.DogId,
        //            Date = model.DateTime,
        //            Duration = model.Duration, // Make sure to include duration if necessary
        //            Status = "Requested" // Set status as "Requested"
        //        };

        //        // Save the new walk to the database (you'll need a method in your repository for this)
        //        //_walkRepo.AddWalk(newWalk);

        //        return RedirectToAction(nameof(Index));
        //    }

        //    // If the model state is not valid, re-display the form
        //    return View(model);
        //}









        //ADDED for only see walkers in your neighborhood:
        // Method to get the current user's ID
        //u need to Get id of the current logged in user many times in our controller, so create own helper method:
        //logged in owner can only see their own dogs
        //so get current user id
        //ADD WHERE in sql querry
        private int GetCurrentUserId()
        {
            
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }


       
    }
}
