using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using NUnit.Framework;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IntegrationTest1
    {
        private IButton PowerButton;
        private IButton TimeButton;
        private IButton startCancel;

        private IDoor Door;

        private IDisplay Display;
        private ICookController CookControl;
        private ILight Light;

        private IUserInterface UserInterface;

        [SetUp]
        public void setup()
        {
            PowerButton = new Button();
            TimeButton = new Button();
            startCancel = new Button();

            Door = new Door();

            Display = Substitute.For<IDisplay>();
            CookControl = Substitute.For<ICookController>();
            Light = Substitute.For<ILight>();

            UserInterface = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, CookControl);
        }
        //billede 2
        [Test]
        public void UserInterfaceButton()
        {

            PowerButton.Press();
            Display.Received().ShowPower(50);

        }
    }
}