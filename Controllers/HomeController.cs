using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dojodachi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Dojodachi.Controllers
{
    public static class SessionExtensions
    {
    // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // This helper function simply serializes theobject to JSON and stores it as a string in session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        
        // generic type T is a stand-in indicating that we need to specify the type on retrieval
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // Upon retrieval the object is deserialized based on the type we specified
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("Fullness") >= 100 && 
                    HttpContext.Session.GetInt32("Energy") >= 100 && 
                    HttpContext.Session.GetInt32("Happiness") >= 100  )
            {
                    int? Fullness = HttpContext.Session.GetInt32("Fullness");
                    int? Happiness = HttpContext.Session.GetInt32("Happiness");
                    int? Meal = HttpContext.Session.GetInt32("Meal");
                    int? Energy = HttpContext.Session.GetInt32("Energy");
                    string img = "win.gif";
                    HttpContext.Session.SetString("image", img);
                    string image = HttpContext.Session.GetString("image");
                    ViewBag.image = image;
                    ViewBag.fullness = Fullness;
                    ViewBag.happiness = Happiness;
                    ViewBag.meal = Meal;
                    ViewBag.energy = Energy;
                    ViewBag.result = "You Win";
                    TempData["message"] = "Your dojodachi survived, You Win!!!";
                    return View("result", TempData["message"]);
            }else if(HttpContext.Session.GetInt32("Fullness") == 0 || 
                    HttpContext.Session.GetInt32("Happiness") == 0)
            {
                    int? Fullness = HttpContext.Session.GetInt32("Fullness");
                    int? Happiness = HttpContext.Session.GetInt32("Happiness");
                    int? Meal = HttpContext.Session.GetInt32("Meal");
                    int? Energy = HttpContext.Session.GetInt32("Energy");
                    string img = "lose.gif";
                    HttpContext.Session.SetString("image", img);
                    string image = HttpContext.Session.GetString("image");
                    ViewBag.image = image;
                    ViewBag.fullness = Fullness;
                    ViewBag.happiness = Happiness;
                    ViewBag.meal = Meal;
                    ViewBag.energy = Energy;
                    ViewBag.result = "You Lose";
                    TempData["message"] = "it passed away, so sad.......";
                    return View("result" ,TempData["message"]);
            }

            if(HttpContext.Session.GetInt32("Fullness") == null){
                HttpContext.Session.SetInt32("Fullness", 20);
                HttpContext.Session.SetInt32("Happiness", 20);
                HttpContext.Session.SetInt32("Meal", 3);
                HttpContext.Session.SetInt32("Energy", 50);
                string img = "home.gif";
                HttpContext.Session.SetString("image", img);
                int? Fullness = HttpContext.Session.GetInt32("Fullness");
                int? Happiness = HttpContext.Session.GetInt32("Happiness");
                int? Meal = HttpContext.Session.GetInt32("Meal");
                int? Energy = HttpContext.Session.GetInt32("Energy");
                string image = HttpContext.Session.GetString("image");
                ViewBag.fullness = Fullness;
                ViewBag.happiness = Happiness;
                ViewBag.meal = Meal;
                ViewBag.energy = Energy;
                ViewBag.image = image;
                return View("index");
            
            }else{
                int? Fullness = HttpContext.Session.GetInt32("Fullness");
                int? Happiness = HttpContext.Session.GetInt32("Happiness");
                int? Meal = HttpContext.Session.GetInt32("Meal");
                int? Energy = HttpContext.Session.GetInt32("Energy");
                string image = HttpContext.Session.GetString("image");
                ViewBag.fullness = Fullness;
                ViewBag.happiness = Happiness;
                ViewBag.meal = Meal;
                ViewBag.energy = Energy;
                ViewBag.image = image;
                return View("index" ,TempData["message"]);
            }
        }

        [HttpPost]
        [Route("feed")]
        public IActionResult Feed(){
            // Feeding cost one meal and loss random amount of fullness in 5 - 10
            Random rand = new Random();
            int AddFull = rand.Next(5,11);
            int? fullness = HttpContext.Session.GetInt32("Fullness");
            int? LossMeal = HttpContext.Session.GetInt32("Meal");

            if(LossMeal > 0 ){
                int Fullness = fullness.GetValueOrDefault() + AddFull;
                int meal = LossMeal.GetValueOrDefault() - 1;
                HttpContext.Session.SetInt32("Meal", meal);
                HttpContext.Session.SetInt32("Fullness", Fullness);
                //image
                TempData["message"] = "Cost 1 meal increase fullness by " + AddFull;
                string img = "eat.gif";
                HttpContext.Session.SetString("image", img);
            }else{
                string img = "need_food.gif";
                HttpContext.Session.SetString("image", img);
                TempData["message"] = "No food to feed your dojodachi";
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("play")]
        public IActionResult Play(){
            Random rand = new Random();
            int happy = rand.Next(5,11);
            int like = rand.Next(1,5);
            int pick = rand.Next(1,3);
            int? Fullness = HttpContext.Session.GetInt32("Fullness");
            int? Happiness = HttpContext.Session.GetInt32("Happiness");
            int? Meal = HttpContext.Session.GetInt32("Meal");
            int? Energy = HttpContext.Session.GetInt32("Energy");
            if(like == 1){
                // 25% chances
                if(pick == 1){
                    //decrease Energy
                    int lossEnergy = Energy.GetValueOrDefault() - 5;
                    TempData["message"] = "it dont want to play, loss 5 energy";
                    string img = "refuse.gif";
                    HttpContext.Session.SetString("image", img);
                }else if (pick == 2){
                    //decrease Meal
                    int lossMeal = Meal.GetValueOrDefault() - 1;
                    TempData["message"] = "it dont want top play, loss 1 meal";
                    string img = "refuse.gif";
                    HttpContext.Session.SetString("image", img);
                }
            }
            else {
                // 75% chances
                if(Energy > 0)
                {
                    // Play cost 5 energy and gain random amount of happiness 5 - 10
                    int LossEnergy = Energy.GetValueOrDefault() - 5;
                    int gainHappy = Happiness.GetValueOrDefault() + happy;
                    if(LossEnergy < 0){
                        LossEnergy = 0;
                        
                    }   
                    if(gainHappy >100){
                        gainHappy = 100;
                    }
                    TempData["message"] = "Playing loss 5 energy but gain " + happy + " happiness";
                    HttpContext.Session.SetInt32("Happiness", gainHappy);      
                    HttpContext.Session.SetInt32("Energy", LossEnergy);
                    string img = "play.gif";
                    HttpContext.Session.SetString("image", img);
                    
                    }else{
                        TempData["message"] = "No energy to play";
                        string img = "no_energy.gif";
                        HttpContext.Session.SetString("image", img);
                    }
                }
            return RedirectToAction("Index");             

        }

        [HttpPost]
        [Route("work")]
        public IActionResult Work(){
            // working cost 5 energy and randomly earn 1 - 3 meals
            Random rand = new Random();
            int EarnMeal = rand.Next(1,4);
            int? Meal = HttpContext.Session.GetInt32("Meal");
            int? Energy = HttpContext.Session.GetInt32("Energy");

            if(Energy > 0){
                int LossEnergy = Energy.GetValueOrDefault() - 5;
                int Earn = Meal.GetValueOrDefault() + EarnMeal;
                if(LossEnergy < 0){
                    LossEnergy = 0;
                }
                HttpContext.Session.SetInt32("Meal", Earn);
                HttpContext.Session.SetInt32("Energy",LossEnergy);
                TempData["message"] = "Loss 5 energy and earn " + EarnMeal + " Meal"; 
                string img = "work.gif";
                    HttpContext.Session.SetString("image", img);
            }else{
                TempData["message"] = "No Energy!!! Need some sleep?"; 
                string img = "no_energy.gif";
                    HttpContext.Session.SetString("image", img);
            }
             
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Route("sleep")]
        public IActionResult Sleep(){
            // sleep earn 15 energy and loss fullness and happiness by 5
            int? Fullness = HttpContext.Session.GetInt32("Fullness");
            int? Happiness = HttpContext.Session.GetInt32("Happiness");
            int? Energy = HttpContext.Session.GetInt32("Energy");

            if(Fullness > 0 || Happiness > 0){
                int EarnEnergy = Energy.GetValueOrDefault() + 15;
                int hungry = Fullness.GetValueOrDefault() - 5;
                int unhappy = Happiness.GetValueOrDefault() - 5;

                if(hungry < 0){
                    hungry = 0;
                }

                if(unhappy < 0){
                    unhappy = 0;
                }
                HttpContext.Session.SetInt32("Happiness", unhappy);
                HttpContext.Session.SetInt32("Fullness", hungry);
                HttpContext.Session.SetInt32("Energy", EarnEnergy);
                TempData["message"] = "Sleeping ZZZ~~~ add 15 Energy and loss fullness and Happiness by 5 "; 
                string img = "sleep.gif";
                HttpContext.Session.SetString("image", img);
            }else{
                TempData["message"] = " Full Energy !!! Don't sleep!!!"; 
                string img = "home.gif";
                HttpContext.Session.SetString("image", img);
            }                    
            return RedirectToAction("Index");

        }

        [HttpPost]
        [Route("result")]
        public IActionResult Result(){
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            
        }


    }
}
