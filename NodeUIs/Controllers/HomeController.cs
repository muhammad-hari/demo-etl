using Microsoft.AspNetCore.Mvc;
using NodeUIs.Models;
using RestSharp.Authenticators;
using RestSharp;
using System.Diagnostics;
using System.Net;

namespace NodeUIs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
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


        public JsonResult SendActionQuery(Credentials credentials)
        {
            return Json(new { });
        }

        [HttpPost]
        public IActionResult CreateCredential([FromBody] Credentials credentials)
        {
            var client = new RestClient("http://localhost:8080/api/v1")
            {
                Authenticator = new HttpBasicAuthenticator("airflow", "airflow")
            };

            var request = new RestRequest("/connections", Method.Post);
            request.AddHeader("content-type", "application/json");
            request.AddJsonBody(credentials);
            client.Execute(request);

            var response = GetConnectionStatus(credentials);

            return Json(new { data = response.message });
        }

        [HttpPost]
        public IActionResult GetListCredential()
        {
            var client = new RestClient("http://localhost:8080/api/v1")
            {
                Authenticator = new HttpBasicAuthenticator("airflow", "airflow")
            };

            var request = new RestRequest("/connections", Method.Get);
            request.AddHeader("content-type", "application/json");
            var response = client.Execute<Responses>(request);

            return Json(new { data = response?.Data });
        }

        [HttpPost]
        public IActionResult TestConnection([FromBody] Credentials credentials)
        {
            var response = new Responses()
            {
                message = "failed to connect to server",
                status = false
            };

            if (credentials != null)
            {
                credentials.connection_id = "";
                credentials.password = "postgres";
            }
            var client = new RestClient("http://localhost:8080/api/v1")
            {
                Authenticator = new HttpBasicAuthenticator("airflow", "airflow")
            };

            var request = new RestRequest("/connections/test", Method.Post);
            request.AddJsonBody(credentials!);
            var rest = client.Execute<Responses>(request);
            response = rest?.Data;

            return Json(response.message);
        }


        [HttpPost]
        public IActionResult UploadFile([FromBody] Nodes node)
        {
            var rnd = new Random();
            int num = rnd.Next();
            string fileName = $"postgre_{num}.py";

            string tempQuery = $"CREATE TABLE temp_{node.nodeId}_{num} AS {node.query}";
            string text = System.IO.File.ReadAllText("D:/Git/Web/Angular/devtools-api/NodeUI/NodeUIs/wwwroot/template.py");
            text = text.Replace("[QUERY_ENV]", tempQuery);
            text = text.Replace("[CREDENTIAL_ID_ENV]", node.credentialId);
            text = text.Replace("[NODE_ID_ENV]", $"{node.nodeId}_{num}");
            text = text.Replace("[NODE_DESC_ENV]", node.nodeDescription);
            text = text.Replace("[NODE_TASK_ENV]", $"postgre_{node.nodeId}_{num}"); 
                
         
            System.IO.File.WriteAllText($"D:/Git/Web/Angular/devtools-api/NodeUI/NodeUIs/wwwroot/{fileName}", text);

            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("/dags/uploadFile", Method.Post);
            request.AddFile("file", $"D:/Git/Web/Angular/devtools-api/NodeUI/NodeUIs/wwwroot/{fileName}");
            var rest = client.Execute<Responses>(request);
                return Ok();
        }

        public Responses GetConnectionStatus(Credentials credentials)
        {
            var response = new Responses()
            {
                message = "failed to connect to server",
                status = false
            };

            if (credentials != null)
                credentials.password = "postgres";

            var client = new RestClient("http://localhost:8080/api/v1")
            {
                Authenticator = new HttpBasicAuthenticator("airflow", "airflow")
            };

            var request = new RestRequest("/connections/test", Method.Post);
            request.AddJsonBody(credentials!);
            var rest = client.Execute<Responses>(request);
            response = rest?.Data;

            return response!;
        }

        public JsonResult GetListDatabase(Credentials credentials)
        {
            return Json(new { });
        }

        public JsonResult GetListTable(Credentials credentials)
        {
            return Json(new { });
        }
    }
}