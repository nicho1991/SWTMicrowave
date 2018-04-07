using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using Timer = System.Threading.Timer;

namespace Microwave.Application
{
    class Program
    {
        private static ICookController cookcontroller;

        private static IButton PowerButton;
        private static IButton TimeButton;
        private static IButton startCancel;
        private static IUserInterface UI;
        private static ILight Light;
        private static IDisplay Display;
        private static IPowerTube powerTube;
        private static ITimer timer;
        private static IOutput output;
        private static IDoor Door;
        private static Mutex mut = new Mutex();
   



        static void Main(string[] args)
        {
            Door = new Door();
            PowerButton = new Button();
            TimeButton = new Button();
            startCancel = new Button();
            timer = new MicrowaveOvenClasses.Boundary.Timer();
            output = new Output();
            Display = new Display(output);
            powerTube = new PowerTube(output);
            Light = new Light(output);
            cookcontroller = new CookController(timer, Display, powerTube, UI);

            UI = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, cookcontroller);


            Door.Open();
            Door.Close();

            System.Console.WriteLine("Tast enter når applikationen skal afsluttes");
            System.Console.ReadLine();
        }
    }
}