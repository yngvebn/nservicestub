﻿using System.Collections.Generic;

namespace NServiceStub
{
    public class TriggeredMessageSequence : IStepConfigurableMessageSequence
    {
        private readonly StepChain _sequenceOfEvents = new StepChain();
        private readonly List<IStep> _currentSequenceExecutions = new List<IStep>();
        private readonly object _currentSequenceExecutionsLock = new object();

        public void ExecuteNextStep(SequenceExecutionContext executionContext)
        {
            List<IStep> currentSteps;
            lock (_currentSequenceExecutionsLock)
            {
                currentSteps = new List<IStep>(_currentSequenceExecutions);
            }

            foreach (var currentStep in currentSteps)
            {
                currentStep.Execute(executionContext);
            }

            lock(_currentSequenceExecutionsLock)
            {
                int index = 0;

                foreach (var currentStep in currentSteps)
                {
                    IStep next = _sequenceOfEvents.GetStepAfter(currentStep);

                    if (next != null)
                        _currentSequenceExecutions[index] = next;
                    else
                        _currentSequenceExecutions.Remove(currentStep);
                    index++;
                }
                
            }
        }

        public void TriggerNewSequenceOfEvents()
        {
            if (_sequenceOfEvents.Root == null)
                return;

            lock(_currentSequenceExecutionsLock)
            {
                _currentSequenceExecutions.Add(_sequenceOfEvents.Root);
            }
        }

        public bool Done { get; set; }

        public void ReplaceStep(IStep stepToReplace, IStep replacement)
        {
            _sequenceOfEvents.ReplaceStep(stepToReplace, replacement);
        }

        public void SetNextStep(IStep nextStep)
        {
            _sequenceOfEvents.SetNextStep(nextStep);
        }
    }
}