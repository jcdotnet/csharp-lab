using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class BookController : Controller
    {
        [Route("/books")]
        public IActionResult Index()
        {
            return Content("<h1>Books</h1>", "text/html");
        }
    
        // url example: http://localhost:5035/books/1
        [Route("/books/{id}")]
        public IActionResult Find(Book book)
        {
            int? bookId = book.Id;
            Console.WriteLine(bookId);
            // id logic goes here...
            return File("/pg1513.txt", "text/plain"); // project gutenberg free book
        }

        // url example: http://localhost:5035/bookstore/?isloggedin=true&bookid=1
        // moved to http://localhost:5035/books/1
        [Route("/bookstore")]
        [Route("/store/books")]
        // model binding: bookid and isloggedin are nullable (= Nullable<T> = can be null)
        // FromQuery is optional here because we are binding from the query string only...
        // and not from the route data (to bind id from the route data we use [FromRoute]) 
        public IActionResult FindLegacy([FromQuery]int? bookid, bool? isloggedin) 
        {
            // validating here (not in the model, see User) for learning purposes
            if (!bookid.HasValue) // Nullable<T> property
            {
                // Response.StatusCode = 400;
                // return new BadRequestResult();
                return BadRequest("Book id not supplied"); ;
            }

            // checking id here rather than in router validation to avoid non-reachable code
            // since we have a /books route and no id value will match with the /books route
            if (bookid < 0 || bookid > 1000)
            {
                // Response.StatusCode = 400;
                return BadRequest("Book id is not valid");
            }

            // we can use model binding as well
            // 
            if (isloggedin == false)
            {
                // Response.StatusCode = 401;
                // return Content("You must be authenticated to get this book");
                return Unauthorized("You must be authenticated to get this book");
            }

            // redirection (several ways)
            // 302
            // return new RedirectToActionResult("Find", "Book", new { id = bookid });
            // return RedirectToAction("Find", "Book", new { id = bookid });
            return new LocalRedirectResult($"/books/{bookid}");
            // return Redirect($"books/{bookId}");

            //301
            // return new RedirectToActionResult("Find", "Book", new { id = bookid }, permanent: true); 
            // return RedirectToActionPermanent("Find", "Book", new { id = bookid });
            // return new LocalRedirectResult($"/books/{bookid}", true);
            // return LocalRedirectPermanent($"/books/{bookid}");
            // return RedirectPermanent($"books/{bookid}"); 
        }
    }
}