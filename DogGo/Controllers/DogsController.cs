using DogGo.Models;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class DogsController : Controller
    {
        //added this------------------------ Added a private field for IDogRepository and a constructor
        private readonly IDogRepository _dogRepo;

        // ASP.NET will give us an instance of our Dog Repository. This is called "Dependency Injection"
        public DogsController(IDogRepository dogRepository)
        {
            _dogRepo = dogRepository;
        }

        //end------------------------

        //PROTECT ROUTES: /dogs and /dogs/create shouldnt be accessible by non logged in users
        //put an [Authorize] above action that user must be logged in to see
        //instead they'll be rerouted to login page
        [Authorize]
        // GET: DogsController
        public ActionResult Index()
        {
            //add this 2 lines after created new helper method at the bottom:GetCurrentUserId()
            int ownerId = GetCurrentUserId();
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);

            //List<Dog> dogs = _dogRepo.GetAllDogs(); //Update the Program class to tell ASP.NET about the DogRepository.
            return View(dogs);
        }

        // GET: DogsController/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            if (dog == null)
            {
                return NotFound();
            }
            return View(dog);
        }

        // GET: DogsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //put an [Authorize] above action that user must be logged in to see
        public ActionResult Create(Dog dog)
        {
            try
            {
                //This line added for logged in owner to see ONLY their dog:
                // update the dogs OwnerId to the current user's Id
                dog.OwnerId = GetCurrentUserId();

                _dogRepo.AddDog(dog);
                return RedirectToAction("Index");//MAKE SURE CHANGE TO "Index"
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

        [Authorize]
        // GET: DogsController/Edit/5
        public ActionResult Edit(int id)
        {
            

            //add this for edit only logged in user's dogs
            int ownerId = GetCurrentUserId();
            

            Dog dog = _dogRepo.GetDogById(id);

            //ADDED CURRENT USER CHECK:|| dog.OwnerId != ownerId:

            if (dog == null || dog.OwnerId != ownerId)
            {
                return NotFound();
            }
            return View(dog);
        }

        // POST: DogsController/Edit/5
        //click on method name to create view: Edit.cshtml change id form group to type hidden. (create can remove, edit must keep but hidden otherwise id default to 0)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            try
            {
                //added these two lines for edit only logged in user's dogs:
                //get current user id and set dog's owner id to current user!
                int ownerId = GetCurrentUserId();
                // Update the dog object with the current user's ID
                dog.OwnerId = ownerId;

                // Update the dog in the repository
                _dogRepo.UpdateDog(dog);

                // Redirect to the index view
                return RedirectToAction("Index"); //MAKE SURE CHANGE TO "Index"
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

        [Authorize]
        // GET: DogsController/Delete/5
        public ActionResult Delete(int id)
        {
            //add this for edit only logged in user's dogs
            int ownerId = GetCurrentUserId();

            Dog dog = _dogRepo.GetDogById(id);

            //ADDED CURRENT USER CHECK for logged in user accessing their OWN dogs:|| dog.OwnerId != ownerId:
            if (dog == null || dog.OwnerId != ownerId)
            {
                return NotFound();
            }
            return View(dog);
        }

        // POST: DogsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            try
            {
                //added these 2 lines for edit only logged in user's dogs:
                //get current user id and set dog's owner id to current user!
                int ownerId = GetCurrentUserId();
                // Update the dog object with the current user's ID
                dog.OwnerId = ownerId;

                _dogRepo.DeleteDog(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

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
