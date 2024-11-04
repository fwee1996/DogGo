using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class OwnersController : Controller
    {
        //added this------------------------ Added a private field for IOwnerRepository and a constructor 
        private readonly IOwnerRepository _ownerRepo;

        //chapter 3 add this: bcs Owner Details view will need to know about more than just the owner, we'll need access to other repositories. 
        private readonly IDogRepository _dogRepo;
        private readonly IWalkerRepository _walkerRepo;
        private readonly INeighborhoodRepository _neighborhoodRepo;



        // ASP.NET will give us an instance of our Owner Repository. This is called "Dependency Injection"
        public OwnersController(IOwnerRepository ownerRepository, IDogRepository dogRepository,
    IWalkerRepository walkerRepository, INeighborhoodRepository neighborhoodRepo)
        {
            _ownerRepo = ownerRepository;
            _dogRepo = dogRepository;
            _walkerRepo = walkerRepository;
            _neighborhoodRepo = neighborhoodRepo;
        }

        //end------------------------

        // GET: OwnersController
        public ActionResult Index()
        {
            List<Owner> owners = _ownerRepo.GetAllOwners(); //Update the Program class to tell ASP.NET about the OwnerRepository.

            return View(owners);
        }

        // GET: OwnersController/Details/5
        public ActionResult Details(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(owner.Id);
            List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);

            if (owner == null)
            {
                return NotFound();
            }

            //return View(owner);

            OwnerProfileViewModel vm = new OwnerProfileViewModel()
            {
                Owner = owner,
                Dogs = dogs,
                Walkers = walkers
            };

            return View(vm);

            //if you try run code by going to /owners/details/1, InvalidOperationException: The model item passed into the ViewDataDictionary is of type 'DogWalker.Models.ViewModels.ProfileViewModel', but this ViewDataDictionary instance requires a model item of type 'DogWalker.Models.Owner' 
            //so fo to file Details.cshtml, change the first line of  to this: @model DogGo.Models.ViewModels.ProfileViewModel
        }

        //if you notice there's 2 create functions:
        //GET create is like getting a form from clerk
        // GET: OwnersController/Create
        //Right click the "Create" method name and select "Add View". Name the view "Create", give it the template of "Create", and the model class Owner
        // Since the only thing the server needs to do is hand the user a blank html form,GET Create is actually fine as return View(). The only thing we have to do is create that html form. Right click the "Create" method name and select "Add View". Name the view "Create", give it the template of "Create", and the model class Owner
        public ActionResult Create()
        {
            //Update the GET Create method to now create a view model and pass it to the view
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();

            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };
            return View(vm);  //Now go to CReate.cshtml to update
        }

        //POST create is like you filled up the form and you submit it back to the clerk - same url
        // POST: OwnersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Owner owner)
        {
            try
            {
                _ownerRepo.AddOwner(owner);
                return RedirectToAction("Index");//MAKE SURE CHANGE TO "Index"
            }
            catch (Exception ex)
            {
                return View(owner);
            }
        }

        // GET: OwnersController/Edit/5
        //edit form is like create form but prepopulated with data
        public ActionResult Edit(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);
            if (owner == null)
            {
                return NotFound();
            }
            //added this for dropdown neighborhood selection:

            var viewModel = new OwnerFormViewModel
            {
                Owner = owner,
                Neighborhoods = _neighborhoodRepo.GetAllNeighborhoods() // Fetch neighborhoods
            };


            return View(viewModel);
        }

        // POST: OwnersController/Edit/5
        //Post Edit is same as Post Create except we are updating the database instead of inserting into.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, OwnerFormViewModel viewModel)
        {
            try
            {
                viewModel.Owner.Id = id;
                _ownerRepo.UpdateOwner(viewModel.Owner);
                return RedirectToAction("Index");
            }


            catch (Exception ex)
            {
                return View(viewModel);
            }
        }



        // GET: OwnersController/Delete/5
        //Notice that the GET method for Delete accepts an int id parameter. ASP.NET will get this value from the route. i.e. owners/delete/5 suggests that the user is attempting to delete the owner with Id of 5. Let's assume that owner with the Id of 5 is Mo Silvera. We want the view to have some text on it that says "Are you sure you want to delete the owner Mo Silvera?"

        //right clicking the method name and selecting Add View. Keep the name of the template "Delete", choose "Delete" from the template dropdown, and set the model to Owner.

        public ActionResult Delete(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);
            return View(owner);
        }

        // POST: OwnersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Owner owner)
        {
            try
            {
                _ownerRepo.DeleteOwner(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(owner);
            }
        }

        //GET Login
        public ActionResult Login()
        {
            return View();
        }

        //POST Login
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            // Look up an owner by their email address using the repository
            Owner owner = _ownerRepo.GetOwnerByEmail(viewModel.Email);

            // If no owner is found with the given email, return Unauthorized response
            if (owner == null)
            {
                return Unauthorized(); //HTTP 401 Unauthorized response.
            }

            //Claim(object)=user info
            List<Claim> claims = new List<Claim>
    {//The server populates that license with whatever information it chooses--in this case our code is choosing to add the owner's Id, email address, and role.
        new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()), //unique identifier for the user ( user ID).
        new Claim(ClaimTypes.Email, owner.Email),
        new Claim(ClaimTypes.Role, "DogOwner"), //user's role or group
    };
            // Create a ClaimsIdentity object with the claims and the authentication scheme
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);


            //EXPLANATION:AWAIT pauses the execution of the Login method until SignInAsync completes. This ensures that the authentication cookie is properly created and sent to the user’s browser before moving on to the next line of code.
            //AWAIT:writing data to a cookie and interacting with the authentication system takes time, so other process can happen at the same time the authentication process is going on
            // Sign in the user: CREATE authentication COOKIE with the claims identity
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, //It’s a predefined string used to identify the authentication method. For cookies, it’s typically "Cookies".
                new ClaimsPrincipal(claimsIdentity));

            // Redirect the user to the "Index" action of the "Dogs" controller upon successful login
            return RedirectToAction("Index", "Dogs");
        }

        public async Task<ActionResult> Logout()
        {
            // Sign out the user by removing the authentication cookie 
            await HttpContext.SignOutAsync();
            // Redirect the user to the "Index" action of the "Home" controller after logout
            // The await keyword pauses the execution of the Logout method until SignOutAsync completes. This ensures that the user’s authentication cookie is properly removed before redirecting them.
            //removing cookies and updating the authentication state takes time . Using await allows these operations to complete without blocking the thread (execution within a process.Process is many threads)
            return RedirectToAction("Index", "Home");
        }
    }
}
