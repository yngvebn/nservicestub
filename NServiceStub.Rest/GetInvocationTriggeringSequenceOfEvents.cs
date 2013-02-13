﻿namespace NServiceStub.Rest
{
    public class GetInvocationTriggeringSequenceOfEvents : IInvocationMatcher
    {
        private readonly IRouteTemplate _routeOwningUrl;
        private readonly IInvocationMatcher _matcher;
        private readonly TriggeredMessageSequence _sequence;

        public GetInvocationTriggeringSequenceOfEvents(IRouteTemplate routeOwningUrl, IInvocationMatcher matcher, TriggeredMessageSequence sequence)
        {
            _routeOwningUrl = routeOwningUrl;
            _matcher = matcher;
            _sequence = sequence;
        }

        public bool Matches(RequestWrapper request)
        {
            if (_matcher.Matches(request))
            {
                _sequence.TriggerNewSequenceOfEvents(new CapturedGetInvocation(request.Request, _routeOwningUrl));
                return true;
            }
            else
                return false;
        }
    }
}