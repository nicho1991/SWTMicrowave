using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Interfaces;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using NUnit.Framework;
using NSubstitute;
using System.Threading;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{

    class IntergrationTest3
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

        [SetUp]
        public void setup()

        {
            Door = new Door();
            PowerButton = new Button();
            TimeButton = new Button();
            startCancel = new Button();
            timer = new Timer();
            output = Substitute.For<IOutput>();
            Display = new Display(output);
            powerTube = new PowerTube(output);
            Light = new Light(output);
           // UI = Substitute.For<IUserInterface>();
            cookcontroller = new CookController(timer, Display, powerTube, UI);

            UI = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, cookcontroller);

        }

        [Test]

        public void StartCooking()
        {
            cookcontroller.StartCooking(80, 3000);

        }

        [TestCase(0, 3000)]
        [TestCase(101, 3000)]

        public void StartCookingException(int power, int time)
        {
            Assert.That(() => cookcontroller.StartCooking(power, time), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]

        public void AllreadyOnException()
        {
            powerTube.TurnOn(80);

            Assert.That(() => cookcontroller.StartCooking(80, 3000), Throws.TypeOf<ApplicationException>());
        }

        [TestCase(3001, 3)]
        [TestCase(3000, 2)]

        public void LoggerIsCalled(int time, int ticks)
        {
            cookcontroller.StartCooking(80, time);
            Thread.Sleep(time + 3);
            output.Received(ticks + 1);
        }

        [Test]

        public void TimeExpired()
        {
            mut.WaitOne();

            cookcontroller.StartCooking(50, 999);
            Thread.Sleep(1222);
            output.Received().OutputLine("PowerTube turned off");

            mut.ReleaseMutex();

            
        }

        [Test]
        public void PrintToUI()
        {
            mut.WaitOne();

            cookcontroller.StartCooking(50, 999);
            Thread.Sleep(1100);
            UI.Received().CookingIsDone();

            mut.ReleaseMutex();
        }

        [Test]

        public void ClearDisp()
        {
            mut.WaitOne();

            cookcontroller.StartCooking(50, 999);
            Thread.Sleep(1000);
            cookcontroller.Stop();
            output.Received().OutputLine("PowerTube turned off");

            mut.ReleaseMutex();
        }


        [Test]

        public void LightOnIfDoorOpen()
        {
            UI.OnDoorOpened(this, EventArgs.Empty);  
            output.Received().OutputLine("Light is turned on");
        }

        [Test]

        public void LightOffIfDoorClose()
        {
            UI.OnPowerPressed(this, EventArgs.Empty);
            UI.OnTimePressed(this, EventArgs.Empty);
            UI.OnDoorClosed(this, EventArgs.Empty);
            output.Received().OutputLine("Light is turned off");
        }

        [Test]
        public void CancelIsPressed_LightTurnsOn()
        {
            UI.OnPowerPressed(this, EventArgs.Empty);
            UI.OnTimePressed(this, EventArgs.Empty);
            UI.OnStartCancelPressed(this, EventArgs.Empty);

            output.Received().OutputLine("Light is turned on");
        }

        [Test]
        public void CookDone_LightsOff()
        {
            UI.OnPowerPressed(this, EventArgs.Empty);
            UI.OnTimePressed(this, EventArgs.Empty);
            UI.CookingIsDone();

            output.Received().OutputLine("Light is turned off");
        }


    }
}
