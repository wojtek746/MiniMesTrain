using Microsoft.AspNetCore.Mvc;

namespace MiniMesTrainApi.Controllers
{
    [Route("greeting")]
    public class GreetingController : Controller
    {
        [HttpGet]
        [Route("hello/{name}")]
        public IActionResult HelloWorld([FromRoute] string name)
        {
            string message = $"Hello {name}";
            //Console.WriteLine(message);
            return Ok(message);
        }

        [HttpGet]
        [Route("car")]
        public IActionResult GetCar()
        {
            string brand = "Toyota";
            string model = "Corolla";
            string color = "Red";
            Car myCar = new()
            {
                Brand = brand,
                Model = model,
                Color = color
            };
            return Ok(myCar);
        }

        [HttpPost]
        [Route("start")]
        public IActionResult StartCar([FromBody] Car car)
        {
            var message = car.Start();
            return Ok(message);
        }

        [HttpPut]
        [Route("paint")]
        public IActionResult Paint([FromBody] Car car, [FromQuery] string color)
        {
            var message = car.Paint(color);
            return Ok(message);
        }

        [HttpDelete]
        [Route("crash")]
        public IActionResult Crash([FromBody] Car car)
        {
            var message = car.Crash();
            return Ok(message);
        }
    }

    public class Car
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }

        public string Start()
        {
            var message = $"{Color} {Brand} {Model} drove away.";
            //Console.WriteLine(message);
            return message;
        }

        public string Paint(string newColor)
        {
            var message = $"{Color} {Brand} {Model} was repainted {newColor}.";
            Color = newColor;
            //Console.WriteLine(message);
            return message;
        }

        public string Crash()
        {
            var message = $"{Color} {Brand} {Model} crashed!";
            //Console.WriteLine(message);
            return message;
        }
    }
}
