using System;
using System.Runtime.CompilerServices;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using NUnit.Framework;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IntegrationTest2
    {
        private IButton PowerButton;
        private IButton TimeButton;
        private IButton startCancel;

        private IDoor Door;

        private IDisplay Display;
        private ICookController CookControl;
        private ILight Light;
        private IPowerTube power;
        private ITimer timer;
        private IOutput output;
        private IUserInterface UserInterface;

        [SetUp]
        public void setup()
        {
            PowerButton = new Button();
            TimeButton = new Button();
            startCancel = new Button();

            Door = new Door();
            timer = Substitute.For<ITimer>();
            Display = Substitute.For<IDisplay>();
            power = Substitute.For<IPowerTube>();
            output = Substitute.For<IOutput>();
            CookControl = new CookController(timer, Display, power);
            Light = new Light(output);

            UserInterface = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, CookControl);
        }

        #region Light
        [Test]
        public void Light_Door_Open_IsOn()
        {
            Door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains("on")));
        }

        [Test]
        public void Light_Door_close_IsOff()
        {
            Door.Open();
            Door.Close();
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains("off")));
        }

        [Test]
        public void Light_Start_IsOn()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            //cooking should start and light start
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains("on")));
        }

        [Test]
        public void Light_StartAndCancel_IsOff()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            startCancel.Press();
            //light should go off and stop cook
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains("off")));
        }

        [Test]
        public void Light_Done_IsOff()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            //after a while cooking is done (timer stub)
            UserInterface.CookingIsDone();
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains("off")));
        }

        #endregion

        #region CookControl

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void CookControl_Start_TimerStart(int presses)
        {
            PowerButton.Press();
            for (int i = 0; i < presses; i++)
            {
                TimeButton.Press();
            }
            startCancel.Press();
            timer.Received().Start(presses*60); 
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void CookControl_Start_powerStart(int presses)
        {

            for (int i = 0; i < presses; i++)
            {
                PowerButton.Press();
            }
            TimeButton.Press();
            startCancel.Press();
            power.Received().TurnOn(presses*50);
        }

        [Test]
        public void CookControl_Cooking_OpensDoor_TimerStop()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Door.Open();

            timer.Received().Stop();
        }
        [Test]
        public void CookControl_Cooking_OpensDoor_powerStop()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Door.Open();

            power.Received().TurnOff();
        }

        [Test]
        public void CookControl_Cooking_cancel_timerStop()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Door.Open();

            timer.Received().Stop();
        }

        [Test]
        public void CookControl_Cooking_cancel_powerStop()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Door.Open();

            power.Received().TurnOff();
        }

        #endregion
    }
}