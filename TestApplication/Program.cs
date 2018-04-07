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
        private ICookController cookcontroller;

        private IButton PowerButton;
        private IButton TimeButton;
        private IButton startCancel;
        private IUserInterface UI;
        private ILight Light;
        private IDisplay Display;
        private IPowerTube powerTube;
        private ITimer timer;
        private IOutput output;
        private IDoor Door;
        private static Mutex mut = new Mutex();
   
        public void setup()
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

        }
        static void Main(string[] args)
        {
            System.Console.WriteLine("Tast enter når applikationen skal afsluttes");
            System.Console.ReadLine();
        }
    }
}