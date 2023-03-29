using Employee_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Web;

namespace Employee_Management_System.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        #region Declarations
        public static String SqlConnectionString = "";

        public String SecretKey = ""; // Key Used for encrypted/decryption (passwords/cookies/etc..)
        public string WebRootPath = ""; //Website absolute path


        #endregion Declarations

        #region Constructor
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public object JsonRequestBehavior { get; private set; }

        public HomeController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment; _configuration = configuration;
            SqlConnectionString = configuration.GetConnectionString("SqlConnectionString");
            SecretKey = configuration.GetValue<String>("SecretKey");
            WebRootPath = _webHostEnvironment.WebRootPath;
        }
        #endregion Constructor 

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add_Employee()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add_Employee(IFormCollection form)
        {
            using (CommonCLibrary.DAL db = new CommonCLibrary.DAL(_configuration))
            {
                string name     = form["name"];
                string address  = form["address"];
                string dob      = form["dob"];
                string gender   = form["gender"];
                List<SqlParameter> sqlParams = new List<SqlParameter>();                sqlParams.Add(new SqlParameter("@name", name));
                sqlParams.Add(new SqlParameter("@address", address));
                sqlParams.Add(new SqlParameter("@dob", dob));
                sqlParams.Add(new SqlParameter("@gender", gender));
                DataSet dsResult = db.SqlDataSetResult("USP_Addemp", sqlParams);
                string s = dsResult.Tables[0].Rows[0]["message"].ToString();
                TempData["Message"] = s;
                return View();

            }
            


        }
        public JsonResult EMP_exist(string name)
        {
            using (CommonCLibrary.DAL db = new CommonCLibrary.DAL(_configuration))
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>(); 
                sqlParams.Add(new SqlParameter("@name", name));
               
                DataSet dsResult = db.SqlDataSetResult("USP_EMP_exist", sqlParams);
                string result = dsResult.Tables[0].Rows[0]["count"].ToString();

                return Json(result);

            }

        }


        public JsonResult Deleteemp(string EMP_DocNo)
        {
            using (CommonCLibrary.DAL db = new CommonCLibrary.DAL(_configuration))
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                sqlParams.Add(new SqlParameter("@EMP_DocNo", EMP_DocNo));

                DataSet dsResult = db.SqlDataSetResult("USP_Deleteemp", sqlParams);

                return Json(true);

            }

        }


        


        public IActionResult EditEMP(String EMP_DocNo)
        {
            ViewBag.EMP_DocNo = EMP_DocNo;
            return View();
        }
        [HttpPost]
        public IActionResult EditEMP(IFormCollection form)
        {
            using (CommonCLibrary.DAL db = new CommonCLibrary.DAL(_configuration))
            {
                string name = form["name"];
                string address = form["address"];
                string dob = form["dob"];
                string gender = form["gender"]; 
                string docno = form["docno"];
                List<SqlParameter> sqlParams = new List<SqlParameter>(); 
                sqlParams.Add(new SqlParameter("@name", name));
                sqlParams.Add(new SqlParameter("@address", address));
                sqlParams.Add(new SqlParameter("@dob", dob));
                sqlParams.Add(new SqlParameter("@gender", gender));
                sqlParams.Add(new SqlParameter("@docno", docno));
                DataSet dsResult = db.SqlDataSetResult("USP_Editemp", sqlParams);
                string s = dsResult.Tables[0].Rows[0]["message"].ToString();
                TempData["Message"] = s;
                return RedirectToAction("ViewEmployee");

            }



        }






        public IActionResult ViewEmployee()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}