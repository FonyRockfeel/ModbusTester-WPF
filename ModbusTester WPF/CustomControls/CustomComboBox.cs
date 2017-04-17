using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModbusTester_WPF.CustomControls
{
    /// <summary>
    /// Implements ICommandSource interface, Command property is available to be binded to ICommand member.
    /// </summary>
    class CustomComboBox: ComboBox, ICommandSource
    {
        public CustomComboBox()
        {
            EventManager.RegisterClassHandler(typeof(CustomComboBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnCustomMouseButtonDown), true);
        }

        private void OnCustomMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;

                if (command != null)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    ((ICommand)Command).Execute(CommandParameter);
                }
            }
        }

        /// <summary>
        /// Command for control
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
           DependencyProperty.Register(
               "Command",
               typeof(ICommand),
               typeof(CustomComboBox),
               new PropertyMetadata((ICommand)null,
               new PropertyChangedCallback(CommandChanged)));

        [Bindable(true), Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }





        /// <summary>
        /// CommandTArget for command
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register("CommandTarget",
            typeof(IInputElement),
            typeof(CustomComboBox));

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

      

        /// <summary>
        /// CommandParameter for Command
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty =
                       DependencyProperty.Register("CommandParameter",
                           typeof(object),
                           typeof(CustomComboBox));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }





        private EventHandler canExecuteChangedHandler;

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomComboBox cs = (CustomComboBox)d;
            cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
           
        }
       
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers.
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {

            EventHandler handler = new EventHandler(CanExecuteChanged);
            canExecuteChangedHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }
        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;

                // If a RoutedCommand.
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                // If a not RoutedCommand.
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
            }
        }

        protected void OnValueChanged(double oldValue, double newValue)
        {
            if (this.Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;

                if (command != null)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    ((ICommand)Command).Execute(CommandParameter);
                }
            }
        }
       



    }
}
