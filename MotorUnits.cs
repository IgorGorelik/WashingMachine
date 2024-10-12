using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Timers;

namespace WashingMachine
{
    public abstract class MotorUnit : SwitchableUnit
    {
        #region Properties
        protected TimeSpan CycleTime = TimeSpan.Zero;
        #endregion

        #region Construction
        public MotorUnit(TimeSpan cycleTime, MachineUnitType unitType, Point location, Size size) : base(unitType, location, size)
        {
            CycleTime = cycleTime;
        }
        #endregion

        #region Methods
        public abstract void ExecuteCycle();
        #endregion
    }

    public class SpinMotorUnit : MotorUnit
    {
        #region Properties
        #endregion

        #region Construction
        public SpinMotorUnit(TimeSpan cycleTime, Point location, Size size) : base(cycleTime, MachineUnitType.SpinMotor, location, size)
        {
        }
        #endregion

        #region Methods

        #endregion

        #region Overrides
        public override void ExecuteCycle()
        {
            Task.Run(async () =>
            {
                try
                {
                    TurnedOn = true;

                    TimeSpan CycleTimeRemaining = CycleTime;
                    while (!TokenSource.IsCancellationRequested && (CycleTimeRemaining.TotalSeconds != 0))
                    {
                        try
                        {
                            OnOperationStateChanged(new OperationStateChangedEventArgs(CycleTimeRemaining));

                            await Task.Delay(TimeSpan.FromSeconds(1));
                            CycleTimeRemaining = CycleTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
                        }
                        catch (Exception ex) { Logger.Instance.LogError(ex); }
                    }

                    OnExecutionFinished();
                }
                catch (Exception ex) { Logger.Instance.LogError(ex); }
            }, cancellationToken: TokenSource.Token);
        }

        protected override void UpdateUnitState(EventArgs e)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    var stateArgs = e as OperationStateChangedEventArgs;
                    var labelText = $"{UnitType} activated, {stateArgs.TimeRemaining} remaining";

                    UnitNameText = labelText;
                    UnitNameColor = (stateArgs.TimeRemaining.TotalSeconds % 2) == 1 ? Color.DarkGreen : Color.ForestGreen;
                }));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }
        #endregion

        #region Handlers
        private void CycleTimer_Finished(object sender, ElapsedEventArgs e)
        {
            OnExecutionFinished();
        }
        #endregion
    }

    public class WashMotorUnit : MotorUnit
    {
        #region Definitions
        public class WashMotorOperationStateChangedEventArgs : OperationStateChangedEventArgs
        {
            #region Properties
            public WashDirection Direction { get; set; } = WashDirection.Clockwise;
            public TimeSpan DirectionCycleRemaining { get; set; } = default;
            public int MiniCyclesRemaining { get; set; }
            #endregion

            #region Construction
            public WashMotorOperationStateChangedEventArgs(TimeSpan timeRemaining, WashDirection direction, TimeSpan miniCycleTimeRemaining, int miniCyclesRemaining) : base(timeRemaining)
            {
                Direction = direction;
                MiniCyclesRemaining = miniCyclesRemaining;
                DirectionCycleRemaining = miniCycleTimeRemaining;
            }
            #endregion
        }
        #endregion

        #region Enums
        public enum WashDirection
        {
            Clockwise,
            CounterClockwise
        }
        #endregion

        #region Properties
        protected WashDirection Direction = WashDirection.Clockwise;
        protected int MiniCyclesCount = 0;
        protected TimeSpan MiniCycleTime = default;
        #endregion

        #region Construction
        public WashMotorUnit(TimeSpan miniCycleTime, int miniCyclesCount, Point location, Size size) : base(TimeSpan.FromSeconds(miniCycleTime.TotalSeconds * miniCyclesCount), MachineUnitType.WashMotor, location, size)
        {
            MiniCyclesCount = miniCyclesCount;
            MiniCycleTime = miniCycleTime;
        }
        #endregion

        #region Methods
        protected void SwitchCycleDirection()
        {
            try
            {
                Direction = Direction == WashDirection.Clockwise ? WashDirection.CounterClockwise : WashDirection.Clockwise;
            }
            catch (Exception ex) { Logger.Instance.LogError(ex); }
        }
        #endregion

        #region Overrides
        public override void ExecuteCycle()
        {
            Task.Run(async () =>
            {
                try
                {
                    TurnedOn = true;

                    TimeSpan CycleTimeRemaining = CycleTime;
                    while (!TokenSource.IsCancellationRequested && (MiniCyclesCount != 0))
                    {
                        try
                        {
                            TimeSpan MiniCycleTimeRemaining = MiniCycleTime;
                            while (!TokenSource.IsCancellationRequested && (MiniCycleTimeRemaining.TotalSeconds != 0))
                            {
                                try
                                {
                                    OnOperationStateChanged(new WashMotorOperationStateChangedEventArgs(CycleTimeRemaining, Direction, MiniCycleTimeRemaining, MiniCyclesCount));

                                    await Task.Delay(TimeSpan.FromSeconds(1));

                                    MiniCycleTimeRemaining = MiniCycleTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
                                    CycleTimeRemaining = CycleTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
                                }
                                catch (Exception ex) { Logger.Instance.LogError(ex); }
                            }

                            SwitchCycleDirection();
                            MiniCyclesCount--;
                        }
                        catch (Exception ex) { Logger.Instance.LogError(ex); }
                    }

                    OnExecutionFinished();
                }
                catch (Exception ex) { Logger.Instance.LogError(ex); }
            }, cancellationToken: TokenSource.Token);
        }

        protected override void UpdateUnitState(EventArgs e)
        {
            Invoke(new Action(() =>
            {
                var stateArgs = e as WashMotorOperationStateChangedEventArgs;
                var labelText = $"{UnitType} activated.\nRotating {stateArgs.Direction}, {stateArgs.DirectionCycleRemaining}({stateArgs.MiniCyclesRemaining}).\nTime remaining: {stateArgs.TimeRemaining}";

                UnitNameText = labelText;
                UnitNameColor = (stateArgs.TimeRemaining.TotalSeconds % 2) == 1 ? Color.DarkGreen : Color.ForestGreen;
            }));
        }
        #endregion

        #region Handlers
        #endregion
    }
}
