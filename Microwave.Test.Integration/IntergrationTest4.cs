//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MicrowaveOvenClasses.Interfaces;
//using MicrowaveOvenClasses.Boundary;
//using MicrowaveOvenClasses.Controllers;
//using NUnit.Framework;
//using NSubstitute;
//using System.Threading;
//using Timer = MicrowaveOvenClasses.Boundary.Timer;

//namespace Microwave.Test.Integration
//{

//    class IntergrationTest4
//    {
//        private ICookController cookController;
//        private IButton PowerButton;
//        private IButton TimeButton;
//        private IButton startCancel;
//        private IUserInterface UI;
//        private ILight Light;
//        private IDisplay Display;
//        private IPowerTube powerTube;
//        private ITimer timer;
//        private IOutput output;
//        private IDoor Door;
//        private static Mutex mut = new Mutex();
//        private int power;
//        private int min;
//        private int sec;

//        [SetUp]
//        public void setup()

//        {
//            Door = new Door();
//            PowerButton = new Button();
//            TimeButton = new Button();
//            startCancel = new Button();
//            timer = new Timer();
//            output = Substitute.For<IOutput>();
//            Display = new Display(output);
//            powerTube = new PowerTube(output);
//            Light = new Light(output);
//            // UI = Substitute.For<IUserInterface>();
//            cookController = new CookController(timer, Display, powerTube, UI);

//            UI = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, cookController);

//        }

//        [Test]
//        public void powerTubeOn()
//        {
            
//            cookController.StartCooking(50,3000);

//            output.Received().OutputLine("PowerTube works with 50 %");
//        }

//        [Test]

//        public void TimerExpired()
//        {
//            cookController.StartCooking(50, 999);
//            Thread.Sleep(1001);
//            output.Received().OutputLine("PowerTube turned off");
//        }

//        [Test]
//        public void PowerButtonPressed()
//        {
//            PowerButton.Press();
//            output.Received().OutputLine("Display shows: 50 W");
//        }

//        [Test]

//        public void TimeButtonPressed()
//        {
            
//            TimeButton.Press();

//            output.Received().OutputLine($"Display shows: {min:D2}:{sec:D2}");
//        }

//        [Test]
//        public void ClearDisplayWithText()
//        {
//            UI.OnStartCancelPressed(this,EventArgs.Empty);
//            output.Received().OutputLine("Display cleared");
//        }

//    }
//}
