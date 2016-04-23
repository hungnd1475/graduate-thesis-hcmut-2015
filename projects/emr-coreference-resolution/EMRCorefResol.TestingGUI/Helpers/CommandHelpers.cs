using Prism.Commands;
using Stateless;
using System;

namespace EMRCorefResol.TestingGUI
{
    static class CommandHelpers
    {
        public static DelegateCommand TriggerToCommand<TSTate, TTrigger>(
            this StateMachine<TSTate, TTrigger> stateMachine,
            TTrigger trigger)
        {
            return new DelegateCommand(() => stateMachine.Fire(trigger), () => stateMachine.CanFire(trigger));
        }

        public static DelegateCommand TriggerToCommand<TSTate, TTrigger, TParam>(
            this StateMachine<TSTate, TTrigger> stateMachine,
            StateMachine<TSTate, TTrigger>.TriggerWithParameters<TParam> twp, Func<TParam> getParameter)
        {
            return new DelegateCommand(() => stateMachine.Fire(twp, getParameter()), () => stateMachine.CanFire(twp.Trigger));
        }

        public static DelegateCommand<TParam> TriggerToCommand<TState, TTrigger, TParam>(
            this StateMachine<TState, TTrigger> stateMachine,
            StateMachine<TState, TTrigger>.TriggerWithParameters<TParam> twp)
        {
            return new DelegateCommand<TParam>(p => stateMachine.Fire(twp, p), _ => stateMachine.CanFire(twp.Trigger));
        }
    }
}
